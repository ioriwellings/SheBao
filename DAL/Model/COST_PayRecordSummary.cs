using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    /// <summary>
    /// 社保支出费用汇总（支出费用加入对比的列表中用到）
    /// </summary>
    public class COST_PayRecordSummary
    {
        public int YearMonth { get; set; } // 年月
        public int CostType { get; set; }  // 险种
        public int Count { get; set; } // 人数
        public int Status { get; set; }  // 状态（0：可以对比；1：不可对比）
        public string CityId { get; set; }  // 缴纳地
        public string CityName { get; set; } // 缴纳地名称
        public Nullable<decimal> PersonCost { get; set; }  // 单位金额
        public Nullable<decimal> CompanyCost { get; set; }  // 个人金额
    }
}
