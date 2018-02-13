using Microsoft.Win32;
using PowerQueryNet.Engine;
using PowerQueryNet.Client;
using PowerQueryNet.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace PowerQueryNet.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PowerQueryService : IPowerQueryService
    {
        public string MashupFromFile(string fileName)
        {
            try
            {
                return Command.MashupFromFile(fileName);
            }
            catch (Exception ex)
            {
                Program.Log.WriteEntry(string.Format("MashupFromFile exception\nfileName : {0}\nException : {1}", fileName, ex.ToString()), EventLogEntryType.Error);
                return null;
            }
        }

        public Queries MashupToQueries(string fileName)
        {
            try
            {
                return Command.MashupToQueries(fileName);
            }
            catch (Exception ex)
            {
                Program.Log.WriteEntry(string.Format("MashupToQueries exception\nfileName : {0}\nException : {1}", fileName, ex.ToString()), EventLogEntryType.Error);
                return null;
            }
        }

        public string ExecuteToSQL(string connectionString, string queryName, Queries queries, Credentials credentials)
        {
            ExecuteResponse executeResponse = Execute(queryName, queries, credentials);

            if (executeResponse.ExceptionMessage != null)
                return executeResponse.ExceptionMessage;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.CommandText = CreateTable(executeResponse.DataTable);
                        sqlCommand.Connection = connection;
                        sqlCommand.ExecuteNonQuery();
                    }

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        foreach (DataColumn c in executeResponse.DataTable.Columns)
                            bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                        bulkCopy.DestinationTableName = executeResponse.DataTable.TableName;

                        bulkCopy.WriteToServer(executeResponse.DataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        private static string CreateTable(DataTable table)
        {
            string tableName = table.TableName;
            string sqlsc;
            sqlsc = "CREATE TABLE " + tableName + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        }

        public ExecuteResponse Execute(string queryName, Queries queries, Credentials credentials)
        {
            ExecuteResponse executeResponse = new ExecuteResponse();
            CommandCredentials commandCredentials = new CommandCredentials();
            string mashup = "section Section1;\n\r";
            try
            {
                if (credentials != null)
                {
                    foreach (Credential credential in credentials)
                    {
                        if (credential is CredentialFile)
                            commandCredentials.SetCredentialFile(((CredentialFile)credential).Path);
                        else if (credential is CredentialWeb)
                            commandCredentials.SetCredentialWeb(((CredentialWeb)credential).Url);
                        else
                            throw new NotImplementedException("Only CredentialFile is supported for now.");
                    }
                }                    

                Command command = new Command(commandCredentials);

                DataTable dataTable = null;
                
                if (queries != null)
                {
                    foreach (Query p in queries)
                    {
                        mashup += string.Format("\n\rshared {0} = {1};", p.Name, p.Formula);
                    }
                }

                dataTable = command.Execute(queryName, mashup);

                command.Dispose();

                executeResponse.DataTable = dataTable;
                executeResponse.Xml = dataTable.ToXML();

            }
            catch (Exception ex)
            {
                Program.Log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                Program.Log.WriteEntry(mashup, EventLogEntryType.Error);
                if (queries != null)
                    Program.Log.WriteEntry(queries.ToXML(), EventLogEntryType.Error);
                if (credentials != null)
                    Program.Log.WriteEntry(credentials.ToXML(), EventLogEntryType.Error);                
                executeResponse.ExceptionMessage = ex.Message;
            }

            return executeResponse;
        }
        
        internal static void Start()
        {
            TimeSpan ipcTimeout;
            string ipcAddress;
            using (var keyBP = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\PowerQueryNet"))
            {
                ipcTimeout = TimeSpan.Parse(keyBP.GetValue("IpcTimeout").ToString());
                ipcAddress = keyBP.GetValue("IpcAddress").ToString();
            }
            string address = ipcAddress + Process.GetCurrentProcess().Id;

            ServiceHost serviceHost = new ServiceHost(typeof(PowerQueryService));
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.SendTimeout = ipcTimeout;
            binding.ReceiveTimeout = ipcTimeout;
            binding.OpenTimeout = ipcTimeout;
            binding.CloseTimeout = ipcTimeout;
            serviceHost.AddServiceEndpoint(typeof(IPowerQueryService), binding, address);
            serviceHost.Open();            
        }

        public void Stop()
        {
            Environment.Exit(0);
        }
    }
}
