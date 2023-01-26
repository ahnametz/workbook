<Query Kind="Expression">
  <Connection>
    <ID>3a9f97a2-b8f0-4e5f-b642-905a78f4c4e7</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\MSSQLSERVER01</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>WestWind</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//collection: Payments
//filtering: none
//grouping: Year, Month, Description
//grouping processing: count payments for the group

Payments
	.GroupBy(p => new {p.PaymentDate.Year, p.PaymentDate.Month, p.PaymentType.PaymentTypeDescription})
	.OrderBy(pg => pg.Key.Year)
	.ThenBy(pg => pg.Key.Month)
	.ThenBy(pg => pg.Key.PaymentTypeDescription)
	.Select(pg => new
	{
		Year = pg.Key.Year,
		Month = pg.Key.Month,
		Payment = pg.Key.PaymentTypeDescription,
		Count = pg.Count(),
		Amount = pg.Sum(x => x.Amount) //a property off an instance of the raw
										//	data that makes up the group's mini collection
	})