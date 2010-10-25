// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Client
{
    using System.Linq.Expressions;

    /// <summary>
    /// The Expression to Uri converter interface
    /// </summary>
    public interface IExpressionToUriConverter
    {
        /// <summary>
        /// Converts expression to uri
        /// </summary>
        /// <param name="exp">The expression</param>
        /// <returns>uri which respresents the expression</returns>
        Uri Convert(Expression exp);
    }
}
