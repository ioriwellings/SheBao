using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyMetadata))]//使用CRM_CompanyMetadata对CRM_Company进行数据验证
    public partial class CRM_Company 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业编号", Order = 2)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object CompanyCode { get; set; }

            [Required(ErrorMessage="请填写{0}")]
            //[Remote("CheckCompanyName", "CRM_Company", ErrorMessage = "该公司名已存在")]
			[ScaffoldColumn(true)]
			[Display(Name = "企业名称", Order = 3)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CompanyName { get; set; }

            [Required(ErrorMessage = "请选择{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "所属行业", Order = 4)]
			[StringLength(20, ErrorMessage = "长度不可超过20")]
			public object Dict_HY_Code { get; set; }

		
			[ScaffoldColumn(true)]
			[Display(Name = "组织机构代码证", Order = 6)]
			[StringLength(100, ErrorMessage = "长度不可超过100")]
			public object OrganizationCode { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "企业注册地址", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object RegisterAddress { get; set; }

             [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "企业办公地址", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object OfficeAddress { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "数据来源（1：平台推送 2：系统录入）", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Source { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（1：启用 2：修改中）", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "关联的企业", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ParentID { get; set; }

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


    }
}
 

