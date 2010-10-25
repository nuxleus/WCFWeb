// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Description;
    using Microsoft.Http;
    
    public class MethodAndUriTemplateOperationSelector : ComposableHttpOperationSelector
    {
        private IDictionary<string, UriTemplateOperationSelector> delegates;

        public MethodAndUriTemplateOperationSelector(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            this.delegates =
                new Dictionary<string, UriTemplateOperationSelector>();

            var operations = endpoint.Contract.Operations.Select(od => new HttpOperationDescription(od));

            foreach (var methodGroup in operations.GroupBy(od => od.GetWebMethod()))
            {
                UriTemplateTable table = new UriTemplateTable(endpoint.ListenUri);
                foreach (var operation in methodGroup)
                {
                    UriTemplate template = new UriTemplate(operation.GetUriTemplateString());
                    table.KeyValuePairs.Add(
                        new KeyValuePair<UriTemplate, object>(template, operation.Name));
                }

                table.MakeReadOnly(false);
                UriTemplateOperationSelector templateSelector =
                    new UriTemplateOperationSelector(table);
                this.delegates.Add(methodGroup.Key, templateSelector);
            }
        }

        public MethodAndUriTemplateOperationSelector(
                   IDictionary<string, UriTemplateOperationSelector> delegates)
        {
            if (delegates == null)
            {
                throw new ArgumentNullException("delegates");
            }

            this.delegates = delegates;
        }

        public override HttpOperationSelector SelectDelegateOperationSelector(
                                                        HttpRequestMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            string method = message.Method;

            UriTemplateOperationSelector selector = null;
            if (method != null)
            {
                this.delegates.TryGetValue(method, out selector);
            }

            return selector;
        }

        public override string SelectOperation(HttpRequestMessage message)
        {
            var operation = base.SelectOperation(message);
            SelectedOperation.Set(message, operation);
            return operation;
        }
    }
}
