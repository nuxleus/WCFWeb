// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.IO;
    using Microsoft.Http;

    internal class HttpMessageEncoderFactory : MessageEncoderFactory
    {
        private HttpMessageEncoder encoder;

        public HttpMessageEncoderFactory()
        {
            this.encoder = new HttpMessageEncoder();
        }

        public override MessageEncoder Encoder
        {
            get 
            {
                return this.encoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get 
            {
                return MessageVersion.None;    
            }
        }

        private class HttpMessageEncoder : MessageEncoder
        {
            private static readonly string httpMessageBindingClassName = typeof(HttpMessageBinding).FullName;
            private static readonly string httpResponseMessageClassName = typeof(HttpResponseMessage).FullName;

            public override string ContentType
            {
                get 
                {
                    return string.Empty;
                }
            }

            public override string MediaType
            {
                get
                {
                    return string.Empty;
                }
            }

            public override MessageVersion MessageVersion
            {
                get
                {
                    return MessageVersion.None;
                }
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                if (contentType == null)
                {
                    throw new ArgumentNullException("contentType");
                }

                return true;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Caller owns the Message and disposal of the Message.")]
            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                if (bufferManager == null)
                {
                    throw new ArgumentNullException("bufferManager");
                }

                byte[] content = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, content, 0, content.Length);
                bufferManager.ReturnBuffer(buffer.Array);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Content = HttpContent.Create(content, contentType);

                Message message = request.ToMessage();
                message.Properties.Encoder = this;

                return message;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Caller owns the Message and disposal of the Message.")]
            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException("stream");
                }

                HttpRequestMessage request = new HttpRequestMessage();
                request.Content = HttpContent.Create(stream, contentType, null);
             
                Message message = request.ToMessage();
                message.Properties.Encoder = this;

                return message;
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                if (bufferManager == null)
                {
                    throw new ArgumentNullException("bufferManager");
                }

                if (maxMessageSize < 0)
                {
                    throw new ArgumentOutOfRangeException("maxMessageSize");
                }

                if (messageOffset < 0)
                {
                    throw new ArgumentOutOfRangeException("messageOffset");
                }

                if (messageOffset > maxMessageSize)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            SR.ParameterMustBeLessThanOrEqualSecondParameter, 
                            "messageOffset", 
                            "maxMessageSize"));
                }

                message.Properties.Encoder = this;

                HttpResponseMessage response = GetHttpResponseMessageOrThrow(message);

                if (response.Content == null)
                {
                    return new ArraySegment<byte>();
                }

                byte[] messageBytes = response.Content.ReadAsByteArray();
                int messageLength = messageBytes.Length;
                int totalLength = messageLength + messageOffset;
                if (totalLength > maxMessageSize)
                {
                    throw new QuotaExceededException();
                }

                byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
                Array.Copy(messageBytes, 0, totalBytes, messageOffset, totalLength - messageOffset);
                return new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                if (stream == null)
                {
                    throw new ArgumentNullException("stream");
                }

                message.Properties.Encoder = this;

                HttpResponseMessage response = GetHttpResponseMessageOrThrow(message);

                if (response.Content != null)
                {
                    response.Content.WriteTo(stream);
                }
            }

            private static HttpResponseMessage GetHttpResponseMessageOrThrow(Message message)
            {
                HttpResponseMessage response = message.ToHttpResponseMessage();
                if (response == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                        CultureInfo.CurrentCulture,
                            SR.MessageInvalidForHttpMessageEncoder,
                            httpMessageBindingClassName,
                            HttpMessageExtensionMethods.ToMessageMethodName,
                            httpResponseMessageClassName));
                }

                return response;
            }
        }
    }
}
