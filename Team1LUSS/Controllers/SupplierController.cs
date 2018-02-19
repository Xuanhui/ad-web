using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Supplier
        //Reuben
        public ActionResult SupplierList()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            //if (e.RoleID < 3)
            //{
            //    return RedirectToAction("NotAuthorized");
            //}
            //EVERYPAGE

            ViewBag.user = e;
            ViewBag.SupplierList = new BusinessLogic().GetSuppliers;
            return View();
        }

        //Reuben
        public ActionResult SupplierCard()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            //if (e.RoleID < 3)
            //{
            //    return RedirectToAction("NotAuthorized");
            //}
            //EVERYPAGE

            string sid = Request.QueryString["SuppID"];
            ViewBag.user = e;
            ViewBag.Aitems = new BusinessLogic().GetAItemsBySupplier(sid);
            ViewBag.Bitems = new BusinessLogic().GetBItemsBySupplier(sid);
            ViewBag.Citems = new BusinessLogic().GetCItemsBySupplier(sid);
            ViewBag.Allitems = new BusinessLogic().GetAllItemsBySupplier(sid);
            return View();
        }
    }
}