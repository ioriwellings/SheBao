using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_Company_AuditMetadata))]//使用CRM_Company_AuditMetadata对CRM_Company_Audit进行数据验证
    public partial class CRM_Company_Audit 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_Company_AuditMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业编号", Order = 3)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object CompanyCode { get; set; }

            [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "企业名称", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CompanyName { get; set; }

            [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "所属行业", Order = 5)]
			[StringLength(20, ErrorMessage = "长度不可超过20")]
			public object Dict_HY_Code { get; set; }

		
			[ScaffoldColumn(true)]
			[Display(Name = "组织机构代码证", Order = 7)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object OrganizationCode { get; set; }

            [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "企业注册地址", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object RegisterAddress { get; set; }

            [Required]
			[ScaffoldColumn(true)]
			[Display(Name = "企业办公地址", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object OfficeAddress { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "数据来源（1：平台推送 2：系统录入）", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Source { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 12)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 13)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 14)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作状态（0：失败 1：待处理 2：成功）", Order = 15)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作节点（1：销售经理 2：质控）", Order = 16)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateNode { get; set; }


    }
}
 

