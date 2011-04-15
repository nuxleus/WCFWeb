//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.16613
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.ApplicationServer.ServiceModel {
    
    
    internal partial class SR {
        
        static System.Resources.ResourceManager resourceManager;
        
        static System.Globalization.CultureInfo resourceCulture;
        
        private SR() {
        }
        
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceManager, null)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Microsoft.ApplicationServer.ServiceModel.SR", typeof(SR).Assembly);
                    resourceManager = temp;
                }
                return resourceManager;
            }
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("StrictResXFileCodeGenerator", "4.0.0.0")]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>Gets localized string like: Certificate-based client authentication is not supported in TransportCredentialOnly security mode. Select the Transport security mode.</summary>
        internal static string CertificateUnsupportedForHttpTransportCredentialOnly {
            get {
                return ResourceManager.GetString("CertificateUnsupportedForHttpTransportCredentialOnly", Culture);
            }
        }
        
        /// <summary>Gets localized string like: Cannot load the X.509 certificate identity specified in the configuration</summary>
        internal static string UnableToLoadCertificateIdentity {
            get {
                return ResourceManager.GetString("UnableToLoadCertificateIdentity", Culture);
            }
        }
        
        /// <summary>Gets localized string like: The endpoint specified cannot be null or an empty string.  Please specify a valid endpoint.  Valid endpoint values can be found in the system.serviceModel/extensions/endpointExtensions collection.</summary>
        internal static string ConfigEndpointTypeCannotBeNullOrEmpty {
            get {
                return ResourceManager.GetString("ConfigEndpointTypeCannotBeNullOrEmpty", Culture);
            }
        }
        
        /// <summary>Gets localized string like: The inner listener factory of {0} must be set before this operation.</summary>
        /// <param name="param0">Parameter 0 for string: The inner listener factory of {0} must be set before this operation.</param>
        internal static string InnerListenerFactoryNotSet(object param0) {
            return string.Format(Culture, ResourceManager.GetString("InnerListenerFactoryNotSet", Culture), param0);
        }
        
        /// <summary>Gets localized string like: The type {0} registered as a WSDL extension does not implement IWsdlImportExtension.</summary>
        /// <param name="param0">Parameter 0 for string: The type {0} registered as a WSDL extension does not implement IWsdlImportExtension.</param>
        internal static string InvalidWsdlExtensionTypeInConfig(object param0) {
            return string.Format(Culture, ResourceManager.GetString("InvalidWsdlExtensionTypeInConfig", Culture), param0);
        }
        
        /// <summary>Gets localized string like: The type {0} registered as a WSDL extension does not have a public default constructor. WSDL extensions must have a public default constructor.</summary>
        /// <param name="param0">Parameter 0 for string: The type {0} registered as a WSDL extension does not have a public default constructor. WSDL extensions must have a public default constructor.</param>
        internal static string WsdlExtensionTypeRequiresDefaultConstructor(object param0) {
            return string.Format(Culture, ResourceManager.GetString("WsdlExtensionTypeRequiresDefaultConstructor", Culture), param0);
        }
        
        /// <summary>Gets localized string like: Could not find default endpoint element that references contract '{0}' in the ServiceModel client configuration section. This might be because no configuration file was found for your application, or because no endpoint element matching this contract could be found in the client element.</summary>
        /// <param name="param0">Parameter 0 for string: Could not find default endpoint element that references contract '{0}' in the ServiceModel client configuration section. This might be because no configuration file was found for your application, or because no endpoint element matching this contract could be found in the client element.</param>
        internal static string SFxConfigContractNotFound(object param0) {
            return string.Format(Culture, ResourceManager.GetString("SFxConfigContractNotFound", Culture), param0);
        }
        
        /// <summary>Gets localized string like: Could not find endpoint element with name '{0}' and contract '{1}' in the ServiceModel client configuration section. This might be because no configuration file was found for your application, or because no endpoint element matching this name could be found in the client element.</summary>
        /// <param name="param0">Parameter 0 for string: Could not find endpoint element with name '{0}' and contract '{1}' in the ServiceModel client configuration section. This might be because no configuration file was found for your application, or because no endpoint element matching this name could be found in the client element.</param>
        /// <param name="param1">Parameter 1 for string: Could not find endpoint element with name '{0}' and contract '{1}' in the ServiceModel client configuration section. This might be because no configuration file was found for your application, or because no endpoint element matching this name could be found in the client element.</param>
        internal static string SFxConfigChannelConfigurationNotFound(object param0, object param1) {
            return string.Format(Culture, ResourceManager.GetString("SFxConfigChannelConfigurationNotFound", Culture), param0, param1);
        }
        
        /// <summary>Gets localized string like: The '{0}' configuration section cannot be created. The machine.config file is missing information. Verify that this configuration section is properly registered and that you have correctly spelled the section name. For Windows Communication Foundation sections, run ServiceModelReg.exe -i to fix this error.</summary>
        /// <param name="param0">Parameter 0 for string: The '{0}' configuration section cannot be created. The machine.config file is missing information. Verify that this configuration section is properly registered and that you have correctly spelled the section name. For Windows Communication Foundation sections, run ServiceModelReg.exe -i to fix this error.</param>
        internal static string ConfigSectionNotFound(object param0) {
            return string.Format(Culture, ResourceManager.GetString("ConfigSectionNotFound", Culture), param0);
        }
        
        /// <summary>Gets localized string like: Configuration endpoint extension '{0}' could not be found. Verify that this endpoint extension is properly registered in system.serviceModel/extensions/endpointExtensions and that it is spelled correctly.</summary>
        /// <param name="param0">Parameter 0 for string: Configuration endpoint extension '{0}' could not be found. Verify that this endpoint extension is properly registered in system.serviceModel/extensions/endpointExtensions and that it is spelled correctly.</param>
        internal static string ConfigEndpointExtensionNotFound(object param0) {
            return string.Format(Culture, ResourceManager.GetString("ConfigEndpointExtensionNotFound", Culture), param0);
        }
        
        /// <summary>Gets localized string like: The type {0} registered as a policy extension does not implement IPolicyImportExtension</summary>
        /// <param name="param0">Parameter 0 for string: The type {0} registered as a policy extension does not implement IPolicyImportExtension</param>
        internal static string InvalidPolicyExtensionTypeInConfig(object param0) {
            return string.Format(Culture, ResourceManager.GetString("InvalidPolicyExtensionTypeInConfig", Culture), param0);
        }
        
        /// <summary>Gets localized string like: UriTemplateTable (with allowDuplicateEquivalentUriTemplates = false) does not support both '{0}' and '{1}', since they are equivalent. Call MakeReadOnly with allowDuplicateEquivalentUriTemplates = true to use both of these UriTemplates in the same table. See the documentation for UriTemplateTable for more detail.</summary>
        /// <param name="param0">Parameter 0 for string: UriTemplateTable (with allowDuplicateEquivalentUriTemplates = false) does not support both '{0}' and '{1}', since they are equivalent. Call MakeReadOnly with allowDuplicateEquivalentUriTemplates = true to use both of these UriTemplates in the same table. See the documentation for UriTemplateTable for more detail.</param>
        /// <param name="param1">Parameter 1 for string: UriTemplateTable (with allowDuplicateEquivalentUriTemplates = false) does not support both '{0}' and '{1}', since they are equivalent. Call MakeReadOnly with allowDuplicateEquivalentUriTemplates = true to use both of these UriTemplates in the same table. See the documentation for UriTemplateTable for more detail.</param>
        internal static string UTTDuplicate(object param0, object param1) {
            return string.Format(Culture, ResourceManager.GetString("UTTDuplicate", Culture), param0, param1);
        }
        
        /// <summary>Gets localized string like: Operation '{0}' in contract '{1}' has both '{2}' and '{3}'; only one can be present.</summary>
        /// <param name="param0">Parameter 0 for string: Operation '{0}' in contract '{1}' has both '{2}' and '{3}'; only one can be present.</param>
        /// <param name="param1">Parameter 1 for string: Operation '{0}' in contract '{1}' has both '{2}' and '{3}'; only one can be present.</param>
        /// <param name="param2">Parameter 2 for string: Operation '{0}' in contract '{1}' has both '{2}' and '{3}'; only one can be present.</param>
        /// <param name="param3">Parameter 3 for string: Operation '{0}' in contract '{1}' has both '{2}' and '{3}'; only one can be present.</param>
        internal static string MultipleWebAttributes(object param0, object param1, object param2, object param3) {
            return string.Format(Culture, ResourceManager.GetString("MultipleWebAttributes", Culture), param0, param1, param2, param3);
        }
        
        /// <summary>Gets localized string like: A child element named '{0}' with same key already exists at the same configuration scope. Collection elements must be unique within the same configuration scope (e.g. the same application.config file). Duplicate key value:  '{1}'.</summary>
        /// <param name="param0">Parameter 0 for string: A child element named '{0}' with same key already exists at the same configuration scope. Collection elements must be unique within the same configuration scope (e.g. the same application.config file). Duplicate key value:  '{1}'.</param>
        /// <param name="param1">Parameter 1 for string: A child element named '{0}' with same key already exists at the same configuration scope. Collection elements must be unique within the same configuration scope (e.g. the same application.config file). Duplicate key value:  '{1}'.</param>
        internal static string ConfigDuplicateKeyAtSameScope(object param0, object param1) {
            return string.Format(Culture, ResourceManager.GetString("ConfigDuplicateKeyAtSameScope", Culture), param0, param1);
        }
        
        /// <summary>Gets localized string like: No elements matching the key '{0}' were found in the configuration element collection.</summary>
        /// <param name="param0">Parameter 0 for string: No elements matching the key '{0}' were found in the configuration element collection.</param>
        internal static string ConfigKeyNotFoundInElementCollection(object param0) {
            return string.Format(Culture, ResourceManager.GetString("ConfigKeyNotFoundInElementCollection", Culture), param0);
        }
        
        /// <summary>Gets localized string like: The key does not match the indexer key. When setting the value of a specific index, the key of the desired value must match the index at which it is being set. Key on element (expected value): {0}. Key provided to indexer: {1}.</summary>
        /// <param name="param0">Parameter 0 for string: The key does not match the indexer key. When setting the value of a specific index, the key of the desired value must match the index at which it is being set. Key on element (expected value): {0}. Key provided to indexer: {1}.</param>
        /// <param name="param1">Parameter 1 for string: The key does not match the indexer key. When setting the value of a specific index, the key of the desired value must match the index at which it is being set. Key on element (expected value): {0}. Key provided to indexer: {1}.</param>
        internal static string ConfigKeysDoNotMatch(object param0, object param1) {
            return string.Format(Culture, ResourceManager.GetString("ConfigKeysDoNotMatch", Culture), param0, param1);
        }
        
        /// <summary>Gets localized string like: Start must be between 0 and {0}. Value passed in is {1}.</summary>
        /// <param name="param0">Parameter 0 for string: Start must be between 0 and {0}. Value passed in is {1}.</param>
        /// <param name="param1">Parameter 1 for string: Start must be between 0 and {0}. Value passed in is {1}.</param>
        internal static string ConfigInvalidStartValue(object param0, object param1) {
            return string.Format(Culture, ResourceManager.GetString("ConfigInvalidStartValue", Culture), param0, param1);
        }
        
        /// <summary>Gets localized string like: The '{0}' configuration element key cannot be null.</summary>
        /// <param name="param0">Parameter 0 for string: The '{0}' configuration element key cannot be null.</param>
        internal static string ConfigElementKeyNull(object param0) {
            return string.Format(Culture, ResourceManager.GetString("ConfigElementKeyNull", Culture), param0);
        }
        
        /// <summary>Gets localized string like: At least one of the configuration element keys '{0}' must not be null.</summary>
        /// <param name="param0">Parameter 0 for string: At least one of the configuration element keys '{0}' must not be null.</param>
        internal static string ConfigElementKeysNull(object param0) {
            return string.Format(Culture, ResourceManager.GetString("ConfigElementKeysNull", Culture), param0);
        }
        
        /// <summary>Gets localized string like: UriTemplate does not support '{0}' as a valid format for a segment or a query part.</summary>
        /// <param name="param0">Parameter 0 for string: UriTemplate does not support '{0}' as a valid format for a segment or a query part.</param>
        internal static string UTInvalidFormatSegmentOrQueryPart(object param0) {
            return string.Format(Culture, ResourceManager.GetString("UTInvalidFormatSegmentOrQueryPart", Culture), param0);
        }
    }
}
