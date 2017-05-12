using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeMiddleMetadata))]//使用EmployeeMiddleMetadata对EmployeeMiddle进行数据验证
    public partial class EmployeeMiddle 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class EmployeeMiddleMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保种类", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? InsuranceKindId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业员工关系", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyEmployeeRelationId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用期间", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentBetween { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用类型", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PaymentStyle { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "单位基数", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyBasePayment { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "单位承担金额", Order = 7)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyPayment { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人基数", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object EmployeeBasePayment { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人承担金额", Order = 9)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object EmployeePayment { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳月数", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PaymentMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "开始时间段", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? StartDate { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "结束时间段", Order = 12)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EndedDate { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "使用期间", Order = 13)]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
            public int? UseBetween { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属账单", Order = 14)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object BillId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 15)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 16)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 17)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 18)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 19)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 20)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

