using Common;
using Langben.BLL;
using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Authority.Controllers
{
    public class UserController : Controller
    {
        #region Get
        public ActionResult Index()
        {
            return View();
        }
        //查询数据列表
        public ActionResult GetData()
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(this.InitData()));
        }
        #endregion

        #region Post

        #endregion

               #region Private
        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        private dynamic InitData()
        {
            try
            {
                int total = 0;
                //接收easyui的DataGrid控件传到后台的分页参数   post方式用Request.Form["page"]
                int pageIndex = int.Parse(Request["page"]);
                int pageSize = int.Parse(Request["rows"]);

                string realName = Request["search"] ?? "";

                List<dynamic> queryData = m_BLL.GetUserList(null, pageIndex, pageSize, realName, ref total);

                var data = new Common.ClientResult.DataResult
                {
                    total = total,
                    rows = queryData
                };
                return data; 
            }
            catch
            {
                return null;
            }

        }
        #endregion
            #endregion

         IBLL.IORG_UserBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public UserController()
            : this(new ORG_UserBLL()) { }

        public UserController(ORG_UserBLL bll)
        {
            m_BLL = bll;
        }
      
	}
}