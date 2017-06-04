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
    /// 员工补缴
    /// </summary>
    public class EmployeeGoonPaymentController : BaseController
    {
        #region 生成代码
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {

            return View();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexSef()
        {

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
        #endregion

        #region 详细页面
        /// <summary>
        /// 查看详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Details(int id)
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
                object YearMonth = Request["YearMonth"];
                ViewBag.Parameter = Parameter;
                ViewBag.YearMonth = YearMonth;
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

        #region 社保补缴添加
        /// <summary>
        /// 首次创建
        /// </summary>
        /// <returns></returns>
        [SupportFilter]

        public ActionResult Create(int id)
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
                ViewBag.CompanyEmployeeRelationid = id;
                ViewBag.Station = CompanyEmployeeRelationlist.Station;
                ViewBag.PoliceAccountNatureId = CompanyEmployeeRelationlist.PoliceAccountNatureId;
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

        #region 责任客服审核平台数据

        /// <summary>
        /// 责任客服审核列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult ApproveList()
        {
            return View();
        }

        #endregion

        #region 责任客服修改列表页面
        /// <summary>
        /// 责任客服修改列表 jing 
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CustomerModifyList()
        {
            return View();
        }
        #endregion

        #region 员工客服确认列表页面
        /// <summary>
        /// 员工客服确认列表页面 jing 
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CustomerList()
        {
            return View();
        }
        #endregion

        #region 责任客服修改页面
        /// <summary>
        /// 责任客服修改页面 jing
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CustomerModify(int id)
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
                
                object YearMonth = Request["YearMonth"];
               
                ViewBag.YearMonth = YearMonth;
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

        #region 社保专员提取补缴数据列表页面
        /// <summary>
        /// 社保专员提取补缴数据列表页面 jing 
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult ExtractionList()
        {
            return View();
        }
        #endregion

        #region 社保专员补缴信息 回退页面
        /// 社保专员补缴信息 回退页面
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeFallback(string ids)
        {
            ViewBag.ids = ids.TrimEnd(',').ToString();
            return View();

        }
        #endregion
    }
}


