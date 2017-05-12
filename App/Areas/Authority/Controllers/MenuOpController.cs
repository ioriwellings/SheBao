using Common;
using Langben.BLL;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Langben.App.Areas.Authority.Controllers
{
    public class MenuOpController : BaseController
    {
        ValidationErrors validationErrors = new ValidationErrors();
        string AddButton = "1052-1";//添加按钮权限码
        string EditButton = "1052-2";//编辑按钮权限码
        string DeleteButton = "1052-3";//删除按钮权限码
        #region Get
        public ActionResult Index()
        {
            #region 权限验证
            ViewBag.AddButton = this.MenuOpAuthority(AddButton);
            ViewBag.EditButton = this.MenuOpAuthority(EditButton);
            ViewBag.DeleteButton = this.MenuOpAuthority(DeleteButton);
            #endregion
            IBLL.IORG_MenuBLL m_BLL = new ORG_MenuBLL();
            List<ORG_Menu> menulist = m_BLL.GetMenuList(0);

            ViewData["ddlMenu"] = new SelectList(menulist, "ID", "MenuName", "");
            return View();
        }
        public ActionResult Create()
        {
            IBLL.IORG_MenuBLL m_BLL = new ORG_MenuBLL();
            List<ORG_Menu> menulist = m_BLL.GetMenuList(0);

            ViewData["ddlMenu"] = new SelectList(menulist, "ID", "MenuName");
            return View();
        }

        public ActionResult Edit(string id)
        {
            IBLL.IORG_MenuBLL m_BLL = new ORG_MenuBLL();
            IBLL.IORG_MenuOpBLL o_BLL = new ORG_MenuOpBLL();
            ORG_MenuOp item = o_BLL.GetById(id);

            List<ORG_Menu> menulist = m_BLL.GetMenuList(0);

            ViewData["ddlMenu"] = new SelectList(menulist, "ID", "MenuName", item.ORG_Menu_ID.ToString());
            ViewBag.Id = id;
            return View(item);
        }
        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetData()
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(this.InitData()));
        }
        #endregion


        #region Post
        /// <summary>
        /// 添加菜单功能信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Post(ORG_MenuOp entity)
        {
            IBLL.IORG_MenuOpBLL o_bll = new ORG_MenuOpBLL();

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.ID = entity.ORG_Menu_ID + "-" + entity.ID;
                entity.Sort = 100;
                entity.XYBZ = "Y";

                #region 添加前进行判断
                //编码不为空时，需要对编码唯一性进行判断
                if (!string.IsNullOrEmpty(entity.ID.Trim()))
                {
                    List<ORG_MenuOp> list = o_bll.GetMenuOpId(entity.ID);
                    if (list.Count > 0)
                    {
                        return Json(new { Code = 0, Message = "此菜单功能ID已被占用" });
                    }
                }
                #endregion
                
                string returnValue = string.Empty;
                if (o_bll.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，菜单功能的信息的Id为" + entity.ID, "菜单功能"
                        );//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed;
                    //return result; //提示创建成功
                    return Json(new { Code = result.Code, Message = result.Message });
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，菜单功能的信息，" + returnValue, "菜单功能"
                        );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    //return result; //提示插入失败
                    return Json(new { Code = result.Code, Message = result.Message });
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            //return result;
            return Json(new { Code = result.Code, Message = result.Message });
        }

        /// <summary>
        /// 编辑菜单功能信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Put([FromBody]ORG_MenuOp entity)
        {
            IBLL.IORG_MenuOpBLL b_BLL = new ORG_MenuOpBLL();
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                string returnValue = string.Empty;
                if (b_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，菜单功能的Id为" + entity.ID, "菜单功能"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    //return result; //提示更新成功 
                    return Json(new { Code = result.Code, Message = result.Message });
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，菜单功能的Id为" + entity.ID + "," + returnValue, "菜单功能"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    //return result; //提示更新失败
                    return Json(new { Code = result.Code, Message = result.Message });
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "请核对输入的数据的格式";
            //return result; //提示输入的数据的格式不对  
            return Json(new { Code = result.Code, Message = result.Message });
        }

        /// <summary>
        /// 删除菜单功能
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            IBLL.IORG_MenuOpBLL b_BLL = new ORG_MenuOpBLL();
            string returnValue = string.Empty;
            if (ids != null && ids.Length > 0)
            {
                string[] deleteId = Array.ConvertAll<string, string>(ids.Split(','), delegate(string s) { return s; });
                if (b_BLL.DeleteMenuOp(ref validationErrors, deleteId))
                {
                    LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + string.Join(",", ids), "消息"
                        );//删除成功，写入日志

                    return Json(new { Code = "ok", Message = Suggestion.DeleteSucceed });
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", ids) + "," + returnValue, "消息"
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

        #region Private
        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        private dynamic InitData()
        {
            IBLL.IORG_MenuOpBLL opbll = new ORG_MenuOpBLL();
            try
            {
                int total = 0;
                int pageIndex = int.Parse(Request["page"]);
                int pageSize = int.Parse(Request["rows"]);
                string menuID = Request["menuID"] ?? "";
                List<dynamic> queryData = opbll.GetMenuOpList(null, pageIndex, pageSize, menuID, ref total);
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
    }
}
