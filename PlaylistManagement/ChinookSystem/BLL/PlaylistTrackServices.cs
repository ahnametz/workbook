using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.Models;
#endregion

namespace ChinookSystem.BLL
{
    public class PlaylistTrackServices
    {
        #region Constructor for Context Dependency
        private readonly ChinookContext _context;
        internal PlaylistTrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        List<PlaylistInfo> PlaylistTrack_FetchPlaylist(string playlistname, string username)
        {
            IEnumerable<PlaylistInfo> info = null;
            info = _context.PlaylistTracks
                    .Where(x => x.Playlist.Name.Equals(playlistname) &&
                                x.Playlist.UserName.Equals(username))
                    .OrderBy(x => x.TrackNumber)
                    .Select(x => new PlaylistInfo
                    {
                        TrackId = x.TrackId,
                        TrackNumber = x.TrackNumber,
                        SongName = x.Track.Name,
                        Milliseconds = x.Track.Milliseconds
                    });
            return info.ToList();
        }
        #endregion

        #region Service TRXs
        void PlaylistTrack_AddTrack(string playlistname, string username, int trackid)
        {
            //this collection will hold all the errors found while processing the
            //	service method
            //why?
            //  if there are posssibly multiple errors in the processing, it would
            //	be nice to know all of the problem at once, instead of one error at
            //	a time (must I execute the process to possible see that I have 10
            //	errors, run it 10 times)

            List<Exception> errorlist = new List<Exception>();
            Track trackexist = null;
            Playlist playlistexist = null;
            int tracknumber = 0;
            PlaylistTrack playlisttrackexist = null;

            if (string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }
            trackexist = _context.Tracks
                            .FirstOrDefault(trk => trk.TrackId == trackid);
            //null the track is not on the database, not null the track is on the database
            if (trackexist == null)
            {
                errorlist.Add(new Exception($"Track {trackid} id does not exist on database"));
            }
            else
            {
                playlistexist = _context.Playlists
                                    .FirstOrDefault(p => p.Name.ToUpper().Equals(playlistname)
                                                      && p.UserName.ToUpper().Equals(username));
                if (playlistexist == null)
                {
                    //this will be a new playlist
                    playlistexist = new Playlist()
                    {
                        Name = playlistname,
                        UserName = username
                    };
                    //STAGE the creation of a new playlist
                    _context.Playlists.Add(playlistexist);
                    tracknumber = 1;
                }
                else
                {
                    //append to an existing playlist
                    //business rule that a track can only exist once on the playlist tracks
                    playlisttrackexist = _context.PlaylistTracks
                                    .FirstOrDefault(p => p.Playlist.Name.ToUpper().Equals(playlistname)
                                                      && p.Playlist.UserName.ToUpper().Equals(username)
                                                      && p.TrackId == trackid);
                    if (playlisttrackexist == null)
                    {
                        tracknumber = _context.PlaylistTracks
                                    .Where(p => p.Playlist.Name.ToUpper().Equals(playlistname)
                                                      && p.Playlist.UserName.ToUpper().Equals(username))
                                    .Count();
                        tracknumber++;


                    }
                    else
                    {
                        errorlist.Add(new Exception($"Select track {trackid} already on play list."));
                    }

                }
                //Add the track to the playlist
                playlisttrackexist = new PlaylistTrack();
                playlisttrackexist.TrackId = trackid;
                playlisttrackexist.TrackNumber = tracknumber;

                //setup the PlaylistId pkey component
                //PROBLEM!!!!!!!!
                //what if this is a NEW playlist?
                //
                //in our situation where there is a NEW playlist, we do not know
                //		the pkey value to that playlist AND it is needed to stage
                //		the adding of the playlist track.

                //the new playlist record is only staged at this moment and NOT
                //		on the database.
                //Once the playlist record is passed to the database via the SaveChanges
                //		the database will create the identity pkey value
                //
                //However the SaveChanges for this transaction NOT CAN be executed UNTIL
                //		all records for the transaction have been staged
                //We cannot stage the adding of the playlist track UNTIL we have the 
                //		playlist identity pkey

                //this seems like a "Catch-22" scenario (aka which comes first chicken or the egg)

                //Solution

                // it is built into EntityFramework software and is based on using the
                //		navigational property in Playlist pointing to its "child"

                //Staging a typical Add in the past was to reference the entity
                //	and us the syntax  entity.Add(xxxxx)
                //  thus we assume on could use PlaylistTracks.Add(xxxxx)
                //IF you use this statement, the playlistid would be zero (0)
                //	resulting in an abort

                //INSTEAD. 
                // do the staging using the syntax of
                //		parentinstance.navigationalproperty.Add(xxxxx)
                //
                // parentinstance here will be either
                //		a) the NEW staged playlist instance
                //   OR b) the existing playlist instance

                //EntityFrame will process the stage records in the correct order to
                //  a) create the new Playlist record (if one was staged)
                //  b) if the playlisttrack record is missing the PlaylistID, EntityFramework
                //		will place the new identity pkey into the correct place on
                //		the playlisttrack record
                //OR
                //  it simply uses the existing playlist pkey for the playlisttrack record

                playlistexist.PlaylistTracks.Add(playlisttrackexist);


            }
            //At this point, NO data has yet to be sent to the database
            //All required transactional records SHOULD have be stage at this point
            //Decide whether to SaveChanges OR throw the error collect
            if (errorlist.Count() > 0)
            {
                //throw the collection of errors 
                throw new AggregateException("Processing concerns, please review", errorlist);
                //drop out of method NO SAVECHANGES!!!!!!
            }
            else
            {
                //Send all STAGED processing to the database to actual manipulation
                //	of the database data
                _context.SaveChanges();
            }
        }

