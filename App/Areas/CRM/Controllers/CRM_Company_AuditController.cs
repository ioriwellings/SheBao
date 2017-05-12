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
    /// 客户_企业信息
    /// </summary>
    public class CRM_Company_AuditController : BaseController
    {
        private string menuID = "1002";

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();

            List<ORG_User> UserListXS = new List<ORG_User>();

            UserListXS = bll.GetGroupUsers(Common.ORG_Group_Code.XS.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsersXS = new SelectList(UserListXS, "ID", "RName", null);
            ViewBag.listUserXS = selUsersXS;

            List<ORG_User> UserListKF = new List<ORG_User>();

            UserListKF = bll.GetGroupUsers(Common.ORG_Group_Code.ZRKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsersKF = new SelectList(UserListKF, "ID", "RName", null);
            ViewBag.listUserKF = selUsersKF;

            return View();
        }

        /// <summary>
        /// 基本信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditBasic(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加联系人信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddLinkMan(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改联系人信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditLinkMan(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加银行信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddBank(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改银行信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditBank(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加财务信息（开票）
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddFinanceBill(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改财务信息对比（开票）
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditFinanceBill(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加财务信息(付款)
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddFinancePayment(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改财务信息对比(付款)
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditFinancePayment(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加报价信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddPrice(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改报价信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditPrice(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加阶梯报价信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddLadderPrice(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改阶梯报价信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditLadderPrice(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加合同信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddContract(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改合同信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditContract(string id, string mainTableId, int state)
        {
            ViewBag.ID = id;
            ViewBag.MainTableID = mainTableId;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 添加社保信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AddInsurance(int id, string cityID, int state)
        {
            ViewBag.CityID = cityID;
            ViewBag.CompanyID = id;
            ViewBag.State = state;
            return View();
        }

        /// <summary>
        /// 修改社保信息对比
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditInsurance(int id, string cityID, int state)
        {
            ViewBag.CityID = cityID;
            ViewBag.CompanyID = id;
            ViewBag.State = state;
            return View();
        }
    }
}


