// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Utilities
{
    using System;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// MSTest utility for testing code that throws exceptions.
    /// </summary>
    public static class ExceptionAssert
    {
        /// <summary>
        /// Asserts that the code under test given by the <paramref name="codeThatThrows"/> parameter
        /// throws an exception of a given type.  
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of exception that the code under test should throw.</param>
        /// <param name="noExceptionMessage">The message to include with the failed <see cref="Assert"/> if the code under test does not throw an exception.</param>
        /// <param name="codeThatThrows">The code under test that is expected to throw an exception.</param>
        public static void Throws(Type exceptionType, string noExceptionMessage, Action codeThatThrows)
        {
            Throws(exceptionType, noExceptionMessage, codeThatThrows, paramName: null);
        }

        /// <summary>
        /// Asserts that the code under test given by the <paramref name="codeThatThrows"/> parameter
        /// throws an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <param name="noExceptionMessage">
        /// The message to include with the failed <see cref="Assert"/> if the code under test does not 
        /// throw an <see cref="ArgumentNullException"/>.
        /// </param>
        /// <param name="paramName">The name of the null parameter that results in the <see cref="ArgumentNullException"/>.</param>
        /// <param name="codeThatThrows">The code under test that is expected to throw an <see cref="ArgumentNullException"/>.</param>
        public static void ThrowsArgumentNull(string noExceptionMessage, string paramName, Action codeThatThrows)
        {
            Throws(typeof(ArgumentNullException), noExceptionMessage, codeThatThrows, paramName);
        }

        /// <summary>
        /// Asserts that the code under test given by the <paramref name="codeThatThrows"/> parameter
        /// throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="noExceptionMessage">
        /// The message to include with the failed <see cref="Assert"/> if the code under test does not 
        /// throw an <see cref="InvalidOperationException"/>.
        /// </param>
        /// <param name="codeThatThrows">The code under test that is expected to throw an <see cref="InvalidOperationException"/>.</param>
        public static void ThrowsInvalidOperation(string noExceptionMessage, Action codeThatThrows)
        {
            Throws(typeof(InvalidOperationException), noExceptionMessage, codeThatThrows, paramName: null);
        }

        /// <summary>
        /// Asserts that the code under test given by the <paramref name="codeThatThrows"/> parameter
        /// throws an exception of a given type with a ParamName value given by the <paramref name="paramName"/> parameter.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type"/> of exception that the code under test should throw.</param>
        /// <param name="noExceptionMessage">
        /// The message to include with the failed <see cref="Assert"/> if the code under test does not 
        /// throw an exception.
        /// </param>
        /// <param name="paramName">The name of the null parameter that results in the <see cref="ArgumentNullException"/>.</param>
        /// <param name="codeThatThrows">The code under test that is expected to throw an <see cref="ArgumentNullException"/>.</param>
        public static void Throws(Type exceptionType, string noExceptionMessage, Action codeThatThrows, string paramName)
        {
            Exception actualException = null;
            AssertFailedException assertFailedException = null;
            try
            {
                codeThatThrows();
            }
            catch (Exception exception)
            {
                actualException = exception;

                // Let assert failure in the callback escape these checks
                assertFailedException = exception as AssertFailedException;
                if (assertFailedException != null)
                {
                    throw;
                }
            }
            finally
            {
                if (assertFailedException == null)
                {
                    if (actualException == null)
                    {
                        Assert.Fail(noExceptionMessage);
                    }

                    Assert.IsInstanceOfType(
                        actualException,
                        exceptionType,
                        string.Format(
                            "Expected an exception of type '{0}' but encountered an exception of type '{1}' with the message: {2}.",
                            exceptionType.FullName,
                            actualException.GetType().FullName,
                            actualException.Message));

                    if (paramName != null)
                    {
                        PropertyInfo propInfo = actualException.GetType().GetProperty("ParamName");
                        string actualParamName = propInfo == null 
                                                    ? "<not available>" 
                                                    : propInfo.GetValue(actualException, null) as string;
                        Assert.AreEqual(
                            paramName,
                            actualParamName,
                            string.Format(
                                "Expected exception to have paramName='{0}' but found instead '{1}'",
                                paramName,
                                actualParamName));
                    }
                }
            }
        }
    }
}
