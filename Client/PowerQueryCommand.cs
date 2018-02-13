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
            Process process = null;
            string response = null;
            IPowerQueryService powerQueryService = null;

            try
            {
                TimeSpan ipcTimeout;
                string ipcAddress;
                string exePath;
                using (var keyBP = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\PowerQueryNet"))
                {
                    if (keyBP == null)
                        throw new Exception("PowerQueryNet registry key was not found.");

                    ipcTimeout = TimeSpan.Parse(keyBP.GetValue("IpcTimeout").ToString());
                    ipcAddress = keyBP.GetValue("IpcAddress").ToString();
                    exePath = keyBP.GetValue("ExePath").ToString();
                }
                process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = "-ipc";
                process.Start();

                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                EndpointAddress endpointAddress = new EndpointAddress(ipcAddress + process.Id);

                int runTry = 1;
                while (response == null && runTry <= 3)
                {
                    Thread.Sleep(500); //wait for named pipe channel to initialize
                    try
                    {
                        binding.SendTimeout = ipcTimeout;
                        binding.ReceiveTimeout = ipcTimeout;
                        binding.OpenTimeout = ipcTimeout;
                        binding.CloseTimeout = ipcTimeout;
                        binding.MaxReceivedMessageSize = 2147483647;
                        powerQueryService = ChannelFactory<IPowerQueryService>.CreateChannel(binding, endpointAddress);

                        if (method == "Execute")
                            if (obj[1] is Queries)
                                return powerQueryService.Execute((string)obj[0], (Queries)obj[1], (Credentials)obj[2]);

                        if (method == "ExecuteToSQL")
                            return powerQueryService.ExecuteToSQL((string)obj[0], (string)obj[1], (Queries)obj[2], (Credentials)obj[3]);

                        if (method == "MashupFromFile")
                            return powerQueryService.MashupFromFile((string)obj[0]);
                    }
                    catch (EndpointNotFoundException)
                    {
                    }
                    runTry++;
                }
            }
            finally
            {
                bool stopped = false;
                if (powerQueryService != null)
                {
                    try
                    {
                        powerQueryService.Stop();
                    }
                    catch (CommunicationException)
                    {
                        stopped = true;
                    }
                }

                if (!stopped && process != null)
                    {
                    process.Kill();
                    process.Dispose();
                }
            }
            return response;
        }
    }
}
