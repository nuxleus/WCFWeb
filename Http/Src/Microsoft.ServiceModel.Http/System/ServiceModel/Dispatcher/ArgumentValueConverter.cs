// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    internal abstract class ArgumentValueConverter<T>
    {
        public abstract T ConvertFrom(object value);
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "We allow generic classes and non-generic classes live in the same file")]
    internal class ArgumentValueConverter
    {
        public static ArgumentValueConverter<T> CreateValueConverter<T>()
        {
            return 
                (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                ? (ArgumentValueConverter<T>)new ReferenceTypeConverter<T>()
                : typeof(T).IsValueType
                    ? (ArgumentValueConverter<T>)new ValueTypeConverter<T>()
                    : (ArgumentValueConverter<T>)new ReferenceTypeConverter<T>();
        }

        internal class ReferenceTypeConverter<T> : ArgumentValueConverter<T>
        {
            public override T ConvertFrom(object value)
            {
                if (value == null || typeof(T).IsInstanceOfType(value))
                {
                    return (T)value;
                }

                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SR.ValueCannotBeConverted,
                        value.ToString(),
                        typeof(T).Name));
            }
        }

        internal class ValueTypeConverter<T> : ArgumentValueConverter<T>
        {
            public override T ConvertFrom(object value)
            {
                if (value == null)
                {
                    return default(T);
                }

                if (typeof(T).IsInstanceOfType(value))
                {
                    return (T)value;
                }

                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SR.ValueCannotBeConverted,
                        ((value == null) ? "null" : value.ToString()),
                        typeof(T).Name));
            }
        }
    }
}