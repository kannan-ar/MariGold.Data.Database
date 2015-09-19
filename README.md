<h1>MariGold.Data.Database</h1>
<p>
A small ORM built on the top of IDbConnection. Like any other ORM, it has options to convert sql to plain CLR objects. In addition to that, it is also possible to implement your own logic to convert data reader to CLR objects. 
</p>
<h2>Usage</h2>
<pre code="c#">
using MariGold.Data;

using (SqlConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	var person = conn.Get<Person>("select Id,Name from Person where Id = 1");
}
</pre>