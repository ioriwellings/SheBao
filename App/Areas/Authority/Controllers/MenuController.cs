using Common;
using Langben.App.Areas.Authority.Models;
using Langben.DAL;
using Langben.DAL.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Authority.Controllers
{
    public class MenuController : BaseController
    {
        IBLL.IORG_MenuBLL m_BLL = new BLL.ORG_MenuBLL();
        ValidationErrors validationErrors = new ValidationErrors();

        //string menuID = "1051";//菜单ID
        string AddButton = "1051-1";//添加按钮权限码
        string EditButton = "1051-2";//编辑按钮权限码
        string DeleteButton = "1051-3";//删除按钮权限码

        #region Get

        /// <summary>
        /// 菜单列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region 权限验证
            ViewBag.AddButton = this.MenuOpAuthority(AddButton);
            ViewBag.EditButton = this.MenuOpAuthority(EditButton);
            ViewBag.DeleteButton = this.MenuOpAuthority(DeleteButton);
            #endregion

            return View();
        }

        public ActionResult Create()
        {
            List<ORG_Menu> menulist = m_BLL.GetMenuList(0);

            ViewData["ddlMenu"] = new SelectList(menulist, "ID", "MenuName", "");
            return View();
        }

        public ActionResult Edit(int id)
        {
            ORG_Menu menu = m_BLL.GetById(id);
            List<ORG_Menu> menulist = m_BLL.GetMenuList(id);

            ViewData["ddlMenu"] = new SelectList(menulist, "ID", "MenuName", "");
            return View(menu);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetData()
        {
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(this.InitData());
            return Content(content);
        }

        #endregion

        #region Post

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="ids">菜单编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMenu(string ids)
        {
            string returnValue = string.Empty;
            int[] deleteId = Array.ConvertAll<string, int>(ids.Split(','), delegate(string s) { return int.Parse(s); });
            if (ids != null && ids.Length > 0)
            {
                if (m_BLL.DeleteMenuCollection(ref validationErrors, deleteId))
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
                return Json(new { Code = "error", Message = "未选择任何数据"});
            }
            
        }

        [HttpPost]
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(ORG_Menu menu)
        {
            try
            {
                // 根据上级菜单类别确定菜单类别
                if (menu.ParentID != 0)
                {
                    ORG_Menu ParentMenu = m_BLL.GetById(menu.ParentID);
                    menu.NodeLevel = ParentMenu.NodeLevel + 1;
                }
                else
                {
                    menu.ParentID = 0;
                    menu.NodeLevel = 1;
                }

                // 权限配置操作
                menu.DepartmentScopeAuthority = menu.DepartmentScopeAuthority == null ? "N" : "Y";
                menu.DepartmentAuthority = menu.DepartmentAuthority == null ? "N" : "Y";
                menu.IsDisplay = menu.IsDisplay == null ? "N" : "Y";

                menu.XYBZ = "Y";

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, menu))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，菜单表的信息的Id为" + menu.ID, "菜单表"
                        );//写入日志 
                    return Json(new{ Code = "ok", Message = Suggestion.InsertSucceed }); //提示创建成功
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，菜单表的信息，" + returnValue, "菜单表"
                        );//写入日志                      
                    return Json(new { Code = "error", Message = Suggestion.InsertFail + returnValue }); //提示插入失败
                }
            }
            catch(Exception ex)
            {
                return Json(new { Code = "error", Message = "操作失败：服务器错误" });
            }
        }

        [HttpPost]
        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(ORG_Menu menu)
        {
            try
            {
                // 根据上级菜单类别确定菜单类别
                if (menu.ParentID != 0)
                {
                    ORG_Menu ParentMenu = m_BLL.GetById(menu.ParentID);
                    menu.NodeLevel = ParentMenu.NodeLevel + 1;
                }
                else
                {
                    menu.ParentID = 0;
                    menu.NodeLevel = 1;
                }

                // 权限配置操作
                menu.DepartmentScopeAuthority = menu.DepartmentScopeAuthority == null ? "N" : "Y";
                menu.DepartmentAuthority = menu.DepartmentAuthority == null ? "N" : "Y";
                menu.IsDisplay = menu.IsDisplay == null ? "N" : "Y";

                menu.XYBZ = "Y";

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, menu))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，菜单表的信息的Id为" + menu.ID, "菜单表"
                        );//写入日志 
                    return Json(new { Code = "ok", Message = Suggestion.UpdateSucceed }); //提示创建成功
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，菜单表的信息，" + returnValue, "菜单表"
                        );//写入日志                      
                    return Json(new { Code = "error", Message = Suggestion.UpdateFail + returnValue }); //提示插入失败
                }
            }
            catch (Exception ex)
            {
                return Json(new { Code = "error", Message = "操作失败：服务器错误" });
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
            
            try
            {
                List<MenuTreeModel> menuTreeList = m_BLL.GetMenuTreeList();

                //rows是前台treegrid所需要的，名字是固定的不可修改
                return new { total = menuTreeList.Count, rows = menuTreeList };
            }
            catch
            {
                return new { total = 0, rows = ""};
            }

        }
        #endregion

        #endregion
    }
}
