// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Json;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel.Description;

    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.ServiceModel.Http;
    using Microsoft.ServiceModel.Web;

    public class FormUrlEncodedProcessor : MediaTypeProcessor
    {
        private bool isJsonValueParameter;
        private Type parameterType;

        public FormUrlEncodedProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, MediaTypeProcessorMode.Request)
        {
            if (mode == MediaTypeProcessorMode.Response)
            {
                throw new ArgumentException("mode", "This processor cannot be used in the response");
            }

            if (this.Parameter != null)
            {
                this.parameterType = Parameter.ParameterType;
                this.isJsonValueParameter = typeof(JsonValue).IsAssignableFrom(this.parameterType);
            }
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get
            {
                return new List<string> { "application/x-www-form-urlencoded" };
            }
        }

        public override void WriteToStream(object instance, Stream stream, HttpRequestMessage request)
        {
            throw new NotSupportedException();
        }

        public override object ReadFromStream(Stream stream, HttpRequestMessage request)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var reader = new StreamReader(stream);
            var jsonContent = reader.ReadToEnd();
            var jsonObject = FormUrlEncodedExtensions.ParseFormUrlEncoded(jsonContent);

            if (this.isJsonValueParameter)
            {
                return jsonObject;
            }

            var tempStream = new MemoryStream();
            jsonObject.Save(tempStream);
            tempStream.Position = 0;
            var serializer = new DataContractJsonSerializer(Parameter.ParameterType);
            return serializer.ReadObject(tempStream);
        }
    }
}