using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_CostTableMetadata))]//使用COST_CostTableMetadata对COST_CostTable进行数据验证
    public partial class COST_CostTable 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_CostTableMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用表类型（1：单立户 2：大户代理）", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CostTableType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "收费总额", Order = 3)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object ChargeCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "实际服务总额", Order = 4)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object ServiceCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 5)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（1：待责任客服验证 2：待客户确认 3：客户作废 4：待责任客服确认 5：责任客服作废 6：待核销 7：财务作废 8：已核销 9：已支付）", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 10)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "数据来源", Order = 12)]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
            public int? CreateFrom { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "供应商", Order = 13)]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
            public int? Suppler_ID { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "是否最低报价", Order = 14)]
            [StringLength(200, ErrorMessage = "长度不可超过200")]
            public object IsLadderPrice { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "费用月", Order = 15)]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
            public int? YearMonth { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "费用表编号", Order = 14)]
            [StringLength(36, ErrorMessage = "长度不可超过36")]
            public object SerialNumber { get; set; }

    }
}
 

