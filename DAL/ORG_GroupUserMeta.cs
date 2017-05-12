using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_GroupUserMetadata))]//使用ORG_GroupUserMetadata对ORG_GroupUser进行数据验证
    public partial class ORG_GroupUser 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_GroupUserMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "ID", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "ORG_Group_ID", Order = 2)]
			[Required(ErrorMessage = "不能为空")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ORG_Group_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "ORG_User_ID", Order = 3)]
			[Required(ErrorMessage = "不能为空")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ORG_User_ID { get; set; }


    }
}
 

