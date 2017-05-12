using Langben.DAL.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_Company_InsuranceController : BaseController
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index(string id, string path)
        {
            ViewBag.Path = path;
            ViewBag.CompanyID = id;
            return View();
        }

        /// <summary>
        /// 首次创建
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.CompanyID = id;
            return View();
        }

        /// <summary>
        /// 首次编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult Edit(int id, string city)
        {
            ViewBag.CompanyID = id;
            ViewBag.CityID = city;

            //CompanyInsurance_EditView model = new CompanyInsurance_EditView();

            //IBLL.ICRM_Company_InsuranceBLL m_BLL = new BLL.CRM_Company_InsuranceBLL();
            //model = m_BLL.GetByCompanyCity(id, city);

            //ViewBag.Model = model;

            return View();
        }
    }
}