        void PlaylistTrack_ReOrgTracks(string playlist, string username, List<PlayListTrackTRX> trackcollection)
        {
            List<Exception> errorlist = new List<Exception>();
            Track trackexist = null;
            Playlist playlistexist = null;
            int tracknumber = 0;
            PlaylistTrack playlisttrackexist = null;

            if (string.IsNullOrWhiteSpace(playlist))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }
            //is there a List<T> instance
            if (trackcollection == null)
            {
                throw new ArgumentNullException("No tracks supplied for processing");

            }
            //does the List<T> instance have any items
            if (trackcollection.Count() == 0)
            {
                errorlist.Add(new Exception("You record list is empty. Nothing to resequence."));
            }
            else
            {
                //you have tracks to process
                //What if no tracks were selected to remove?
                //What if the playlist did not exists?
                playlistexist = _context.Playlists
                                .FirstOrDefault(x => x.Name.ToUpper().Equals(playlist.ToUpper())
                                                  && x.UserName.ToUpper().Equals(username.ToUpper()));
                if (playlistexist == null)
                {
                    errorlist.Add(new Exception($"Your playlist {playlist} does not exist on file."));

                }

                //sort trackcollection by the reorganize numbers
                trackcollection.Sort((x, y) => x.NewTrackNumber.CompareTo(y.NewTrackNumber));

                //check that all NewTrackNumbers are non-zero positive values
                //
                foreach (PlayListTrackTRX item in trackcollection)
                {
                    if (item.NewTrackNumber < 1)
                    {
                        //retrieve the songname from the database to use in my error message
                        var songname = _context.Tracks
                                        .Where(x => x.TrackId == item.TrackId)
                                        .Select(x => x.Name)
                                        .SingleOrDefault();
                        errorlist.Add(new Exception($"The track {songname} has an invalid resequence number {item.NewTrackNumber}. Try again."));
                    }
                }
                //test for duplicate reorganize numbers
                //    1,2,3,3,3,7,120,12345
                //List<T> can be referenced in the same fashion as an array, with an index
                for (int i = 0; i < trackcollection.Count() - 1; i++)
                {
                    var songname1 = _context.Tracks
                                        .Where(x => x.TrackId == trackcollection[i].TrackId)
                                        .Select(x => x.Name)
                                        .SingleOrDefault();
                    var songname2 = _context.Tracks
                                        .Where(x => x.TrackId == trackcollection[i + 1].TrackId)
                                        .Select(x => x.Name)
                                        .SingleOrDefault();
                    if (trackcollection[i].NewTrackNumber == trackcollection[i + 1].NewTrackNumber)
                    {
                        errorlist.Add(new Exception($"{songname1} and {songname2} have the same resequence number {trackcollection[i].NewTrackNumber}. Try again."));
                    }
                }

                //resequence, regardless of the current track numbers, all tracks from 1 to keeplist.count
                tracknumber = 1;
                foreach (PlayListTrackTRX item in trackcollection)
                {
                    //does the track exists on the database for this playlist
                    playlisttrackexist = _context.PlaylistTracks
                                         .FirstOrDefault(x => x.Playlist.Name.ToUpper().Equals(playlist.ToUpper())
                                                  && x.Playlist.UserName.ToUpper().Equals(username.ToUpper())
                                                  && x.TrackId == item.TrackId);
                    if (playlisttrackexist != null)
                    {
                        playlisttrackexist.TrackNumber = tracknumber;
                        _context.PlaylistTracks.Update(playlisttrackexist);
                        tracknumber++;
                    }
                    else
                    {
                        //retrieve the songname from the database to use in my error message
                        var songname = _context.Tracks
                                        .Where(x => x.TrackId == item.TrackId)
                                        .Select(x => x.Name)
                                        .SingleOrDefault();
                        errorlist.Add(new Exception($"The track {songname} is no longer on your play list. Refresh your play list."));
                    }
                }
            }



