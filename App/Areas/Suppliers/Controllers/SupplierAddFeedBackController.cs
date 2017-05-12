using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierAddFeedBackController : BaseController
    {

        #region Get Action
       /// <summary>
       /// 首页
       /// </summary>
       /// <returns></returns>
        public ActionResult Index()
        {
            IBLL.ISupplierBLL BLL = new BLL.SupplierBLL();

            var SupplierList = BLL.GetByParam("", "ASC", "Id", "CustomerServiceIdDDL_Int&" + LoginInfo.UserID);
            SelectList selSuppliers = new SelectList(SupplierList, "Id", "Name", null);
            ViewBag.listSupplier = selSuppliers;
            return View();
        }

        /// <summary>
        /// 供应商客服审核成功界面
        /// </summary>
        /// <returns></returns>

        public ActionResult SupFeedbackSuccess(string ids, string Cityid, string CompanyEmployeeRelationId, string alltype, string countnn)
        {
            ViewBag.ids = ids;
            ViewBag.alltype = HttpUtility.HtmlDecode(alltype);
            ViewBag.Cityid = Cityid;
            ViewBag.CompanyEmployeeRelationId = CompanyEmployeeRelationId;
            ViewBag.flag = countnn;
            ViewBag.Cityid = Cityid;
            return View();
        }

        /// <summary>
        /// 供应商客服审核失败页面
        /// </summary>
        /// <param name="ids">社保id</param>
        /// <param name="Cityid">城市编码</param>
        /// <param name="CompanyEmployeeRelationId">员工关系</param>
        /// <param name="alltype">险种</param>
   
        /// <returns></returns>
        [SupportFilter]
        public ActionResult FeedbackIndex(string ids, string Cityid, string CompanyEmployeeRelationId, string alltype)
        {

            if (!string.IsNullOrEmpty(ids))
            {
                ViewBag.ids = ids.TrimEnd(',').ToString();
            }

            ViewBag.Cityid = Cityid;
            ViewBag.CompanyEmployeeRelationId = CompanyEmployeeRelationId;
            ViewBag.alltype = HttpUtility.HtmlDecode(alltype);
       


            return View();

        }
        #endregion

        #region 报增失败页面
      
        #endregion
    }
}