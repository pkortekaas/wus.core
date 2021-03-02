using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CoreWUS.Serialization
{
    public static class Serializer
    {
        [SuppressMessage("Microsoft.CodeAnalysis.FxCopAnalyzers", "CA5369:PotentialUnsafeOverload")]
        public static T Deserialize<T>(string input) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static T Deserialize<T>(XmlReader reader, XmlRootAttribute rootAttribute = null) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T), rootAttribute);
            return (T)ser.Deserialize(reader);
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}