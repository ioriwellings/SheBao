using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_Company_InsuranceMetadata))]//使用CRM_Company_InsuranceMetadata对CRM_Company_Insurance进行数据验证
    public partial class CRM_Company_Insurance 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_Company_InsuranceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "ID", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "公司ID", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保种类", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceKind { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "账号", Order = 4)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object Account { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "缴纳地", Order = 5)]
            [StringLength(20, ErrorMessage = "长度不可超过20")]
            public object City { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 7)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 9)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object UpdatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 10)]
			[StringLength(10, ErrorMessage = "长度不可超过10")]
			public object State { get; set; }


    }
}
 

