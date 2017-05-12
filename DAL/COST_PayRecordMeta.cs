using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_PayRecordMetadata))]//使用COST_PayRecordMetadata对COST_PayRecord进行数据验证
    public partial class COST_PayRecord 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_PayRecordMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳社保月", Order = 2)]
			[StringLength(6, ErrorMessage = "长度不可超过6")]
			public object PaymentSocialMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "年月", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? YearMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 4)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 6)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

