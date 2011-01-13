using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Microsoft.Net.Http
{
    public static class HttpClientExtensions
    {
        public static HttpResponseMessage Get(this HttpClient client, Uri uri, HttpRequestMessage request)
        {
            request.Method = HttpMethod.Get;
            request.RequestUri = uri;
            return client.Send(request);
        }
    }
}
