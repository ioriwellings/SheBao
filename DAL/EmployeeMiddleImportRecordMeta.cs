using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(EmployeeMiddleImportRecordMetadata))]//使用EmployeeMiddleImportRecordMetadata对EmployeeMiddleImportRecord进行数据验证
    public partial class EmployeeMiddleImportRecord 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class EmployeeMiddleImportRecordMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "下载路径", Order = 2)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object URL { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "导入条数", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ImportCount { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "导入金额", Order = 4)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object ImportPayment { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 5)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 6)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 7)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }


    }
}
 

