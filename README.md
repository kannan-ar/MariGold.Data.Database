MariGold.Data.Database
========================================
MariGold.Data.Database is a minimalist set of components to automate most of the tedious tasks in database query operations. It is a zero-configuration library which works with all types of IDbConnection implementations. MariGold.Data.Database will also supports dynamic types.
Various components of MariGold.Data.Database can be used to:
- Fetch a data reader from an sql string.
- Create CLR object from an sql string.
- Create a CLR object from a data reader.
- Execute sql string using an IDbConnection.
- Helper methods to fetch values from data reader without boxing.###
Installing via NuGet-----------
In Package Manager Console, enter the following command:
```Install-Package MariGold.Data.Database```
Usage-----------
Create IDataReader from sql string
-----------------------------------------
```csharp
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	IDataReader dr = conn.GetDataReader("Select Id, Name From Employee Where Id = @Id",
		new Dictionary<string,object>() {
		{ "Id", 1 }});
	
}```
Create CLR object from an sql string
-----------------------------------------
```csharpusing MariGold.Data;

public class Employee
{
	public Int32 Id{ get; set; }
	public String Name{ get; set; }
}


using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
	Employee emp = conn.Get<Employee>("Select Id, Name From Employee Where Id = @Id",
		new Dictionary<string,object>() {
		{ "Id", 1 }});
}```
Create a CLR object from a data reader-----------------------------------------
```csharpusing MariGold.Data;

using (IDataReader dr = GetDataReader())
{
	Employee emp = dr.Get<Employee>();
}```
Execute sql string using an IDbConnection-----------------------------------------
```csharpusing MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	conn.Execute("Delete From Employee Where Id = @Id",
		new Dictionary<string,object>() {
		{ "Id", 1 }});
}```