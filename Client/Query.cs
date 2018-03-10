using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Represents a query to execute Power Query (M) formulas.
    /// </summary>
    [DataContract(Namespace = "")]
    public class Query
    {
        /// <summary>
        /// Initializes a new instance of the Query class.
        /// </summary>
        public Query()
        {
            Name = "Query1";
        }

        /// <summary>
        /// Initializes a new instance of the Query class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        public Query(string name, string formula)
        {
            Name = name;
            Formula = formula;
        }

        /// <summary>
        /// Name of the query.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Power Query (M) expresion.
        /// </summary>
        [DataMember]
        public string Formula { get; set; }

        /// <summary>
        /// Loads Query from a *.pq or *.m file.
        /// </summary>
        /// <param name="path">Full path of the file</param>
        /// <returns></returns>
        public static Query LoadFromFile(string path)
        {
            var query = new Query()
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Formula = File.ReadAllText(path),
            };

            //if (query.Name.Contains(" "))
            //    query.Name = string.Format("#\"{0}\"", query.Name);

            return query;
        }
    }
}
