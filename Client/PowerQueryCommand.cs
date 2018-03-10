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
        /// <summary>
        /// Collection of instances of Query to execute Power Query (M) formulas.
        /// </summary>
        public Queries Queries { get; set; }

        /// <summary>
        /// Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.
        /// </summary>
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Initializes a new instance of the PowerQueryCommand class.
        /// </summary>
        public PowerQueryCommand()
        {
            Queries = new Queries();
            Credentials = new Credentials();
        }

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="queryName">Name of the query to execute</param>
        /// <returns></returns>
        public ExecuteResponse Execute(string queryName)
        {
            return ExecuteMethod("Execute", new ExecuteRequest { QueryName = queryName, Queries = this.Queries, Credentials = this.Credentials });
        }

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="query">Query object to execute</param>
        /// <returns></returns>
        public ExecuteResponse Execute(Query query)
        {
            if (Queries[query.Name] == null)
                Queries.Add(query);

            return ExecuteMethod("Execute", new ExecuteRequest { QueryName = query.Name, Queries = this.Queries, Credentials = this.Credentials });
        }

        /// <summary>
        /// Execute the specified query.
        /// </summary>
        /// <param name="queryName">Name of the query to execute</param>
        /// <param name="queries">Collection of instances of Query to execute Power Query (M) formulas.</param>
        /// <param name="credentials">Collection of instances of Credential to access one or many ressources from the Power Query (M) formulas.</param>
        /// <returns></returns>
        public static ExecuteResponse Execute(string queryName, Queries queries, Credentials credentials)
        {
            //return ExecuteMethod("Execute", queryName, queries.ToArray(), credentials);
            return ExecuteMethod("Execute", new ExecuteRequest { QueryName = queryName, Queries = queries, Credentials = credentials });
        }

        //public string ExecuteToSQL(string connectionString, string queryName, Queries queries, Credentials credentials)
        //{
        //    return ExecuteMethod("ExecuteToSQL", connectionString, queryName, queries, credentials);
        //}

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
            //using (var keyPQ = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\PowerQueryNet"))
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
            binding.MaxReceivedMessageSize = 2147483647;
            powerQueryService = ChannelFactory<IPowerQueryService>.CreateChannel(binding, endpointAddress);

            if (method == "Execute")
                return powerQueryService.Execute((ExecuteRequest)obj[0]);
            //else if (method == "ExecuteToSQL")
            //    return powerQueryService.ExecuteToSQL((string)obj[0], (string)obj[1], (Queries)obj[2], (Credentials)obj[3]);
            else if (method == "MashupFromFile")
                return powerQueryService.MashupFromFile((string)obj[0]);
            else
                throw new NotImplementedException(string.Format("Method '{0}' is not implemented.", method));
        }
    }
}
