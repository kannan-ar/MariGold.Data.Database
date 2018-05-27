## MariGold.Data.Database
MariGold.Data.Database is a minimalist set of components to automate most of the tedious tasks in database query operations. It is a zero-configuration library which works with all types of IDbConnection implementations.

MariGold.Data.Database is not just an ORM tool but also a collection of methods for various database operations. For example, MariGold.Data.Database can be used to generate an IDataReader from an IDbConnection or convert an IDataReader into a CLR entity.


### Installing via NuGet

In Package Manager Console, enter the following command:
```
Install-Package MariGold.Data.Database
```
### Usage
MariGold.Data.Database supports both static and dynamic data types.

##### Create IDataReader from sql string

```csharp

using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	IDataReader dr = conn.GetDataReader("Select Id, Name From Employee Where Id = @Id", new { Id = 1 });
	
}
```
##### Create a CLR object from an sql string
```csharp
using MariGold.Data;

public class Employee
{
	public Int32 Id{ get; set; }
	public String Name{ get; set; }
}


using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	Employee emp = conn.Get<Employee>("Select Id, Name From Employee Where Id = @Id", new { Id = 1 });
}
```
##### Create a CLR object from an IDataReader
```csharp
using MariGold.Data;

using (IDataReader dr = GetDataReader())
{
	if(dr.Read())
	{
		Employee emp = dr.Get<Employee>();
	}
}
```
##### Execute sql string using an IDbConnection
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	conn.Execute("Delete From Employee Where Id = @Id", new { Id = 1 });
}
```
##### Create a dynamic object
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	var emp = conn.Get("Select Id, Name From Employee Where Id = @Id", new { Id = 1 });
}
```
##### Create IList from IDbConnection
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	IList<Employee> lstEmp = conn.GetList<Employee>("Select Id, Name From Employee");
}
```
##### Create Enumerable List
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	var lstEmp = conn.GetEnumerable<Employee>("Select Id, Name From Employee");
}
```
##### Create dynamic IList
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	var lstEmp = conn.GetList("Select Id, Name From Employee");
}
```
##### Create dynamic an IEnumerable List
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	var lstEmp = conn.GetEnumerable("Select Id, Name From Employee");
}
```
##### Query multiple records in one query
QueryMultiple function utilizes the IDataReader's NextResult method to process multiple results from a single data reader.
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	var records = conn.QueryMultiple("Select Id, Name From Employee;Select Count(Id) From Employee");

	var lstEmp = records.GetList<Employee>();

	Int32 count = Convert.ToInt32(records.GetScalar());
}
```
##### Utility methods
MariGold.Data.Database also contains several utility methods to handle the data from IDataReader. For example, the below code illustrates how to convert Datetime and decimal values from an opened IDataReader.
```csharp
using(IDataReader dr = conn.GetDataReader("Select DOB,Salary From Employee Where Id = @Id", new { Id = 1 }))
{
	if (dr.Read())
	{
		DateTime? dob = dr.ConvertToDateTime("DOB", null);
		decimal salary = dr.ConvertToDecimal("Salary", 0);
	}
}
```
#### Custom mappings
Sometimes the database field names does not match its equivalent CLR object field names. This can be solved using the custom mappings.
```csharp
using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	EntityManager<Employee>.Map(p => p.EmployeeId, "Id").Map(p => p.EmployeeName, "Name");

	Employee emp = conn.Get<Employee>("Select Id, Name From Employee Where Id = @Id", new { Id = 1 });
}
```
##### Dispose After Use
Once created, the custom mapping will remain throughout the life time of AppDomain. The DisposeAfterUse method can use if the mapping needs to be disposed after the very first use. The following statment will dispose the mapping immediately after the select statment executed.
```csharp
EntityManager<Employee>.Map(p => p.EmployeeId, "Id").Map(p => p.EmployeeName, "Name").DisposeAfterUse();
```
#### Map database fields with underscore to pascal case entity fields
Some databases like PostgreSQL following the naming conventions with lower cases and underscores which does not match the C# property naming conventions. The conversion from lower case database column names with underscore to pascal case can be done without any custom mapping using the UnderscoreToPascalCase configuration settings property. For example, a "first_name" data base field can be automatically mapped to "FirstName" entity field.
```csharp
Config.UnderscoreToPascalCase = true;
```
#### Property Mapping
MariGold.Data.Database automatically resolves all root level properties except complext types like a class objects. You can explicitly map these properties with database fields using the following methods.

##### Map a single a property

