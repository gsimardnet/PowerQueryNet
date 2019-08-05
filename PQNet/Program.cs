using PowerQueryNet.Client;
using PowerQueryNet.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PowerQueryNet.PQNet
{
    class Program
    {
        const string usage = @"
    PowerQueryNet Command Line Interface
    
    Usage: pqnet source [query name] [options]
    
    source (one of the following):
      ■ Power Query file (*.pq) to execute
      ■ Directory of the Power Query file(s) (*.pq) to load
      ■ Power BI file (*.pbix;*.pbit)
      ■ Excel file (*.xlsx;*.xlsm)
    
    query name:
      Name of the query to execute
    
    options:
      -c|--credentials ""file path""     Path to the credentials file.
      -o|--output {csv|json|html|xml}  Output format of the resulting data.
      -f|--file ""file path""            Output result to the specified file path.
      -w|--window                      Output the result to a window.
      -s|--sql ""connection string""     Output result to the specified server.
      -t|--table ""table name""          Table name of the SQL or XML output.
      -a|--action {c|dc|di|i|ti}       SQL action: 
                                         c  - create
                                         dc - drop/create
                                         di - delete/insert
                                         i  - insert
                                         ti - truncate/insert
        ";

        private static PowerQueryService powerQueryService = new PowerQueryService();

        [STAThread]
        static void Main(string[] args)
        {
            int exitCode = 0;
            try
            {
                if (args == null || args.Length == 0)
                    Console.Write(usage);
                else
                {
                    var a = new Arguments(args);

                    if (!string.IsNullOrWhiteSpace(a.OutputFile))
                        File.WriteAllText(a.OutputFile, "");

                    ExecuteRequest request = LoadRequest(a);

                    ExecuteResponse response = powerQueryService.Execute(request);

                    OutputResponse(a, response);
                }
                exitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                exitCode = 1;
            }
            Environment.Exit(exitCode);
        }

        private static void OutputResponse(Arguments a, ExecuteResponse response)
        {
            if (response.ExceptionMessage != null)
                throw new Exception($"{response.ExceptionMessage}");

            if (string.IsNullOrWhiteSpace(a.OutputFile))
            {
                switch (a.OutputFlags)
                {
                    case ExecuteOutputFlags.Csv:
                        Console.WriteLine(response.Csv);
                        break;
                    case ExecuteOutputFlags.Html:
                        Console.WriteLine(response.Html);
                        break;
                    case ExecuteOutputFlags.Json:
                        Console.WriteLine(response.Json);
                        break;
                    case ExecuteOutputFlags.Xml:
                        Console.WriteLine(response.Xml);
                        break;
                    default:
                        break;
                }
            }

            if (response.DataTable != null && a.OutputToWindow)
                new WindowGrid(response.DataTable).ShowDialog();
        }

        private static ExecuteRequest LoadRequest(Arguments a)
        {
            var r = new ExecuteRequest()
            {
                QueryName = a.QueryName,
            };

            if (!string.IsNullOrWhiteSpace(a.CredentialsFile))
                r.Credentials = Credentials.LoadFromFile(a.CredentialsFile);

            if (!string.IsNullOrWhiteSpace(a.OutputFile))
            {
                switch (a.OutputFlags)
                {
                    case ExecuteOutputFlags.Csv:
                        r.CsvFileName = a.OutputFile;
                        break;
                    //case ExecuteOutputFlags.DataTable:
                    //    break;
                    case ExecuteOutputFlags.Html:
                        r.HtmlFileName = a.OutputFile;
                        break;
                    case ExecuteOutputFlags.Json:
                        r.JsonFileName = a.OutputFile;
                        break;
                    case ExecuteOutputFlags.Xml:
                        r.XmlFileName = a.OutputFile;
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(a.SqlConnectionString))
            {
                r.SqlConnectionString = a.SqlConnectionString;
                r.SqlTableName = a.TableName;
                r.SqlTableAction = a.SqlTableAction;
                r.SqlDecimalPrecision = 18;
                r.SqlDecimalScale = 2;
            }

            if (string.IsNullOrWhiteSpace(a.SourceFileExtension))
                r.Queries = Queries.LoadFromFolder(a.Source);

            if (a.SourceFileExtension == ".pq")
                r.Queries.AddFromFile(a.Source);

            if (a.SourceFileExtension == ".xlsx"
             || a.SourceFileExtension == ".xlsm"
             || a.SourceFileExtension == ".pbix"
             || a.SourceFileExtension == ".pbit"
            )
            {
                r.Mashup = powerQueryService.MashupFromFile(a.Source);
                //req.Queries = powerQueryService.MashupToQueries(source);
            }

            r.ExecuteOutputFlags = a.OutputFlags.Value;

            if (a.OutputToWindow)
                r.ExecuteOutputFlags |= ExecuteOutputFlags.DataTable;

            return r;
        }
    }
}
