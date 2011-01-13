// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides extension methods for getting an <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> instance or
    /// an <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> instance from a <see cref="Message"/> instance and
    /// provides extension methods for creating a <see cref="Message"/> instance from either an 
    /// <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> instance or an 
    /// <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> instance.
    /// </summary>
    public static class HttpMessageExtensionMethods
    {
        internal const string ToHttpRequestMessageMethodName = "ToHttpRequestMessage";
        internal const string ToHttpResponseMessageMethodName = "ToHttpResponseMessage";
        internal const string ToMessageMethodName = "ToMessage";
        private static ConditionalWeakTable<HttpRequestMessage, List<object>> requestProperties = new ConditionalWeakTable<HttpRequestMessage, List<object>>();
        private static ConditionalWeakTable<HttpResponseMessage, List<object>> responseProperties = new ConditionalWeakTable<HttpResponseMessage, List<object>>();

        /// <summary>
        /// Returns a reference to the <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> 
        /// instance held by the given <see cref="Message"/> or null if the <see cref="Message"/> does not
        /// hold a refernce to an <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> 
        /// instance.
        /// </summary>
        /// <param name="message">The given <see cref="Message"/> that holds a reference to an 
        /// <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> instance.
        /// </param>
        /// <returns>
        /// A reference to the <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> 
        /// instance held by the given <see cref="Message"/> or null if the <see cref="Message"/> does not
        /// hold a refernce to an <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> 
        /// instance.
        /// </returns>
        public static HttpRequestMessage ToHttpRequestMessage(this Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            HttpMessage httpMessage = message as HttpMessage;
            if (httpMessage != null && httpMessage.IsRequest)
            {
                return httpMessage.GetHttpRequestMessage();
            }

            return null;
        }

        /// <summary>
        /// Returns a reference to the <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> 
        /// instance held by the given <see cref="Message"/> or null if the <see cref="Message"/> does not
        /// hold a refernce to an <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> 
        /// instance.
        /// </summary>
        /// <param name="message">The given <see cref="Message"/> that holds a reference to an 
        /// <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> instance.
        /// </param>
        /// <returns>
        /// A reference to the <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> 
        /// instance held by the given <see cref="Message"/> or null if the <see cref="Message"/> does not
        /// hold a refernce to an <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> 
        /// instance.
        /// </returns>
        public static HttpResponseMessage ToHttpResponseMessage(this Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            HttpMessage httpMessage = message as HttpMessage;
            if (httpMessage != null && !httpMessage.IsRequest)
            {
                return httpMessage.GetHttpResponseMessage();
            }

            return null;
        }

        /// <summary>
        /// Creates a new <see cref="Message"/> that holds a reference to the given 
        /// <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> instance.
        /// </summary>
        /// <param name="request">The <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> 
        /// instance to which the new <see cref="Message"/> should hold a reference.
        /// </param>
        /// <returns>A <see cref="Message"/> that holds a reference to the given
        /// <see cref="Microsoft.Http.HttpRequestMessage">HttpRequestMessage</see> instance.
        /// </returns>
        public static Message ToMessage(this HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return new HttpMessage(request);
        }

        /// <summary>
        /// Creates a new <see cref="Message"/> that holds a reference to the given
        /// <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> instance.
        /// </summary>
        /// <param name="response">The <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> 
        /// instance to which the new <see cref="Message"/> should hold a reference.
        /// </param>
        /// <returns>A <see cref="Message"/> that holds a reference to the given
        /// <see cref="Microsoft.Http.HttpResponseMessage">HttpResponseMessage</see> instance.
        /// </returns>
        public static Message ToMessage(this HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            return new HttpMessage(response);
        }

        public static IList<object> GetProperties(this HttpRequestMessage message)
        {
            return requestProperties.GetOrCreateValue(message);
        }

        public static IList<object> GetProperties(this HttpResponseMessage message)
        {
            return responseProperties.GetOrCreateValue(message);
        }

        internal static HttpRequestMessageProperty GetHttpRequestMessageProperty(this Message message)
        {
            object requestProperty = null;

            if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out requestProperty))
            {
                return requestProperty as HttpRequestMessageProperty;
            }

            return null;
        }

        internal static HttpResponseMessageProperty GetHttpResponseMessageProperty(this Message message)
        {
            object responseProperty = null;

            if (message.Properties.TryGetValue(HttpResponseMessageProperty.Name, out responseProperty))
            {
                return responseProperty as HttpResponseMessageProperty;
            }

            return null;
        }
    }
}
