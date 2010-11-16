namespace Microsoft.Silverlight.Cdf.Test.Common.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    public static class CreatorSettings
    {
        static CreatorSettings()
        {
            MaxStringLength = 100;
            CreateOnlyAsciiChars = false;
            NullValueProbability = 0.01;
        }

        public static int MaxStringLength { get; set; }

        public static bool CreateOnlyAsciiChars { get; set; }

        public static double NullValueProbability { get; set; }
    }

    public static class PrimitiveCreator
    {
        public static string CreateRandomString(Random rndGen, int size, string charsToUse)
        {
            int maxSize = CreatorSettings.MaxStringLength;
            
            // invalid per the XML spec (http://www.w3.org/TR/REC-xml/#charsets), cannot be sent as XML
            string invalidXmlChars = "\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u000B\u000C\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F\uFFFE\uFFFF";

            const int LowSurrogateMin = 0xDC00;
            const int LowSurrogateMax = 0xDFFF;
            const int HighSurrogateMin = 0xD800;
            const int HighSurrogateMax = 0xDBFF;

            if (size < 0)
            {
                double rndNumber = rndGen.NextDouble();
                if (rndNumber < CreatorSettings.NullValueProbability)
                {
                    return null; // 1% chance of null value
                }

                size = (int)Math.Pow(maxSize, rndNumber); // this will create more small strings than large ones
                size--;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char c;
                if (charsToUse != null)
                {
                    c = charsToUse[rndGen.Next(charsToUse.Length)];
                    sb.Append(c);
                }
                else
                {
                    if (CreatorSettings.CreateOnlyAsciiChars || rndGen.Next(2) == 0)
                    {
                        c = (char)rndGen.Next(0x20, 0x7F); // low-ascii chars
                        sb.Append(c);
                    }
                    else
                    {
                        do
                        {
                            c = (char)rndGen.Next((int)char.MinValue, (int)char.MaxValue + 1);
                        }
                        while ((LowSurrogateMin <= c && c <= LowSurrogateMax) || (invalidXmlChars.IndexOf(c) >= 0));

                        sb.Append(c);
                        if (HighSurrogateMin <= c && c <= HighSurrogateMax)
                        {
                            // need to add a low surrogate
                            c = (char)rndGen.Next(LowSurrogateMin, LowSurrogateMax + 1);
                            sb.Append(c);
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}
