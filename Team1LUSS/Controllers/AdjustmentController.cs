using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class AdjustmentController : Controller
    {
        // GET: Adjustment
        //Claire - start
        static String sa;        
        public ActionResult CreateAdjustmentList()
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
            string date = Request.Form["date"];
            string createpeople = Request.Form["createpeople"];
            //int employeeid = BusinessLogic.FindClerkIDByName();
            string itemcode = Request.Form["id"];
            string addorminus = Request.Form["addorminus"];
            string quantity = Request.Form["quantity"];
            string reason = Request.Form["reason"];
            List<AdjustmentVoucher> newAdjustmentList = new List<AdjustmentVoucher>();
            string[] itemcodelist = itemcode.Split(',');
            string[] addorminuslist = addorminus.Split(',');
            string[] quantitylist = quantity.Split(',');
            string[] reasonlist = reason.Split(',');
            for (int i = 0; i < itemcodelist.Count(); i++)
            {
                AdjustmentVoucher ad = new AdjustmentVoucher();
                ad.Date = Convert.ToDateTime(date);
                ad.ItemCode = itemcodelist[i];
                ad.AddorMinus = addorminuslist[i];
                ad.Qty = Convert.ToInt32(quantitylist[i]);
                ad.Reason = reasonlist[i];
                ad.Status = "Pending";
                ad.ClerkID = e.EmployeeID;
                newAdjustmentList.Add(ad);
            }
            BusinessLogic.CreateAdjustment(newAdjustmentList);
            Response.Redirect("/Adjustment/ViewAdjustmentForClerk/");           
            return View();

        }
        public ActionResult SelectAdjustmentItems()
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
            ViewBag.cataloguelist = BusinessLogic.FindAllCatalogue();
            ViewBag.testm = sa;
            return View();
        }


        public ActionResult CreateNewAdjustment()
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
            List<Catalogue> selectedlist = new List<Catalogue>();
            string selecteditem = Request.Form["selecteditems"];
            if (selecteditem != null)
            {
                foreach (var item in selecteditem.Split(','))
                {
                    Catalogue c = BusinessLogic.FindItemByItemID(item);
                    selectedlist.Add(c);
                }

            }
            ViewBag.catalogueList = selectedlist;

            return View();
        }


        public ActionResult ViewAdjustmentForClerk()
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
            //ViewBag.Count = new BusinessLogic().getCount();
            ViewBag.AdjustmentListPending = BusinessLogic.FindAllAdjustmentPendingClerk();
            ViewBag.AdjustmentListCpmplete = BusinessLogic.FindAllAdjustmentComplete();
            ViewBag.Approvalname = BusinessLogic.FindApproveEmployees();
            return View();
        }
        public ActionResult ViewAdjustmentForSuperior()
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
            ViewBag.adjustmentlistforsupervisorPending = BusinessLogic.FindAllAdjustmentForSupervisorPending();
            ViewBag.adjustmentlistforsupervisorComplete = BusinessLogic.FindAllAdjustmentForSupervisorComplete();
            ViewBag.adjustmentlistformanagerPending = BusinessLogic.FindAllAdjustmentForManagerPending();
            ViewBag.adjustmentlistformanagerComplete = BusinessLogic.FindAllAdjustmentForManagerComplete();
            return View();
        }
        public ActionResult ViewAdjustmentForSuperiorAgree(String id)
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
            int aid = Convert.ToInt32(id);
            //BusinessLogic.SetAdjustmentToAgreeByID(aid);
            BusinessLogic.SetAdjustmentToAgreeByID(aid, e.EmployeeID);
            BusinessLogic.CreateNewStockCard(aid);
            BusinessLogic.UpdateCurrentStockQty(aid);
            Response.Redirect("/Adjustment/ViewAdjustmentForSuperior/");            
            return View();
        }

        public ActionResult ViewAdjustmentForSuperiorReject(String id)
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
            int aid = Convert.ToInt32(id);
            //BusinessLogic.SetAdjustmentToRejectByID(aid);
            BusinessLogic.SetAdjustmentToRejectByID(aid, e.EmployeeID);
            Response.Redirect("/Adjustment/ViewAdjustmentForSuperior/");
            return View();

        }
        public ActionResult EditAdjustmentVoucher(string id)
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
            int aid = Convert.ToInt16(id);
            ViewBag.adjustmentforedit = BusinessLogic.FindAdjustmentByID(aid);
            return View();
        }
        public ActionResult UpdateAdjustmentVoucher()
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
            string id = Request.Form["id"];
            int aid = Convert.ToInt16(id);
            string addorminus = Request.Form["addorminus"];
            string Qty = Request.Form["Qty"];
            int qty = Convert.ToInt16(Qty);

            string reason = Request.Form["Reason"];
            BusinessLogic.UpdateAdjustmentByID(aid, addorminus, qty, reason);
            Response.Redirect("/Adjustment/ViewAdjustmentForClerk/");
            return View();
        }
        public ActionResult DeleteAdjustmentVoucher(string id)
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
            int aid = Convert.ToInt16(id);
            BusinessLogic.DeleteAdjustmentByID(aid);
            Response.Redirect("/Adjustment/ViewAdjustmentForClerk/");
            return View();
        }
        //Claire - end
    }
}