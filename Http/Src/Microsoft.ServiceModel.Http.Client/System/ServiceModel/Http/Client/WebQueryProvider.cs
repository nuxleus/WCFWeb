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
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;

    using System.Net.Http;

    using HttpException = System.Web.HttpException;
    using System.ServiceModel.Web.Client;

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

        internal Task<IEnumerable<T>> ExecuteInternalAsync<T>(HttpRequestMessage requestMessage)
        {
            TaskCompletionSource<IEnumerable<T>> tcs = new TaskCompletionSource<IEnumerable<T>>();
            this.client.SendAsync(requestMessage).ContinueWith(task =>
            {
                try
                {
                    if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception.GetBaseException());
                        return;
                    }

                    if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                        return;
                    }

                    HttpResponseMessage response = task.Result;
                    if (response == null)
                    {
                        tcs.TrySetResult(null);
                        return;
                    }

                    IEnumerable<T> results = Deserialize<T>(response);
                    tcs.TrySetResult(results);
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }

            });

            return tcs.Task;

        }

        private IEnumerable<T> Deserialize<T>(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                HttpStatusCode statusCode = response.StatusCode;
                response.Dispose();

                throw new System.Web.HttpException((int)statusCode, SR.WebQueryResponseDidNotReturnStatusOK);
            }

            if (response.Content == null || response.Content.Headers.ContentLength == 0 || response.Content.Headers.ContentType == null)
            {
                // return a empty IEnumerable rather than null so that
                // the result of a query operation is never null, similar to other LINQ frameworks
                return new List<T>();
            }

            IEnumerable<T> results = null;

            if (response.Content.Headers.ContentType.MediaType == null)
                throw new ArgumentException(SR.WebQueryResponseMessageMissingMediaType);

            if (response.Content.Headers.ContentType.MediaType.IsXmlContent())
            {
                results = ReadAsDataContract<IEnumerable<T>>(response.Content);
            }
            else if (response.Content.Headers.ContentType.MediaType.IsJsonContent())
            {
                results = ReadAsJsonDataContract<IEnumerable<T>>(response.Content);
            }
            else
            {
                throw
                    new NotSupportedException(SR.WebQueryResponseMessageInUnsupportedFormat);
            }

            return results;
        }

        static T ReadAsDataContract<T>(HttpContent content)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (var stream = content.ContentReadStream)
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        static T ReadAsJsonDataContract<T>(HttpContent content)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (var r = content.ContentReadStream)
            {
                return (T)serializer.ReadObject(r);
            }
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


    }
}
