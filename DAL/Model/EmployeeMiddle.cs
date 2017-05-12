using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public partial class EmployeeMiddleShow
    {
        public int Id { get; set; }
        public Nullable<int> InsuranceKindId { get; set; }
        public string InsuranceKindName { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CardId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> CompanyEmployeeRelationId { get; set; }
        public string PaymentBetween { get; set; }
        public Nullable<int> PaymentStyle { get; set; }
        public string PaymentStyleName { get; set; }
        public Nullable<decimal> CompanyBasePayment { get; set; }
        public Nullable<decimal> CompanyPayment { get; set; }
        public Nullable<decimal> EmployeeBasePayment { get; set; }
        public Nullable<decimal> EmployeePayment { get; set; }
        public Nullable<int> PaymentMonth { get; set; }
        public Nullable<int> StartDate { get; set; }
        public Nullable<int> EndedDate { get; set; }
        public int UseBetween { get; set; }
        public string BillId { get; set; }
        public string Remark { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdatePerson { get; set; }
        public string CityId { get; set; }
    }
}
