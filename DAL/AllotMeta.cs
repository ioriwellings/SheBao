using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(AllotMetadata))]
    public partial class Allot : IBaseEntity
    {
        
        #region 自定义属性

        #endregion

    }
    public class AllotMetadata
    {
			[Display(Name = "企业编号", Order = 1)]
			public object CompanyId { get; set; }

			[Display(Name = "企业名称", Order = 2)]
			public object CompanyName { get; set; }

			[Display(Name = "缴纳地", Order = 3)]
			public object City { get; set; }

			[Display(Name = "缴纳地编号", Order = 4)]
			public object CityId { get; set; }

			[Display(Name = "申报人员", Order = 5)]
			public object EmployeeAddSum { get; set; }

			[Display(Name = "服务人员", Order = 6)]
			public object EmployeeServerSum { get; set; }

			[Display(Name = "责任客服", Order = 7)]
			public object RealName_ZR { get; set; }

			[Display(Name = "员工客服", Order = 8)]
			public object RealName_YG { get; set; }

			[Display(Name = "责任客服ID", Order = 9)]
			public object UserID_ZR { get; set; }

			[Display(Name = "员工客服ID", Order = 10)]
			public object UserID_YG { get; set; }

			[Display(Name = "分配状态", Order = 11)]
			public object AllotState { get; set; }


    }


}

