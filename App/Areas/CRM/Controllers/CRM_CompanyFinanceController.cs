using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using Langben.App.Models;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业财务信息
    /// </summary>
    public class CRM_CompanyFinanceController : BaseController
    {

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index(string id, string path)
        {
            ViewBag.Path = path;
            ViewBag.Company_ID = id;
            return View();
        }
 
 
        /// <summary>
        /// 创建_开票名称
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CreateBill(string id)
        {
            ViewBag.CompanyID = id;
            return View();
        }

        /// <summary>
        /// 编辑_开票名称
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult EditBill(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 创建_付款名称
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CreatePayment(string id)
        {
            ViewBag.CompanyID = id;
            return View();
        }

        /// <summary>
        /// 编辑_付款名称
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult EditPayment(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}


