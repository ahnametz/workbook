<Query Kind="Statements">
  <Connection>
    <ID>ed9cd8c6-cbb7-4f2e-a9c7-7320e07ddd41</ID>
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

//Sorting

//there is a significant difference between query syntax
//	and method syntax

//query syntax is much like sql
// sql order by attribute [ascending,decending], attribute, ...
// linq orderby attribute [ascending,decending], attribute, ...

//method syntax
//method syntax has multiple methods for sorting
// OrderBy(x => x.property)
// OrderByDescending(x => x.Property)
// multiple properties to sort on
// ThenBy(x => x.property)
// ThenByDescending(x => x.property)

//within method syntax EACH property is sorted within it's own
//	method

//Find all of the album tracks for the band Queen.
// Order the tracks by the track name within composer (descedning)
// within album title alphabetically

//investigate the use of the navigational properties in the
//	class schema structure
//the navigational properties reflect your relationship schema
//	between table (foreign key relationships)


// the order of sort and filter does not matter
Tracks
	//.Where (x => x.Album.Artist.Name.Contains("Queen"))
	.OrderBy(x => x.Album.Title)
	.ThenByDescending(x => x.Composer)
	.ThenBy(x => x.Name)
	.Where (x => x.Album.Artist.Name.Contains("Queen"))
	.Dump();