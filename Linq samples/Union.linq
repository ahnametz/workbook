<Query Kind="Expression">
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

//Union

//union in linq serves the same purpose as union in sql
//since linq is converted to sql
//   the rules in linq for union are the same as the
//	 the rules in sql

// number of columns are the same
// column datatype must much
// ordering should be done as a method after the last union
// literals values must appear in the first union (linq)

//List the stats (count, cost total, average track length) of Tracks
//   for Albums

//What is an Albums has no tracks?
//cost total & average track length will not be able to be calculate

//therefore 2 report queries need to be used
//a) report for albums with no tracks
//b) report for albums with at least one track

//HOWEVER, we wish only one final collection (report)

//Solution: create two reports that are format identically
//			and Union the reports together

//syntax of union
//   (report1).Union(report2)[.Union(reportn)]

(Albums
	.Where(x => x.Tracks.Count( ) == 0)
	.Select(x => new
	{
		title = x.Title,
		totalTracks = x.Tracks.Count( ),
		totalcost = 0.0m,   	//Sum returns a decimal
		averagelength = 0.0		//Average returns a double
	})
).Union(
	Albums
		.Where(x => x.Tracks.Count() > 0)
		.Select(x => new
		{
			title = x.Title,
			totalTracks = x.Tracks.Count(),
			totalcost = x.Tracks.Sum(t => t.UnitPrice),
			averagelength = x.Tracks.Select(t => t.Milliseconds).Average()   //Average returns a double
		})
)










