// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test
{
    using System.Collections.Generic;
    using Microsoft.Net.Http.Extensions.Test.UnitTests;

    public class CustomerWithItems : Customer
    {
        public List<object> Items { get; set; }
    }
}