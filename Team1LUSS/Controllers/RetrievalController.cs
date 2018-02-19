using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class RetrievalController : Controller
    {
        // GET: Retrieval
        public ActionResult StockRetrievalMain()
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

            //Creation of Retrieval
            string s = Request.Form["itemcode"];
            if (s != null)
            {
                string sqty = Request.Form["qty"];
                int rid = BusinessLogic.InsertRetrieval(e.EmployeeID);
                for (int i = 0; i < s.Split(',').Count(); i++)
                {
                    BusinessLogic.InsertRetrievalDetail(rid, s.Split(',')[i], Convert.ToInt32(sqty.Split(',')[i]));
                    BusinessLogic.UpdateInterimRetrievalStockItemForRetrieval(s.Split(',')[i], Convert.ToInt32(sqty.Split(',')[i]));
                }
            }
            //Retrieval from Retrieval
            string ri = Request.Form["rdid"];
            if (ri != null)
            {
                string qty = Request.Form["QtyCollected"];
                for (int i = 0; i < ri.Split(',').Count(); i++)
                {
                    //Update Retrieval Item
                    BusinessLogic.UpdateRetrievalItemCollected(Convert.ToInt32(ri.Split(',')[i]), Convert.ToInt32(qty.Split(',')[i]));
                    //Update Interim Stock Item
                    ViewBag.ritem = BusinessLogic.getRetrievalItem(Convert.ToInt32(ri.Split(',')[i]));
                    int diff = Convert.ToInt32(ViewBag.ritem.QtyNeeded) - Convert.ToInt32(qty.Split(',')[i]);
                    BusinessLogic.UpdateInterimRetrievalStockItemFromDisbursement(ViewBag.ritem.ItemCode, diff);
                    //Update Retrieval Form
                    BusinessLogic.UpdateRetrievalStatus(Convert.ToInt32(Request.Form["rid"]), "Retrieved");
                }
            }
            ViewBag.rplist = BusinessLogic.listRetrievalPending();
            return View();
        }


        public ActionResult StockRetrievalDetail()
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
            //list view of Retrieval Details
            int rid = 0; ;
            if (Request.Form["rid"] != null)
            {
                rid = Convert.ToInt32(Request.Form["rid"]);
            }
            else if (Request.QueryString["rid"] != null)
            {
                rid = Convert.ToInt32(Request.QueryString["rid"]);
            }
            ViewBag.rdd = BusinessLogic.getRetrievalByID(rid);
            ViewBag.rdlist = BusinessLogic.listRetrievalDetailByID(rid);
            RetrievalDetail r = new RetrievalDetail();
            ViewBag.ritem = r;
            //View Retrieval Item
            if (Request.QueryString["retDID"] != null)
            {
                int retDId = Convert.ToInt32(Request.QueryString["retDID"]);
                ViewBag.ritem = BusinessLogic.getRetrievalItem(retDId);
                ViewBag.CQty = Convert.ToInt32(Request.QueryString["qty"]);
                String itemcode = ViewBag.ritem.ItemCode;
            }
            //Update Retrieval Item
            if (Request.Form["Qty"] != null)
            {
                string itemcode = Request.Form["itemcode"];
                int retDId = Convert.ToInt32(Request.Form["retDID"]);
                int actQty = Convert.ToInt32(Request.Form["Qty"]);
                //Update Interim Stock Item
                int diff = 0;
                if (ViewBag.ritem.Status == "Collected")
                { diff = Convert.ToInt32(ViewBag.ritem.ActualQty) - actQty; }
                else if (ViewBag.ritem.Status != "Collected")
                { diff = Convert.ToInt32(ViewBag.ritem.QtyNeeded) - actQty; }
                BusinessLogic.UpdateInterimRetrievalStockItemFromDisbursement(itemcode, diff);
                //Update Adjustment Voucher
                string y = Request.Form["AdjQty"];
                if (y != "")
                {
                    int AdjQty = Convert.ToInt32(y);
                    if (AdjQty != 0)
                    {
                        String pom = Request.Form["pom"];
                        String reason = Request.Form["reason"];
                        BusinessLogic.InsertAdjustmentVoucher(itemcode, pom, AdjQty, reason, e.EmployeeID);
                    }
                }
                //Update Retrieval Item 
                BusinessLogic.UpdateRetrievalItemCollected(retDId, actQty);
            }
            return View();
        }
        public ActionResult StockRetrievalDisbursement()
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
            string s = "";
            //Display Retrieval info from main list
            if (Request.QueryString["rid"] != null)
            {
                s = Request.QueryString["rid"];
                BusinessLogic.UpdateRetrievalStatus(Convert.ToInt32(s), "Distributing");//lock retrieval list
            }
            //Information from Stock Retrieval Disbursement Detail
            else if (Request.Form["rid"] != null)
            {
                s = Request.Form["rid"];//retrieval Id
                string itemc = Request.Form["itemcode"];
                string rdid = Request.Form["rdid"];//RetrievalDetailId
                String dqty = Request.Form["disbursedQty"];  //disbursed qty for requisition item
                if (dqty != null)
                {
                    string reqdid = Request.Form["reqdid"];//get string of requisiton detailIDs
                    for (int i = 0; i < reqdid.Split(',').Count(); i++)//for every Requisition item
                    {
                        int reqDID = Convert.ToInt32(reqdid.Split(',')[i]);
                        int Dqty = Convert.ToInt32(dqty.Split(',')[i]);
                        ViewBag.reqitem = BusinessLogic.getRequisitionDetailItem(reqDID); //get the requisition item
                        if (Dqty != 0)
                        {
                            //Update Disbursement
                            ViewBag.Ditem = BusinessLogic.getDisbursementByDept(ViewBag.reqitem.Requisition.Employee1.DepartmentID); //find Disbursement form by Department ID (from requisition)
                            if (ViewBag.Ditem != null)//yes, there is Disbursement Form
                            {
                                ViewBag.DDitem = BusinessLogic.getDisbursementDetailItem(Convert.ToInt32(ViewBag.Ditem.DisbursementID), itemc);
                                if (ViewBag.DDitem != null)
                                {
                                    BusinessLogic.UpdateDisbursementDetailItem(Convert.ToInt32(ViewBag.DDitem.DisbursementDetailID), Dqty);
                                }
                                else if (ViewBag.DDitem == null)
                                {
                                    BusinessLogic.InsertDisbursementDetail(Convert.ToInt32(ViewBag.Ditem.DisbursementID), itemc, Dqty);
                                }
                            }
                            if (ViewBag.Ditem == null)//No, do not have Disbursment Form
                            {
                                BusinessLogic.InsertDisbursement(ViewBag.reqitem.Requisition.Employee1.DepartmentID, Convert.ToInt32(ViewBag.reqitem.RequisitionID), e.EmployeeID);
                                ViewBag.Ditem = BusinessLogic.getDisbursementByDept(ViewBag.reqitem.Requisition.Employee1.DepartmentID);
                                int did = Convert.ToInt32(ViewBag.Ditem.DisbursementID);
                                BusinessLogic.InsertDisbursementDetail(did, itemc, Dqty);
                            }
                            //Update Requisition item
                            BusinessLogic.UpdateRequisitionItem(reqDID, Dqty, Convert.ToInt32(ViewBag.Ditem.DisbursementID));
                            ViewBag.reqDlist = BusinessLogic.listRequisitionDetailByID(Convert.ToInt32(ViewBag.reqitem.RequisitionID));
                            //Update Requsition Status
                            foreach (var item in ViewBag.reqDlist)
                            {
                                if (item.OutstandingQty != 0)
                                {
                                    BusinessLogic.UpdateRequisitionStatus(Convert.ToInt32(item.RequisitionID), "Partially Fulfilled");
                                    break;
                                }
                                else if (item.OutstandingQty == 0)
                                { BusinessLogic.UpdateRequisitionStatus(Convert.ToInt32(item.RequisitionID), "Fulfilled"); }
                            }

                        }
                    }
                }
                BusinessLogic.UpdateRetrievalItemDisbursed(rdid); //update retrieval item
            }
            ViewBag.ritem = BusinessLogic.getRetrievalByID(Convert.ToInt32(s));
            ViewBag.rdlist = BusinessLogic.listRetrievalDetailByID(Convert.ToInt32(s));
            return View();
        }
        public ActionResult StockRetrievalDisbursementDetail()
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
            //Find by ItemCode
            string itemc = Request.QueryString["itemcode"]; // to find requisition form
            ViewBag.rdid = Request.QueryString["rdid"];// used to relate back to item list
            int rdid = Convert.ToInt32(Request.QueryString["rdid"]);
            ViewBag.ritem = BusinessLogic.getRetrievalItem(rdid);
            ViewBag.retQty = Convert.ToInt32(Request.QueryString["retQty"]);// view retrieval qty
            ViewBag.inlist = BusinessLogic.listRequisitionDetailByItemCode(itemc); //view list of related RequisitionItems
            ViewBag.rlist = BusinessLogic.listRequisitionApproved(); //to view Department ID

            return View();
        }

        public ActionResult StockRetrievalCompleted()
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
            if (Request.Form["rid"] != null)
            {
                string s = Request.Form["rid"];
                BusinessLogic.UpdateRetrievalStatus(Convert.ToInt32(s), "Completed");
            }
            ViewBag.rclist = BusinessLogic.listRetrievalComplete();

            return View();
        }
        public ActionResult StockRetrievalDetailCompleted(string id)
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
            int rid = Convert.ToInt32(id);
            ViewBag.rdd = BusinessLogic.getRetrievalByID(rid);
            ViewBag.rdlist = BusinessLogic.listRetrievalDetailByID(rid);

            return View();
        }

        public ActionResult RequisitionFormMain()
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
            //Initial View
            ViewBag.reqlist = BusinessLogic.listRequisitionApproved();

            //Generate Item View
            //--1.Get Requisition Detail for each Requisiton
            List<RequisitionDetail> rdlist = new List<RequisitionDetail>();
            //--2.Put them in combined list
            List<RequisitionDetail> rdlistall = new List<RequisitionDetail>();
            //--String of Req ID
            string s = Request.Form["reqID"];
            //--3.Create item list
            PrepForRetrieval w;
            List<PrepForRetrieval> listw = new List<PrepForRetrieval>();
            if (s != null)
            {
                int rid;
                foreach (var item in s.Split(','))
                {
                    rid = Convert.ToInt32(item);
                    rdlist = BusinessLogic.listRequisitionDetailByID(rid);
                    foreach (var i in rdlist) { rdlistall.Add(i); }
                }
                List<Catalogue> clist = BusinessLogic.listCatalogue();

                foreach (var item in clist)
                {
                    w = new PrepForRetrieval(item.ItemCode, item.Category.CategoryName, item.Description, 0);
                    listw.Add(w);
                }

                foreach (var item in rdlistall)
                {
                    foreach (var i in listw)
                        if (item.ItemCode == i.ItemCode)
                        {
                            i.Qty = i.Qty + (int)item.OutstandingQty;
                        }
                }
                List<InterimRetrievalStockTable> irslist = BusinessLogic.listInterimRetrievalStock();
                foreach (var item in irslist)
                {
                    foreach (var i in listw)
                        if (item.ItemCode == i.ItemCode)
                        {
                            i.Qty = Math.Min(i.Qty, Convert.ToInt32(item.WaitingQty));
                        }
                }
                //--Remove 0 items
                listw.RemoveAll(x => x.Qty == 0);

                ViewBag.cat = clist;
                ViewBag.relist = listw;
            }


            return View();
        }
        public ActionResult RequisitionDetail(string id)//view only
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
            int rid = Convert.ToInt32(id);
            ViewBag.rdd = BusinessLogic.getRequisitionByID(rid);
            ViewBag.reqDlist = BusinessLogic.listRequisitionDetailByID(rid);

            return View();
        }

    }
}