using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Credential to access an OData resource from the Power Query (M) formulas.
    /// </summary>
    public class CredentialOData : Credential
    {
        /// <summary>
        /// URL address of the resource
        /// </summary>
        public string Url { get; set; }
    }
}
