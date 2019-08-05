using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Credential to access a ressource from the Power Query (M) formulas.
    /// </summary>
    [XmlInclude(typeof(CredentialFile))]
    [KnownType(typeof(CredentialFile))]
    [XmlInclude(typeof(CredentialFolder))]
    [KnownType(typeof(CredentialFolder))]
    [XmlInclude(typeof(CredentialWeb))]
    [KnownType(typeof(CredentialWeb))]
    [XmlInclude(typeof(CredentialSQL))]
    [KnownType(typeof(CredentialSQL))]
    [XmlInclude(typeof(CredentialOData))]
    [KnownType(typeof(CredentialOData))]
    public abstract class Credential
    {

    }
}
