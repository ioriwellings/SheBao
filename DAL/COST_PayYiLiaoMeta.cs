using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_PayYiLiaoMetadata))]//使用COST_PayYiLiaoMetadata对COST_PayYiLiao进行数据验证
    public partial class COST_PayYiLiao 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_PayYiLiaoMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "社保支出导入记录", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? COST_PayRecordStatusID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "姓名", Order = 3)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object EmployeeName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件号码", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴纳社保月", Order = 5)]
			[StringLength(6, ErrorMessage = "长度不可超过6")]
			public object PaymentSocialMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "年月", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? YearMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "缴费基数", Order = 7)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object Radix { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "个人金额", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业金额", Order = 9)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCost { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "合计", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object Total { get; set; }

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

			[ScaffoldColumn(true)]
			[Display(Name = "社保局证件号码", Order = 15)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateNumberSB { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "单位编码", Order = 16)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyId { get; set; }


    }
}
 

