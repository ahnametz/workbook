<Query Kind="Statements">
  <Connection>
    <ID>e68d5372-6d65-4d1b-8aa4-33be4d83764c</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//.First vs .FirstOrDefault
//preference is FirstOrDefault
//it a void the exception error if no result is found
//if no result is found the receiving variable becomes null

string artistparam = "Deep Purple";
var resultsFOD = Albums
				.Where(a => a.Artist.Name.Equals(artistparam))
				.Select(a => a)
				.OrderBy(a => a.ReleaseYear)
				.FirstOrDefault()
				//.Dump()
				;


//.Single vs .SingleOrDefault
//works like FirstOrDefault
//Single expects ONLY on item to be returned
//great for quering pkey items
int albumid = 1;
var resultsSOD = Albums
				.Where(a => a.AlbumId == albumid)
				.Select(a => a)
				.SingleOrDefault()
				//Dump()
				;
//if (resultsSOD != null)
//	resultsSOD.Dump();
//else
//	"No ablums found for query".Dump();
	
//Any and ALL

//Genres.Dump();

//Show Genres that have tracks which are NOT on any playlist

//Do we have a Genres that no one is listening to?
Genres
	.Where(g => g.Tracks.Any(tr => tr.PlaylistTracks.Count() == 0))
	//.Dump()
	;
	
//show genres that have all their tracks appearing at least once
// on a playlist
Genres
	.Where(g => g.Tracks.All(tr => tr.PlaylistTracks.Count() > 0))
	//.Dump()
	;

//there maybe times that desiring a !Any is done using a All(!relationship)
//there maybe times that desiring a !All is done using a Any(!relationship)

//a common use of the All and Any is in comparing 2 complex collections
//IF your collection is NOT a complex record such as list of integers or strings
//		there is a Linq method called .Except that can be used to solve your query

//https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.except?view=net-6.0
//https://dotnettutorials.net/lesson/linq-except-method

//Compare the track collection of 2 people using All and Any
//create a small anonymous collectio for two people
//  Roberto Almeida (AlmeidaR) and Michelle Brooks (BrooksM)

var almeidar = PlaylistTracks
				.Where(x => x.Playlist.UserName.Equals("AlmeidaR"))
				.Select(x => new
				{
					song = x.Track.Name,
					genre = x.Track.Genre.Name,
					id = x.TrackId,
					artist = x.Track.Album.Artist.Name
				})
				.Distinct()
				.OrderBy(x => x.song)
				//.Dump()
				; //110 unique playlists tracks

var brooksm = PlaylistTracks
				.Where(x => x.Playlist.UserName.Equals("BrooksM"))
				.Select(x => new
				{
					song = x.Track.Name,
					genre = x.Track.Genre.Name,
					id = x.TrackId,
					artist = x.Track.Album.Artist.Name
				})
				.Distinct()
				.OrderBy(x => x.song)
				//.Dump()
				; //88 unique playlists tracks
				
//List the tracks that BOTH Roberto and Michelle like

// compare 2 collections together (ListA and ListB)
// assume ListA (Roberto) and ListB (Michelle)
// ListA is the collection you wish to report on
// ListB is what you wish to compare ListA to (no reporting)

almeidar
	.Where(listarec => brooksm.Any(listbrec => listbrec.id == listarec.id))
	.OrderBy(listarec => listarec.song)
	//.Dump()
	;

brooksm
	.Where(listarec => almeidar.Any(listbrec => listbrec.id == listarec.id))
	.OrderBy(listarec => listarec.song)
	//.Dump()
	;

//What songs does Roberto like but not Michelle
almeidar
	.Where(listarec => brooksm.All(listbrec => listbrec.id != listarec.id))
	.OrderBy(listarec => listarec.song)
	//.Dump()
	;


//What songs does Michelle like but not roberto
brooksm
	.Where(listarec => almeidar.All(listbrec => listbrec.id != listarec.id))
	.OrderBy(listarec => listarec.song)
	.Dump()
	;






