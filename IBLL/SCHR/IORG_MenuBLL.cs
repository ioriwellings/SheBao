using Common;
using Langben.DAL;
using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.IBLL
{
    public partial interface IORG_MenuBLL
    {
        #region 信伟青
        /// <summary>
        /// 根据条件查询菜单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<ORG_Menu> GetMenuList(int id);
        #endregion

        /// <summary>
        /// 获取菜单树（包含上级菜单名称）
        /// </summary>
        /// <returns></returns>
        List<MenuTreeModel> GetMenuTreeList();

        /// <summary>
        /// 批量删除菜单数据
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="ids">菜单编号</param>
        /// <returns></returns>
        bool DeleteMenuCollection(ref ValidationErrors validationErrors, int[] ids);
    }
}
