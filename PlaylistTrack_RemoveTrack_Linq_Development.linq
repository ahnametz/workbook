<Query Kind="Program">
  <Connection>
    <ID>1e60c205-2a80-4713-80b8-ee23984ea5d1</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>Chinook</Database>
    <DisplayName>ChinookSystem</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
      <EFVersion>6.0.12</EFVersion>
      <TrustServerCertificate>True</TrustServerCertificate>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	
	try
	{
		//Driver
		List<PlayListTrackTRX> trackcollection = null;
		string playlist = "hansenb1";
		string username = "HansenB";
		//see the old data
		DisplayPlaylist(playlist, username );
		//need to create a good set of data
		
		//need to create a bad set of data
		
		//execute service to test for ALL possible errors that have been coded
		
		//testing for missing parameters
		//PlaylistTrack_RemoveTracks(playlist, username, trackcollection);
		
		//testing for empty collection, empty instance
		//PlaylistTrack_RemoveTracks(playlist, username, new List<PlayListTrackTRX>());
		
		//see the altered data
		DisplayPlaylist(playlist, username);
	}
	//for missing parameter values
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	//collection of errors
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	//general errors
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}

}

// You can define other methods, fields, classes and namespaces here

//CQRS data model
// no data model is required

//given routine to obtain the inner most exception on an error
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
	{
		ex = ex.InnerException;
	}
	return ex;
}

//creating test data collection
public class PlayListTrackTRX
{
	public bool SelectedTrack { get; set; }
	public int TrackId { get; set; }
	public int CurrentTrackNumber { get; set; }
	public int NewTrackNumber { get; set; }
}

//Display query for manipulate database data
// display the playlist and the playlisttracks

public void DisplayPlaylist(string playlistname, string username)
{
	IEnumerable info = Playlists
						.Where(p => p.Name.ToUpper().Equals(playlistname)
								 && p.UserName.ToUpper().Equals(username))
						.Select(p => new
						{
							PlaylistID = p.PlaylistId,
							Name = p.Name,
							theTracks = p.PlaylistTracks
										.Select(trk => trk)
										.OrderBy(trk => trk.TrackNumber)
										.AsEnumerable()
						}
						);
	info.Dump();
}

// TRX service method 
void PlaylistTrack_RemoveTracks(string playlist, string username, List<PlayListTrackTRX> trackcollection)
{
	List<Exception> errorlist = new List<Exception>();
	Tracks trackexist = null;
	Playlists playlistexist = null;
	int tracknumber = 0;
	PlaylistTracks playlisttrackexist = null;

	if (string.IsNullOrWhiteSpace(playlist))
	{
		throw new ArgumentNullException("No playlist name submitted");
	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("No user name submitted");
	}
	//is there a List<T> instance
	if(trackcollection == null)
	{
		throw new ArgumentNullException("No tracks supplied for processing");

	}
	//does the List<T> instance have any items
	if(trackcollection.Count() == 0)
	{
		errorlist.Add(new Exception("You record list is empty. Nothing to remove."));
	}
	else
	{
		//you have tracks to process
		//What if no tracks were selected to remove?
		//What if the playlist did not exists?
		playlistexist = Playlists
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
			playlisttrackexist = PlaylistTracks
								 .FirstOrDefault(x => x.Playlist.Name.ToUpper().Equals(playlist.ToUpper())
										  && x.Playlist.UserName.ToUpper().Equals(username.ToUpper())
										  && x.TrackId == item.TrackId);
			if (playlisttrackexist !=null)
			{
				PlaylistTracks.Remove(playlisttrackexist);
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
		foreach(PlayListTrackTRX item in keeplist)
		{
			//does the track exists on the database for this playlist
			playlisttrackexist = PlaylistTracks
								 .FirstOrDefault(x => x.Playlist.Name.ToUpper().Equals(playlist.ToUpper())
										  && x.Playlist.UserName.ToUpper().Equals(username.ToUpper())
										  && x.TrackId == item.TrackId);
			if (playlisttrackexist != null)
			{
				playlisttrackexist.TrackNumber = tracknumber;
				PlaylistTracks.Update(playlisttrackexist);
				tracknumber++;
			}
			else
			{
				//retrieve the songname from the database to use in my error message
				var songname = Tracks
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
		SaveChanges();
	}
}






