namespace MariGold.Data.Database.Tests.PostgresTests
{
    using System;
    using System.Configuration;

    public static class PostgresUtility
    {
        public static string ConnectionString
        {
            get
            {
                return @"User ID=postgres;Password=pass@word1;Host=localhost;Port=5432;Database=Tests;";
            }
        }
    }
}
