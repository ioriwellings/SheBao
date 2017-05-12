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
    public class CRM_EmployeeBankController : BaseController
    {
        #region 银行信息

        public ActionResult Index(int id)
        {
            //得到登陆者是员工客服还是责任客服,经理级别的不可以修改
            List<CRM_CompanyToBranch> branchlist = new List<CRM_CompanyToBranch>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();
            IBLL.ICRM_CompanyToBranchBLL branchbll = new BLL.CRM_CompanyToBranchBLL();
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 首次创建银行信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.eId = id;
            return View();
        }

        /// <summary>
        /// 首次编辑银行信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult Edit()
        {
            ViewBag.Id = Request.QueryString["id"];
            ViewBag.eId = Request.QueryString["eid"];
            return View();
        }

        #endregion
    }
}
