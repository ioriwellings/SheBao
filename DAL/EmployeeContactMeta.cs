using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeContactMetadata))]//使用EmployeeContactMetadata对EmployeeContact进行数据验证
    public partial class EmployeeContact 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "员工")]
        public string EmployeeIdOld { get; set; }
        
        #endregion

    }
    public partial class EmployeeContactMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "固定电话", Order = 2)]
            [RegularExpression(@"(0[0-9]{2,3})-([2-9][0-9]{6,7})|(0[0-9]{2,3})([2-9][0-9]{6,7})$", ErrorMessage = "{0}不正确")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Telephone { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "移动电话", Order = 3)]
            [Required(ErrorMessage = "移动电话不能为空")]
            [RegularExpression(@"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$", ErrorMessage = "{0}不正确")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object MobilePhone { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "邮箱", Order = 4)]
            [RegularExpression(@"(\w)+(\.\w+)*@(\w)+((\.\w+)+)", ErrorMessage = "{0}格式不正确")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Email { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "地址", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Address { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EmployeeId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 7)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 9)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 10)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 11)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

