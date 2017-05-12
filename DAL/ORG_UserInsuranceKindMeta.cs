using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_UserInsuranceKindMetadata))]//使用ORG_UserInsuranceKindMetadata对ORG_UserInsuranceKind进行数据验证
    public partial class ORG_UserInsuranceKind 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "社保种类")]
        public string InsuranceKindIdOld { get; set; }
        
        #endregion

    }
    public partial class ORG_UserInsuranceKindMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "客服", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? UserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保种类", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceKindId { get; set; }


    }
}
 

