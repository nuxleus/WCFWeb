// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http
{
    using System;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    
    internal static class Utility
    {
        public const string ApplicationXml = "application/xml";
        public const string TextXml = "text/xml";
        public const string ApplicationJson = "application/json";
        public const string TextJson = "text/json";
        public const string GET = "GET";

        public static bool IsXmlContent(this string contentType)
        {
            if (contentType == null)
            {
                return true;
            }

            string contentTypeProcessed = contentType.Trim();

            return contentTypeProcessed.StartsWith(ApplicationXml, StringComparison.OrdinalIgnoreCase)
                || contentTypeProcessed.StartsWith(TextXml, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsJsonContent(this string contentType)
        {
            if (contentType == null)
            {
                return true;
            }

            string contentTypeProcessed = contentType.Trim();

            return contentTypeProcessed.StartsWith(ApplicationJson, StringComparison.OrdinalIgnoreCase)
                || contentTypeProcessed.StartsWith(TextJson, StringComparison.OrdinalIgnoreCase);
        }

        public static string CombineUri(string former, string latter)
        {
            // Appending the latter string to the form string,
            // while making sure there is a single slash char seperating the latter and the former.
            // This method behaves differently than new Uri(baseUri, relativeUri)
            // as CombineUri simply appends, whereas new Uri() actually replaces the last segment
            // of the its base path with the relative uri.
            StringBuilder builder = new StringBuilder();
            if (former.Length > 0 && latter.Length > 0)
            {
                if (former[former.Length - 1] == '/' && latter[0] == '/')
                {
                    builder.Append(former, 0, former.Length - 1);
                    builder.Append(latter);
                    return builder.ToString();
                }

                if (former[former.Length - 1] != '/' && latter[0] != '/')
                {
                    builder.Append(former);
                    builder.Append('/');
                    builder.Append(latter);
                    return builder.ToString();
                }
            }

            return former + latter;
        }

        public static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                if (exception.GetType().Name == "FatalException" ||
                    (exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) ||
                    exception is ThreadAbortException ||
                    exception.GetType().Name == "FatalInternalException")
                {
                    return true;
                }

                // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                // count as fatal.
                if (exception is TypeInitializationException ||
                    exception is TargetInvocationException)
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
    }
}
