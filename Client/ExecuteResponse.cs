using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Response from the PowerQueryCommand.Execute method.
    /// </summary>
    public class ExecuteResponse
    {
        /// <summary>
        /// Result returned as a System.Data.DataTable serialized in XML.
        /// </summary>
        public string DataTableXML { get; set; }

        /// <summary>
        /// Result returned as a readable XML.
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Exception message when an error occured.
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Result returned as a System.Data.DataTable.
        /// </summary>
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
