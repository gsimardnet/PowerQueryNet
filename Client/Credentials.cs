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
    /// <summary>
    /// Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.
    /// </summary>
    [XmlInclude(typeof(Credential))]
    [KnownType(typeof(Credential))]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Credentials")]
    public class Credentials : IEnumerable, ICollection
    {
        private List<Credential> credentialList;

        /// <summary>
        /// Initializes a new instance of the Credentials class.
        /// </summary>
        public Credentials()
        {
            credentialList = new List<Credential>();
        }

        /// <summary>
        /// Adds a Credential to the collection.
        /// </summary>
        /// <param name="credential"></param>
        public void Add(object credential)
        {
            if (credential is Credential)
            {
                credentialList.Add((Credential)credential);
            }
        }

        /// <summary>
        /// Removes all Credential from the collection.
        /// </summary>
        public void Clear()
        {
            credentialList = new List<Credential>();
        }

        /// <summary>
        /// Loads Credentials from an XML file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Credentials LoadFromFile(string path)
        {
            if (path == null) return null;
            string xml = System.IO.File.ReadAllText(path);
            Credentials credentials = xml.DeserializeXML<Credentials>();
            return credentials;
        }

        /// <summary>
        /// Adds Credentials from an XML file.
        /// </summary>
        /// <param name="path"></param>
        public void AddFromFile(string path)
        {
            if (path == null) return;
            string xml = System.IO.File.ReadAllText(path);
            Credentials credentials = xml.DeserializeXML<Credentials>();
            credentialList.AddRange(credentials.ToArray());
        }

        /// <summary>
        /// Saves Credentials to an XML file.
        /// </summary>
        /// <param name="path"></param>
        public void SaveToFile(string path)
        {
            if (path == null) return;
            string xml = this.ToXML();
            System.IO.File.WriteAllText(path, xml);
        }

        /// <summary>
        /// Removes the first occurence of a Credential from the collection.
        /// </summary>
        /// <param name="credential"></param>
        public void Remove(Credential credential)
        {
            credentialList.Remove(credential);
        }

        /// <summary>
        /// Gets a Credential by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Credential this[int index]
        {
            get { return credentialList[index]; }
            set { credentialList.Insert(index, value); }
        }

        /// <summary>
        /// Copies the elements of Credentials to a new array.
        /// </summary>
        /// <returns></returns>
        public Credential[] ToArray()
        {
            return credentialList.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Credentials collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return credentialList.GetEnumerator();
        }
        
        /// <summary>
        /// Gets the number of Credential in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return credentialList.Count;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// NotImplemented yet.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current array.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
