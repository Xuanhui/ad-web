using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace Team1LUSS.Models
{
    public class ObjectForBar
    {
        string categoryName;
        ArrayList arr;
        public ObjectForBar(string name, ArrayList arrlist)
        {
            CategoryName = this.getName(name);
            Arr = arrlist;
        }
        public string CategoryName { get; set; }
        public ArrayList Arr { get; set; }
        public string getName(string ID)
        {
            int CaName = int.Parse(ID);
            LUSSISEntities lussis = new Models.LUSSISEntities();
            string name = lussis.Categories.Where(x => x.CategoryID == CaName).Select(x => x.CategoryName).First();
            return name;
        }
    }
}