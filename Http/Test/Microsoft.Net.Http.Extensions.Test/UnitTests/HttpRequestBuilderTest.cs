using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpRequestBuilderTest
    {
        private HttpRequestMessage _request;
        private HttpRequestBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _request = new HttpRequestMessage();
            _builder = new HttpRequestBuilder(_request);
        }

        [TestMethod]
        public void WhenUriIsCalledThenRequestUriIsSet()
        {
            var uri = new Uri("http://www.microsoft.com");
            _builder.Uri(uri);
            Assert.AreEqual(uri, _request.RequestUri.AbsoluteUri);
        }

        [TestMethod]
        public void WhenMethodIsCalledThenRequestMethodIsSet()
        {
            _builder.Method(HttpMethod.Post);
            Assert.AreEqual(HttpMethod.Post, _request.Method);
        }

        [TestMethod]
        public void WhenContentIsCalledThenRequestContentIsSet()
        {
            var content = new StringContent(String.Empty);
            _builder.Content(()=>content);
            Assert.AreEqual(content, _request.Content);
        }

        [TestMethod]
        public void WhenAcceptIsCalledThenRequestAcceptIsSet()
        {
            var mediaType = new MediaTypeWithQualityHeaderValue("application/xml");
            _builder.Accept(mediaType);
            Assert.AreEqual(mediaType, _request.Headers.Accept.First());
        }

        [TestMethod]
        public void WhenAcceptIsCalledPassingStringsThenRequestAcceptIsSet()
        {
            var mediaType = "application/xml";
            _builder.Accept(mediaType);
            Assert.AreEqual(mediaType, _request.Headers.Accept.First().MediaType);
        }

        [TestMethod]
        public void WhenAcceptCharsetIsCalledThenRequestAcceptCharsetIsSet()
        {
            var charset = new StringWithQualityHeaderValue("unicode-1-1");
            _builder.AcceptCharset(charset);
            Assert.AreEqual(charset, _request.Headers.AcceptCharset.First());
        }

        [TestMethod]
        public void WhenAcceptCharsetIsCalledPassingStringsThenRequestAcceptCharsetIsSet()
        {
            var charset = "unicode-1-1";
            _builder.AcceptCharset(charset);
            Assert.AreEqual(charset, _request.Headers.AcceptCharset.First().Value);
        }

        [TestMethod]
        public void WhenAcceptEncodingIsCalledThenRequestAcceptEncodingIsSet()
        {
            var encoding = new StringWithQualityHeaderValue("gzip");
            _builder.AcceptEncoding(encoding);
            Assert.AreEqual(encoding, _request.Headers.AcceptEncoding.First());
        }

        [TestMethod]
        public void WhenAcceptEncodingIsCalledPassingStringsThenRequestAcceptEncodingIsSet()
        {
            var encoding = "gzip";
            _builder.AcceptEncoding(encoding);
            Assert.AreEqual(encoding, _request.Headers.AcceptEncoding.First().Value);
        }

        [TestMethod]
        public void WhenAcceptLanguageIsCalledThenRequestAcceptLanguageIsSet()
        {
            var language = new StringWithQualityHeaderValue("en");
            _builder.AcceptLanguage(language);
            Assert.AreEqual(language, _request.Headers.AcceptLanguage.First());
        }

        [TestMethod]
        public void WhenAcceptLanguageIsCalledPassingStringsThenRequestAcceptLanguageIsSet()
        {
            var language = "en";
            _builder.AcceptLanguage(language);
            Assert.AreEqual(language, _request.Headers.AcceptLanguage.First().Value);
        }

        [TestMethod]
        public void WhenAuthorizationIsCalledThenRequestAuthorizationIsSet()
        {
            var authorization = new AuthenticationHeaderValue("https");
            _builder.Authorization(authorization);
            Assert.AreEqual(authorization, _request.Headers.Authorization);
        }

        [TestMethod]
        public void WhenAuthorizationIsCalledPassingStringThenRequestAuthorizationIsSet()
        {
            var authorization = "http";
            _builder.Authorization(authorization);
            Assert.AreEqual(authorization, _request.Headers.Authorization.Scheme);
        }

        [TestMethod]
        public void WhenCacheControlIsCalledThenRequestCacheControlIsSet()
        {
            var cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            _builder.CacheControl(cacheControl);
            Assert.AreEqual(cacheControl, _request.Headers.CacheControl);
        }

        [TestMethod]
        public void WhenCacheControlIsCalledPassingStringThenRequestCacheControlIsSet()
        {
            var cacheControl = "no-cache";
            _builder.CacheControl(cacheControl);
            Assert.AreEqual(true, _request.Headers.CacheControl.NoCache);
        }

        [TestMethod]
        public void WhenConnectionIsCalledThenRequestConnectionIsSet()
        {
            var connection = "12345";
            _builder.Connection(connection);
            Assert.AreEqual(connection, _request.Headers.Connection.First());
        }

        [TestMethod]
        public void WhenConnectionIsCalledPassingCloseThenRequestCloseConnectionIsSet()
        {
            _builder.Connection("close");
            Assert.AreEqual(true, _request.Headers.ConnectionClose);
        }

        [TestMethod]
        public void WhenDateIsCalledThenRequestDateIsSet()
        {
            var date = new DateTimeOffset(DateTime.Now);
            _builder.Date(date);
            Assert.AreEqual(date, _request.Headers.Date);
        }

        [TestMethod]
        public void WhenDateIsCalledPassingStringThenRequestDateIsSet()
        {
            var date = new DateTimeOffset(DateTime.Now);
            _builder.Date(date);
            Assert.AreEqual(date, _request.Headers.Date);
        }

        [TestMethod]
        public void WhenExpectIsCalledThenRequestExpectIsSet()
        {
            var expect = new NameValueWithParametersHeaderValue("Foo", "bar");
            _builder.Expect(expect);
            Assert.AreEqual(expect, _request.Headers.Expect.First());
        }

        [TestMethod]
        public void WhenExpectIsCalledPassingStringsThenRequestExpectIsSet()
        {
            var expect = "Foo=Bar";
            _builder.Expect(expect);
            Assert.AreEqual("Foo", _request.Headers.Expect.First().Name);
        }

        [TestMethod]
        public void WhenExpectIsCalledPassingContinueThenExpectContinueIsSet()
        {
            var expect = "100-Continue";
            _builder.Expect(expect);
            Assert.AreEqual(true, _request.Headers.ExpectContinue);
        }

        [TestMethod]
        public void WhenFromIsCalledThenRequestFromIsSet()
        {
            var from = "foo@foo.com";
            _builder.From(from);
            Assert.AreEqual(from, _request.Headers.From);
        }

        [TestMethod]
        public void WhenHostIsCalledThenRequestHostIsSet()
        {
            var host = "foo.com";
            _builder.Host(host);
            Assert.AreEqual(host, _request.Headers.Host);
        }

        [TestMethod]
        public void WhenIfMatchIsCalledThenRequestIfMatchIsSet()
        {
            var ifMatch = new EntityTagHeaderValue("\"Foo\"");
            _builder.IfMatch(ifMatch);
            Assert.AreEqual(ifMatch, _request.Headers.IfMatch.First());
        }

        [TestMethod]
        public void WhenIfMatchIsCalledPassingStringsThenRequestIfMatchIsSet()
        {
            var ifMatch = "\"Foo\"";
            _builder.IfMatch(ifMatch);
            Assert.AreEqual(ifMatch, _request.Headers.IfMatch.First().Tag);
        }


    }
}
