using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(SupplierBillMetadata))]//使用SupplierBillMetadata对SupplierBill进行数据验证
    public partial class SupplierBill 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "供应商")]
        public string SupplierIdOld { get; set; }
        
        #endregion

    }
    public partial class SupplierBillMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "供应商", Order = 2)]
			public object SupplierId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "付款方名称", Order = 3)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PayName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "开票名称", Order = 4)]
            [Required(ErrorMessage = "开票名称不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object BillName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "税务登记证号", Order = 5)]
            [Required(ErrorMessage = "税务登记证号不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object TaxRegistryNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 9)]
            [StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Status { get; set; }


    }
}
 

