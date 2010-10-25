// <copyright file="JXmlToJsonValueConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;

    internal static class JXmlToJsonValueConverter
    {
        internal const string RootElementName = "root";
        internal const string ItemElementName = "item";
        internal const string TypeAttributeName = "type";
        internal const string ArrayAttributeValue = "array";
        internal const string BooleanAttributeValue = "boolean";
        internal const string NullAttributeValue = "null";
        internal const string NumberAttributeValue = "number";
        internal const string ObjectAttributeValue = "object";
        internal const string StringAttributeValue = "string";
        private const string TypeHintAttributeName = "__type";

        private static readonly char[] FloatingPointChars = new char[] { '.', 'e', 'E' };

        public static void JsonValueToJXML(XmlDictionaryWriter jsonWriter, JsonValue jsonValue)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(jsonWriter, "jsonWriter");
            jsonWriter.WriteStartElement(RootElementName);
            if (jsonValue == null)
            {
                jsonWriter.WriteAttributeString(TypeAttributeName, NullAttributeValue);
            }
            else
            {
                jsonValue.SaveCore(jsonWriter);
            }

            jsonWriter.WriteEndElement();
        }

        public static JsonValue JXMLToJsonValue(Stream jsonStream)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(jsonStream, "jsonStream");

            using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonStream, XmlDictionaryReaderQuotas.Max))
            {
                return JXMLToJsonValue(jsonReader);
            }
        }

        public static JsonValue JXMLToJsonValue(string jsonString)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(jsonString, "jsonString");

            if (jsonString.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentException(DiagnosticUtility.GetString(SR.JsonStringCannotBeEmpty), "jsonString"));
            }

            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max))
            {
                return JXMLToJsonValue(jsonReader);
            }
        }

        public static JsonValue JXMLToJsonValue(XmlDictionaryReader jsonReader)
        {
            DiagnosticUtility.ExceptionUtility.ThrowOnNull(jsonReader, "jsonReader");

            Stack<JsonValue> jsonStack = new Stack<JsonValue>();
            Stack<string> keyStack = new Stack<string>();
            JsonValue result = null;
            string type = null;
            bool isEmptyElement = false;

            MoveToRootNode(jsonReader);

            do
            {
                bool isProcessingCollection = false;

                if (type == null)
                {
                    type = jsonReader.GetAttribute(TypeAttributeName);
                    if (type == null)
                    {
                        type = StringAttributeValue; // the default
                    }

                    if (jsonStack.Count > 0 && jsonStack.Peek() is JsonArray)
                    {
                        // For arrays, the element name has to be "item"
                        if (jsonReader.Name != ItemElementName)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SR.IncorrectJsonFormat));
                        }
                    }
                }
                else
                {
                    if (jsonStack.Count == 0)
                    {
                        // "Element of type [{0}] has already been processed but cannot add it to a collection, the stack is empty!!"
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SR.IncorrectJsonFormat));
                    }

                    type = jsonStack.Peek() is JsonArray ? ArrayAttributeValue : ObjectAttributeValue;
                    isProcessingCollection = true;
                }

                switch (type)
                {
                    case NullAttributeValue:
                    case BooleanAttributeValue:
                    case StringAttributeValue:
                    case NumberAttributeValue:
                        result = ReadPrimitive(type, jsonReader);
                        break;
                    case ArrayAttributeValue:
                        JsonArray jsonArray;
                        isEmptyElement = false;

                        if (!isProcessingCollection)
                        {
                            isEmptyElement = jsonReader.IsEmptyElement;
                            jsonReader.ReadStartElement();
                            SkipWhitespace(jsonReader);
                            jsonArray = new JsonArray();
                        }
                        else
                        {
                            jsonArray = jsonStack.Pop() as JsonArray;
                            jsonArray.Add(result);
                        }

                        if (!isEmptyElement && jsonReader.NodeType != XmlNodeType.EndElement)
                        {
                            jsonStack.Push(jsonArray);
                            type = null;
                            continue;
                        }

                        result = jsonArray;
                        if (!isEmptyElement)
                        {
                            jsonReader.Read();
                            SkipWhitespace(jsonReader);
                        }

                        break;
                    case ObjectAttributeValue:
                        JsonObject jsonObject;
                        isEmptyElement = false;

                        if (!isProcessingCollection)
                        {
                            jsonObject = CreateObjectWithTypeHint(jsonReader, ref isEmptyElement);
                        }
                        else
                        {
                            jsonObject = jsonStack.Pop() as JsonObject;
                            jsonObject.Add(keyStack.Pop(), result);
                        }

                        if (!isEmptyElement && jsonReader.NodeType != XmlNodeType.EndElement)
                        {
                            string name = GetMemberName(jsonReader);
                            keyStack.Push(name);
                            jsonStack.Push(jsonObject);
                            type = null;
                            continue;
                        }

                        result = jsonObject;
                        if (!isEmptyElement)
                        {
                            jsonReader.Read();
                            SkipWhitespace(jsonReader);
                        }

                        break;
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SR.IncorrectJsonFormat));
                }
            }
            while (jsonStack.Count > 0);

            return result;
        }

        private static string GetMemberName(XmlDictionaryReader jsonReader)
        {
            string name;
            if (jsonReader.NamespaceURI == ItemElementName && jsonReader.LocalName == ItemElementName)
            {
                // JXML special case for names which aren't valid XML names
                name = jsonReader.GetAttribute(ItemElementName);

                if (name == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SR.IncorrectJsonFormat));
                }
            }
            else
            {
                name = jsonReader.Name;
            }

            return name;
        }

        private static JsonObject CreateObjectWithTypeHint(XmlDictionaryReader jsonReader, ref bool isEmptyElement)
        {
            JsonObject jsonObject;
            string typeHintAttribute = jsonReader.GetAttribute(TypeHintAttributeName);
            isEmptyElement = jsonReader.IsEmptyElement;
            jsonReader.ReadStartElement();
            SkipWhitespace(jsonReader);
            jsonObject = new JsonObject();
            if (typeHintAttribute != null)
            {
                jsonObject.Add(TypeHintAttributeName, typeHintAttribute);
            }

            return jsonObject;
        }

        private static void MoveToRootNode(XmlDictionaryReader jsonReader)
        {
            while (!jsonReader.EOF && (jsonReader.NodeType == XmlNodeType.None || jsonReader.NodeType == XmlNodeType.XmlDeclaration))
            {
                // read into <root> node
                jsonReader.Read();
                SkipWhitespace(jsonReader);
            }

            if (jsonReader.NodeType != XmlNodeType.Element || !string.IsNullOrEmpty(jsonReader.NamespaceURI) || jsonReader.Name != RootElementName)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SR.IncorrectJsonFormat));
            }
        }

        private static JsonValue ReadPrimitive(string type, XmlDictionaryReader jsonReader)
        {
            JsonValue result = null;
            switch (type)
            {
                case NullAttributeValue:
                    jsonReader.Skip();
                    result = null;
                    break;
                case BooleanAttributeValue:
                    result = jsonReader.ReadElementContentAsBoolean();
                    break;
                case StringAttributeValue:
                    result = jsonReader.ReadElementContentAsString();
                    break;
                case NumberAttributeValue:
                    string temp = jsonReader.ReadElementContentAsString();
                    result = ConvertStringToJsonNumber(temp);
                    break;
            }

            SkipWhitespace(jsonReader);
            return result;
        }

        private static void SkipWhitespace(XmlDictionaryReader reader)
        {
            while (!reader.EOF && (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace))
            {
                reader.Read();
            }
        }

        private static JsonValue ConvertStringToJsonNumber(string value)
        {
            if (value.IndexOfAny(FloatingPointChars) < 0)
            {
                int intVal;
                if (int.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out intVal))
                {
                    return intVal;
                }

                long longVal;
                if (long.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out longVal))
                {
                    return longVal;
                }
            }

            decimal decValue;
            if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decValue) && decValue != 0)
            {
                return decValue;
            }

            double dblValue;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out dblValue))
            {
                return dblValue;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(DiagnosticUtility.GetString(SR.InvalidJsonPrimitive, value)));
        }
    }
}
