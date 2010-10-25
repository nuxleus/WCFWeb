// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test
{
    using System;
    using System.ServiceModel.Web;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    class QueryServiceHost
    {
        WebServiceHost host;
        public QueryServiceHost(Uri address, object service)
        {
            host = new WebServiceHost(service);
            host.AddServiceEndpoint(typeof(IQueryService), new WebHttpBinding(), address);
            WebHttpBehavior behavior = new WebHttpBehavior();
            host.Description.Endpoints[0].Behaviors.Add(behavior);
        }

        public void Open()
        {
            Console.WriteLine("[Start service]");
            host.Open();
            Console.WriteLine("[Service started]");
        }

        public void Close()
        {
            Console.WriteLine("[Stopping service]");
            host.Close();
            Console.WriteLine("[Service stopped]");
        }
    }
}
