<Query Kind="Expression">
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

//method syntax, simply dump of a table
Albums

//method syntax long way
Albums
	.Select(x => x.Title)

//query syntax
//the placeholder name in this sample is anyrowindatacollection
//typically you will see extremely short placeholder names such as x
from anyrowindatacollection in Albums
select anyrowindatacollection

//Filtering your query data
//Where
//method syntax is .Where(x => condition1 [&& or || conditionx])
//query syntax is where condition1 [&& or || conditionx]

//method syntax
Customers
	.Where(x => x.Country.Contains("US"))
	.Select(x => x)
	
//query syntax
from x in Customers
where x.Country.Contains("US")
select x

//Show all the Tracks that have a file size greater 700000000 bytes
Tracks
	.Where(trk => trk.Bytes > 700000000)

from trk in Tracks
where trk.Bytes > 700000000
select trk