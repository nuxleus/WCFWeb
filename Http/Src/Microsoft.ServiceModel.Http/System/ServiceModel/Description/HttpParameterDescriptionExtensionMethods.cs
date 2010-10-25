// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Web
{
    using System.ServiceModel.Description;

    /// <summary>
    /// Provides extension methods for <see cref="MessagePartDescription"/>
    /// to translate to <see cref="HttpParameterDescription"/>.
    /// </summary>
    public static class HttpParameterDescriptionExtensionMethods
    {
        /// <summary>
        /// Creates a new <see cref="HttpParameterDescription"/> instance from the given
        /// <see cref="MessagePartDescription"/>.
        /// </summary>
        /// <param name="description">The <see cref="MessagePartDescription"/> to use.</param>
        /// <returns>A new <see cref="HttpParameterDescription"/> instance.</returns>
        public static HttpParameterDescription ToHttpParameterDescription(this MessagePartDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            return new HttpParameterDescription(description);
        }
    }
}
