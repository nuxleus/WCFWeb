// <copyright file="JsonValueExtensions.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Web
{
    using System.Collections.Specialized;
    using System.Json;
    using System.ServiceModel.Web;
    using System.Web;

    /// <summary>
    /// Extension methods for JsonValue / JsonObject classes.
    /// </summary>
    public static class JsonValueExtensions
    {
        /// <summary>
        /// Returns the query string from the incoming web context as a <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        /// <param name="context">The <see cref="System.ServiceModel.Web.IncomingWebRequestContext"/> instance
        /// where the query string can be retrieved.</param>
        /// <returns>The query string parsed as a <see cref="System.Json.JsonObject"/> instance.</returns>
        /// <remarks>The main usage of this extension method is to retrieve the query string within
        /// an operation using the System.ServiceModel.Web.WebOperationContext.Current.IncomingContext object.
        /// The query string is parsed as x-www-form-urlencoded data.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Call to DiagnosticUtility validates the parameter.")]
        public static JsonObject GetQueryStringAsJsonObject(this IncomingWebRequestContext context)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(context, "context");

            NameValueCollection query = context.UriTemplateMatch.QueryParameters;
            return ParseFormUrlEncoded(query);
        }

        /// <summary>
        /// Parses a query string (x-www-form-urlencoded) as a <see cref="System.Json.JsonObject"/>.
        /// </summary>
        /// <param name="queryString">The query string to be parsed.</param>
        /// <returns>The <see cref="System.Json.JsonObject"/> corresponding to the given query string.</returns>
        public static JsonObject ParseFormUrlEncoded(string queryString)
        {
            return ParseFormUrlEncoded(queryString, int.MaxValue);
        }

        /// <summary>
        /// Parses a query string (x-www-form-urlencoded) as a <see cref="System.Json.JsonObject"/>.
        /// </summary>
        /// <param name="queryString">The query string to be parsed.</param>
        /// <param name="maxDepth">The maximum depth of object graph encoded as x-www-form-urlencoded.</param>
        /// <returns>The <see cref="System.Json.JsonObject"/> corresponding to the given query string.</returns>
        public static JsonObject ParseFormUrlEncoded(string queryString, int maxDepth)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(queryString, "queryString");
            return ParseFormUrlEncoded(HttpUtility.ParseQueryString(queryString), maxDepth);
        }

        /// <summary>
        /// Parses a collection of query string values as a <see cref="System.Json.JsonObject"/>.
        /// </summary>
        /// <param name="queryStringValues">The collection of query string values.</param>
        /// <returns>The <see cref="System.Json.JsonObject"/> corresponding to the given query string values.</returns>
        public static JsonObject ParseFormUrlEncoded(NameValueCollection queryStringValues)
        {
            return ParseFormUrlEncoded(queryStringValues, int.MaxValue);
        }

        /// <summary>
        /// Parses a collection of query string values as a <see cref="System.Json.JsonObject"/>.
        /// </summary>
        /// <param name="queryStringValues">The collection of query string values.</param>
        /// <param name="maxDepth">The maximum depth of object graph encoded as x-www-form-urlencoded.</param>
        /// <returns>The <see cref="System.Json.JsonObject"/> corresponding to the given query string values.</returns>
        public static JsonObject ParseFormUrlEncoded(NameValueCollection queryStringValues, int maxDepth)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(queryStringValues, "queryString");
            return FormUrlEncodedHelper.Parse(queryStringValues, maxDepth);
        }
    }
}
