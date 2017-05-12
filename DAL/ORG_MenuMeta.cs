using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(ORG_MenuMetadata))]//使用ORG_MenuMetadata对ORG_Menu进行数据验证
    public partial class ORG_Menu 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class ORG_MenuMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "菜单名称", Order = 2)]
			[StringLength(50, ErrorMessage = "长度不可超过50")]
			public object MenuName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "父菜单", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? ParentID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "菜单地址", Order = 4)]
			[StringLength(300, ErrorMessage = "长度不可超过300")]
			public object MenuUrl { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否拥有部门范围权限配置功能（Y：拥有 N：不拥有）", Order = 5)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object DepartmentScopeAuthority { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否拥有部门业务权限配置功能(Y：拥有 N：不拥有)", Order = 6)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object DepartmentAuthority { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "层级", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? NodeLevel { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "排序（从小到大）", Order = 8)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? Sort { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否显示导航链接", Order = 9)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object IsDisplay { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "是否启用(Y：启用 N：关闭)", Order = 10)]
			[StringLength(1, ErrorMessage = "长度不可超过1")]
			public object XYBZ { get; set; }


    }
}
 

