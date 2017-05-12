using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(PoliceOperationPoliceInsurancePoliceAccountNature2Metadata))]//使用PoliceOperationPoliceInsurancePoliceAccountNature2Metadata对PoliceOperationPoliceInsurancePoliceAccountNature2进行数据验证
    public partial class PoliceOperationPoliceInsurancePoliceAccountNature2 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class PoliceOperationPoliceInsurancePoliceAccountNature2Metadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保政策", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PoliceInsuranceId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "政策手续", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PoliceOperationId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "户口性质", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PoliceAccountNatureId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否企业计算", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsCompany { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "公式", Order = 6)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Expression { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 7)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "工本费", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object FeeCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 11)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 12)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 13)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

