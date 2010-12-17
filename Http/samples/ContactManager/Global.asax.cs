// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager
{
    using System;
    using System.ServiceModel.Activation;
    using System.Web.Routing;
    using Microsoft.ServiceModel.Http;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var configuration = new ContactManagerConfiguration();
            RouteTable.Routes.AddServiceRoute<ContactResource>("contact", configuration);
            RouteTable.Routes.AddServiceRoute<ContactsResource>("contacts", configuration);
        }
    }
}