using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_GroupMetadata))]//使用ORG_GroupMetadata对ORG_Group进行数据验证
    public partial class ORG_Group 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_GroupMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "ID", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "Code", Order = 2)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object Code { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "GroupName", Order = 3)]
			[Required(ErrorMessage = "不能为空")]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object GroupName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "Des", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Des { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "XYBZ", Order = 5)]
			[Required(ErrorMessage = "不能为空")]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object XYBZ { get; set; }


    }
}
 

