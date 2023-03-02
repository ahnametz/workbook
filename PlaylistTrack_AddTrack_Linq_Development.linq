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
		string playlistname, username;
		int trackid;
		//setup test data
		//good data
		//playlistname="hansenb1";
		//username="HansenB";
		//trackid = 12;

		//good data new playlist
		playlistname = "hansenb111";
		username = "HansenB";
		trackid = 12;
		
		//bad data (test validation)
		//playlistname = "";
		//username = "";
		//trackid = -1; //pkey cannot be negative

		//display the original playlist and tracks BEFORE adding
		DisplayPlaylist(playlistname, username);


		//call the BLL service method for processing: AddTrack
		PlaylistTrack_AddTrack(playlistname, username, trackid);

		//display the modified playlist and tracks AFTER adding
		DisplayPlaylist(playlistname, username);

	}
	//for missing parameter values
	catch(ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	//collection of errors
	catch(AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	//general errors
	catch(Exception ex)
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
// no data collection, individual parameter values

//Display query for manipulate database data
// display the playlist and the playlisttracks

public void DisplayPlaylist(string playlistname, string username)
{
	IEnumerable info = Playlists
						.Where(p => p.Name.ToUpper().Equals(playlistname)
						         && p.UserName.ToUpper().Equals(username) )
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
	Tracks trackexist = null;
	Playlists playlistexist = null;
	int tracknumber = 0;
	PlaylistTracks playlisttrackexist = null;
	
	if (string.IsNullOrWhiteSpace(playlistname))
	{
		throw new ArgumentNullException("No playlist name submitted");
	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("No user name submitted");
	}
	trackexist = Tracks
					.FirstOrDefault(trk => trk.TrackId == trackid);
	//null the track is not on the database, not null the track is on the database
	if (trackexist == null)
	{
		errorlist.Add(new Exception($"Track {trackid} id does not exist on database"));
	}
	else
	{
		playlistexist = Playlists
							.FirstOrDefault(p => p.Name.ToUpper().Equals(playlistname)
							                  && p.UserName.ToUpper().Equals(username));
		if (playlistexist == null)
		{
			//this will be a new playlist
			playlistexist = new Playlists()
							{
								Name = playlistname,
								UserName = username
							};
			//STAGE the creation of a new playlist
			Playlists.Add(playlistexist);
			tracknumber = 1;
		}
		else
		{
			//append to an existing playlist
			//business rule that a track can only exist once on the playlist tracks
			playlisttrackexist = PlaylistTracks
							.FirstOrDefault(p => p.Playlist.Name.ToUpper().Equals(playlistname)
											  && p.Playlist.UserName.ToUpper().Equals(username)
											  && p.TrackId == trackid);
			if (playlisttrackexist == null)
			{
				tracknumber = PlaylistTracks
							.Where(p => p.Playlist.Name.ToUpper().Equals(playlistname)
											  && p.Playlist.UserName.ToUpper().Equals(username))
							.Count( );
				tracknumber++;		  


			}
			else
			{
				errorlist.Add(new Exception($"Select track {trackid} already on play list."));
			}
			
		}
		//Add the track to the playlist
		playlisttrackexist = new PlaylistTracks();
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
	if(errorlist.Count( ) > 0)
	{
		//throw the collection of errors 
		throw new AggregateException("Processing concerns, please review",errorlist);
		//drop out of method NO SAVECHANGES!!!!!!
	}
	else
	{
		//Send all STAGED processing to the database to actual manipulation
		//	of the database data
		SaveChanges();
	}
}






