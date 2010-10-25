//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.ServiceModel.Http.Test
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class OrderItemData
    {
        static IList<Order> _orders;
        static IList<Item> _items;
        static Order[] _orderArray;
        static OrderItemData()
        {
            _orders = new Order[]{
              new Order(){ OrderId=0, Customer = "FirstName0 LastName0", Price=10, Items = new List<Item>()},
              new Order(){ OrderId=1, Customer = "FirstName1 LastName1", Price=250, Items = new List<Item>()},
              new Order(){ OrderId=2, Customer = "FirstName2 LastName2",Price = 4, Items = new List<Item>()},
              new Order(){ OrderId=3, Customer = "FirstName3 LastName3", Price=30, Items = new List<Item>()},};
            _items = new Item[]{
              new Item(){ Product="Chai",  Quantity=1 },
              new Item(){ Product="Admiration Red Wine Vinegar",  Quantity=10 },
              new Item(){ Product="Aniseed Syrup",  Quantity=30 },
              new Item(){ Product="Balsamic Vinaigrette",   Quantity=2 },
              new Item(){ Product="Caffe Latte",  Quantity=1},
              new Item(){ Product="Chef Anton's Cajun Seasoning",  Quantity=25}};
            _orders[0].Items.Add(_items[0]);
            _orders[0].Items.Add(_items[1]);
            _orders[1].Items.Add(_items[2]);
            _orders[1].Items.Add(_items[3]);
            _orders[2].Items.Add(_items[4]);
            _orders[3].Items.Add(_items[5]);

            _orderArray = new Order[_orders.Count];
            for (int i = 0; i < _orders.Count; i++)
            {
                _orderArray[i] = _orders[i];
            }
        }

        public IQueryable<Order> Orders
        {
            get { return _orders.AsQueryable<Order>(); }
        }
        public IQueryable<Item> Items
        {
            get { return _items.AsQueryable<Item>(); }
        }

        public IList<Order> OrderList
        {
            get { return _orders; }
        }

        public Order[] OrderArray
        {
            get { return _orderArray; }
        }

    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string Customer { get; set; }
        public IList<Item> Items { get; set; }
        public int Price { get; set; }
    }

    public class Item
    {
        [Key]
        public string Product { get; set; }

        public int Quantity { get; set; }
    }

}

