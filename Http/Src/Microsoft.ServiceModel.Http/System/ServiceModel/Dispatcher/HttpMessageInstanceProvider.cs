// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Declares methods that provide a service object or recycle a service object for a service
    /// based on <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see>.
    /// </summary>
    public abstract class HttpMessageInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// Returns a service object given the specified <see cref="InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="InstanceContext"/> object.</param>
        /// <returns>The service object.</returns>
        public abstract object GetInstance(InstanceContext instanceContext);

        /// <summary>
        /// Called when an <see cref="InstanceContext"/> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        public abstract void ReleaseInstance(InstanceContext instanceContext, object instance);

        /// <summary>
        /// Returns a service object given the specified <see cref="InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">he current <see cref="InstanceContext"/> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>The service object.</returns>
        object IInstanceProvider.GetInstance(InstanceContext instanceContext, Message message)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException("instanceContext");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            HttpRequestMessage request = message.ToHttpRequestMessage();
            if (request == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.HttpInstanceProviderNullRequest,
                        this.GetType().Name));
            }

            return this.GetInstance(instanceContext, request);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="InstanceContext"/> object
        /// and <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="InstanceContext"/> object.</param>
        /// <param name="request">The request message that triggered the creation of a service object.</param>
        /// <returns>The service object.</returns>
        protected abstract object GetInstance(
                                   InstanceContext instanceContext,
                                   HttpRequestMessage request);
    }
}
