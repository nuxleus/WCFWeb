// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web.Routing;

    using Microsoft.ServiceModel.Description;

    public static class RouteCollectionExtensions
    {
        public static void AddServiceRoute<TService>(this RouteCollection routes, string routePrefix, HttpHostConfiguration configuration = null)
        {
            AddServiceRoute<TService, WebHttpServiceHostFactory>(routes, routePrefix, configuration);
        }

        public static void AddServiceRoute<TService, TServiceHostFactory>(this RouteCollection routes, string routePrefix, HttpHostConfiguration configuration = null) where TServiceHostFactory : ServiceHostFactoryBase, IConfigurableServiceHostFactory, new()
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            var route = new ServiceRoute(routePrefix, new TServiceHostFactory() { Configuration = configuration }, typeof(TService));
            routes.Add(route);
        }

        public static void AddServiceRoute<TService, TServiceHostFactory>(this RouteCollection routes, string routePrefix) where TServiceHostFactory : ServiceHostFactoryBase, new()
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            var route = new ServiceRoute(routePrefix, new TServiceHostFactory(), typeof(TService));
            routes.Add(route);
        }
    }
}
