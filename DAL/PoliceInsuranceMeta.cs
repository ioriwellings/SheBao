using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(PoliceInsuranceMetadata))]//使用PoliceInsuranceMetadata对PoliceInsurance进行数据验证
    public partial class PoliceInsurance 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "社保种类")]
        public string InsuranceKindIdOld { get; set; }
        
        #endregion

    }
    public partial class PoliceInsuranceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "名称", Order = 2)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Name { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保种类", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceKindId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "开始时间", Order = 4)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

			public DateTime? StartTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "结束时间", Order = 5)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

			public DateTime? EndTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "允许补缴月数", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? MaxPayMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "报增社保月", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceAdd { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "报减社保月", Order = 8)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceReduce { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业比例", Order = 9)]
			public object CompanyPercent { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业最低基数", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyLowestNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工最低基数", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object EmployeeLowestNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业最高基数", Order = 12)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyHighestNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工最高基数", Order = 13)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object EmployeeHighestNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工比例", Order = 14)]
			public object EmployeePercent { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "默认", Order = 15)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsDefault { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 16)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 17)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 18)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 19)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 20)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 21)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

