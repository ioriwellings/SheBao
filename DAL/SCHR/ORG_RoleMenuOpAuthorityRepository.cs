using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 组织机构_角色操作权限
    /// </summary>
    public partial class ORG_RoleMenuOpAuthorityRepository : BaseRepository<ORG_RoleMenuOpAuthority>, IDisposable
    {
        #region 配置菜单权限
        /// <summary>
        /// 配置菜单权限
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="type">设置类别（role;user）</param>
        /// <param name="isEditDpt">是否修改菜单操作权限</param>
        /// <param name="isEditOpt">是否修改菜单部门权限</param>
        /// <param name="isEditData">是否修改菜单数据范围权限</param>
        /// <param name="selectedMenuOpt">勾选的菜单:操作集合(菜单1:操作1,操作2;菜单2:操作3，操作4)</param>
        /// <param name="selectedMenuDpt">勾选的菜单:部门权限(菜单1:部门1,部门2;菜单2:部门1，部门4)</param>
        /// <param name="selectedMenuScope">勾选的菜单:菜单数据范围权限(菜单1:范围;菜单2:范围)</param>
        /// <returns></returns>
        public bool ConfigAuthority(int id, string type, bool isEditOpt, bool isEditDpt, bool isEditData, string selectedMenuOpt, string selectedMenuDpt, string selectedMenuScope)
        {
            bool ret = false;
            //Transaction tran = new Transaction();
            //tran.BeginTransaction();
            try
            {
                //CommonDAL dal = new CommonDAL(tran);
                if (type == "role")
                {
                    ret = ConfigRloe(id, isEditOpt, isEditDpt, isEditData, selectedMenuOpt, selectedMenuDpt, selectedMenuScope);
                }
                else//user
                {
                    ret = ConfigUser(id, isEditOpt, isEditDpt, isEditData, selectedMenuOpt, selectedMenuDpt, selectedMenuScope);
                }
                //if (ret)
                //{
                //    tran.Commit();
                //}
                //else
                //{
                //    tran.RollBack();
                //}
            }
            catch (Exception e)
            {
                //tran.RollBack();
                throw e;
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// 配置角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isEditOpt"></param>
        /// <param name="isEditDpt"></param>
        /// <param name="isEditData"></param>
        /// <param name="selectedMenuOpt"></param>
        /// <param name="selectedMenuDpt"></param>
        /// <param name="selectedMenuScope"></param>
        /// <returns></returns>
        private bool ConfigRloe(int id, bool isEditOpt, bool isEditDpt, bool isEditData, string selectedMenuOpt, string selectedMenuDpt, string selectedMenuScope)
        {
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    if (isEditOpt)
                    {
                        #region 配置操作权限
                        //删除操作权限
                        //dal.Delete<ORG_RoleMenuOpAuthority>(string.Format(" ORG_Role_ID = {0}", id));
                        //db.Database.ExecuteSqlCommand("delete from ORG_RoleMenuOpAuthority where ORG_Role_ID = " + id, null);
                        IEnumerable<ORG_RoleMenuOpAuthority> MenuOps_Old = db.ORG_RoleMenuOpAuthority.Where(c => c.ORG_Role_ID == id);
                        db.ORG_RoleMenuOpAuthority.RemoveRange(MenuOps_Old);

                        //重新添加操作权限
                        if (!string.IsNullOrEmpty(selectedMenuOpt))
                        {
                            //拆解成（菜单:操作1,操作2）类型的集合
                            string[] tmpMenusOpts = selectedMenuOpt.Split(';');

                            ORG_RoleMenuOpAuthority MenuOp = new ORG_RoleMenuOpAuthority();
                            foreach (string tmpMenuOpts in tmpMenusOpts)
                            {
                                if (!string.IsNullOrEmpty(tmpMenuOpts))
                                {
                                    //拆解菜单和菜单操作
                                    string[] strMenuOpts = tmpMenuOpts.Split(':');

                                    MenuOp = new ORG_RoleMenuOpAuthority();
                                    MenuOp.ORG_Role_ID = id;

                                    MenuOp.ORG_Menu_ID = strMenuOpts[0];//菜单ID
                                    //若有操作，则存入菜单操作ID集合
                                    if (strMenuOpts.Length > 1)
                                    {
                                        MenuOp.ORG_MenuOp_ID_List = strMenuOpts[1];
                                    }
                                    //dal.Add<ORG_RoleMenuOpAuthority>(MenuOp);
                                    db.ORG_RoleMenuOpAuthority.Add(MenuOp);
                                }
                            }
                        }
                        #endregion
                    }

                    if (isEditDpt)
                    {
                        #region 配置部门业务权限
                        //拆解成（菜单:部门1,部门2）类型的集合
                        string[] tmpMenuDpt = selectedMenuDpt.Split(';');
                        foreach (string md in tmpMenuDpt)
                        {
                            string[] menu_dpt = md.Split(':');
                            if (menu_dpt.Length > 0)
                            {
                                //删除部门的菜单部门权限
                                //dal.Delete<ORG_RoleDepartmentAuthority>(string.Format(" ORG_Role_ID = {0} and ORG_Menu_ID = '{1}'", id, menu_dpt[0]));
                                string menuId = menu_dpt[0];
                                IEnumerable<ORG_RoleDepartmentAuthority> RoleDeparts_Old = db.ORG_RoleDepartmentAuthority.Where(c => c.ORG_Role_ID == id && c.ORG_Menu_ID == menuId);
                                db.ORG_RoleDepartmentAuthority.RemoveRange(RoleDeparts_Old);
                            }

                            if (menu_dpt.Length == 2 && menu_dpt[1] != "")
                            {
                                //添加部门权限
                                ORG_RoleDepartmentAuthority DptAuthority = new ORG_RoleDepartmentAuthority();
                                DptAuthority = new ORG_RoleDepartmentAuthority();
                                DptAuthority.ORG_Role_ID = id;
                                DptAuthority.ORG_Menu_ID = menu_dpt[0];
                                DptAuthority.ORG_Department_ID_List = menu_dpt[1];

                                //dal.Add<ORG_RoleDepartmentAuthority>(DptAuthority);
                                db.ORG_RoleDepartmentAuthority.Add(DptAuthority);
                            }
                        }
                        #endregion
                    }

                    if (isEditData)
                    {
                        #region 配置部门范围权限
                        //分解成（菜单:范围）类型的集合
                        string[] tmpMenuScope = selectedMenuScope.Split(';');

                        foreach (string strMenuScope in tmpMenuScope)
                        {
                            string[] menuScope = strMenuScope.Split(':');

                            //删除原有权限
                            //dal.Delete<ORG_RoleDepartmenScopetAuthority>(string.Format("ORG_Role_ID = {0} and ORG_Menu_ID = '{1}'", id, menuScope[0]));
                            string menuId = menuScope[0];
                            IEnumerable<ORG_RoleDepartmenScopetAuthority> Scopes_Olds = db.ORG_RoleDepartmenScopetAuthority.Where(c => c.ORG_Role_ID == id && c.ORG_Menu_ID == menuId);
                            db.ORG_RoleDepartmenScopetAuthority.RemoveRange(Scopes_Olds);

                            //添加部门范围权限
                            ORG_RoleDepartmenScopetAuthority Scopet = new ORG_RoleDepartmenScopetAuthority();
                            Scopet.ORG_Role_ID = id;
                            Scopet.ORG_Menu_ID = menuScope[0];
                            Scopet.DepartmentScope = Convert.ToInt32(menuScope[1]);
                            //dal.Add<ORG_RoleDepartmenScopetAuthority>(Scopet);
                            db.ORG_RoleDepartmenScopetAuthority.Add(Scopet);

                            //菜单范围权限不为0（无限制）时，删除部门业务权限
                            if (menuScope[1] != "0")
                            {
                                //删除部门的菜单部门权限
                                //dal.Delete<ORG_RoleDepartmentAuthority>(string.Format("ORG_Role_ID = {0} and ORG_Menu_ID = '{1}'", id, menuScope[0]));
                                IEnumerable<ORG_RoleDepartmentAuthority> RoleDeparts_Err = db.ORG_RoleDepartmentAuthority.Where(c => c.ORG_Role_ID == id && c.ORG_Menu_ID == menuId);
                                db.ORG_RoleDepartmentAuthority.RemoveRange(RoleDeparts_Err);
                            }
                        }
                        #endregion
                    }

                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 配置人员权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isEditOpt"></param>
        /// <param name="isEditDpt"></param>
        /// <param name="isEditData"></param>
        /// <param name="selectedMenuOpt"></param>
        /// <param name="selectedMenuDpt"></param>
        /// <param name="selectedMenuScope"></param>
        /// <returns></returns>
        private bool ConfigUser(int id, bool isEditOpt, bool isEditDpt, bool isEditData, string selectedMenuOpt, string selectedMenuDpt, string selectedMenuScope)
        {
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    if (isEditOpt)
                    {
                        #region 配置操作权限
                        //删除操作权限
                        //dal.Delete<ORG_UserMenuOpAuthority>(string.Format(" ORG_User_ID = {0}", id));
                        db.ORG_UserMenuOpAuthority.RemoveRange(db.ORG_UserMenuOpAuthority.Where(c => c.ORG_User_ID == id));

                        //重新添加操作权限
                        if (!string.IsNullOrEmpty(selectedMenuOpt))
                        {
                            //拆解成（菜单:操作1,操作2）类型的集合
                            string[] tmpMenusOpts = selectedMenuOpt.Split(';');

                            ORG_UserMenuOpAuthority MenuOp = new ORG_UserMenuOpAuthority();
                            foreach (string tmpMenuOpts in tmpMenusOpts)
                            {
                                if (!string.IsNullOrEmpty(tmpMenuOpts))
                                {
                                    //拆解菜单和菜单操作
                                    string[] strMenuOpts = tmpMenuOpts.Split(':');

                                    MenuOp = new ORG_UserMenuOpAuthority();
                                    MenuOp.ORG_User_ID = id;

                                    MenuOp.ORG_Menu_ID = strMenuOpts[0];//菜单ID
                                    //若有操作，则存入菜单操作ID集合
                                    if (strMenuOpts.Length > 1)
                                    {
                                        MenuOp.ORG_MenuOp_ID_List = strMenuOpts[1];
                                    }
                                    //dal.Add<ORG_UserMenuOpAuthority>(MenuOp);
                                    db.ORG_UserMenuOpAuthority.Add(MenuOp);
                                }
                            }
                        }
                        #endregion
                    }

                    if (isEditDpt)
                    {
                        #region 配置部门业务权限
                        //拆解成（菜单:部门1,部门2）类型的集合
                        string[] tmpMenuDpt = selectedMenuDpt.Split(';');
                        foreach (string md in tmpMenuDpt)
                        {
                            string[] menu_dpt = md.Split(':');
                            if (menu_dpt.Length > 0)
                            {
                                //删除部门的菜单部门权限
                                //dal.Delete<ORG_UserDepartmentAuthority>(string.Format("ORG_User_ID = {0} and ORG_Menu_ID = '{1}'", id, menu_dpt[0]));
                                string menuId = menu_dpt[0];
                                db.ORG_UserDepartmentAuthority.RemoveRange(db.ORG_UserDepartmentAuthority.Where(c => c.ORG_User_ID == id && c.ORG_Menu_ID == menuId));
                            }

                            if (menu_dpt.Length == 2 && menu_dpt[1] != "")
                            {
                                //添加部门权限
                                ORG_UserDepartmentAuthority DptAuthority = new ORG_UserDepartmentAuthority();
                                DptAuthority = new ORG_UserDepartmentAuthority();
                                DptAuthority.ORG_User_ID = id;
                                DptAuthority.ORG_Menu_ID = menu_dpt[0];
                                DptAuthority.ORG_Department_ID_List = menu_dpt[1];

                                //dal.Add<ORG_UserDepartmentAuthority>(DptAuthority);
                                db.ORG_UserDepartmentAuthority.Add(DptAuthority);
                            }
                        }
                        #endregion
                    }

                    if (isEditData)
                    {
                        #region 配置部门范围权限
                        //分解成（菜单:范围）类型的集合
                        string[] tmpMenuScope = selectedMenuScope.Split(';');

                        foreach (string strMenuScope in tmpMenuScope)
                        {
                            string[] menuScope = strMenuScope.Split(':');

                            //删除原有权限
                            //dal.Delete<ORG_UserDepartmenScopetAuthority>(string.Format("ORG_User_ID = {0} and ORG_Menu_ID = '{1}'", id, menuScope[0]));
                            string menuId = menuScope[0];
                            db.ORG_UserDepartmenScopetAuthority.RemoveRange(db.ORG_UserDepartmenScopetAuthority.Where(c => c.ORG_User_ID == id && c.ORG_Menu_ID == menuId));

                            //添加部门范围权限
                            ORG_UserDepartmenScopetAuthority Scopet = new ORG_UserDepartmenScopetAuthority();
                            Scopet.ORG_User_ID = id;
                            Scopet.ORG_Menu_ID = menuScope[0];
                            Scopet.DepartmentScope = Convert.ToInt32(menuScope[1]);
                            //dal.Add<ORG_UserDepartmenScopetAuthority>(Scopet);
                            db.ORG_UserDepartmenScopetAuthority.Add(Scopet);

                            //菜单范围权限不为0（无限制）时，删除部门业务权限
                            if (menuScope[1] != "0")
                            {
                                //删除部门的菜单部门权限
                                //dal.Delete<ORG_UserDepartmentAuthority>(string.Format("ORG_User_ID = {0} and ORG_Menu_ID = '{1}'", id, menuScope[0]));
                                db.ORG_UserDepartmentAuthority.RemoveRange(db.ORG_UserDepartmentAuthority.Where(c => c.ORG_User_ID == id && c.ORG_Menu_ID == menuId));
                            }
                        }
                        #endregion
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 批量删除角色（将标志位置为‘N’）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <param name="id">菜单功能编号</param>
        /// <returns></returns>
        public bool DeleteRoleOp(int[] ids)
        {
            using (SysEntities db=new SysEntities())
            {
                try
                {
                    var query = from menuop in db.ORG_Role
                                where ids.Contains(menuop.ID)
                                select menuop;
                    var f = query.ToList();
                    foreach (var item in f)
                    {
                        item.XYBZ = "N";
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
           

        }
    }
}

