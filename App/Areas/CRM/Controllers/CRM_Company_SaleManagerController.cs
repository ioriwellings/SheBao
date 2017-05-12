using Common;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_Company_SaleManagerController : BaseController
    {
        private string menuID = "";
        //
        // GET: /CRM/CRM_Company_XSManager/
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
            return View();
        }

        /// <summary>
        /// 销售分配
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleAssigned(string id)
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
            ViewBag.ID = id;
            return View();
        }

	}
}