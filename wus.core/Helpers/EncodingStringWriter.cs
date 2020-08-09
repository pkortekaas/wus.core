using System.IO;
using System.Text;

namespace CoreWUS
{
    internal class EncodingStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public override Encoding Encoding => _encoding;

        public EncodingStringWriter(Encoding encoding)
        {
            _encoding = encoding;
        }
    }
}