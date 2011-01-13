// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Xml.Serialization
{
    using System;
    using System.Xml.Serialization;
    using System.IO;
    using System.Net.Http;
    using Microsoft.Internal;

    public static class XmlSerializerContentExtensions
    {
        public static T ReadAsXmlSerializable<T>(this HttpContent content)
        {
            FX.ThrowIfNull(content, "content");

            return ReadAsXmlSerializable<T>(content, new XmlSerializer(typeof(T)));
        }

        public static T ReadAsXmlSerializable<T>(this HttpContent content, params Type[] extraTypes)
        {
            FX.ThrowIfNull(content, "content");

            return ReadAsXmlSerializable<T>(content, new XmlSerializer(typeof(T), extraTypes));
        }

        public static T ReadAsXmlSerializable<T>(this HttpContent content, XmlSerializer serializer)
        {
            FX.ThrowIfNull(content, "content");
            FX.ThrowIfNull(serializer, "serializer");

            using (var r = content.ReadAsXmlReader())
            {
                return (T)serializer.Deserialize(r);
            }
        }

        public static object ReadAsXmlSerializable(this HttpContent content, XmlSerializer serializer)
        {
            FX.ThrowIfNull(content, "content");
            FX.ThrowIfNull(serializer, "serialzer");

            using (var r = content.ReadAsXmlReader())
            {
                return serializer.Deserialize(r);
            }
        }

        public static HttpContent ToContentUsingXmlSerializer(this object instance)
        {
            FX.ThrowIfNull(instance, "instance");

            var serializer = new XmlSerializer(instance.GetType());
            return ToContentUsingXmlSerializer(instance, serializer);
        }

        public static HttpContent ToContentUsingXmlSerializer(this object instance, XmlSerializer serializer)
        {
            var stream = new MemoryStream();
            try
            {
                serializer.Serialize(stream, instance);
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
