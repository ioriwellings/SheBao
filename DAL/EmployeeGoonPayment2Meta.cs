using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeGoonPayment2Metadata))]//使用EmployeeGoonPayment2Metadata对EmployeeGoonPayment2进行数据验证
    public partial class EmployeeGoonPayment2 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "增加员工")]
        public string EmployeeAddIdOld { get; set; }
        
        #endregion

    }
    public partial class EmployeeGoonPayment2Metadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保月", Order = 2)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

			public DateTime? InsuranceMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "旧工资", Order = 3)]
			public object OldWage { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "新工资", Order = 4)]
			public object NewWage { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "增加员工", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EmployeeAddId { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "社保种类", Order = 6)]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
            public int? InsuranceKindId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否调基月", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsChangeMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 8)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

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

            [ScaffoldColumn(true)]
            [Display(Name = "缴纳地", Order = 14)]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
            public int? CityId { get; set; }
    }
}
 

