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
        }
    }
}
