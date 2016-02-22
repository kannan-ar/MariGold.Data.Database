<h1>MariGold.Data.Database</h1>
<p>
MariGold.Data.Database is a minimalist set of components to automate most of the tedious tasks in database query operations. It is a zero-configuration library which works with all types of IDbConnection implementations. MariGold.Data.Database will also supports dynamic types.
</p>
<p>
Various components of MariGold.Data.Database can be used to
<ul>
<li>Fetch a data reader from an sql string.</li>
<li>Create CLR object from an sql string.</li>
<li>Create a CLR object from a data reader.</li>
<li>Execute sql string using an IDbConnection.</li>
<li>Helper methods to fetch values from data reader without boxing.</li>
</ul>
</p>
<h2>Installing via NuGet</h2>
<p>
In Package Manager Console, enter the following command:
</p>
<pre>
Install-Package MariGold.Data.Database
</pre>
<h3>Create IDataReader from sql string</h3>
<div class="highlight highlight-source-cs">
<pre>
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	IDataReader dr = conn.GetDataReader("Select Id, Name From Employee Where Id = @Id",
		new Dictionary<string,object>() {
		{ "Id", 1 }});
	
}
</pre>
</div>
<h3>Create CLR object from an sql string</h3>
<div class="highlight highlight-source-cs">
<pre>
using MariGold.Data;

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
}
</pre>
</div>
<h3>Create a CLR object from a data reader</h3>
<div class="highlight highlight-source-cs">
<pre>
using MariGold.Data;

using (IDataReader dr = GetDataReader())
{
	Employee emp = dr.Get<Employee>();
}
</pre>
</div>
<h3>Execute sql string using an IDbConnection</h3>
<div class="highlight highlight-source-cs">
<pre>
using MariGold.Data;

using (IDbConnection conn = new SqlConnection(connectionString))
{
	conn.Open();
				
	conn.Execute("Delete From Employee Where Id = @Id",
		new Dictionary<string,object>() {
		{ "Id", 1 }});
}
</pre>
</div>