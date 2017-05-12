using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(SupplierMetadata))]//使用SupplierMetadata对Supplier进行数据验证
    public partial class Supplier 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class SupplierMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "编号", Order = 2)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Code { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "名称", Order = 3)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Name { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "组织机构代码证", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object OrganizationCode { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "注册地址", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object RegisterAddress { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "办公地址", Order = 6)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object OfficeAddress { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "客服", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CustomerServiceId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 8)]
            [StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 9)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 11)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }


    }
}
 

