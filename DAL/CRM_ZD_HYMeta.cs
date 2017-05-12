using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_ZD_HYMetadata))]//使用CRM_ZD_HYMetadata对CRM_ZD_HY进行数据验证
    public partial class CRM_ZD_HY 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_ZD_HYMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "行业代码", Order = 1)]
			public object Code { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "行业名称", Order = 2)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object HYMC { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "行业全称", Order = 3)]
			[StringLength(400, ErrorMessage = "长度不可超过400")]
			public object HYQC { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "上级行业代码", Order = 4)]
			[StringLength(4, ErrorMessage = "长度不可超过4")]
			public object ParentCode { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "层级", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? NodeLevel { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "选用标识(Y:启用 N：停用)", Order = 6)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object XYBZ { get; set; }


    }
}
 

