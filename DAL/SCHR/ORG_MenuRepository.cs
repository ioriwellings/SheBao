using Common;
using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class ORG_MenuRepository : BaseRepository<ORG_Menu>, IDisposable
    {
        #region 信伟青
        /// <summary>
        /// 根据条件查询菜单列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public IQueryable<ORG_Menu> GetMenuList(SysEntities db, int id)
        {
            //获取员工姓名查询员工信息
            IQueryable<ORG_Menu> menu = db.ORG_Menu.Where(o => o.XYBZ == "Y");

            if (id != null && id != 0)
            {
                // 排除自身为id的，及父级菜单为id的数据
                menu = menu.Where(a => a.ID != id && a.ParentID != id);
            }
            return menu;
        }
        #endregion

        /// <summary>
        /// 获取菜单树（包含上级菜单）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <returns></returns>
        public IQueryable<MenuTreeModel> GetMenuTreeList(SysEntities db)
        {
            var query = from a in db.ORG_Menu
                        where a.XYBZ == "Y"
                        join b in db.ORG_Menu on a.ParentID equals b.ID
                        into c
                        from menu in c.DefaultIfEmpty()
                        select new MenuTreeModel()
                        {
                            ID = a.ID,
                            MenuName = a.MenuName,
                            ParentID = a.ParentID,
                            MenuUrl = a.MenuUrl,
                            DepartmentScopeAuthority = a.DepartmentScopeAuthority,
                            DepartmentAuthority = a.DepartmentAuthority,
                            NodeLevel = a.NodeLevel,
                            Sort = a.Sort,
                            IsDisplay = a.IsDisplay,
                            _parentId = a.ParentID,
                            ParentName = menu.MenuName
                        };
            return query;
        }

        /// <summary>
        /// 删除菜单（将标志位置为‘N’）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <param name="id">菜单编号</param>
        /// <returns></returns>
        public void DeleteMenu(SysEntities db, int[] ids)
        {
            var query = from menu in db.ORG_Menu
                        where ids.Contains(menu.ID)
                        select menu;

            foreach (var item in query)
            {
                item.XYBZ = "N";
            }
        }
    }
}
