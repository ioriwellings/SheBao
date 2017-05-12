using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(LadderPriceMetadata))]//使用LadderPriceMetadata对LadderPrice进行数据验证
    public partial class LadderPrice 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        [Display(Name = "产品")]
        public string ProductIdOld { get; set; }
        
        #endregion

    }
    public partial class LadderPriceMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object Id { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "单人服务费", Order = 3)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.Currency,ErrorMessage="货币格式不正确")]
			public object SinglePrice { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "终止阶梯", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? EndLadder { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "起始阶梯", Order = 5)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BeginLadder { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "状态", Order = 6)]
            [StringLength(200, ErrorMessage = "长度不可超过200")]
			public object Status { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 8)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 10)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }


    }
}
 

