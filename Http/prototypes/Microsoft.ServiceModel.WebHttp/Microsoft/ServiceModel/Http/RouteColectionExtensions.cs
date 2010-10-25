// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel.Activation;
    using System.Web.Routing;

    public static class RouteCollectionExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1004", Justification = "By design for usability")]
        [SuppressMessage("Microsoft.Design", "CA1026", Justification = "Not applicable")]
        public static void AddServiceRoute<TService>(this RouteCollection routes, string routePrefix, HostConfiguration configuration = null)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            var route = new ServiceRoute(routePrefix, new WebHttpServiceHostFactory(configuration), typeof(TService));
            routes.Add(route);
        }
    }
}
