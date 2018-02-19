using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class UsageChargeBackController : Controller
    {
        // GET: UsageChargeBack
        //christine
        public ActionResult UsageChargeBack()
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
            string did = Request.Form["depselect"];
            string t = Request.Form["monselect"];
            List<Disbursement> lcom = new List<Disbursement>();
            List<DisbursementDetail> ldb = new List<DisbursementDetail>();
            List<DisbursementDetail> ldball = new List<DisbursementDetail>();
            if (did == null)
            {
                if (t == null)
                {
                    lcom = BusinessLogic.GetDisbursmentCompleted();
                }
                else
                {

                    lcom = BusinessLogic.GetDisbursmentCompletedBymonth(t);
                }
            }
            else
            {
                if (t == null)
                {
                    lcom = BusinessLogic.GetDisbursmentCompletedByDepartment(did);
                }
                else
                {
                    lcom = BusinessLogic.GetDisbursmentCompletedByDepartmentBymonth(did, t);
                }
            }
            foreach (Disbursement item in lcom)
            {
                ldb = BusinessLogic.GetDisbursmentDetailCompleted(item.DisbursementID);
                foreach (DisbursementDetail ditem in ldb)
                {
                    ldball.Add(ditem);
                }
            }
            ViewBag.dblist = ldball;
            ViewBag.dlist = BusinessLogic.getDepartment();
            ViewBag.deptid = did;                       

            return View();
        }

        //Irene
        public ActionResult Notification()
        {
            if (Request.Form["send"] != null)
            {
                String dID = Request.Form["DisbursementID"];
                String departmentID = Request.Form["deptID"];
                String disbursementID;

                //For email
                Employee e = BusinessLogic.GetDeptHeadByDeptID(departmentID);
                List<Disbursement> d = BusinessLogic.GetDisbursementListByDept(e.DepartmentID);

                Decimal amount = 0;

                for (int i = 0; i < d.Count; i++)
                {
                    amount = amount + (Decimal)d[i].Amount;
                }
                Email email;
                String emailaddress;
                string emailname;
                String bodytext;
                String subject = "Usage Charge-Back";

                emailaddress = e.EmailAdd;
                emailname = e.EmployeeName;
                bodytext = "Dear " + emailname + ",<br /><br />" + "Your account was charged $" + (string.Format("{0:C}", amount)) + " today. This is for stationery usage for this month.<br /><br /><br /><br />================================================================<br /><br />This is a system generated email, Please do not reply. <br /><br />Store Administrator";
                email = new Email(emailaddress, subject, bodytext);
                BusinessLogic.SendEmail(email);

                //For updating status from "Completed" to "Charge Back Completed"
                for (int i = 0; i < dID.Split(',').Count(); i++)
                {
                    disbursementID = dID.Split(',')[i];
                    BusinessLogic.UpdateStatus(Convert.ToInt32(disbursementID));
                }                
            }
            Response.Redirect("/UsageChargeBack/UsageChargeBack/");
            return View();
        }
    }

}