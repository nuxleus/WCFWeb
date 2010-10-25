// <copyright file="DiagnosticUtility.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System.Globalization;

    internal static class DiagnosticUtility
    {
        public static string GetString(string format, params object[] args)
        {
            CultureInfo culture = SR.Culture;
            string text = format;
            if (args != null && args.Length > 0)
            {
                text = String.Format(culture, format, args);
            }

            return text;
        }

        internal static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                if (exception is OutOfMemoryException ||
                    exception is AccessViolationException)
                {
                    return true;
                }

                // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                // count as fatal.
                if (exception is TypeInitializationException)
                {
                    exception = exception.InnerException;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        internal static class ExceptionUtility
        {
            public static Exception ThrowHelperError(Exception e)
            {
                return e;
            }

            internal static void ThrowOnNull(object obj, string parameterName)
            {
                if (obj == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(parameterName));
                }
            }

            internal static void ThrowOnDefaultArg(JsonValue value)
            {
                if (value != null && value.JsonType == JsonType.Default)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.UseOfDefaultNotAllowed));
                }
            }

            internal static void ThrowOnDefaultInstance(JsonValue instance)
            {
                if (instance != null && instance.JsonType == JsonType.Default)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.OperationNotAllowedOnDefault));
                }
            }
        }
    }
}
