using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyLadderPriceMetadata))]//使用CRM_CompanyLadderPriceMetadata对CRM_CompanyLadderPrice进行数据验证
    public partial class CRM_CompanyLadderPrice 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyLadderPriceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

            [ScaffoldColumn(true)]
            [Required(ErrorMessage = "请选择{0}")]
			[Display(Name = "产品", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PRD_Product_ID { get; set; }

            [ScaffoldColumn(true)]
            [Required(ErrorMessage = "请填写{0}")]
			[Display(Name = "单人服务费", Order = 4)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object SinglePrice { get; set; }

            [ScaffoldColumn(true)]
            [Required(ErrorMessage = "请填写{0}")]
			[Display(Name = "终止阶梯", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EndLadder { get; set; }

            [ScaffoldColumn(true)]
            [Required(ErrorMessage = "请填写{0}")]
			[Display(Name = "起始阶梯", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BeginLadder { get; set; }

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
 

