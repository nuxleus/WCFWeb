namespace System.Json.Test
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Json;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonValueLinqExtensionsTest
    {
        [TestMethod]
        public void ToJsonArrayTest()
        {
            string json = "{\"SearchResponse\":{\"Phonebook\":{\"Results\":[{\"name\":1,\"rating\":1}, {\"name\":2,\"rating\":2}, {\"name\":3,\"rating\":3}]}}}";
            string expected = "[{\"name\":2,\"rating\":2},{\"name\":3,\"rating\":3}]";

            JsonValue jv = JsonValue.Parse(json);
            double rating = 1;
            var jsonResult = from n in jv.ValueOrDefault("SearchResponse", "Phonebook", "Results")
                             where n.Value.ValueOrDefault("rating").ReadAs<double>(0.0) > rating
                             select n.Value;
            var ja = jsonResult.ToJsonArray();

            Assert.AreEqual<string>(expected, ja.ToString());
        }

        [TestMethod]
        public void ToJsonObjectTest()
        {
            string json = "{\"Name\":\"Bill Gates\",\"Age\":23,\"AnnualIncome\":45340.45,\"MaritalStatus\":\"Single\",\"EducationLevel\":\"MiddleSchool\",\"SSN\":432332453,\"CellNumber\":2340393420}";
            string expected = "{\"AnnualIncome\":45340.45,\"SSN\":432332453,\"CellNumber\":2340393420}";

            JsonValue jv = JsonValue.Parse(json);
            decimal decVal;
            var jsonResult = from n in jv
                             where n.Value.TryReadAs<decimal>(out decVal) && decVal > 100
                             select n;
            var jo = jsonResult.ToJsonObject();

            Assert.AreEqual<string>(expected, jo.ToString());
        }

        [TestMethod]
        public void ToJsonObjectFromArrayTest()
        {
            string json = "{\"SearchResponse\":{\"Phonebook\":{\"Results\":[{\"name\":1,\"rating\":1}, {\"name\":2,\"rating\":2}, {\"name\":3,\"rating\":3}]}}}";
            string expected = "{\"1\":{\"name\":2,\"rating\":2},\"2\":{\"name\":3,\"rating\":3}}";

            JsonValue jv = JsonValue.Parse(json);
            double rating = 1;
            var jsonResult = from n in jv.ValueOrDefault("SearchResponse", "Phonebook", "Results")
                             where n.Value.ValueOrDefault("rating").ReadAs<double>(0.0) > rating
                             select n;
            var jo = jsonResult.ToJsonObject();

            Assert.AreEqual<string>(expected, jo.ToString());
        }
    }
}
