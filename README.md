<h1>MariGold.Data.Database</h1>
<p>
A small ORM built on the top of IDbConnection. Like any other ORM, it has options to convert sql to plain CLR objects. In addition to that, it is also possible to implement your own logic to convert data reader to CLR objects. 
</p>
<h2>Usage</h2>
<div class="highlight highlight-source-cs">
<pre>
using MariGold.Data;

using (SqlConnection conn = new SqlConnection(connectionString))
{
	conn.Open();

	var person = conn.Get<Person>("select Id,Name from Person where Id = 1");
}
</pre>
</div>
<h2>Installing via NuGet</h2>
<p>
In Package Manager Console, enter the following command:
</p>
<pre>
Install-Package MariGold.Data.Database
</pre>