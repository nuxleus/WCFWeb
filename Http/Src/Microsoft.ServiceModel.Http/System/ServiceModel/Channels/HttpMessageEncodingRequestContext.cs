// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    internal class HttpMessageEncodingRequestContext : RequestContext
    {
        private RequestContext innerContext;
        private Message configuredRequestMessage;
        private bool isRequestConfigured;
        private object requestConfigurationLock;

        public HttpMessageEncodingRequestContext(RequestContext innerContext)
        {
            Debug.Assert(innerContext != null, "The 'innerContext' parameter should not be null.");
            this.innerContext = innerContext;
            this.isRequestConfigured = false;
            this.requestConfigurationLock = new object();         
        }

        public override Message RequestMessage
        {
            get
            {
                if (!this.isRequestConfigured)
                {
                    lock (this.requestConfigurationLock)
                    {
                        if (!this.isRequestConfigured)
                        {
                            this.isRequestConfigured = true;
                            Message innerMessage = this.innerContext.RequestMessage;
                            this.configuredRequestMessage = ConfigureRequestMessage(innerMessage);
                        }
                    }
                }

                return this.configuredRequestMessage;
            }
        }

        public override void Abort()
        {
            this.innerContext.Abort();
        }

        public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            message = ConfigureResponseMessage(message);
            return this.innerContext.BeginReply(message, timeout, callback, state);
        }

        public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
        {
            message = ConfigureResponseMessage(message);
            return this.innerContext.BeginReply(message, callback, state);
        }

        public override void Close(TimeSpan timeout)
        {
            this.innerContext.Close(timeout);
        }

        public override void Close()
        {
            this.innerContext.Close();
        }

        public override void EndReply(IAsyncResult result)
        {
            this.innerContext.EndReply(result);
        }

        public override void Reply(Message message, TimeSpan timeout)
        {
            message = ConfigureResponseMessage(message);
            this.innerContext.Reply(message, timeout);
        }

        public override void Reply(Message message)
        {
            ConfigureResponseMessage(message);
            this.innerContext.Reply(message);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Caller owns the Message and disposal of the Message.")]
        private static Message ConfigureRequestMessage(Message message)
        {
            if (message == null)
            {
                return null;
            }
            
            HttpRequestMessageProperty requestProperty = message.GetHttpRequestMessageProperty();
            if (requestProperty == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.RequestMissingHttpRequestMessageProperty,
                        HttpRequestMessageProperty.Name,
                        typeof(HttpRequestMessageProperty).FullName));
            }

            Uri uri = message.Headers.To;
            if (uri == null)
            {
                throw new InvalidOperationException(SR.RequestMissingToHeader);
            }

            HttpRequestMessage httpRequestMessage = message.ToHttpRequestMessage();
            if (httpRequestMessage == null)
            {
                httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Content = new StringContent(String.Empty);
                httpRequestMessage.Content.Headers.ContentLength = 0;
                message.Close();
                message = httpRequestMessage.ToMessage();
            }
            else
            {
                message.Headers.Clear();
                message.Properties.Clear();
                httpRequestMessage.Headers.Clear();
                httpRequestMessage.GetProperties().Clear();
            }

            message.Headers.To = uri;

            httpRequestMessage.RequestUri = uri;
            httpRequestMessage.Method = new HttpMethod(requestProperty.Method);
            //httpRequestMessage.Headers.Clear();
            //if (httpRequestMessage.Content != null)
                //httpRequestMessage.Content.Headers.Clear();
 
            foreach (var headerName in requestProperty.Headers.AllKeys)
            {
                if (headerName.StartsWith("content-", StringComparison.InvariantCultureIgnoreCase) ||
                    headerName.Equals("Allow", StringComparison.InvariantCultureIgnoreCase) ||
                    headerName.Equals("Expires") ||
                    headerName.Equals("Expires", StringComparison.InvariantCulture))
                {
                    httpRequestMessage.Content.Headers.Remove(headerName);
                    httpRequestMessage.Content.Headers.Add(headerName, requestProperty.Headers[headerName]);
                    continue;
                }
                httpRequestMessage.Headers.Remove(headerName);
                httpRequestMessage.Headers.Add(headerName, requestProperty.Headers[headerName]);
            }

            return message;
        }

        private static Message ConfigureResponseMessage(Message message)
        {
            if (message == null)
            {
                return null;
            }

            HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty();

            HttpResponseMessage httpResponseMessage = message.ToHttpResponseMessage();
            if (httpResponseMessage == null)
            {
                responseProperty.StatusCode = HttpStatusCode.InternalServerError;
                responseProperty.SuppressEntityBody = true;
            }
            else
            {
                responseProperty.StatusCode = httpResponseMessage.StatusCode;
                HttpResponseHeaders responseHeaders = httpResponseMessage.Headers;
                if (responseHeaders != null)
                {
                    foreach (var entry in responseHeaders)
                    {
                        foreach (var value in entry.Value)
                        {
                            responseProperty.Headers.Add(entry.Key, value);
                        }
                    }
                }


                if (httpResponseMessage.Content==null || httpResponseMessage.Content.Headers.ContentLength==0)
                {
                    responseProperty.SuppressEntityBody = true;
                }
                else 
                {
                    foreach (var entry in httpResponseMessage.Content.Headers)
                    {
                        foreach (var value in entry.Value)
                        {
                            responseProperty.Headers.Add(entry.Key, value);
                        }
                    }
                }
            }

            message.Properties.Clear();
            message.Headers.Clear();

            message.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);

            return message;
        }
    }
}
