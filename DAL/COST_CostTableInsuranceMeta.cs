using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_CostTableInsuranceMetadata))]//使用COST_CostTableInsuranceMetadata对COST_CostTableInsurance进行数据验证
    public partial class COST_CostTableInsurance 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_CostTableInsuranceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用表", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? COST_CostTable_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴费类型（1：正常 2：退费 3：补收 4：补缴 5：补差 6：调整）", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PaymentStyle { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳区间", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentInterval { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳社保月", Order = 5)]
			[StringLength(6, ErrorMessage = "长度不可超过6")]
			public object PaymentSocialMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳月数", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PaymentMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业缴费基数", Order = 7)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyRadix { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业金额", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业缴纳比例(百分比)", Order = 9)]
			public object CompanyRatio { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人缴费基数", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonRadix { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人金额", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人缴纳比例(百分比)", Order = 12)]
			public object PersonRatio { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "费用类型（1：养老 2：医疗 3：生育 4：工伤 5：失业6：公积金）", Order = 13)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CostType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工", Order = 14)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Employee_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工姓名", Order = 15)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object EmployName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件类型", Order = 16)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件号码", Order = 17)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 18)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态（0：作废 1：正常 2：已经修改3：待确认）", Order = 19)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "关联的社保明细", Order = 20)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ParentID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 21)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 22)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 23)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 24)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 25)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }


    }
}
 

