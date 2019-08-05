using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Credential to access a folder from the Power Query (M) formulas.
    /// </summary>
    public class CredentialFolder : Credential
    {
        /// <summary>
        /// Full path of the folder
        /// </summary>
        public string Path { get; set; }
    }
}
