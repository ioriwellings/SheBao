using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(COST_PayTemporaryMetadata))]//使用COST_PayTemporaryMetadata对COST_PayTemporary进行数据验证
    public partial class COST_PayTemporary 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class COST_PayTemporaryMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "年月", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? YearMonth { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 3)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "姓名", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PersonName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "身份证号", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CardId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工编号", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? PersonId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业编号", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业名称", Order = 11)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CompanyName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "供应商", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Suppliers { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "保险缴纳地", Order = 13)]
			[StringLength(36, ErrorMessage = "长度不可超过36")]
			public object CityId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "养老缴纳社保月", Order = 14)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentSocialMonthYL { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "养老缴费基数", Order = 15)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object RadixYL { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "养老企业", Order = 16)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCosYL { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "养老个人", Order = 17)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCostYL { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "失业缴纳社保月", Order = 18)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentSocialMonthSY { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "失业缴费基数", Order = 19)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object RadixSY { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "失业企业", Order = 20)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostSY { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "失业个人", Order = 21)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCostSY { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "工伤缴纳社保月", Order = 22)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentSocialMonthGS { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "工伤缴费基数", Order = 23)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object RadixGS { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "工伤企业", Order = 24)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostGS { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "医疗缴纳社保月", Order = 25)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentSocialMonthYiL { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "医疗缴费基数", Order = 26)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object RadixYil { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "医疗企业", Order = 27)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostYil { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "医疗个人", Order = 28)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCostYil { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "医疗大额企业", Order = 29)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostYilMax { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "医疗大额个人", Order = 30)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCostYilMax { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "生育企业", Order = 31)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostShengY { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "公积金缴纳社保月", Order = 32)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentSocialMonthGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "公积金缴费基数", Order = 33)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object RadixGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "公积金企业", Order = 34)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "公积金个人", Order = 35)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCostGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "补充公积金缴纳社保月", Order = 36)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentSocialMonthBCGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "补充公积金缴费基数", Order = 37)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object RadixBCGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "补充公积金企业", Order = 38)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object CompanyCostBCGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "补充公积金个人", Order = 39)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PersonCostBCGJJ { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "其他社保费", Order = 40)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PayOtherSocial { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "其他费用", Order = 41)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PayOther { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "工本费", Order = 42)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object PayFee { get; set; }


    }
}
 

