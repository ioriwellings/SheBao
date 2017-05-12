using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{

    #region 社保报增查询视图
    public class EmployeeStopView
    {
        [Display(Name = "分公司名称（操作人所在的公司）")]
        public string Operator_CompanyName { get; set; }


        [Display(Name = "员工关系表主键")]
        public int? CompanyEmployeeRelationId { get; set; }

        [Display(Name = "公司主键")]
        public int? CompanyId { get; set; }

        [Display(Name = "公司编号")]
        public string CompanyCode { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "证件号码")]
        public string CertificateNumber { get; set; }

        [Display(Name = "险种")]
        public string InsuranceKinds { get; set; }

        [Display(Name = "申报日期")]
        public int YearMonth { get; set; }

        [Display(Name = "社保缴纳地")]
        public string City { get; set; }

        [Display(Name = "社保户口性质")]
        public string PoliceAccountNatureName { get; set; }

        [Display(Name = "工作岗位")]
        public string Station { get; set; }

        [Display(Name = "申报IDs")]
        public string StopIds { get; set; }

        [Display(Name = "社保状态")]
        public string State { get; set; }
      
        [Display(Name = "养老政策")]
        public string PoliceInsuranceName_1 { get; set; }

        [Display(Name = "养老报减时间")]
        public DateTime? InsuranceMonth_1 { get; set; }

        [Display(Name = "养老报减方式")]
        public string PoliceOperationName_1 { get; set; }

        [Display(Name = "养老状态")]
        public string State_1 { get; set; }

        [Display(Name = "养老申报时间")]
        public int? YearMonth_1 { get; set; }

        [Display(Name = "养老是否单立户")]
        public string IsIndependentAccount_1 { get; set; }

        [Display(Name = "养老社保编号")]
        public string InsuranceCode_1 { get; set; }

        [Display(Name = "养老创建时间")]
        public DateTime? CreateTime_1 { get; set; }




       


        [Display(Name = "医疗政策")]
        public string PoliceInsuranceName_2 { get; set; }

        [Display(Name = "医疗报减时间")]
        public DateTime? InsuranceMonth_2 { get; set; }

        [Display(Name = "医疗报减方式")]
        public string PoliceOperationName_2 { get; set; }

        [Display(Name = "医疗状态")]
        public string State_2 { get; set; }

        [Display(Name = "医疗申报时间")]
        public int? YearMonth_2 { get; set; }

        [Display(Name = "医疗是否单立户")]
        public string IsIndependentAccount_2 { get; set; }

        [Display(Name = "医疗社保编号")]
        public string InsuranceCode_2 { get; set; }

        [Display(Name = "医疗创建时间")]
        public DateTime? CreateTime_2 { get; set; }




        
        [Display(Name = "工伤政策")]
        public string PoliceInsuranceName_3 { get; set; }

        [Display(Name = "工伤报减时间")]
        public DateTime? InsuranceMonth_3 { get; set; }

        [Display(Name = "工伤报减方式")]
        public string PoliceOperationName_3 { get; set; }

        [Display(Name = "工伤状态")]
        public string State_3 { get; set; }

        [Display(Name = "工伤申报时间")]
        public int? YearMonth_3 { get; set; }

        [Display(Name = "工伤是否单立户")]
        public string IsIndependentAccount_3 { get; set; }

        [Display(Name = "工伤社保编号")]
        public string InsuranceCode_3 { get; set; }

        [Display(Name = "工伤创建时间")]
        public DateTime? CreateTime_3 { get; set; }




      



        [Display(Name = "失业政策")]
        public string PoliceInsuranceName_4 { get; set; }

        [Display(Name = "失业报减时间")]
        public DateTime? InsuranceMonth_4 { get; set; }

        [Display(Name = "失业报减方式")]
        public string PoliceOperationName_4 { get; set; }

        [Display(Name = "失业状态")]
        public string State_4 { get; set; }

        [Display(Name = "失业申报时间")]
        public int? YearMonth_4 { get; set; }


        [Display(Name = "失业是否单立户")]
        public string IsIndependentAccount_4 { get; set; }

        [Display(Name = "失业社保编号")]
        public string InsuranceCode_4 { get; set; }

        [Display(Name = "失业创建时间")]
        public DateTime? CreateTime_4 { get; set; }



      



        [Display(Name = "公积金政策")]
        public string PoliceInsuranceName_5 { get; set; }

        [Display(Name = "公积金报减时间")]
        public DateTime? InsuranceMonth_5 { get; set; }

        [Display(Name = "公积金报减方式")]
        public string PoliceOperationName_5 { get; set; }

        [Display(Name = "公积金状态")]
        public string State_5 { get; set; }

        [Display(Name = "公积金申报时间")]
        public int? YearMonth_5 { get; set; }

        [Display(Name = "公积金是否单立户")]
        public string IsIndependentAccount_5 { get; set; }

        [Display(Name = "公积金社保编号")]
        public string InsuranceCode_5 { get; set; }

        [Display(Name = "公积金创建时间")]
        public DateTime? CreateTime_5 { get; set; }



       



        [Display(Name = "生育政策")]
        public string PoliceInsuranceName_6 { get; set; }

        [Display(Name = "生育报减时间")]
        public DateTime? InsuranceMonth_6 { get; set; }

        [Display(Name = "生育报减方式")]
        public string PoliceOperationName_6 { get; set; }

        [Display(Name = "生育状态")]
        public string State_6 { get; set; }

        [Display(Name = "生育申报时间")]
        public int? YearMonth_6 { get; set; }

        [Display(Name = "生育是否单立户")]
        public string IsIndependentAccount_6 { get; set; }

        [Display(Name = "生育社保编号")]
        public string InsuranceCode_6 { get; set; }

        [Display(Name = "生育创建时间")]
        public DateTime? CreateTime_6 { get; set; }



        



        [Display(Name = "医疗大额政策")]
        public string PoliceInsuranceName_7 { get; set; }

        [Display(Name = "医疗大额报减时间")]
        public DateTime? InsuranceMonth_7 { get; set; }

        [Display(Name = "医疗大额报减方式")]
        public string PoliceOperationName_7 { get; set; }

        [Display(Name = "医疗大额状态")]
        public string State_7 { get; set; }

        [Display(Name = "医疗大额申报时间")]
        public int? YearMonth_7 { get; set; }

        [Display(Name = "医疗大额是否单立户")]
        public string IsIndependentAccount_7 { get; set; }

        [Display(Name = "医疗大额社保编号")]
        public string InsuranceCode_7 { get; set; }

        [Display(Name = "医疗大额创建时间")]
        public DateTime? CreateTime_7 { get; set; }



      



        [Display(Name = "补充公积金政策")]
        public string PoliceInsuranceName_8 { get; set; }

        [Display(Name = "补充公积金报减时间")]
        public DateTime? InsuranceMonth_8 { get; set; }

        [Display(Name = "补充公积金报减方式")]
        public string PoliceOperationName_8 { get; set; }

        [Display(Name = "补充公积金状态")]
        public string State_8 { get; set; }

        [Display(Name = "补充公积金申报时间")]
        public int? YearMonth_8 { get; set; }

        [Display(Name = "补充公积金是否单立户")]
        public string IsIndependentAccount_8 { get; set; }

        [Display(Name = "补充公积金社保编号")]
        public string InsuranceCode_8 { get; set; }

        [Display(Name = "补充公积金创建时间")]
        public DateTime? CreateTime_8 { get; set; }




        [Display(Name = "大病政策")]
        public string PoliceInsuranceName_9 { get; set; }

        [Display(Name = "大病报减时间")]
        public DateTime? InsuranceMonth_9 { get; set; }

        [Display(Name = "大病报减方式")]
        public string PoliceOperationName_9 { get; set; }

        [Display(Name = "大病状态")]
        public string State_9 { get; set; }

        [Display(Name = "大病申报时间")]
        public int? YearMonth_9 { get; set; }

        [Display(Name = "大病是否单立户")]
        public string IsIndependentAccount_9 { get; set; }

        [Display(Name = "大病社保编号")]
        public string InsuranceCode_9 { get; set; }

        [Display(Name = "大病创建时间")]
        public DateTime? CreateTime_9 { get; set; }

       

       
    }
    #endregion
}
 

