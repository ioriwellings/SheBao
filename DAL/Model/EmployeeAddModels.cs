using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    #region 初始化
    public class PostInfo
    {
        public object EmployeeId { get; set; }

        public string Name { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }

        public int CompanyId { get; set; }

        public string Telephone { get; set; }
        public string ResidentType { get; set; }
        public string ResidentLocation { get; set; }

        public string SSResidentType { get; set; }

        public string City { get; set; }
        public string attachments { get; set; }

        public string Station { get; set; }//岗位
        public int PoliceAccountNature { get; set; } //户口性质

        #region 养老
        public decimal? Pension_Wage { get; set; }//养老工资
        public string Pension_PaymentTime { get; set; }//起缴时间
        public string Pension_SocialInsuranceMonth { get; set; }//社保月
        public string Pension_Mode { get; set; }//社保报增方式

        public string Pension_InsuranceNumber { get; set; }//社保编号
        public int? Pension_Percentage { get; set; }//社保政策标示
        public int? Pension_PoliceOperation { get; set; }//政策手续


        #endregion

        #region
        public decimal? Medical_Wage { get; set; }
        public string Medical_PaymentTime { get; set; }
        public string Medical_SocialInsuranceMonth { get; set; }
        public string Medical_Mode { get; set; }
        public string Medical_InsuranceNumber { get; set; }
        public int? Medical_Percentage { get; set; }//社保政策标示
        public int? Medical_PoliceOperation { get; set; }//政策手续




        public decimal? WorkInjury_Wage { get; set; }
        public string WorkInjury_PaymentTime { get; set; }
        public string WorkInjury_SocialInsuranceMonth { get; set; }
        public string WorkInjury_Mode { get; set; }
        public string WorkInjury_InsuranceNumber { get; set; }
        public int? WorkInjury_Percentage { get; set; }//社保政策标示
        public int? WorkInjury_PoliceOperation { get; set; }//政策手续


        public decimal? Unemployment_Wage { get; set; }
        public string Unemployment_PaymentTime { get; set; }
        public string Unemployment_SocialInsuranceMonth { get; set; }
        public string Unemployment_Mode { get; set; }
        public string Unemployment_InsuranceNumber { get; set; }
        public int? Unemployment_Percentage { get; set; }//社保政策标示
        public int? Unemployment_PoliceOperation { get; set; }//政策手续



        public decimal? HousingFund_Wage { get; set; }
        public string HousingFund_PaymentTime { get; set; }
        public string HousingFund_SocialInsuranceMonth { get; set; }
        public string HousingFund_Mode { get; set; }
        public string HousingFund_InsuranceNumber { get; set; }
        public int? HousingFund_Percentage { get; set; }//社保政策标示
        public int? HousingFund_PoliceOperation { get; set; }//政策手续


        public decimal? Maternity_Wage { get; set; }
        public string Maternity_PaymentTime { get; set; }
        public string Maternity_SocialInsuranceMonth { get; set; }
        public string Maternity_Mode { get; set; }
        public string Maternity_InsuranceNumber { get; set; }
        public int? Maternity_Percentage { get; set; }//社保政策标示
        public int? Maternity_PoliceOperation { get; set; }//政策手续



        public decimal? Seriousillness_Wage { get; set; }
        public string Seriousillness_PaymentTime { get; set; }
        public string Seriousillness_SocialInsuranceMonth { get; set; }
        public string Seriousillness_Mode { get; set; }
        public string Seriousillness_InsuranceNumber { get; set; }
        public int? Seriousillness_Percentage { get; set; }//社保政策标示
        public int? Seriousillness_PoliceOperation { get; set; }//政策手续
        #endregion
    }
    #endregion
    public class JsonMessageResult<T>
    {
        public JsonMessageResult(string code, string message, T item)
        {
            this.item = item;
            this.message = message;
            this.code = code;
        }
        public string code { get; set; }
        public string message { get; set; }
        public T item { get; set; }
    }
    /// <summary>
    /// ajax
    /// </summary>
    public class idname__
    {
        public string Cityid;
        public int ID { get; set; }
        public string Name { get; set; }

    }

    public class EmployeeAddCheckModel
    {

        public string CityId;
        public int? PoliceAccountNatureId { get; set; }
        public int? PoliceInsuranceId { get; set; }

        public int? PoliceOperationId;
    }

    #region 新报增
    public class returnData
    {
        public object EmployeeId { get; set; }

        public string Name { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }

        public int CompanyId { get; set; }

        public string Telephone { get; set; }
        public string ResidentType { get; set; }
        public string ResidentLocation { get; set; }

        public string SSResidentType { get; set; }

        public string City { get; set; }
        public string attachments { get; set; }

        public string Station { get; set; }//岗位
        public int PoliceAccountNature { get; set; } //户口性质
        public List<Insurance> Insurance { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppliersId { get; set; }
    }
    public class Insurance
    {
        public int? Id { get; set; }//
        public string InsuranceKind { get; set; }//
        public decimal? Wage { get; set; }//养老工资
        public string StartTime { get; set; }//起缴时间
        //  public string Pension_SocialInsuranceMonth { get; set; }//社保月
        // public string Pension_Mode { get; set; }//社保报增方式

        public string InsuranceNumber { get; set; }//社保编号
        public int? PoliceInsurance { get; set; }//社保政策标示
        public string PoliceInsurancename { get; set; }//社保政策标示
        public int? PoliceOperation { get; set; }//政策手续
        public string PoliceOperationname { get; set; }//政策手续

        public string SupplierRemark { get; set; }//供应商备注
    }
    #endregion
}
