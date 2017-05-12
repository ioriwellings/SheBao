using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    /// <summary>
    /// 员工社保收支对比
    /// </summary>
    public class CostPayPersonContrasted
    {
        public int EmployeeId { get; set; } // 员工ID
        public int CompanyId { get; set; }  // 公司ID
        public string CityId { get; set; }  // 缴纳地ID
        public string CityName { get; set; }  // 缴纳地名称
        public string CompanyCode { get; set; }  // 公司编号
        public string CompanyName { get; set; }  // 公司名称
        public string EmployeeName { get; set; }  // 员工姓名
        public string Certificate { get; set; }  // 身份证号
        public Nullable<decimal> PayCompanyCost { get; set; } // 公司支出费用
        public Nullable<decimal> PayPersonCost { get; set; }  // 个人支出费用
        public Nullable<decimal> CostCompanyCost { get; set; } // 公司收入费用
        public Nullable<decimal> CostPersonCost { get; set; }  // 个人收入费用
    }
}
