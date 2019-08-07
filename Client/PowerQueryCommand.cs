using Microsoft.Win32;
using PowerQueryNet.Client;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Gives access to different Power Query methods.
    /// </summary>
    public class PowerQueryCommand
    {
        private string mashup = null;
        private Queries queries = null;

        /// <summary>
        /// Initializes a new instance of the PowerQueryCommand class.
        /// </summary>
        public PowerQueryCommand()
        {
            Credentials = new Credentials();
            //Parameters = new Parameters();
            Queries = new Queries();
        }

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
        /// Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Name of the query to execute.
        /// </summary>
        public string QueryName { get; set; }

        /// <summary>
        /// Specifies the outputs that will be generated in memory after the execution of the query.
        /// </summary>
        public ExecuteOutputFlags ExecuteOutputFlags { get; set; } = ExecuteOutputFlags.DataTable;

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
        public int SqlDecimalScale { get; set; }

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

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="queryName">Name of the query to execute</param>
        /// <returns></returns>
        public PowerQueryResponse Execute(string queryName)
        {
            QueryName = queryName;
            return ExecuteMethod("Execute", this);
        }

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="query">Query object to execute</param>
        /// <returns></returns>
        public PowerQueryResponse Execute(Query query)
        {
            if (Queries[query.Name] == null)
                Queries.Add(query);

            return ExecuteMethod("Execute", this);
        }

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="queryName">Name of the query to execute</param>
        /// <param name="queries">Collection of instances of Query to execute Power Query (M) formulas.</param>
        /// <param name="credentials">Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.</param>
        /// <returns></returns>
        public static PowerQueryResponse Execute(string queryName, Queries queries, Credentials credentials = null)
        {
            //return ExecuteMethod("Execute", queryName, queries.ToArray(), credentials);
            var c = new PowerQueryCommand()
            {
                QueryName = queryName,
                Queries = queries,
                Credentials = credentials,
            };
            return ExecuteMethod("Execute", c);
        }

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="queryName">Name of the query to execute</param>
        /// <param name="mashup">Mashup (queries) from which the query will be executed</param>
        /// <param name="credentials">Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.</param>
        /// <returns></returns>
        public static PowerQueryResponse Execute(string queryName, string mashup, Credentials credentials = null)
        {
            //return ExecuteMethod("Execute", queryName, queries.ToArray(), credentials);
            var c = new PowerQueryCommand()
            {
                QueryName = queryName,
                Mashup = mashup,
                Credentials = credentials,
            };
            return ExecuteMethod("Execute", c);
        }

        /// <summary>
        /// Get the mashup (queries) from an Excel or Power BI file.
        /// </summary>
        /// <param name="fileName">Full path of the file</param>
        /// <returns></returns>
        public static string MashupFromFile(string fileName)
        {
            return ExecuteMethod("MashupFromFile", fileName);
        }

        private static dynamic ExecuteMethod(string method, params object[] obj)
        {
            IPowerQueryService powerQueryService = null;

            TimeSpan ipcTimeout;
            string ipcAddress;
            using (var keyPQ = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\PowerQueryNet"))
            {
                if (keyPQ == null)
                    throw new Exception("PowerQueryNet was not found. Please install the application first.");

                ipcTimeout = TimeSpan.Parse(keyPQ.GetValue("IpcTimeout").ToString());
                ipcAddress = keyPQ.GetValue("IpcAddress").ToString();
            }

            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            EndpointAddress endpointAddress = new EndpointAddress(ipcAddress);

            binding.SendTimeout = ipcTimeout;
            binding.ReceiveTimeout = ipcTimeout;
            binding.OpenTimeout = ipcTimeout;
            binding.CloseTimeout = ipcTimeout;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            powerQueryService = ChannelFactory<IPowerQueryService>.CreateChannel(binding, endpointAddress);

            if (method == "Execute")
            {
                var powerQueryCommand = (PowerQueryCommand)obj[0];
                powerQueryCommand.TempPath = System.IO.Path.GetTempPath();
                var executeResponse = powerQueryService.Execute(powerQueryCommand);
                executeResponse.LoadReturnValues(powerQueryCommand.ExecuteOutputFlags);

                return executeResponse;
            }                
            else if (method == "MashupFromFile")
                return powerQueryService.MashupFromFile((string)obj[0]);
            else
                throw new NotImplementedException(string.Format("Method '{0}' is not implemented.", method));
        }
    }
}
