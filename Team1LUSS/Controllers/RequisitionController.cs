using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class RequisitionController : Controller
    {
        // GET: Requisition
        //Reuben
        public ActionResult DeptHeadReqMain()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2 && e.Authorization != "Authorized")
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            string c = Request.Form["description"];
            string s = Request.Form["approval"];
            string id = Request.Form["rid"];
            int rid = Convert.ToInt32(id);
            int aid = e.EmployeeID;
            if (s == "Approved")
            {
                //BusinessLogic.UpdateRequsitionByID(rid, aid, DateTime.Now, s, c);
                using (TransactionScope ts = new TransactionScope())
                {
                    BusinessLogic.UpdateRequsitionByID(rid, aid, DateTime.Now, s, c);
                    BusinessLogic.UpdateInterimQtyFromReq(rid);

                    //For Email notification
                    Requisition r = BusinessLogic.getRequisitionByID(rid);
                    Employee emp = BusinessLogic.getEmployeeByID((int)r.EmployeeID);
                    Email email;
                    String emailaddress;
                    string emailname;
                    String bodytext;
                    String subject = "Requisition Approval Status";

                    emailaddress = emp.EmailAdd;
                    emailname = emp.EmployeeName;
                    bodytext = "Dear " + emailname + ",<br /><br />" + "Your requisition has been approved by " + e.EmployeeName + ".<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                    email = new Email(emailaddress, subject, bodytext);
                    BusinessLogic.SendEmail(email);

                    ts.Complete();
                }
            }
            if (s == "Rejected")
            {
                BusinessLogic.UpdateRequsitionByID(rid, aid, DateTime.Now, s, c);

                //For Email notification
                Requisition r = BusinessLogic.getRequisitionByID(rid);
                Employee emp = BusinessLogic.getEmployeeByID((int)r.EmployeeID);
                Email email;
                String emailaddress;
                string emailname;
                String bodytext;
                String subject = "Requisition Approval Status";

                emailaddress = emp.EmailAdd;
                emailname = emp.EmployeeName;
                bodytext = "Dear " + emailname + ",<br /><br />" + "Your requisition has been rejected by " + e.EmployeeName + ".<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                email = new Email(emailaddress, subject, bodytext);
                BusinessLogic.SendEmail(email);
            }
            ViewBag.PendingList = new BusinessLogic().GetPendingReqs(e.DepartmentID);
            ViewBag.ApprovedList = new BusinessLogic().GetAllOtherReqs(e.DepartmentID);
            return View();
        }
        //Reuben
        public ActionResult DeptHeadReqPending(int id)
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2 && e.Authorization != "Authorized")
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            ViewBag.ReqDetails = new BusinessLogic().GetRequisitionDetails(id);
            ViewBag.ReqID = id;
            return View();
        }
        //Reuben
        public ActionResult DeptHeadReqDetails(int id)
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2 && e.Authorization != "Authorized")
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            ViewBag.ReqDetails = new BusinessLogic().GetRequisitionDetails(id);
            return View();
        }

        //Sandor
        public ActionResult EmployeeReq()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 1)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            //list requisition
            String s = Request.Form["name1"];

            List<int> ridlist = new List<int>();

            if (s != null)
            {
                foreach (var item in s.Split(','))
                {
                    int id = Convert.ToInt32(item);
                    BusinessLogic.deleteReqListDetail(id);
                    BusinessLogic.deleteReqList(id);
                }
            }
            //End list

            //Create requisition
            String i = Request.Form["Item"];

            if (i != null)
            {
                String[] itemarr = i.Split(',');

                BusinessLogic.createRequisition(e.EmployeeID);

                int reqID = BusinessLogic.getBottomRequisitionID();


                for (int j = 0; j < itemarr.Length; j++)
                {
                    String qty = Request.Form[itemarr[j]];
                    int Qty = Convert.ToInt16(qty);

                    BusinessLogic.createRequisitionDetail(reqID, itemarr[j], Qty);

                }

                //For Email notification (Send to Head of Dept)
                Employee emp = BusinessLogic.GetDeptHeadByDeptID(e.DepartmentID);
                Email email;
                String emailaddress;
                string emailname;
                String bodytext;
                String subject = "Requisition for Approval";

                emailaddress = emp.EmailAdd;
                emailname = emp.EmployeeName;
                bodytext = "Dear " + emailname + ",<br /><br />" + "There is a pending request waiting for your approval.<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                email = new Email(emailaddress, subject, bodytext);
                BusinessLogic.SendEmail(email);

                //For Email notification (send to employee)                                
                subject = "Status of Requisition";

                emailaddress = e.EmailAdd;
                emailname = e.EmployeeName;
                bodytext = "Dear " + emailname + ",<br /><br />" + "Your request has been submitted and is awating approval.<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                email = new Email(emailaddress, subject, bodytext);
                BusinessLogic.SendEmail(email);
            }
            //End creation

            //Update Begin
            String reqID2 = Request.Form["reqID"];
            int ReqID = Convert.ToInt16(reqID2);
            String itemcode = Request.Form["itemcode"];
            String qty2 = Request.Form["qty"];


            if (itemcode != null)
            {
                String[] ItemCode = itemcode.Split(',');
                String[] Qty2 = qty2.Split(',');

                for (int k = 0; k < ItemCode.Length; k++)
                {
                    int Quantity = Convert.ToInt16(Qty2[k]);

                    BusinessLogic.updateReqDetail(ReqID, ItemCode[k], Quantity);
                }

                //For Email notification (send to dept head)
                Employee emp = BusinessLogic.GetDeptHeadByDeptID(e.DepartmentID);
                Email email;
                String emailaddress;
                string emailname;
                String bodytext;
                String subject = "Updated Requisition for Approval";

                emailaddress = emp.EmailAdd;
                emailname = emp.EmployeeName;
                bodytext = "Dear " + emailname + ",<br /><br />" + "There is a pending updated request waiting for your approval.<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                email = new Email(emailaddress, subject, bodytext);
                BusinessLogic.SendEmail(email);

                //For Email notification (send to employee)                                
                subject = "Status of Updated Requisition";

                emailaddress = e.EmailAdd;
                emailname = e.EmployeeName;
                bodytext = "Dear " + emailname + ",<br /><br />" + "Your updated request has been submitted and is awating approval.<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                email = new Email(emailaddress, subject, bodytext);
                BusinessLogic.SendEmail(email);
            }
            //End Updating
            ViewBag.rlist = BusinessLogic.getRequisitionByEmployeeID(e.EmployeeID);
            return View();
        }

        //Sandor
        public ActionResult EmployeeReqDetail(string id)
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 1)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            int rid = Convert.ToInt32(id);
            ViewBag.rlist = BusinessLogic.getReqDetailByID(rid);
            ViewBag.r = BusinessLogic.getRequisitionByID(rid);
            return View();
        }

        //Sandor
        public ActionResult EmployeeReqCrt()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 1)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            //Add item begin

            //String s = Request.Params["Catalogue"];
            String s = Request.Form["Catalogue"];
            String already = (String)Request.Form["addalready"];
            //already.GetType();

            List<Catalogue> slist = new List<Catalogue>();
            List<Catalogue> mlist = new List<Catalogue>();
            List<Catalogue> alist = BusinessLogic.listAllCatelogue();


            if (already == "" || already == null)
            {
                if (s != null)
                {
                    //string already = Request.Form["addalready"] + "," + s;

                    foreach (var itemcode in s.Split(','))
                    {
                        slist.Add(BusinessLogic.listSelectedItem(itemcode));
                        mlist = alist;
                        var itemToRemove = mlist.SingleOrDefault(r => r.ItemCode == itemcode);
                        mlist.Remove(itemToRemove);

                    }
                }
                ViewBag.s = s + already;
            }
            else
            {
                ViewBag.s = s + "," + already;
                already = ViewBag.s;
                foreach (var itemcode in already.Split(','))
                {
                    slist.Add(BusinessLogic.listSelectedItem(itemcode));
                    mlist = alist;
                    var itemToRemove = mlist.SingleOrDefault(r => r.ItemCode == itemcode);
                    mlist.Remove(itemToRemove);

                }
            }
            //String x = Request.Form["addalready"];
            //List<Catalogue> xlist = x.ToList<Catalogue>();

            ViewBag.slist = slist;
            ViewBag.alist = alist;
            ViewBag.mlist = mlist;

            //add item end


            return View();
        }

        //Sandor
        public ActionResult EmployeeReqEdit(int id)
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 1)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            List<RequisitionDetail> reqdl = BusinessLogic.getReqDetailByID(id);
            ViewBag.reqdl = reqdl;
            ViewBag.reqID = id;
            return View();

        }

        //Irene
        public ActionResult ClerkReq()
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
            
            ViewBag.AllApprovedReqList = new BusinessLogic().GetAllApprovedReqs();
            return View();
        }
        //Irene
        public ActionResult ClerkReqDetails(int id)
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
            ViewBag.ReqDetails = BusinessLogic.getReqDetailByID(id);
            return View();
        }

        //Reuben- Representative Requisition Page
        public ActionResult DeptRepReqMain()
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
            ViewBag.PendingList = new BusinessLogic().GetPendingReqs(e.DepartmentID);
            ViewBag.ApprovedList = new BusinessLogic().GetAllOtherReqs(e.DepartmentID);
            return View();
        }

        //Reuben - Requisition Detail Page
        public ActionResult DeptRepReqDetails(int id)
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
            ViewBag.ReqDetails = new BusinessLogic().GetRequisitionDetails(id);
            return View();
        }
        public ActionResult DeptRepReqPending(int id)
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
            ViewBag.ReqDetails = new BusinessLogic().GetRequisitionDetails(id);
            return View();
        }
    }
}