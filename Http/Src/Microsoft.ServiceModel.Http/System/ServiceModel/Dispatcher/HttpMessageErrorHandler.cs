// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Globalization;
    using System.ServiceModel.Channels;
    using Microsoft.Http;

    /// <summary>
    /// Abstract base class to provide an <see cref="IErrorHandler"/> for the
    /// <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see>
    /// </summary>
    public abstract class HttpMessageErrorHandler : IErrorHandler
    {
        /// <summary>
        /// Enables the creation of a custom <see cref="FaultException"/> 
        /// that is returned from an exception in the course of a service method.
        /// </summary>
        /// <remarks>
        /// This method is implemented solely to delegate control to 
        /// <see cref="ProvideFault(Exception, HttpResponseMessagee)"/>
        /// </remarks>
        /// <param name="error">The <see cref="Exception"/> object thrown in the course 
        /// of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="System.ServiceModel.Channels.Message"/> object 
        /// that is returned to the client, or service, in the duplex case.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
            Justification = "The 'ProvideReponse' method provides this functionality and it is visible to derived classes.")]
        void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            HttpResponseMessage responseMessage = (fault == null) ? this.GetDefaultResponse() : fault.ToHttpResponseMessage();
            if (responseMessage == null)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, SR.HttpErrorMessageNullResponse, this.GetType().Name);
                throw new InvalidOperationException(errorMessage);
            }

            this.ProvideResponse(error, responseMessage);
            fault = responseMessage.ToMessage();
        }

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases. 
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>true if should not abort the session (if there is one) and instance context if the instance context is not Single; otherwise, false. The default is false.</returns>
        public abstract bool HandleError(Exception error);

        /// <summary>
        /// Enables the creation of a custom response describing the specified <paramref name="error"/>.
        /// </summary>
        /// <param name="exception">The error for which a response is required.</param>
        /// <param name="response">The response message to populate.</param>
        protected abstract void ProvideResponse(Exception exception, HttpResponseMessage response);

        /// <summary>
        /// Gets or creates a new <see cref="HttpResponseMessage"/> to be filled by
        /// <see cref="ProvideResponse"/>.
        /// </summary>
        /// <remarks>The base class creates an empty response message.  Derived types
        /// are expected to use this to create more complete responses</remarks>
        /// <returns>The <see cref="HttpResponseMessage"/>.  It must not be null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", 
            Justification = "This is not really property since it creates a new response message per incoming request.")]
        protected virtual HttpResponseMessage GetDefaultResponse()
        {
            return new HttpResponseMessage();
        }
    }
}
