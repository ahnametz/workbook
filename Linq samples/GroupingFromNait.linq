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

//Grouping

//when you create a group it builds two (2) components
//	a) Key component (group by criteria values)
//		reference this component using the groupname.Key[.Property]
//  (property < - > column < - > field < - > attribute < - > value)
//	b) the data of the group (instances of the original collection)

//ways to group
//a) by a single property (column,field, attribute,value)	groupname.Key
//b) by a set of properties (anonymous key set)				groupname.Key.PropertyName
//c) by using an entity (x.navproperty) ** try to avoid **	groupname.Key.PropertyName

//concept processing
//start with a "pile" of data (original collection)
//specify the grouping criteria (value(s))
//result of the group operation will be to "place the data into smaller piles"
//	the piles are dependant on the grouping criteria value(s)
//	the grouping criteria (property (ies)) become the Key
//	the individual instances are the data in the smaller piles
//	the entire individual instance of the original collection is placed in the
//		smaller pile
//Manipulation of each of the "smaller piles" is now possible with your linq commands

//grouping is different then Ordering
//Ordering is the re-sequencing of a collection for display
//Grouping re-organizes a collection into separate, usually smaller collections
//	for processing

//Point to remember about "when to use" grouping
//Will usually find grouping done on properties that are NOT a foreign key
//	thus will NOT have a natural navigational property to identify the 
//	collection

//Display albums by ReleaseYear
//	this request does NOT need grouping
//  this request is a re-sequence (ordering) of output (OrderBy)
//  this affects display only
Albums
	.OrderBy(a => a.ReleaseYear)

//Display albums grouped by ReleaseYear
//	NOT one display of albums BUT displays of album in smaller collections
//		based of the specific value of ReleaseYear
Albums
	.GroupBy(a => a.ReleaseYear)

//query syntax
from a in Albums
group a by a.ReleaseYear


// add some processing to the group
// do a simple count of the records in each group

Albums
	.GroupBy(a => a.ReleaseYear) //the results of the methods is serveral small piles of data
	.Select(aGroupPile => new
		{
			Year = aGroupPile.Key,
			NumberofAlbumsInGroupPile = aGroupPile.Count( )
		}) //process each group pile as if it were a mini-collection
		   //each mini-collection is processed one at a time to produce the final report line

//query syntax
from a in Albums
group a by a.ReleaseYear into aGroupPile //specific the placeholder name on the group by clause
select new
{
	Year = aGroupPile.Key,
	NumberofAlbumsInGroupPile = aGroupPile.Count()
}

//once you have grouped ALL FURTHER processing is done against the group

//use of multiple sets of criteria (properties) to form the group

//Display albums grouped by ReleaseLabel the ReleaseYear
//Display the release year and the number of albums.
//List only the years with 2 or more albums.
//For each album display the title, year of release and count of tracks

//thought process
//original collection: Albums
//filtering? problem CANNOT filter directly on the album
//			 I need to gather the album for a particular year to determine the number of albums for
//					that year
// I will need to group
//grouping: specifies ReleaseLabel then ReleaseYear
// Once group, one can count the number of raw instances placed in each group:  group.Count >=2
//report
//   for each report line
//     year, number of albums, nest report for each album in the group (title, year, track count)


Albums
	.GroupBy(a => new { a.ReleaseLabel, a.ReleaseYear} ) //creating an anonymous key set
	.Where(aGroupPile => aGroupPile.Count( ) >=2)
	.OrderBy(aGroupPile => aGroupPile.Key.ReleaseLabel) //if you have multiple key value, you MUST
														//  specifically indicate the property of the key
	.Select(aGroupPile => new
		{
			Label = aGroupPile.Key.ReleaseLabel,
			Year = aGroupPile.Key.ReleaseYear,
			NumberOfAlums = aGroupPile.Count(),
			AlbumDetailsOfTheGroupPile = aGroupPile //the mimi collection of the group
										 .Select(eachinstanceinGroupPile => new
										 {
										 	Title = eachinstanceinGroupPile.Title,
											Year = eachinstanceinGroupPile.ReleaseYear,
											NumberofAlbumTracks = eachinstanceinGroupPile.Tracks.Count()
										 })
		
		
		})





