using PowerQueryNet.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerQueryNet.Service
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static EventLog Log = new EventLog { Source = "PowerQueryNet" };

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ServiceBase.Run(new WindowsService());
            }
            else if (args.FirstOrDefault(x => x == "-ipc") != null)
            {
                try
                {
                    PowerQueryService.Start();
                    var taskHandle = new ManualResetEvent(false);
                    taskHandle.WaitOne();
                }
                catch (Exception ex)
                {
                    Log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                }
            }
            else //if (args.Length == 1)
            {
                string filePath = args[0];
                string fileExtension = Path.GetExtension(filePath);
                ExecuteResponse executeResponse = null;

                AllocConsole();

                try
                {
                    var powerQueryService = new PowerQueryService();

                    if (fileExtension == ".pq")
                    {
                        string powerQueryFolder = Path.GetDirectoryName(filePath);
                        string powerQueryFile = Path.GetFileNameWithoutExtension(filePath);
                        var queries = Queries.LoadFromFolder(powerQueryFolder);
                        var credentials = Credentials.LoadFromFile(Path.Combine(powerQueryFolder, "#credentials.xml"));
                        var executeRequest = new ExecuteRequest
                        {
                            QueryName = powerQueryFile,
                            Queries = queries,
                            Credentials = credentials,
                        };

                        executeResponse = powerQueryService.Execute(executeRequest);

                        OutputResponse(executeResponse);
                    }

                    if (fileExtension == ".xlsx" 
                     || fileExtension == ".xlsm"
                     || fileExtension == ".pbix"
                     || fileExtension == ".pbit"
                    )
                    {
                        string mashup = powerQueryService.MashupFromFile(filePath);
                        Console.WriteLine(mashup);
                        //Queries queries = powerQueryService.MashupToQueries(filePath);
                    }

                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ex = {0}", ex.ToString());
                }

            }
            //else
            //{
            //    AllocConsole();
            //    Console.WriteLine(" Usage: -ipc");
            //    Console.WriteLine(" Usage: [Power Query File Path]");
            //    Console.WriteLine(" Usage: [Excel or Power BI File Path]");
            //    Console.WriteLine("");
            //    Console.WriteLine(" -ipc\t\t\t\tOpen named pipes service for interprocess communication");
            //    Console.WriteLine(" [Power Query File Path]\tRun Power Query formula (*.pq)");
            //    Console.WriteLine(" [Excel or Power BI File Path]\tGet Mashup from Excel or Power BI (*.xlsx;*.xlsm;*.pbix;*.pbit)");
            //    Console.ReadLine();
            //}

        }

        private static void OutputResponse(ExecuteResponse executeResponse)
        {
            if (executeResponse.Xml != null)
                Console.WriteLine("Xml = {0}", executeResponse.Xml);
            else if (executeResponse.ExceptionMessage != null)
                Console.WriteLine("ExceptionMessage = {0}", executeResponse.ExceptionMessage);
            else
                Console.WriteLine("executeResponse is empty");
        }
    }
}
