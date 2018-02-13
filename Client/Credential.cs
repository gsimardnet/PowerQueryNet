using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PowerQueryNet.Client
{
    [XmlInclude(typeof(CredentialFile))]
    [KnownType(typeof(CredentialFile))]
    [XmlInclude(typeof(CredentialWeb))]
    [KnownType(typeof(CredentialWeb))]
    public abstract class Credential
    {

    }
}
