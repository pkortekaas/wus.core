using System;

namespace CoreWUS
{
    public sealed class WusException : SoapException
    {
        public string WusCode { get; private set; }
        public string WusMessage { get; private set; }

        public WusException() : base() {}
        public WusException(string message) : base(message) {}
        public WusException(string message, Exception innerException) : base(message, innerException) {}

        public WusException(string message, string code, string actor,
                                string wusCode, string wusMessage)
            : base(message, code, actor)
        {
            WusCode = wusCode;
            WusMessage=wusMessage;
        }
    }
}