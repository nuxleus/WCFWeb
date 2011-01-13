// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http.Headers;

    public interface IContentFormatter
    {
        IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes { get; }

        void WriteToStream(object instance, Stream stream);

        object ReadFromStream(Stream stream);
    }
}
