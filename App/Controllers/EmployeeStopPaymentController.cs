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
    /// 员工停缴
    /// </summary>
    public class EmployeeStopPaymentController : BaseController
    {

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
        /// 社保报减查询列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeStopViewList()
        {
            return View();
        }

        /// <summary>
        /// 社保专员提取报减信息列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult SupplierList()
        {
            return View();
        }


        /// 社保专员报增信息 回退页面
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeFallback(string ids, string cityid)
        {
            ViewBag.Cityid = cityid;
            ViewBag.ids = ids.TrimEnd(',').ToString();
            return View();

        }
        /// 责任客服修改报减
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult FeedbackList()
        {

            return View();

        } 
        /// 责任客服修改报减
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult StopCustomerModifyList()
        {

            return View();

        }
        #region 责任客服修改 报减社保月
        /// <summary>
        /// 责任客服修改
        /// </summary>
        /// <param name="id">增加员工表的信息id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult StopCustomerModify(int id)
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


    }
}


