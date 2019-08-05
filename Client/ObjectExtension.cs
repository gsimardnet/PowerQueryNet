using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PowerQueryNet.Client
{
    /// <summary>
    /// Extension methods for System.Object.
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Converts the current contents of the DataTable to XML
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string ToContentXML(this DataTable dataTable)
        {
            var stringWriter = new StringWriter();
            dataTable.WriteXml(stringWriter);
            return stringWriter.ToString();            
        }

        /// <summary>
        /// Converts the current contents of the DataTable to HTML
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string ToHTML(this DataTable dataTable)
        {
            string columnHeaders = "";
            foreach (DataColumn dc in dataTable.Columns)
            {
                columnHeaders += $"\n                        <th>{dc.ColumnName}</th>";
            }

            string rows = "";
            foreach (DataRow dr in dataTable.Rows)
            {
                string row = "";
                foreach (var i in dr.ItemArray)
                {
                    row += $"\n                        <td>{HttpUtility.HtmlEncode(i.ToString())}</td>";
                }
                row = $"\n                    <tr>{row}</tr>";
                rows += row;
            }

            string html = $@"
                <html>
                  <style type='text/css'>
                    html {{
                      background-color : #1E1E1E; 
                    }}
                    h1 {{ 
                      color : #2C91AE;
                      text-align: center;
                      font-size : 24px; 
                    }}
                    th {{
                      color : white;
                      text-align: left;
                      font-size : 18px; 
                    }}
                    td {{
                      color : #DCDCDC;
                    }} 
                  </style>
                  <h1>{dataTable.TableName}</h1>
                  <table style='width:100%'>
                    <tr>{columnHeaders}
                    </tr>{rows}
                  </table>
                </html>
            ";

            return html;
        }

        /// <summary>
        /// Converts the current contents of the DataTable to JSON
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string ToContentJSON(this DataTable dataTable)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dataTable.Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn dc in dataTable.Columns)
                {
                    if (dr[dc] is DateTime dt)
                        row.Add(dc.ColumnName, dt.ToString());
                    else
                        row.Add(dc.ColumnName, dr[dc]); 
                }
                rows.Add(row);
            }
            return javaScriptSerializer.Serialize(rows);
        }

        /// <summary>
        /// Converts the current contents of the DataRow to XML
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static string ToContentXML(this DataRow dataRow)
        {
            DataTable dataTable = new DataTable();
            dataTable = dataRow.Table.Clone();
            dataTable.ImportRow(dataRow);
            return dataTable.ToXML();
        }

        /// <summary>
        /// Converts the current contents of the DataTable to comma-separated values (CSV)
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="delimiter"></param>
        /// <param name="inQuote"></param>
        /// <returns></returns>
        public static string ToDelimitedFile(this DataTable dataTable, char delimiter, bool inQuote)
        {
            string content = "";
            string lastColumn = dataTable.Columns[dataTable.Columns.Count - 1].ColumnName;
            foreach (DataColumn dc in dataTable.Columns)
            {
                content += dc.ColumnName;
                if (dc.ColumnName != lastColumn)
                    content += delimiter;
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                content += Environment.NewLine;

                foreach (DataColumn dc in dataTable.Columns)
                {
                    if (inQuote)
                        content += '"' + dr[dc.ColumnName].ToString() + '"';
                    else
                        content += dr[dc.ColumnName].ToString();

                    //if (inQuote)                        
                    //    content += '"' + dr.Field<string>(dc.ColumnName) + '"';
                    //else
                    //    content += dr.Field<string>(dc.ColumnName);

                    if (dc.ColumnName != lastColumn)
                        content += delimiter;
                }
            }

            return content;
        }

        /// <summary>
        /// Serialize object to JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToJSON<T>(this T value, Type type = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(value.GetType());
            serializer.WriteObject(memoryStream, value);
            byte[] memoryStreamArray = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(memoryStreamArray, 0, memoryStreamArray.Length);
        }

        /// <summary>
        /// Serialize object to XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToXML<T>(this T value, Type type = null)
        {
            if (value == null)
            {
                return null;
            }

            if (type == null)
            {
                type = value.GetType();
            }

            if (type == typeof(DataTable))
            {
                var xmlSerializer = new XmlSerializer(typeof(DataTable));
                var sw = new StringWriter();
                xmlSerializer.Serialize(sw, value);
                return sw.ToString();
            }

            XmlSerializer serializer = new XmlSerializer(type);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;

            Stream stream = null;
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                stream = textWriter.ToString().ToStream();
            }

            XDocument xdoc = XDocument.Load(stream);
            xdoc.Root.RemoveNilTrue();
            return xdoc.ToString();
        }

        private static void RemoveNilTrue(this XElement value)
        {
            XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
            XName n = ns + "nil";
            value.DescendantsAndSelf().Where(x => x.Attribute(n) != null && x.Attribute(n).Value == "true").Remove();
        }

        /// <summary>
        /// Convert string to UTF8 memory stream.
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns></returns>
        public static Stream ToStream(this string s)
        {
            byte[] resultBytes = Encoding.UTF8.GetBytes(s);
            return new MemoryStream(resultBytes);
        }

        /// <summary>
        /// Deserialize XML to object.
        /// </summary>
        /// <param name="xml">XML content</param>
        /// <param name="type">Type of the object</param>
        /// <returns></returns>
        public static dynamic DeserializeXML(this string xml, Type type)
        {
            if (xml == null || xml == "")
                return null;
            XmlSerializer serializer = new XmlSerializer(type);
            StringReader reader = new StringReader(xml);
            var o = Convert.ChangeType(serializer.Deserialize(reader), type);
            reader.Close();
            return o;
        }

        /// <summary>
        /// Deserialize XML to object.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="xml">XML content</param>
        /// <returns></returns>
        public static dynamic DeserializeXML<T>(this string xml)
        {
            return DeserializeXML(xml, typeof(T));
        }

        /// <summary>
        /// Inserts a string into another string. It deletes a specified length of characters in the first string at the start position and then inserts the second string into the first string at the start position.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Stuff(this string str, int start, int length, string replacement)
        {
            string beg = "";
            try
            {
                beg = str.Substring(0, start);
            }
            catch (ArgumentOutOfRangeException)
            {
                beg = "";
            }
            string end = "";
            try
            {
                end = str.Substring(start + length);
            }
            catch (ArgumentOutOfRangeException)
            {
                end = "";
            }
            return beg + replacement + end;
        }
    }
}
