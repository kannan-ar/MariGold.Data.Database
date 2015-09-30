namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper class to embed sql query string and other properties to create an IDbCommand.
    /// This class can be expand in future to accommodate more options to create an IDbCommand.
    /// </summary>
    public sealed class Query
    {
        private string sql;
        private IDictionary<string, object> parameters;
        private CommandType commandType;

        /// <summary>
        /// A valid sql query. Thorws ArgumentNullException if the string is null or empty.
        /// </summary>
        public string Sql
        {
            get
            {
                return sql;
            }

            set
            {
                if (!ValidateString(value))
                {
                    throw new ArgumentNullException("Sql");
                }

                sql = value;
            }
        }

        /// <summary>
        /// System.Data.CommandType to create the IDbCommand.
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return commandType;
            }

            set
            {
                commandType = value;
            }
        }

        /// <summary>
        /// A collection of parameters to create the IDbCommand.
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get
            {
                return parameters;
            }

            set
            {
                if (value != null)
                {
                    parameters = value;
                }
            }
        }

        private bool ValidateString(string value)
        {
            value = value ?? string.Empty;

            return !string.IsNullOrEmpty(value.Trim());
        }

        /// <summary>
        /// Constructs an instance of Query with the given parameters.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public Query(
            string sql,
            CommandType commandType,
            IDictionary<string, object> parameters)
        {
            if (!ValidateString(sql))
            {
                throw new ArgumentNullException("sql");
            }

            this.sql = sql;
            this.commandType = commandType;
            this.parameters = parameters ?? new Dictionary<string, object>();
        }

        public Query(
            string sql,
            CommandType commandType)
            : this(sql, commandType, null)
        {
        }

        public Query(
            string sql,
            IDictionary<string, object> parameters)
            : this(sql, CommandType.Text, parameters)
        {
        }

        public Query(string sql)
            : this(sql, CommandType.Text, null)
        {
        }

        /// <summary>
        /// Merge the given parameters to existing parameter collection. 
        /// </summary>
        /// <param name="parameters"></param>
        public void MergeParameters(IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            if (parameters.Count == 0)
            {
                return;
            }

            foreach (KeyValuePair<string, object> param in parameters)
            {
                parameters[param.Key] = param.Value;
            }
        }

        static internal IDbCommand GetCommand(
            IDbConnection connection,
            string sql,
            CommandType commandType,
            IDictionary<string, object> parameters)
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Database connection is not established yet");
            }

            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Database connection is not established yet");
            }

            if(string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            IDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = sql;

            cmd.CommandType = commandType;

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var p in parameters)
                {
                    IDbDataParameter param = cmd.CreateParameter();
                    param.ParameterName = p.Key;
                    param.Value = p.Value;

                    cmd.Parameters.Add(param);
                }
            }

            return cmd;
        }

        static internal IDbCommand GetCommand(
            IDbConnection connection,
            string sql,
            IDictionary<string, object> parameters)
        {
            return GetCommand(connection, sql, CommandType.Text, parameters);
        }

        static internal IDbCommand GetCommand(
            IDbConnection connection,
            string sql,
            CommandType commandType)
        {
            return GetCommand(connection, sql, commandType, null);
        }

        static internal IDbCommand GetCommand(
            IDbConnection connection,
            string sql)
        {
            return GetCommand(connection, sql, CommandType.Text, null);
        }

        internal IDbCommand GetCommand(IDbConnection connection)
        {
            return GetCommand(connection, sql, commandType, parameters);
        }
    }
}
