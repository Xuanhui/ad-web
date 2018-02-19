using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Inventory
        //Reuben
        public ActionResult StoreCatalogue()
        {
            //NEED THIS FOR EVERY PAGE (except login, logout, notauthorized)
            //
            //Stops someone from typing in the url to access pages if they have not logged in.
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            //Stops users from having access to pages for other roles.
            Employee e = (Employee)Session["user"];
            //if (e.RoleID < 3) //make sure this is correct for each page.
            //{
            //    return RedirectToAction("NotAuthorized");
            //}
            //
            //NEED THIS FOR EVERY PAGE ^

            ViewBag.user = e;
            ViewBag.ItemList = new BusinessLogic().GetCatalogue;
            return View();
        }
        
        //Reuben
        public ActionResult StockCard()
        {
            //EVERY PAGE
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
            string rid = Request.QueryString["ItemCode"];
            ViewBag.StockCard = new BusinessLogic().GetStockCard(rid);
            ViewBag.Suppliers = new BusinessLogic().GetSuppliersByItem(rid);
            return View();
        }
    }
}