using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopSystem.Models
{
    public class Order
    {
        public int orderID { get; set; }
        public int custID { get; set; }
        public string rcptName { get; set; }
        public string rcptPhone { get; set; }
        public string rcptAddr { get; set; }
        public int DTID { get; set; }
        public int Shipping { get; set; }
        public int orderState { get; set; }
        public string orderNote { get; set; }
        public DateTime orderTime { get; set; }
    }
}