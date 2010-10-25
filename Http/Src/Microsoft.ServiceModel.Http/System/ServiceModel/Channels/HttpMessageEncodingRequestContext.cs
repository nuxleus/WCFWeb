// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using Microsoft.Http;
    using Microsoft.Http.Headers;

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
                httpRequestMessage.Content = HttpContent.CreateEmpty();
                httpRequestMessage.Headers.ContentLength = 0;
                message.Close();
                message = httpRequestMessage.ToMessage();
            }
            else
            {
                message.Headers.Clear();
                message.Properties.Clear();
                httpRequestMessage.Headers.Clear();
                httpRequestMessage.Properties.Clear();
            }

            message.Headers.To = uri;

            httpRequestMessage.Uri = uri;
            httpRequestMessage.Method = requestProperty.Method;
            foreach (var headerName in requestProperty.Headers.AllKeys)
            {
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
                ResponseHeaders responseHeaders = httpResponseMessage.Headers;
                if (responseHeaders != null)
                {
                    foreach (string header in responseHeaders.Keys)
                    {
                        responseProperty.Headers.Add(header, responseHeaders[header]);
                    }
                }

                if (httpResponseMessage.Content == null)
                {
                    responseProperty.SuppressEntityBody = true;
                }
                else if (httpResponseMessage.Content.HasLength() && httpResponseMessage.Content.GetLength() == 0)
                {
                    responseProperty.SuppressEntityBody = true;
                }
            }

            message.Properties.Clear();
            message.Headers.Clear();

            message.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);

            return message;
        }
    }
}
