using Langben.App.Areas.Authority.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;

namespace Langben.App.Areas.Authority.Controllers
{
    public class ConfigController : BaseController
    {
        #region Get

        #region 配置权限
        /// <summary>
        /// 权限配置页
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="type">类型(role;user)</param>
        /// <returns></returns>
        public ActionResult ConfigAuthority(int id, string type)
        {
            ViewBag.ID = id;
            ViewBag.Type = type;
            return View();
        }
        #endregion

        #region 获取菜单树Json
        /// <summary>
        /// 得到菜单树Json
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMenuTree(string nodeID, int id, string type)
        {
            List<SysMenu> SysMenuList = new List<SysMenu>();
            // 获取菜单树
            SysMenuList = GetMenuTree(id, type, nodeID, true);

            if (SysMenuList.Count > 0)
            {
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(SysMenuList);
                return Content(content);
            }
            else
            {
                return Content("[]");
            }
        }
        #endregion

        #region 获取部门树
        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="type">类别(role;user)</param>
        /// <returns></returns>
        public ActionResult GetDepTree(int id, string type)
        {
            //CommonBLL bll = new CommonBLL();
            IBLL.IORG_DepartmentBLL bll = new BLL.ORG_DepartmentBLL();

            //获取页面参数
            string strDptID = Request["menuDpt"] ?? "";//页面选中的部门
            //string strRoleID = Request["roleID"] ?? "0";//当前角色
            string strMenuId = Request["menuId"] ?? "0";//页面选中的菜单
            //页面是否曾经加载过，若未加载过，需选中的菜单以数据库数据为准，否则以页面参数为准
            string strWindow = Request["isWindow"] ?? "0";

            List<DepTree> list = new List<DepTree>();
            //获取所有部门
            List<ORG_Department> dt = bll.GetByParam("", "asc", "Sort", "XYBZDDL_String&Y");

            if (dt.Count > 0)
            {
                foreach (ORG_Department dr in dt)
                {
                    DepTree tree = new DepTree();
                    tree.ID = dr.ID;
                    tree.DepartmentName = dr.DepartmentName;

                    tree.ParentID = dr.ParentID == null ? 0 : Convert.ToInt32(dr.ParentID);

                    tree.open = true;

                    if (strWindow == "0")
                    {
                        if (type == "role")
                        {
                            IBLL.IORG_RoleDepartmentAuthorityBLL RoleDepBll = new BLL.ORG_RoleDepartmentAuthorityBLL();

                            ORG_RoleDepartmentAuthority RoleDep = new ORG_RoleDepartmentAuthority();
                            RoleDep = RoleDepBll.GetByParam("", "asc", "ID", string.Format("ORG_Role_IDDDL_Int&{0}^ORG_Menu_IDDDL_String&{1}", id, strMenuId)).FirstOrDefault(); ;


                            if (RoleDep != null && !string.IsNullOrEmpty(RoleDep.ORG_Department_ID_List))
                            {
                                foreach (string opt in RoleDep.ORG_Department_ID_List.Split(','))
                                {
                                    if (opt == tree.ID.ToString())
                                    {
                                        tree.Checked = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else//user
                        {
                            IBLL.IORG_UserDepartmentAuthorityBLL UserDepBll = new BLL.ORG_UserDepartmentAuthorityBLL();
                            ORG_UserDepartmentAuthority UserDep = new ORG_UserDepartmentAuthority();
                            UserDep = UserDepBll.GetByParam("", "asc", "ID", string.Format("ORG_User_IDDDL_Int&{0}^ORG_Menu_IDDDL_String&{1}", id, strMenuId)).FirstOrDefault();


                            if (UserDep != null && !string.IsNullOrEmpty(UserDep.ORG_Department_ID_List))
                            {
                                foreach (string opt in UserDep.ORG_Department_ID_List.Split(','))
                                {
                                    if (opt == tree.ID.ToString())
                                    {
                                        tree.Checked = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string opt in strDptID.Split(','))
                        {
                            if (opt == tree.ID.ToString())
                            {
                                tree.Checked = true;
                                break;
                            }
                        }
                    }
                    list.Add(tree);
                }

                string content = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                content = content.Replace("Checked", "checked");
                return Content(content);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region ChoseDepartment
        //public ActionResult ChoseDepartment(string menuID = "")
        //{
        //    CommonBLL bll = new CommonBLL();
        //    string where = "XYBZ='Y'";

        //    int departmentScope = base.MenuDepartmentScopeAuthority(menuID);

        //    //departmentScope = 4;
        //    if (departmentScope == 0)//无限制
        //    {
        //        /*****组织机构—菜单表“DepartmentAuthority（是否拥有部门业务权限配置功能）”字段为“Y”时才有如下逻辑判断
        //            *
        //            *不管“DepartmentAuthority”是否为“Y”，都写上以下判断逻辑，程序也没有问题
        //            * 
        //            */
        //        string departments = base.MenuDepartmentAuthority(menuID);
        //        if (!string.IsNullOrEmpty(departments))
        //        {
        //            where += " and b.ORG_Department_ID in (" + departments + ")";
        //        }
        //        else//查询所有部门数据
        //        {

        //        }
        //    }
        //    else if (departmentScope == 1)//本机构及下属机构
        //    {
        //        //当前用户直属机构
        //        ORG_Department branch = bll.GetModelByID<ORG_Department>(base.LoginInfo.BranchID);
        //        //查询本机构及下属机构所有用户数据
        //        where += " and  b.LeftValue>=" + branch.LeftValue + " and b.RightValue<=" + branch.RightValue;
        //    }
        //    else if (departmentScope == 2) //本机构
        //    {
        //        //查询本机构所有用户数据
        //        where += " and  b.BranchID=" + base.LoginInfo.BranchID;
        //    }
        //    else if (departmentScope == 3)//本部门及其下属部门
        //    {
        //        //当前用户所属部门
        //        ORG_Department department = bll.GetModelByID<ORG_Department>(base.LoginInfo.DepartmentID);
        //        //查询本部门及下属部门所有用户数据
        //        where += " and  b.LeftValue>=" + department.LeftValue + " and b.RightValue<=" + department.RightValue;
        //    }
        //    else if (departmentScope == 4) //本部门
        //    {
        //        //查询本部门所有用户数据
        //        where += " and  b.ID=" + base.LoginInfo.DepartmentID;
        //    }

        //    string backColumn = "ID,DepartmentName,ParentID";
        //    DataTable dt = bll.ExecuteTable("ORG_Department b", where, backColumn);
        //    ViewBag.Json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

        //    return View();
        //}
        #endregion

        #region ChoseDepart
        //public ActionResult ChoseDepart(int? DepartmentID = null, int? DepartmentType = null, string menuID = "")
        //{
        //    CommonBLL bll = new CommonBLL();
        //    string where = "XYBZ='Y'";

        //    if (DepartmentID != null && DepartmentType != null)
        //    {
        //        if (DepartmentType == 1)
        //            where += " and  ParentID!=" + DepartmentID + "or DepartmentType=" + DepartmentType;
        //        if (DepartmentType == 2)
        //            where += " and  DepartmentType=1";
        //    }


        //    if (menuID != "")
        //    {
        //        #region  权限配置
        //        int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
        //        if (departmentScope == (int)Common.EnumDict.DepartmentScope.无限制)//无限制
        //        {
        //            /*****组织机构—菜单表“DepartmentAuthority（是否拥有部门业务权限配置功能）”字段为“Y”时才有如下逻辑判断
        //             *
        //             *不管“DepartmentAuthority”是否为“Y”，都写上以下判断逻辑，程序也没有问题
        //             * 
        //             */
        //            string departments = base.MenuDepartmentAuthority(menuID);
        //            if (!string.IsNullOrEmpty(departments))
        //            {
        //                where += " and b.ID in (" + departments + ")";
        //            }
        //            else//查询所有部门数据
        //            {

        //            }
        //        }
        //        else if (departmentScope == (int)Common.EnumDict.DepartmentScope.本机构及下属机构)//本机构及下属机构
        //        {
        //            //当前用户直属机构
        //            ORG_Department branch = bll.GetModelByID<ORG_Department>(base.LoginInfo.BranchID);
        //            //查询本机构及下属机构所有用户数据
        //            where += " and  b.LeftValue>=" + branch.LeftValue + " and b.RightValue<=" + branch.RightValue;
        //        }
        //        else if (departmentScope == (int)Common.EnumDict.DepartmentScope.本机构) //本机构
        //        {
        //            //查询本机构所有用户数据
        //            where += " and  b.BranchID=" + base.LoginInfo.BranchID;
        //        }
        //        else if (departmentScope == (int)Common.EnumDict.DepartmentScope.本部门及其下属部门)//本部门及其下属部门
        //        {
        //            //当前用户所属部门
        //            ORG_Department department = bll.GetModelByID<ORG_Department>(base.LoginInfo.DepartmentID);
        //            //查询本部门及下属部门所有用户数据
        //            where += " and  b.LeftValue>=" + department.LeftValue + " and b.RightValue<=" + department.RightValue;
        //        }
        //        else if (departmentScope == (int)Common.EnumDict.DepartmentScope.本部门) //本部门
        //        {
        //            //查询本部门所有用户数据
        //            where += " and  b.ID=" + base.LoginInfo.DepartmentID;
        //        }
        //        else if (departmentScope == (int)Common.EnumDict.DepartmentScope.本人) //本人
        //        {
        //            //本人不能选部门
        //            where += " and 1=0 ";
        //        }

        //        #endregion
        //    }

        //    string backColumn = "ID,DepartmentName,ParentID,DepartmentType";
        //    DataTable dt = bll.ExecuteTable("ORG_Department b", where, backColumn);
        //    ViewBag.Json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

        //    return View();
        //}
        #endregion

        #region 选择菜单
        //public ActionResult ChoseMenu(int? MenuId = null)
        //{
        //    CommonBLL bll = new CommonBLL();
        //    string where = "XYBZ='Y'";

        //    string backColumn = "ID,MenuName,ParentID";
        //    DataTable dt = bll.ExecuteTable("ORG_Menu", where, backColumn);

        //    if (MenuId != null)
        //    {
        //        foreach (DataRow dr in dt.Select("ID =" + MenuId))
        //        {
        //            DeleteChildMenu(dr, ref dt);
        //            dt.Rows.Remove(dr);
        //        }
        //    }
        //    string context = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        //    //context = context.Replace("\"ParentID\":\"0\"", "\"ParentID\":0");
        //    ViewBag.Json = context;
        //    return View();
        //}
        #endregion

        #endregion

        #region Post

        #region 提交数据
        /// <summary>
        /// 配置角色权限
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="type">（role;user）</param>
        /// <param name="isEditDpt">是否修改菜单操作权限</param>
        /// <param name="isEditOpt">是否修改菜单部门权限</param>
        /// <param name="isEditData">是否修改菜单数据范围权限</param>
        /// <param name="selectedMenuOpt">勾选的菜单:操作集合(菜单1:操作1,操作2;菜单2:操作3，操作4)</param>
        /// <param name="selectedMenuDpt">勾选的菜单:部门权限(菜单1:部门1,部门2;菜单2:部门1，部门4)</param>
        /// <param name="selectedMenuScope">勾选的菜单:菜单数据范围权限(菜单1:范围;菜单2:范围)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Config(int id, string type, bool isEditOpt, bool isEditDpt, bool isEditData, string selectedMenuOpt, string selectedMenuDpt, string selectedMenuScope)
        {
            try
            {
                // 配置权限
                IBLL.IORG_RoleMenuOpAuthorityBLL bll = new BLL.ORG_RoleMenuOpAuthorityBLL();
                bool rtn = bll.ConfigAuthority(id, type, isEditOpt, isEditDpt, isEditData, selectedMenuOpt, selectedMenuDpt, selectedMenuScope);

                if (rtn)
                {
                    return Json(new { Code = "ok", Message = "权限配置成功" });
                }
                else
                {
                    return Json(new { Code = "error", Message = "权限配置失败" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Code = "error", Message = "操作失败：服务器错误" });
            }

        }
        #endregion

        #endregion

        #region Private

        #region 获取菜单树List
        /// <summary>
        /// 获取菜单树List
        /// </summary>
        /// <param name="IsOpt">是否获取权限(true:是；false:否  默认值：true)</param>
        /// <returns></returns>
        private List<SysMenu> GetMenuTree(int id, string type, string nodeID, bool IsOpt = true)
        {
            IBLL.IORG_MenuBLL bll = new BLL.ORG_MenuBLL();
            //获取菜单树数据源
            //DataTable dt = bll.ExecuteTable<ORG_Menu>("XYBZ='Y'");

            List<ORG_Menu> dt = bll.GetByParam("", "asc", "Sort", "XYBZDDL_String&Y");

            List<SysMenu> SysMenuList = new List<SysMenu>();

            if (dt.Count > 0)
            {
                foreach (ORG_Menu dr in dt.Where(c => c.NodeLevel == 1))
                {
                    if (nodeID == null || nodeID.Split(':').Contains(dr.ID.ToString()))
                    {
                        SysMenu tr = new SysMenu();
                        tr.id = dr.ID;
                        tr.ParentID = dr.ParentID;
                        tr.name = dr.MenuName;
                        tr.NodeLevel = dr.NodeLevel;
                        tr.DepartmentAuthority = dr.DepartmentAuthority;
                        tr.DepartmentScopeAuthority = dr.DepartmentScopeAuthority;

                        //若需要获取菜单权限，则同时获取菜单相关操作的权限
                        if (IsOpt)
                        {
                            GetChecked(id, type, ref tr);
                        }

                        // 获取子目录
                        tr.children = bindChildrenMenu(dt.Where(c => c.ParentID == tr.id).ToList(), dt, nodeID, IsOpt, id, type);
                        //if (MO.children.Count != 0)
                        //{
                        tr.open = true;
                        SysMenuList.Add(tr);
                    }
                }
            }

            return SysMenuList;
        }
        #endregion

        #region 根据栏目生成子目录
        /// <summary>
        /// 根据栏目生成子目录
        /// </summary>
        /// <param name="drs">栏目</param>
        /// <param name="dt">目录树的表</param>
        /// <param name="roleID">角色ID</param>
        /// <param name="isOpt">是否获取权限(true:是；false:否)</param>
        /// <returns></returns>
        private List<SysMenu> bindChildrenMenu(List<ORG_Menu> drs, List<ORG_Menu> dt, string nodeID, bool isOpt, int id, string type)
        {
            //IBLL.IORG_MenuBLL bll = new BLL.ORG_MenuBLL();
            List<SysMenu> list = new List<SysMenu>();
            if (drs.Count() == 0)
            {
                return list;
            }
            foreach (ORG_Menu dr in drs)
            {
                if (nodeID == null || nodeID.Split(':').Contains(dr.ID.ToString()))
                {
                    SysMenu tr = new SysMenu();
                    tr.id = dr.ID;
                    tr.ParentID = dr.ParentID;
                    tr.name = dr.MenuName;
                    tr.NodeLevel = dr.NodeLevel;
                    tr.DepartmentAuthority = dr.DepartmentAuthority;
                    tr.DepartmentScopeAuthority = dr.DepartmentScopeAuthority;
                    //tr.MType = Convert.ToInt32(dr["MType"]);

                    // 获取子目录
                    tr.children = bindChildrenMenu(dt.Where(c => c.ParentID == tr.id).ToList(), dt, nodeID, isOpt, id, type);
                    tr.open = true;
                    //若需要获取菜单权限，则同时获取菜单相关操作的权限
                    if (isOpt)
                    {
                        GetChecked(id, type, ref tr);

                    }
                    list.Add(tr);
                }
            }
            return list;
        }

        #endregion

        #region 获取默认选中的菜单
        /// <summary>
        /// 获取默认选中的菜单
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="type">类别(role;user)</param>
        /// <param name="tr"></param>
        private void GetChecked(int id, string type, ref SysMenu tr)
        {
            IBLL.IORG_MenuOpBLL MoBll = new BLL.ORG_MenuOpBLL();
            //获取菜单相关操作
            List<ORG_MenuOp> ListOpt = new List<ORG_MenuOp>();
            //ListOpt = MoBll.GetByParam("","ID","asc","XYBZ = 'Y' and Org_Menu_ID = '" + tr.id + "' order by Sort asc");
            ListOpt = MoBll.GetByParam("", "asc", "Sort", "XYBZDDL_String&Y^Org_Menu_IDDDL_Int&" + tr.id.ToString());
            string MenuOpIds = "";
            //获取菜单操作权限
            if (type == "role")
            {
                IBLL.IORG_RoleMenuOpAuthorityBLL RmoBll = new BLL.ORG_RoleMenuOpAuthorityBLL();
                ORG_RoleMenuOpAuthority MenuOpAuthority = new ORG_RoleMenuOpAuthority();
                //MenuOpAuthority = bll.GetByParam("", "asc", "Sort", string.Format("ORG_Role_ID = {0} and ORG_Menu_ID = '{1}'", id, tr.id)).FirstOrDefault();
                MenuOpAuthority = RmoBll.GetByParam("", "asc", "ID", string.Format("ORG_Role_IDDDL_Int&{0}^ORG_Menu_IDDDL_String&{1}", id, tr.id)).FirstOrDefault();

                if (MenuOpAuthority != null)
                {
                    MenuOpIds = MenuOpAuthority.ORG_MenuOp_ID_List ?? "";
                    //tr.DataScope = MenuOpAuthority.DataScope;
                    tr.IsChecked = true;
                }

                ORG_RoleDepartmenScopetAuthority DepartmentScopet = new ORG_RoleDepartmenScopetAuthority();
                IBLL.IORG_RoleDepartmenScopetAuthorityBLL RdsBll = new BLL.ORG_RoleDepartmenScopetAuthorityBLL();
                //DepartmentScopet = RdsBll.GetModel<ORG_RoleDepartmenScopetAuthority>(string.Format("ORG_Role_ID = {0} and ORG_Menu_ID = '{1}'", id, tr.id));
                DepartmentScopet = RdsBll.GetByParam("", "asc", "ID", string.Format("ORG_Role_IDDDL_Int&{0}^ORG_Menu_IDDDL_String&{1}", id, tr.id)).FirstOrDefault();
                if (DepartmentScopet != null)
                {
                    tr.DataScope = DepartmentScopet.DepartmentScope;
                }

            }
            else//user
            {
                IBLL.IORG_UserMenuOpAuthorityBLL UmoBll = new BLL.ORG_UserMenuOpAuthorityBLL();
                ORG_UserMenuOpAuthority MenuOpAuthority = new ORG_UserMenuOpAuthority();
                //MenuOpAuthority = bll.GetModel<ORG_UserMenuOpAuthority>(string.Format("ORG_User_ID = {0} and ORG_Menu_ID = '{1}'", id, tr.id));
                MenuOpAuthority = UmoBll.GetByParam("", "asc", "ID", string.Format("ORG_User_IDDDL_Int&{0}^ORG_Menu_IDDDL_String&{1}", id, tr.id)).FirstOrDefault();
                if (MenuOpAuthority != null)
                {
                    MenuOpIds = MenuOpAuthority.ORG_MenuOp_ID_List ?? "";
                    //tr.DataScope = MenuOpAuthority.DataScope;
                    tr.IsChecked = true;
                }

                ORG_UserDepartmenScopetAuthority DepartmentScopet = new ORG_UserDepartmenScopetAuthority();
                IBLL.IORG_UserDepartmenScopetAuthorityBLL UdsBll = new BLL.ORG_UserDepartmenScopetAuthorityBLL();
                //DepartmentScopet = bll.GetModel<ORG_UserDepartmenScopetAuthority>(string.Format("ORG_User_ID = {0} and ORG_Menu_ID = '{1}'", id, tr.id));
                DepartmentScopet = UdsBll.GetByParam("", "asc", "ID", string.Format("ORG_User_IDDDL_Int&{0}^ORG_Menu_IDDDL_String&{1}", id, tr.id)).FirstOrDefault();
                if (DepartmentScopet != null)
                {
                    tr.DataScope = DepartmentScopet.DepartmentScope;
                }
            }

            List<MenuOption> ListMenuOpt = new List<MenuOption>();
            MenuOption Option = new MenuOption();
            foreach (ORG_MenuOp model in ListOpt)
            {
                Option = new MenuOption();
                Option.ID = model.ID;
                Option.MenuOpName = model.MenuOpName;
                Option.SYS_Menu_ID = model.ORG_Menu_ID;
                // 获取当前操作权限
                foreach (string MenuOpId in MenuOpIds.Split(','))
                {
                    if (MenuOpId == model.ID)
                    {
                        Option.IsChecked = true;
                        break;
                    }
                }
                tr.MenuOptions.Add(Option);
                tr.MenuOptIds += Option.ID + ",";
            }

            if (!string.IsNullOrEmpty(tr.MenuOptIds))
            {
                tr.MenuOptIds = tr.MenuOptIds.Substring(0, tr.MenuOptIds.LastIndexOf(','));
            }
        }
        #endregion

        //private void DeleteChildMenu(DataRow dr, ref DataTable dt)
        //{
        //    foreach (DataRow dr1 in dt.Select("ParentID =" + dr["ID"]))
        //    {
        //        DeleteChildMenu(dr1, ref dt);
        //        dt.Rows.Remove(dr1);
        //    }
        //}

        #endregion

    }
}
