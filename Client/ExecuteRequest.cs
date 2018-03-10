using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Inputs for the PowerQueryCommand.Execute method.
    /// </summary>
    public class ExecuteRequest
    {
        /// <summary>
        /// Credentials to access one or many ressources when executing the query.
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Queries that will compose the mashup from which the query will be executed.
        /// </summary>
        public Queries Queries { get; set; }

        /// <summary>
        /// Name of the query to execute.
        /// </summary>
        public string QueryName { get; set; }

        /// <summary>
        /// Specifies the outputs that will be generated in memory after the execution of the query.
        /// </summary>
        public ExecuteOutputFlags ExecuteOutputFlags { get; set; }

        /// <summary>
        /// Full path of the CSV file that will be generated after the execution of the query.
        /// </summary>
        public string CsvFileName { get; set; }

        /// <summary>
        /// Full path of the XML file that will be generated after the execution of the query.
        /// </summary>
        public string XmlFileName { get; set; }

        /// <summary>
        /// Connection string to the SQL Server and Database where the Table will be generated after the execution of the query.
        /// </summary>
        public string SqlConnectionString { get; set; }

        /// <summary>
        /// When SqlConnectionString is not null, name of the SQL Table that will be generated after the execution of the query. If this property is null, the default name will correspond to the QueryName.
        /// </summary>
        public string SqlTableName { get; set; }

    }
}
