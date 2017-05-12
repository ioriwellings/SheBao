using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeMetadata))]//使用EmployeeMetadata对Employee进行数据验证
    public partial class Employee 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class EmployeeMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "姓名", Order = 2)]
            [RegularExpression(@"^[A-Za-z\u4e00-\u9fa5]*$", ErrorMessage = "{0}只能输入汉字和字母")]//只能输入汉字和字母
            [Required(ErrorMessage = "姓名不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Name { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "标签", Order = 3)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Tag { get; set; }

			[ScaffoldColumn(true)]
            //[Required(ErrorMessage = "请选择{0}")]
			[Display(Name = "证件类型", Order = 4)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "证件号码", Order = 5)]
            [Required(ErrorMessage = "证件号码不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CertificateNumber { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "性别", Order = 6)]
            //[Required(ErrorMessage = "请选择性别")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Sex { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "生日", Order = 7)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

			public DateTime? Birthday { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "附件", Order = 8)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Attachments { get; set; }

			[ScaffoldColumn(true)]
            //[Required(ErrorMessage = "请选择{0}")]
			[Display(Name = "户口类型", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object AccountType { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 10)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 11)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 12)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 13)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 14)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 15)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

