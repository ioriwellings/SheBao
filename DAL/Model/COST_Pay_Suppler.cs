using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public partial class COST_Pay_Suppler
    {
        public int ID { get; set; }
        public int YearMonth { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public int BranchID { get; set; }
        public string PersonName { get; set; }
        public string CardId { get; set; }
        public Nullable<int> PersonId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Suppliers { get; set; }
        public string CityId { get; set; }
        public string PaymentSocialMonthYL { get; set; }
        public Nullable<decimal> RadixYL { get; set; }
        public Nullable<decimal> CompanyCosYL { get; set; }
        public Nullable<decimal> PersonCostYL { get; set; }
        public string PaymentSocialMonthSY { get; set; }
        public Nullable<decimal> RadixSY { get; set; }
        public Nullable<decimal> CompanyCostSY { get; set; }
        public Nullable<decimal> PersonCostSY { get; set; }
        public string PaymentSocialMonthGS { get; set; }
        public Nullable<decimal> RadixGS { get; set; }
        public Nullable<decimal> CompanyCostGS { get; set; }
        public string PaymentSocialMonthYiL { get; set; }
        public Nullable<decimal> RadixYil { get; set; }
        public Nullable<decimal> CompanyCostYil { get; set; }
        public Nullable<decimal> PersonCostYil { get; set; }
        public Nullable<decimal> CompanyCostYilMax { get; set; }
        public Nullable<decimal> PersonCostYilMax { get; set; }
        public Nullable<decimal> CompanyCostShengY { get; set; }
        public string PaymentSocialMonthGJJ { get; set; }
        public Nullable<decimal> RadixGJJ { get; set; }
        public Nullable<decimal> CompanyCostGJJ { get; set; }
        public Nullable<decimal> PersonCostGJJ { get; set; }
        public string PaymentSocialMonthBCGJJ { get; set; }
        public Nullable<decimal> RadixBCGJJ { get; set; }
        public Nullable<decimal> CompanyCostBCGJJ { get; set; }
        public Nullable<decimal> PersonCostBCGJJ { get; set; }
        public Nullable<decimal> PayOtherSocial { get; set; }
        public Nullable<decimal> PayOther { get; set; }
        public string BatchGuid { get; set; }
        public string Remark { get; set; }
        public int SuppliersId { get; set; }
        public string CardIdSB { get; set; }
        public Nullable<decimal> PayFee { get; set; }
        public Nullable<decimal> ServiceCost { get; set; }
        public Nullable<decimal> YanglaoPayFee { get; set; }
        public Nullable<decimal> YiliaoPayFee { get; set; }
        public Nullable<decimal> GongjijinPayFee { get; set; }
        public Nullable<decimal> ShiyePayFee { get; set; }
        public Nullable<decimal> GongshangPayFee { get; set; }
        public Nullable<decimal> ShengYuPayFee { get; set; }
        public int? PaymentStyle { get; set; }
    }
}
