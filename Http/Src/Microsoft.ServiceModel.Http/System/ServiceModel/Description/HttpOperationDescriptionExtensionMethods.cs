// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Web
{
    using System.ServiceModel.Description;

    /// <summary>
    /// Provides extension methods for <see cref="OperationDescription"/> instances.
    /// </summary>
    public static class HttpOperationDescriptionExtensionMethods
    {
        /// <summary>
        /// Creates an <see cref="HttpOperationDescription"/> instance based on the given
        /// <see cref="OperationDescription"/> instance.
        /// </summary>
        /// <param name="description">The <see cref="OperationDescription"/> instance.</param>
        /// <returns>A new <see cref="HttpOperationDescription"/> instance.</returns>
        public static HttpOperationDescription ToHttpOperationDescription(this OperationDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            return new HttpOperationDescription(description);
        }
    }
}
