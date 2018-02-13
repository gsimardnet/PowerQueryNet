using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerQueryNet.Client
{
    [XmlInclude(typeof(Credential))]
    [KnownType(typeof(Credential))]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Credentials")]
    public class Credentials : IEnumerable, ICollection
    {
        private List<Credential> credentialList;

        public Credentials()
        {
            credentialList = new List<Credential>();
        }

        public void Add(object credential)
        {
            if (credential is Credential)
            {
                credentialList.Add((Credential)credential);
            }
        }

        public void Clear()
        {
            credentialList = new List<Credential>();
        }

        public static Credentials LoadFromFile(string path)
        {
            string xml = System.IO.File.ReadAllText(path);
            Credentials credentials = xml.DeserializeXML<Credentials>();
            return credentials;
        }

        public void AddFromFile(string path)
        {
            string xml = System.IO.File.ReadAllText(path);
            Credentials credentials = xml.DeserializeXML<Credentials>();
            credentialList.AddRange(credentials.ToArray());
        }

        public void SaveToFile(string path)
        {
            string xml = this.ToXML();
            System.IO.File.WriteAllText(path, xml);
        }

        public void Remove(Credential credential)
        {
            credentialList.Remove(credential);
        }

        public Credential this[int index]
        {
            get { return credentialList[index]; }
            set { credentialList.Insert(index, value); }
        }
        
        public Credential[] ToArray()
        {
            return credentialList.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return credentialList.GetEnumerator();
        }


        public Credential[] Query2Dict
        {
            get
            {
                return credentialList.ToArray();
            }
        }

        public int Count
        {
            get
            {
                return credentialList.Count;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }    

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
