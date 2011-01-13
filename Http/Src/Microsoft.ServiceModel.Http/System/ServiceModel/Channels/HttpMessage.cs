// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.Http;
    using System.Xml;

    internal class HttpMessage : Message
    {
        internal static readonly string MessageTypeFullName = typeof(Message).FullName;

        private HttpRequestMessage request;
        private HttpResponseMessage response;
        private MessageHeaders headers;
        private MessageProperties properties;

        public HttpMessage(HttpRequestMessage request)
        {
            Debug.Assert(request != null, "The 'request' parameter should not be null.");
            this.request = request;
            this.IsRequest = true;
        }

        public HttpMessage(HttpResponseMessage response)
        {
            Debug.Assert(response != null, "The 'response' parameter should not be null.");
            this.response = response;
            this.IsRequest = false;
        }

        public bool IsRequest { get; private set; }

        public override MessageVersion Version
        {
            get
            {
                this.EnsureNotDisposed();
                return MessageVersion.None;
            }
        }
        
        public override MessageHeaders Headers
        {
            get
            {
                this.EnsureNotDisposed();
                if (this.headers == null)
                {
                    this.headers = new MessageHeaders(MessageVersion.None);
                }

                return this.headers;
            }
        }

        public override MessageProperties Properties
        {
            get
            {
                this.EnsureNotDisposed();
                if (this.properties == null)
                {
                    this.properties = new MessageProperties();
                    this.properties.AllowOutputBatching = false;
                }

                return this.properties;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                long? contentLength = this.GetHttpContentLength();
                return contentLength.HasValue && contentLength.Value == 0;
            }
        }

        public override bool IsFault
        {
            get
            {
                return false;
            }
        }

        public HttpRequestMessage GetHttpRequestMessage()
        {
            this.EnsureNotDisposed();
            Debug.Assert(this.IsRequest, "This method should only be called when IsRequest is true.");
            return this.request;
        }

        public HttpResponseMessage GetHttpResponseMessage()
        {
            this.EnsureNotDisposed();
            Debug.Assert(!this.IsRequest, "This method should only be called when IsRequest is false.");
            return this.response;
        }

        protected override void OnWriteBodyContents(Xml.XmlDictionaryWriter writer)
        {
            throw GetNotSupportedException();
        }

        protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
        {
            throw GetNotSupportedException();
        }

        protected override XmlDictionaryReader OnGetReaderAtBodyContents()
        {
            throw GetNotSupportedException();
        }

        protected override string OnGetBodyAttribute(string localName, string ns)
        {
            throw GetNotSupportedException();
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            throw GetNotSupportedException();
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            throw GetNotSupportedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "The base class always construct a writer")]
        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            long? contentLength = this.GetHttpContentLength();
            string contentString = null;

            if (this.IsRequest)
            {
                contentString = contentLength.HasValue ?
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        SR.MessageBodyIsHttpRequestMessageWithKnownContentLength, 
                        contentLength.Value) :
                    SR.MessageBodyIsHttpRequestMessageWithUnknownContentLength;
            }
            else
            {
                contentString = contentLength.HasValue ?
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        SR.MessageBodyIsHttpResponseMessageWithKnownContentLength, 
                        contentLength.Value) :
                    SR.MessageBodyIsHttpResponseMessageWithUnknownContentLength;
            }

            writer.WriteString(contentString);
        }

        protected override void OnWriteMessage(XmlDictionaryWriter writer)
        {
            throw GetNotSupportedException();
        }

        protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            throw GetNotSupportedException();
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (this.IsRequest)
            {
                this.request.Dispose();
                this.request = null;
            }
            else
            {
                this.response.Dispose();
                this.response = null;
            }
        }

        private static NotSupportedException GetNotSupportedException()
        {
            return new NotSupportedException(
                                    string.Format(
                                        CultureInfo.CurrentCulture,
                                        SR.MessageReadWriteCopyNotSupported,
                                        HttpMessageExtensionMethods.ToHttpRequestMessageMethodName,
                                        HttpMessageExtensionMethods.ToHttpResponseMessageMethodName,
                                        MessageTypeFullName));
        }

        private void EnsureNotDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(string.Empty, SR.MessageClosed);
            }
        }

        private long? GetHttpContentLength()
        {
            HttpContent content = this.IsRequest ?
                this.GetHttpRequestMessage().Content :
                this.GetHttpResponseMessage().Content;

            if (content == null)
            {
                return 0;
            }

            return content.Headers.ContentLength;
        }
    }
}
