namespace System.Json.Test
{
    using System.Json;
    using Microsoft.CSharp.RuntimeBinder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonValueDynamicTests
    {
        string teamNameValue = "WCF RIA Base";
        string[] teamMembersValues = { "Carlos", "Chris", "Joe", "Miguel", "Yavor" };

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
    }
}
