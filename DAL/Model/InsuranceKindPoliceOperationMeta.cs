using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(InsuranceKindPoliceOperationMetadata))]//使用EmployeeAddMetadata对EmployeeAdd进行数据验证
    public partial class InsuranceKindPoliceOperation
    {
      
        #region 自定义属性，即由数据实体扩展的实体

        [Display(Name = "社保种类")]
        public int? InsuranceKindId { get; set; }

        [Display(Name = "政策手续")]
        public int? PoliceOperationId { get; set; }        
        
        #endregion

    }
    public partial class InsuranceKindPoliceOperationMetadata
    {


        [ScaffoldColumn(true)]
        [Display(Name = "社保种类", Order = 1)]
        public int? InsuranceKindId { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "政策手续", Order = 2)]
        public int? PoliceOperationId { get; set; }

    }
}
 

