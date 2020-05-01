using System.Collections.Generic;
using System.Data.SQLite;

namespace IQueryableTask
{
    /// <summary>
    /// Data Access Service
    /// </summary>
    public class PersonService
    {
        readonly string _connectionString = @"Data Source = C:\Users\trewor\source\repos\IQueryableTask\IQueryableTask.Test\people.db;";

        public IEnumerable<Person> Search(string sql)
        {
            var search = new List<Person>();
            using (SQLiteConnection con = new SQLiteConnection(_connectionString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        search.Add(new Person
                        {
                            Id = rdr.GetInt32(0),
                            FirstName = rdr.GetString(1),
                            LastName = rdr.GetString(2),
                            Sex = (Sex) rdr.GetInt32(3),
                            Age = rdr.GetInt32(4)
                        });
                    }
                }
                con.Close();
            }
            return search;
        }
    }
}
