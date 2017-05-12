using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_PayRecordStatusMetadata))]//使用COST_PayRecordStatusMetadata对COST_PayRecordStatus进行数据验证
    public partial class COST_PayRecordStatus 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_PayRecordStatusMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "险种", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CostType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：已锁定 1：未锁定）", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用_社保支出导入主表", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? COST_PayRecordId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "单位合计", Order = 5)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人合计", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "导入总条数", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? AllCount { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人Id", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 10)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }


    }
}
 

