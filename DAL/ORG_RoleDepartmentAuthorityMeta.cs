using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_RoleDepartmentAuthorityMetadata))]//使用ORG_RoleDepartmentAuthorityMetadata对ORG_RoleDepartmentAuthority进行数据验证
    public partial class ORG_RoleDepartmentAuthority 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_RoleDepartmentAuthorityMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "角色", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ORG_Role_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "系统_菜单", Order = 3)]
			[StringLength(20, ErrorMessage = "长度不可超过20")]
			public object ORG_Menu_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "权限部门", Order = 4)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object ORG_Department_ID_List { get; set; }


    }
}
 

