using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeAddMetadata))]//使用EmployeeAddMetadata对EmployeeAdd进行数据验证
        public partial class EmployeeAdd 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "供应商")]
        public string SuppliersIdOld { get; set; }
        
        [Display(Name = "企业员工关系")]
        public string CompanyEmployeeRelationIdOld { get; set; }
        
        [Display(Name = "社保政策")]
        public string PoliceInsuranceIdOld { get; set; }
        
        [Display(Name = "户口性质")]
        public string PoliceAccountNatureIdOld { get; set; }
        
        [Display(Name = "政策手续")]
        public string PoliceOperationIdOld { get; set; }
        
        #endregion

    }
    public partial class EmployeeAddMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "工资", Order = 2)]
			public object Wage { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保种类", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceKindId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业员工关系", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyEmployeeRelationId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保政策", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PoliceInsuranceId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保编号", Order = 6)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object InsuranceCode { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "政策手续", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PoliceOperationId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保月", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

			public DateTime? InsuranceMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "起缴时间", Order = 9)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

			public DateTime? StartTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "户口性质", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PoliceAccountNatureId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "供应商", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? SuppliersId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否单立户", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsIndependentAccount { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 13)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 14)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 15)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 16)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 17)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 18)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

