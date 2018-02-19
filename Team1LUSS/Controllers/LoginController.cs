using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;

namespace Team1LUSS.Controllers
{
    public class LoginController : Controller
    {
        // GET: Home
        //Reuben - login page. 
        public ActionResult Login()
        {
            string welcome = "Log in to start your session";
            ViewBag.Msg = welcome;
            //For username, we are using EmployeeID.
            string userid = Request.Form["username"];
            int id = Convert.ToInt32(userid);
            string pwd = Request.Form["password"];

            Employee emp = new BusinessLogic().GetEmployeeByID(id);

            if (emp != null && pwd == emp.Password)
            {
                //Set the session user to Employeee emp.
                Session["user"] = emp;

                //Set which page they will start at, depending on their role.
                if (emp.RoleID == 1 && emp.Authorization == null)
                {                    
                    return RedirectToAction("StoreCatalogue", "Inventory");
                }
                if (emp.RoleID == 1 && emp.Authorization == "Representative") //employee that is the Rep.
                {                    
                    return RedirectToAction("StoreCatalogue", "Inventory"); //Page to be changed once everything is integrated.
                }
                if (emp.RoleID == 1 && emp.Authorization == "Authorized")//employe with authority
                {                    
                    return RedirectToAction("StoreCatalogue", "Inventory"); //Page to be changed once everything is integrated.
                }
                if (emp.RoleID == 2)//dept head
                {
                    return RedirectToAction("StoreCatalogue", "Inventory"); //This is the correct starting page for Dept Head.
                }
                else //store side
                {
                    return RedirectToAction("StoreCatalogue", "Inventory"); //Page to be changed once everything is integrated.
                }
            }
            if (emp == null && pwd != null)
            {
                string error = "Login failed. Employee ID supplied doesn't exist. Try Again";
                ViewBag.Msg = error;
            }
            if (emp != null && pwd != emp.Password)
            {
                string error = "Login failed. Password is incorrect. Try Again";
                ViewBag.Msg = error;
            }
            return View();
        }
        //Reuben - Lougout page. Clears session and redirects to login
        public ActionResult Logout()
        {
            Session.Clear();
            System.Web.HttpContext.Current.Session.RemoveAll();
            return RedirectToAction("Login");
        }
        //Reuben - Error page if someone tries to access page for different role
        public ActionResult NotAuthorized()
        {
            return View();
        }
        
    }
}