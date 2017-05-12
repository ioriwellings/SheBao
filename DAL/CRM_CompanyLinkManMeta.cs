using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyLinkManMetadata))]//使用CRM_CompanyLinkManMetadata对CRM_CompanyLinkMan进行数据验证
    public partial class CRM_CompanyLinkMan 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyLinkManMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "姓名", Order = 3)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object LinkManName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "职务", Order = 4)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Position { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "联系人地址", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Address { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "手机号", Order = 6)]
            [RegularExpression(@"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$", ErrorMessage = "{0}不正确")]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Mobile { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "固话", Order = 7)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Telephone { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "邮箱", Order = 8)]
            [RegularExpression(@"(\w)+(\.\w+)*@(\w)+((\.\w+)+)",ErrorMessage = "{0}格式不正确")]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object Email { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 9)]
			[StringLength(500, ErrorMessage = "长度不可超过500")]
			public object Remark { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "默认联系人", Order = 13)]
            public object IsDefault { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：停用 1：启用 2：修改中）", Order = 13)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 14)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

