// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System.ServiceModel.Description;

    public class MockFaultDescription : FaultDescription
    {
        public MockFaultDescription(string action) : base(action) { }
    }
}
