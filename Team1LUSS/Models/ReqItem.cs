using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team1LUSS.Models
{
    public class ReqItem
    {
        int CategoryID;
        String month;
        string year;
        string DepartmentID;
        List<Requisition> reql = new List<Requisition>();
        List<Requisition> dlist = new List<Requisition>();

        public string categoryID { get; set; }

        public ReqItem(string departmentid, string catagoryid, string year, string month)
        {
            this.CategoryID = int.Parse(catagoryid);
            this.year = year;
            this.month = month;
            this.DepartmentID = departmentid;
        }

        public List<Requisition> getReqByDepid()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<int> empid = new List<int>();
            List<Requisition> temp = new List<Requisition>();
            empid = e.Employees.Where(x => x.DepartmentID == DepartmentID).Select(x => x.EmployeeID).ToList();
            foreach (var item in empid)
            {
                temp = e.Requisitions.Where(x => x.EmployeeID == item).ToList();
                reql.AddRange(temp);
            }
            return reql;
        }

        public List<Requisition> selectedReqByDate()
        {
            if (month == "02")
            {
                DateTime monthstart = Convert.ToDateTime(year + "-" + month + "-01");
                DateTime monthend = Convert.ToDateTime(year + "-" + month + "-28");
                foreach (Requisition req in reql)
                {
                    if (req.Date > monthstart && req.Date < monthend)
                        dlist.Add(req);
                }
                return dlist;
            }
            else if (month == "01" || month == "03" || month == "05" || month == "07" || month == "08" || month == "10" || month == "12")
            {
                DateTime monthstart = Convert.ToDateTime(year + "-" + month + "-01");
                DateTime monthend = Convert.ToDateTime(year + "-" + month + "-31");
                foreach (Requisition req in reql)
                {
                    if (req.Date > monthstart && req.Date < monthend)
                        dlist.Add(req);
                }
                return dlist;
            }
            else
            {
                DateTime monthstart = Convert.ToDateTime(year + "-" + month + "-01");
                DateTime monthend = Convert.ToDateTime(year + "-" + month + "-30");
                foreach (Requisition req in reql)
                {
                    if (req.Date > monthstart && req.Date < monthend)
                        dlist.Add(req);
                }
                return dlist;
            }
        }

        public int? criteriaAmount()
        {
            LUSSISEntities e = new LUSSISEntities();
            List<RequisitionDetail> reqdetaillist = new List<RequisitionDetail>();
            List<RequisitionDetail> temp = new List<RequisitionDetail>();
            List<string> cata = new List<string>();

            List<Catalogue> cataBy = new List<Catalogue>();
            cata = e.Catalogues.Where(x => x.CategoryID == CategoryID).Select(x => x.ItemCode).ToList();
            int? amount = 0;

            foreach (Requisition req in dlist)
            {
                temp = e.RequisitionDetails.Where(x => x.RequisitionID == req.RequisitionID).ToList();
                reqdetaillist.AddRange(temp);
            }

            foreach (string code in cata)
            {
                foreach (RequisitionDetail reqd in reqdetaillist)
                {
                    if (reqd.ItemCode == code)
                        amount += reqd.ItemQty;
                }
            }
            return amount;
        }
    }
}