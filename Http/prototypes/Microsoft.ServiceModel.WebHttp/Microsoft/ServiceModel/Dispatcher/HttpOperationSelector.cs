// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Http;

    public abstract class HttpOperationSelector : IDispatchOperationSelector
    {
        public abstract string SelectOperation(HttpRequestMessage message);

        string IDispatchOperationSelector.SelectOperation(ref Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            HttpRequestMessage requestMessage = message.ToHttpRequestMessage();
            return this.SelectOperation(requestMessage);
        }
    }
}
