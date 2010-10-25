// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;
    using System.Collections;

    public class MockQueryComposer : IQueryComposer
    {
        IEnumerable IQueryComposer.ComposeQuery(IEnumerable rootedQuery, string url)
        {
            throw new InvalidOperationException("This is the MockQueryComposer");
        }
    }
}
