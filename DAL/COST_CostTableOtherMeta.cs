using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_CostTableOtherMetadata))]//使用COST_CostTableOtherMetadata对COST_CostTableOther进行数据验证
    public partial class COST_CostTableOther 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_CostTableOtherMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用表", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? COST_CostTable_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴费类型（1：正常 2：退费 3：补收 4：补缴 5：补差 6：调整）", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PaymentStyle { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "收费金额", Order = 4)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object ChargeCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用类型（1：其他费用（残保金、工会费） 2：其他社保费用（滞纳金） 3：工本费）", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CostType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Employee_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工姓名", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object EmployName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件类型", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件号码", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 10)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：作废 1：正常 2：已经修改3：待确认）", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "关联的其他费用", Order = 12)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ParentID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 13)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 14)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 15)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 16)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 17)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

