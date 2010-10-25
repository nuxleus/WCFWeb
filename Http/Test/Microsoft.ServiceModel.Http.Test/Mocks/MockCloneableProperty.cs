// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;

    public class MockCloneableProperty : ICloneable
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public object Clone()
        {
            return new MockCloneableProperty { Id = this.Id, Name = this.Name };
        }
    }
}
