using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    public class ExecuteResponse
    {
        public DataTable DataTable { get; set; }
        public string Xml { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
