// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System.Net.Http;

    public class HttpMessageProperty
    {
        public static string Name
        {
            get { return "HttpMessageProperty"; }
        }

        public HttpRequestMessage Request { get; set; }

        public HttpResponseMessage Response { get; set; }
    }
}
