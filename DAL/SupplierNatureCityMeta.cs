using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(SupplierNatureCityMetadata))]//使用SupplierNatureCityMetadata对SupplierNatureCity进行数据验证
    public partial class SupplierNatureCity 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "供应商")]
        public string SupplierIdOld { get; set; }
        
        #endregion

    }
    public partial class SupplierNatureCityMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳地", Order = 2)]
			[StringLength(36, ErrorMessage = "长度不可超过36")]
			public object NatureCityId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "供应商", Order = 3)]
		
			public int SupplierId { get; set; }


    }
}
 

