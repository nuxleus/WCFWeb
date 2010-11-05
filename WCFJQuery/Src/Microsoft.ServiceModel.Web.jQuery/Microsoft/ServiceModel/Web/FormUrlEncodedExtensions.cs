// <copyright file="FormUrlEncodedExtensions.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Json;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Web;

    /// <summary>
    /// This class provides helper methods for decoding form url-encoded strings.
    /// </summary>
    public static class FormUrlEncodedExtensions
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
            return FormUrlEncodedExtensions.Parse(queryStringValues, maxDepth);
        }

               internal static JsonObject Parse(NameValueCollection nvc, int maxDepth)
        {
            JsonObject result = new JsonObject();
            foreach (string key in nvc.Keys)
            {
                if (key == null)
                {
                    foreach (string value in nvc.GetValues(null))
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.QueryStringNameShouldNotNull), "nvc"));
                        }

                        string[] path = new string[] { value };
                        Insert(result, path, null);
                    }
                }
                else
                {
                    string[] path = GetPath(key, maxDepth);
                    Insert(result, path, nvc.GetValues(key));
                }
            }

            FixContiguousArrays(result);
            return result;
        }

        private static string[] GetPath(string key, int maxDepth)
        {
            Debug.Assert(key != null, "Key cannot be null (this function is only called by Parse if key != null)");

            if (string.IsNullOrEmpty(key))
            {
                return new string[] { string.Empty };
            }

            ValidateQueryString(key);
            string[] path = key.Split('[');
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i].EndsWith("]", StringComparison.Ordinal))
                {
                    path[i] = path[i].Substring(0, path[i].Length - 1);
                }
            }

            // For consistency with JSON, the depth of a[b]=1 is 3 (which is the depth of {a:{b:1}}, given
            // that in the JSON-XML mapping there's a <root> element wrapping the JSON object:
            // <root><a><b>1</b></a></root>. So if the length of the path is greater than *or equal* to
            // maxDepth, then we throw.
            if (path.Length >= maxDepth)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.MaxDepthExceeded, maxDepth)));
            }

            return path;
        }

        private static void ValidateQueryString(string key)
        {
            bool hasUnMatchedLeftBraket = false;
            for (int i = 0; i < key.Length; i++)
            {
                switch (key[i])
                {
                    case '[':
                        if (!hasUnMatchedLeftBraket)
                        {
                            hasUnMatchedLeftBraket = true;
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.NestedBracketNotValid, i)));
                        }

                        break;
                    case ']':
                        if (hasUnMatchedLeftBraket)
                        {
                            hasUnMatchedLeftBraket = false;
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.UnMatchedBracketNotValid, i)));
                        }

                        break;
                }
            }

            if (hasUnMatchedLeftBraket)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.NestedBracketNotValid, key.LastIndexOf('['))));
            }
        }

        private static void Insert(JsonObject root, string[] path, string[] values)
        {
            if (values == null)
            {
                Debug.Assert(path.Length == 1, "This should only be hit in the case of only a value-only query part");
                if (root.ContainsKey(path[0]) && root[path[0]] != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.FormUrlEncodedMismatchingTypes, BuildPathString(path, 0))));
                }

                root[path[0]] = null;
            }
            else
            {
                foreach (string value in values)
                {
                    JsonValue current = root;
                    JsonObject parent = null;

                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        if (String.IsNullOrEmpty(path[i]))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.InvalidArrayInsert, BuildPathString(path, i))));
                        }

                        JsonObject jo = current as JsonObject;

                        if (jo != null && jo.ContainsKey(path[i]))
                        {
                            if (jo[path[i]] == null)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.FormUrlEncodedMismatchingTypes, BuildPathString(path, i))));
                            }
                        }
                        else
                        {
                            current[path[i]] = new JsonObject();
                        }

                        parent = jo;
                        current = current[path[i]];
                    }

                    string lastKey = path[path.Length - 1];
                    if (string.IsNullOrEmpty(lastKey) && path.Length > 1)
                    {
                        AddToArray(parent, path, value);
                    }
                    else
                    {
                        JsonObject jo = current as JsonObject;
                        if (jo == null)
                        {
                            int pathIndex = path.Length - 1;
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.FormUrlEncodedMismatchingTypes, BuildPathString(path, pathIndex))));
                        }

                        AddToObject(jo, path, value);
                    }
                }
            }
        }

        private static void AddToObject(JsonObject obj, string[] path, string value)
        {
            int pathIndex = path.Length - 1;
            string key = path[pathIndex];

            if (obj.ContainsKey(key))
            {
                if (obj[key] == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.FormUrlEncodedMismatchingTypes, BuildPathString(path, pathIndex))));
                }

                bool isRoot = path.Length == 1;
                if (isRoot)
                {
                    // jQuery 1.3 behavior, make it into an array if primitive
                    if (obj[key].JsonType == JsonType.String)
                    {
                        obj[key] = new JsonArray(obj[key], value);
                    }
                    else if (obj[key].JsonType == JsonType.Array)
                    {
                        // if it was already an array, simply add the value
                        ((JsonArray)obj[key]).Add(value);
                    }
                    else
                    {
                        // if it was an object, it's a bad input: a[b]=1&a=5
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.FormUrlEncodedMismatchingTypes, BuildPathString(path, pathIndex))));
                    }
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.JQuery13CompatModeNotSupportNestedJson, BuildPathString(path, pathIndex))));
                }
            }
            else
            {
                // if the object didn't contain the key, simply add it now
                obj[key] = value;
            }
        }

        private static void AddToArray(JsonObject parent, string[] path, string value)
        {
            Debug.Assert(path.Length >= 2, "The path must be at least 2, one for the ending [], and one for before the '[' (which can be empty)");

            string parentPath = path[path.Length - 2];

            Debug.Assert(parent.ContainsKey(parentPath), "It was added on insert to get to this point");

            JsonObject jo = parent[parentPath] as JsonObject;
            if (jo != null && jo.Count == 0)
            {
                // everything starts as an object, but if it's supposed to be
                // an array then we will turn it into one.
                parent[parentPath] = new JsonArray();
            }

            JsonArray array = parent[parentPath] as JsonArray;
            if (array == null)
            {
                // a[b][c]=1&a[b][]=2 => invalid
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.FormUrlEncodedMismatchingTypes, BuildPathString(path, path.Length - 1))));
            }
            else
            {
                array.Add(value);
            }
        }

        private static void FixContiguousArrays(JsonValue jv)
        {
            JsonArray ja = jv as JsonArray;

            if (ja != null)
            {
                for (int i = 0; i < ja.Count; i++)
                    {
                        if (ja[i] != null)
                        {
                            ja[i] = FixSingleContiguousArray(ja[i]);
                            FixContiguousArrays(ja[i]);
                        }
                    }
            }
            else
            { 
                JsonObject jo = jv as JsonObject;
                
                if (jo != null)
                {
                    List<string> keys = new List<string>(jo.Keys);
                    foreach (string key in keys)
                    {
                        if (jo[key] != null)
                        {
                            jo[key] = FixSingleContiguousArray(jo[key]);
                            FixContiguousArrays(jo[key]);
                        }
                    }
                }
            }

            //// do nothing for primitives
        }

        private static JsonValue FixSingleContiguousArray(JsonValue original)
        {
            JsonObject jo = original as JsonObject;
            if (jo != null && jo.Count > 0)
            {
                List<string> childKeys = new List<string>(jo.Keys);
                if (CanBecomeArray(jo, childKeys))
                {
                    JsonArray newResult = new JsonArray();
                    for (int i = 0; i < childKeys.Count; i++)
                    {
                        newResult.Add(jo[i.ToString(CultureInfo.InvariantCulture)]);
                    }

                    return newResult;
                }
            }

            return original;
        }

        private static bool CanBecomeArray(JsonObject obj, List<string> keys)
        {
            List<int> intKeys = new List<int>();
            bool areContiguousIndices = true;
            foreach (string key in keys)
            {
                int intKey;
                if (!int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out intKey))
                {
                    // if not a number, it cannot become an array
                    areContiguousIndices = false;
                    break;
                }
                else if (obj[key].JsonType == JsonType.String)
                {
                    // we should only convert non-leaf nodes, to differentiate between
                    // {a:[1]} => a[]=1 and {a:{"0":1}} => a[0]=1
                    areContiguousIndices = false;
                    break;
                }

                intKeys.Add(intKey);
            }

            if (areContiguousIndices)
            {
                intKeys.Sort();

                for (int i = 0; i < intKeys.Count; i++)
                {
                    if (intKeys[i] != i)
                    {
                        areContiguousIndices = false;
                        break;
                    }
                }
            }

            return areContiguousIndices;
        }

        private static string BuildPathString(string[] path, int i)
        {
            StringBuilder errorPath = new StringBuilder(path[0]);
            for (int p = 1; p <= i; p++)
            {
                errorPath.AppendFormat("[{0}]", path[p]);
            }

            return errorPath.ToString();
        }
    }
}
