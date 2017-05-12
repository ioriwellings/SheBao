using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_RoleMetadata))]//使用ORG_RoleMetadata对ORG_Role进行数据验证
    public partial class ORG_Role 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_RoleMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "角色编码", Order = 2)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object RoleCode { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "角色名称", Order = 3)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object RoleName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "角色描述", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Des { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "选用标志(Y：启用 N：删除)", Order = 5)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object XYBZ { get; set; }


    }
}
 

