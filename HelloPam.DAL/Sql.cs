using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloPam.DAL
{
    public class Sql
    {
        private readonly string connexionString;

        private readonly string providerName;

        private readonly DbProviderFactory factory;


        public Sql(string connexion, string providerName)
        {
            this.connexionString = connexion;
            this.providerName = providerName;
            factory = DbProviderFactories.GetFactory(providerName);
        }

        public Sql(string connectionStringName)
            : this(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString,
            ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName)
        {

        }

        public DbConnection GetConnexion()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connexionString;
            connection.Open();
            return connection;
        }

        public void Execute(string query, IEnumerable<Parameter> parameters, bool isStoredProcedure = false)
        {
            using (var connection = GetConnexion())
            {
                using (var command = GetCommand(query, parameters, isStoredProcedure))
                {
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
        }

        public DbDataReader Read(string query, IEnumerable<Parameter> parameters, bool isStoredProcedure = false)
        {
            var connection = GetConnexion();
            using (var command = GetCommand(query, parameters, isStoredProcedure))
            {
                command.Connection = connection;
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
        }

        private DbCommand GetCommand(string query, IEnumerable<Parameter> parameters, bool IsStoredProcedure)
        {
            var command = factory.CreateCommand();

            command.CommandText = query;
            if (IsStoredProcedure)
                command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var p = factory.CreateParameter();
                    p.ParameterName = parameter.Name;
                    p.DbType = parameter.Type;
                    p.Value = parameter.Value;
                    command.Parameters.Add(p);
                }
            }
            return command;
        }

        public class Parameter
        {
            public string Name { get; set; }

            public DbType Type { get; set; }

            public object Value { get; set; }


            public Parameter(string name, DbType type, object value)
            {
                Name = name;
                Type = type;
                Value = value;
            }
        }

        public IEnumerable<T> Read2<T>(string query, IEnumerable<Parameter> parameters, Func<DbDataReader, T> callBack, bool IsStoredProcedure = false)
        {
            List<T> datas = new List<T>();
            using (var connection = GetConnexion())
            {
                using (var command = GetCommand(query, parameters, IsStoredProcedure))
                {
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            callBack?.Invoke(reader);
                        }
                        reader.Close();
                    }
                }

            }
            return datas;
        }
    }
}

