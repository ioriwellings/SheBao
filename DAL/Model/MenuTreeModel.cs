using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    /// <summary>
    /// 菜单树模型
    /// </summary>
    public class MenuTreeModel
    {
        public int ID { get; set; }
        public string MenuName { get; set; }
        public int ParentID { get; set; }
        public string MenuUrl { get; set; }
        public string DepartmentScopeAuthority { get; set; }
        public string DepartmentAuthority { get; set; }
        public int NodeLevel { get; set; }
        public int Sort { get; set; }
        public string IsDisplay { get; set; }
        public string XYBZ { get; set; }
        public int _parentId { get; set; }  // 上级菜单编号（用于构建树）
        public string ParentName { get; set; }  // 上级菜单名称

    };
}
