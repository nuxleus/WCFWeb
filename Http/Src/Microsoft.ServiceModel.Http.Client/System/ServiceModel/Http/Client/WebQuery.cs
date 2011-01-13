// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.QueryComposition.Client;

    /// <summary>
    /// This class will be used to capture the IQueryable list returned from the HttpClient.CreateQuery 
    /// method, where people can compose query further. 
    /// </summary>
    /// <typeparam name="T">The type of the item in the returning list</typeparam>
    public class WebQuery<T> : IQueryable<T>
    {
        private Expression expression;
        private WebQueryProvider provider;
        private Uri requestUri;

        internal WebQuery(HttpClient client, Uri relativeAddress, IExpressionToUriConverter converter)
        {           
            string resourceBaseAddress = null;
            string resourceFullAddress = Utility.CombineUri(client.BaseAddress.AbsoluteUri, relativeAddress.OriginalString);
            string resourceName = ExtractResourceName(resourceFullAddress, out resourceBaseAddress);

            this.expression = new ResourceSetExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(resourceName), typeof(T), null, CountOption.None, null, null);
            this.provider = new WebQueryProvider(resourceBaseAddress, client, converter);
        }

        internal WebQuery(Expression expression, WebQueryProvider provider)
        {
            this.provider = provider;
            this.expression = expression;
        }

        /// <summary>
        /// Gets the query provider implementation
        /// </summary>
        public IQueryProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        /// <summary>
        /// Gets the linq expression 
        /// </summary>
        public Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Gets the type of the item in the list
        /// </summary>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets the request Uri from the request message
        /// </summary>
        public Uri RequestUri
        {
            get
            {
                // we can cache the request uri because a WebQuery is immutable
                if (this.requestUri == null)
                {
                    this.requestUri = this.provider.GetRequestUri(this.expression);
                }

                return this.requestUri;
            }
        }

        /// <summary>
        /// Executes the query by sending a request to the server and return the enumerator of the result list
        /// </summary>
        /// <returns>The enumerator of the result list</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Execute().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Executes the query by sending a request to the server
        /// </summary>
        /// <returns>A list of the objects</returns>
        public IEnumerable<T> Execute()
        {
            return this.provider.ExecuteInternal<T>(this.CreateRequestMessage());
        }

        /// <summary>
        /// The end part of async implementation of execute method
        /// </summary>
        /// <returns>A task which contains the result</returns>
        public Task<IEnumerable<T>> ExecuteAsync()
        {
            return this.provider.ExecuteInternalAsync<T>(this.CreateRequestMessage());
        }

        internal HttpRequestMessage CreateRequestMessage()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = this.RequestUri,
                Method = HttpMethod.Get
            };
            return requestMessage;
        }

        // the method below extracts the name of the resource from the relative address,
        // baseUri contains the relative path to the resource minus the name of the resource
        private static string ExtractResourceName(string relativeUri, out string baseUri)
        {
            int begin = 0, end;

            end = relativeUri.Length - 1;
            if (relativeUri[end] == '/')
            {
                end--;
            }

            for (int i = end; i >= 0; i--)
            {
                if (relativeUri[i] == '/')
                {
                    begin = i + 1;
                    break;
                }
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < begin; i++)
            {
                sb.Append(relativeUri[i]);
            }

            baseUri = sb.ToString();

            sb.Clear();
            for (int i = begin; i <= end; i++)
            {
                sb.Append(relativeUri[i]);
            }

            return sb.ToString();
        }

        internal class WebOrderedQuery : WebQuery<T>, IOrderedQueryable<T>
        {
            internal WebOrderedQuery(HttpClient client, Uri resourceAddress, IExpressionToUriConverter converter)
                : base(client, resourceAddress, converter)
            {
            }

            internal WebOrderedQuery(Expression expression, WebQueryProvider provider)
                : base(expression, provider)
            {
            }
        }
    }
}
