using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    public class ExecuteResponse
    {
        public string DataTableXML { get; set; }
        public string Xml { get; set; }
        public string ExceptionMessage { get; set; }

        public DataTable DataTable
        {
            get
            {
                if (DataTableXML != null)
                {
                    var dt = new DataTable();
                    StringReader sr = new StringReader(DataTableXML);
                    dt.ReadXml(sr);
                    return dt;
                }
                else
                    return null;
            }

        }

    }
}
