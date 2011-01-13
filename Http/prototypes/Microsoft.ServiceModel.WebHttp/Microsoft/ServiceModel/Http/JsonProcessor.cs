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
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading;

    using System.Net.Http;
    using Microsoft.ServiceModel.Http;

    public class JsonProcessor : MediaTypeProcessor
    {
        private bool isJsonValueParameter;
        private Type parameterType;
        private bool usesQueryComposition;
        private Type queryCompositionType;

        public JsonProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, mode)
        {
            if (this.Parameter != null)
            {
                this.parameterType = this.Parameter.ParameterType;
                this.isJsonValueParameter = typeof(JsonValue).IsAssignableFrom(this.parameterType);
            }

            //IQueryable support
            if (operation.Behaviors.Contains(typeof(QueryCompositionAttribute)))
            {
                usesQueryComposition = true;
                var queryCompositionItemType = operation.ReturnValue.ParameterType.GetGenericArguments()[0];
                queryCompositionType = typeof(List<>).MakeGenericType(queryCompositionItemType);
            }
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get
            {
                return new List<string> { "text/json", "application/json" };
            }
        }

        public override void WriteToStream(object instance, Stream stream, HttpRequestMessage request)
        {
            JsonValue value = null;

            if (this.usesQueryComposition)
            {
                instance = Activator.CreateInstance(this.queryCompositionType, instance);
                var serializer = new DataContractJsonSerializer(this.queryCompositionType);
                serializer.WriteObject(stream, instance);
            }
            else
            {
                if (this.isJsonValueParameter)
                {
                    value = (JsonValue)instance;
                    value.Save(stream);
                }
                else
                {
                    var serializer = new DataContractJsonSerializer(Parameter.ParameterType);
                    serializer.WriteObject(stream, instance);
                }
            }
        }

        public override object ReadFromStream(Stream stream, HttpRequestMessage request)
        {
            var reader = new StreamReader(stream);
            var jsonObject = JsonValue.Load(stream);

            if (this.isJsonValueParameter)
            {
                return jsonObject;
            }

            var serializer = new DataContractJsonSerializer(Parameter.ParameterType);
            stream.Position = 0;
            return serializer.ReadObject(stream);
        }
    }
}