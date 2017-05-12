using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_CompanyContractController : BaseController
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index(string id, string path)
        {
            ViewBag.IsCreate = true;
            IBLL.ICRM_CompanyContract_AuditBLL bll = new BLL.CRM_CompanyContract_AuditBLL();
            List<Langben.DAL.CRM_CompanyContract_Audit> list = bll.GetAll();
            if (list.Count > 0) ViewBag.IsCreate = false;

            IBLL.ICRM_CompanyContractBLL Bll = new BLL.CRM_CompanyContractBLL();
            List<Langben.DAL.CRM_CompanyContract> List = Bll.GetAll();
            if (List.Count > 0) ViewBag.IsCreate = false;

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
        public ActionResult Edit(int id)
        {
            ViewBag.ContactID = id;
            return View();
        }
    }
}