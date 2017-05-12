using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_MenuOpMetadata))]//使用ORG_MenuOpMetadata对ORG_MenuOp进行数据验证
    public partial class ORG_MenuOp 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_MenuOpMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
            [Required(ErrorMessage = "不能为空")]
            //[RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "{0}不正确")]
            [StringLength(20, ErrorMessage = "长度不可超过20")]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属菜单", Order = 2)]
            [Required(ErrorMessage = "所属菜单不能为空")]
            [Range(0, 2147483646, ErrorMessage = "数值超出范围")]
			public object ORG_Menu_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "功能名称", Order = 3)]
            [Required(ErrorMessage = "功能名称不能为空")]
			[StringLength(20, ErrorMessage = "长度不可超过20")]
			public object MenuOpName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "排序（从小到大）", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Sort { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否启用(Y：启用 N：关闭)", Order = 5)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object XYBZ { get; set; }


    }
}
 

