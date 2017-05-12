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
    /// 客户_企业阶梯报价
    /// </summary>
    public class CRM_CompanyLadderPriceController : BaseController
    {

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index(int id, string path)
        {
            ViewBag.Path = path;
            ViewBag.companyID = id;
            return View();
        }

        /// <summary>
        /// 创建报价
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.companyID = id;
            List<PRD_Product> listPrd = GetPrd();
            SelectList select = new SelectList(listPrd, "ID", "ProductName", null);
            ViewBag.listPrd = select;
            return View();
        }

        /// <summary>
        /// 创建阶梯报价
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CreateLadder(string id, int productID)
        {
            ViewBag.companyID = id;
            ViewBag.productID = productID;
            List<PRD_Product> listPrd = GetPrd();
            SelectList select = new SelectList(listPrd, "ID", "ProductName", null);
            ViewBag.listPrd = select;

            IBLL.IPRD_ProductBLL bll = new BLL.PRD_ProductBLL();
            PRD_Product product = new PRD_Product();
            product = bll.GetById(productID);

            ViewBag.productName = product.ProductName;
            return View();
        }

        /// <summary>
        /// 编辑报价
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            List<PRD_Product> listPrd = GetPrd();
            SelectList select = new SelectList(listPrd, "ID", "ProductName", null);
            ViewBag.listPrd = select;
            return View();
        }

        /// <summary>
        /// 编辑阶梯报价
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult EditLadder(int id)
        {
            ViewBag.Id = id;
            List<PRD_Product> listPrd = GetPrd();
            SelectList select = new SelectList(listPrd, "ID", "ProductName", null);
            ViewBag.listPrd = select;
            return View();
        }

        #region 内置
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


