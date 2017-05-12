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
    /// 费用对比数据更新
    /// </summary>
    public class COST_PayRecord_ContrastedController : BaseController
    {
        //string menuId = "1046";   // 菜单“对比数据更新”
        string AddButton = "1046-1";//加入对比按钮权限码

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            #region 按钮权限
            ViewBag.AddButton = this.MenuOpAuthority(AddButton);
            #endregion

            return View();
        }
    }
}


