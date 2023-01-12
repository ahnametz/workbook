<Query Kind="Expression">
  <Connection>
    <ID>d2dd5f12-7ae4-4027-9030-31b1fe10f36c</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>W309-DWELCH\MSSQLSERVER01</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

Customers.Where(x => x.Country.Contains("USA")).OrderBy(x => x.FirstName )