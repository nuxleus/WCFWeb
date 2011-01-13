// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.Mocks
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http.Headers;

    public class MockContentFormatter : IContentFormatter
    {
        private MediaTypeHeaderValue _mediaType;

        public MockContentFormatter(MediaTypeHeaderValue mediaType)
        {
            _mediaType = mediaType;
        }

        public bool WriteToStreamCalled { get; set; }

        public void WriteToStream(object instance, Stream stream)
        {
            WriteToStreamCalled = true;
        }

        public bool ReadFromStreamCalled { get; set; }

        public object ReadFromStream(Stream stream)
        {
            ReadFromStreamCalled = true;
            return null;
        }

        public IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes
        {
            get { yield return _mediaType; }
        }
    }
}
