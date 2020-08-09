using System;

namespace CoreWUS
{
    public class WusException : SoapException
    {
        public string WusCode { get; private set; }
        public string WusMessage { get; private set; }

        public WusException(string message, string code, string actor,
                                string wusCode, string wusMessage)
            : base(message, code, actor)
        {
            WusCode = wusCode;
            WusMessage=wusMessage;
        }
    }
}