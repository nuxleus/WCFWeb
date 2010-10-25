// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Client
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.QueryComposition.Client;

    /// <summary>
    /// This class implements the default expression to uri translation logic
    /// </summary>
    public class WebExpressionConverter : IExpressionToUriConverter
    {
        // static instantiation singleton pattern
        private static readonly WebExpressionConverter instance = new WebExpressionConverter();

        /// <summary>
        /// Construct an instance of the expression converter
        /// </summary>
        public WebExpressionConverter()
        { 
        }

        /// <summary>
        /// Gets an staic instance of the expression converter
        /// </summary>
        public static WebExpressionConverter Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Converts expression to Uri
        /// </summary>
        /// <param name="e">The expression</param>
        /// <returns>Converted uri</returns>
        public virtual Uri Convert(Expression e)
        {
            Uri uri;

            Dictionary<Expression, Expression> rewrites = null;
            if (!(e is ResourceSetExpression))
            {
                rewrites = new Dictionary<Expression, Expression>(ReferenceEqualityComparer<Expression>.Instance);
                e = Evaluator.PartialEval(e);
                e = ExpressionNormalizer.Normalize(e, rewrites);
                e = ResourceBinder.Bind(e);
            }

            UriWriter.Translate(false, e, out uri);

            return uri;
        }
    }
}
