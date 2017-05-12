using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyPriceMetadata))]//使用CRM_CompanyPriceMetadata对CRM_CompanyPrice进行数据验证
    public partial class CRM_CompanyPrice 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyPriceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

            [ScaffoldColumn(true)]
            [Required]
			[Display(Name = "产品", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PRD_Product_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "报价类型(0：固定价格 1：阶梯价格)", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PriceType { get; set; }

            [ScaffoldColumn(true)]
            [Required]
			[Display(Name = "整户服务费", Order = 5)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object LowestPrice { get; set; }

            [ScaffoldColumn(true)]
            [Required]
			[Display(Name = "补缴服务费", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object AddPrice { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：停用 1：启用 2：修改中）", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 8)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

