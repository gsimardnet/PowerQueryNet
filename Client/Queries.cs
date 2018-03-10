using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Collection of instances of Query to execute Power Query (M) formulas.
    /// </summary>
    [XmlInclude(typeof(Query))]
    [KnownType(typeof(Query))]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Queries")]
    public class Queries : IEnumerable, ICollection
    {
        private Dictionary<string, Query> queryDict;

        /// <summary>
        /// Initializes a new instance of the Queries class.
        /// </summary>
        public Queries()
        {
            queryDict = new Dictionary<string, Query>();
        }

        /// <summary>
        /// Adds a Query to the collection.
        /// </summary>
        /// <param name="query"></param>
        public void Add(object query)
        {
            if (query is Query)
            {
                queryDict.Add(((Query)query).Name, (Query)query);
            }
        }

        /// <summary>
        /// Adds a Query to the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        public void Add(string name, string formula)
        {
            var query = new Query
            {
                Name = name,
                Formula = formula,
            };
            queryDict.Add(name, query);
        }

        /// <summary>
        /// Adds Queries from a *.pq or *.m file.
        /// </summary>
        /// <param name="path"></param>
        public void AddFromFile(string path)
        {
            if (path == null) return;
            Add(Query.LoadFromFile(path));
        }

        /// <summary>
        /// Loads Queries from every *.pq or *.m file in a folder.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static Queries LoadFromFolder(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (path == null) return null;
            var queries = new Queries();
            var files = Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => s.EndsWith(".pq") || s.EndsWith(".m"));
            foreach (string file in files)
            {
                queries.AddFromFile(file);
            }
            return queries;
        }

        /// <summary>
        /// Removes all Query from the collection.
        /// </summary>
        public void Clear()
        {
            queryDict = new Dictionary<string, Query>();
        }

        /// <summary>
        /// Removes the first occurence of a Query from the collection.
        /// </summary>
        /// <param name="query"></param>
        public void Remove(Query query)
        {
            queryDict.Remove(query.Name);
        }

        /// <summary>
        /// Gets a Query by name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Query this[string key]
        {           
            get
            {
                Query query = null;
                queryDict.TryGetValue(key, out query);
                return query;
            }
            set { queryDict[key] = value; }
        }

        /// <summary>
        /// Copies the elements of Queries to a new array.
        /// </summary>
        /// <returns></returns>
        public Query[] ToArray()
        {
            return queryDict.Values.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Queries collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return queryDict.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of Query in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return queryDict.Count;
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
        /// Gets a Query by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Query this[int index]
        {
            get
            {
                Query q = queryDict.Values.ToArray()[index];
                return q;
            }
        }

        /// <summary>
        /// NotImplemented yet.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
