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
    public static class ObjectExtension
    {
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
                DataTable dataTable = (DataTable)Convert.ChangeType(value, type);
                var stringWriter = new StringWriter();
                dataTable.WriteXml(stringWriter);
                return stringWriter.ToString();
            }

            if (type == typeof(DataRow))
            {
                DataRow dataRow = (DataRow)Convert.ChangeType(value, type);
                DataTable dataTable = new DataTable();
                dataTable = dataRow.Table.Clone();
                dataTable.ImportRow(dataRow);
                return dataTable.ToXML();
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

        public static void RemoveNilTrue(this XElement value)
        {
            XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
            XName n = ns + "nil";
            value.DescendantsAndSelf().Where(x => x.Attribute(n) != null && x.Attribute(n).Value == "true").Remove();
        }

        public static Stream ToStream(this string s)
        {
            byte[] resultBytes = Encoding.UTF8.GetBytes(s);
            return new MemoryStream(resultBytes);
        }

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

        public static dynamic DeserializeXML<T>(this string xml)
        {
            return DeserializeXML(xml, typeof(T));
        }

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
