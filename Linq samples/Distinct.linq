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

//Distinct is not an aggregate
//Distinct removes duplicate result rows from a collection

//Create a list of countries are customers are from

Customers
	.Select(x => x.Country) //returns a collection of country names
	.Distinct( ) //return a collection of unique country names