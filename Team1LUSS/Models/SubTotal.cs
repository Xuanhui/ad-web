using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team1LUSS.Models
{
    public class SubTotal
    {
        private String suppliername;
        private double stotal;
        public String SupplierName { get { return this.suppliername; } set { this.suppliername = value; } }
        public double Stotal { get { return this.stotal; } set { this.stotal = value; } }

        public SubTotal(String n)
        {
            this.suppliername = n;
            this.stotal = 0;
        }
    }
}