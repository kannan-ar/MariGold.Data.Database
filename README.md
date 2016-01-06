<h1>MariGold.Data.Database</h1>
<p>
MariGold.Data.Database is a set of components to do common database query operations. It helps to automate most tedious 
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