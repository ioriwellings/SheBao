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
    /// 费用支出确认
    /// </summary>
    public class COST_PayRecord_ConfirmController : BaseController
    {
        //string menuId = "1045";   // 菜单“费用支出确认”
        string LockButton = "1045-1";//锁定按钮权限码
        string DeleteButton = "1045-2";//删除按钮权限码

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            #region 按钮权限
            ViewBag.LockButton = this.MenuOpAuthority(LockButton);
            ViewBag.DeleteButton = this.MenuOpAuthority(DeleteButton);
            #endregion

            return View();
        }
    }
}


