<Query Kind="Program">
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

void Main()
{
	//list tracks that match a given partial song name.
	//show the album title, song name and artist name.
	//order by song name.
	
	//this is similar to a console application program
	//therefore you will use C# statements
	
	//imagine this is a OnPost method on your web page
	string inputValueFromWebForm = "sing";
	List<SongItem> songCollection = SongsByPartialName(inputValueFromWebForm);
	
	songCollection.Dump();
}

// You can define other methods, fields, classes and 
//namespaces here

//C# really enjoyes strongly type data fields
//	whether these fields are primitive data types (int, double, ...)
//	or developer defined datatype (class)

public class SongItem
{
	public string Title {get;set;}
	public string Song {get;set;}
	public string Artist {get;set;}
}

//imagine the following mehtod exists in a service in your BLL
//this method receives the web page argument value for the query
//this method will need to return a collection

List<SongItem> SongsByPartialName(string partialSongName)
{
	//var variables are defined (receives their datatype)
	//		at execution time
	//strongly type variables are check at compile time
	IEnumerable<SongItem> songCollection = Tracks
						.Where(trk => trk.Name.Contains(partialSongName))
						.OrderBy(trk => trk.Name)
						.Select(trk => new SongItem
						{
							Title = trk.Album.Title,
							Song = trk.Name,
							Artist = trk.Album.Artist.Name
						});
	return songCollection.ToList();
}







