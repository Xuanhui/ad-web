using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        //Luowei
        //static String testm;
        public ActionResult OrderDetails()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID < 3)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            ViewBag.slist = BusinessLogic.GetSuplierListInPO();

            int intgetid = Convert.ToInt32(this.Request.QueryString["arid"]);
            if (this.Request.QueryString["arid"] != null)
            {
                ViewBag.slist = BusinessLogic.GetSuplierListInPOByID(intgetid);
            }

            ViewBag.po = BusinessLogic.getPurchaseOrderID(intgetid);

            ViewBag.odlist = BusinessLogic.listorderdetails(intgetid);

            List<SubTotal> sublist = new List<SubTotal>();

            List<PurchaseOrderDetail> polist = BusinessLogic.listorderdetails(intgetid);

            SubTotal sub;
            List<Supplier> supplierlist = BusinessLogic.getSupplierlist();
            foreach (var item in supplierlist)
            {
                sub = new SubTotal(item.SupplierID);
                sublist.Add(sub);
            }
            foreach (var item in polist)
            {
                foreach (var i in sublist)
                {
                    if (item.Supplier == i.SupplierName)
                    {
                        i.Stotal = i.Stotal + Convert.ToDouble(item.Qty) * Convert.ToInt32(item.Catalogue.AveragePrice);
                    }
                }
            }
            double gtotal = 0;
            foreach (var i in sublist)
            {
                gtotal = gtotal + i.Stotal;
            }

            ViewBag.gtotal = gtotal;
            ViewBag.subtotal = sublist;
            return View();
        }
        //Luowei
        public ActionResult OrderApproval()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID < 3)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            ViewBag.olist = BusinessLogic.listorder();
            ViewBag.oplist = BusinessLogic.listpendingorder();
            ViewBag.polistbyemployee = BusinessLogic.listapprovedorderbyEmployee(e.EmployeeID);
            ViewBag.popendinglistbyemployee = BusinessLogic.listpendingorderbyEmployee(e.EmployeeID);
            return View();
        }
        //Luowei
        public ActionResult PendingApprovalOrderDetails()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID < 3)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            ViewBag.slist = BusinessLogic.GetSuplierListInPO();

            int intgetid = Convert.ToInt32(this.Request.QueryString["oid"]);
            if (this.Request.QueryString["oid"] != null)
            {
                ViewBag.slist = BusinessLogic.GetSuplierListInPOByID(intgetid);

            }
            ViewBag.po = BusinessLogic.getPurchaseOrderID(intgetid);

            ViewBag.odlist = BusinessLogic.listorderdetails(intgetid);

            List<SubTotal> sublist = new List<SubTotal>();

            List<PurchaseOrderDetail> polist = BusinessLogic.listorderdetails(intgetid);

            SubTotal sub;
            List<Supplier> supplierlist = BusinessLogic.getSupplierlist();
            foreach (var item in supplierlist)
            {
                sub = new SubTotal(item.SupplierID);
                sublist.Add(sub);
            }
            foreach (var item in polist)
            {
                foreach (var i in sublist)
                {
                    if (item.Supplier == i.SupplierName)
                    {
                        i.Stotal = i.Stotal + Convert.ToDouble(item.Qty) * Convert.ToInt32(item.Catalogue.AveragePrice);
                    }
                }
            }
            double gtotal = 0;
            foreach (var i in sublist)
            {
                gtotal = gtotal + i.Stotal;
            }
            ViewBag.gtotal = gtotal;
            ViewBag.subtotal = sublist;
            return View();
        }
        //Luowei
        public ActionResult Approved()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID < 4)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            string s = this.Request.QueryString["approvedpoid"];
            int intapprovedpoid = Convert.ToInt32(this.Request.QueryString["approvedpoid"]);
            if (s != null)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    BusinessLogic.setApprovedPO(intapprovedpoid, e.EmployeeID);
                    BusinessLogic.UpdateStockCardAndCatalogue(intapprovedpoid);
                    ts.Complete();
                }

            }
            return View();
        }
        //Luowei
        public ActionResult Rejected()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID < 4)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            string s = this.Request.QueryString["rejectedpoid"];
            int intrejectedpoid = Convert.ToInt32(this.Request.QueryString["rejectedpoid"]);
            if (s != null)
            {
                BusinessLogic.setRejectedPO(intrejectedpoid, e.EmployeeID);
            }
            return View();
        }

        //Irene
        public ActionResult EditPurchaseOrder()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 3)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            String s = Request.Form["items"];
            List<Catalogue> clist = new List<Catalogue>();
            List<ItemSupplier> slist = new List<ItemSupplier>();

            if (s != null)
            {
                Catalogue c;                
                
                foreach (var selection in s.Split(','))
                {
                    c = BusinessLogic.getCatalogueByID(selection);
                    clist.Add(c);
                }

                ItemSupplier itemSupplier;
                String rank1 = "A";
                String rank2 = "B";
                String rank3 = "C";
                
                foreach (var selection in s.Split(','))
                {
                    
                        itemSupplier = BusinessLogic.getItemSupplierByID(selection, rank1);
                        slist.Add(itemSupplier);
                        itemSupplier = BusinessLogic.getItemSupplierByID(selection, rank2);
                        slist.Add(itemSupplier);
                        itemSupplier = BusinessLogic.getItemSupplierByID(selection, rank3);
                        slist.Add(itemSupplier);
                    
                }
            }
            ViewBag.clist = clist;            
            ViewBag.slist = slist;
            return View();
        }        
        //Irene
        public ActionResult CreatePurchaseOrder()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 3)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            String s = Request.Form["items"];
            if (s != null)
            {
                decimal amount = 0;
                int QtyNo = Request.Form["qty"].Split(',').Count();
                Response.Write(QtyNo);
                
                int oid = BusinessLogic.AddItem(e.EmployeeID);
                for (int i = 0; i < QtyNo; i++)
                {
                    if (Convert.ToInt32(Request.Form["qty"].Split(',')[i]) > 0)
                    {
                        BusinessLogic.AddItemDetail(oid, Request.Form["supplier"].Split(',')[i], s.Split(',')[i], Convert.ToInt32(Request.Form["qty"].Split(',')[i]));
                    }
                    amount = amount + Convert.ToInt32(Request.Form["qty"].Split(',')[i]) * Convert.ToDecimal(BusinessLogic.getCatalogueByID(s.Split(',')[i]).AveragePrice);
                }
                BusinessLogic.UpdateAmountByID(oid, amount);

            }
            ViewBag.amount = s;
            ViewBag.olist = BusinessLogic.GetOrderCatalogue();

            return View();
        }
        //Irene
        public ActionResult Add()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 3)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            String s = Request.Form["itemcode"];
            if (s != null)
            {
                decimal amount = 0;
                int QtyNo = Request.Form["qty"].Split(',').Count();
                //validation
                bool cancontinue = true; int i1, i2, i3; String itemcode; int errorrow = 0;
                for (int i = 0; i < QtyNo; i = i + BusinessLogic.getSupplierActive().Count)
                {
                    i1 = Convert.ToInt32(Request.Form["qty"].Split(',')[i]);
                    i2 = Convert.ToInt32(Request.Form["qty"].Split(',')[i + 1]);
                    i3 = Convert.ToInt32(Request.Form["qty"].Split(',')[i + 2]);
                    itemcode = s.Split(',')[i];
                    if (!CheckMin(i1, i2, i3, itemcode)) { cancontinue = false; errorrow = i; }
                }
                if (cancontinue)
                {                    
                    int oid = BusinessLogic.AddItem(e.EmployeeID);
                    for (int i = 0; i < QtyNo; i++)
                    {
                        BusinessLogic.AddItemDetail(oid, Request.Form["supplier"].Split(',')[i], s.Split(',')[i], Convert.ToInt32(Request.Form["qty"].Split(',')[i]));
                        amount = amount + Convert.ToInt32(Request.Form["qty"].Split(',')[i]) * Convert.ToDecimal(BusinessLogic.getCatalogueByID(s.Split(',')[i]).AveragePrice);
                    }
                    BusinessLogic.UpdateAmountByID(oid, amount);
                    Response.Redirect("./CreatePurchaseOrder");
                }
                else { ViewBag.ugoterror = errorrow; Response.Redirect("./EditPurchaseOrder"); }
            }
            return View();
        }
        //Irene
        public bool CheckMin(int i1, int i2, int i3, String itemcode)
        {
            if ((i1 + i2 + i3) > BusinessLogic.getCatalogueByID(itemcode).ReorderMinQty)
            {
                return true;
            }
            else { return false; }
        }
    }
}