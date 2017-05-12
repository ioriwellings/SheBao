using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierBillController : BaseController
    {
        public ActionResult Index(string id,string path)
        {
            ViewBag.Path = path;
            ViewBag.SupplierID = id;
            return View();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(string id)
        {
            ViewBag.SupplierID = id;
            return View();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            ViewBag.BillID = id;
            return View();
        }
    }
}
