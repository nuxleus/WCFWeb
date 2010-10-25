// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Text;

    public static class WebProgrammingModelExtensionMethods
    {
        public static WebGetAttribute GetWebGetAttribute(this HttpOperationDescription operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var webGet = operation.ToOperationDescription().Behaviors.Find<WebGetAttribute>();
            if (webGet != null)
            {
                return webGet;
            }

            return null;
        }

        public static WebInvokeAttribute GetWebInvokeAttribute(
                                             this HttpOperationDescription operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            
            var webInvoke = operation.ToOperationDescription().Behaviors.Find<WebInvokeAttribute>();
            if (webInvoke != null)
            {
                return webInvoke;
            }

            return null;
        }

        public static string GetWebMethod(this HttpOperationDescription operation)
        {
            WebGetAttribute webGet = operation.GetWebGetAttribute();
            if (webGet != null)
            {
                return "GET";
            }

            WebInvokeAttribute webInvoke = operation.GetWebInvokeAttribute();
            if (webInvoke == null)
            {
                return "POST";
            }

            return webInvoke.Method ?? "POST";
        }

        [SuppressMessage("Microsoft.Design", "CA1055", Justification = "By Design")]
        public static string GetUriTemplateString(this HttpOperationDescription operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            
            WebGetAttribute webGet = operation.GetWebGetAttribute();
            WebInvokeAttribute webInvoke = operation.GetWebInvokeAttribute();

            if (webGet != null && webGet.UriTemplate != null)
            {
                return webGet.UriTemplate;
            }

            if (webInvoke != null && webInvoke.UriTemplate != null)
            {
                return webInvoke.UriTemplate;
            }

            if (operation.GetWebMethod() == "GET")
            {
                return GetDefaultWebGetUriTemplate(operation);
            }

            return operation.Name;
        }

        public static UriTemplate GetUriTemplate(this HttpOperationDescription operation)
        {
            return new UriTemplate(operation.GetUriTemplateString());
        }

        public static HttpParameterDescription GetBodyParameter(
                                                    this HttpOperationDescription operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            UriTemplate template = operation.GetUriTemplate();
            IEnumerable<string> uriVariables = template.PathSegmentVariableNames
                                                       .Concat(template.QueryValueVariableNames);
            return operation.InputParameters
                            .Where(p =>
                                !uriVariables.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                            .FirstOrDefault();
        }

        private static string GetDefaultWebGetUriTemplate(HttpOperationDescription operation)
        {
            StringBuilder builder = new StringBuilder(operation.Name);
            builder.Append("?");

            foreach (HttpParameterDescription parameter in operation.InputParameters)
            {
                string name = parameter.Name;
                builder.Append(name);
                builder.Append("={");
                builder.Append(name);
                builder.Append("}&");
            }

            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
    }
}
