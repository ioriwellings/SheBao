using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierStopFeedBackController : BaseController
    {
        //
        // GET: /Suppliers/SupplierStop/
        public ActionResult Index()
        {
            IBLL.ISupplierBLL BLL = new BLL.SupplierBLL();

            var SupplierList = BLL.GetByParam("", "ASC", "Id", "CustomerServiceIdDDL_Int&" + LoginInfo.UserID);
            SelectList selSuppliers = new SelectList(SupplierList, "Id", "Name", null);
            ViewBag.listSupplier = selSuppliers;

            return View();
        }

        #region 报减成功
        /// <summary>
        /// 报减成功页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SupFeedbackSuccess(string stopIds)
        {
            ViewBag.stopIds = stopIds;
            return View();
        }

        #endregion

        /// <summary>
        /// 设置报减失败页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SetFail(string stopIds)
        {
            ViewBag.stopIds = stopIds;
            return View();
        }

    }
}