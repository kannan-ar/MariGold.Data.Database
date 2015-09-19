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
        private string _sql;
        private IDictionary<string, object> _parameters;

        /// <summary>
        /// A valid sql query. Thorws ArgumentNullException if the string is null or empty.
        /// </summary>
        public string Sql 
        {
            get
            {
                return _sql;
            }

            set
            {
                if (!ValidateString(value))
                {
                    throw new ArgumentNullException("Sql");
                }

                _sql = value;
            }
        }
        
        /// <summary>
        /// System.Data.CommandType to create the IDbCommand.
        /// </summary>
        public CommandType CommandType { get; set; }
        
        /// <summary>
        /// A collection of parameters to create the IDbCommand.
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }

            set
            {
                if (value != null)
                {
                    _parameters = value;
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
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            if (!ValidateString(sql))
            {
                throw new ArgumentNullException("sql");
            }

            _sql = sql;
            CommandType = commandType;
            _parameters = parameters ?? new Dictionary<string, object>();
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
                _parameters[param.Key] = param.Value;
            }
        }

        internal IDbCommand GetCommand(IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = Sql;

            cmd.CommandType = CommandType;

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (var p in Parameters)
                {
                    IDbDataParameter param = cmd.CreateParameter();
                    param.ParameterName = p.Key;
                    param.Value = p.Value;

                    cmd.Parameters.Add(param);
                }
            }

            return cmd;
        }
    }
}
