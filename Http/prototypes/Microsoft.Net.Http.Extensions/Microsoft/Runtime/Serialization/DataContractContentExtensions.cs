// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Runtime.Serialization
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Microsoft.Internal;

    public static class DataContractContentExtensions
    {
        public static T ReadAsDataContract<T>(this HttpContent content)
        {
            FX.ThrowIfNull(content, "content");

            return ReadAsDataContract<T>(content, new DataContractSerializer(typeof(T)));
        }

        public static T ReadAsDataContract<T>(this HttpContent content, params Type[] extraTypes)
        {
            FX.ThrowIfNull(content, "content");
            
            return ReadAsDataContract<T>(content, new DataContractSerializer(typeof(T), extraTypes));
        }

        public static T ReadAsDataContract<T>(this HttpContent content, DataContractSerializer serializer)
        {
            FX.ThrowIfNull(content, "content");
            FX.ThrowIfNull(content, "serializer");

            using (var stream = content.ContentReadStream)
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        public static HttpContent ToContentUsingDataContractSerializer(this object instance, params Type[] knownTypes)
        {
            FX.ThrowIfNull(instance, "instance");

            var serializer = new DataContractSerializer(instance.GetType(), knownTypes);
            return ToContentUsingDataContractSerializer(instance, serializer);
        }

        public static HttpContent ToContentUsingDataContractSerializer(this object instance, DataContractSerializer serializer)
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
