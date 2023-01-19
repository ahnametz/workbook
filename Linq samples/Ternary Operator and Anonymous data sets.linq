<Query Kind="Expression">
  <Connection>
    <ID>ed9cd8c6-cbb7-4f2e-a9c7-7320e07ddd41</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//Ternary Operator and Anonymous data sets (collections)

//  condition(s) ? true value : false value

//list all ablums by relase label. Any album with no label
//	should be flaged with the string Unknown
//list Title, Label and Artist Name
//Order by the label

//understand the problem
//  collection: albums
//	selective data: anonymous data set
//  label (nullable): either Unknown or label name
//  ordering: alphabetical on label field

//an anonymous data set is anything that is NOT a single complete raw instance
//an anonymous data set (collection) could be:
//	a) a partial list of fields from an instance
//  b) a combination of fields from multiple tables
//  c) a field that is derived from using navigational properties
Albums
	//.OrderBy(a => a.ReleaseLabel)
	.Select(a => new
				{
					Title = a.Title,
					Label = a.ReleaseLabel == null ? "Unknown" : a.ReleaseLabel,
					Artist = a.Artist.Name
				}
			)
	.OrderBy(a => a.Label)
	
//List all albums showing the Title, Artist Name, Year and Decade of release using
// oldies, 70s, 80s, 90s, or modern
// order by decade

//Hint: Can you have nested ternary operators?
Albums
	.Select(a => new
			{
				Title = a.Title,
				Artist = a.Artist.Name,
				Year = a.ReleaseYear,
				Decade = a.ReleaseYear < 1970 ? "Oldies" : 
				           	a.ReleaseYear < 1980 ? "70s" :
							a.ReleaseYear < 1990 ? "80s" :
							a.ReleaseYear < 2000 ? "90s" : "Modern"
			}
			)
	.OrderBy(a => a.Year )

	
	
	
	
	
	
	
	
	
	
	
	
	
	