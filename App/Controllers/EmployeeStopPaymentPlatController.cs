using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentPlatController : Controller
    {
        //
        // GET: /EmployeeStopPaymentPlat/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Update(string id, string state)
        {
            ViewBag.Id = id;
            ViewBag.state = state;

            return View();
        }
	}
}