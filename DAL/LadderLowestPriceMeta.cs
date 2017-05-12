using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(LadderLowestPriceMetadata))]//使用LadderLowestPriceMetadata对LadderLowestPrice进行数据验证
    public partial class LadderLowestPrice 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "产品")]
        public string ProductIdOld { get; set; }
        
        #endregion

    }
    public partial class LadderLowestPriceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CompanyId { get; set; }

			public object SupplierId { get; set; }

			public object ProductId { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "整户服务费", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object LowestPrice { get; set; }

            [ScaffoldColumn(true)]
            [Display(Name = "补缴服务费", Order = 7)]
            [DataType(System.ComponentModel.DataAnnotations.DataType.Currency, ErrorMessage = "货币格式不正确")]
            public object AddPrice { get; set; }
			
			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 8)]
            [StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 10)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 12)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }


    }
}
 

