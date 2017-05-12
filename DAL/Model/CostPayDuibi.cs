using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public partial class CostPayDuibi
    {
        public Int32 CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal? CompanyCost { get; set; }
        public decimal? PersonCost { get; set; }
        public decimal? CompanyPay { get; set; }
        public decimal? PersonPay { get; set; }
    }
    public partial class CostPayDuibiDetails
    {
        public Int32 CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int YearMonth { get; set; }
        public decimal? CompanyCost { get; set; }
        public decimal? PersonCost { get; set; }
        public decimal? CompanyPay { get; set; }
        public decimal? PersonPay { get; set; }
    }
}
