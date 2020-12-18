using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Converter;
using DataAccessLayer.Models;

namespace DataAccessLayer
{
    public class DataAccessLayer : IDataAccessLayer
    {
        SqlConnection connection;
        IParser parser;

        public DataAccessLayer(Options.ConnectionOptions options, IParser parser)
        {
            string connectionString = $"Data Source={options.DataSource}; Database={options.Database}; User={options.User}; Password={options.Password}; Integrated Security=False";
            this.parser = parser;

            using (TransactionScope scope = new TransactionScope())
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                scope.Complete();
            }
        }

        public Person GetPerson(int id)
        {
            Person person = GetPersonOpts<Person>(id);

            return person;
        }

        public T GetPersonOpts<T>(int id) where T : new()
        {
            SqlCommand command = new SqlCommand($"Get{typeof(T).Name}", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("id", id));

            using (TransactionScope scope = new TransactionScope())
            {
                SqlDataReader reader = command.ExecuteReader();
                T opt = Map<T>(reader, parser);
                scope.Complete();

                if (opt == null)
                {
                    return new T();
                }
                else
                {
                    return opt;
                }
            }
        }

        T Map<T>(SqlDataReader reader, IParser parser) where T : new()
        {
            Dictionary<string, object> parse = Parse(reader);
            T res = new T();
            res = parser.Map<T>(parse);

            return res;
        }

        Dictionary<string, object> Parse(SqlDataReader reader)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            while (reader.Read())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    object val = reader.GetValue(i);

                    dict.Add(name, val);
                }
                res = dict;
            }
            reader.Close();

            return res;
        }

        // Write a log.
        public void Log(DateTime ourDate, string message)
        {
            SqlCommand command = new SqlCommand("Log", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("date", ourDate));
            command.Parameters.Add(new SqlParameter("message", message));

            using (TransactionScope scope = new TransactionScope())
            {
                command.ExecuteNonQuery();
                scope.Complete();
            }
        }
    }
}