// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Globalization;
    using System.Net.Http;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Abstract base class that defines methods to deserialize request messages
    /// and serialize response messages for the
    /// <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see>.
    /// </summary>
    public abstract class HttpMessageFormatter : IDispatchMessageFormatter
    {
        /// <summary>
        /// Deserializes a message into an array of parameters.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <param name="parameters">The objects that are passed to the operation as parameters.</param>
        void IDispatchMessageFormatter.DeserializeRequest(Message message, object[] parameters)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            HttpRequestMessage request = message.ToHttpRequestMessage();
            if (request == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        SR.HttpMessageFormatterNullRequest, 
                        this.GetType().Name));
            }

            this.DeserializeRequest(request, parameters);
        }

        /// <summary>
        /// Serializes a reply message from a specified message version, array of parameters, and a return value.
        /// </summary>
        /// <param name="messageVersion">The SOAP message version.</param>
        /// <param name="parameters">The out parameters.</param>
        /// <param name="result">The return value.</param>
        /// <returns>he serialized reply message.</returns>
        Message IDispatchMessageFormatter.SerializeReply(
                                              MessageVersion messageVersion, 
                                              object[] parameters, 
                                              object result)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (messageVersion != MessageVersion.None)
            {
                throw new NotSupportedException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.HttpMessageFormatterMessageVersion,
                        this.GetType()));
            }

            HttpResponseMessage response = this.GetDefaultResponse();
            if (response == null)
            {
                 throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.HttpMessageFormatterNullResponse,
                        this.GetType()));
            }

            this.SerializeReply(parameters, result, response);

            return response.ToMessage();
        }

        /// <summary>
        /// Deserializes a message into an array of parameters.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <param name="parameters">The objects that are passed to the operation as parameters.</param>
        protected abstract void DeserializeRequest(HttpRequestMessage message, object[] parameters);

        /// <summary>
        /// Serializes a reply message from an array of parameters and a return value into the
        /// given <paramref name="response"/>.
        /// </summary>
        /// <param name="parameters">The out parameters.</param>
        /// <param name="result">The return value.</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> to receive the serialized reply.</param>
        protected abstract void SerializeReply(object[] parameters, object result, HttpResponseMessage response);

        /// <summary>
        /// Gets or creates an <see cref="HttpResponseMessage"/> for use by
        /// <see cref="SerializeReply"/>.
        /// </summary>
        /// <remarks>The base class creates an empty response message.
        /// Derived classes are expected to override this method if they
        /// can provide a more informative response prior to serialization.</remarks>
        /// <returns>An <see cref="HttpResponseMessage"/> instance.   It must not be null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", 
            Justification = "This method always creates the HttpResponseMessage.")]
        protected virtual HttpResponseMessage GetDefaultResponse()
        {
            return new HttpResponseMessage();
        }
    }
}
