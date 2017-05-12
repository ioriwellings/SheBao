using Common;
using Langben.BLL;
using Langben.DAL;
using Models;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;


namespace Langben.App.Areas.Authority.Controllers
{
    public class GroupController : BaseController
    {

        #region Get
        //
        // GET: /Authority/Group/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            int idnow = 0;
            int.TryParse(id, out idnow);
            ORG_Group model = new ORG_Group();
            ORG_Group model1 = m_BLL.GetById(idnow);
            if (model1 != null)
            {
                model = model1;
            }
            return View(model);
        }
        public ActionResult GetData()
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(this.InitData()));
        }

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

                StringBuilder sqlWhere = new StringBuilder();
                sqlWhere.Append("XYBZDDL_String&Y");
                string roleName = Request["search"] ?? "";
                if (!string.IsNullOrEmpty(roleName))
                {
                    sqlWhere.Append("^GroupName&" + roleName);
                }

                List<ORG_Group> queryData = m_BLL.GetByParam(null, pageIndex, pageSize, "Desc", "ID", sqlWhere.ToString(), ref total);

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

        IBLL.IORG_GroupBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public GroupController()
            : this(new ORG_GroupBLL()) { }

        public GroupController(ORG_GroupBLL bll)
        {
            m_BLL = bll;
        }
    }
}