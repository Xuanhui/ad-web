using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team1LUSS.Models
{
    public class PrepForRetrieval
    {
        private string itemCode;
        private string category;
        private string description;
        private int qty;

        public string ItemCode { get { return this.itemCode; } set { this.itemCode = value; } }
        public string Category { get { return this.category; } set { this.category = value; } }
        public string Description { get { return this.description; } set { this.description = value; } }
        public int Qty { get { return this.qty; } set { this.qty = value; } }

        public PrepForRetrieval(string name, string category, string des, int i)
        {
            this.itemCode = name;
            this.category = category;
            this.description = des;
            this.qty = i;

        }
    }
}