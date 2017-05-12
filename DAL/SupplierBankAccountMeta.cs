using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(SupplierBankAccountMetadata))]//使用SupplierIdBankAccountMetadata对SupplierIdBankAccount进行数据验证
    public partial class SupplierBankAccount
    {

        #region 自定义属性，即由数据实体扩展的实体

        [Display(Name = "供应商")]
        public string SupplierIdOld { get; set; }

        #endregion

    }
    public partial class SupplierBankAccountMetadata
    {
        [ScaffoldColumn(false)]
        [Display(Name = "主键", Order = 1)]
        public object Id { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "供应商", Order = 2)]
        [StringLength(36, ErrorMessage = "长度不可超过36")]
        public object SupplierId { get; set; }


        [Required(ErrorMessage = "开户行名称不能为空")]
        [ScaffoldColumn(true)]
        [Display(Name = "开户行名称", Order = 3)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public object Bank { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "银行账号", Order = 4)]
        [Required(ErrorMessage = "账号不能为空")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "{0}不正确")]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public object Account { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "状态", Order = 5)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public object Status { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "创建时间", Order = 6)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime, ErrorMessage = "时间格式不正确")]
        public DateTime? CreateTime { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "创建人ID", Order = 7)]
        [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
        public int? CreateUserID { get; set; }

        [ScaffoldColumn(true)]
        [Display(Name = "创建人名称", Order = 8)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public object CreateUserName { get; set; }


    }
}


