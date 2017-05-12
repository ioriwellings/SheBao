using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyContract_AuditMetadata))]//使用CRM_CompanyContract_AuditMetadata对CRM_CompanyContract_Audit进行数据验证
    public partial class CRM_CompanyContract_Audit 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyContract_AuditMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业合同信息", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_CompanyContract_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "待审核企业", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_Audit_ID { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "账单日", Order = 5)]
            [RegularExpression(@"^((0?[1-9])|((1|2)[0-9])|30|31)$", ErrorMessage = "{0}不能够大于31日")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BillDay { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "回款日", Order = 6)]
            [RegularExpression(@"^((0?[1-9])|((1|2)[0-9])|30|31)$", ErrorMessage = "{0}不能够大于31日")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ReceivedDay { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "结费周期(月度、季度、半年、年度)", Order = 7)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object FeesCycle { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "变动截止日", Order = 8)]
            [RegularExpression(@"^((0?[1-9])|((1|2)[0-9])|30|31)$", ErrorMessage = "{0}不能够大于31日")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ChangeDay { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "资料交付日", Order = 9)]
            [RegularExpression(@"^((0?[1-9])|((1|2)[0-9])|30|31)$", ErrorMessage = "{0}不能够大于31日")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? DatumDay { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "服务截止日", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
			public DateTime? ServceEndDay { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "发送账单日", Order = 11)]
            [RegularExpression(@"^((0?[1-9])|((1|2)[0-9])|30|31)$", ErrorMessage = "{0}不能够大于31日")]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? SendBillDay { get; set; }

          [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "服务起始日", Order = 12)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
			public DateTime? ServiceBeginDay { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 13)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 14)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 15)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 16)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作状态（0：失败 1：待处理 2：成功）", Order = 17)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作节点（1：销售经理 2：质控）", Order = 18)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateNode { get; set; }


    }
}
 

