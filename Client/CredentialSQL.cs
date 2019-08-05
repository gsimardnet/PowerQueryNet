using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Credential to access a SQL ressource from the Power Query (M) formulas.
    /// </summary>
    public class CredentialSQL : Credential
    {
        /// <summary>
        /// SQL Server Name and database name. Format: serverName;databaseName
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// Username value
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password value
        /// </summary>
        public string Password { get; set; }
    }
}
