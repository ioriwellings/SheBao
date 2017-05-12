using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    /// <summary>
    /// 社保支出费用
    /// </summary>
    public class CostPayInsurance
    {
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public Nullable<decimal> PayCompanyCost { get; set; } // 公司支出费用
        public Nullable<decimal> PayPersonCost { get; set; }  // 个人支出费用
        public Nullable<decimal> CostCompanyCost { get; set; } // 公司收入费用
        public Nullable<decimal> CostPersonCost { get; set; }  // 个人收入费用
    }
}
