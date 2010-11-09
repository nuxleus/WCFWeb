// <copyright file="SRCompat.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>.

namespace System.Runtime.Serialization
{
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    internal partial class SR
    {
        public static string GetString(string format, params object[] args)
        {
            global::System.Globalization.CultureInfo culture = SR.Culture;
            string text = format;
            if (args != null && args.Length > 0)
            {
                text = String.Format(culture, format, args);
            }

            return text;
        }
    }
}
