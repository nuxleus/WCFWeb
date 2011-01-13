// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using Microsoft.Internal;

    public static class JsonContentExtensions
    {
        public static T ReadAsJsonDataContract<T>(this HttpContent content)
        {
            FX.ThrowIfNull(content, "content");

            return (T)ReadAsJsonDataContract(content, new DataContractJsonSerializer(typeof(T)));
        }

        public static T ReadAsJsonDataContract<T>(this HttpContent content, params Type[] extraTypes)
        {
            FX.ThrowIfNull(content, "content");

            return (T)ReadAsJsonDataContract(content, new DataContractJsonSerializer(typeof(T), extraTypes));
        }

        public static object ReadAsJsonDataContract(this HttpContent content, DataContractJsonSerializer serializer)
        {
            FX.ThrowIfNull(content, "content");
            FX.ThrowIfNull(serializer, "serializer");

            using (var r = content.ContentReadStream)
            {
                return serializer.ReadObject(r);
            }
        }

        public static T ReadAsJsonDataContract<T>(this HttpContent content, DataContractJsonSerializer serializer)
        {
            FX.ThrowIfNull(content, "content");
            FX.ThrowIfNull(serializer, "serializer");

            using (var r = content.ContentReadStream)
            {
                return (T)serializer.ReadObject(r);
            }
        }

        public static HttpContent ToContentUsingDataContractJsonSerializer(this object instance, params Type[] knownTypes)
        {
            FX.ThrowIfNull(instance, "instance");

            var serializer = new DataContractJsonSerializer(instance.GetType(), knownTypes);
            return ToContentUsingDataContractJsonSerializer(instance, serializer);
        }

        public static HttpContent ToContentUsingDataContractJsonSerializer(this object instance, DataContractJsonSerializer serializer)
        {
            FX.ThrowIfNull(instance, "instance");
            FX.ThrowIfNull(serializer, "serializer");

            var stream = new MemoryStream();
            try
            {
                serializer.WriteObject(stream, instance);
                stream.Position = 0;
                return new StreamContent(stream);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }
    }
}
