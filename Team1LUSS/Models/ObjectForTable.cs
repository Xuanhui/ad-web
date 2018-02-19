using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace Team1LUSS.Models
{
    public class ObjectForTable
    {
        string itemName = string.Empty;
        int numbers;

        public ObjectForTable(ArrayList arr)
        {
            Numbers = this.caculate(arr);
        }
        public string ItemName { get; set; }
        public int Numbers { get; set; }
        public int caculate(ArrayList amount)
        {
            foreach (int item in amount)
            {
                numbers += item;
            }
            return numbers;
        }
    }
}