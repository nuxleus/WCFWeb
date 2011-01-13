// <copyright>using 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.ScenarioTests
{
    using System.Net.Http;
    using System.ServiceModel.Channels;

    internal static class HttpMessageResponseExtensionMethods
    {
        public static void CopyTo(this HttpResponseMessage from, HttpResponseMessage to)
        {
            to.RequestMessage = from.RequestMessage;
            to.StatusCode = from.StatusCode;
            to.Content = from.Content;
            to.Headers.Clear();
            foreach (var header in from.Headers)
            {
                to.Headers.Add(header.Key, header.Value);
            }

            to.GetProperties().Clear();
            foreach (var obj in from.GetProperties())
            {
                to.GetProperties().Add(obj);
            }
        }

    }
}
