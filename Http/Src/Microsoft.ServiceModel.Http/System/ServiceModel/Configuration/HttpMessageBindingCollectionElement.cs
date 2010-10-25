// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Represents a configuration element that contains sub-elements that specify settings for using 
    /// the <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> binding.
    /// </summary>
    public class HttpMessageBindingCollectionElement : StandardBindingCollectionElement<HttpMessageBinding, HttpMessageBindingElement>
    {
        private const string SectionPath = "system.serviceModel/bindings";

        internal static HttpMessageBindingCollectionElement GetBindingCollectionElement()
        {
            BindingsSection bindings = ConfigurationManager.GetSection(SectionPath) as BindingsSection;

            if (bindings != null)
            {            
                foreach (BindingCollectionElement bindingCollection in bindings.BindingCollections)
                {
                    if (bindingCollection.BindingName == HttpMessageBinding.CollectionElementName)
                    {
                        return bindingCollection as HttpMessageBindingCollectionElement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a default <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> binding.
        /// </summary>
        /// <returns>
        /// A default <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> binding.
        /// </returns>
        protected override Binding GetDefault()
        {
            return new HttpMessageBinding();
        }
    }
}
