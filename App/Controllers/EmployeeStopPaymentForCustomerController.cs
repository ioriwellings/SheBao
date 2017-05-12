using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentForCustomerController : Controller
    {
        //
        // GET: /EmployeeStopPaymentForCustomer/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Update(string id, string state,string backUrl)
        {
            ViewBag.Id = id;
            ViewBag.state = state;
            if (string.IsNullOrEmpty(backUrl))
            {
                backUrl = "EmployeeStopPaymentForCustomer";
            }
            ViewBag.BackUrl = backUrl;

            return View();
        }
        public ActionResult Edit(string id, string state)
        {
            ViewBag.Id = id;
            ViewBag.state = state;

            return View();
        }

        public ActionResult SetStop(string stopIds, string stopName, string empName)
        {
            ViewBag.EmpName = empName;
            string[] arrIDs = stopIds.Split(',');
            string[] arrName = stopName.Split(',');
            string html = string.Empty;
            for (int i = 0; i < arrIDs.Count(); i++)
            {
                html += string.Format("<input type='checkbox' id='cbox_{0}'/>{1}", arrIDs[i], arrName[i]);
            }
            ViewBag.strHtml = html;
            return View();
        }
    }
}