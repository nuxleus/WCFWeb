// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UrlQueryComposerTests
    {
        UrlQueryComposer queryComposer = new UrlQueryComposer();
        string baseAddress = "http://localhost:8000/Orders";
        IQueryable<Order> rootQuery = new OrderItemData().Orders;

        #region Filter test

        [TestMethod]
        public void TestFilter1()
        {
            string finalString = baseAddress + "?$filter=Customer eq 'FirstName0 LastName0'";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();
            
            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
        }

        [TestMethod]
        public void TestFilter2()
        {
            string finalString = baseAddress + "?$filter=OrderId eq 2";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(2, orders[0].OrderId);
        }

        [TestMethod]
        public void TestFilter3()
        {
            string finalString = baseAddress + "?$filter=OrderId ne 2";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(3, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
            Assert.AreEqual(1, orders[1].OrderId);
            Assert.AreEqual(3, orders[2].OrderId);
        }

        [TestMethod]
        public void TestFilter4()
        {
            string finalString = baseAddress + "?$filter=OrderId gt 2";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(3, orders[0].OrderId);
        }

        [TestMethod]
        public void TestFilter5()
        {
            string finalString = baseAddress + "?$filter=OrderId lt 1";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
        }

        [TestMethod]
        public void TestFilter6()
        {
            string finalString = baseAddress + "?$filter=OrderId lt 0";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(0, orders.Count);
        }

        [TestMethod]
        public void TestFilter7()
        {
            string finalString = baseAddress + "?$filter=OrderId sub 2 eq 0";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(2, orders[0].OrderId);
        }

        [TestMethod]
        public void TestFilter8()
        {
            string finalString = baseAddress + "?$filter=OrderId sub 3 le 0";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
        }


        [TestMethod]
        public void TestFilter9()
        {
            string finalString = baseAddress + "?$filter=OrderId mod 2 eq 0";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(2, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
            Assert.AreEqual(2, orders[1].OrderId);
        }


        [TestMethod]
        public void TestFilter10()
        {
            string finalString = baseAddress + "?$filter=Price ge 10 and OrderId le 3";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(3, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
            Assert.AreEqual(1, orders[1].OrderId);
            Assert.AreEqual(3, orders[2].OrderId);
        }


        [TestMethod]
        public void TestFilter11()
        {
            string finalString = baseAddress + "?$filter=Price ge 10 and OrderId le 3";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(3, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
            Assert.AreEqual(1, orders[1].OrderId);
            Assert.AreEqual(3, orders[2].OrderId);
        }

        #endregion

        #region Skip Top Test

        [TestMethod]
        public void TestSkipTop1()
        {
            string finalString = baseAddress + "?$top=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
        }

        [TestMethod]
        public void TestSkipTop2()
        {
            string finalString = baseAddress + "?$top=10&$skip=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(0, orders.Count);
        }

        [TestMethod]
        public void TestSkipTop3()
        {
            string finalString = baseAddress + "?$skip=1";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(3, orders.Count);
            Assert.AreEqual(1, orders[0].OrderId);
            Assert.AreEqual(2, orders[1].OrderId);
            Assert.AreEqual(3, orders[2].OrderId);
        }

        [TestMethod]
        public void TestSkipTop4()
        {
            string finalString = baseAddress + "?$skip=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(0, orders.Count);
        }

        [TestMethod]
        public void TestSkipTop5()
        {
            string finalString = baseAddress + "?$filter=OrderId eq 2&$top=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(2, orders[0].OrderId);
        }

        [TestMethod]
        public void TestSkipTop6()
        {
            string finalString = baseAddress + "?$filter=OrderId eq 2&$skip=1";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(0, orders.Count);
        }

        [TestMethod]
        public void TestSkipTop7()
        {
            string finalString = baseAddress + "?OrderId eq 2&$top=10&$skip=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(0, orders.Count);
        }

        [TestMethod]
        public void TestSkipTop8()
        {
            string finalString = baseAddress + "?$filter=Price ge 10 and OrderId le 3&$top=1";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
        }

        [TestMethod]
        public void TestSkipTop9()
        {
            string finalString = baseAddress + "?$filter=Price le 10 and OrderId le 4&$skip=1";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(1, orders.Count);
            Assert.AreEqual(2, orders[0].OrderId);
        }

        #endregion


        #region Orderby Test

        [TestMethod]
        public void TestOrderby1()
        {
            string finalString = baseAddress + "?$orderby=Customer asc&$skip=0";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
            Assert.AreEqual(1, orders[1].OrderId);
            Assert.AreEqual(2, orders[2].OrderId);
            Assert.AreEqual(3, orders[3].OrderId);
        }

        [TestMethod]
        public void TestOrderby2()
        {
            string finalString = baseAddress + "?$orderby=OrderId asc";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(0, orders[0].OrderId);
            Assert.AreEqual(1, orders[1].OrderId);
            Assert.AreEqual(2, orders[2].OrderId);
            Assert.AreEqual(3, orders[3].OrderId);

        }

        [TestMethod]
        public void TestOrderby3()
        {
            string finalString = baseAddress + "?$orderby=OrderId desc";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(3, orders[0].OrderId);
            Assert.AreEqual(2, orders[1].OrderId);
            Assert.AreEqual(1, orders[2].OrderId);
            Assert.AreEqual(0, orders[3].OrderId);
        }

        [TestMethod]
        public void TestOrderby4()
        {
            string finalString = baseAddress + "?$orderby=OrderId desc&$top=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(3, orders[0].OrderId);
            Assert.AreEqual(2, orders[1].OrderId);
            Assert.AreEqual(1, orders[2].OrderId);
            Assert.AreEqual(0, orders[3].OrderId);
        }

        [TestMethod]
        public void TestOrderby5()
        {
            string finalString = baseAddress + "?$orderby=Customer desc&$top=10";
            IEnumerable<Order> finalQuery = this.queryComposer.ComposeQuery(rootQuery, finalString) as IEnumerable<Order>;
            List<Order> orders = finalQuery.ToList<Order>();

            Assert.AreEqual(4, orders.Count);
            Assert.AreEqual(3, orders[0].OrderId);
            Assert.AreEqual(2, orders[1].OrderId);
            Assert.AreEqual(1, orders[2].OrderId);
            Assert.AreEqual(0, orders[3].OrderId);
        }

        #endregion
    }
}
