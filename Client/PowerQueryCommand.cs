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
    public class PowerQueryCommand
    {
        public Queries Queries { get; set; }
        public Credentials Credentials { get; set; }

        public PowerQueryCommand()
        {
            Queries = new Queries();
            Credentials = new Credentials();
        }

        public ExecuteResponse Execute(string queryName)
        {
            return ExecuteMethod("Execute", queryName, Queries, Credentials);
        }

        public ExecuteResponse Execute(Query query)
        {
            if (Queries[query.Name] == null)
                Queries.Add(query);

            return ExecuteMethod("Execute", query.Name, Queries, Credentials);
        }

        public static ExecuteResponse Execute(string queryName, Queries queries, Credentials credentials)
        {
            return ExecuteMethod("Execute", queryName, queries.ToArray(), credentials);
        }

        public string ExecuteToSQL(string connectionString, string queryName, Queries queries, Credentials credentials)
        {
            return ExecuteMethod("ExecuteToSQL", connectionString, queryName, queries, credentials);
        }

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

            if (method == "Execute" && obj[1] is Queries)
                return powerQueryService.Execute((string)obj[0], (Queries)obj[1], (Credentials)obj[2]);
            else if (method == "ExecuteToSQL")
                return powerQueryService.ExecuteToSQL((string)obj[0], (string)obj[1], (Queries)obj[2], (Credentials)obj[3]);
            else if (method == "MashupFromFile")
                return powerQueryService.MashupFromFile((string)obj[0]);
            else
                throw new NotImplementedException(string.Format("Method '{0}' is not implemented.", method));
        }
    }
}
