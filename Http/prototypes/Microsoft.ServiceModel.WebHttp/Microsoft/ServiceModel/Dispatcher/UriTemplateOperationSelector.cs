// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Channels;

    public class UriTemplateOperationSelector : HttpOperationSelector
    {
        private static readonly string stringTypeFullName = typeof(string).FullName;

        private UriTemplateTable uriTemplateTable;

        public UriTemplateOperationSelector(UriTemplateTable uriTemplateTable)
        {
            if (uriTemplateTable == null)
            {
                throw new ArgumentNullException("uriTemplateTable");
            }

            this.uriTemplateTable = uriTemplateTable;
            this.UseMatchSingle = true;
        }

        public bool UseMatchSingle { get; set; }

        public string DefaultOperationName { get; set; }

        public override string SelectOperation(HttpRequestMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            Uri uri = message.RequestUri;
            
            if (uri == null)
            {
                return this.OnMatchNotFound(message);
            }

            UriTemplateMatch match = null;
            if (this.UseMatchSingle)
            {
                match = this.uriTemplateTable.MatchSingle(uri);
                if (match == null)
                {
                    uri = new Uri(uri.OriginalString+"/");
                    match = this.uriTemplateTable.MatchSingle(uri);
                }
            }
            else
            {
                Collection<UriTemplateMatch> matches = this.uriTemplateTable.Match(uri);
                if (matches.Count > 1)
                {
                    match = this.OnMultipleMatchesFound(matches);
                }
                else if (matches.Count == 1)
                {
                    match = matches[0];
                }
            }

            if (match == null)
            {
                return this.OnMatchNotFound(message);
            }

            return this.OnMatchFound(match, message);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UriTemplateMatch", Justification = "Breaking change")]
        protected virtual string OnMatchFound(UriTemplateMatch match, HttpRequestMessage message)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (match.Data == null)
            {
                return string.Empty;
            }

            string operationName = match.Data as string;
            if (operationName == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The UriTemplateMatch data is expected to be of type '{0}' but is of type '{1}'.",
                        match.Data.GetType().FullName,
                        stringTypeFullName));
            }

            message.GetProperties().Add(match);

            return operationName;
        }

        protected virtual UriTemplateMatch OnMultipleMatchesFound(
                                               Collection<UriTemplateMatch> matches)
        {
            if (matches == null)
            {
                throw new ArgumentNullException("matches");
            }

            return matches.FirstOrDefault();
        }

        protected virtual string OnMatchNotFound(HttpRequestMessage message)
        {
            return this.DefaultOperationName ?? string.Empty;
        }
    }
}
