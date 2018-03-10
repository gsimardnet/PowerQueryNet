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
        /// Full path of the file
        /// </summary>
        public string SQL { get; set; }
    }
}
