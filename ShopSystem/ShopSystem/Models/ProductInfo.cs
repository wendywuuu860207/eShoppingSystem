using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopSystem.Models
{
    public class ProductInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int PTID { get; set; }
        public string PTName { get; set; }
        public int Price { get; set; }
        public string Note { get; set; }
        public int Stock { get; set; }
        public int State { get; set; }
        public string Pic { get; set; }
    }
}