using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyPrice_AuditMetadata))]//使用CRM_CompanyPrice_AuditMetadata对CRM_CompanyPrice_Audit进行数据验证
    public partial class CRM_CompanyPrice_Audit 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyPrice_AuditMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业报价", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_CompanyPrice_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "待审核企业", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_Audit_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

            [ScaffoldColumn(true)]
            [Required(ErrorMessage = "请选择{0}")]
			[Display(Name = "产品", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PRD_Product_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "报价类型(0：固定价格 1：阶梯价格)", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PriceType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "整户服务费", Order = 7)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object LowestPrice { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "补缴服务费", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object AddPrice { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作状态（0：失败 1：待处理 2：成功）", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作节点（1：销售经理 2：质控）", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateNode { get; set; }


    }
}
 

