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
    /// 费用_费用表
    /// </summary>
    public class COST_CostTableController : BaseController
    {
        //string menuId = "1043";   // 菜单“责任客服审核费用”
        string LockButton = "1043-1";//锁定费用表按钮权限码
        string DeleteButton = "1043-2";//作废按钮权限码
        string ExportButton = "1043-3";//导出按钮权限码
        string RemarkButton = "1043-4";//备注按钮权限码
        string DetailButton = "1043-5";//查看详情按钮权限码

        /// <summary>
        /// 责任客服费用审核列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            #region 按钮权限
            ViewBag.LockButton = this.MenuOpAuthority(LockButton);
            ViewBag.DeleteButton = this.MenuOpAuthority(DeleteButton);
            ViewBag.ExportButton = this.MenuOpAuthority(ExportButton);
            ViewBag.RemarkButton = this.MenuOpAuthority(RemarkButton);
            ViewBag.DetailButton = this.MenuOpAuthority(DetailButton);
            #endregion

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
        /// 查看/修改备注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Remark(int id)
        {
            ViewBag.Id = id;
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
     
    }
}


