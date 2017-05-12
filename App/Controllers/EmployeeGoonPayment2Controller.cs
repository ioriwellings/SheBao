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

namespace Langben.App.Controllers
{
    /// <summary>
    /// 员工调基
    /// </summary>
    public class EmployeeGoonPayment2Controller : BaseController
    {
        string menuID = "1037";//员工客服确认调基信息菜单ID
        string AddButton = "1034-1";//责任客服添加调基按钮权限码
        string PassButton = "1037-1";//员工客服确认按钮权限码
        string EndButton = "1037-2";//员工客服终止按钮权限码

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            #region 权限验证
            ViewBag.AddButton = this.MenuOpAuthority(AddButton);
            #endregion

            return View();
        }
         /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ApproveIndex()
        {
            ViewBag.PassButton = this.MenuOpAuthority(PassButton);
            ViewBag.EndButton = this.MenuOpAuthority(EndButton);
            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            string departments = "";

            if (departmentScope == 0)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();
            List<ORG_User> UserListKF = new List<ORG_User>();

            UserListKF = bll.GetGroupUsers(Common.ORG_Group_Code.ZRKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            SelectList selUsersKF = new SelectList(UserListKF, "ID", "RName", null);
            ViewBag.listUserKF = selUsersKF;

            return View();
        }

        /// <summary>
        /// 查看详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SupportFilter]  
        public ActionResult Details(int id)
        {
            ViewBag.Id = id;
            return View();

        }
 
        /// <summary>
        /// 首次创建
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 首次编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter] 
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 社保专员提取信息列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult SupplierList()
        {
            return View();
        }

        #region 员工客服详情页面
        /// <summary>
        /// 员工客服详情页面
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult ApproveDetails(int id)
        {
            IBLL.IEmployeeBLL e_BLL = new BLL.EmployeeBLL();
            IBLL.ICityBLL city_BLL = new BLL.CityBLL();
            IBLL.ICRM_CompanyBLL crm_Company_BLL = new BLL.CRM_CompanyBLL();
            IBLL.IPoliceAccountNatureBLL policeAccountNature_BLL = new BLL.PoliceAccountNatureBLL();
            IBLL.ICompanyEmployeeRelationBLL CompanyEmployeeRelation_BLL = new BLL.CompanyEmployeeRelationBLL();
            var CompanyEmployeeRelationlist = CompanyEmployeeRelation_BLL.GetById(id);

            if (CompanyEmployeeRelationlist != null)
            {
                var Employeelist = e_BLL.GetById((int)CompanyEmployeeRelationlist.EmployeeId);
                ViewBag.Employee = Employeelist;
                ViewBag.CityId = CompanyEmployeeRelationlist.CityId;
                ViewBag.CompanyId = CompanyEmployeeRelationlist.CompanyId;
                ViewBag.PoliceAccountNatureId = CompanyEmployeeRelationlist.PoliceAccountNatureId;
                ViewBag.Station = CompanyEmployeeRelationlist.Station;
                ViewBag.CompanyEmployeeRelationid = id;
                object Parameter = Request["Parameter"];
                ViewBag.Parameter = Parameter;
            }
            List<City> Citylist = city_BLL.GetAll();
            ViewBag.Citylist = Citylist.ToList();
            List<CRM_Company> CRM_Companylist = crm_Company_BLL.GetAll();
            ViewBag.CRM_Companylist = CRM_Companylist.ToList();
            List<PoliceAccountNature> PoliceAccountNaturelist = policeAccountNature_BLL.GetAll();
            ViewBag.PoliceAccountNaturelist = PoliceAccountNaturelist.ToList();
            return View();
        }
        #endregion

        /// <summary>
        /// 社保调基数查询列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeGoonPayment2ViewList()
        {
            return View();
        }
    }
}