            if (errorlist.Count() > 0)
            {
                //throw the collection of errors 
                throw new AggregateException("Processing concerns, please review", errorlist);
                //drop out of method NO SAVECHANGES!!!!!!
            }
            else
            {
                //Send all STAGED processing to the database to actual manipulation
                //	of the database data
                _context.SaveChanges();
            }
        }

        void PlaylistTrack_RemoveTracks(string playlist, string username, List<PlayListTrackTRX> trackcollection)
        {
            List<Exception> errorlist = new List<Exception>();
            Track trackexist = null;
            Playlist playlistexist = null;
            int tracknumber = 0;
            PlaylistTrack playlisttrackexist = null;

            if (string.IsNullOrWhiteSpace(playlist))
            {
                throw new ArgumentNullException("No playlist name submitted");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("No user name submitted");
            }
            //is there a List<T> instance
            if (trackcollection == null)
            {
                throw new ArgumentNullException("No tracks supplied for processing");

            }
            //does the List<T> instance have any items
            if (trackcollection.Count() == 0)
            {
                errorlist.Add(new Exception("You record list is empty. Nothing to remove."));
            }
            else
            {
                //you have tracks to process
                //What if no tracks were selected to remove?
                //What if the playlist did not exists?
                playlistexist = _context.Playlists
                                .FirstOrDefault(x => x.Name.ToUpper().Equals(playlist.ToUpper())
                                                  && x.UserName.ToUpper().Equals(username.ToUpper()));
                if (playlistexist == null)
                {
                    errorlist.Add(new Exception($"Your playlist {playlist} does not exist on file."));

                }
                int anyselectedtrack = trackcollection
                                        .Where(x => x.SelectedTrack)
                                        .Count();
                if (anyselectedtrack == 0)
                {
                    errorlist.Add(new Exception($"You did not select any track to remove from play list {playlist}."));
                }

                //create a collection from the trackcollection of the tracks to keep AND order by the
                //		current track number for re-sequencing later
                IEnumerable<PlayListTrackTRX> keeplist = trackcollection
                                                            .Where(t => !t.SelectedTrack)
                                                            .Select(t => t)
                                                            .OrderBy(t => t.CurrentTrackNumber);
                //remove the unwanted tracks
                IEnumerable<PlayListTrackTRX> removelist = trackcollection
                                                            .Where(t => t.SelectedTrack)
                                                            .Select(t => t);

                //process the tracks to remove (Stage a .Remove(xxxx) on PlaylistTracks)
                //whats need?
                //   I need the instance of the table (via EF) to stage the Remove
                foreach (PlayListTrackTRX item in removelist)
                {
                    playlisttrackexist = _context.PlaylistTracks
                                         .FirstOrDefault(x => x.Playlist.Name.ToUpper().Equals(playlist.ToUpper())
                                                  && x.Playlist.UserName.ToUpper().Equals(username.ToUpper())
                                                  && x.TrackId == item.TrackId);
                    if (playlisttrackexist !=null)
                    {
                        _context.PlaylistTracks.Remove(playlisttrackexist);
                    }
                    else
                    {
                        //what if it does not exist?
                        //what if someone has already removed it?
                        //Do we care?
                        //maybe since it was to be remove anyways: why worry?
                    }
                }

                //handle the keep list
                //resequence, regardless of the current track numbers, all tracks from 1 to keeplist.count
                tracknumber = 1;
                foreach (PlayListTrackTRX item in keeplist)
                {
                    //does the track exists on the database for this playlist
                    playlisttrackexist = _context.PlaylistTracks
                                         .FirstOrDefault(x => x.Playlist.Name.ToUpper().Equals(playlist.ToUpper())
                                                  && x.Playlist.UserName.ToUpper().Equals(username.ToUpper())
                                                  && x.TrackId == item.TrackId);
                    if (playlisttrackexist != null)
                    {
                        playlisttrackexist.TrackNumber = tracknumber;
                        _context.PlaylistTracks.Update(playlisttrackexist);
                        tracknumber++;
                    }
                    else
                    {
                        //retrieve the songname from the database to use in my error message
                        var songname = _context.Tracks
                                        .Where(x => x.TrackId == item.TrackId)
                                        .Select(x => x.Name)
                                        .SingleOrDefault();
                        errorlist.Add(new Exception($"The track {songname} is no longer on your play list. Refresh your play list."));
                    }
                }
            }



            if (errorlist.Count() > 0)
            {
                //throw the collection of errors 
                throw new AggregateException("Processing concerns, please review", errorlist);
                //drop out of method NO SAVECHANGES!!!!!!
            }
            else
            {
                //Send all STAGED processing to the database to actual manipulation
                //	of the database data
                _context.SaveChanges();
            }
        }

        #endregion
    }
}
