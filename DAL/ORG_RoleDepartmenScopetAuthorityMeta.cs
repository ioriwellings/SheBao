using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_RoleDepartmenScopetAuthorityMetadata))]//使用ORG_RoleDepartmenScopetAuthorityMetadata对ORG_RoleDepartmenScopetAuthority进行数据验证
    public partial class ORG_RoleDepartmenScopetAuthority 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_RoleDepartmenScopetAuthorityMetadata
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
			[Display(Name = "部门范围（0：无限制 1：本机构及下属机构 2：本机构 3：本部门及其下属部门 4：本部门 5：本人）", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? DepartmentScope { get; set; }


    }
}
 

