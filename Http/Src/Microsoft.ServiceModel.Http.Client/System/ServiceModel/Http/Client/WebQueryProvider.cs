// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Client
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Web;
    using Microsoft.Http;

    internal class WebQueryProvider : IQueryProvider
    {
        private string resourceBaseAddress;
        private HttpClient client;
        private IExpressionToUriConverter converter;

        internal WebQueryProvider(string resourceBaseAddress, HttpClient client, IExpressionToUriConverter converter)
        {
            // the web query provider is per HTTP client, per converter, and per resource base address.
            // i.e. one can use the same web query provider to query various resources that are exposed at the same 
            // base address, using the same http client, and the same expression to URI conversion.
            this.resourceBaseAddress = resourceBaseAddress;
            this.client = client;
            this.converter = converter;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Type elementType = TypeHelper.GetElementType(expression.Type);
            Type queryType = typeof(WebQuery<>.WebOrderedQuery).MakeGenericType(new Type[] { elementType });
            object[] arguments = new object[] { expression, this };
            return (IQueryable)queryType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Expression), typeof(WebQueryProvider) }, null).Invoke(arguments);
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new WebQuery<T>.WebOrderedQuery(expression, this);
        }

        public object Execute(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Type elementType = TypeHelper.GetElementType(expression.Type);

            // GetMethod on generic methods does not work.  Has to resort to GetMethods.
            MethodInfo returnSingleton =
                (from m in typeof(WebQueryProvider).GetMethods()
                 where m.Name.StartsWith("ReturnSingleton")
                 select m).Single();

            returnSingleton = returnSingleton.MakeGenericMethod(new Type[] { elementType });
            return returnSingleton.Invoke(this, new object[] { expression });
        }

        // the entry point into the provider for actually executing single element query expressions.
        public T Execute<T>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return ReturnSingleton<T>(expression);
        }

        public T ReturnSingleton<T>(Expression expression)
        {
            SequenceMethod method;
            MethodCallExpression m = expression as MethodCallExpression;

            if (m == null)
            {
                throw new NotSupportedException(SR.ExpressionMustStartWithMethodCall);
            }

            if (!LinqReflectionUtil.TryIdentifySequenceMethod(m.Method, out method))
            {
                throw new NotSupportedException(string.Format(SR.MethodNotSupported, m.Method.Name));
            }

            WebQuery<T> query = new WebQuery<T>.WebOrderedQuery(expression, this);

            // internally, First is implemented using $top = 1
            // and Single is implemented using $top = 2
            switch (method)
            {
                case SequenceMethod.First:
                    return query.AsEnumerable().First();

                case SequenceMethod.FirstOrDefault:
                    return query.AsEnumerable().FirstOrDefault();

                case SequenceMethod.Single:
                    return query.AsEnumerable().Single();

                case SequenceMethod.SingleOrDefault:
                    return query.AsEnumerable().SingleOrDefault();

                default:
                    throw new NotSupportedException();
            }
        }

        internal Uri GetRequestUri(Expression expression)
        {
            Uri resourceRelativeUri = this.converter.Convert(expression);
            string resourceRelativeAddress = resourceRelativeUri.ToString();
            return new Uri(Utility.CombineUri(this.resourceBaseAddress, resourceRelativeAddress));
        }

        internal IEnumerable<T> ExecuteInternal<T>(HttpRequestMessage requestMessage)
        {
            HttpResponseMessage response = this.client.Send(requestMessage);
            return Deserialize<T>(response);
        }

        internal IAsyncResult BeginExecute<T>(HttpRequestMessage requestMessage, AsyncCallback callback, object state)
        {
            return new WebQueryAsyncResult(requestMessage, this.client, callback, state);
        }

        internal IEnumerable<T> EndExecute<T>(IAsyncResult asyncResult)
        {
            WebQueryAsyncResult result = (WebQueryAsyncResult)asyncResult;
            WebQueryAsyncResult.End(result);
            return Deserialize<T>(result.Response);
        }

        private IEnumerable<T> Deserialize<T>(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                HttpStatusCode statusCode = response.StatusCode;
                response.Dispose();

                throw new HttpException((int)statusCode, SR.WebQueryResponseDidNotReturnStatusOK);
            }

            if (response.Content == null || response.Content.GetLength() == 0)
            {
                // return a empty IEnumerable rather than null so that
                // the result of a query operation is never null, similar to other LINQ frameworks
                return new List<T>();
            }

            IEnumerable<T> results = null;

            if (response.Headers.ContentType.IsXmlContent())
            {
                results = response.Content.ReadAsDataContract<IEnumerable<T>>();
            }
            else if (response.Headers.ContentType.IsJsonContent())
            {
                results = response.Content.ReadAsJsonDataContract<IEnumerable<T>>();
            }
            else
            {
                throw
                    new NotSupportedException(SR.WebQueryResponseMessageInUnsupportedFormat);
            }

            return results;
        }

        private static class TypeHelper
        {
            internal static Type GetElementType(Type seqType)
            {
                Type type = FindIEnumerable(seqType);
                if (type == null)
                {
                    return seqType;
                }

                return type.GetGenericArguments()[0];
            }

            internal static Type FindIEnumerable(Type seqType)
            {
                if (seqType == null || seqType == typeof(string))
                {
                    return null;
                }

                if (seqType.IsArray)
                {
                    return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
                }

                if (seqType.IsGenericType)
                {
                    Type[] genericArguments = seqType.GetGenericArguments();
                    if (genericArguments.Length == 1)
                    {
                        Type ienum = typeof(IEnumerable<>).MakeGenericType(genericArguments[0]);
                        if (ienum.IsAssignableFrom(seqType))
                        {
                            return ienum;
                        }
                    }
                }

                Type[] ifaces = seqType.GetInterfaces();
                if (ifaces != null && ifaces.Length > 0)
                {
                    foreach (Type iface in ifaces)
                    {
                        Type ienum = FindIEnumerable(iface);
                        if (ienum != null)
                        {
                            return ienum;
                        }
                    }
                }

                if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                {
                    return FindIEnumerable(seqType.BaseType);
                }

                return null;
            }
        }

        private class WebQueryAsyncResult : AsyncResult
        {
            // 1 kb buffer size for reading from a network stream
            private const int BufferSize = 1024;

            private HttpClient client;
            private Stream networkStream;
            private MemoryStream memoryStream;
            private HttpResponseMessage response;

            private byte[] buffer;

            public WebQueryAsyncResult(HttpRequestMessage request, HttpClient client, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.client = client;
                this.buffer = new byte[BufferSize];
            
                // this buffer can grow
                this.memoryStream = new MemoryStream(BufferSize);

                // call HttpClient asynchronous send
                IAsyncResult result = client.BeginSend(request, this.PrepareAsyncCompletion(this.CompleteSend), this);
                if (this.SyncContinue(result))
                {
                    Complete(true);
                }
            }

            public HttpResponseMessage Response
            {
                get
                {
                    return this.response;
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<WebQueryAsyncResult>(result);
            }

            private bool CompleteSend(IAsyncResult result)
            {
                this.response = this.client.EndSend(result);

                if (this.response.Content == null)
                {
                    return true;
                }
                else
                {
                    this.networkStream = this.response.Content.ReadAsStream();

                    // asynchronously receive the content from the network stream
                    return this.ReceiveLoop();
                }
            }

            private bool ReceiveLoop()
            {
                while (true)
                {
                    IAsyncResult result = this.networkStream.BeginRead(this.buffer, 0, BufferSize, this.PrepareAsyncCompletion(this.ReceiveComplete), this);
                    if (!result.CompletedSynchronously)
                    {
                        return false;
                    }

                    if (this.CompleteReceive(result))
                    {
                        return true;
                    }
                }
            }

            private bool ReceiveComplete(IAsyncResult result)
            {
                if (this.CompleteReceive(result))
                {
                    return true;
                }

                return this.ReceiveLoop();
            }

            private bool CompleteReceive(IAsyncResult result)
            {
                int numBytes = this.networkStream.EndRead(result);
                if (numBytes > 0)
                {
                    this.memoryStream.Write(this.buffer, 0, numBytes);
                }

                bool done = numBytes == 0;

                if (done)
                {
                    this.memoryStream.Seek(0, SeekOrigin.Begin);
                    this.response.Content = HttpContent.Create(this.memoryStream);
                }

                return done;
            }
        }
    }
}
