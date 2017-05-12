using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_UserRoleMetadata))]//使用ORG_UserRoleMetadata对ORG_UserRole进行数据验证
    public partial class ORG_UserRole 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_UserRoleMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "人员", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ORG_User_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "角色", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ORG_Role_ID { get; set; }


    }
}
 

