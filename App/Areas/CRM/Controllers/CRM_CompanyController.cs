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
using System.Web.UI;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业信息
    /// </summary>
    public class CRM_CompanyController : BaseController
    {
        private string menuID = "10";

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            List<ORG_User> UserList = new List<ORG_User>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();

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

            UserList = bll.GetGroupUsers(Common.ORG_Group_Code.XS.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsers = new SelectList(UserList, "ID", "RName", null);
            ViewBag.listUser = selUsers;
            ViewBag.UserID = LoginInfo.UserID;
            return View();
        }
        //创建公司
        [SupportFilter]
        public ActionResult Create()
        {
            List<CRM_ZD_HY> listHY = GetHY();//大行业
            SelectList select = new SelectList(listHY, "code", "hymc", null);
            ViewBag.HY = select;
            List<CRM_ZD_HY> listSon = new List<CRM_ZD_HY>();//子行业
            ViewBag.HYSon = new SelectList(listSon, "code", "hymc", null);
            List<PRD_Product> listPrd = GetPrd();
            SelectList selectPRD = new SelectList(listPrd, "ID", "ProductName", null);
            ViewBag.listPrd = selectPRD;
            return View();
        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CompanyDetail(string id)
        {
            ViewBag.Company_ID = id;
            ViewBag.Id = id;
            List<CRM_ZD_HY> listHY = GetHY();
            SelectList select = new SelectList(listHY, "code", "hymc", null);
            ViewBag.HY = select;
            return View();
        }
        /// <summary>
        /// 首次编辑基本信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult ModifyBaseInfo(string id)
        {
            ViewBag.Id = id;
            List<CRM_ZD_HY> listHY = GetHY();
            SelectList select = new SelectList(listHY, "code", "hymc", null);
            ViewBag.HY = select;
            List<CRM_ZD_HY> listSon = new List<CRM_ZD_HY>();//子行业
            ViewBag.HYSon = new SelectList(listSon, "code", "hymc", null);
            return View();
        }
        //验证公司名唯一性
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]   //清除缓存 
        public JsonResult CheckCompanyName(string CompanyName)
        {
            IBLL.ICRM_CompanyBLL c_BLL = new BLL.CRM_CompanyBLL();
            bool valid = false;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("CompanyNameDDL_String&" + CompanyName);
            List<CRM_Company> list = c_BLL.GetByParam(null, "Desc", "CompanyName", strSql.ToString());
            if (list.Count == 0)
                valid = true;
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 财务信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult FinanceList(string id)
        {
            ViewBag.CRM_Company_ID = id;
            return View();
        }
        /// <summary>
        /// 添加财务信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult AddFinance(string id)
        {
            ViewBag.CompanyID = id;
            return View();
        }
        /// <summary>
        /// 编辑财务信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult ModifyFinance(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        #region 内置
        //得到行业
        public List<CRM_ZD_HY> GetHY()
        {
            List<CRM_ZD_HY> listHY = new List<CRM_ZD_HY>();
            IBLL.ICRM_ZD_HYBLL hy_BLL = new BLL.CRM_ZD_HYBLL();
            string sqlWhere = "NodeLevelDDL_Int&1^XYBZDDL_String&Y";
            listHY = hy_BLL.GetByParam(null, "asc", "code", sqlWhere);
            return listHY;
        }
        //得到产品
        public List<PRD_Product> GetPrd()
        {
            List<PRD_Product> listPrd = new List<PRD_Product>();
            IBLL.IPRD_ProductBLL hy_BLL = new BLL.PRD_ProductBLL();

            listPrd = hy_BLL.GetAll();
            return listPrd;
        }
        #endregion
    }
}


