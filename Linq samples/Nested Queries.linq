<Query Kind="Program">
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

void Main()
{
	//Nested queries
	//sometimes referred to as subqueries
	//simply put: it is a query within a query [within a query ....]
	
	//List all sales support employees showing their fullname
	// (last, first), title and phone
	//for each employee, show a list of customers they support
	//Show the customer fullname (last, first), city and state
	
	//employee 1, title, phone
	//	customer 2000, city, state
	//  customer 2109, city, state
	//   ...
	//employee 2, title, phone
	//  customer ....
	
	//there appears to be 2 separate list that need to be
	//	within one final dataset collection
	// list of employees
	//		list of employee customers
	//concern: the lists are intermixed
	
	var results = Employees
					.Where(e => e.Title.Contains("Sales Support"))
					.Select(e => new EmployeeItem
							{
								Name = e.LastName + ", " + e.FirstName,
								Title = e.Title,
								Phone = e.Phone,
								CustomerList = e.SupportRepCustomers
														.Select(c => new CustomerItem
														{
															Name = c.LastName + ", " + c.FirstName,
															City = c.City,
															State = c.State
														})
							});
	results.Dump();
	
	//list all albums that are from 1990.
	//display the album title and artist
	//for each album, display the track info Name and genre description.
	//(you do not need to strongly type the collections
	//  if you want to, you can practice setting up a strongly type set of classes)
	
}

// You can define other methods, fields, classes and namespaces here

public class CustomerItem
{
	public string Name {get;set;}
	public string City {get;set;}
	public string State {get; set;}
}

public class EmployeeItem
{
	public string Name {get;set;}
	public string Title {get; set;}
	public string Phone {get;set;}
	public IEnumerable<CustomerItem> CustomerList{get;set;}
}