using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class COST_PayRecordConfirm
    {
        public int ID { get; set; }
        public int CostType { get; set; }
        public int Status { get; set; }
        public int COST_PayRecordId { get; set; }
        public Nullable<decimal> CompanyCost { get; set; }
        public Nullable<decimal> PersonCost { get; set; }
        public Nullable<int> AllCount { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserID { get; set; }
        public string CreateUserName { get; set; }

        public string CityId { get; set; }
        public string CityName { get; set; }
    }
}
