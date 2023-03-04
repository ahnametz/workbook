<Query Kind="Program">
  <Connection>
    <ID>1e60c205-2a80-4713-80b8-ee23984ea5d1</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
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
		string playlist = "hansenb400";
		string username = "HansenB";
		//see the old data
		DisplayPlaylist(playlist, username );
		//need to create a good set of data
		//trackcollection = CreateGoodData();
		//need to create a bad set of data
		//trackcollection = CreateBadData();
		
		//execute service to test for ALL possible errors that have been coded
		//PlaylistTrack_ReOrgTracks(playlist, username, trackcollection);
		
		//testing for missing parameters
		//PlaylistTrack_ReOrgTracks(playlist, username, trackcollection);
		
		//testing for empty collection, empty instance
		//PlaylistTrack_ReOrgTracks(playlist, username, new List<PlayListTrackTRX>());
		
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
public List<PlayListTrackTRX> CreateGoodData()
{
	List<PlayListTrackTRX> info = new List<UserQuery.PlayListTrackTRX>();
	PlayListTrackTRX row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 54,
		CurrentTrackNumber= 1,
		NewTrackNumber = 0};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 122,
		CurrentTrackNumber = 2,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = true,
		TrackId = 1,
		CurrentTrackNumber = 3,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = true,
		TrackId = 12,
		CurrentTrackNumber = 4,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 11,
		CurrentTrackNumber = 5,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = true,
		TrackId = 200,
		CurrentTrackNumber = 6,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 111,
		CurrentTrackNumber = 7,
		NewTrackNumber = 0
	};
	info.Add(row);
	return info;
}

public List<PlayListTrackTRX> CreateBadData()
{
	List<PlayListTrackTRX> info = new List<UserQuery.PlayListTrackTRX>();
	PlayListTrackTRX row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 54,
		CurrentTrackNumber = 1,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 122,
		CurrentTrackNumber = 2,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = true,
		TrackId = 1,
		CurrentTrackNumber = 3,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = true,
		TrackId = 12,
		CurrentTrackNumber = 4,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 110,
		CurrentTrackNumber = 5,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 200,
		CurrentTrackNumber = 6,
		NewTrackNumber = 0
	};
	info.Add(row);
	row = new PlayListTrackTRX()
	{
		SelectedTrack = false,
		TrackId = 111,
		CurrentTrackNumber = 7,
		NewTrackNumber = 0
	};
	info.Add(row);
	return info;
}
// CQRS class
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
void PlaylistTrack_ReOrgTracks(string playlist, string username, List<PlayListTrackTRX> trackcollection)
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
		errorlist.Add(new Exception("You record list is empty. Nothing to resequence."));
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
		
		//sort trackcollection by the reorganize numbers
		trackcollection.Sort((x,y) => x.NewTrackNumber.CompareTo(y.NewTrackNumber));

		//check that all NewTrackNumbers are non-zero positive values
		//
		foreach (PlayListTrackTRX item in trackcollection)
		{
			if (item.NewTrackNumber < 1)
			{
				//retrieve the songname from the database to use in my error message
				var songname = Tracks
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
			var songname1 = Tracks
								.Where(x => x.TrackId == trackcollection[i].TrackId)
								.Select(x => x.Name)
								.SingleOrDefault();
			var songname2 = Tracks
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
		foreach(PlayListTrackTRX item in trackcollection)
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






