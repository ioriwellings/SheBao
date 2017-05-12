using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(PoliceMasterRelationshipMetadata))]//使用PoliceMasterRelationshipMetadata对PoliceMasterRelationship进行数据验证
    public partial class PoliceMasterRelationship 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "社保种类")]
        public string InsuranceKindIdOld { get; set; }
        
        #endregion

    }
    public partial class PoliceMasterRelationshipMetadata
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
			[Display(Name = "标签", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Tag { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "类型", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Style { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "主险", Order = 6)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsMaster { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 7)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 9)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 10)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

