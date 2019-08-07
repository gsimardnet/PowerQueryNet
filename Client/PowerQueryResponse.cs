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
    public class PowerQueryResponse
    {

        /// <summary>
        /// Result returned as a System.Data.DataTable serialized in XML.
        /// </summary>
        private string DataTableXML { get; set; } = null;

        /// <summary>
        /// Result returned as a comma-separated values (CSV)
        /// </summary>
        public string Csv { get; set; }

        /// <summary>
        /// Result returned as a System.Data.DataTable.
        /// </summary>
        public DataTable DataTable { get; set; } = null;

        /// <summary>
        /// Path of the temporary file created from DataTableXML.
        /// </summary>
        public string DataTableFile { get; set; }

        /// <summary>
        /// Exception message when an error occured.
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Result returned as HTML
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Result returned as JSON
        /// </summary>
        public string Json { get; set; }

        /// <summary>
        /// Result returned as a readable XML.
        /// </summary>
        public string Xml { get; set; } = null;

        internal void LoadReturnValues(ExecuteOutputFlags executeOutputFlags)
        {
            if (DataTableFile != null)
            {
                DataTableXML = File.ReadAllText(DataTableFile);
                File.Delete(DataTableFile);
                DataTableFile = null;

                var dataTable = new DataTable();
                StringReader sr = new StringReader(DataTableXML);
                dataTable.ReadXml(sr);

                if (executeOutputFlags.HasFlag(ExecuteOutputFlags.DataTable))
                    DataTable = dataTable;

                if (executeOutputFlags.HasFlag(ExecuteOutputFlags.Csv))
                    Csv = dataTable.ToDelimitedFile(',', true);

                if (executeOutputFlags.HasFlag(ExecuteOutputFlags.Html))
                    Html = dataTable.ToHTML();

                if (executeOutputFlags.HasFlag(ExecuteOutputFlags.Json))
                    Json = dataTable.ToContentJSON();

                if (executeOutputFlags.HasFlag(ExecuteOutputFlags.Xml))
                    Xml = dataTable.ToContentXML();
            }
        }
    }
}
