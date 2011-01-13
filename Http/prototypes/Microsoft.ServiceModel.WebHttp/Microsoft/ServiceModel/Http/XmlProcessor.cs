// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Web;
    using System.Xml.Serialization;

    using System.Net.Http;

    public class XmlProcessor : MediaTypeProcessor
    {
        private bool usesQueryComposition;
        private Type queryCompositionType;

        public XmlProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, mode)
        {
            var returnType = operation.ReturnValue;

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
                return new List<string> { "text/xml", "application/xml" };
            }
        }

        public override void WriteToStream(object instance, System.IO.Stream stream, HttpRequestMessage request)
        { 
            //IQueryable support
            if (this.usesQueryComposition)
            {
                //wrap it in a list
                instance = Activator.CreateInstance(this.queryCompositionType, instance);
                var serializer = new DataContractSerializer(this.queryCompositionType);
                serializer.WriteObject(stream, instance);
            }
            else
            {
                var serializer = new XmlSerializer(Parameter.ParameterType);
                serializer.Serialize(stream, instance);
            }
        }

        public override object ReadFromStream(System.IO.Stream stream, HttpRequestMessage request)
        {
            var serializer = new XmlSerializer(Parameter.ParameterType);
            return serializer.Deserialize(stream);
        }
    }
}
