// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace QueryableSample
{
    using System;
    using System.ServiceModel.Activation;
    using System.Web.Routing;

    using Microsoft.ServiceModel.Http;

    public class Global : System.Web.HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // setting up contacts services
            RouteTable.Routes.AddServiceRoute<ContactsResource>("contacts", new QueryableSampleConfiguration());
        }
    }
}
