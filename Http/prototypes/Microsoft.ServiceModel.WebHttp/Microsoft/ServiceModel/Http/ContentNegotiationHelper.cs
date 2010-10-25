// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;
    using System.ServiceModel;

    public static class ContentNegotiationHelper
    {
        private static string acceptHeaderWhenHeaderElementsCached;
        private static Collection<ContentType> cachedAcceptHeaderElements;

        public static ContentType GetBestMatch(string acceptHeader, IEnumerable<string> availableContentTypes)
        { 
            if (availableContentTypes == null)
            {
                throw new ArgumentNullException("availableContentTypes");
            }

            if (!availableContentTypes.Any())
            {
                return null;
            }

            var acceptHeaderElements = GetAcceptHeaderElements(acceptHeader);
            foreach (var element in acceptHeaderElements)
            {
                foreach (var availableContentType in availableContentTypes)
                {
                    if (availableContentType != null && element.MediaType.Contains(availableContentType))
                    {
                        return element;
                    }
                }
            }

            return new ContentType(availableContentTypes.FirstOrDefault());
        }

        // This method extracts substrings from an HTTP header starting at the offset
        // and up until the next comma in the header.  The sub string extraction is 
        // quote aware such that commas inside quoted-strings are ignored.  On return, 
        // offset points to the next char beyond the comma of the substring returned 
        // and may point beyond the length of the header.
        internal static string QuoteAwareSubString(string header, ref int offset)
        {
            // this method will filter out empty-string and white-space-only items in 
            // the header.  For example "x,,y" and "x, ,y" would result in just "x" and "y"
            // substrings being returned.
            if (string.IsNullOrEmpty(header) || offset >= header.Length)
            {
                return null;
            }

            int startIndex = (offset > 0) ? offset : 0;

            // trim whitespace and commas from the begining of the item
            while (char.IsWhiteSpace(header[startIndex]) || header[startIndex] == ',')
            {
                startIndex++;
                if (startIndex >= header.Length)
                {
                    return null;
                }
            }

            int endIndex = startIndex;
            bool insideQuotes = false;

            while (endIndex < header.Length)
            {
                if (header[endIndex] == '\"' &&
                   (!insideQuotes || endIndex == 0 || header[endIndex - 1] != '\\'))
                {
                    insideQuotes = !insideQuotes;
                }
                else if (header[endIndex] == ',' && !insideQuotes)
                {
                    break;
                }

                endIndex++;
            }

            offset = endIndex + 1;

            // trim whitespace from the end of the item; the substring is guaranteed to
            // have at least one non-whitespace character
            while (char.IsWhiteSpace(header[endIndex - 1]))
            {
                endIndex--;
            }

            return header.Substring(startIndex, endIndex - startIndex);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "by design")]
        internal static List<string> QuoteAwareStringSplit(string header)
        {
            List<string> subStrings = new List<string>();
            int offset = 0;
            while (true)
            {
                string subString = QuoteAwareSubString(header, ref offset);
                if (subString == null)
                {
                    break;
                }

                subStrings.Add(subString);
            }

            return subStrings;
        }

        private static Collection<ContentType> GetAcceptHeaderElements(string acceptHeader)
        {
            if (cachedAcceptHeaderElements == null ||
                (!string.Equals(acceptHeaderWhenHeaderElementsCached, acceptHeader, StringComparison.OrdinalIgnoreCase)))
            {
                if (string.IsNullOrEmpty(acceptHeader))
                {
                    cachedAcceptHeaderElements = new Collection<ContentType>();
                    acceptHeaderWhenHeaderElementsCached = acceptHeader;
                }
                else
                {
                    List<ContentType> contentTypeList = new List<ContentType>();
                    int offset = 0;
                    while (true)
                    {
                        string nextItem = QuoteAwareSubString(acceptHeader, ref offset);
                        if (nextItem == null)
                        {
                            break;
                        }

                        ContentType contentType = GetContentTypeOrNull(nextItem);
                        if (contentType != null)
                        {
                            contentTypeList.Add(contentType);
                        }
                    }

                    contentTypeList.Sort(new AcceptHeaderElementComparer());
                    cachedAcceptHeaderElements = new Collection<ContentType>(contentTypeList);
                    acceptHeaderWhenHeaderElementsCached = acceptHeader;
                }
            }

            return cachedAcceptHeaderElements;
        }

        private static ContentType GetContentTypeOrNull(string contentType)
        {
            try
            {
                // Fx.Assert(contentType == contentType.Trim(), "The ContentType input argument should already be trimmed.");
                // Fx.Assert(!string.IsNullOrEmpty(contentType), "The ContentType input argument should not be null or empty.");
                ContentType contentTypeToReturn = new ContentType(contentType);

                // Need to check for "*/<Something-other-than-*>" because the ContentType constructor doesn't catch this
                string[] typeAndSubType = contentTypeToReturn.MediaType.Split('/');

                // Fx.Assert(typeAndSubType.Length == 2, "The creation of the ContentType would have failed if there wasn't a type and subtype.");
                if (typeAndSubType[0][0] == '*' && typeAndSubType[0].Length == 1 &&
                    !(typeAndSubType[1][0] == '*' && typeAndSubType[1].Length == 1))
                {
                    // TODO: Opened bug #130861 to enable this exception but right now adding the resource string
                    //  doesn't meet the ssk mode bar for RC since we're just going to swallow it 
                    //  and use it for tracing.

                    // throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new FormatException(
                    // SR2.GetString(SR2.InvalidContentType, contentType)));
                    return null;
                }

                return contentTypeToReturn;
            }
            catch (FormatException)
            {
                // Return null to indicate that the content type creation failed
                // System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(e, TraceEventType.Warning);
            }

            return null;
        }

        private class AcceptHeaderElementComparer : IComparer<ContentType>
        {
            private static NumberStyles numberStyles = NumberStyles.AllowDecimalPoint;

            public int Compare(ContentType x, ContentType y)
            {
                if (x == null)
                {
                    throw new ArgumentNullException("x");
                }

                if (y == null)
                {
                    throw new ArgumentNullException("x");
                }

                string[] typeSubType_x = x.MediaType.Split('/');
                string[] typeSubType_y = y.MediaType.Split('/');

                if (string.Equals(typeSubType_x[0], typeSubType_y[0], StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(typeSubType_x[1], typeSubType_y[1], StringComparison.OrdinalIgnoreCase))
                    {
                        // need to check the number of parameters to determine which is more specific
                        bool hasParamx = HasParameters(x);
                        bool hasParamy = HasParameters(y);
                        if (hasParamx && !hasParamy)
                        {
                            return 1;
                        }
                        else if (!hasParamx && hasParamy)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (typeSubType_x[1][0] == '*' && typeSubType_x[1].Length == 1)
                        {
                            return 1;
                        }

                        if (typeSubType_y[1][0] == '*' && typeSubType_y[1].Length == 1)
                        {
                            return -1;
                        }
                    }
                }
                else if (typeSubType_x[0][0] == '*' && typeSubType_x[0].Length == 1)
                {
                    return 1;
                }
                else if (typeSubType_y[0][0] == '*' && typeSubType_y[0].Length == 1)
                {
                    return -1;
                }

                decimal qualityDifference = GetQualityFactor(x) - GetQualityFactor(y);
                if (qualityDifference < 0)
                {
                    return 1;
                }
                else if (qualityDifference > 0)
                {
                    return -1;
                }

                return 0;
            }

            private static decimal GetQualityFactor(ContentType contentType)
            {
                decimal result;
                foreach (string key in contentType.Parameters.Keys)
                {
                    if (string.Equals("q", key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (decimal.TryParse(contentType.Parameters[key], numberStyles, CultureInfo.InvariantCulture, out result) &&
                            (result <= (decimal)1.0))
                        {
                            return result;
                        }
                    }
                }

                return (decimal)1.0;
            }

            private static bool HasParameters(ContentType contentType)
            {
                int number = 0;
                foreach (string param in contentType.Parameters.Keys)
                {
                    if (!string.Equals("q", param, StringComparison.OrdinalIgnoreCase))
                    {
                        number++;
                    }
                }

                return number > 0;
            }
        }
    }
}
