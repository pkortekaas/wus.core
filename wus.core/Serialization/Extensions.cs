using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CoreWUS.Extensions
{
    public static class WusExtensions
    {
        public static string ToXml<T>(this T value, XmlWriterSettings settings) where T: class
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                Encoding encoding = settings?.Encoding ?? Encoding.UTF8;
                using (EncodingStringWriter sw = new EncodingStringWriter(encoding))
                {
                    using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    {
                        XmlSerializerNamespaces xns = null;
                        // Suppress xsi and xsd namespaces and provide only default namespace
                        XmlRootAttribute xmlRootAttribute = typeof(T).GetCustomAttribute<XmlRootAttribute>(false);
                        if (xmlRootAttribute != null)
                        {
                            string ns = xmlRootAttribute.Namespace ?? string.Empty;
                            xns = new XmlSerializerNamespaces();
                            xns.Add(string.Empty, ns);
                        }
                        xmlserializer.Serialize(writer, value, xns);
                    }
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static XElement ToXElement<T>(this T value, XmlWriterSettings settings) where T: class
        {
            if (value == null)
            {
                return null;
            }
            return XElement.Parse(value.ToXml(settings));
        }

        public static XmlDocument ToXmlDocument<T>(this T value) where T: XDocument
        {
            if (value == null)
            {
                return null;
            }
            XmlDocument document = new XmlDocument { PreserveWhitespace = true };
            document.LoadXml(value.ToString(SaveOptions.DisableFormatting));
            return document;
        }

        public static bool IsSuccessStatusCode<T>(this T value) where T: HttpWebResponse
        {
            Contract.Requires(value != null);
            return ((int)value.StatusCode >= 200) && ((int)value.StatusCode <= 299);
        }
    }
}