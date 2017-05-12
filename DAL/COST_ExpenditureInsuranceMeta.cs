using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_ExpenditureInsuranceMetadata))]//使用COST_ExpenditureInsuranceMetadata对COST_ExpenditureInsurance进行数据验证
    public partial class COST_ExpenditureInsurance 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_ExpenditureInsuranceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "姓名", Order = 2)]
			public object 姓名 { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件号码", Order = 3)]
			public object IDCard { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴费社保月", Order = 4)]
			public object 缴费社保月 { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴费基数", Order = 5)]
			public object 缴费基数 { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "单位承担", Order = 6)]
			public object 单位承担 { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人承担", Order = 7)]
			public object 个人承担 { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保月", Order = 8)]
			public object 社保月 { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（费用已生成，客服处理中，待客户确认，客户已确认，责任客服作废，财务作废，客户作废，责任客服锁定，账单已支付，已核销）", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 12)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 13)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 14)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

