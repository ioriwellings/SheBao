using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(PRD_ProductMetadata))]//使用PRD_ProductMetadata对PRD_Product进行数据验证
    public partial class PRD_Product 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class PRD_ProductMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "产品名称", Order = 2)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object ProductName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：停用 1：启用）", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }


    }
}
 

