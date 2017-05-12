using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CompanyEmployeeRelationMetadata))]//使用CompanyEmployeeRelationMetadata对CompanyEmployeeRelation进行数据验证
    public partial class CompanyEmployeeRelation 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CompanyEmployeeRelationMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EmployeeId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 4)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳地", Order = 10)]
			[StringLength(36, ErrorMessage = "长度不可超过36")]
			public object CityId { get; set; }


    }
}
 

