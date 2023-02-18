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
	//pretend that this console application is your web applicaton page

	//pretend that info is a property on the PageModel web page
	 List<TrackSelection> info  = null;

	//BindProperty()
	 string searcharg;
	//BindProperty()
	 string searchtype;

	//pretend that this console application is the OnPost method
	// of your web applicaton page
	//pretend that the user has enter appropriate values on the
	//	web page and pressed the submit for the search
	//since the two fields searcharg and searchtype would have been
	//	created under BindProperty as properties; the system would
	//	have filled the fields with data
	
	searcharg = "deep";
	searchtype = "Artist";

	try
	{

		info = Track_FetchBy(searcharg, searchtype);
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error: {ex.Message}");
	}
	//pretend that the .Dump() is the table control on the web form
	info.Dump();
}

// You can define other methods, fields, classes and namespaces here

//pretend that this class definition is in the class library project
public class TrackSelection
{
	public int TrackId { get; set; }
	public string SongName { get; set; }
	public string AlbumTitle { get; set; }
	public string ArtistName { get; set; }
	public int Milliseconds { get; set; }
	public decimal Price{get; set;}
}

//pretend that this method is actually inside the service class in the
//	class library
List<TrackSelection> Track_FetchBy(string searcharg, string searchtype)
{
	IEnumerable<TrackSelection> info = null;
	info = Tracks
			.Where(x => (searchtype.Equals("Album") && x.Album.Title.Contains(searcharg))
			         ||
					    (searchtype.Equals("Artist") && x.Album.Artist.Name.Contains(searcharg))) 
			.Select(x => new TrackSelection
			{
				TrackId = x.TrackId,
				SongName = x.Name,
				AlbumTitle = x.Album.Title,
				ArtistName = x.Album.Artist.Name,
				Milliseconds = x.Milliseconds,
				Price = x.UnitPrice
			})
			.OrderBy(x => x.SongName );
	return info.ToList();
}













