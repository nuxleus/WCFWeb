// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using Microsoft.Http;

    public abstract class ComposableHttpOperationSelector : HttpOperationSelector
    {
        public string DefaultOperationName { get; set; }

        public abstract HttpOperationSelector SelectDelegateOperationSelector(
                                                        HttpRequestMessage message);

        public override string SelectOperation(HttpRequestMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            HttpOperationSelector selector = this.SelectDelegateOperationSelector(message);
            string operationName = string.Empty;
            if (selector != null)
            {
                operationName = selector.SelectOperation(message);
            }

            if (string.IsNullOrWhiteSpace(operationName))
            {
                operationName = this.OnEmptyOperationNameSelected(message);
            }

            return operationName;
        }

        protected virtual string OnEmptyOperationNameSelected(HttpRequestMessage message)
        {
            return this.DefaultOperationName ?? string.Empty;
        }
    }
}
