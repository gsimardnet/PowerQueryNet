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
        private string mashup = null;
        private Queries queries = null;

        /// <summary>
        /// Initializes a new instance of the ExecuteRequest class.
        /// </summary>
        public ExecuteRequest()
        {
            Credentials = new Credentials();
            //Parameters = new Parameters();
            Queries = new Queries();
            SqlDecimalPrecision = 18;
            SqlDecimalScale = 6;
        }

        /// <summary>
        /// Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Collection of instances of Parameter in the Power Query (M) formulas.
        /// </summary>
        //public Parameters Parameters { get; set; }

        /// <summary>
        /// Collection of instances of Query to execute Power Query (M) formulas.
        /// </summary>
        public Queries Queries
        {
            get
            {
                return queries;
            }
            set
            {
                if (value != null && value.Count > 0 && mashup != null)
                    throw new InvalidOperationException("Queries cannot be assigned when Mashup is defined.");

                queries = value;
            }
        }

        /// <summary>
        /// Mashup (queries) from which the query will be executed.
        /// </summary>
        public string Mashup
        {
            get
            {
                return mashup;
            }
            set
            {
                if (value != null && Queries != null && Queries.Count > 0)
                    throw new InvalidOperationException("Mashup cannot be assigned when Queries are defined.");

                mashup = value;
            }
        }

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
        /// Full path of the HTML file that will be generated after the execution of the query.
        /// </summary>
        public string HtmlFileName { get; set; }

        /// <summary>
        /// Full path of the JSON file that will be generated after the execution of the query.
        /// </summary>
        public string JsonFileName { get; set; }

        /// <summary>
        /// Full path of the XML file that will be generated after the execution of the query.
        /// </summary>
        public string XmlFileName { get; set; }

        /// <summary>
        /// Connection string to the SQL Server and Database where the Table will be generated after the execution of the query.
        /// </summary>
        public string SqlConnectionString { get; set; }

        /// <summary>
        /// For SQL decimal data type, the maximum total number of decimal digits to be stored.
        /// </summary>
        public int SqlDecimalPrecision { get; set; }

        /// <summary>
        /// For SQL decimal data type, the number of decimal digits that are stored to the right of the decimal point.
        /// </summary>
        public int SqlDecimalScale{ get; set; }

        /// <summary>
        /// When SqlConnectionString is not null, name of the SQL Table that will be generated after the execution of the query. If this property is null, the default name will correspond to the QueryName.
        /// </summary>
        public string SqlTableName { get; set; }

        /// <summary>
        /// Action taken when SqlConnectionString and SqlTableName are not null
        /// </summary>
        public SqlTableAction SqlTableAction { get; set; }

        /// <summary>
        /// Path of the folder to create temporary files.
        /// </summary>
        public string TempPath { get; set; }
    }
}
