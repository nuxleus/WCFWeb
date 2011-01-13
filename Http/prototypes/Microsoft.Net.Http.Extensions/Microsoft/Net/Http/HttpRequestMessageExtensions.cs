using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Net.Http
{
    using System.Net.Http;

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestBuilder With(this HttpRequestMessage request)
        {
            return new HttpRequestBuilder(request);
        }
    }
}
