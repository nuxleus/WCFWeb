﻿// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Services
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class InvalidParameterService1
    {
        [WebGet()]
        public string Operation(MemoryStream stream)
        {
            return null;
        }
    }
}
