using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(SupplierLinkManMetadata))]//使用SupplierLinkManMetadata对SupplierLinkMan进行数据验证
    public partial class SupplierLinkMan 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "供应商")]
        public string SupplierIdOld { get; set; }
        
        #endregion

    }
    public partial class SupplierLinkManMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

            [Required(ErrorMessage="请填写{0}")]
			[ScaffoldColumn(true)]
			[Display(Name = "姓名", Order = 2)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Name { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "供应商", Order = 3)]
			public object SupplierId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "职务", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Position { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "联系人地址", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Address { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "手机号", Order = 6)]
            [RegularExpression(@"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$", ErrorMessage = "{0}不正确")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Mobile { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "固话", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Telephone { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "邮箱", Order = 8)]
            [RegularExpression(@"(\w)+(\.\w+)*@(\w)+((\.\w+)+)", ErrorMessage = "{0}格式不正确")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Email { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 9)]
			[StringLength(2000, ErrorMessage = "长度不可超过2000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "默认联系人", Order = 10)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsDefault { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 13)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 14)]
            [StringLength(200, ErrorMessage = "长度不可超过200")]
			public int? Status { get; set; }


    }
}
 

