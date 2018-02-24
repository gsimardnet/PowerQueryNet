using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Service
{
    public class WindowsService : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            try
            {
                Program.Log.WriteEntry("PowerQueryNet.Service Start", EventLogEntryType.Information);
                PowerQueryService.Start();
            }
            catch (Exception ex)
            {
                Program.Log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            Program.Log.WriteEntry("PowerQueryNet.Service Stop", EventLogEntryType.Information);
        }
    }
}
