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

        private static void OutputToSQL(PowerQueryCommand powerQueryCommand, DataTable dataTable)
        {
            string tableName;

            using (SqlConnection connection = new SqlConnection(powerQueryCommand.SqlConnectionString))
            {
                connection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = connection;

                    var dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        sqlCommand.CommandText = $@"                            
                            DECLARE @ObjectName sysname
                            DECLARE @SchemaName sysname

                            IF PARSENAME(@TableName, 1) IS NULL
                            OR PARSENAME(@TableName, 3) IS NOT NULL
                            OR PARSENAME(@TableName, 4) IS NOT NULL
                            BEGIN
	                            ;THROW 50000, 'Table name cannot include server name or database name. Use the connection string instead.', 1
                            END

                            SET @ObjectName = PARSENAME(@TableName, 1)
                            SET @SchemaName = PARSENAME(@TableName, 2)

                            SELECT SchemaName = isnull(@SchemaName, ''), ObjectName = isnull(@ObjectName, '')
                        ";
                        var sqlParameter = new SqlParameter("TableName", SqlDbType.NVarChar, 128);
                        sqlParameter.Value = dataTable.TableName;
                        sqlCommand.Parameters.Add(sqlParameter);
                        da.Fill(dt);
                        sqlCommand.Parameters.Clear();
                        string schemaName = dt.Rows[0][0].ToString();
                        string objectName = dt.Rows[0][1].ToString();
                        tableName = schemaName == "" ? $"[{objectName}]" : $"[{schemaName}].[{objectName}]";
                    }

                    if (powerQueryCommand.SqlTableAction == SqlTableAction.DropAndCreate)
                    {
                        sqlCommand.CommandText = $"IF OBJECT_ID('{tableName}', 'U') IS NOT NULL DROP TABLE {tableName}";
                        sqlCommand.ExecuteNonQuery();
                    }

                    if (powerQueryCommand.SqlTableAction == SqlTableAction.DropAndCreate || powerQueryCommand.SqlTableAction == SqlTableAction.Create)
                    {
                        sqlCommand.CommandText = CreateTable(tableName, dataTable, powerQueryCommand.SqlDecimalPrecision, powerQueryCommand.SqlDecimalScale);
                        sqlCommand.ExecuteNonQuery(); 
                    }

                    if (powerQueryCommand.SqlTableAction == SqlTableAction.TruncateAndInsert)
                    {
                        sqlCommand.CommandText = $"TRUNCATE TABLE {tableName}";
                        sqlCommand.ExecuteNonQuery();
                    }

                    if (powerQueryCommand.SqlTableAction == SqlTableAction.DeleteAndInsert)
                    {
                        sqlCommand.CommandText = $"DELETE FROM {tableName}";
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn c in dataTable.Columns)
                        bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                    bulkCopy.DestinationTableName = tableName;

                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }

        private static string CreateTable(string tableName, DataTable table, int decimalPrecision, int decimalScale)
        {
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

        public PowerQueryResponse Execute(PowerQueryCommand powerQueryCommand)
        {
            Command command = null;
            PowerQueryResponse powerQueryResponse = new PowerQueryResponse();
            CommandCredentials commandCredentials = new CommandCredentials();
            string mashup;
            try
            {
                if (powerQueryCommand.Credentials != null && powerQueryCommand.Credentials.Count > 0)
                {
                    foreach (Credential credential in powerQueryCommand.Credentials)
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

                if (powerQueryCommand.Queries != null && powerQueryCommand.Queries.Count > 0)
                {
                    if (powerQueryCommand.Queries.Count == 1)
                        powerQueryCommand.QueryName = powerQueryCommand.Queries[0].Name;
                    
                    mashup = "section Section1;\n\r";

                    foreach (Query q in powerQueryCommand.Queries)
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
                    mashup = powerQueryCommand.Mashup;

                dataTable = command.Execute(powerQueryCommand.QueryName, mashup);

                if (powerQueryCommand.ExecuteOutputFlags == 0 && powerQueryCommand.SqlConnectionString != null)
                    powerQueryCommand.ExecuteOutputFlags = ExecuteOutputFlags.Sql;
                else if (powerQueryCommand.ExecuteOutputFlags == 0)
                    powerQueryCommand.ExecuteOutputFlags = ExecuteOutputFlags.DataTable | ExecuteOutputFlags.Xml;

                if (isRemote)
                {
                    powerQueryResponse.DataTableFile = $"{powerQueryCommand.TempPath}{Guid.NewGuid()}.xml";
                    File.WriteAllText(powerQueryResponse.DataTableFile, dataTable.ToXML()); 
                }
                else
                {
                    if (powerQueryCommand.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.DataTable))
                        powerQueryResponse.DataTable = dataTable;
                }

                if (powerQueryCommand.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Csv) || !string.IsNullOrWhiteSpace(powerQueryCommand.CsvFileName))
                {
                    powerQueryResponse.Csv = dataTable.ToDelimitedFile(',', true);

                    if (!string.IsNullOrWhiteSpace(powerQueryCommand.CsvFileName))
                        File.WriteAllText(powerQueryCommand.CsvFileName, powerQueryResponse.Csv);

                    if (isRemote)
                        powerQueryResponse.Csv = null;
                }

                if (powerQueryCommand.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Html) || !string.IsNullOrWhiteSpace(powerQueryCommand.HtmlFileName))
                {
                    powerQueryResponse.Html = dataTable.ToHTML();

                    if (!string.IsNullOrWhiteSpace(powerQueryCommand.HtmlFileName))
                        File.WriteAllText(powerQueryCommand.HtmlFileName, powerQueryResponse.Html);

                    if (isRemote)
                        powerQueryResponse.Html = null;
                }

                if (powerQueryCommand.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Json) || !string.IsNullOrWhiteSpace(powerQueryCommand.JsonFileName))
                {
                    powerQueryResponse.Json = dataTable.ToContentJSON();

                    if (!string.IsNullOrWhiteSpace(powerQueryCommand.JsonFileName))
                        File.WriteAllText(powerQueryCommand.JsonFileName, powerQueryResponse.Json);

                    if (isRemote)
                        powerQueryResponse.Json = null;
                }

                if (powerQueryCommand.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Sql))
                {
                    if (powerQueryCommand.SqlConnectionString == null)
                        throw new InvalidOperationException("Cannot output to SQL. SqlConnectionString must be defined.");

                    dataTable.TableName = powerQueryCommand.SqlTableName;

                    OutputToSQL(powerQueryCommand, dataTable);
                }

                if (powerQueryCommand.ExecuteOutputFlags.HasFlag(ExecuteOutputFlags.Xml) || !string.IsNullOrWhiteSpace(powerQueryCommand.XmlFileName))
                {
                    powerQueryResponse.Xml = dataTable.ToContentXML();

                    if (!string.IsNullOrWhiteSpace(powerQueryCommand.XmlFileName))
                        File.WriteAllText(powerQueryCommand.XmlFileName, powerQueryResponse.Xml);

                    if (isRemote)
                        powerQueryResponse.Xml = null;
                }

            }
            catch (Exception ex)
            {
                Program.Log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                if (powerQueryCommand.Queries != null && powerQueryCommand.Queries.Count > 0)
                    Program.Log.WriteEntry(powerQueryCommand.Queries.ToXML(), EventLogEntryType.Error);
                if (powerQueryCommand.Mashup != null)
                    Program.Log.WriteEntry(powerQueryCommand.Mashup, EventLogEntryType.Error);
                if (powerQueryCommand.Credentials != null && powerQueryCommand.Credentials.Count > 0)
                    Program.Log.WriteEntry(powerQueryCommand.Credentials.ToXML(), EventLogEntryType.Error);
                powerQueryResponse.ExceptionMessage = ex.Message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }

            return powerQueryResponse;
        }

        internal static void Start()
        {
            TimeSpan ipcTimeout;
            string ipcAddress;
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
