using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(UserCityCompanyMetadata))]//使用UserCityCompanyMetadata对UserCityCompany进行数据验证
    public partial class UserCityCompany 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class UserCityCompanyMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳地", Order = 2)]
			[StringLength(36, ErrorMessage = "长度不可超过36")]
			public object CityId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工客服", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? UserID_YG { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }


    }
}
 

