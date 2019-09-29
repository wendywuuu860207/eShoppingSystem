using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopSystem.Models
{
    public class CartItem
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? Stock { get; set; }
        
        private double price;
        public double Price
        {
            get{ return this.price; }
            set {
                this.price = value;
                this.subTotal = this.price * this.quantity;
            }
        }
        
        private int quantity;
        public int Quantity
        {
            get { return this.quantity; }
            set
            {
                this.quantity = value;
                this.subTotal = this.price * this.quantity;
            }
        }

        private double subTotal;
        public double SubTotal
        {
            get { return this.subTotal; }
        }

        public override int GetHashCode()
        {
            return this.ID.Value;
        }

        public override bool Equals(object obj)
        {
            bool ans = false;
            if (obj is CartItem)
            {
                CartItem cc = obj as CartItem;
                if (cc.ID == this.ID)
                    ans = true;
            }

            return ans;
        }

    }
}