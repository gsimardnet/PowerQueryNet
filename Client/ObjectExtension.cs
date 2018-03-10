using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Writes the current contents of the DataTable as XML
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
        /// Writes the current contents of the DataRow as XML
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
