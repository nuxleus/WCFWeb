// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageBindingConfigurationTests
    {
        [TestMethod]
        [Description("The HttpMessageBinding constructor throws when there is no config file for the AppDomain.")]
        public void HttpMessageBinding_Throws_When_No_Config_File()
        {
            ExceptionAssert.Throws(
                typeof(ConfigurationErrorsException),
                "The HttpMessageBinding should have thrown since there is no config file for the AppDomain.",
                () =>
                {
                    new HttpMessageBinding("configuredBinding");
                });
        }

        [TestMethod]
        [Description("The HttpMessageBinding constructor throws when there is no binding section in the config file.")]
        [DeploymentItem("ConfigFiles\\NoBindingConfigurationTest.config")]
        public void HttpMessageBinding_Throws_When_No_Binding_Section()
        {
            ConfigAssert.Execute("NoBindingConfigurationTest.config", () =>
            {
                ExceptionAssert.Throws(
                    typeof(ConfigurationErrorsException),
                    "The HttpMessageBinding should have thrown since there is no binding section in the config file.",
                    () =>
                    {
                        new HttpMessageBinding("configuredBinding");
                    });
            });
        }

        [TestMethod]
        [Description("The HttpMessageBinding constructor throws with an unknown configurationName parameter.")]
        [DeploymentItem("ConfigFiles\\BindingConfigurationTest.config")]
        public void HttpMessageBinding_Throws_With_Unknown_ConfigurationName_Parameter()
        {
            ConfigAssert.Execute("BindingConfigurationTest.config", () =>
            {
                ExceptionAssert.Throws(
                    typeof(ConfigurationErrorsException),
                    "The HttpMessageBinding should have thrown since the configurationName was unknown.",
                    () =>
                    {
                        new HttpMessageBinding("noSuchConfiguredBinding");
                    });
            });
        }
    }
}
