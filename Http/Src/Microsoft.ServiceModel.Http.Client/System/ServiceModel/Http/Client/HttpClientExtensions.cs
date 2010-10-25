// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Client
{
    using System;
    using Microsoft.Http;

    /// <summary>
    /// This class is used to extend the existing HttpClient class so that user can start
    /// using the HttpClient proxy to Query the service
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// This method create a WebQuery so that you can further construct the linq expression against 
        /// a list of objects
        /// </summary>
        /// <typeparam name="T">The type of the object in the list</typeparam>
        /// <param name="client">The http client proxy</param>
        /// <returns>The web query which implements the IQueryable</returns>
        public static WebQuery<T> CreateQuery<T>(this HttpClient client)
        {
            return CreateQuery<T>(client, new Uri(string.Empty, UriKind.Relative), null);
        }

        /// <summary>
        /// This method create a WebQuery so that you can further construct the linq expression against 
        /// a list of objects
        /// </summary>
        /// <typeparam name="T">The type of the object in the list</typeparam>
        /// <param name="client">The http client proxy</param>
        /// <param name="relativeAddress">The relative address that represents the resource</param>
        /// <returns>The web query which implements the IQueryable</returns>
        public static WebQuery<T> CreateQuery<T>(this HttpClient client, string relativeAddress)
        {
            return CreateQuery<T>(client, relativeAddress, null);
        }

        /// <summary>
        /// This method create a WebQuery so that you can further construct the linq expression against 
        /// a list of objects
        /// </summary>
        /// <typeparam name="T">The type of the object in the list</typeparam>
        /// <param name="client">The http client proxy</param>
        /// <param name="relativeAddress">The relative address that represents the resource</param>
        /// <param name="converter">The custom expression to uri converter</param>
        /// <returns>The web query which implements the IQueryable</returns>
        public static WebQuery<T> CreateQuery<T>(this HttpClient client, string relativeAddress, IExpressionToUriConverter converter)
        {
            Uri relativeUri = new Uri(relativeAddress, UriKind.Relative);
            return CreateQuery<T>(client, relativeUri, converter);
        }

        /// <summary>
        /// This method create a WebQuery so that you can further construct the linq expression against 
        /// a list of objects
        /// </summary>
        /// <typeparam name="T">The type of the object in the list</typeparam>
        /// <param name="client">The http client proxy</param>
        /// <param name="relativeUri">The relative address Uri that represents the resource</param>
        /// <returns>The web query which implements the IQueryable</returns>
        public static WebQuery<T> CreateQuery<T>(this HttpClient client, Uri relativeUri)
        {
            return CreateQuery<T>(client, relativeUri, null);
        }

        /// <summary>
        /// This method create a WebQuery so that you can further construct the linq expression against 
        /// a list of objects
        /// </summary>
        /// <typeparam name="T">The type of the object in the list</typeparam>
        /// <param name="client">The http client proxy</param>
        /// <param name="relativeUri">The relative address Uri that represents the resource</param>
        /// <param name="converter">The custom expression to uri converter</param>
        /// <returns>The web query which implements the IQueryable</returns>
        public static WebQuery<T> CreateQuery<T>(this HttpClient client, Uri relativeUri, IExpressionToUriConverter converter)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (relativeUri == null)
            {
                throw new ArgumentNullException("relativeUri");
            }

            if (relativeUri.IsAbsoluteUri)
            {
                throw new InvalidOperationException(string.Format(SR.QueryResourceUriMustBeRelative, relativeUri.AbsoluteUri));
            }

            if (converter == null)
            {
                converter = WebExpressionConverter.Instance;
            }

            return new WebQuery<T>.WebOrderedQuery(client, relativeUri, converter);
        }
    }
}
