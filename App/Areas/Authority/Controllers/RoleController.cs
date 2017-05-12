using Common;
using Langben.BLL;
using Langben.DAL;
using Models;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System;
namespace Langben.App.Areas.Authority.Controllers
{
    public class RoleController : BaseController
    {
        #region Get
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            ORG_Role menu = m_BLL.GetById(id);
            return View(menu);

        }

        //查询数据列表
        public ActionResult GetData()
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(this.InitData()));
        }
        #endregion

        #region Post

        #region 添加角色
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            try
            {
                int total = 0;

                ORG_Role role = new ORG_Role()
                {
                    RoleCode = form["txtCode"].ToString(),
                    RoleName = form["txtRoleName"].ToString(),
                    Des = form["txtDes"].ToString(),
                    XYBZ = "Y"
                };
                Dictionary<string, object> par = new Dictionary<string, object>();
                par.Add("@code", role.RoleCode);
                par.Add("@roleName", role.RoleName);

                #region 添加前进行判断
                //编码不为空时，需要对编码唯一性进行判断
                if (!string.IsNullOrEmpty(role.RoleCode.Trim()))
                {
                    StringBuilder sqlWhere = new StringBuilder();
                    sqlWhere.Append("XYBZDDL_String&Y");
                    if (!string.IsNullOrEmpty(role.RoleCode))
                    {
                        sqlWhere.Append("^RoleCode&" + role.RoleCode);
                    }

                    List<ORG_Role> queryData = m_BLL.GetByParam(null, 0, 100, "Desc", "ID", sqlWhere.ToString(), ref total);




                    if (queryData.Count > 0)
                    {
                        return Json(new { Code = "error", Message = "此编码已被使用" });
                    }
                }
                //角色名称不为空时，需要进行判断
                if (!string.IsNullOrEmpty(role.RoleName.Trim()))
                {
                    StringBuilder sqlWhere = new StringBuilder();
                    sqlWhere.Append("XYBZDDL_String&Y");

                    if (!string.IsNullOrEmpty(role.RoleName.Trim()))
                    {
                        sqlWhere.Append("^RoleName&" + role.RoleName.Trim());
                    }

                    List<ORG_Role> queryData = m_BLL.GetByParam(null, 0, 100, "Desc", "ID", sqlWhere.ToString(), ref total);




                    if (queryData.Count > 0)
                    {

                        return Json(new { Code = "error", Message = "此角色名称已被使用" });
                    }
                }
                #endregion
                m_BLL.Create(ref validationErrors, role);

                return Json(new { Code = "ok", Message = "操作成功" });
            }
            catch
            {
                return Json(new { Code = "error", Message = "操作失败：服务器错误" });
            }
        }
        #endregion

        #region 删除角色
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            IBLL.IORG_RoleMenuOpAuthorityBLL bll = new BLL.ORG_RoleMenuOpAuthorityBLL();
            string returnValue = string.Empty;
            int[] deleteId = System.Array.ConvertAll<string, int>(ids.Split(','), delegate(string s) { return int.Parse(s); });
            if (ids != null && ids.Length > 0)
            {
                if (bll.DeleteRoleOp(deleteId))
                {
                    LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + string.Join(",", ids), "消息"
                        );//删除成功，写入日志

                    return Json(new { Code = "ok", Message = Suggestion.DeleteSucceed });
                }
                else
                {

                    LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", ids) + "," + "删除失败", "消息"
                        );//删除失败，写入日志
                    return Json(new { Code = "error", Message = Suggestion.DeleteFail + returnValue });
                }
            }
            else
            {
                return Json(new { Code = "error", Message = "未选择任何数据" });
            }
        }

        #endregion

        #region 编辑角色
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            try
            {
                int total = 0;
                ORG_Role role = m_BLL.GetById(Convert.ToInt32(form["hidID"].ToString()));
                role.ID = Convert.ToInt32(form["hidID"].ToString());
                role.RoleCode = form["txtCode"].ToString();
                role.RoleName = form["txtRoleName"].ToString();
                role.Des = form["txtDes"].ToString();


                Dictionary<string, object> par = new Dictionary<string, object>();
                par.Add("@rolecode", role.RoleCode);
                par.Add("@roleName", role.RoleName);

                #region 添加前进行判断
                //编码不为空时，需要对编码唯一性进行判断
                if (!string.IsNullOrEmpty(role.RoleCode.Trim()))
                {
                    StringBuilder sqlWhere = new StringBuilder();
                    sqlWhere.Append("XYBZDDL_String&Y");


                    if (!string.IsNullOrEmpty(role.RoleCode))
                    {
                        sqlWhere.Append("^RoleCode&" + role.RoleCode);
                    }

                    List<ORG_Role> queryData = m_BLL.GetByParam(null, 0, 100, "Desc", "ID", sqlWhere.ToString(), ref total);

                    var f = queryData.Find(x => x.ID != role.ID);




                    if (f != null)
                    {
                        return Json(new { Code = "error", Message = "此编码已被使用" });
                    }
                }
                //角色名称不为空时，需要进行判断
                if (!string.IsNullOrEmpty(role.RoleName.Trim()))
                {
                    StringBuilder sqlWhere = new StringBuilder();
                    sqlWhere.Append("XYBZDDL_String&Y");

                    if (!string.IsNullOrEmpty(role.RoleName.Trim()))
                    {
                        sqlWhere.Append("^RoleName&" + role.RoleName.Trim());
                    }

                    List<ORG_Role> queryData = m_BLL.GetByParam(null, 0, 100, "Desc", "ID", sqlWhere.ToString(), ref total);


                    var f = queryData.Find(x => x.RoleName != role.RoleName);


                    if (f != null)
                    {

                        return Json(new { Code = "error", Message = "此角色名称已被使用" });
                    }
                }
                #endregion

                m_BLL.Edit(ref validationErrors, role);
                return Json(new { Code = "ok", Message = "操作成功" });
            }
            catch
            {
                return Json(new { Code = "error", Message = "操作失败：服务器错误" });
            }

        }
        #endregion


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
                    sqlWhere.Append("^RoleName&" + roleName);
                }

                List<ORG_Role> queryData = m_BLL.GetByParam(null, pageIndex, pageSize, "Desc", "ID", sqlWhere.ToString(), ref total);

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

        IBLL.IORG_RoleBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public RoleController()
            : this(new ORG_RoleBLL()) { }

        public RoleController(ORG_RoleBLL bll)
        {
            m_BLL = bll;
        }
    }
}