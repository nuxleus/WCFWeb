﻿// <copyright file="JsonPrimitive.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Xml;
    using Fx = System.Diagnostics.Debug;

    /// <summary>
    /// Represents a JavaScript Object Notation (JSON) primitive type in the common language runtime (CLR).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "JsonPrimitive does not represent a collection.")]
    public sealed class JsonPrimitive : JsonValue
    {
        private const string DateTimeIsoFormat = "yyyy-MM-ddTHH:mm:ss.fffK";
        private static readonly string[] JSDateOrTimeLocalFormats = new string[] { "yyyy-MM-dd", "HH:mm:ss", "HH:mm", "yyyy-MM-ddTHH:mm:ss" };
        private static readonly string[] JSDateUtcFormats = { @"ddd, d MMM yyyy HH:mm:ss \U\T\C", @"ddd, d MMM yyyy HH:mm:ss \G\M\T" };
        private static readonly string[] JSDateWithTimezoneFormats = { "yyyy-MM-ddTHH:mm:ssK", DateTimeIsoFormat, "yyyy-MM-ddTHH:mm:sszzz", @"ddd, d MMM yyyy HH:mm:ss zzz" };
        private static readonly long UnixEpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly char[] FloatingPointChars = new char[] { '.', 'e', 'E' };
        private static readonly Dictionary<Type, Func<string, ConvertResult>> stringConverters = new Dictionary<Type, Func<string, ConvertResult>>
            {
                { typeof(bool), new Func<string, ConvertResult>(StringToBool) },
                { typeof(byte), new Func<string, ConvertResult>(StringToByte) },
                { typeof(char), new Func<string, ConvertResult>(StringToChar) },
                { typeof(sbyte), new Func<string, ConvertResult>(StringToSByte) },
                { typeof(short), new Func<string, ConvertResult>(StringToShort) },
                { typeof(int), new Func<string, ConvertResult>(StringToInt) },
                { typeof(long), new Func<string, ConvertResult>(StringToLong) },
                { typeof(ushort), new Func<string, ConvertResult>(StringToUShort) },
                { typeof(uint), new Func<string, ConvertResult>(StringToUInt) },
                { typeof(ulong), new Func<string, ConvertResult>(StringToULong) },
                { typeof(float), new Func<string, ConvertResult>(StringToFloat) },
                { typeof(double), new Func<string, ConvertResult>(StringToDouble) },
                { typeof(decimal), new Func<string, ConvertResult>(StringToDecimal) },
                { typeof(DateTime), new Func<string, ConvertResult>(StringToDateTime) },
                { typeof(DateTimeOffset), new Func<string, ConvertResult>(StringToDateTimeOffset) },
                { typeof(Guid), new Func<string, ConvertResult>(StringToGuid) },
                { typeof(Uri), new Func<string, ConvertResult>(StringToUri) },
            };

        private object value;
        private JsonType jsonType;

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Boolean"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Boolean"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Boolean"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Boolean</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Boolean"/>.</remarks>
        public JsonPrimitive(bool value)
        {
            this.jsonType = JsonType.Boolean;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Byte"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Byte"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Byte"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Byte"/>.</remarks>
        public JsonPrimitive(byte value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.SByte"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.SByte"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.SByte"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.SByte"/>.</remarks>
        [CLSCompliant(false)]
        public JsonPrimitive(sbyte value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Decimal"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Decimal"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Decimal"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Decimal"/>.</remarks>
        public JsonPrimitive(decimal value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Int16"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Int16"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Int16"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Int16"/>.</remarks>
        public JsonPrimitive(short value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.UInt16"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.UInt16"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.UInt16"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.UInt16"/>.</remarks>
        [CLSCompliant(false)]
        public JsonPrimitive(ushort value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Int32"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Int32"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Int32"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Int32"/>.</remarks>
        public JsonPrimitive(int value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.UInt32"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.UInt32"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.UInt32"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.UInt32"/>.</remarks>
        [CLSCompliant(false)]
        public JsonPrimitive(uint value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Int64"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Int64"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Int64"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Int64"/>.</remarks>
        public JsonPrimitive(long value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.UInt64"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.UInt64"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.UInt64"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.UInt64"/>.</remarks>
        [CLSCompliant(false)]
        public JsonPrimitive(ulong value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Single"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Single"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Single"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Single"/>.</remarks>
        public JsonPrimitive(float value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Double"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Double"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Double"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">Number</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Double"/>.</remarks>
        public JsonPrimitive(double value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.String"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.String"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">String</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.String"/>.</remarks>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads",
            Justification = "This operator does not intend to represent a Uri overload.")]
        public JsonPrimitive(string value)
        {
            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
            }
            
            this.jsonType = JsonType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Char"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Char"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Char"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">String</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Char"/>.</remarks>
        public JsonPrimitive(char value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.DateTime"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.DateTime"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.DateTime"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">String</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.DateTime"/>.</remarks>
        public JsonPrimitive(DateTime value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.DateTimeOffset"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.DateTimeOffset"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.DateTimeOffset"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">String</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.DateTimeOffset"/>.</remarks>
        public JsonPrimitive(DateTimeOffset value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Uri"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Uri"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Uri"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">String</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Uri"/>.</remarks>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public JsonPrimitive(Uri value)
        {
            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
            }

            this.jsonType = JsonType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="System.Json.JsonPrimitive"/> type with a <see cref="System.Guid"/> type.
        /// </summary>
        /// <param name="value">The <see cref="System.Guid"/> object that initializes the new instance.</param>
        /// <remarks>A <see cref="System.Json.JsonPrimitive"/> object stores a <see cref="System.Json.JsonType"/> and the value used to initialize it.
        /// When initialized with a <see cref="System.Guid"/> object, the <see cref="System.Json.JsonType"/> is a <see cref="System.Json.JsonType">String</see>, which can be
        /// recovered using the <see cref="System.Json.JsonPrimitive.JsonType"/> property. The value used to initialize the <see cref="System.Json.JsonPrimitive"/>
        /// object can be recovered by casting the <see cref="System.Json.JsonPrimitive"/> to <see cref="System.Guid"/>.</remarks>
        public JsonPrimitive(Guid value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        private JsonPrimitive(object value, JsonType type)
        {
            this.jsonType = type;
            this.value = value;
        }

        private enum ReadAsFailureKind
        {
            NoFailure,
            InvalidCast,
            InvalidDateFormat,
            InvalidFormat,
            InvalidUriFormat,
            Overflow,
        }

        /// <summary>
        /// Gets the JsonType that is associated with this <see cref="System.Json.JsonPrimitive"/> object.
        /// </summary>
        public override JsonType JsonType
        {
            get
            {
                return this.jsonType;
            }
        }

        /// <summary>
        /// Gets the value represented by this instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Value in this context clearly refers to the underlying CLR value")]
        public object Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Attemps to create a <see cref="JsonPrimitive"/> instance from the specified <see cref="object"/> value.
        /// </summary>
        /// <param name="value">The <see cref="object"/> value to create the <see cref="JsonPrimitive"/> instance.</param>
        /// <param name="result">The resulting <see cref="JsonPrimitive"/> instance on success, null otherwise.</param>
        /// <returns>true if the operation is successful, false otherwise.</returns>
        public static bool TryCreate(object value, out JsonPrimitive result)
        {
            bool allowedType = true;
            JsonType jsonType = default(JsonType);

            if (value != null)
            {
                Type type = value.GetType();
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        jsonType = JsonType.Boolean;
                        break;
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                        jsonType = JsonType.Number;
                        break;
                    case TypeCode.String:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                        jsonType = JsonType.String;
                        break;
                    default:
                        if (type == typeof(Uri) || type == typeof(Guid) || type == typeof(DateTimeOffset))
                        {
                            jsonType = JsonType.String;
                        }
                        else
                        {
                            allowedType = false;
                        }

                        break;
                }
            }
            else
            {
                allowedType = false;
            }

            if (allowedType)
            {
                result = new JsonPrimitive(value, jsonType);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonPrimitive"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <returns>An object instance initialized with the <see cref="System.Json.JsonValue"/> value
        /// specified if the conversion.</returns>
        /// <exception cref="System.UriFormatException">If T is <see cref="System.Uri"/> and this value does
        /// not represent a valid Uri.</exception>
        /// <exception cref="OverflowException">If T is a numeric type, and a narrowing conversion would result
        /// in a loss of data. For example, if this instance holds an <see cref="System.Int32"/> value of 10000,
        /// and T is <see cref="System.Byte"/>, this operation would throw an <see cref="System.OverflowException"/>
        /// because 10000 is outside the range of the <see cref="System.Byte"/> data type.</exception>
        /// <exception cref="System.FormatException">If the conversion from the string representation of this
        /// value into another fails because the string is not in the proper format.</exception>
        /// <exception cref="System.InvalidCastException">If this instance cannot be read as type T.</exception>
        public override object ReadAs(Type type)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("type");
            }

            object result;
            ReadAsFailureKind failure = this.TryReadAsInternal(type, out result);
            if (failure == ReadAsFailureKind.NoFailure)
            {
                return result;
            }
            else
            {
                string valueStr = this.value.ToString();
                string typeOfTName = type.Name;
                switch (failure)
                {
                    case ReadAsFailureKind.InvalidFormat:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SG.GetString(SR.CannotReadPrimitiveAsType, valueStr, typeOfTName)));
                    case ReadAsFailureKind.InvalidDateFormat:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new FormatException(
                                SG.GetString(
                                    SR.InvalidDateFormat,
                                    valueStr,
                                    typeOfTName,
                                    JSDateUtcFormats[0],
                                    JSDateUtcFormats[1],
                                    JSDateWithTimezoneFormats[0],
                                    JSDateWithTimezoneFormats[1],
                                    JSDateWithTimezoneFormats[2],
                                    JSDateWithTimezoneFormats[3],
                                    JSDateOrTimeLocalFormats[0],
                                    JSDateOrTimeLocalFormats[1],
                                    JSDateOrTimeLocalFormats[2],
                                    JSDateOrTimeLocalFormats[3])));
                    case ReadAsFailureKind.InvalidUriFormat:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriFormatException(SG.GetString(SR.InvalidUriFormat, valueStr, typeOfTName)));
                    case ReadAsFailureKind.Overflow:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new OverflowException(SG.GetString(SR.OverflowReadAs, valueStr, typeOfTName)));
                    case ReadAsFailureKind.InvalidCast:
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(SG.GetString(SR.CannotReadPrimitiveAsType, valueStr, typeOfTName)));
                }
            }
        }

        /// <summary>
        /// Attempts to convert this <see cref="System.Json.JsonPrimitive"/> instance into an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to which the conversion is being performed.</param>
        /// <param name="value">An object instance to be initialized with this instance or null if the conversion cannot be performed.</param>
        /// <returns>true if this <see cref="System.Json.JsonPrimitive"/> instance can be read as the specified type; otherwise, false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value",
            Justification = "field is used with 'this' and arg is out param which makes it harder to be misused.")]
        public override bool TryReadAs(Type type, out object value)
        {
            return this.TryReadAsInternal(type, out value) == ReadAsFailureKind.NoFailure;
        }

        /// <summary>
        /// Returns the value this object wraps (if any).
        /// </summary>
        /// <returns>The value wrapped by this instance or null if none.</returns>
        internal override object Read()
        {
            return this.value;
        }

        internal override void Save(XmlDictionaryWriter jsonWriter)
        {
            if (this.value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("jsonWriter");
            }
            
            switch (this.JsonType)
            {
                case JsonType.Boolean:
                    jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.TypeAttributeName, JXmlToJsonValueConverter.BooleanAttributeValue);
                    break;
                case JsonType.Number:
                    jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.TypeAttributeName, JXmlToJsonValueConverter.NumberAttributeValue);
                    break;
                default:
                    jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.TypeAttributeName, JXmlToJsonValueConverter.StringAttributeValue);
                    break;
            }

            this.WriteValue(jsonWriter);
        }

        private static ConvertResult StringToBool(string valueString)
        {
            ConvertResult result = new ConvertResult();
            bool tempBool;
            result.ReadAsFailureKind = bool.TryParse(valueString, out tempBool) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidFormat;
            result.Value = tempBool;
            return result;
        }

        private static ConvertResult StringToByte(string valueString)
        {
            ConvertResult result = new ConvertResult();
            byte tempByte;
            result.ReadAsFailureKind = byte.TryParse(valueString, out tempByte) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<byte>(valueString, out tempByte);
            }

            result.Value = tempByte;
            return result;
        }

        private static ConvertResult StringToChar(string valueString)
        {
            ConvertResult result = new ConvertResult();
            char tempChar;
            result.ReadAsFailureKind = char.TryParse(valueString, out tempChar) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidFormat;
            result.Value = tempChar;
            return result;
        }

        private static ConvertResult StringToDecimal(string valueString)
        {
            ConvertResult result = new ConvertResult();
            decimal tempDecimal;
            result.ReadAsFailureKind = decimal.TryParse(valueString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out tempDecimal) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<decimal>(valueString, out tempDecimal);
            }

            result.Value = tempDecimal;
            return result;
        }

        private static ConvertResult StringToDateTime(string valueString)
        {
            ConvertResult result = new ConvertResult();
            DateTime tempDateTime;
            result.ReadAsFailureKind = TryParseDateTime(valueString, out tempDateTime) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidDateFormat;
            result.Value = tempDateTime;
            return result;
        }

        private static ConvertResult StringToDateTimeOffset(string valueString)
        {
            ConvertResult result = new ConvertResult();
            DateTimeOffset tempDateTimeOffset;
            result.ReadAsFailureKind = TryParseDateTimeOffset(valueString, out tempDateTimeOffset) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidDateFormat;
            result.Value = tempDateTimeOffset;
            return result;
        }

        private static ConvertResult StringToDouble(string valueString)
        {
            ConvertResult result = new ConvertResult();
            double tempDouble;
            result.ReadAsFailureKind = double.TryParse(valueString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out tempDouble) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<double>(valueString, out tempDouble);
            }

            result.Value = tempDouble;
            return result;
        }

        private static ConvertResult StringToGuid(string valueString)
        {
            ConvertResult result = new ConvertResult();
            Guid tempGuid;
            result.ReadAsFailureKind = Guid.TryParse(valueString, out tempGuid) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidFormat;
            result.Value = tempGuid;
            return result;
        }

        private static ConvertResult StringToShort(string valueString)
        {
            ConvertResult result = new ConvertResult();
            short tempShort;
            result.ReadAsFailureKind = short.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempShort) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<short>(valueString, out tempShort);
            }

            result.Value = tempShort;
            return result;
        }

        private static ConvertResult StringToInt(string valueString)
        {
            ConvertResult result = new ConvertResult();
            int tempInt;
            result.ReadAsFailureKind = int.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempInt) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<int>(valueString, out tempInt);
            }

            result.Value = tempInt;
            return result;
        }

        private static ConvertResult StringToLong(string valueString)
        {
            ConvertResult result = new ConvertResult();
            long tempLong;
            result.ReadAsFailureKind = long.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempLong) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<long>(valueString, out tempLong);
            }

            result.Value = tempLong;
            return result;
        }

        private static ConvertResult StringToSByte(string valueString)
        {
            ConvertResult result = new ConvertResult();
            sbyte tempSByte;
            result.ReadAsFailureKind = sbyte.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempSByte) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<sbyte>(valueString, out tempSByte);
            }

            result.Value = tempSByte;
            return result;
        }

        private static ConvertResult StringToFloat(string valueString)
        {
            ConvertResult result = new ConvertResult();
            float tempFloat;
            result.ReadAsFailureKind = float.TryParse(valueString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out tempFloat) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<float>(valueString, out tempFloat);
            }

            result.Value = tempFloat;
            return result;
        }

        private static ConvertResult StringToUShort(string valueString)
        {
            ConvertResult result = new ConvertResult();
            ushort tempUShort;
            result.ReadAsFailureKind = ushort.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempUShort) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<ushort>(valueString, out tempUShort);
            }

            result.Value = tempUShort;
            return result;
        }

        private static ConvertResult StringToUInt(string valueString)
        {
            ConvertResult result = new ConvertResult();
            uint tempUInt;
            result.ReadAsFailureKind = uint.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempUInt) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<uint>(valueString, out tempUInt);
            }

            result.Value = tempUInt;
            return result;
        }

        private static ConvertResult StringToULong(string valueString)
        {
            ConvertResult result = new ConvertResult();
            ulong tempULong;
            result.ReadAsFailureKind = ulong.TryParse(valueString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out tempULong) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidCast;
            if (result.ReadAsFailureKind != ReadAsFailureKind.NoFailure)
            {
                result.ReadAsFailureKind = StringToNumberConverter<ulong>(valueString, out tempULong);
            }

            result.Value = tempULong;
            return result;
        }

        private static ConvertResult StringToUri(string valueString)
        {
            ConvertResult result = new ConvertResult();
            Uri tempUri;
            result.ReadAsFailureKind = Uri.TryCreate(valueString, UriKind.RelativeOrAbsolute, out tempUri) ? ReadAsFailureKind.NoFailure : ReadAsFailureKind.InvalidUriFormat;
            result.Value = tempUri;
            return result;
        }

        private static ReadAsFailureKind StringToNumberConverter<T>(string valueString, out T valueNumber)
        {
            string str = valueString.Trim();

            if (str.IndexOfAny(FloatingPointChars) < 0)
            {
                long longVal;
                if (long.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out longVal))
                {
                    return NumberToNumberConverter<T>(longVal, out valueNumber);
                }
            }

            decimal decValue;
            if (decimal.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out decValue) && decValue != 0)
            {
                return NumberToNumberConverter<T>(decValue, out valueNumber);
            }

            double dblValue;
            if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out dblValue))
            {
                return NumberToNumberConverter<T>(dblValue, out valueNumber);
            }

            valueNumber = default(T);
            return ReadAsFailureKind.InvalidFormat;
        }

        private static ReadAsFailureKind NumberToNumberConverter<T>(object valueObject, out T valueNumber)
        {
            object value;
            ReadAsFailureKind failureKind = NumberToNumberConverter(typeof(T), valueObject, out value);
            if (failureKind == ReadAsFailureKind.NoFailure)
            {
                valueNumber = (T)value;
            }
            else
            {
                valueNumber = default(T);
            }
            
            return failureKind;
        }

        private static ReadAsFailureKind NumberToNumberConverter(Type type, object valueObject, out object valueNumber)
        {
            try
            {
                valueNumber = System.Convert.ChangeType(valueObject, type, CultureInfo.InvariantCulture);
                return ReadAsFailureKind.NoFailure;
            }
            catch (OverflowException)
            {
                valueNumber = null;
                return ReadAsFailureKind.Overflow;
            }
        }

        // The returned DateTime object is adjusted to UTC timezone.
        private static bool TryParseDateTime(string valueString, out DateTime dateTime)
        {
            if (DateTime.TryParseExact(valueString, JSDateUtcFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
            {
                dateTime = dateTime.ToUniversalTime();
                return true;
            }

            if (DateTime.TryParseExact(valueString, JSDateWithTimezoneFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                return true;
            }

            if (DateTime.TryParseExact(valueString, JSDateOrTimeLocalFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
            {
                return true;
            }

            if (TryParseAspNetDateTimeFormat(valueString, out dateTime))
            {
                return true;
            }

            return false;
        }

        private static bool TryParseAspNetDateTimeFormat(string valueString, out DateTime dateTime)
        {
            //// The format for the value is given by the following regex:
            //// \/Date\((?<milliseconds>\-?\d+)(?<offset>[\+\-]?\d{4})\)\/
            //// where milliseconds is the number of milliseconds since 1970/01/01:00:00:00.000 UTC (the "unix baseline")
            //// and offset is an optional which indicates whether the value is local or UTC.

            const string DateTimePrefix = "/Date(";
            const int DateTimePrefixLength = 6;
            const string DateTimeSuffix = ")/";
            const int DateTimeSuffixLength = 2;

            if (valueString.StartsWith(DateTimePrefix, StringComparison.Ordinal) && valueString.EndsWith(DateTimeSuffix, StringComparison.Ordinal))
            {
                string ticksValue = valueString.Substring(DateTimePrefixLength, valueString.Length - DateTimePrefixLength - DateTimeSuffixLength);
                DateTimeKind dateTimeKind = DateTimeKind.Utc;
                int indexOfTimeZoneOffset = ticksValue.IndexOf('+', 1);

                if (indexOfTimeZoneOffset < 0)
                {
                    indexOfTimeZoneOffset = ticksValue.IndexOf('-', 1);
                }

                if (indexOfTimeZoneOffset != -1)
                {
                    dateTimeKind = DateTimeKind.Local;
                    ticksValue = ticksValue.Substring(0, indexOfTimeZoneOffset);
                }

                long millisecondsSinceUnixEpoch;
                if (long.TryParse(ticksValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out millisecondsSinceUnixEpoch))
                {
                    long ticks = (millisecondsSinceUnixEpoch * 10000) + UnixEpochTicks;
                    if (ticks < DateTime.MaxValue.Ticks)
                    {
                        dateTime = new DateTime(ticks);
                        if (dateTimeKind == DateTimeKind.Local)
                        {
                            dateTime = dateTime.ToLocalTime();
                        }

                        return true;
                    }
                }
            }

            dateTime = new DateTime();
            return false;
        }

        private static bool TryParseDateTimeOffset(string valueString, out DateTimeOffset dateTime)
        {
            if (DateTimeOffset.TryParseExact(valueString, JSDateUtcFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
            {
                dateTime = dateTime.ToUniversalTime();
                return true;
            }

            if (DateTimeOffset.TryParseExact(valueString, JSDateWithTimezoneFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                return true;
            }

            if (DateTimeOffset.TryParseExact(valueString, JSDateOrTimeLocalFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
            {
                return true;
            }

            dateTime = new DateTimeOffset();
            return false;
        }

        private static string UnescapeJsonString(string val)
        {
            if (val == null)
            {
                return null;
            }

            StringBuilder sb = null;
            int startIndex = 0, count = 0;
            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] == '\\')
                {
                    i++;
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }

                    sb.Append(val, startIndex, count);
                    Fx.Assert(i < val.Length, "Found that a '\' was the last character in a string, which is invalid JSON. Verify the calling method uses a valid JSON string as the input parameter of this method.");
                    switch (val[i])
                    {
                        case '"':
                        case '\'':
                        case '/':
                        case '\\':
                            sb.Append(val[i]);
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            Fx.Assert((i + 3) < val.Length, String.Format(CultureInfo.CurrentCulture, "Unexpected char {0} at position {1}. The unicode escape sequence should be followed by 4 digits.", val[i], i));
                            sb.Append(ParseChar(val.Substring(i + 1, 4), NumberStyles.HexNumber));
                            i += 4;
                            break;
                    }

                    startIndex = i + 1;
                    count = 0;
                }
                else
                {
                    count++;
                }
            }

            if (sb == null)
            {
                return val;
            }

            if (count > 0)
            {
                sb.Append(val, startIndex, count);
            }

            return sb.ToString();
        }

        private static char ParseChar(string value, NumberStyles style)
        {
            try
            {
                int intValue = int.Parse(value, style, NumberFormatInfo.InvariantInfo);
                return System.Convert.ToChar(intValue);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception.Message, exception));
            }
            catch (FormatException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception.Message, exception));
            }
            catch (OverflowException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception.Message, exception));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value",
            Justification = "field is used with 'this' and arg is out param which makes it harder to be misused.")]
        private ReadAsFailureKind TryReadAsInternal(Type type, out object value)
        {
            if (base.TryReadAs(type, out value))
            {
                return ReadAsFailureKind.NoFailure;
            }

            if (type == this.value.GetType())
            {
                value = this.value;
                return ReadAsFailureKind.NoFailure;
            }

            if (this.JsonType == JsonType.Number)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return NumberToNumberConverter(type, this.value, out value);
                    case TypeCode.String:
                        value = this.ToString();
                        return ReadAsFailureKind.NoFailure;
                }
            }

            if (this.JsonType == JsonType.Boolean)
            {
                if (type == typeof(string))
                {
                    value = this.ToString();
                    return ReadAsFailureKind.NoFailure;
                }
            }

            if (this.JsonType == JsonType.String)
            {
                string str = UnescapeJsonString(this.ToString());
                Fx.Assert(str.Length >= 2 && str.StartsWith("\"", StringComparison.Ordinal) && str.EndsWith("\"", StringComparison.Ordinal), "The unescaped string must begin and end with quotes.");
                str = str.Substring(1, str.Length - 2);

                if (stringConverters.ContainsKey(type))
                {
                    ConvertResult result = stringConverters[type].Invoke(str);
                    value = result.Value;
                    return result.ReadAsFailureKind;
                }

                if (type == typeof(string))
                {
                    value = str;
                    return ReadAsFailureKind.NoFailure;
                }
            }

            value = null;
            return ReadAsFailureKind.InvalidCast;
        }

        private void WriteValue(XmlDictionaryWriter jsonWriter)
        {
            Type valueType = this.value.GetType();
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    jsonWriter.WriteValue((bool)this.value);
                    break;
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                    jsonWriter.WriteValue(String.Format(CultureInfo.InvariantCulture, "{0}", this.value));
                    break;
                case TypeCode.Single:
                case TypeCode.Double:
                    jsonWriter.WriteValue(String.Format(CultureInfo.InvariantCulture, "{0:R}", this.value));
                    break;
                case TypeCode.Char:
                    jsonWriter.WriteValue(new string((char)this.value, 1));
                    break;
                case TypeCode.String:
                    jsonWriter.WriteValue((string)this.value);
                    break;
                case TypeCode.DateTime:
                    jsonWriter.WriteValue(((DateTime)this.value).ToString(DateTimeIsoFormat, CultureInfo.InvariantCulture));
                    break;
                default:
                    if (valueType == typeof(Uri))
                    {
                        Uri uri = (Uri)this.value;
                        jsonWriter.WriteValue(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped));
                    }
                    else if (valueType == typeof(DateTimeOffset))
                    {
                        jsonWriter.WriteValue(((DateTimeOffset)this.value).ToString(DateTimeIsoFormat, CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        jsonWriter.WriteValue(this.value);
                    }

                    break;
            }
        }

        private class ConvertResult
        {
            public ReadAsFailureKind ReadAsFailureKind { get; set; }

            public object Value { get; set; }
        }
    }
}
