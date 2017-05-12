using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_UserCityMetadata))]//使用ORG_UserCityMetadata对ORG_UserCity进行数据验证
    public partial class ORG_UserCity 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "缴纳地")]
        public string CityIdOld { get; set; }
        
        #endregion

    }
    public partial class ORG_UserCityMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "客服", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? UserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳地", Order = 3)]
			[StringLength(36, ErrorMessage = "长度不可超过36")]
			public object CityId { get; set; }


    }
}
 

