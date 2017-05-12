using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Langben.App.Areas.Authority.Models
{
    public class SysMenu
    {
        public SysMenu()
        {
            id = 0;
            MenuOptions = new List<MenuOption>();
            children = new List<SysMenu>();
        }

        #region 属性

        /// <summary>
        ///主键
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///菜单名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///父菜单（栏目、频道）
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        ///菜单类型（1：频道 2：栏目 3：Url地址）
        /// </summary>
        public int NodeLevel { get; set; }
        /// <summary>
        ///菜单地址
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        ///排序（从小到大）
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 是否拥有分支机构业务权限配置功能（Y：拥有 N：不拥有）
        /// </summary>
        public string CompanyAuthority { get; set; }

        /// <summary>
        /// 是否拥有分支机构范围权限配置功能（Y：拥有 N：不拥有）
        /// </summary>
        public string CompanyScopeAuthority { get; set; }

        /// <summary>
        /// 是否拥有部门业务权限配置功能（Y：拥有 N：不拥有）
        /// </summary>
        public string DepartmentAuthority { get; set; }

        /// <summary>
        /// 是否拥有部门范围权限配置功能（Y：拥有 N：不拥有）
        /// </summary>
        public string DepartmentScopeAuthority { get; set; }

        /// <summary>
        /// 业务数据范围（0：无限制 1：本机构及下属机构 2：本机构 3：本部门及其下属部门 4：本部门 5：本人）
        /// </summary>
        public int? DataScope { get; set; }

        /// <summary>
        ///是否启用(Y：启用 N：关闭)
        /// </summary>
        public string XYBZ { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// 节点是否打开
        /// </summary>
        public bool open { set; get; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<SysMenu> children { get; set; }

        /// <summary>
        /// 菜单相关操作
        /// </summary>
        public List<MenuOption> MenuOptions { get; set; }
        public string MenuOptIds { get; set; }
        #endregion
    }
}