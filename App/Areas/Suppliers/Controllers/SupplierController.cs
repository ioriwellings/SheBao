using Common;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierController : BaseController
    {
        private string menuID = "";
        //
        // GET: /Suppliers/Supplier/
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

            UserList = bll.GetGroupUsers(Common.ORG_Group_Code.GYSKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsers = new SelectList(UserList, "ID", "RName", null);
            ViewBag.listUser = selUsers;
            ViewBag.UserID = LoginInfo.UserID;

            List<ORG_GroupUser> list = bll.GetGroupAuthority("GYSKFJL", LoginInfo.UserID);
            int users = 0;
            if (list.Count > 0)
            {
                users = 1;
            }
            ViewBag.users = users;
            return View();
        }
        //创建
        public ActionResult Create()
        {
            return View();
        }
        //修改
        public ActionResult Edit(string id)
        {
            ViewBag.Id = id;
            List<City> listCity = new List<City>();//城市
            ViewBag.City = new SelectList(listCity, "Id", "Name", null);
            return View();
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Detail(string id)
        {
            ViewBag.Supplier_ID = id;
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 供应商客服分配
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceAssigned(string id)
        {
            ViewBag.ID = id;
            return View();
        }

        #region
        /// <summary>
        /// 获取当前权限客服人员
        /// </summary>
        /// <returns></returns>
        public ActionResult kfry()
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

            UserList = bll.GetGroupUsers(Common.ORG_Group_Code.GYSKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(UserList);
            return Content(json);
        }
        #endregion

        
	}
}