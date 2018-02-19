using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team1LUSS.Models;
using System.Web.Helpers;

namespace Team1LUSS.Controllers
{
    public class ReportController : Controller
    {
        //Done By Arthur
        public ActionResult ReportByDep()
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

            return View();
        }
        public ActionResult ViewReport()
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
            string departmentID = Request.Form["department"];
            string category = Request.Form["category"];
            string year = Request.Form["year"];
            string month = Request.Form["month"];

            ViewBag.departmentID = Request.Form["department"];
            ViewBag.category = Request.Form["category"];
            ViewBag.year = Request.Form["year"];
            ViewBag.month = Request.Form["month"];

            //List<ArrayList> amountTotal = new List<ArrayList>();
            List<ObjectForBar> amountTotal = new List<Models.ObjectForBar>();
            if (departmentID != null && category != null && year != null && month != null)
            {
                amountTotal = BusinessLogic.GenerateRepByDep(departmentID, category, year, month);
            }

            List<string> months = new List<string>();
            List<string> categories = new List<string>();
            List<string> depatments = new List<string>();

            if (departmentID != null && category != null && year != null && month != null)
            {
                foreach (var item in month.Split(','))
                {
                    months.Add(item);
                }
                foreach (var item in category.Split(','))
                {
                    string itemName = BusinessLogic.getItemName(item);
                    categories.Add(itemName);
                }
                foreach (var item in departmentID.Split(','))
                {
                    depatments.Add(item);
                }
            }

            Chart c = new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla);
            c.AddTitle("Category Demand By Department");
            c.AddLegend();
            c.SetXAxis("Month");
            c.SetYAxis("Amount");
            for (int i = 0; i < amountTotal.Count; i++)
            {
                c.AddSeries(name: amountTotal[i].CategoryName + i, chartType: "column", xValue: months, yValues: amountTotal[i].Arr);
            }
            c.Write("bmp");
            return View();
        }

        public ActionResult TableView()
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
            return View();
        }
        public ActionResult TableShow()
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
            string category = Request.Form["category"];
            List<string> categorNames = new List<string>();
            if (category != null)
            {
                ViewBag.ObjectShow = BusinessLogic.creatTable(category);
            }
            return View();
        }
    }
}