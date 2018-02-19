using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class DisbursementController : Controller
    {
        // GET: Disbursement
        //Reuben
        public ActionResult CollectionPoint()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 1 && e.Authorization != "Representative")
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE
            ViewBag.user = e;
            string point = e.Department.CollectionPoint;
            ViewBag.CollectionPoint = point;
            string collection = Request.Form["collection"];
            if (collection != null)
            {
                BusinessLogic.UpdateCollectionPoint(e.DepartmentID, collection);
                ViewBag.CollectionPoint = collection;
            }
            return View();
        }

        //Irene
        public ActionResult Completed()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 3 && e.Authorization != "Representative" && e.RoleID !=2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE
            ViewBag.user = e;
            ViewBag.diList = BusinessLogic.GetDisbursementByCompleted();
            ViewBag.diListByDept = BusinessLogic.GetDisbursementByCompleted(e.DepartmentID);
            return View();
        }
        //Irene
        public ActionResult Outstanding()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 3 && e.Authorization != "Representative" && e.RoleID != 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE
            ViewBag.user = e;   
            
            
            List<Disbursement> dlist = new List<Disbursement>();            
            if (Request.Form["sendemail"] != null)
            {
                String s = Request.Form["items"];   //DisbursementIDs
                Disbursement d;
                Email email;
                String emailaddress;
                string emailname;
                String bodytext;
                String subject = "Collection";

                foreach (var selection in s.Split(','))
                {
                    d = BusinessLogic.GetDisbursementById(Convert.ToInt32(selection));
                    dlist.Add(d);
                    emailaddress = d.Employee1.EmailAdd;
                    emailname = d.Employee1.EmployeeName;
                    bodytext = "Dear " + emailname + ",<br /><br />" + "Your request has already been processed and is ready for collection.<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                    email = new Email(emailaddress, subject, bodytext);
                    BusinessLogic.SendEmail(email);                    
                    TempData["Notify"] = "The selected items have been sent.";
                }
            }
            ViewBag.dlist = dlist;            
            ViewBag.delist = BusinessLogic.GetDisbursementByIncomplete();
            ViewBag.deptlist = BusinessLogic.GetAllDept();
            ViewBag.delistByDept = BusinessLogic.GetDisbursementByIncomplete(e.DepartmentID);
            return View();
        }
        //Irene
        public ActionResult Detail(int id)
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 3 && e.Authorization != "Representative" && e.RoleID != 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE
            ViewBag.user = e;

            if (Request.Form["save"] != null)
            {
                List<RequisitionDetail> rdList = BusinessLogic.GetRequisitionDetailItemList(id);

                String s = Request.Form["ItemCode"];    //DisbursementDetailID
                int dDetailItemID; int qty;
                bool canupdate = false;
                List<int> l1 = new List<int>();
                List<int> l2 = new List<int>();
                for (int i = 0; i < s.Split(',').Count(); i++)
                {
                    dDetailItemID = Convert.ToInt32(s.Split(',')[i]);
                    qty = Convert.ToInt32(Request.Form["ItemQty"].Split(',')[i]);
                    
                    DisbursementDetail dd = BusinessLogic.GetDisbursementDetail(dDetailItemID);
                    for (int j = 0; j < rdList.Count; j++)
                    {
                        if (rdList[j].ItemCode == dd.ItemCode)
                        {
                            if (qty >= 0)
                            {
                                if (qty <= rdList[j].ItemQty && dd.TempQty == null)
                                {
                                    l1.Add(dDetailItemID);
                                    l2.Add(qty);
                                    if(l1.Count == s.Split(',').Count())
                                    {
                                        canupdate = true;
                                    }                                                                       
                                }                                
                                else if (qty <= rdList[j].ItemQty && dd.TempQty != null)
                                {
                                    if(qty <= (dd.ItemQty-dd.TempQty))
                                    {
                                        l1.Add(dDetailItemID);
                                        l2.Add(qty);
                                        if (l1.Count == s.Split(',').Count())
                                        {
                                            canupdate = true;
                                        }
                                    }
                                    else
                                    {
                                        canupdate = false;
                                    TempData["Notify"] = "Entered quantity should not be more than the quantity of the requisition item.";
                                    }
                                }
                                else if (qty > rdList[j].ItemQty)
                                {
                                    canupdate = false;
                                    TempData["Notify"] = "Entered quantity should not be more than the quantity of the requisition item.";
                                }

                            }
                        }
                    }
                    if (canupdate != false)
                    {
                        for (int j = 0; j < l1.Count; j++)
                        {
                            BusinessLogic.UpdateQty(l1[j], l2[j]);                            
                        }
                        return RedirectToAction("Outstanding", "Disbursement");
                    }
                }
            }
            if (Request.Form["confirmed"] != null)
            {
                int disitem = Convert.ToInt32(Request.Form["DisbursementID"]);    //DisbursementID
                int empid = Convert.ToInt32(Request.Form["empID"]);
                String s = Request.Form["ItemCode"];
                String itemcode;
                for (int i = 0; i < s.Split(',').Count(); i++)
                {
                    itemcode = s.Split(',')[i];
                    BusinessLogic.UpdateVerify(empid, disitem, itemcode);
                }               

                List<DisbursementDetail> ddList = BusinessLogic.GetDisbursementDetailList(disitem);
                String itemCode, dept;
                int qty, bal;

                for (int i = 0; i < ddList.Count; i++)
                {
                    itemCode = ddList[i].ItemCode;
                    dept = ddList[i].Disbursement.DepartmentID;
                    if(ddList[i].TempQty!=null)
                    {
                        qty = (int)ddList[i].TempQty;
                    }
                    else
                    {
                        qty = (int)ddList[i].ItemQty;
                    }
                    Catalogue c = BusinessLogic.getCatalogueByID(itemCode);
                    bal = (int)c.CurrentStockQty - qty;
                    BusinessLogic.UpdateDisburseStockCard(itemCode, dept, qty, bal);
                    BusinessLogic.UpdateCatalogue(itemCode, qty);
                }
                List<RequisitionDetail> rdlist = BusinessLogic.GetRequisitionDetailItemList(disitem);
                int quantity = 0;
                for (int i = 0; i < ddList.Count; i++)
                {
                    if (ddList[i].ItemQty != ddList[i].TempQty)
                    {
                        itemCode = ddList[i].ItemCode;
                        Catalogue c = BusinessLogic.getCatalogueByID(itemCode);
                        if(ddList[i].TempQty!=null)
                        {
                            quantity = (int)(ddList[i].ItemQty - ddList[i].TempQty);
                        }
                        else
                        {
                            quantity = (int)(ddList[i].ItemQty);
                        }
                        BusinessLogic.CreateAdjustmentForDisbursement(itemCode, quantity, (int)ddList[i].Disbursement.Clerk);
                    }
                }
            }

            ViewBag.dlist = BusinessLogic.GetDisbursementById(id);
            ViewBag.delist = BusinessLogic.GetDisbursementDetailList(id);
            ViewBag.rlist = BusinessLogic.GetApprovedRequisition();
            ViewBag.deptitem = BusinessLogic.GetDeptInfo(ViewBag.dlist.DepartmentID);
            ViewBag.rdList = BusinessLogic.GetRequisitionDetailItemList(id);
            return View();
        }
    }
}