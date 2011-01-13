// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Globalization;
    using System.Net.Http;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Defines the contract that associates incoming messages with a local operation 
    /// to customize service execution behavior using
    /// <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see>.
    /// </summary>
    public abstract class HttpMessageOperationSelector : IDispatchOperationSelector
    {
        /// <summary>
        /// Associates a local operation with the incoming method.
        /// </summary>
        /// <param name="message">The incoming <see cref="Message"/> to be associated with an operation.</param>
        /// <returns>The name of the operation to be associated with the message.</returns>
        string IDispatchOperationSelector.SelectOperation(ref Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            HttpRequestMessage requestMessage = message.ToHttpRequestMessage();
            if (requestMessage == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.HttpOperationSelectorNullRequest,
                        this.GetType().Name));
            }

            string operation = this.SelectOperation(requestMessage);
            if (operation == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.HttpOperationSelectorNullOperation,
                        this.GetType().Name));
            }

            return operation;
        }

        /// <summary>
        /// Associates a local operation with the incoming method.
        /// </summary>
        /// <param name="message">The incoming <see cref="HttpRequestMessage"/>.</param>
        /// <returns>The name of the operation to be associated with the message.</returns>
        protected abstract string SelectOperation(HttpRequestMessage message);
    }
}
