using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyBankAccount_AuditMetadata))]//使用CRM_CompanyBankAccount_AuditMetadata对CRM_CompanyBankAccount_Audit进行数据验证
    public partial class CRM_CompanyBankAccount_Audit 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyBankAccount_AuditMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业银行账户", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_CompanyBankAccount_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "待审核企业", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_Audit_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "开户行名称", Order = 5)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Bank { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "银行账号", Order = 6)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Account { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 7)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 8)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作状态（0：失败 1：待处理 2：成功）", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作节点（1：销售经理 2：质控）", Order = 12)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateNode { get; set; }


    }
}
 

