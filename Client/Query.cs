using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace PowerQueryNet.Client
{
    [DataContract(Namespace = "")]
    public class Query
    {
        public Query()
        {
            Name = "Query1";
        }

        public Query(string name, string formula)
        {
            Name = name;
            Formula = formula;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Formula { get; set; }

        public static Query LoadFromFile(string path)
        {
            var query = new Query()
            {
                Name = Path.GetFileNameWithoutExtension(path),
                Formula = File.ReadAllText(path),
            };

            if (query.Name.Contains(" "))
                query.Name = string.Format("#\"{0}\"", query.Name);

            return query;
        }
    }
}
