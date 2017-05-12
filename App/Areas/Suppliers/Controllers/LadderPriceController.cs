using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class LadderPriceController : BaseController
    {
        public ActionResult Index(int id,string path)
        {
            ViewBag.Path = path;
            ViewBag.SupplierID = id;

            return View();
        }

        /// <summary>
        /// 创建最低报价
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(string id)
        {
            ViewBag.SupplierID = id;
            return View();
        }

        /// <summary>
        /// 修改最低报价
        /// </summary>
        /// <returns></returns>
        public ActionResult EditLowest(string id)
        {
            ViewBag.ID = id;
            return View();
        }

        /// <summary>
        /// 创建阶梯价格
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateLadder(string id)
        {
            ViewBag.SupplierID = id;
            return View();
        }

        /// <summary>
        /// 修改阶梯价格
        /// </summary>
        /// <returns></returns>
        public ActionResult EditLadder(string id)
        {
            ViewBag.ID = id;
            return View();
        }

    }
}
