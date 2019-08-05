using Microsoft.Win32;
using PowerQueryNet.Engine;
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
        private static bool isRemote = false;

        public string MashupFromFile(string fileName)
        {
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()) + ".pbix";
                File.Copy(fileName, tempFile);
                string mashup = Command.MashupFromFile(tempFile);
                File.Delete(tempFile);
                return mashup;
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

        private static void OutputToSQL(ExecuteRequest executeRequest, DataTable dataTable)
        {
            using (SqlConnection connection = new SqlConnection(executeRequest.SqlConnectionString))
            {
                connection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = connection;

                    if (executeRequest.SqlTableAction == SqlTableAction.DropAndCreate)
                    {
                        sqlCommand.CommandText = $"IF OBJECT_ID('{dataTable}', 'U') IS NOT NULL DROP TABLE {dataTable}";
                        sqlCommand.ExecuteNonQuery();
                    }

                    if (executeRequest.SqlTableAction == SqlTableAction.DropAndCreate || executeRequest.SqlTableAction == SqlTableAction.Create)
                    {
                        sqlCommand.CommandText = CreateTable(dataTable, executeRequest.SqlDecimalPrecision, executeRequest.SqlDecimalScale);
                        sqlCommand.ExecuteNonQuery(); 
                    }

                    if (executeRequest.SqlTableAction == SqlTableAction.TruncateAndInsert)
                    {
                        sqlCommand.CommandText = $"TRUNCATE TABLE {dataTable}";
                        sqlCommand.ExecuteNonQuery();
                    }

                    if (executeRequest.SqlTableAction == SqlTableAction.DeleteAndInsert)
                    {
                        sqlCommand.CommandText = $"DELETE FROM {dataTable}";
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn c in dataTable.Columns)
                        bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                    bulkCopy.DestinationTableName = dataTable.TableName;

                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }

        private static string CreateTable(DataTable table, int decimalPrecision, int decimalScale)
        {
            string tableName = table.TableName;
            string sqlsc;
            sqlsc = $"CREATE TABLE {tableName} (";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += $"\n [{table.Columns[i].ColumnName}] ";
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
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.Decimal":
                        sqlsc += $" decimal ({decimalPrecision},{decimalScale})";
                        break;
                    case "System.Double":
                        sqlsc += $" decimal ({decimalPrecision},{decimalScale})";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += $" IDENTITY({table.Columns[i].AutoIncrementSeed},{table.Columns[i].AutoIncrementStep}) ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        }

        //public ExecuteResponse Execute(string queryName, Queries queries, Credentials credentials, ExecuteOutputFlags executeOutputFlags = ExecuteOutputFlags.DataTable | ExecuteOutputFlags.Xml)
        public ExecuteResponse Execute(ExecuteRequest executeRequest)
        {
            Command command = null;
            ExecuteResponse executeResponse = new ExecuteResponse();
            CommandCredentials commandCredentials = new CommandCredentials();
            string mashup;
            try
            {
                if (executeRequest.Credentials != null && executeRequest.Credentials.Count > 0)
                {
                    foreach (Credential credential in executeRequest.Credentials)
                    {
                        if (credential is CredentialFile credentialFile)
                            commandCredentials.SetCredentialFile(credentialFile.Path);
                        else if(credential is CredentialFolder credentialFolder)
                            commandCredentials.SetCredentialFolder(credentialFolder.Path);
                        else if (credential is CredentialWeb credentialWeb)
                            commandCredentials.SetCredentialWeb(credentialWeb.Url);
                        else if (credential is CredentialSQL credentialSQL)
                            commandCredentials.SetCredentialSQL(credentialSQL.SQL, credentialSQL.Username, credentialSQL.Password);
                        else if (credential is CredentialOData credentialOData)
                            commandCredentials.SetCredentialOData(((CredentialOData)credential).Url, credentialOData.Username, credentialOData.Password);
                        else
                            throw new NotImplementedException("This Credential kind is not supported for now.");
                    }
                }

                command = new Command(commandCredentials);

                DataTable dataTable = null;

                if (executeRequest.Queries != null && executeRequest.Queries.Count > 0)
                {
                    if (executeRequest.Queries.Count == 1)
                        executeRequest.QueryName = executeRequest.Queries[0].Name;
                    
                    mashup = "section Section1;\n\r";

                    foreach (Query q in executeRequest.Queries)
                    {
                        string name;
                        if (q.Name.Contains(" "))
                            name = string.Format("#\"{0}\"", q.Name);
                        else
                            name = q.Name;
                        mashup += string.Format("\n\rshared {0} = {1};", name, q.Formula);
                    }
                }
                else
                    mashup = executeRequest.Mashup;

                dataTable = command.Execute(executeRequest.QueryName, mashup);

                if (executeRequest.ExecuteOutputFlags == 0 && executeRequest.SqlConnectionString != null)
                    executeRequest.ExecuteOutputFlags = ExecuteOutputFlags.Sql;
                else if (executeRequest.ExecuteOutputFlags == 0)
                    executeRequest.ExecuteOutputFlags = ExecuteOutputFlags.DataTable | ExecuteOutputFlags.Xml;

                if (isRemote)
                {
                    executeResponse.DataTableFile = $"{executeRequest.TempPath}{Guid.NewGuid()}.xml";
                    File.WriteAllText(executeResponse.DataTableFile, dataTable.ToXML()); 
                }
                else
                {
                    if (executeRequest.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.DataTable))
                        executeResponse.DataTable = dataTable;
                }

                if (executeRequest.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Csv) || !string.IsNullOrWhiteSpace(executeRequest.CsvFileName))
                {
                    executeResponse.Csv = dataTable.ToDelimitedFile(',', true);

                    if (!string.IsNullOrWhiteSpace(executeRequest.CsvFileName))
                        File.WriteAllText(executeRequest.CsvFileName, executeResponse.Csv);

                    if (isRemote)
                        executeResponse.Csv = null;
                }

                if (executeRequest.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Html) || !string.IsNullOrWhiteSpace(executeRequest.HtmlFileName))
                {
                    executeResponse.Html = dataTable.ToHTML();

                    if (!string.IsNullOrWhiteSpace(executeRequest.HtmlFileName))
                        File.WriteAllText(executeRequest.HtmlFileName, executeResponse.Html);

                    if (isRemote)
                        executeResponse.Html = null;
                }

                if (executeRequest.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Json) || !string.IsNullOrWhiteSpace(executeRequest.JsonFileName))
                {
                    executeResponse.Json = dataTable.ToContentJSON();

                    if (!string.IsNullOrWhiteSpace(executeRequest.JsonFileName))
                        File.WriteAllText(executeRequest.JsonFileName, executeResponse.Json);

                    if (isRemote)
                        executeResponse.Json = null;
                }

                if (executeRequest.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Sql))
                {
                    if (executeRequest.SqlConnectionString == null)
                        throw new InvalidOperationException("Cannot output to SQL. SqlConnectionString must be defined.");

                    dataTable.TableName = executeRequest.SqlTableName;

                    OutputToSQL(executeRequest, dataTable);
                }

                if (executeRequest.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Xml) || !string.IsNullOrWhiteSpace(executeRequest.XmlFileName))
                {
                    executeResponse.Xml = dataTable.ToContentXML();

                    if (!string.IsNullOrWhiteSpace(executeRequest.XmlFileName))
                        File.WriteAllText(executeRequest.XmlFileName, executeResponse.Xml);

                    if (isRemote)
                        executeResponse.Xml = null;
                }

            }
            catch (Exception ex)
            {
                Program.Log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                if (executeRequest.Queries != null && executeRequest.Queries.Count > 0)
                    Program.Log.WriteEntry(executeRequest.Queries.ToXML(), EventLogEntryType.Error);
                if (executeRequest.Mashup != null)
                    Program.Log.WriteEntry(executeRequest.Mashup, EventLogEntryType.Error);
                if (executeRequest.Credentials != null && executeRequest.Credentials.Count > 0)
                    Program.Log.WriteEntry(executeRequest.Credentials.ToXML(), EventLogEntryType.Error);
                executeResponse.ExceptionMessage = ex.Message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }

            return executeResponse;
        }

        internal static void Start()
        {
            TimeSpan ipcTimeout;
            string ipcAddress;
            //using (var keyBP = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\PowerQueryNet"))
            using (var keyPQ = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\PowerQueryNet"))
            {
                ipcTimeout = TimeSpan.Parse(keyPQ.GetValue("IpcTimeout").ToString());
                ipcAddress = keyPQ.GetValue("IpcAddress").ToString();
            }

            ServiceHost serviceHost = new ServiceHost(typeof(PowerQueryService));
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.SendTimeout = ipcTimeout;
            binding.ReceiveTimeout = ipcTimeout;
            binding.OpenTimeout = ipcTimeout;
            binding.CloseTimeout = ipcTimeout;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            serviceHost.AddServiceEndpoint(typeof(IPowerQueryService), binding, ipcAddress);
            serviceHost.Open();

            isRemote = true;
        }
    }
}
