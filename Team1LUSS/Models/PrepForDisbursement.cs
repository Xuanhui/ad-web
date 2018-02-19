using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team1LUSS.Models
{
    public class PrepForDisbursement
    {
        private string itemCode;
        private string description;
        private int qty;
        private string status;
        public string ItemCode { get { return this.itemCode; } set { this.itemCode = value; } }
        public string Description { get { return this.description; } set { this.description = value; } }
        public int Qty { get { return this.qty; } set { this.qty = value; } }
        public string Status { get { return this.status; } set { this.status = value; } }
        public PrepForDisbursement(string name, string des, int i, string status)
        {
            this.itemCode = name;
            this.description = des;
            this.qty = i;
            this.status = status;
        }
    }
}