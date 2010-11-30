namespace System.Json.Test
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Json;
    using System.Reflection;
    using System.Runtime.Serialization.Json;
    using Microsoft.ServiceModel.Web.Test.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the dynamic support for <see cref="JsonValue"/>.
    /// </summary>
    [TestClass]
    public class JsonValueDynamicTests
    {
        string teamNameValue = "WCF RIA Base";
        string[] teamMembersValues = { "Carlos", "Chris", "Joe", "Miguel", "Yavor" };

        /// <summary>
        /// Tests for the dynamic getters in <see cref="JsonObject"/> instances.
        /// </summary>
        [TestMethod]
        public void JsonObjectDynamicGetters()
        {
            dynamic team = new JsonObject();
            team["TeamSize"] = this.teamMembersValues.Length;
            team["TeamName"] = this.teamNameValue;
            team["TeamMascots"] = null;
            team["TeamMembers"] = new JsonArray 
            { 
                this.teamMembersValues[0], this.teamMembersValues[1], this.teamMembersValues[2],
                this.teamMembersValues[3], this.teamMembersValues[4] 
            };

            Assert.AreEqual(this.teamMembersValues.Length, (int)team.TeamSize);
            Assert.AreEqual(this.teamNameValue, (string)team.TeamName);
            Assert.IsNotNull(team.TeamMascots);
            Assert.IsTrue(team.TeamMascots is JsonValue); // default

            for (int i = 0; i < this.teamMembersValues.Length; i++)
            {
                Assert.AreEqual(this.teamMembersValues[i], (string)team.TeamMembers[i]);
            }

            for (int i = 0; i < this.teamMembersValues.Length; i++)
            {
                Assert.AreEqual(this.teamMembersValues[i], (string)team.TeamMembers[i]);
            }

            // Negative tests for getters
            JsonValueTests.ExpectException<InvalidCastException>(delegate { int fail = (int)team.NonExistentProp; });
        }

        /// <summary>
        /// Tests for the dynamic setters in <see cref="JsonObject"/> instances.
        /// </summary>
        [TestMethod]
        public void JsonObjectDynamicSetters()
        {
            dynamic team = new JsonObject();
            team.TeamSize = this.teamMembersValues.Length;
            team.TeamName = this.teamNameValue;
            team.TeamMascots = null;
            team.TeamMembers = new JsonArray 
            { 
                this.teamMembersValues[0], this.teamMembersValues[1], this.teamMembersValues[2],
                this.teamMembersValues[3], this.teamMembersValues[4] 
            };

            Assert.AreEqual(this.teamMembersValues.Length, (int)team["TeamSize"]);
            Assert.AreEqual(this.teamNameValue, (string)team["TeamName"]);
            Assert.IsNotNull(team["TeamMascots"]);
            Assert.IsTrue(team["TeamMascots"] is JsonValue);

            for (int i = 0; i < this.teamMembersValues.Length; i++)
            {
                Assert.AreEqual(this.teamMembersValues[i], (string)team["TeamMembers"][i]);
            }

            // Could not come up with negative setter
        }

        /// <summary>
        /// Tests for the dynamic indexers in <see cref="JsonArray"/> instances.
        /// </summary>
        [TestMethod]
        public void JsonArrayDynamicSanity()
        {
            // Sanity test for JsonArray to ensure [] still works even if dynamic
            dynamic people = new JsonArray();
            foreach (string member in this.teamMembersValues)
            {
                people.Add(member);
            }

            Assert.AreEqual(this.teamMembersValues[0], (string)people[0]);
            Assert.AreEqual(this.teamMembersValues[1], (string)people[1]);
            Assert.AreEqual(this.teamMembersValues[2], (string)people[2]);
            Assert.AreEqual(this.teamMembersValues[3], (string)people[3]);
            Assert.AreEqual(this.teamMembersValues[4], (string)people[4]);

            // Note: this test and the above execute the dynamic binder differently.
            for (int i = 0; i < people.Count; i++)
            {
                Assert.AreEqual(this.teamMembersValues[i], (string)people[i]);
            }

            people.Add(this.teamMembersValues.Length);
            people.Add(this.teamNameValue);

            Assert.AreEqual(this.teamMembersValues.Length, (int)people[5]);
            Assert.AreEqual(this.teamNameValue, (string)people[6]);
        }

        /// <summary>
        /// Tests for calling methods in dynamic references to <see cref="JsonValue"/> instances.
        /// </summary>
        [TestMethod]
        public void DynamicMethodCalling()
        {
            JsonObject jo = new JsonObject();
            dynamic dyn = jo;
            dyn.Foo = "bar";
            Assert.AreEqual(1, jo.Count);
            Assert.AreEqual(1, dyn.Count);
            dyn.Remove("Foo");
            Assert.AreEqual(0, jo.Count);
        }

        /// <summary>
        /// Tests for using boolean operators in dynamic references to <see cref="JsonValue"/> instances.
        /// </summary>
        [TestMethod]
        public void DynamicBooleanOperators()
        {
            JsonValue jv;
            dynamic dyn;
            foreach (bool value in new bool[] { true, false })
            {
                jv = value;
                dyn = jv;
                Log.Info("IsTrue, {0}", jv);
                if (dyn)
                {
                    Assert.IsTrue(value, "Boolean evaluation should not enter 'if' clause.");
                }
                else
                {
                    Assert.IsFalse(value, "Boolean evaluation should not enter 'else' clause.");
                }
            }

            foreach (bool value in new bool[] { true, false })
            {
                jv = new JsonPrimitive(value.ToString().ToLowerInvariant());
                dyn = jv;
                Log.Info("IsTrue, {0}", jv);
                if (dyn)
                {
                    Assert.IsTrue(value, "Boolean evaluation should not enter 'if' clause.");
                }
                else
                {
                    Assert.IsFalse(value, "Boolean evaluation should not enter 'else' clause.");
                }
            }

            foreach (bool first in new bool[] { false, true })
            {
                dynamic dyn1 = new JsonPrimitive(first);
                Log.Info("Negation, {0}", first);
                Assert.AreEqual(!first, !dyn1);
                foreach (bool second in new bool[] { false, true })
                {
                    dynamic dyn2 = new JsonPrimitive(second);
                    Log.Info("Boolean AND, {0} && {1}", first, second);
                    Assert.AreEqual(first && second, (bool)(dyn1 && dyn2), string.Format("Boolean AND: {0} && {1}", first, second));
                    Log.Info("Boolean OR, {0} && {1}", first, second);
                    Assert.AreEqual(first || second, (bool)(dyn1 || dyn2), string.Format("Boolean OR: {0} && {1}", first, second));
                }
            }
        }

        /// <summary>
        /// Tests for using relational operators in dynamic references to <see cref="JsonValue"/> instances.
        /// </summary>
        [TestMethod]
        public void DynamicRelationalOperators()
        {
            JsonValue jv = new JsonObject { { "one", 1 }, { "one_point_two", 1.2 } };
            dynamic dyn = jv;

            Log.Info("Equality");
            Assert.IsTrue(dyn.one == 1);
            Assert.IsTrue(dyn.one_point_two == 1.2);
            Assert.IsTrue(dyn.one != 1.2);
            Assert.IsTrue(dyn.one_point_two != 1);
            Assert.IsTrue(dyn.one != 2);
            Assert.IsTrue(dyn.one_point_two != 1.3);

            Log.Info("Less than");
            Assert.IsTrue(dyn.one < 2);
            Assert.IsFalse(dyn.one < 1);
            Assert.IsFalse(dyn.one < 0);
            Assert.IsTrue(dyn.one_point_two < 1.3);
            Assert.IsFalse(dyn.one_point_two < 1.2);
            Assert.IsFalse(dyn.one_point_two < 1.1);

            Log.Info("Greater than");
            Assert.IsFalse(dyn.one > 2);
            Assert.IsFalse(dyn.one > 1);
            Assert.IsTrue(dyn.one > 0);
            Assert.IsFalse(dyn.one_point_two > 1.3);
            Assert.IsFalse(dyn.one_point_two > 1.2);
            Assert.IsTrue(dyn.one_point_two > 1.1);

            Log.Info("Less than or equals");
            Assert.IsTrue(dyn.one <= 2);
            Assert.IsTrue(dyn.one <= 1);
            Assert.IsFalse(dyn.one <= 0);
            Assert.IsTrue(dyn.one_point_two <= 1.3);
            Assert.IsTrue(dyn.one_point_two <= 1.2);
            Assert.IsFalse(dyn.one_point_two <= 1.1);

            Log.Info("Greater than or equals");
            Assert.IsFalse(dyn.one >= 2);
            Assert.IsTrue(dyn.one >= 1);
            Assert.IsTrue(dyn.one >= 0);
            Assert.IsFalse(dyn.one_point_two >= 1.3);
            Assert.IsTrue(dyn.one_point_two >= 1.2);
            Assert.IsTrue(dyn.one_point_two >= 1.1);
        }

        /// <summary>
        /// Tests for using arithmetic operators in dynamic references to <see cref="JsonValue"/> instances.
        /// </summary>
        [TestMethod]
        public void ArithmeticOperators()
        {
            int seed = MethodBase.GetCurrentMethod().Name.GetHashCode();
            Log.Info("Seed: {0}", seed);
            Random rndGen = new Random(seed);
            int i1 = rndGen.Next(-10000, 10000);
            int i2 = rndGen.Next(-10000, 10000);
            JsonValue jv1 = i1;
            JsonValue jv2 = i2;
            Log.Info("jv1 = {0}, jv2 = {1}", jv1, jv2);
            dynamic dyn1 = jv1;
            dynamic dyn2 = jv2;

            Log.Info("Unary +");
            Assert.AreEqual<int>(+i1, +dyn1);
            Assert.AreEqual<int>(+i2, +dyn2);

            Log.Info("Unary -");
            Assert.AreEqual<int>(-i1, -dyn1);
            Assert.AreEqual<int>(-i2, -dyn2);

            Log.Info("Unary ~ (bitwise NOT)");
            Assert.AreEqual<int>(~i1, ~dyn1);
            Assert.AreEqual<int>(~i2, ~dyn2);

            Log.Info("Binary +: {0}", i1 + i2);
            Assert.AreEqual<int>(i1 + i2, dyn1 + dyn2);
            Assert.AreEqual<int>(i1 + i2, dyn2 + dyn1);
            Assert.AreEqual<int>(i1 + i2, dyn1 + i2);
            Assert.AreEqual<int>(i1 + i2, dyn2 + i1);

            Log.Info("Binary -: {0}, {1}", i1 - i2, i2 - i1);
            Assert.AreEqual<int>(i1 - i2, dyn1 - dyn2);
            Assert.AreEqual<int>(i2 - i1, dyn2 - dyn1);
            Assert.AreEqual<int>(i1 - i2, dyn1 - i2);
            Assert.AreEqual<int>(i2 - i1, dyn2 - i1);

            Log.Info("Binary *: {0}", i1 * i2);
            Assert.AreEqual<int>(i1 * i2, dyn1 * dyn2);
            Assert.AreEqual<int>(i1 * i2, dyn2 * dyn1);
            Assert.AreEqual<int>(i1 * i2, dyn1 * i2);
            Assert.AreEqual<int>(i1 * i2, dyn2 * i1);

            while (i1 == 0)
            {
                i1 = rndGen.Next(-10000, 10000);
                jv1 = i1;
                dyn1 = jv1;
                Log.Info("Using new (non-zero) i1 value: {0}", i1);
            }

            while (i2 == 0)
            {
                i2 = rndGen.Next(-10000, 10000);
                jv2 = i2;
                dyn2 = jv2;
                Log.Info("Using new (non-zero) i2 value: {0}", i2);
            }

            Log.Info("Binary / (integer division): {0}, {1}", i1 / i2, i2 / i1);
            Assert.AreEqual<int>(i1 / i2, dyn1 / dyn2);
            Assert.AreEqual<int>(i2 / i1, dyn2 / dyn1);
            Assert.AreEqual<int>(i1 / i2, dyn1 / i2);
            Assert.AreEqual<int>(i2 / i1, dyn2 / i1);

            Log.Info("Binary % (modulo): {0}, {1}", i1 % i2, i2 % i1);
            Assert.AreEqual<int>(i1 % i2, dyn1 % dyn2);
            Assert.AreEqual<int>(i2 % i1, dyn2 % dyn1);
            Assert.AreEqual<int>(i1 % i2, dyn1 % i2);
            Assert.AreEqual<int>(i2 % i1, dyn2 % i1);

            Log.Info("Binary & (bitwise AND): {0}", i1 & i2);
            Assert.AreEqual<int>(i1 & i2, dyn1 & dyn2);
            Assert.AreEqual<int>(i1 & i2, dyn2 & dyn1);
            Assert.AreEqual<int>(i1 & i2, dyn1 & i2);
            Assert.AreEqual<int>(i1 & i2, dyn2 & i1);

            Log.Info("Binary | (bitwise OR): {0}", i1 | i2);
            Assert.AreEqual<int>(i1 | i2, dyn1 | dyn2);
            Assert.AreEqual<int>(i1 | i2, dyn2 | dyn1);
            Assert.AreEqual<int>(i1 | i2, dyn1 | i2);
            Assert.AreEqual<int>(i1 | i2, dyn2 | i1);

            Log.Info("Binary ^ (bitwise XOR): {0}", i1 ^ i2);
            Assert.AreEqual<int>(i1 ^ i2, dyn1 ^ dyn2);
            Assert.AreEqual<int>(i1 ^ i2, dyn2 ^ dyn1);
            Assert.AreEqual<int>(i1 ^ i2, dyn1 ^ i2);
            Assert.AreEqual<int>(i1 ^ i2, dyn2 ^ i1);

            i1 = rndGen.Next(1, 10);
            i2 = rndGen.Next(1, 10);
            jv1 = i1;
            jv2 = i2;
            dyn1 = jv1;
            dyn2 = jv2;
            Log.Info("New i1, i2: {0}, {1}", i1, i2);

            Log.Info("Left shift: {0}", i1 << i2);
            Assert.AreEqual<int>(i1 << i2, dyn1 << dyn2);
            Assert.AreEqual<int>(i1 << i2, dyn1 << i2);

            i1 = i1 << i2;
            jv1 = i1;
            dyn1 = jv1;
            Log.Info("New i1: {0}", i1);
            Log.Info("Right shift: {0}", i1 >> i2);
            Assert.AreEqual<int>(i1 >> i2, dyn1 >> dyn2);
            Assert.AreEqual<int>(i1 >> i2, dyn1 >> i2);

            i2 += 4;
            jv2 = i2;
            dyn2 = jv2;
            Log.Info("New i2: {0}", i2);
            Log.Info("Right shift: {0}", i1 >> i2);
            Assert.AreEqual<int>(i1 >> i2, dyn1 >> dyn2);
            Assert.AreEqual<int>(i1 >> i2, dyn1 >> i2);
        }

        /// <summary>
        /// Test for creating a JsonValue from a deep-nested dynamic object.
        /// </summary>
        [TestMethod]
        public void CreateFromDeepNestedDynamic()
        {
            int count = 10000;
            string expected = "";

            dynamic dyn = new TestDynamicObject();
            dynamic cur = dyn;

            for (int i = 0; i < count; i++)
            {
                expected += "{\"" + i + "\":";
                cur[i.ToString()] = new TestDynamicObject();
                cur = cur[i.ToString()];
            }

            expected += "{}";

            for (int i = 0; i < count; i++)
            {
                expected += "}";
            }

            JsonValue jv = JsonValueExtensions.CreateFrom(dyn);
            Assert.AreEqual<string>(expected, jv.ToString());
        }

        /// <summary>
        /// Concrete DynamicObject class for testing purposes.
        /// </summary>
        internal class TestDynamicObject : DynamicObject
        {
            private IDictionary<string, object> _values = new Dictionary<string, object>();

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _values.Keys;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                _values[binder.Name] = value;
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return _values.TryGetValue(binder.Name, out result);
            }

            public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
            {
                string key = indexes[0].ToString();

                if (_values.ContainsKey(key))
                {
                    _values[key] = value;
                }
                else
                {
                    _values.Add(key, value);
                }
                return true;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                string key = indexes[0].ToString();

                if (_values.ContainsKey(key))
                {
                    result = _values[key];
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}
