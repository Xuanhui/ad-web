using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class AuthorityController : Controller
    {
        // GET: Authority
        //Luowei
        public ActionResult DelegateAuthority()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            
            //logic
            Employee cuser = BusinessLogic.getCurrentUserByUserName(e.EmployeeName);
   
            String did = cuser.Department.DepartmentID;
            ViewBag.elist = BusinessLogic.getEmployeelistByDep(did);
            String s = Request.Form["auser"];
            if (s != null)
            {
                BusinessLogic.setActingNullByID(Convert.ToInt32(s));
                BusinessLogic.setHeadBackByID(cuser.EmployeeID);
                cuser = BusinessLogic.getCurrentUserByUserName(e.EmployeeName);
                Session["user"] = cuser;
            }
            ViewBag.actingHead = BusinessLogic.getActingHead(did);
            ViewBag.user = e;
            return View();
        }
        //Luowei
        public ActionResult RevokeAuthority()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            
            Employee cuser = BusinessLogic.getCurrentUserByUserName(e.EmployeeName); //replace this code with the logged in username
            Session["cuser"] = cuser;
            String s = Request.Form["SelectedActingHead"];
            if (s != null)
            {
                DateTime startdate = Convert.ToDateTime(Request.Form["startdate"]);
                DateTime enddate = Convert.ToDateTime(Request.Form["enddate"]);
                BusinessLogic.setActingHeadByID(Convert.ToInt32(s), startdate, enddate);
                BusinessLogic.setActingNullByID(cuser.EmployeeID);
                cuser = BusinessLogic.getCurrentUserByUserName(e.EmployeeName);
                Session["user"] = cuser;
            }
            String did = cuser.Department.DepartmentID;
            ViewBag.elist = BusinessLogic.getEmployeelistByDep(did);
            ViewBag.auser = BusinessLogic.getActingHead(did);
            ViewBag.user = e;
            return View();
        }

        //Claire
        public ActionResult AuthorityRep()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            Employee cuser = BusinessLogic.getCurrentUserByUserName(e.EmployeeName);
            Session["cuser"] = cuser;
            string did = cuser.Department.DepartmentID;
            ViewBag.elist = BusinessLogic.getEmployeelistByDep(did);
            ViewBag.currentRep = cuser.Department.RepresentativeName;
            return View();
        }
        public ActionResult ChangeAuthorityRep()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID != 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE

            ViewBag.user = e;
            string selectedemployeeid = Request.Form["selectedRep"];
            string previousRep = Request.Form["previous"];
            int selectedid = Convert.ToInt16(selectedemployeeid);
            BusinessLogic.UpdatePreviousRep(previousRep);
            BusinessLogic.UpdateWantToChangeRep(selectedid);
            BusinessLogic.UpdateAutorityRep(selectedid);
            Response.Redirect("/Authority/AuthorityRep/");
            return View();
        }

    }
}