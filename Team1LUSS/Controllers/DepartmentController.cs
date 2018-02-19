using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        //christine
        public ActionResult DepartmentList()
        {
            //EVERYPAGE
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            Employee e = (Employee)Session["user"];
            if (e.RoleID == 1 || e.RoleID == 2)
            {
                return RedirectToAction("NotAuthorized");
            }
            //EVERYPAGE
            ViewBag.user = e;
            ViewBag.dlist = BusinessLogic.showDepartmentList();
            return View();
        }
    }
}