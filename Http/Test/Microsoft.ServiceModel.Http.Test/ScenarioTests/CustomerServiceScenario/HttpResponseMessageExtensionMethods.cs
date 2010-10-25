// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.ScenarioTests
{
    using Microsoft.Http;

    internal static class HttpMessageResponseExtensionMethods
    {
        public static void CopyTo(this HttpResponseMessage from, HttpResponseMessage to)
        {
            to.Method = from.Method;
            to.StatusCode = from.StatusCode;
            to.Uri = from.Uri;
            to.Content = from.Content;
            to.Headers.Clear();
            foreach (var header in from.Headers)
            {
                to.Headers.Add(header.Key, header.Value);
            }

            to.Properties.Clear();
            foreach (var obj in from.Properties)
            {
                to.Properties.Add(obj);
            }
        }

    }
}
