using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
   
    public class PoliceInsuranceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InsuranceKindId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? MaxPayMonth { get; set; }
        public int? InsuranceAdd { get; set; }
        public int? InsuranceReduce { get; set; }
        public decimal? CompanyPercent { get; set; }
        public decimal? CompanyLowestNumber { get; set; }
        public decimal? EmployeeLowestNumber { get; set; }
        public decimal? CompanyHighestNumber { get; set; }
        public decimal? EmployeeHighestNumber { get; set; }
        public decimal? EmployeePercent { get; set; }
        public string IsDefault { get; set; }
        public string Remark { get; set; }
        public string State { get; set; }
        public string CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public string UpdateTime { get; set; }
        public string UpdatePerson { get; set; }

    }
}
