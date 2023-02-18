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
	List<PlaylistInfo> info = null;
	string playlistname = "hansenb1";
	string username = "hansenb";
	try
	{
		info = PlaylistTrack_FetchPlaylist(playlistname, username);
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error: {ex.Message}");
	}
	info.Dump();
}

// You can define other methods, fields, classes and namespaces here

public class PlaylistInfo
{
	public int TrackId { get; set; }
	public int TrackNumber { get; set; }
	public string SongName { get; set; }
	public int Milliseconds { get; set; }
}

List<PlaylistInfo> PlaylistTrack_FetchPlaylist(string playlistname, string username)
{
	IEnumerable<PlaylistInfo> info = null;
	info = PlaylistTracks
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