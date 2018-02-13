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
    [XmlInclude(typeof(Query))]
    [KnownType(typeof(Query))]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Queries")]
    public class Queries : IEnumerable, ICollection
    {
        private Dictionary<string, Query> queryDict;

        public Queries()
        {
            queryDict = new Dictionary<string, Query>();
        }

        public void Add(object query)
        {
            if (query is Query)
            {
                queryDict.Add(((Query)query).Name, (Query)query);
            }
        }

        public void Add(string name, string formula)
        {
            var query = new Query
            {
                Name = name,
                Formula = formula,
            };
            queryDict.Add(name, query);
        }

        public void AddFromFile(string path)
        {
            Add(Query.LoadFromFile(path));
        }

        public static Queries LoadFromFolder(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var queries = new Queries();
            var files = Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => s.EndsWith(".pq") || s.EndsWith(".m"));
            foreach (string file in files)
            {
                queries.AddFromFile(file);
            }
            return queries;
        }

        public void Clear()
        {
            queryDict = new Dictionary<string, Query>();
        }

        public void Remove(Query query)
        {
            queryDict.Remove(query.Name);
        }

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

        public Query[] ToArray()
        {
            return queryDict.Values.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return queryDict.Values.GetEnumerator();
        }

        public Query[] Query2Dict
        {
            get
            {
                return queryDict.Values.ToArray();
            }
        }

        public int Count
        {
            get
            {
                return queryDict.Count;
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

        public Query this[int index]
        {
            get
            {
                Query q = queryDict.Values.ToArray()[index];
                return q;
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}
