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

//this set of queries is done under the C# Statement(s) ide
//	of Linqpad

//this environment expects the use of C# statement grammar
//the results of a query are NOT automatically displayed as is
//	in the Expression environment

//to display the results, you need to save the results in a
//	variable, then, use the Linqpad method .Dump() to see
//	the results

//IMPORTANT!!! .Dump() is a Linqpad method. It is NOT a C# method

//image criteria is a parameter in your BLL method

string criteria = "US";

//image the BLL method will do the query against the database
//	using the value of the parameter passed in
//method syntax
var resultm = Customers
	.Where(x => x.Country.Contains(criteria))
	.Select(x => x);
//resultm.Dump();

//query syntax
var resultq = from x in Customers
where x.Country.Contains(criteria)
select x;
//resultq.Dump();

//first sample practice exercise from the Take-Home repo
//this sample uses a declare collection (an array)

int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
var shownumbers = numbers
					.Where(x => x > 4);
shownumbers.Dump();

//Can one use .Dump() directly on the query?
//Yes
numbers
	.Where(x => x > 4)
	.Dump();

//using query syntax
(from x in numbers
 where x > 4
 select x)
	.Dump();
