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

                    PowerQueryCommand powerQueryCommand = LoadCommand(a);

                    PowerQueryResponse powerQueryResponse = powerQueryService.Execute(powerQueryCommand);

                    OutputResponse(a, powerQueryResponse);
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

        private static void OutputResponse(Arguments a, PowerQueryResponse powerQueryResponse)
        {
            if (powerQueryResponse.ExceptionMessage != null)
                throw new Exception($"{powerQueryResponse.ExceptionMessage}");

            if (string.IsNullOrWhiteSpace(a.OutputFile))
            {
                switch (a.OutputFlags)
                {
                    case ExecuteOutputFlags.Csv:
                        Console.WriteLine(powerQueryResponse.Csv);
                        break;
                    case ExecuteOutputFlags.Html:
                        Console.WriteLine(powerQueryResponse.Html);
                        break;
                    case ExecuteOutputFlags.Json:
                        Console.WriteLine(powerQueryResponse.Json);
                        break;
                    case ExecuteOutputFlags.Xml:
                        Console.WriteLine(powerQueryResponse.Xml);
                        break;
                    default:
                        break;
                }
            }

            if (powerQueryResponse.DataTable != null && a.OutputToWindow)
                new WindowGrid(powerQueryResponse.DataTable).ShowDialog();
        }

        private static PowerQueryCommand LoadCommand(Arguments a)
        {
            var c = new PowerQueryCommand()
            {
                QueryName = a.QueryName,
            };

            if (!string.IsNullOrWhiteSpace(a.CredentialsFile))
                c.Credentials = Credentials.LoadFromFile(a.CredentialsFile);

            if (!string.IsNullOrWhiteSpace(a.OutputFile))
            {
                switch (a.OutputFlags)
                {
                    case ExecuteOutputFlags.Csv:
                        c.CsvFileName = a.OutputFile;
                        break;
                    case ExecuteOutputFlags.Html:
                        c.HtmlFileName = a.OutputFile;
                        break;
                    case ExecuteOutputFlags.Json:
                        c.JsonFileName = a.OutputFile;
                        break;
                    case ExecuteOutputFlags.Xml:
                        c.XmlFileName = a.OutputFile;
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(a.SqlConnectionString))
            {
                c.SqlConnectionString = a.SqlConnectionString;
                c.SqlTableName = a.TableName;
                c.SqlTableAction = a.SqlTableAction;
                c.SqlDecimalPrecision = 18;
                c.SqlDecimalScale = 2;
            }

            if (string.IsNullOrWhiteSpace(a.SourceFileExtension))
                c.Queries = Queries.LoadFromFolder(a.Source);

            if (a.SourceFileExtension == ".pq")
                c.Queries.AddFromFile(a.Source);

            if (a.SourceFileExtension == ".xlsx"
             || a.SourceFileExtension == ".xlsm"
             || a.SourceFileExtension == ".pbix"
             || a.SourceFileExtension == ".pbit"
            )
            {
                c.Mashup = powerQueryService.MashupFromFile(a.Source);
                //req.Queries = powerQueryService.MashupToQueries(source);
            }

            c.ExecuteOutputFlags = a.OutputFlags.Value;

            if (a.OutputToWindow)
                c.ExecuteOutputFlags |= ExecuteOutputFlags.DataTable;

            return c;
        }
    }
}
