using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    #region 责任客服审核视图


    public class EmployeeGoonPayment2View
    {
        public EmployeeGoonPayment2View()
        {
            FLG = 0;
        }

        [Display(Name = "员工关系表主键", Order = 1)]
        public int? CompanyEmployeeRelationId { get; set; }

        [Display(Name = "公司主键", Order = 2)]
        public int? CompanyId { get; set; }

        [Display(Name = "公司编号")]
        public string CompanyCode { get; set; }

        [Display(Name = "公司名称", Order = 2)]
        public string CompanyName { get; set; }

        [Display(Name = "员工ID")]
        public int? EmployeeID { get; set; }

        [Display(Name = "姓名", Order = 3)]
        public string Name { get; set; }

        [Display(Name = "身份证号", Order = 4)]
        public string CertificateNumber { get; set; }

        [Display(Name = "缴纳地ID")]
        public string CityID { get; set; }

        [Display(Name = "缴纳地", Order = 5)]
        public string City { get; set; }

        [Display(Name = "社保户口类型", Order = 6)]
        public string PoliceAccountNature { get; set; }

        [Display(Name = "社保户口性质")]
        public string PoliceAccountNatureName { get; set; }

        [Display(Name = "申报月份")]
        public int? YearMonth { get; set; }

        [Display(Name = "提取情况")]
        public string CollectState { get; set; }

        [Display(Name = "id")]
        public int Id { get; set; }

        [Display(Name = "IDs")]
        public string AddIds { get; set; }

        [Display(Name = "险种id")]
        public int? InsuranceKindId { get; set; }

        [Display(Name = "险种")]
        public string InsuranceKindName { get; set; }

        [Display(Name = "责任客服ID")]
        public int? UserID_ZR { get; set; }

        [Display(Name = "责任客服")]
        public string UserName_ZR { get; set; }

        [Display(Name = "员工客服ID")]
        public int? UserID_YG { get; set; }

        [Display(Name = "员工客服")]
        public string UserName_YG { get; set; }

        [Display(Name = "机构ID")]
        public int? BranchID { get; set; }

        [Display(Name = "添加调基FLG")]
        public int FLG { get; set; }

        [Display(Name = "调基申报时间")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "调基状态")]
        public string State { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        [Display(Name = "岗位")]
        public string Station { get; set; }
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
        //调基原数据
        public List<ChangeWageInsuranceKindInfo> LastChangeWageInsuranceKindInfo { get; set; }
    }
    //调基险种
    public class ChangeWageInsuranceKindInfo
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
    }
    //员工险种、基数
    public class AllInsuranceKind
    {
        public string CityID { get; set; }//缴纳地
        public decimal? Pension_Wage { get; set; }//养老工资
        public decimal? Medical_Wage { get; set; }//医疗工资
        public decimal? WorkInjury_Wage { get; set; }//工伤工资
        public decimal? Unemployment_Wage { get; set; }//失业工资
        public decimal? HousingFund_Wage { get; set; }//公积金工资
        public decimal? Maternity_Wage { get; set; }//婚育工资
    }
    #endregion


}