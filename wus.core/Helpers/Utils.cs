using System;

namespace CoreWUS
{
    internal static class Utils
    {
        public static void CheckNullArgument(object arg, string name)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}