using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;

namespace Langben.BLL
{
    /// <summary>
    /// 组织机构_角色操作权限 
    /// </summary>
    public partial class ORG_RoleMenuOpAuthorityBLL : IBLL.IORG_RoleMenuOpAuthorityBLL, IDisposable
    {
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
            return repository.ConfigAuthority(id, type, isEditOpt, isEditDpt, isEditData, selectedMenuOpt, selectedMenuDpt, selectedMenuScope);
        }
        public bool DeleteRoleOp( int[] ids)
        {
           return  repository.DeleteRoleOp( ids);
        }
    }
}

