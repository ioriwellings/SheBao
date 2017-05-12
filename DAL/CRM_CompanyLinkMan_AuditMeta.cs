using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyLinkMan_AuditMetadata))]//使用CRM_CompanyLinkMan_AuditMetadata对CRM_CompanyLinkMan_Audit进行数据验证
    public partial class CRM_CompanyLinkMan_Audit 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyLinkMan_AuditMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业联系人", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_CompanyLinkMan_ID { get; set; }

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
			[Display(Name = "姓名", Order = 5)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object LinkManName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "职务", Order = 6)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Position { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "联系人地址", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Address { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "手机号", Order = 8)]
            [RegularExpression(@"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$", ErrorMessage = "{0}不正确")]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Mobile { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "固话", Order = 9)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Telephone { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "邮箱", Order = 10)]
            [RegularExpression(@"(\w)+(\.\w+)*@(\w)+((\.\w+)+)", ErrorMessage = "{0}格式不正确")]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Email { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 11)]
			[StringLength(500, ErrorMessage = "长度不可超过500")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 12)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 13)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 14)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 15)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作状态（0：失败 1：待处理 2：成功）", Order = 16)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作节点（1：销售经理 2：质控）", Order = 17)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateNode { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "默认联系人", Order = 18)]
            public object IsDefault { get; set; }


    }
}
 

