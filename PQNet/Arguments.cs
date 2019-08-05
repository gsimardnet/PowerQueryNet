using PowerQueryNet.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.PQNet
{
    internal class Arguments
    {
        public string Source { get; set; } = "";
        public string SourceFileExtension { get; set; } = "";

        public string QueryName { get; set; } = "";

        public string CredentialsFile { get; set; } = "";

        public string OutputFormat { get; set; } = "";
        public string OutputFile { get; set; } = "";

        public string SqlConnectionString { get; set; } = "";
        public string TableName { get; set; } = "";
        public string SqlAction { get; set; } = "";

        public bool OutputToWindow { get; set; } = false;

        public ExecuteOutputFlags? OutputFlags { get; set; }

        public SqlTableAction SqlTableAction { get; set; }

        public Arguments(string[] args)
        {
            if (args.Length >= 1)
            {
                Source = args[0];
            }

            for (int i = 1; i < args.Length; i++)
            {
                string undefinedArg = null;
                try
                {
                    switch (args[i].ToLower())
                    {
                        case "-c":
                        case "--credentials":
                            i++;
                            CredentialsFile = args[i];
                            break;
                        case "-o":
                        case "--output":
                            i++;
                            OutputFormat = args[i];
                            break;
                        case "-f":
                        case "--file":
                            i++;
                            OutputFile = args[i];
                            break;
                        case "-s":
                        case "--sql":
                            i++;
                            SqlConnectionString = args[i];
                            break;
                        case "-t":
                        case "--table":
                            i++;
                            TableName = args[i];
                            break;
                        case "-a":
                        case "--action":
                            i++;
                            SqlAction = args[i];
                            break;
                        case "-w":
                        case "--window":
                            OutputToWindow = true;
                            break;
                        default:
                            undefinedArg = args[i];
                            break;
                    }
                    if (undefinedArg != null)
                    {
                        if (i == 1)
                            QueryName = args[i];
                        else
                            throw new Exception($"Undefined argument: {args[i]}");
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    throw new Exception($"Missing parameter for {args[i]}");
                }
            }

            if (!File.Exists(Source) && !Directory.Exists(Source))
                throw new Exception($"Source not found: {Source}");

            if (File.Exists(Source))
                SourceFileExtension = Path.GetExtension(Source);

            //OutputFlags = ExecuteOutputFlags.DataTable;
            if (!string.IsNullOrWhiteSpace(OutputFormat)
             || !string.IsNullOrWhiteSpace(OutputFile)
            )
            {
                if (string.IsNullOrWhiteSpace(OutputFormat))
                {
                    string outputFileExtension = Path.GetExtension(OutputFile);
                    OutputFormat = outputFileExtension.Substring(1);
                }

                switch (OutputFormat.ToLower())
                {
                    case "csv":
                        OutputFlags = ExecuteOutputFlags.Csv;
                        break;
                    case "json":
                        OutputFlags = ExecuteOutputFlags.Json;
                        break;
                    case "html":
                        OutputFlags = ExecuteOutputFlags.Html;
                        break;
                    case "xml":
                        OutputFlags = ExecuteOutputFlags.Xml;
                        break;
                    default:
                        throw new Exception($"Unsupported output format: {OutputFormat}");
                }
            }

            if (!string.IsNullOrWhiteSpace(SqlConnectionString))
            {                
                switch (SqlAction.ToLower())
                {
                    case "":
                        SqlTableAction = SqlTableAction.Create;
                        break;
                    case "c":
                        SqlTableAction = SqlTableAction.Create;
                        break;
                    case "dc":
                        SqlTableAction = SqlTableAction.DropAndCreate;
                        break;
                    case "di":
                        SqlTableAction = SqlTableAction.DeleteAndInsert;
                        break;
                    case "i":
                        SqlTableAction = SqlTableAction.Insert;
                        break;
                    case "ti":
                        SqlTableAction = SqlTableAction.TruncateAndInsert;
                        break;
                    default:
                        throw new Exception($"Unsupported action: {SqlAction}");
                }

                if (OutputFlags == null)
                    OutputFlags = ExecuteOutputFlags.Sql;
                else
                    OutputFlags |= ExecuteOutputFlags.Sql;
            }

            if (OutputFlags == null)
            {
                OutputFlags = ExecuteOutputFlags.DataTable;
                OutputToWindow = true;
            }

            if (string.IsNullOrWhiteSpace(QueryName) && SourceFileExtension == ".pq")
                QueryName = Path.GetFileNameWithoutExtension(Source);

            if (string.IsNullOrWhiteSpace(QueryName))
                throw new Exception($"Query name must be defined.");

            if (string.IsNullOrWhiteSpace(TableName))
                TableName = QueryName;
        }
    }
}
