using System;

namespace CoreWUS
{
    public class SoapException : SystemException
    {
        public string Actor { get; private set; }
        public string Code { get; private set; }

        public SoapException(string message, string code, string actor)
            : base(message)
        {
            Actor = actor;
            Code = code;
        }
    }
}