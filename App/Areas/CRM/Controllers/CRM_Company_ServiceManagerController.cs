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
    public class CRM_Company_ServiceManagerController : BaseController
    {
        private string menuID = "1057";
        private string OmenuID = "1058";
        //
        // GET: /CRM/CRM_ZRManagerCompany/
        /// <summary>
        /// 客服修改所管的企业的信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<ORG_User> UserList = new List<ORG_User>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();
            ViewBag.UserID = LoginInfo.UserID;
            #region 获取权限配置
            string menuID = "1003";
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }

            #endregion
            //UserList = bll.GetUsers(departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            UserList = bll.GetGroupUsers(Common.ORG_Group_Code.ZRKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsers = new SelectList(UserList, "ID", "RName", null);
            ViewBag.listUser = selUsers;

            return View();
        }


        /// <summary>
        /// 已服务的企业
        /// </summary>
        /// <returns></returns>
        public ActionResult AlreadyIndex()
        {
            List<ORG_User> UserList = new List<ORG_User>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();
            ViewBag.menuid = OmenuID;
            #region 获取权限配置
            //部门范围权限
            string menuID = "1005";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            UserList = bll.GetUsers(departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsers = new SelectList(UserList, "ID", "RName", null);
            ViewBag.listUser = selUsers;
            return View();
        }

        /// <summary>
        /// 客服分配新企业列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceIndex()
        {
            ViewBag.menuid = menuID;
            return View();
        }

        /// <summary>
        /// 客服分配
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceAssigned(string id, string menuid)
        {
            List<ORG_User> UserList = new List<ORG_User>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();

            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuid);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            //UserList = bll.GetUsers(departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            UserList = bll.GetGroupUsers(Common.ORG_Group_Code.ZRKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);

            SelectList selUsers = new SelectList(UserList, "ID", "RName", null);
            ViewBag.listUser = selUsers;
            ViewBag.ID = id;
            return View();
        }
	}
}