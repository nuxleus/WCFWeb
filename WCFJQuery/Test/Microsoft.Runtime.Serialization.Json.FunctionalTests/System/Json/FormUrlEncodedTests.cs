namespace Microsoft.ServiceModel.Web.Test
{
    using System;
    using System.Collections.Specialized;
    using System.Json;
    using System.Json.Test;
    using global::Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FormUrlEncodedTests
    {
        [TestMethod]
        public void TestJsonPrimitive()
        {
            this.TestFormEncodedParsing("abc", @"{""abc"":null}");
            this.TestFormEncodedParsing("123", @"{""123"":null}");
            this.TestFormEncodedParsing("true", @"{""true"":null}");
            this.TestFormEncodedParsing("", "{}");
            this.TestFormEncodedParsing("%2fabc%2f", @"{""\/abc\/"":null}");
        }

        [TestMethod]
        public void TestJsonPrimitiveNegative()
        {
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[b]=1&a=2"));
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a=2&a[b]=1"));
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("[]=1"));
            JsonValueTests.ExpectException<ArgumentNullException>(() => JsonValueExtensions.ParseFormUrlEncoded((string)null));
            JsonValueTests.ExpectException<ArgumentNullException>(() => JsonValueExtensions.ParseFormUrlEncoded((NameValueCollection)null));
        }

        [TestMethod]
        public void TestObjects()
        {
            this.TestFormEncodedParsing("a=NaN", @"{""a"":""NaN""}");
            this.TestFormEncodedParsing("a=false", @"{""a"":""false""}");
            this.TestFormEncodedParsing("a=foo", @"{""a"":""foo""}");
            this.TestFormEncodedParsing("1=1", "{\"1\":\"1\"}");
        }

        [TestMethod]
        public void TestArray()
        {
            this.TestFormEncodedParsing("a[]=2", @"{""a"":[""2""]}");
            this.TestFormEncodedParsing("a[]=", @"{""a"":[""""]}");
            this.TestFormEncodedParsing("a[0][0][]=1", @"{""a"":[[[""1""]]]}");
            this.TestFormEncodedParsing("z[]=9&z[]=true&z[]=undefined&z[]=", @"{""z"":[""9"",""true"",""undefined"",""""]}");
            this.TestFormEncodedParsing("z[]=9&z[]=true&z[]=undefined&z[]=null", @"{""z"":[""9"",""true"",""undefined"",""null""]}");
            this.TestFormEncodedParsing("z[0][]=9&z[0][]=true&z[1][]=undefined&z[1][]=null", @"{""z"":[[""9"",""true""],[""undefined"",""null""]]}");
            this.TestFormEncodedParsing("a[0][x]=2", @"{""a"":[{""x"":""2""}]}");
            this.TestFormEncodedParsing("a%5B%5D=2", @"{""a"":[""2""]}");
            this.TestFormEncodedParsing("a%5B%5D=", @"{""a"":[""""]}");
            this.TestFormEncodedParsing("z%5B%5D=9&z%5B%5D=true&z%5B%5D=undefined&z%5B%5D=", @"{""z"":[""9"",""true"",""undefined"",""""]}");
            this.TestFormEncodedParsing("z%5B%5D=9&z%5B%5D=true&z%5B%5D=undefined&z%5B%5D=null", @"{""z"":[""9"",""true"",""undefined"",""null""]}");
            this.TestFormEncodedParsing("z%5B0%5D%5B%5D=9&z%5B0%5D%5B%5D=true&z%5B1%5D%5B%5D=undefined&z%5B1%5D%5B%5D=null", @"{""z"":[[""9"",""true""],[""undefined"",""null""]]}");
        }

        [TestMethod]
        public void TestArrayCompat()
        {
            this.TestFormEncodedParsing("z=9&z=true&z=undefined&z=", @"{""z"":[""9"",""true"",""undefined"",""""]}");
            this.TestFormEncodedParsing("z=9&z=true&z=undefined&z=null", @"{""z"":[""9"",""true"",""undefined"",""null""]}");
            this.TestFormEncodedParsing("z=9&z=true&z=undefined&z=null&a=hello", @"{""z"":[""9"",""true"",""undefined"",""null""],""a"":""hello""}");
        }

        [TestMethod]
        public void TestArrayCompatNegative()
        {
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[z]=2&a[z]=3"), "a[z]");
        }

        [TestMethod]
        public void TestArraySparse()
        {
            this.TestFormEncodedParsing("a[2]=hello", @"{""a"":{""2"":""hello""}}");
            this.TestFormEncodedParsing("a[x][0]=2", @"{""a"":{""x"":{""0"":""2""}}}");
            this.TestFormEncodedParsing("a[x][1]=2", @"{""a"":{""x"":{""1"":""2""}}}");
            this.TestFormEncodedParsing("a[x][0]=0&a[x][1]=1", @"{""a"":{""x"":{""0"":""0"",""1"":""1""}}}");
            this.TestFormEncodedParsing("a[0][0][0]=hello&a[1][0][0][0][]=hello", @"{""a"":[[{""0"":""hello""}],[[[[""hello""]]]]]}");
            this.TestFormEncodedParsing("a[0][0][0]=hello&a[1][0][0][0]=hello", @"{""a"":[[{""0"":""hello""}],[[{""0"":""hello""}]]]}");
            this.TestFormEncodedParsing("a[1][0][]=1", @"{""a"":{""1"":[[""1""]]}}");
            this.TestFormEncodedParsing("a[1][1][]=1", @"{""a"":{""1"":{""1"":[""1""]}}}");
            this.TestFormEncodedParsing("a[1][1][0]=1", @"{""a"":{""1"":{""1"":{""0"":""1""}}}}");
            this.TestFormEncodedParsing("a[0][]=2&a[0][]=3&a[2][]=1", "{\"a\":{\"0\":[\"2\",\"3\"],\"2\":[\"1\"]}}");
        }

        [TestMethod]
        public void TestArrayIndexNegative()
        {
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[x]=2&a[x][]=3"), "a[x]");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[x][]=1&a[x][0]=2"), "a[x][0]");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[x][]=1&a[x][1]=2"), "a[x][1]");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[x][0]=1&a[x][]=2"), "a[x][]");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[][]=0"), "a[]");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[][x]=0"), "a[]");
        }

        [TestMethod]
        public void TestObject()
        {
            string encoded = "a[]=4&a[]=5&b[x][]=7&b[y]=8&b[z][]=9&b[z][]=true&b[z][]=undefined&b[z][]=&c=1&f=";
            string resultStr = @"{""a"":[""4"",""5""],""b"":{""x"":[""7""],""y"":""8"",""z"":[""9"",""true"",""undefined"",""""]},""c"":""1"",""f"":""""}";
            this.TestFormEncodedParsing(encoded, resultStr);

            encoded = "customer[Name]=Pete&customer[Address]=Redmond&customer[Age][0][]=23&customer[Age][0][]=24&customer[Age][1][]=25&" +
                "customer[Age][1][]=26&customer[Phones][]=425+888+1111&customer[Phones][]=425+345+7777&customer[Phones][]=425+888+4564&" +
                "customer[EnrolmentDate]=%22%5C%2FDate(1276562539537)%5C%2F%22&role=NewRole&changeDate=3&count=15";
            resultStr = @"{""customer"":{""Name"":""Pete"",""Address"":""Redmond"",""Age"":[[""23"",""24""],[""25"",""26""]]," +
                @"""Phones"":[""425 888 1111"",""425 345 7777"",""425 888 4564""],""EnrolmentDate"":""\""\\\/Date(1276562539537)\\\/\""""},""role"":""NewRole"",""changeDate"":""3"",""count"":""15""}";
            this.TestFormEncodedParsing(encoded, resultStr);

            encoded = "customers[0][Name]=Pete2&customers[0][Address]=Redmond2&customers[0][Age][0][]=23&customers[0][Age][0][]=24&" +
                "customers[0][Age][1][]=25&customers[0][Age][1][]=26&customers[0][Phones][]=425+888+1111&customers[0][Phones][]=425+345+7777&" +
                "customers[0][Phones][]=425+888+4564&customers[0][EnrolmentDate]=%22%5C%2FDate(1276634840700)%5C%2F%22&customers[1][Name]=Pete3&" +
                "customers[1][Address]=Redmond3&customers[1][Age][0][]=23&customers[1][Age][0][]=24&customers[1][Age][1][]=25&customers[1][Age][1][]=26&" +
                "customers[1][Phones][]=425+888+1111&customers[1][Phones][]=425+345+7777&customers[1][Phones][]=425+888+4564&customers[1][EnrolmentDate]=%22%5C%2FDate(1276634840700)%5C%2F%22";
            resultStr = @"{""customers"":[{""Name"":""Pete2"",""Address"":""Redmond2"",""Age"":[[""23"",""24""],[""25"",""26""]]," +
                @"""Phones"":[""425 888 1111"",""425 345 7777"",""425 888 4564""],""EnrolmentDate"":""\""\\\/Date(1276634840700)\\\/\""""}," +
                @"{""Name"":""Pete3"",""Address"":""Redmond3"",""Age"":[[""23"",""24""],[""25"",""26""]],""Phones"":[""425 888 1111"",""425 345 7777"",""425 888 4564""],""EnrolmentDate"":""\""\\\/Date(1276634840700)\\\/\""""}]}";
            this.TestFormEncodedParsing(encoded, resultStr);

            encoded = "ab%5B%5D=hello";
            resultStr = @"{""ab"":[""hello""]}";
            this.TestFormEncodedParsing(encoded, resultStr);

            encoded = "123=hello";
            resultStr = @"{""123"":""hello""}";
            this.TestFormEncodedParsing(encoded, resultStr);
        }

        [TestMethod]
        public void TestEncodedName()
        {
            string encoded = "some+thing=10";
            string resultStr = @"{""some thing"":""10""}";
            this.TestFormEncodedParsing(encoded, resultStr);

            encoded = "%E5%B8%A6%E4%B8%89%E4%B8%AA%E8%A1%A8=bar";
            resultStr = @"{""带三个表"":""bar""}";
            this.TestFormEncodedParsing(encoded, resultStr);

            encoded = "some+thing=10&%E5%B8%A6%E4%B8%89%E4%B8%AA%E8%A1%A8=bar";
            resultStr = @"{""some thing"":""10"",""带三个表"":""bar""}";
            this.TestFormEncodedParsing(encoded, resultStr);
        }

        [TestMethod]
        public void TestNegative()
        {
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[b=2"), "1");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[[b]=2"), "2");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("a[b]]=2"), "4");
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("&some+thing=10&%E5%B8%A6%E4%B8%89%E4%B8%AA%E8%A1%A8=bar"));
            JsonValueTests.ExpectException<ArgumentException>(() => JsonValueExtensions.ParseFormUrlEncoded("some+thing=10&%E5%B8%A6%E4%B8%89%E4%B8%AA%E8%A1%A8=bar&"));
        }

        void TestFormEncodedParsing(string encoded, string expectedResult)
        {
            JsonObject result = JsonValueExtensions.ParseFormUrlEncoded(encoded);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result.ToString());
        }
    }
}
