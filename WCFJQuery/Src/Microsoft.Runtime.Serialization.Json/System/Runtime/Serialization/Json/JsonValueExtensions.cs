// <copyright file="JsonValueExtensions.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using System.Json;
    using System.Xml;

    /// <summary>
    /// This class extends the funcionality of the <see cref="JsonValue"/> type. 
    /// </summary>
    public static class JsonValueExtensions
    {
        /// <summary>
        /// Creates a <see cref="System.Json.JsonValue"/> object based on an arbitrary CLR object.
        /// </summary>
        /// <param name="value">The object to be converted to <see cref="System.Json.JsonValue"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> which represents the given object.</returns>
        /// <remarks>The conversion is done through the <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>;
        /// the object is first serialized into JSON using the serializer, then parsed into a <see cref="System.Json.JsonValue"/>
        /// object.</remarks>
        public static JsonValue CreateFrom(object value)
        {
            JsonValue jsonValue = null;

            if (value != null)
            {
                jsonValue = value as JsonValue;

                if (jsonValue == null)
                {
                    jsonValue = JsonValueExtensions.CreatePrimitive(value);

                    if (jsonValue == null)
                    {
                        DataContractJsonSerializer dcjs = new DataContractJsonSerializer(value.GetType());
                        using (MemoryStream ms = new MemoryStream())
                        {
                            dcjs.WriteObject(ms, value);
                            ms.Position = 0;
                            jsonValue = JsonValue.Load(ms);
                        }
                    }
                }
            }

            return jsonValue;
        }

        /// <summary>
        /// Deserializes JSON from a XML reader which implements the
        /// <a href="http://msdn.microsoft.com/en-us/library/bb924435.aspx">mapping between JSON and XML</a>.
        /// </summary>
        /// <param name="jsonReader">The <see cref="System.Xml.XmlDictionaryReader"/> which
        /// exposes JSON as XML.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> that represents the parsed
        /// JSON/XML as a CLR type.</returns>
        public static JsonValue Load(XmlDictionaryReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonReader"));
            }

            return JXmlToJsonValueConverter.JXMLToJsonValue(jsonReader);
        }

        /// <summary>
        /// Serializes this <see cref="System.Json.JsonValue"/> CLR type into a JSON/XML writer using the
        /// <a href="http://msdn.microsoft.com/en-us/library/bb924435.aspx">mapping between JSON and XML</a>.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="jsonWriter">The JSON/XML writer used to serialize this instance.</param>
        public static void Save(this JsonValue jsonValue, XmlDictionaryWriter jsonWriter)
        {
            if (jsonValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonValue"));
            }

            if (jsonWriter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonWriter"));
            }

            JXmlToJsonValueConverter.JsonValueToJXML(jsonWriter, jsonValue);
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="valueOfT">An instance of T initialized with this instance, or the default
        /// value of T, if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonValue"/> instance can be read as type T; otherwise, false.</returns>
        public static bool TryReadAsComplex<T>(this JsonValue jsonValue, out T valueOfT)
        {
            if (jsonValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonValue"));
            }

            object value;
            if (jsonValue.TryReadAsComplex(typeof(T), out value))
            {
                valueOfT = (T)value;
                return true;
            }

            valueOfT = default(T);
            return false;
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <returns>An instance of T initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.NotSupportedException">If this <see cref="System.Json.JsonValue"/> value cannot be
        /// converted into the type T.</exception>
        public static T ReadAsComplex<T>(this JsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonValue"));
            }

            return (T)jsonValue.ReadAsComplex(typeof(T));
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <returns>An object instance initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.NotSupportedException">If this <see cref="System.Json.JsonValue"/> value cannot be
        /// converted into the type T.</exception>
        public static object ReadAsComplex(this JsonValue jsonValue, Type type)
        {
            if (jsonValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonValue"));
            }

            object result;
            if (JsonValueExtensions.TryReadAsComplex(jsonValue, type, out result))
            {
                return result;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="jsonValue">The <see cref="JsonValue"/> instance this method extension is to be applied to.</param>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <param name="value">An object to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonValue"/> instance can be read as the specified type; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate",
                    Justification = "This is the non-generic version of the method.")]
        public static bool TryReadAsComplex(this JsonValue jsonValue, Type type, out object value)
        {
            if (jsonValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("jsonValue"));
            }

            if (type == typeof(JsonValue) || type == typeof(object))
            {
                value = jsonValue;
                return true;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonValue.Save(ms);
                    ms.Position = 0;
                    DataContractJsonSerializer dcjs = new DataContractJsonSerializer(type);
                    value = dcjs.ReadObject(ms);
                }

                return true;
            }
            catch (Exception e)
            {
                if (DiagnosticUtility.IsFatal(e))
                {
                    throw;
                }

                value = null;
                return false;
            }
        }

        private static JsonValue CreatePrimitive(object value)
        {
            Type type = value.GetType();
            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return new JsonPrimitive((bool)value);

                case TypeCode.Byte:
                    return new JsonPrimitive((byte)value);

                case TypeCode.Char:
                    return new JsonPrimitive((char)value);

                case TypeCode.DateTime:
                    return new JsonPrimitive((DateTime)value);

                case TypeCode.Decimal:
                    return new JsonPrimitive((decimal)value);

                case TypeCode.Double:
                    return new JsonPrimitive((double)value);

                case TypeCode.Int16:
                    return new JsonPrimitive((short)value);

                case TypeCode.Int32:
                    return new JsonPrimitive((int)value);

                case TypeCode.Int64:
                    return new JsonPrimitive((long)value);

                case TypeCode.SByte:
                    return new JsonPrimitive((sbyte)value);

                case TypeCode.Single:
                    return new JsonPrimitive((float)value);

                case TypeCode.String:
                    return new JsonPrimitive((string)value);

                case TypeCode.UInt16:
                    return new JsonPrimitive((ushort)value);

                case TypeCode.UInt32:
                    return new JsonPrimitive((uint)value);

                case TypeCode.UInt64:
                    return new JsonPrimitive((ulong)value);

                default:
                    if (type == typeof(DateTimeOffset))
                    {
                        return new JsonPrimitive((DateTimeOffset)value);
                    }

                    if (type == typeof(Guid))
                    {
                        return new JsonPrimitive((Guid)value);
                    }

                    if (type == typeof(Uri))
                    {
                        return new JsonPrimitive((Uri)value);
                    }

                    return null;
            }
        }
    }
}
