using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeBankMetadata))]//使用EmployeeBankMetadata对EmployeeBank进行数据验证
    public partial class EmployeeBank 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "员工")]
        public string EmployeeIdOld { get; set; }
        
        #endregion

    }
    public partial class EmployeeBankMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "开户名称", Order = 2)]
            //[Required(ErrorMessage = "开户名称不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
            
			public object AccountName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "开户银行", Order = 3)]
            //[Required(ErrorMessage = "开户银行不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Bank { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "支行名称", Order = 4)]
            //[Required(ErrorMessage = "支行名称不能为空")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object BranchBank { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "账号", Order = 5)]
            //[Required(ErrorMessage = "账号不能为空")]
            [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "{0}不正确")]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Account { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "默认", Order = 6)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object IsDefault { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "员工", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EmployeeId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "备注", Order = 8)]
			[StringLength(4000, ErrorMessage = "长度不可超过4000")]
			public object Remark { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 9)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object State { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人", Order = 11)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreatePerson { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改时间", Order = 12)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? UpdateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "修改人", Order = 13)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object UpdatePerson { get; set; }


    }
}
 

