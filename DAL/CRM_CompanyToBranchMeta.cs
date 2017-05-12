using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyToBranchMetadata))]//使用CRM_CompanyToBranchMetadata对CRM_CompanyToBranch进行数据验证
    public partial class CRM_CompanyToBranch 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyToBranchMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工客服", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? UserID_YG { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "销售", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? UserID_XS { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "责任客服", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? UserID_ZR { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：停用 1：启用 2：修改中）", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

