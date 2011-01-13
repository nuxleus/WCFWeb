// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.UnitTests.Mocks
{
    using System;
    using System.Xml.Serialization;

    public class MockXmlSerializer : XmlSerializer
    {
        public MockXmlSerializer(Type type)
        {
            
        }
        protected override void Serialize(object o, XmlSerializationWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override object Deserialize(XmlSerializationReader reader)
        {
            throw new NotImplementedException();
        }

        public override bool CanDeserialize(System.Xml.XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }

        protected override XmlSerializationReader CreateReader()
        {
            throw new NotImplementedException();
        }

        protected override XmlSerializationWriter CreateWriter()
        {
            throw new NotImplementedException();
        }

    }
}