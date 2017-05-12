using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model 
{
    public class Cost_CostTableDetails
    {
        public int No { get; set; }  // 序号
        public int COST_CostTable_ID { get; set; }  // 费用表编号
        public int PaymentStyle { get; set; }  // 缴费类型
        public string CityName { get; set; }  // 社保缴纳地


        public string Operator_CompanyName { get; set; }//操作人所在的分公司


        public int CRM_Company_ID { get; set; }
        public Nullable<int> CreateFrom { get; set; } //费用来源
        public string SupplierName { get; set; }
        public string Flag { get; set; }
        // 个人相关
        public Nullable<int> Employee_ID { get; set; }
        public string EmployName { get; set; }
        public string CertificateType { get; set; }
        public string CertificateNumber { get; set; }

        // 养老相关
        public string YanglaoPaymentInterval { get; set; }  // 缴纳区间
        public string YanglaoPaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> YanglaoPaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> YanglaoCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> YanglaoCompanyCost { get; set; }  // 企业费用
        public Nullable<double> YanglaoCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> YanglaoPersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> YanglaoPersonCost { get; set; }  // 个人费用
        public Nullable<double> YanglaoPersonRatio { get; set; }  // 个人缴纳比例

        // 医疗相关
        public string YiliaoPaymentInterval { get; set; }  // 缴纳区间
        public string YiliaoPaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> YiliaoPaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> YiliaoCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> YiliaoCompanyCost { get; set; }  // 企业费用
        public Nullable<double> YiliaoCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> YiliaoPersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> YiliaoPersonCost { get; set; }  // 个人费用
        public Nullable<double> YiliaoPersonRatio { get; set; }  // 个人缴纳比例

        // 大额医疗相关
        public string DaePaymentInterval { get; set; }  // 缴纳区间
        public string DaePaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> DaePaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> DaeCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> DaeCompanyCost { get; set; }  // 企业费用
        public Nullable<double> DaeCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> DaePersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> DaePersonCost { get; set; }  // 个人费用
        public Nullable<double> DaePersonRatio { get; set; }  // 个人缴纳比例

        // 生育相关
        public string ShengyuPaymentInterval { get; set; }  // 缴纳区间
        public string ShengyuPaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> ShengyuPaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> ShengyuCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> ShengyuCompanyCost { get; set; }  // 企业费用
        public Nullable<double> ShengyuCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> ShengyuPersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> ShengyuPersonCost { get; set; }  // 个人费用
        public Nullable<double> ShengyuPersonRatio { get; set; }  // 个人缴纳比例

        // 工伤相关
        public string GongshangPaymentInterval { get; set; }  // 缴纳区间
        public string GongshangPaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> GongshangPaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> GongshangCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> GongshangCompanyCost { get; set; }  // 企业费用
        public Nullable<double> GongshangCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> GongshangPersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> GongshangPersonCost { get; set; }  // 个人费用
        public Nullable<double> GongshangPersonRatio { get; set; }  // 个人缴纳比例

        // 失业相关
        public string ShiyePaymentInterval { get; set; }  // 缴纳区间
        public string ShiyePaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> ShiyePaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> ShiyeCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> ShiyeCompanyCost { get; set; }  // 企业费用
        public Nullable<double> ShiyeCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> ShiyePersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> ShiyePersonCost { get; set; }  // 个人费用
        public Nullable<double> ShiyePersonRatio { get; set; }  // 个人缴纳比例

        // 公积金相关
        public string GongjijinPaymentInterval { get; set; }  // 缴纳区间
        public string GongjijinPaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> GongjijinPaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> GongjijinCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> GongjijinCompanyCost { get; set; }  // 企业费用
        public Nullable<double> GongjijinCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> GongjijinPersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> GongjijinPersonCost { get; set; }  // 个人费用
        public Nullable<double> GongjijinPersonRatio { get; set; }  // 个人缴纳比例

        // 补充公积金相关
        public string GongjijinBCPaymentInterval { get; set; }  // 缴纳区间
        public string GongjijinBCPaymentSocialMonth { get; set; }  // 缴纳社保月
        public Nullable<int> GongjijinBCPaymentMonth { get; set; }  // 缴纳月数
        public Nullable<decimal> GongjijinBCCompanyRadix { get; set; }  // 企业缴费基数
        public Nullable<decimal> GongjijinBCCompanyCost { get; set; }  // 企业费用
        public Nullable<double> GongjijinBCCompanyRatio { get; set; }  // 企业缴纳比例
        public Nullable<decimal> GongjijinBCPersonRadix { get; set; }  // 个人缴费基数
        public Nullable<decimal> GongjijinBCPersonCost { get; set; }  // 个人费用
        public Nullable<double> GongjijinBCPersonRatio { get; set; }  // 个人缴纳比例

        // 费用相关
        public Nullable<decimal> OtherCost { get; set; }  // 其他费用
        public Nullable<decimal> OtherInsuranceCost { get; set; }  // 其他社保费用
        public Nullable<decimal> ProductionCost { get; set; }  // 工本费
        public Nullable<decimal> ServiceCost { get; set; }  // 服务费

    }


    public class Cost_Cost_Company
    {

        public string CompanyName { set; get; }
        public int CompanyID { set; get; }
        public int YearMouth { set; get; }
        public string CompanyShuiHao { set; get; }
        public string SerialNumber { set; get; }
    }
}
