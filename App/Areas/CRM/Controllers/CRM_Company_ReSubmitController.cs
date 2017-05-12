using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_Company_ReSubmitController : BaseController
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
        /// 编辑基本信息
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EditBaseInfo(string id)
        {
            ViewBag.Id = id;
            List<CRM_ZD_HY> listHY = new List<CRM_ZD_HY>();
            IBLL.ICRM_ZD_HYBLL hy_BLL = new BLL.CRM_ZD_HYBLL();
            string sqlWhere = "NodeLevelDDL_Int&1^XYBZDDL_String&Y";
            listHY = hy_BLL.GetByParam(null, "asc", "code", sqlWhere);
            SelectList select = new SelectList(listHY, "code", "hymc", null);
            ViewBag.HY = select;
            return View();
        }
	}
}