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

//Aggregates
// .Count() counts the number of instances in the collection
// .Sum(x => expression) sums (totaling) a numeric field (numeric expression) in a collection
// .Min(x => expression) finds the minimum value of a collection for a field (expression)
// .Max(x => expression) finds the maximum value of a collection for a field (expression)
// .Average(x => expression) finds the average value of a numeric field (numeric espression) in a collection

//IMPORTANT!!!!
//Aggregates work ONLY on a collection of values
//The collection can be empty for .Count BUT should have a minimum of one instance for the other
//		aggregates
//Aggregates DO NOT work on a single instance (not in a declared collection) 
//Note Sum and Average MUST work on numeric fields and the field CANNOT be null

//syntax: method
//collectionset.aggregate(x => expression)
//collectionset.Select(....).aggregate()

//for Sum, Min, Max and Average: the result is a single value

//you can use multiple aggrefates on a single column
//   .Sum(x => expression).Min(x => expression)

//Find the average playing time (length) of tracks in our music collection

//thought process
//average is an aggregate
//what is the collection? The Tracks table is a collection
//what is the expression? Milliseconds

Tracks
	.Average(t => t.Milliseconds) //the expression is indicating which field

Tracks
	.Select(t => t.Milliseconds) //the select is creating a collection with a single value on each instance
	.Average() //the average does not need an expression for the Select collection

Tracks.Average() //aborts because no specific field was referred on the Track instance

//List all albums of the 60s showing the title artist and various
// aggregates for albums containing tracks

//for each album show the number of tracks, the total price of all tracks and
//  the average playing length of the album tracks

//thought process
//where to begin? Albums
//can I get the artist name? .Artist.Name
//can I get a collection of tracks for an album? .Tracks
//what aggregates will be needed for the collection? .Count; .Sum(UnitPrice); Average(Milliseconds)
//Is there a filter on the query? yes year in 60s and album needs a track
//Is there any ordering? no

Albums
	.Where(x => x.Tracks.Count() > 0
	 		&& (x.ReleaseYear > 1959 && x.ReleaseYear < 1970))
	//.OrderBy(x => x.Title)
	.Select(a => new
		{
			Title = a.Title,
			Artist = a.Artist.Name,
			NumberofTracks = a.Tracks.Count(),
			TotalPrice = a.Tracks.Sum(tr => tr.UnitPrice), // aggregate is looking a multiple fields
			AveragePlayLength = a.Tracks.Select(tr => tr.Milliseconds).Average() //aggregate is looking at a single field
		})















