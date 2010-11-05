// <copyright file="JsonValueDynamicMetaObject.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// This class provides dynamic behavior support for the JsonValue types.
    /// </summary>
    internal class JsonValueDynamicMetaObject : DynamicMetaObject
    {
        private static readonly MethodInfo GetValueByIndexMethodInfo = typeof(JsonValue).GetMethod("GetValue", new Type[] { typeof(int) });
        private static readonly MethodInfo GetValueByKeyMethodInfo = typeof(JsonValue).GetMethod("GetValue", new Type[] { typeof(string) });
        private static readonly MethodInfo SetValueByIndexMethodInfo = typeof(JsonValue).GetMethod("SetValue", new Type[] { typeof(int), typeof(object) });
        private static readonly MethodInfo SetValueByKeyMethodInfo = typeof(JsonValue).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });
        private static readonly MethodInfo CastValueMethodInfo = typeof(JsonValue).GetMethod("CastValue", new Type[] { typeof(JsonValue) });
        private static readonly MethodInfo ReadAsMethodInfo = typeof(JsonValue).GetMethod("ReadAs", new Type[] { typeof(Type) });

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="parameter">The expression representing this <see cref="DynamicMetaObject"/> during the dynamic binding process.</param>
        /// <param name="value">The runtime value represented by the <see cref="DynamicMetaObject"/>.</param>
        internal JsonValueDynamicMetaObject(Expression parameter, JsonValue value)
            : base(parameter, BindingRestrictions.Empty, value)
        {
        }

        /// <summary>
        /// Represents the level of support for operations.
        /// </summary>
        private enum OperationSupport
        {
            /// <summary>
            /// Operation fully supported on operands.
            /// </summary>
            Supported,

            /// <summary>
            /// Operation not supported on operand.
            /// </summary>
            NotSupported,

            /// <summary>
            /// Operation not supported on a <see cref="JsonValue "/> instance of certain <see cref="JsonType"/> type.
            /// </summary>
            NotSupportedOnJsonType,

            /// <summary>
            /// Operation not supported on second operand type.
            /// </summary>
            NotSupportedOnOperand,

            /// <summary>
            ///  Operation not supported on second operand's value type.
            /// </summary>
            NotSupportedOnValueType
        }

        /// <summary>
        /// Gets the default binding restrictions for this type.
        /// </summary>
        private BindingRestrictions DefaultRestrictions
        {
            get { return BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType); }
        }

        /// <summary>
        /// Performs the binding of the dynamic unary operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="UnaryOperationBinder"/> that represents the details of the dynamic operation</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding</returns>
        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

            Expression operExpression = null;
            JsonValue jsonValue = this.Value as JsonValue;

            if (JsonValueDynamicMetaObject.IsPrimitive(jsonValue))
            {
                Type operationType = jsonValue.Read().GetType();
                Expression instance = Expression.Convert(this.Expression, this.LimitType);
                Expression thisExpression = Expression.Convert(Expression.Call(instance, ReadAsMethodInfo, new Expression[] { Expression.Constant(operationType) }), operationType);

                operExpression = JsonValueDynamicMetaObject.GetUnaryOperationExpression(binder.Operation, thisExpression);
            }
            
            if (operExpression == null)
            {
                operExpression = JsonValueDynamicMetaObject.GetErrorExpression(OperationSupport.NotSupportedOnJsonType, binder.Operation, jsonValue, null);
            }

            operExpression = Expression.Convert(operExpression, binder.ReturnType);

            return new DynamicMetaObject(operExpression, this.DefaultRestrictions);
        }

        /// <summary>
        /// Performs the binding of the dynamic binary operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="BinaryOperationBinder"/> that represents the details of the dynamic operation.</param>
        /// <param name="arg">An instance of the <see cref="DynamicMetaObject"/> representing the right hand side of the binary operation.</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

            if (arg == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("arg"));
            }

            Expression thisExpression = this.Expression;
            Expression otherExpression = arg.Expression;
            Expression operExpression = null;

            JsonValue otherValue = arg.Value as JsonValue;
            JsonValue thisValue = this.Value as JsonValue;

            OperationSupport supportValue = JsonValueDynamicMetaObject.IsBinaryOperationSupported(thisValue, arg.Value, binder.Operation);

            if (supportValue == OperationSupport.Supported)
            {
                object dummyValue;
                if (otherValue == null && !thisValue.TryReadAs(arg.LimitType, out dummyValue))
                {
                    supportValue = OperationSupport.NotSupportedOnValueType;
                }
            }

            if (supportValue == OperationSupport.Supported)
            {
                if (JsonValueDynamicMetaObject.IsPrimitive(thisValue))
                {
                    bool otherIsNonNullClrType = arg.Value != null && otherValue == null;

                    if (otherIsNonNullClrType || JsonValueDynamicMetaObject.IsPrimitive(otherValue))
                    {
                        //// 2nd operand is a non-null non-JsonValue or a JsonPrimitive value.

                        Type thisResultType = thisValue.Read().GetType();
                        Expression instance = Expression.Convert(this.Expression, this.LimitType);
                        thisExpression = Expression.Convert(Expression.Call(instance, ReadAsMethodInfo, new Expression[] { Expression.Constant(thisResultType) }), thisResultType);

                        if (JsonValueDynamicMetaObject.IsPrimitive(otherValue))
                        {
                            Type otherResultType = otherValue.Read().GetType();
                            Expression otherInstance = Expression.Convert(arg.Expression, arg.LimitType);
                            otherExpression = Expression.Convert(Expression.Call(otherInstance, ReadAsMethodInfo, new Expression[] { Expression.Constant(otherResultType) }), otherResultType);
                        }
                    }
                }
                else
                {
                    if (thisValue.JsonType == JsonType.Default && otherValue == null)
                    {
                        thisExpression = Expression.Constant(null);
                    }
                }

                operExpression = JsonValueDynamicMetaObject.GetBinaryOperationExpression(binder.Operation, thisExpression, otherExpression);
            }
            
            if (operExpression == null)
            {
                operExpression = JsonValueDynamicMetaObject.GetErrorExpression(supportValue, binder.Operation, thisValue, arg.Value);
            }

            operExpression = Expression.Convert(operExpression, typeof(object));

            return new DynamicMetaObject(operExpression, this.DefaultRestrictions);
        }

        /// <summary>
        /// Implements dynamic cast for JsonValue types.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="ConvertBinder"/> that represents the details of the dynamic operation.</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

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

        /// <summary>
        /// Implements setter for dynamic indexer by index (JsonArray)
        /// </summary>
        /// <param name="binder">An instance of the <see cref="GetIndexBinder"/> that represents the details of the dynamic operation.</param>
        /// <param name="indexes">An array of <see cref="DynamicMetaObject"/> instances - indexes for the get index operation.</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

            if (indexes == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("indexes"));
            }

            if ((indexes[0].LimitType != typeof(int) && indexes[0].LimitType != typeof(string)) || indexes.Length != 1)
            {
                return new DynamicMetaObject(Expression.Throw(Expression.Constant(new ArgumentException(SR.IndexTypeNotSupported)), typeof(object)), this.DefaultRestrictions);
            }

            MethodInfo methodInfo = indexes[0].LimitType == typeof(int) ? GetValueByIndexMethodInfo : GetValueByKeyMethodInfo;
            Expression[] args = new Expression[] { Expression.Convert(indexes[0].Expression, indexes[0].LimitType) };

            return this.GetMethodMetaObject(methodInfo, args);
        }

        /// <summary>
        /// Implements getter for dynamic indexer by index (JsonArray).
        /// </summary>
        /// <param name="binder">An instance of the <see cref="SetIndexBinder"/> that represents the details of the dynamic operation.</param>
        /// <param name="indexes">An array of <see cref="DynamicMetaObject"/> instances - indexes for the set index operation.</param>
        /// <param name="value">The <see cref="DynamicMetaObject"/> representing the value for the set index operation.</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

            if (indexes == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("indexes"));
            }

            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
            }

            if ((indexes[0].LimitType != typeof(int) && indexes[0].LimitType != typeof(string)) || indexes.Length != 1 || !indexes[0].HasValue)
            {
                return new DynamicMetaObject(Expression.Throw(Expression.Constant(new ArgumentException(SR.IndexTypeNotSupported)), typeof(object)), this.DefaultRestrictions);
            }

            MethodInfo methodInfo = indexes[0].LimitType == typeof(int) ? SetValueByIndexMethodInfo : SetValueByKeyMethodInfo;
            Expression[] args = new Expression[] { Expression.Convert(indexes[0].Expression, indexes[0].LimitType), Expression.Convert(value.Expression, typeof(object)) };

            return this.GetMethodMetaObject(methodInfo, args);
        }

        /// <summary>
        /// Implements getter for dynamic indexer by key (JsonObject).
        /// </summary>
        /// <param name="binder">An instance of the <see cref="GetMemberBinder"/> that represents the details of the dynamic operation.</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

            PropertyInfo propInfo = this.LimitType.GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public);

            if (propInfo != null)
            {
                return base.BindGetMember(binder);
            }

            Expression[] args = new Expression[] { Expression.Constant(binder.Name) };

            return this.GetMethodMetaObject(GetValueByKeyMethodInfo, args);
        }

        /// <summary>
        /// Implements setter for dynamic indexer by key (JsonObject).
        /// </summary>
        /// <param name="binder">An instance of the <see cref="SetMemberBinder"/> that represents the details of the dynamic operation.</param>
        /// <param name="value">The <see cref="DynamicMetaObject"/> representing the value for the set member operation.</param>
        /// <returns>The new <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            if (binder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binder"));
            }

            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
            }

            Expression[] args = new Expression[] { Expression.Constant(binder.Name), Expression.Convert(value.Expression, typeof(object)) };

            return this.GetMethodMetaObject(SetValueByKeyMethodInfo, args);
        }

        /// <summary>
        /// Checks whether the specified value is a JSON primitive.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> value to check.</param>
        /// <returns>true if the value represents a JSON primitive, false otherwise.</returns>
        private static bool IsPrimitive(JsonValue value)
        {
            return value != null && (value.JsonType == JsonType.String || value.JsonType == JsonType.Number || value.JsonType == JsonType.Boolean);
        }

        /// <summary>
        /// Determines whether the binary or unary primitive operation type is supported on the specified 
        /// JsonValue instance (state) and the other operand.
        /// </summary>
        /// <param name="value">the JsonValue instance</param>
        /// <param name="operand">the second operand instance.</param>
        /// <param name="operation">the operation type.</param>
        /// <returns>an <see cref="OperationSupport"/> value.</returns>
        private static OperationSupport IsBinaryOperationSupported(JsonValue value, object operand, ExpressionType operation)
        {
            bool isCompareOperation = false;

            //// Unsupported operations:
            switch (operation)
            {
                //// binary operations
                case ExpressionType.AddAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.RightShiftAssign:
                    return OperationSupport.NotSupported;

                //// compare operations:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    isCompareOperation = true;
                    break;
            }

            JsonValue otherValue = operand as JsonValue;

            //// JSON Primitive supports most other operands

            if (JsonValueDynamicMetaObject.IsPrimitive(value))
            {
                //// not supported on operand of type JsonValue if it isn't a primitive and not comparing JsonValue types.

                if (otherValue != null && !JsonValueDynamicMetaObject.IsPrimitive(otherValue) && !isCompareOperation)
                {
                    return OperationSupport.NotSupportedOnJsonType;
                }
            }
            else
            {
                //// When value is non-primitive, it must be a compare operation and (the other operand must null or a JsonValue)

                if (!isCompareOperation)
                {
                    return OperationSupport.NotSupportedOnJsonType;
                }

                if (operand != null && otherValue == null)
                {
                    return OperationSupport.NotSupportedOnOperand;
                }
            }
            
            return OperationSupport.Supported;
        }

        /// <summary>
        /// Returns an expression representing a unary operation based on the specified operation type.
        /// </summary>
        /// <param name="operation">The operation type.</param>
        /// <param name="thisExpression">The operand.</param>
        /// <returns>The expression representing the unary operation.</returns>
        private static Expression GetUnaryOperationExpression(ExpressionType operation, Expression thisExpression)
        {
            //// Unary operators: +, -, !, ~, false (&&), true (||)
            //// unsupported: ++, --

            Expression operExpression = null;

            switch (operation)
            {
                case ExpressionType.UnaryPlus:
                    operExpression = Expression.UnaryPlus(thisExpression);
                    break;
                case ExpressionType.Negate:
                    operExpression = Expression.Negate(thisExpression);
                    break;
                case ExpressionType.Not:
                    operExpression = Expression.Not(thisExpression);
                    break;
                case ExpressionType.OnesComplement:
                    operExpression = Expression.OnesComplement(thisExpression);
                    break;
                case ExpressionType.IsFalse:
                    operExpression = Expression.IsFalse(thisExpression);
                    break;
                case ExpressionType.IsTrue:
                    operExpression = Expression.IsTrue(thisExpression);
                    break;
            }

            return operExpression;
        }

        /// <summary>
        /// Returns an Expression representing a binary operation based on the specified operation type.
        /// </summary>
        /// <param name="operation">The operation type.</param>
        /// <param name="thisExpression">An expression representing the left operand.</param>
        /// <param name="otherExpression">An expression representing the right operand.</param>
        /// <returns>The expression representing the binary operation.</returns>
        private static Expression GetBinaryOperationExpression(ExpressionType operation, Expression thisExpression, Expression otherExpression)
        {
            //// Binary operators: +, -, *, /, %, &, |, ^, <<, >>, ==, !=, >, <, >=, <=
            //// The '&&' and '||' operators are conditional versions of the '&' and '|' operators.
            //// Unsupported: Compound assignment operators.

            Expression operExpression = null;

            switch (operation)
            {
                case ExpressionType.Equal:
                    operExpression = Expression.Equal(thisExpression, otherExpression);
                    break;
                case ExpressionType.NotEqual:
                    operExpression = Expression.NotEqual(thisExpression, otherExpression);
                    break;
                case ExpressionType.GreaterThan:
                    operExpression = Expression.GreaterThan(thisExpression, otherExpression);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operExpression = Expression.GreaterThanOrEqual(thisExpression, otherExpression);
                    break;
                case ExpressionType.LessThan:
                    operExpression = Expression.LessThan(thisExpression, otherExpression);
                    break;
                case ExpressionType.LessThanOrEqual:
                    operExpression = Expression.LessThanOrEqual(thisExpression, otherExpression);
                    break;
                case ExpressionType.LeftShift:
                    operExpression = Expression.LeftShift(thisExpression, otherExpression);
                    break;
                case ExpressionType.RightShift:
                    operExpression = Expression.RightShift(thisExpression, otherExpression);
                    break;
                case ExpressionType.And:
                    operExpression = Expression.And(thisExpression, otherExpression);
                    break;
                case ExpressionType.Or:
                    operExpression = Expression.Or(thisExpression, otherExpression);
                    break;
                case ExpressionType.ExclusiveOr:
                    operExpression = Expression.ExclusiveOr(thisExpression, otherExpression);
                    break;
                case ExpressionType.Add:
                    operExpression = Expression.Add(thisExpression, otherExpression);
                    break;
                case ExpressionType.Subtract:
                    operExpression = Expression.Subtract(thisExpression, otherExpression);
                    break;
                case ExpressionType.Multiply:
                    operExpression = Expression.Multiply(thisExpression, otherExpression);
                    break;
                case ExpressionType.Divide:
                    operExpression = Expression.Divide(thisExpression, otherExpression);
                    break;
                case ExpressionType.Modulo:
                    operExpression = Expression.Modulo(thisExpression, otherExpression);
                    break;
            }

            return operExpression;
        }

        /// <summary>
        /// Returns an expression representing a 'throw' instruction based on the specified <see cref="OperationSupport"/> value.
        /// </summary>
        /// <param name="supportValue">The <see cref="OperationSupport"/> value.</param>
        /// <param name="operation">The operation type.</param>
        /// <param name="thisValue">The operation left operand.</param>
        /// <param name="operand">The operation right operand.</param>
        /// <returns>A <see cref="Expression"/> representing a 'throw' instruction.</returns>
        private static Expression GetErrorExpression(OperationSupport supportValue, ExpressionType operation, JsonValue thisValue, object operand)
        {
            string exceptionMessage;

            switch (supportValue)
            {
                default:
                case OperationSupport.NotSupported:
                case OperationSupport.NotSupportedOnJsonType:
                case OperationSupport.NotSupportedOnValueType:
                    exceptionMessage = DiagnosticUtility.GetString(SR.OperatorNotDefinedForJsonType, operation, thisValue.JsonType);
                    break;

                case OperationSupport.NotSupportedOnOperand:
                    Debug.Assert(operand != null, "Operand is null!");
                    string operandTypeName = operand != null ? operand.GetType().FullName : "'Operand'";
                    exceptionMessage = DiagnosticUtility.GetString(SR.OperatorNotAllowedOnOperands, operation, thisValue.GetType().FullName, operandTypeName);
                    break;
            }

            return Expression.Throw(Expression.Constant(new InvalidOperationException(exceptionMessage)), typeof(object));
        }

        /// <summary>
        /// Gets a <see cref="DynamicMetaObject"/> for a method call.
        /// </summary>
        /// <param name="methodInfo">Info for the method to be performed.</param>
        /// <param name="args">expression array representing the method arguments</param>
        /// <returns>A meta object for the method call.</returns>
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
