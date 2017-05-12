using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;
using Models;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentSingleController : BaseController
    {
        // GET: EmployeeStopPaymentSingle
        public ActionResult Index()
        {
            return View();
        }
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}