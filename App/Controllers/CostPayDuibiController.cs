using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Langben.App.Controllers
{
    public class CostPayDuibiController : BaseController
    {

        string menuID = "1";//菜单ID
        string AddButton = "1-1";//添加按钮权限码
        string EditButton = "1-2";//编辑按钮权限码
        string DeleteButton = "1-3";//删除按钮权限码
        //
        // GET: /CostPayDuibi/
        public ActionResult Index()
        {
            #region 权限验证
            ViewBag.AddButton = this.MenuOpAuthority(AddButton);
            ViewBag.EditButton = this.MenuOpAuthority(EditButton);
            ViewBag.DeleteButton = this.MenuOpAuthority(DeleteButton);
            #endregion

            return View();
        }
        /// <summary>
        /// 查看详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Details(int id, string yearMonthStart, string yearMonthEnd,int costType)
        {
            ViewBag.Id = id;
            ViewBag.yearMonthStart = yearMonthStart;
            ViewBag.yearMonthEnd = yearMonthEnd;
            ViewBag.costType = costType;
            return View();
        }
    }
}