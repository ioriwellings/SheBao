using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public class SingleStopPaymentView
    {
        /// <summary>
        /// 企业编号
        /// </summary>
        [Display(Name = "企业ID", Order = 1)]
        public int CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "企业名称", Order = 1)]
        public string CompanyName { get; set; }
        /// <summary>
        /// 员工编号
        /// </summary>
        [Display(Name = "员工ID", Order = 1)]
        public int EmployeeID { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        [Display(Name = "员工姓名", Order = 1)]
        public string EmployeeName { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [Display(Name = "身份证号", Order = 1)]
        public string CertificateNumber { get; set; }
        /// <summary>
        /// 能够报减的险种名称
        /// </summary>
        [Display(Name = "可停缴险种", Order = 1)]
        public string CanSotpInsuranceKindName { get; set; }

        /// <summary>
        /// 能够报减的险种名称ID集合
        /// </summary>
        public string CanSotpInsuranceKindIDs { get; set; }

        /// <summary>
        /// 保险缴纳地
        /// </summary>
        [Display(Name = "缴纳地", Order = 1)]
        public string CityName { get; set; }
        /// <summary>
        /// 保险缴纳地ID
        /// </summary>
        /// 

        public string CityID { get; set; }


        /// <summary>
        /// 申报月
        /// </summary>
        [Display(Name = "申报月", Order = 1)]
        public int? YearMonth { get; set; }        /// <summary>
        /// 添加员工表ID
        /// </summary>
        [Display(Name = "添加员工表ID", Order = 1)]
        public int EmployeeAddId { get; set; }

        [Display(Name = "员工关系表主键", Order = 1)]
        public int? CompanyEmployeeRelationId { get; set; }
    }

    public class SingleStopPaymentViewDuty
    {


        /// <summary>
        /// ID报减
        /// </summary>
        [Display(Name = "报减ID", Order = 1)]
        public int ID { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        [Display(Name = "企业ID", Order = 1)]
        public int CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "企业名称", Order = 1)]
        public string CompanyName { get; set; }
        /// <summary>
        /// 员工编号
        /// </summary>
        [Display(Name = "员工ID", Order = 1)]
        public int EmployeeID { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        [Display(Name = "员工姓名", Order = 1)]
        public string EmployeeName { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [Display(Name = "身份证号", Order = 1)]
        public string CertificateNumber { get; set; }
        /// <summary>
        /// 能够报减的险种名称
        /// </summary>
        [Display(Name = "可停缴险种", Order = 1)]
        public string CanSotpInsuranceKindName { get; set; }

        /// <summary>
        /// 能够报减的险种名称ID集合
        /// </summary>
        public string CanSotpInsuranceKindIDs { get; set; }

        /// <summary>
        /// 保险缴纳地
        /// </summary>
        [Display(Name = "缴纳地", Order = 1)]
        public string CityName { get; set; }
        /// <summary>
        /// 保险缴纳地ID
        /// </summary>
        /// 

        public string CityID { get; set; }


        /// <summary>
        /// 申报月
        /// </summary>
        [Display(Name = "申报月", Order = 1)]
        public int? YearMonth { get; set; }        /// <summary>
        /// 添加员工表ID
        /// </summary>
        [Display(Name = "添加员工表ID", Order = 1)]
        public int EmployeeAddId { get; set; }

        [Display(Name = "员工关系表主键", Order = 1)]
        public int? CompanyEmployeeRelationId { get; set; }
    }

    public class StoppaymentTypeYearmonth
    {



        /// <summary>
        /// 报建表ID
        /// </summary>
        [Display(Name = "ID", Order = 1)]
        public int? ID { get; set; }    

         /// <summary>
        /// 报减社保月
        /// </summary>
        [Display(Name = "社保月", Order = 1)]
        public DateTime? InsuranceMonth { get; set; }        /// <summary>
      /// <summary>
        /// 政策手续
        /// </summary>
        [Display(Name = "政策手续", Order = 1)]
        public int? PoliceOperationId { get; set; }                                           /// 
        /// <summary>
        /// 险种
        /// </summary>
        [Display(Name = "险种", Order = 1)]
        public int? InsuranceKindId { get; set; }

        /// <summary>
        /// 险种名称
        /// </summary>
        [Display(Name = "险种名称", Order = 1)]
        public string InsuranceKindName { get; set; }/// <summary>
       
    }
    /// <summary>
    /// Api接口——停缴员工信息
    /// </summary>
    public class StopPaymentEmployeeInfoForApi
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int EmployeeId { get; set; }
        public string CardId { get; set; }
        public string EmployeeName { get; set; }
        public List<StopPaymentInsuranceKindInfoForApi> LstInsuranceKindInfo { get; set; }
    }
    /// <summary>
    /// Api接口——停缴险种信息
    /// </summary>
    public class StopPaymentInsuranceKindInfoForApi
    {
        /// <summary>
        /// 社保政策ID
        /// </summary>
        public int PoliceInsuranceId { get; set; }
        /// <summary>
        /// 险种名称
        /// </summary>
        public string InsuranceKindName { get; set; }
        ///// <summary>
        ///// 社保月
        ///// </summary>
        //public DateTime InsuranceMonth { get; set; }
        /// <summary>
        /// 政策手续ID
        /// </summary>
        public int PoliceOperationId { get; set; }
        /// <summary>
        /// 政策手续名称
        /// </summary>
        public string PoliceOperationName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatePerson { get; set; }
    }

    public class PlatStopPaymentEmployeeInfo
    {


        /// <summary>
        /// 员工姓名
        /// </summary>
        [Display(Name = "员工姓名", Order = 1)]
        public string EmployeeName { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Display(Name = "身份证号", Order = 1)]
        public string CertificateNumber { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [Display(Name = "企业名称", Order = 1)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        [Display(Name = "岗位名称", Order = 1)]
        public string PositionName { get; set; }

        /// <summary>
        /// 保险缴纳地
        /// </summary>
        [Display(Name = "缴纳地", Order = 1)]
        public string CityName { get; set; }

        /// <summary>
        /// 户口类型
        /// </summary>
        [Display(Name = "户口类型", Order = 1)]
        public string PoliceName { get; set; }

        /// <summary>
        /// 社保工资
        /// </summary>
        [Display(Name = "社保工资", Order = 1)]
        public decimal? Salary { get; set; }

        /// <summary>
        /// 公积金基数
        /// </summary>
        [Display(Name = "公积金基数", Order = 1)]
        public int GongJiJinMoney { get; set; }

        /// <summary>
        /// 报减时间
        /// </summary>
        [Display(Name = "报减时间", Order = 1)]
        public string BaoJianTime { get; set; }
        /// <summary>
        /// 申报月
        /// </summary>
        [Display(Name = "申报月", Order = 1)]
        public string YearMouth { get; set; }
    }

    public class StopPaymentEmployeeInfo
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        [Display(Name = "员工姓名")]
        public string EmployeeName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string CardId { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [Display(Name = "企业名称")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        [Display(Name = "岗位")]
        public string Station { get; set; }
        /// <summary>
        /// 缴纳地
        /// </summary>
        [Display(Name = "缴纳地")]
        public string CityName { get; set; }
        /// <summary>
        /// 户口性质
        /// </summary>
        [Display(Name = "户口性质")]
        public string PoliceAccountNatureName { get; set; }
        /// <summary>
        /// 社保工资
        /// </summary>
        [Display(Name = "社保工资")]
        public decimal SB_Wage { get; set; }
        /// <summary>
        /// 公积金基数
        /// </summary>
        [Display(Name = "公积金基数")]
        public decimal GJJ_Wage { get; set; }
        public List<StopPaymentInsuranceKindInfo> LstStopPaymentInsuranceKindInfos { get; set; }
    }

    public class StopPaymentInsuranceKindInfo
    {
        /// <summary>
        /// 增加员工表ID
        /// </summary>
        public int EmployeeAddId { get; set; }
        /// <summary>
        /// 社保种类ID
        /// </summary>
        public int InsuranceKindId { get; set; }
        /// <summary>
        /// 社保种类名称
        /// </summary>
        public string InsuranceKindName { get; set; }
        /// <summary>
        /// 社保政策ID
        /// </summary>
        public int PoliceInsuranceId { get; set; }
        /// <summary>
        /// 基数
        /// </summary>
        public decimal Wage { get; set; }
        public int CompanyEmployeeRelationId { get; set; }

        /// <summary>
        /// 报减方式
        /// </summary>
        public string PoliceOperationName { get; set; }

        /// <summary>
        /// 报减时间
        /// </summary>
        public DateTime? StopDate { get; set; }

        /// <summary>
        /// 停缴的id
        /// </summary>
        public int EmployeeStopPaymentID { get; set; }
    }

    public class EmployeeStopPaymentSingle
    {
        /// <summary>
        /// 社保种类ID
        /// </summary>
        public int InsuranceKindId { get; set; }
        /// <summary>
        /// 企业员工关系ID
        /// </summary>
        public int CompanyEmployeeRelationId { get; set; }
        /// <summary>
        /// 政策ID
        /// </summary>
        public int PoliceInsuranceId { get; set; }
        /// <summary>
        /// 员工停缴表
        /// </summary>
        public EmployeeStopPayment StopPayment { get; set; }

    }
}
