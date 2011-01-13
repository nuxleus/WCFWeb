namespace Microsoft.Net.Http
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class HttpRequestBuilder
    {
        private HttpRequestMessage request;

        public HttpRequestBuilder(HttpRequestMessage request)
        {
            this.request = request;
        }

        public HttpRequestBuilder Uri(Uri uri)
        {
            this.request.RequestUri = uri;
            return this;
        }

        public HttpRequestBuilder Method(HttpMethod method)
        {
            this.request.Method = method;
            return this;
        }

        public HttpRequestBuilder Content(Func<HttpContent> content)
        {
            this.request.Content = content();
            return this;
        }

        public HttpRequestBuilder Accept(params MediaTypeWithQualityHeaderValue[] items)
        {
            items.ToList().ForEach(this.request.Headers.Accept.Add);
            return this;
        }

        public HttpRequestBuilder Accept(params string[] acceptItems)
        {
            this.request.Headers.Add("Accept", acceptItems);
            return this;
        }

        public HttpRequestBuilder AcceptCharset(params StringWithQualityHeaderValue[] acceptCharsetItems)
        {
            acceptCharsetItems.ToList().ForEach(this.request.Headers.AcceptCharset.Add);
            return this;
        }

        public HttpRequestBuilder AcceptCharset(params string[] items)
        {
            this.request.Headers.Add("Accept-Charset", items);
            return this;
        }

        public HttpRequestBuilder AcceptEncoding(params StringWithQualityHeaderValue[] acceptEncodingitems)
        {
            acceptEncodingitems.ToList().ForEach(this.request.Headers.AcceptEncoding.Add);
            return this;
        }

        public HttpRequestBuilder AcceptEncoding(params string[] items)
        {
            this.request.Headers.Add("Accept-Encoding", items);
            return this;
        }

        public HttpRequestBuilder AcceptLanguage(params StringWithQualityHeaderValue[] acceptLanguageitems)
        {
            acceptLanguageitems.ToList().ForEach(this.request.Headers.AcceptLanguage.Add);
            return this;
        }

        public HttpRequestBuilder AcceptLanguage(params string[] acceptLanguageItems)
        {
            this.request.Headers.Add("Accept-Language", acceptLanguageItems);
            return this;
        }

        public HttpRequestBuilder Authorization(AuthenticationHeaderValue authorizationValue)
        {
            this.request.Headers.Authorization = authorizationValue;
            return this;
        }

        public HttpRequestBuilder Authorization(string authorizationValue)
        {
            this.AddOrReplaceHeader("Authorization", authorizationValue);
            return this;
        }

        public HttpRequestBuilder CacheControl(CacheControlHeaderValue cacheControlValue)
        {
            this.request.Headers.CacheControl = cacheControlValue;
            return this;
        }

        public HttpRequestBuilder CacheControl(string cacheControlValue)
        {
            this.AddOrReplaceHeader("Cache-Control", cacheControlValue);
            return this;
        }


        public HttpRequestBuilder Connection(params string[] connectionItems)
        {
            foreach(var item in connectionItems)
            {
                if (item.Equals("Close", StringComparison.OrdinalIgnoreCase))
                {
                    this.request.Headers.ConnectionClose = true;
                }
                else
                {
                    request.Headers.Connection.Add(item);
                }
            }
            return this;
        }

        public HttpRequestBuilder Date(DateTimeOffset? dateValue)
        {
            this.request.Headers.Date = dateValue;
            return this;
        }

        public HttpRequestBuilder Date(string dateValue)
        {
            this.AddOrReplaceHeader("Date", dateValue);
            return this;
        }

        public HttpRequestBuilder Expect(params NameValueWithParametersHeaderValue[] expectItems)
        {
            expectItems.ToList().ForEach(this.request.Headers.Expect.Add);
            return this;
        }

        public HttpRequestBuilder Expect(params string[] expectItems)
        {
            this.request.Headers.Add("Expect", expectItems);
            return this;
        }

        public HttpRequestBuilder From(string fromValue)
        {
            this.request.Headers.From = fromValue;
            return this;
        }

        private void AddOrReplaceHeader(string header, string value)
        {
            if (this.request.Headers.Contains(header))
            {
                request.Headers.Remove(header);
            }
            request.Headers.Add(header, value);
        }

        public HttpRequestBuilder Host(string hostValue)
        {
            this.request.Headers.Host = hostValue;
            return this;
        }

        public HttpRequestBuilder IfMatch(params EntityTagHeaderValue[] ifMatchItems)
        {
            ifMatchItems.ToList().ForEach(this.request.Headers.IfMatch.Add);
            return this;
        }

        public HttpRequestBuilder IfMatch(params string[] ifMatchItems)
        {
            this.request.Headers.Add("If-Match", ifMatchItems);
            return this;
        }

    }
}