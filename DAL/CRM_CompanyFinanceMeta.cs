using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyFinanceMetadata))]//使用CRM_CompanyFinanceMetadata对CRM_CompanyFinance进行数据验证
    public partial class CRM_CompanyFinance 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyFinanceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "财务类型（1：开票名称 2：付款方名称）", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? FinanceType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "名称", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object FinanceName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 5)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：停用 1：启用 2：修改中）", Order = 8)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

