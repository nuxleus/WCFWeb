﻿// <copyright file="JsonValue.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// This is the base class for JavaScript Object Notation (JSON) common language runtime (CLR) types. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "JsonValue is by definition either a collection or a single object.")]
    public class JsonValue : IEnumerable<KeyValuePair<string, JsonValue>>, IDynamicMetaObjectProvider
    {
        private static object lockKey = new object();
        private static JsonValue defaultInstance;

        internal JsonValue()
        {
        }

        /// <summary>
        /// Gets the JSON CLR type represented by this instance.
        /// </summary>
        public virtual JsonType JsonType
        {
            get { return JsonType.Default; }
        }

        /// <summary>
        /// Gets the number of items in this object.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return 0;
            }
        }

        private static JsonValue DefaultInstance
        {
            get
            {
                if (defaultInstance == null)
                {
                    lock (lockKey)
                    {
                        if (defaultInstance == null)
                        {
                            defaultInstance = new JsonValue();
                        }
                    }
                }

                return defaultInstance;
            }
        }

        /// <summary>
        /// This indexer is not supported for this base class and throws an exception.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns><see cref="System.Json.JsonValue"/>.</returns>
        /// <remarks>The exception thrown is the <see cref="System.InvalidOperationException"/>.
        /// This method is overloaded in the implementation of the <see cref="System.Json.JsonObject"/>
        /// class, which inherits from this class.</remarks>
        public virtual JsonValue this[string key]
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(DiagnosticUtility.GetString(SR.UnsupportedOnThisJsonValue, this.GetType())));
            }

            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(DiagnosticUtility.GetString(SR.UnsupportedOnThisJsonValue, this.GetType())));
            }
        }

        /// <summary>
        /// This indexer is not supported for this base class and throws an exception.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns><see cref="System.Json.JsonValue"/>.</returns>
        /// <remarks>The exception thrown is the <see cref="System.InvalidOperationException"/>.
        /// This method is overloaded in the implementation of the <see cref="System.Json.JsonArray"/>
        /// class, which inherits from this class.</remarks>
        public virtual JsonValue this[int index]
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(DiagnosticUtility.GetString(SR.UnsupportedOnThisJsonValue, this.GetType())));
            }

            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(DiagnosticUtility.GetString(SR.UnsupportedOnThisJsonValue, this.GetType())));
            }
        }

        /// <summary>
        /// Deserializes text-based JSON into a JSON CLR type.
        /// </summary>
        /// <param name="json">The text-based JSON to be parsed into a JSON CLR type.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> object that represents the parsed
        /// text-based JSON as a CLR type.</returns>
        /// <exception cref="System.ArgumentException">The length of jsonString is zero.</exception>
        /// <exception cref="System.ArgumentNullException">jsonString is null.</exception>
        /// <remarks>The result will be an instance of either <see cref="System.Json.JsonArray"/>,
        /// <see cref="System.Json.JsonObject"/> or <see cref="System.Json.JsonPrimitive"/>,
        /// depending on the text-based JSON supplied to the method.</remarks>
        public static JsonValue Parse(string json)
        {
            return JXmlToJsonValueConverter.JXMLToJsonValue(json);
        }

        /// <summary>
        /// Deserializes text-based JSON from a text reader into a JSON CLR type.
        /// </summary>
        /// <param name="textReader">A <see cref="System.IO.TextReader"/> over text-based JSON content.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> object that represents the parsed
        /// text-based JSON as a CLR type.</returns>
        /// <exception cref="System.ArgumentNullException">textReader is null.</exception>
        /// <remarks>The result will be an instance of either <see cref="System.Json.JsonArray"/>,
        /// <see cref="System.Json.JsonObject"/> or <see cref="System.Json.JsonPrimitive"/>,
        /// depending on the text-based JSON supplied to the method.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Call to DiagnosticUtility validates the parameter.")]
        public static JsonValue Load(TextReader textReader)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(textReader, "textReader");
            return JsonValue.Parse(textReader.ReadToEnd());
        }

        /// <summary>
        /// Deserializes text-based JSON from a stream into a JSON CLR type.
        /// </summary>
        /// <param name="stream">A <see cref="System.IO.Stream"/> that contains text-based JSON content.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> object that represents the parsed
        /// text-based JSON as a CLR type.</returns>
        /// <exception cref="System.ArgumentNullException">stream is null.</exception>
        /// <remarks>The result will be an instance of either <see cref="System.Json.JsonArray"/>,
        /// <see cref="System.Json.JsonObject"/> or <see cref="System.Json.JsonPrimitive"/>,
        /// depending on the text-based JSON supplied to the method.</remarks>
        public static JsonValue Load(Stream stream)
        {
            return JXmlToJsonValueConverter.JXMLToJsonValue(stream);
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
            return JXmlToJsonValueConverter.JXMLToJsonValue(jsonReader);
        }

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
            if (value == null)
            {
                return null;
            }

            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(value.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                dcjs.WriteObject(ms, value);
                ms.Position = 0;
                return JsonValue.Load(ms);
            }
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.String"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.String"/> object.</param>
        /// <returns>The <see cref="System.String"/> initialized with the <see cref="System.Json.JsonValue"/> value specified or null if value is null.</returns>
        public static explicit operator string(JsonValue value)
        {
            if (value == null)
            {
                return null;
            }

            return CastValue<string>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Double"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Double"/> object.</param>
        /// <returns>The <see cref="System.Double"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator double(JsonValue value)
        {
            return CastValue<double>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Single"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Single"/> object.</param>
        /// <returns>The <see cref="System.Single"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator float(JsonValue value)
        {
            return CastValue<float>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Decimal"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Decimal"/> object.</param>
        /// <returns>The <see cref="System.Decimal"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator decimal(JsonValue value)
        {
            return CastValue<decimal>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Int64"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Int64"/> object.</param>
        /// <returns>The <see cref="System.Int64"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator long(JsonValue value)
        {
            return CastValue<long>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.UInt64"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.UInt64"/> object.</param>
        /// <returns>The <see cref="System.UInt64"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        [CLSCompliant(false)]
        public static explicit operator ulong(JsonValue value)
        {
            return CastValue<ulong>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Int32"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Int32"/> object.</param>
        /// <returns>The <see cref="System.Int32"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator int(JsonValue value)
        {
            return CastValue<int>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.UInt32"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.UInt32"/> object.</param>
        /// <returns>The <see cref="System.UInt32"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        [CLSCompliant(false)]
        public static explicit operator uint(JsonValue value)
        {
            return CastValue<uint>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Int16"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Int16"/> object.</param>
        /// <returns>The <see cref="System.Int16"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator short(JsonValue value)
        {
            return CastValue<short>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.UInt16"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.UInt16"/> object.</param>
        /// <returns>The <see cref="System.UInt16"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        [CLSCompliant(false)]
        public static explicit operator ushort(JsonValue value)
        {
            return CastValue<ushort>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.SByte"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.SByte"/> object.</param>
        /// <returns>The <see cref="System.SByte"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        [CLSCompliant(false)]
        public static explicit operator sbyte(JsonValue value)
        {
            return CastValue<sbyte>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Byte"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Byte"/> object.</param>
        /// <returns>The <see cref="System.Byte"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator byte(JsonValue value)
        {
            return CastValue<byte>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Uri"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Uri"/> object.</param>
        /// <returns>The <see cref="System.Uri"/> initialized with the <see cref="System.Json.JsonValue"/> value specified or null if value is null.</returns>
        public static explicit operator Uri(JsonValue value)
        {
            if (value == null)
            {
                return null;
            }

            return CastValue<Uri>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Guid"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Guid"/> object.</param>
        /// <returns>The <see cref="System.Guid"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator Guid(JsonValue value)
        {
            return CastValue<Guid>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.DateTime"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.DateTime"/> object.</param>
        /// <returns>The <see cref="System.DateTime"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator DateTime(JsonValue value)
        {
            return CastValue<DateTime>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Char"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Char"/> object.</param>
        /// <returns>The <see cref="System.Char"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator char(JsonValue value)
        {
            return CastValue<char>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.Boolean"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.Boolean"/> object.</param>
        /// <returns>The <see cref="System.Boolean"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator bool(JsonValue value)
        {
            return CastValue<bool>(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Json.JsonValue"/> to a <see cref="System.DateTimeOffset"/> object.
        /// </summary>
        /// <param name="value">The instance of <see cref="System.Json.JsonValue"/> used to initialize the <see cref="System.DateTimeOffset"/> object.</param>
        /// <returns>The <see cref="System.DateTimeOffset"/> initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        public static explicit operator DateTimeOffset(JsonValue value)
        {
            return CastValue<DateTimeOffset>(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Boolean"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Boolean"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Boolean"/> specified.</returns>
        public static implicit operator JsonValue(bool value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Byte"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Byte"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Byte"/> specified.</returns>
        public static implicit operator JsonValue(byte value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Decimal"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Decimal"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Decimal"/> specified.</returns>
        public static implicit operator JsonValue(decimal value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Double"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Double"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Double"/> specified.</returns>
        public static implicit operator JsonValue(double value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Int16"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Int16"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Int16"/> specified.</returns>
        public static implicit operator JsonValue(short value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Int32"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Int32"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Int32"/> specified.</returns>
        public static implicit operator JsonValue(int value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Int64"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Int64"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Int64"/> specified.</returns>
        public static implicit operator JsonValue(long value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Single"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Single"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Single"/> specified.</returns>
        public static implicit operator JsonValue(float value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.String"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.String"/> specified, or null if the value is null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This operator does not intend to represent a Uri overload.")]
        public static implicit operator JsonValue(string value)
        {
            return value == null ? null : new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Char"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Char"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Char"/> specified.</returns>
        public static implicit operator JsonValue(char value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.DateTime"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.DateTime"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.DateTime"/> specified.</returns>
        public static implicit operator JsonValue(DateTime value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Guid"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Guid"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Guid"/> specified.</returns>
        public static implicit operator JsonValue(Guid value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.Uri"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Uri"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.Uri"/> specified, or null if the value is null.</returns>
        public static implicit operator JsonValue(Uri value)
        {
            return value == null ? null : new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.SByte"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.SByte"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.SByte"/> specified.</returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(sbyte value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.UInt16"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.UInt16"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.UInt16"/> specified.</returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(ushort value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.UInt32"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.UInt32"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.UInt32"/> specified.</returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(uint value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.UInt64"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.UInt64"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.UInt64"/> specified.</returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(ulong value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Enables implicit casts from type <see cref="System.DateTimeOffset"/> to a <see cref="System.Json.JsonPrimitive"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.DateTimeOffset"/> instance used to initialize the <see cref="System.Json.JsonPrimitive"/>.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> initialized with the <see cref="System.DateTimeOffset"/> specified.</returns>
        public static implicit operator JsonValue(DateTimeOffset value)
        {
            return new JsonPrimitive(value);
        }

        /// <summary>
        /// Performs a cast operation from a <see cref="JsonValue"/> instance into the specified type parameter./>
        /// </summary>
        /// <typeparam name="T">The type to cast the instance to.</typeparam>
        /// <param name="value">The <see cref="System.Json.JsonValue"/> instance.</param>
        /// <returns>An object of type T initialized with the <see cref="System.Json.JsonValue"/> value specified.</returns>
        /// <remarks>This method is to support the framework and is not intended to be used externally, use explicit type cast instead.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T CastValue<T>(JsonValue value)
        {
            if (value == null || value.JsonType == JsonType.Default)
            {
                if (typeof(T).IsValueType)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(DiagnosticUtility.GetString(SR.InvalidCastNonNullable, typeof(T).FullName)));
                }
                else
                {
                    return default(T);
                }
            }

            try
            {
                return value.ReadAs<T>();
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is NotSupportedException || ex is InvalidCastException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(DiagnosticUtility.GetString(SR.CannotCastJsonValue, value.GetType().FullName, typeof(T).FullName), ex));
                }

                throw;
            }
        }

        /// <summary>
        /// Returns an enumerator which iterates through the values in this object.
        /// </summary>
        /// <returns>An enumerator which which iterates through the values in this object.</returns>
        /// <remarks>The enumerator returned by this class is empty; subclasses will override this method to return appropriate enumerators for themselves.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033",
            Justification = "Cannot make this class sealed, it need to have subclasses. But its subclasses are sealed themselves.")]
        public virtual IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            yield break;
        }

        /// <summary>
        /// Returns an enumerator which iterates through the values in this object.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"/> which iterates through the values in this object.</returns>
        /// <remarks>The enumerator returned by this class is empty; subclasses will override this method to return appropriate enumerators for themselves.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033",
            Justification = "Cannot make this class sealed, there's already a GetEnumerator method.")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, JsonValue>>)this).GetEnumerator();
        }

        /// <summary>
        /// Gets this instance as a <code>dynamic</code> object.
        /// </summary>
        /// <returns>This instance as <code>dynamic</code>.</returns>
        public dynamic AsDynamic()
        {
            return this;
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="valueOfT">An instance of T initialized with this instance, or the default
        /// value of T, if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonValue"/> instance can be read as type T; otherwise, false.</returns>
        public bool TryReadAs<T>(out T valueOfT)
        {
            object value;
            if (this.TryReadAs(typeof(T), out value))
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
        /// <returns>An instance of T initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.NotSupportedException">If this <see cref="System.Json.JsonValue"/> value cannot be
        /// converted into the type T.</exception>
        public T ReadAs<T>()
        {
            return (T)this.ReadAs(typeof(T));
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into the type T.
        /// </summary>
        /// <typeparam name="T">The type to which the conversion is being performed.</typeparam>
        /// <param name="fallback">The fallback value to be returned, if the conversion cannot be made.</param>
        /// <returns>An instance of T initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion, or the fallback value, if the conversion cannot be made.</returns>
        public T ReadAs<T>(T fallback)
        {
            return (T)this.ReadAs(typeof(T), fallback);
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance to an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <param name="fallback">The fallback value to be returned, if the conversion cannot be made.</param>
        /// <returns>An object instance initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion, or the fallback value, if the conversion cannot be made.</returns>
        public object ReadAs(Type type, object fallback)
        {
            object result;
            if (this.JsonType != JsonType.Default && this.TryReadAs(type, out result))
            {
                return result;
            }
            else
            {
                return fallback;
            }
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <returns>An object instance initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.NotSupportedException">If this <see cref="System.Json.JsonValue"/> value cannot be
        /// converted into the type T.</exception>
        public virtual object ReadAs(Type type)
        {
            object result;
            if (this.TryReadAs(type, out result))
            {
                return result;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonValue"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <param name="value">An object to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonValue"/> instance can be read as the specified type; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", 
            Justification = "This is the non-generic version of the method.")]
        public virtual bool TryReadAs(Type type, out object value)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    this.Save(ms);
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

        /// <summary>
        /// Serializes the <see cref="System.Json.JsonValue"/> CLR type into text-based JSON using a stream.
        /// </summary>
        /// <param name="stream">Stream to which to write text-based JSON.</param>
        public void Save(Stream stream)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnDefaultInstance(this);
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(stream, "stream");

            using (XmlDictionaryWriter jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false))
            {
                jsonWriter.WriteStartElement(JXmlToJsonValueConverter.RootElementName);
                this.SaveCore(jsonWriter);
                jsonWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Serializes the <see cref="System.Json.JsonValue"/> CLR type into text-based JSON using a text writer.
        /// </summary>
        /// <param name="textWriter">The <see cref="System.IO.TextWriter"/> used to write text-based JSON.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "Call to DiagnosticUtility validates the parameter.")]
        public virtual void Save(TextWriter textWriter)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnDefaultInstance(this);
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(textWriter, "textWriter");

            using (MemoryStream ms = new MemoryStream())
            {
                this.Save(ms);
                ms.Position = 0;
                textWriter.Write(new StreamReader(ms).ReadToEnd());
            }
        }

        /// <summary>
        /// Serializes this <see cref="System.Json.JsonValue"/> CLR type into a JSON/XML writer using the
        /// <a href="http://msdn.microsoft.com/en-us/library/bb924435.aspx">mapping between JSON and XML</a>.
        /// </summary>
        /// <param name="jsonWriter">The JSON/XML writer used to serialize this instance.</param>
        public void Save(XmlDictionaryWriter jsonWriter)
        {
            JXmlToJsonValueConverter.JsonValueToJXML(jsonWriter, this);
        }

        /// <summary>
        /// Saves (serializes) this JSON CLR type into text-based JSON.
        /// </summary>
        /// <returns>A <see cref="System.String"/>, which contains text-based JSON.</returns>
        public override string ToString()
        {
            if (this.JsonType == JsonType.Default)
            {
                return "Default";
            }

            using (MemoryStream ms = new MemoryStream())
            {
                this.Save(ms);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }

        /// <summary>
        /// Checks whether a key/value pair with a specified key exists in the JSON CLR object type.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>false in this class; subclasses may override this method to return other values.</returns>
        /// <remarks>This method is overloaded in the implementation of the <see cref="System.Json.JsonObject"/>
        /// class, which inherits from this class.</remarks>
        public virtual bool ContainsKey(string key)
        {
            return false;
        }

        /// <summary>
        /// Returns the value returned by the safe string indexer for this instance.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>If this is an instance of <see cref="System.Json.JsonObject"/>, it contains
        /// the given key and the value corresponding to the key is not null, then it will return that value.
        /// Otherwise it will return a <see cref="System.Json.JsonValue"/> instance with <see cref="System.Json.JsonValue.JsonType"/>
        /// equals to <see cref="System.Json.JsonType">Default</see>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual JsonValue GetValue(string key)
        {
            return this.ValueOrDefault(key);
        }

        /// <summary>
        /// Returns the value returned by the safe int indexer for this instance.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>If this is an instance of <see cref="System.Json.JsonArray"/>, the index is within the array
        /// bounds, and the value corresponding to the index is not null, then it will return that value.
        /// Otherwise it will return a <see cref="System.Json.JsonValue"/> instance with <see cref="System.Json.JsonValue.JsonType"/>
        /// equals to <see cref="System.Json.JsonType">Default</see>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual JsonValue GetValue(int index)
        {
            return this.ValueOrDefault(index);
        }

        /// <summary>
        /// Sets the value and returns it.
        /// </summary>
        /// <param name="key">The key of the element to set.</param>
        /// <param name="value">The value to be set.</param>
        /// <returns>The value, converted into a JsonValue, set in this collection.</returns>
        /// <exception cref="System.ArgumentException">If the value cannot be converted into a JsonValue.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual JsonValue SetValue(string key, object value)
        {
            this[key] = ResolveObject(value);
            return this[key];
        }

        /// <summary>
        /// Sets the value and returns it.
        /// </summary>
        /// <param name="index">The zero-based index of the element to set.</param>
        /// <param name="value">The value to be set.</param>
        /// <returns>The value, converted into a JsonValue, set in this collection.</returns>
        /// <exception cref="System.ArgumentException">If the value cannot be converted into a JsonValue.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual JsonValue SetValue(int index, object value)
        {
            this[index] = ResolveObject(value);
            return this[index];
        }

        /// <summary>
        /// Safe string indexer for the <see cref="System.Json.JsonValue"/> type. 
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>If this is an instance of <see cref="System.Json.JsonObject"/>, it contains
        /// the given key and the value corresponding to the key is not null, then it will return that value.
        /// Otherwise it will return a <see cref="System.Json.JsonValue"/> instance with <see cref="System.Json.JsonValue.JsonType"/>
        /// equals to <see cref="System.Json.JsonType">Default</see>.</returns>
        public virtual JsonValue ValueOrDefault(string key)
        {
            return JsonValue.DefaultInstance;
        }

        /// <summary>
        /// Safe indexer for the <see cref="System.Json.JsonValue"/> type. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>If this is an instance of <see cref="System.Json.JsonArray"/>, the index is within the array
        /// bounds, and the value corresponding to the index is not null, then it will return that value.
        /// Otherwise it will return a <see cref="System.Json.JsonValue"/> instance with <see cref="System.Json.JsonValue.JsonType"/>
        /// equals to <see cref="System.Json.JsonType">Default</see>.</returns>
        public virtual JsonValue ValueOrDefault(int index)
        {
            return JsonValue.DefaultInstance;
        }

        /// <summary>
        /// Safe deep indexer for the <see cref="JsonValue"/> type.
        /// </summary>
        /// <param name="indices">The indices to index this type. The indices can be
        /// of type <see cref="System.Int32"/> or <see cref="System.String"/>.</param>
        /// <returns>A <see cref="JsonValue"/> which is equivalent to calling<see cref="ValueOrDefault(int)"/> or
        /// <see cref="ValueOrDefault(string)"/> on the first index, then calling it again on the result
        /// for the second index and so on.</returns>
        /// <exception cref="System.ArgumentException">If any of the indices is not of type
        /// <see cref="System.Int32"/> or <see cref="System.String"/>.</exception>
        public JsonValue ValueOrDefault(params object[] indices)
        {
            if (indices == null)
            {
                return JsonValue.DefaultInstance;
            }

            if (indices.Length == 0)
            {
                return this;
            }

            JsonValue result = this;
            for (int i = 0; i < indices.Length; i++)
            {
                object index = indices[i];
                if (index is int)
                {
                    result = result.ValueOrDefault((int)index);
                }
                else if (index == null || index is string)
                {
                    result = result.ValueOrDefault((string)index);
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.InvalidIndexType, "indices"));
                }
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033",
            Justification = "Cannot make this class sealed, it need to have subclasses. But its subclasses are sealed themselves.")]
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new DynamicJsonValueMetaObject(parameter, this);
        }

        internal static JsonValue ResolveObject(object value)
        {
            JsonPrimitive primitive;

            if (value == null)
            {
                return null;
            }
            
            JsonValue jsonValue = value as JsonValue;

            if (jsonValue != null)
            {
                return jsonValue;
            }

            if (JsonPrimitive.TryCreate(value, out primitive))
            {
                return primitive;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.TypeNotSupported, "value")));
        }

        internal virtual void SaveCore(XmlDictionaryWriter jsonWriter)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(jsonWriter, "jsonWriter");

            Stack<JsonValue> objectStack = new Stack<JsonValue>();
            Stack<int> indexStack = new Stack<int>();
            int currentIndex = 0;
            JsonValue currentValue = this;

            this.OnSaveStarted();

            this.WriteAttributeString(jsonWriter);

            while (currentIndex < currentValue.Count || objectStack.Count > 0)
            {
                if (currentValue.Count > currentIndex)
                {
                    JsonValue nextValue = currentValue.WriteStartElementAndGetNext(jsonWriter, currentIndex);

                    if (JsonValue.IsJsonCollection(nextValue))
                    {
                        nextValue.OnSaveStarted();
                        nextValue.WriteAttributeString(jsonWriter);

                        objectStack.Push(currentValue);
                        currentValue = nextValue;
                        indexStack.Push(currentIndex);
                        currentIndex = 0;
                    }
                    else
                    {
                        if (nextValue == null)
                        {
                            jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.TypeAttributeName, JXmlToJsonValueConverter.NullAttributeValue);
                        }
                        else
                        {
                            nextValue.SaveCore(jsonWriter);
                        }

                        currentIndex++;
                        jsonWriter.WriteEndElement();
                    }
                }
                else
                {
                    if (objectStack.Count > 0)
                    {
                        currentValue.OnSaveEnded();
                        currentValue = objectStack.Pop();
                        currentIndex = indexStack.Pop() + 1;
                        jsonWriter.WriteEndElement();
                    }
                }
            }

            this.OnSaveEnded();
        }

        /// <summary>
        /// Callback method called during Save operations to let the instance write the start element
        /// and return the next element in the collection.
        /// </summary>
        /// <param name="jsonWriter">The JXML writer used to write JSON.</param>
        /// <param name="index">The index within this collection.</param>
        /// <returns>The next item in the collection, or null of there are no more items.</returns>
        protected virtual JsonValue WriteStartElementAndGetNext(XmlDictionaryWriter jsonWriter, int index)
        {
            return null;
        }

        /// <summary>
        /// Callback method called to let an instance write the proper JXML attribute when saving this
        /// instance.
        /// </summary>
        /// <param name="jsonWriter">The JXML writer used to write JSON.</param>
        protected virtual void WriteAttributeString(XmlDictionaryWriter jsonWriter)
        {
        }

        /// <summary>
        /// Callback method called when a Save operation is starting for this instance.
        /// </summary>
        protected virtual void OnSaveStarted()
        {
        }

        /// <summary>
        /// Callback method called when a Save operation is finished for this instance.
        /// </summary>
        protected virtual void OnSaveEnded()
        {
        }

        private static bool IsJsonCollection(JsonValue value)
        {
            return value is IList<JsonValue> || value is IDictionary<string, JsonValue>;
        }

        private static bool IsSupportedExplicitCastType(Type type)
        {
            return
                type == typeof(bool) || type == typeof(byte) || type == typeof(char) ||
                type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(decimal) ||
                type == typeof(double) || type == typeof(float) || type == typeof(Guid) ||
                type == typeof(int) || type == typeof(long) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(string) || type == typeof(uint) ||
                type == typeof(ulong) || type == typeof(Uri) || type == typeof(ushort);
        }

        private class DynamicJsonValueMetaObject : DynamicMetaObject
        {
            private static readonly MethodInfo GetValueByIndexMethodInfo = typeof(JsonValue).GetMethod("GetValue", new Type[] { typeof(int) });
            private static readonly MethodInfo GetValueByKeyMethodInfo = typeof(JsonValue).GetMethod("GetValue", new Type[] { typeof(string) });
            private static readonly MethodInfo SetValueByIndexMethodInfo = typeof(JsonValue).GetMethod("SetValue", new Type[] { typeof(int), typeof(object) });
            private static readonly MethodInfo SetValueByKeyMethodInfo = typeof(JsonValue).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });
            private static readonly MethodInfo CastValueMethodInfo = typeof(JsonValue).GetMethod("CastValue", new Type[] { typeof(JsonValue) });

            internal DynamicJsonValueMetaObject(System.Linq.Expressions.Expression parameter, JsonValue value)
                : base(parameter, BindingRestrictions.Empty, value)
            {
            }

            private BindingRestrictions DefaultRestrictions
            {
                get { return BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType); }
            }
            
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            public override DynamicMetaObject BindConvert(ConvertBinder binder)
            {
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(binder, "binder");

                Expression expression = this.Expression;

                // instance type to cast from is expected to be JsonValue (safe check).
                if (typeof(JsonValue).IsAssignableFrom(this.LimitType) && this.Value != null)
                {
                    bool implicitCastSupported = 
                        binder.Type.IsAssignableFrom(this.LimitType) ||
                        binder.Type == typeof(IEnumerable<KeyValuePair<string, JsonValue>>) ||
                        binder.Type == typeof(IDynamicMetaObjectProvider) ||
                        binder.Type == typeof(object);

                    if (!implicitCastSupported)
                    {
                        if (JsonValue.IsSupportedExplicitCastType(binder.Type))
                        {
                            Expression instance = Expression.Convert(this.Expression, this.LimitType);
                            expression = Expression.Call(CastValueMethodInfo.MakeGenericMethod(binder.Type), new Expression[] { instance });
                        }
                        else
                        {
                            string exceptionMessage = DiagnosticUtility.GetString(SR.CannotCastJsonValue, this.LimitType.FullName, binder.Type.FullName);
                            expression = Expression.Throw(Expression.Constant(new InvalidCastException(exceptionMessage)), typeof(object));
                        }
                    }
                }

                expression = Expression.Convert(expression, binder.Type);

                return new DynamicMetaObject(expression, this.DefaultRestrictions);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
            {
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(binder, "binder");
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(indexes, "indexes");
            
                if ((indexes[0].LimitType != typeof(int) && indexes[0].LimitType != typeof(string)) || indexes.Length != 1)
                {
                    return new DynamicMetaObject(Expression.Throw(Expression.Constant(new ArgumentException(SR.IndexTypeNotSupported)), typeof(object)), this.DefaultRestrictions);
                }

                MethodInfo methodInfo = indexes[0].LimitType == typeof(int) ? GetValueByIndexMethodInfo : GetValueByKeyMethodInfo;
                Expression[] args = new Expression[] { Expression.Convert(indexes[0].Expression, indexes[0].LimitType) };

                return this.GetMethodMetaObject(methodInfo, args);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
            {
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(binder, "binder");
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(indexes, "indexes");
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(value, "value");
                
                if ((indexes[0].LimitType != typeof(int) && indexes[0].LimitType != typeof(string)) || indexes.Length != 1 || !indexes[0].HasValue)
                {
                    return new DynamicMetaObject(Expression.Throw(Expression.Constant(new ArgumentException(SR.IndexTypeNotSupported)), typeof(object)), this.DefaultRestrictions);
                }

                MethodInfo methodInfo = indexes[0].LimitType == typeof(int) ? SetValueByIndexMethodInfo : SetValueByKeyMethodInfo;
                Expression[] args = new Expression[] { Expression.Convert(indexes[0].Expression, indexes[0].LimitType), Expression.Convert(value.Expression, typeof(object)) };

                return this.GetMethodMetaObject(methodInfo, args);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(binder, "binder");

                PropertyInfo propInfo = this.LimitType.GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public);

                if (propInfo != null)
                {
                    return base.BindGetMember(binder);
                }

                Expression[] args = new Expression[] { Expression.Constant(binder.Name) };

                return this.GetMethodMetaObject(GetValueByKeyMethodInfo, args);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
                Justification = "Call to DiagnosticUtility validates the parameter.")]
            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(binder, "binder");
                DiagnosticUtility.ExceptionUtility.ThrowOnNull(value, "value");
                
                Expression[] args = new Expression[] { Expression.Constant(binder.Name), Expression.Convert(value.Expression, typeof(object)) };

                return this.GetMethodMetaObject(SetValueByKeyMethodInfo, args);
            }

            private DynamicMetaObject GetMethodMetaObject(MethodInfo methodInfo, Expression[] args)
            {
                Expression instance = Expression.Convert(this.Expression, this.LimitType);
                Expression methodCall = Expression.Call(instance, methodInfo, args);
                BindingRestrictions restrictions = this.DefaultRestrictions;

                DynamicMetaObject metaObj = new DynamicMetaObject(methodCall, restrictions);

                return metaObj;
            }
        }
    }
}
