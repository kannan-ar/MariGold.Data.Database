namespace MariGold.Data.Database.Tests
{
    using System;
    using System.Collections.Generic;

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static List<Person> GetMokeDb()
        {
            List<Person> lst = new List<Person>();

            lst.Add(new Person() { Id = 1, Name = "One" });
            lst.Add(new Person() { Id = 2, Name = "Two" });
            lst.Add(new Person() { Id = 3, Name = "Three" });

            return lst;
        }

        public static bool ComparePersons(List<Person> p1, List<Person> p2)
        {
            if (p1 == null || p2 == null)
            {
                return false;
            }

            if (p1.Count != p2.Count)
            {
                return false;
            }

            for (int i = 0; p1.Count > i; i++)
            {
                if (p1[i].Id != p2[i].Id)
                {
                    return false;
                }

                if (p1[i].Name != p2[i].Name)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
