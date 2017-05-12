using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    #region 责任客服审核视图


    public class EmployeeApprove
    {

        [Display(Name = "员工关系表主键", Order = 1)]
        public int? CompanyEmployeeRelationId { get; set; }

        [Display(Name = "公司主键", Order = 2)]
        public int? CompanyId { get; set; }

        [Display(Name = "公司名称", Order = 2)]
        public string CompanyName { get; set; }

        [Display(Name = "姓名", Order = 3)]
        public string Name { get; set; }

        [Display(Name = "证件号码", Order = 4)]
        public string CertificateNumber { get; set; }

        [Display(Name = "缴纳地", Order = 5)]
        public string City { get; set; }

        [Display(Name = "社保户口类型", Order = 6)]
        public string PoliceAccountNature { get; set; }

        [Display(Name = "险种", Order = 6)]
        public string InsuranceKinds { get; set; }

        [Display(Name = "申报月份")]
        public int? YearMonth { get; set; }

        [Display(Name = "申报IDs")]
        public string AddIds { get; set; }

        [Display(Name = "提取情况")]
        public string CollectState { get; set; }

        [Display(Name = "员工ID")]
        public int? Employee_ID { get; set; }
        [Display(Name = "险种id")]
        public int InsuranceKindid { get; set; }

        [Display(Name = "报增表id")]
        public int Id { get; set; }
        [Display(Name = "险种名称")]
        public string InsuranceKindname { get; set; }

        public string CityID { get; set; }

        [Display(Name = "供应商")]
        public int SupplierID { get; set; }

        [Display(Name = "供应商")]
        public string SupplierName { get; set; }
    }
    #endregion

    #region 社保报增查询视图 
    public class EmployeeAddView
    {

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
        [Display(Name = "社保缴纳地id")]
        public string CityID { get; set; }

        [Display(Name = "社保户口性质")]
        public string PoliceAccountNatureName { get; set; }

        [Display(Name = "工作岗位")]
        public string Station { get; set; }

        [Display(Name = "申报IDs")]
        public string AddIds { get; set; }

        [Display(Name = "社保状态")]
        public string State { get; set; }

        [Display(Name = "养老险种编号")]
        public string InsuranceCode_1 { get; set; }

        [Display(Name = "养老单位基数")]
        public decimal? CompanyNumber_1 { get; set; }

        [Display(Name = "养老单位比例")]
        public decimal? CompanyPercent_1 { get; set; }

        [Display(Name = "养老单位缴纳金额")]
        public decimal? CompanyMoney_1
        {
            get
            {
                if (CompanyNumber_1 == null || CompanyPercent_1 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_1;
                    decimal per = (decimal)CompanyPercent_1;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "养老个人基数")]
        public decimal? EmployeeNumber_1 { get; set; }

        [Display(Name = "养老个人比例")]
        public decimal? EmployeePercent_1 { get; set; }

        [Display(Name = "养老个人缴纳金额")]
        public decimal? EmployeeMoney_1
        {
            get
            {
                if (EmployeeNumber_1 == null || EmployeePercent_1 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_1;
                    decimal per = (decimal)EmployeePercent_1;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "养老状态")]
        public string State_1 { get; set; }

        [Display(Name = "养老政策")]
        public string PoliceInsuranceName_1 { get; set; }

        [Display(Name = "养老申报时间")]
        public int? YearMonth_1 { get; set; }


        [Display(Name = "养老工资")]
        public decimal? Wage_1 { get; set; }

        [Display(Name = "养老报增方式")]
        public string PoliceOperationName_1 { get; set; }

        [Display(Name = "养老起缴时间")]
        public DateTime? StartTime_1 { get; set; }

        [Display(Name = "养老社保月")]
        public DateTime? InsuranceMonth_1 { get; set; }

        [Display(Name = "养老是否单立户")]
        public string IsIndependentAccount_1 { get; set; }

        [Display(Name = "养老创建时间")]
        public DateTime? CreateTime_1 { get; set; }



        [Display(Name = "医疗险种编号")]
        public string InsuranceCode_2 { get; set; }

        [Display(Name = "医疗单位基数")]
        public decimal? CompanyNumber_2 { get; set; }

        [Display(Name = "医疗单位比例")]
        public decimal? CompanyPercent_2 { get; set; }

        [Display(Name = "医疗单位缴纳金额")]
        public decimal? CompanyMoney_2
        {
            get
            {
                if (CompanyNumber_2 == null || CompanyPercent_2 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_2;
                    decimal per = (decimal)CompanyPercent_2;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "医疗个人基数")]
        public decimal? EmployeeNumber_2 { get; set; }

        [Display(Name = "医疗个人比例")]
        public decimal? EmployeePercent_2 { get; set; }

        [Display(Name = "医疗个人缴纳金额")]
        public decimal? EmployeeMoney_2
        {
            get
            {
                if (EmployeeNumber_2 == null || EmployeePercent_2 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_2;
                    decimal per = (decimal)EmployeePercent_2;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "医疗状态")]
        public string State_2 { get; set; }

        [Display(Name = "医疗政策")]
        public string PoliceInsuranceName_2 { get; set; }

        [Display(Name = "医疗申报时间")]
        public int? YearMonth_2 { get; set; }


        [Display(Name = "医疗工资")]
        public decimal? Wage_2 { get; set; }

        [Display(Name = "医疗报增方式")]
        public string PoliceOperationName_2 { get; set; }

        [Display(Name = "医疗起缴时间")]
        public DateTime? StartTime_2 { get; set; }

        [Display(Name = "医疗社保月")]
        public DateTime? InsuranceMonth_2 { get; set; }

        [Display(Name = "医疗是否单立户")]
        public string IsIndependentAccount_2 { get; set; }

        [Display(Name = "医疗创建时间")]
        public DateTime? CreateTime_2 { get; set; }


        [Display(Name = "工伤险种编号")]
        public string InsuranceCode_3 { get; set; }

        [Display(Name = "工伤单位基数")]
        public decimal? CompanyNumber_3 { get; set; }

        [Display(Name = "工伤单位比例")]
        public decimal? CompanyPercent_3 { get; set; }

        [Display(Name = "工伤单位缴纳金额")]
        public decimal? CompanyMoney_3
        {
            get
            {
                if (CompanyNumber_3 == null || CompanyPercent_3 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_3;
                    decimal per = (decimal)CompanyPercent_3;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "工伤个人基数")]
        public decimal? EmployeeNumber_3 { get; set; }

        [Display(Name = "工伤个人比例")]
        public decimal? EmployeePercent_3 { get; set; }

        [Display(Name = "工伤个人缴纳金额")]
        public decimal? EmployeeMoney_3
        {
            get
            {
                if (EmployeeNumber_3 == null || EmployeePercent_3 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_3;
                    decimal per = (decimal)EmployeePercent_3;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "工伤状态")]
        public string State_3 { get; set; }

        [Display(Name = "工伤政策")]
        public string PoliceInsuranceName_3 { get; set; }

        [Display(Name = "工伤申报时间")]
        public int? YearMonth_3 { get; set; }


        [Display(Name = "工伤工资")]
        public decimal? Wage_3 { get; set; }

        [Display(Name = "工伤报增方式")]
        public string PoliceOperationName_3 { get; set; }

        [Display(Name = "工伤起缴时间")]
        public DateTime? StartTime_3 { get; set; }

        [Display(Name = "工伤社保月")]
        public DateTime? InsuranceMonth_3 { get; set; }

        [Display(Name = "工伤是否单立户")]
        public string IsIndependentAccount_3 { get; set; }

        [Display(Name = "工伤创建时间")]
        public DateTime? CreateTime_3 { get; set; }



        [Display(Name = "失业险种编号")]
        public string InsuranceCode_4 { get; set; }

        [Display(Name = "失业单位基数")]
        public decimal? CompanyNumber_4 { get; set; }

        [Display(Name = "失业单位比例")]
        public decimal? CompanyPercent_4 { get; set; }

        [Display(Name = "失业单位缴纳金额")]
        public decimal? CompanyMoney_4
        {
            get
            {
                if (CompanyNumber_4 == null || CompanyPercent_4 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_4;
                    decimal per = (decimal)CompanyPercent_4;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "失业个人基数")]
        public decimal? EmployeeNumber_4 { get; set; }

        [Display(Name = "失业个人比例")]
        public decimal? EmployeePercent_4 { get; set; }

        [Display(Name = "失业个人缴纳金额")]
        public decimal? EmployeeMoney_4
        {
            get
            {
                if (EmployeeNumber_4 == null || EmployeePercent_4 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_4;
                    decimal per = (decimal)EmployeePercent_4;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "失业状态")]
        public string State_4 { get; set; }

        [Display(Name = "失业政策")]
        public string PoliceInsuranceName_4 { get; set; }

        [Display(Name = "失业申报时间")]
        public int? YearMonth_4 { get; set; }


        [Display(Name = "失业工资")]
        public decimal? Wage_4 { get; set; }

        [Display(Name = "失业报增方式")]
        public string PoliceOperationName_4 { get; set; }

        [Display(Name = "失业起缴时间")]
        public DateTime? StartTime_4 { get; set; }

        [Display(Name = "失业社保月")]
        public DateTime? InsuranceMonth_4 { get; set; }

        [Display(Name = "失业是否单立户")]
        public string IsIndependentAccount_4 { get; set; }

        [Display(Name = "失业创建时间")]
        public DateTime? CreateTime_4 { get; set; }



        [Display(Name = "公积金险种编号")]
        public string InsuranceCode_5 { get; set; }

        [Display(Name = "公积金单位基数")]
        public decimal? CompanyNumber_5 { get; set; }

        [Display(Name = "公积金单位比例")]
        public decimal? CompanyPercent_5 { get; set; }

        [Display(Name = "公积金单位缴纳金额")]
        public decimal? CompanyMoney_5
        {
            get
            {
                if (CompanyNumber_5 == null || CompanyPercent_5 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_5;
                    decimal per = (decimal)CompanyPercent_5;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "公积金个人基数")]
        public decimal? EmployeeNumber_5 { get; set; }

        [Display(Name = "公积金个人比例")]
        public decimal? EmployeePercent_5 { get; set; }

        [Display(Name = "公积金个人缴纳金额")]
        public decimal? EmployeeMoney_5
        {
            get
            {
                if (EmployeeNumber_5 == null || EmployeePercent_5 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_5;
                    decimal per = (decimal)EmployeePercent_5;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "公积金状态")]
        public string State_5 { get; set; }

        [Display(Name = "公积金政策")]
        public string PoliceInsuranceName_5 { get; set; }

        [Display(Name = "公积金申报时间")]
        public int? YearMonth_5 { get; set; }


        [Display(Name = "公积金工资")]
        public decimal? Wage_5 { get; set; }

        [Display(Name = "公积金报增方式")]
        public string PoliceOperationName_5 { get; set; }

        [Display(Name = "公积金起缴时间")]
        public DateTime? StartTime_5 { get; set; }


        [Display(Name = "公积金社保月")]
        public DateTime? InsuranceMonth_5 { get; set; }

        [Display(Name = "公积金是否单立户")]
        public string IsIndependentAccount_5 { get; set; }

        [Display(Name = "公积金创建时间")]
        public DateTime? CreateTime_5 { get; set; }



        [Display(Name = "生育险种编号")]
        public string InsuranceCode_6 { get; set; }

        [Display(Name = "生育单位基数")]
        public decimal? CompanyNumber_6 { get; set; }

        [Display(Name = "生育单位比例")]
        public decimal? CompanyPercent_6 { get; set; }

        [Display(Name = "生育单位缴纳金额")]
        public decimal? CompanyMoney_6
        {
            get
            {
                if (CompanyNumber_6 == null || CompanyPercent_6 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_6;
                    decimal per = (decimal)CompanyPercent_6;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "生育个人基数")]
        public decimal? EmployeeNumber_6 { get; set; }

        [Display(Name = "生育个人比例")]
        public decimal? EmployeePercent_6 { get; set; }

        [Display(Name = "生育个人缴纳金额")]
        public decimal? EmployeeMoney_6
        {
            get
            {
                if (EmployeeNumber_6 == null || EmployeePercent_6 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_6;
                    decimal per = (decimal)EmployeePercent_6;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "生育状态")]
        public string State_6 { get; set; }

        [Display(Name = "生育政策")]
        public string PoliceInsuranceName_6 { get; set; }

        [Display(Name = "生育申报时间")]
        public int? YearMonth_6 { get; set; }


        [Display(Name = "生育工资")]
        public decimal? Wage_6 { get; set; }

        [Display(Name = "生育报增方式")]
        public string PoliceOperationName_6 { get; set; }

        [Display(Name = "生育起缴时间")]
        public DateTime? StartTime_6 { get; set; }

        [Display(Name = "生育社保月")]
        public DateTime? InsuranceMonth_6 { get; set; }

        [Display(Name = "生育是否单立户")]
        public string IsIndependentAccount_6 { get; set; }

        [Display(Name = "生育创建时间")]
        public DateTime? CreateTime_6 { get; set; }



        [Display(Name = "医疗大额险种编号")]
        public string InsuranceCode_7 { get; set; }

        [Display(Name = "医疗大额单位基数")]
        public decimal? CompanyNumber_7 { get; set; }

        [Display(Name = "医疗大额单位比例")]
        public decimal? CompanyPercent_7 { get; set; }

        [Display(Name = "医疗大额单位缴纳金额")]
        public decimal? CompanyMoney_7
        {
            get
            {
                if (CompanyNumber_7 == null || CompanyPercent_7 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_7;
                    decimal per = (decimal)CompanyPercent_7;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "医疗大额个人基数")]
        public decimal? EmployeeNumber_7 { get; set; }

        [Display(Name = "医疗大额个人比例")]
        public decimal? EmployeePercent_7 { get; set; }

        [Display(Name = "医疗大额个人缴纳金额")]
        public decimal? EmployeeMoney_7
        {
            get
            {
                if (EmployeeNumber_7 == null || EmployeePercent_7 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_7;
                    decimal per = (decimal)EmployeePercent_7;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "医疗大额状态")]
        public string State_7 { get; set; }

        [Display(Name = "医疗大额政策")]
        public string PoliceInsuranceName_7 { get; set; }

        [Display(Name = "医疗大额申报时间")]
        public int? YearMonth_7 { get; set; }


        [Display(Name = "医疗大额工资")]
        public decimal? Wage_7 { get; set; }

        [Display(Name = "医疗大额报增方式")]
        public string PoliceOperationName_7 { get; set; }

        [Display(Name = "医疗大额起缴时间")]
        public DateTime? StartTime_7 { get; set; }

        [Display(Name = "医疗大额社保月")]
        public DateTime? InsuranceMonth_7 { get; set; }

        [Display(Name = "医疗大额是否单立户")]
        public string IsIndependentAccount_7 { get; set; }

        [Display(Name = "医疗大额创建时间")]
        public DateTime? CreateTime_7 { get; set; }




        [Display(Name = "补充公积金险种编号")]
        public string InsuranceCode_8 { get; set; }

        [Display(Name = "补充公积金单位基数")]
        public decimal? CompanyNumber_8 { get; set; }

        [Display(Name = "补充公积金单位比例")]
        public decimal? CompanyPercent_8 { get; set; }

        [Display(Name = "补充公积金单位缴纳金额")]
        public decimal? CompanyMoney_8
        {
            get
            {
                if (CompanyNumber_8 == null || CompanyPercent_8 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_8;
                    decimal per = (decimal)CompanyPercent_8;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "补充公积金个人基数")]
        public decimal? EmployeeNumber_8 { get; set; }

        [Display(Name = "补充公积金个人比例")]
        public decimal? EmployeePercent_8 { get; set; }

        [Display(Name = "补充公积金个人缴纳金额")]
        public decimal? EmployeeMoney_8
        {
            get
            {
                if (EmployeeNumber_8 == null || EmployeePercent_8 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_8;
                    decimal per = (decimal)EmployeePercent_8;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "补充公积金状态")]
        public string State_8 { get; set; }

        [Display(Name = "补充公积金政策")]
        public string PoliceInsuranceName_8 { get; set; }

        [Display(Name = "补充公积金申报时间")]
        public int? YearMonth_8 { get; set; }


        [Display(Name = "补充公积金工资")]
        public decimal? Wage_8 { get; set; }

        [Display(Name = "补充公积金报增方式")]
        public string PoliceOperationName_8 { get; set; }

        [Display(Name = "补充公积金起缴时间")]
        public DateTime? StartTime_8 { get; set; }

        [Display(Name = "补充公积金社保月")]
        public DateTime? InsuranceMonth_8 { get; set; }

        [Display(Name = "补充公积金是否单立户")]
        public string IsIndependentAccount_8 { get; set; }

        [Display(Name = "补充公积金创建时间")]
        public DateTime? CreateTime_8 { get; set; }



        //
        [Display(Name = "大病险种编号")]
        public string InsuranceCode_9 { get; set; }

        [Display(Name = "大病单位基数")]
        public decimal? CompanyNumber_9 { get; set; }

        [Display(Name = "大病单位比例")]
        public decimal? CompanyPercent_9 { get; set; }

        [Display(Name = "大病单位缴纳金额")]
        public decimal? CompanyMoney_9
        {
            get
            {
                if (CompanyNumber_9 == null || CompanyPercent_9 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber_9;
                    decimal per = (decimal)CompanyPercent_9;
                    return Math.Round(num * per, 2);
                }
            }
        }


        [Display(Name = "大病个人基数")]
        public decimal? EmployeeNumber_9 { get; set; }

        [Display(Name = "大病个人比例")]
        public decimal? EmployeePercent_9 { get; set; }

        [Display(Name = "大病个人缴纳金额")]
        public decimal? EmployeeMoney_9
        {
            get
            {
                if (EmployeeNumber_9 == null || EmployeePercent_9 == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber_9;
                    decimal per = (decimal)EmployeePercent_9;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "大病状态")]
        public string State_9 { get; set; }

        [Display(Name = "大病政策")]
        public string PoliceInsuranceName_9 { get; set; }

        [Display(Name = "大病申报时间")]
        public int? YearMonth_9 { get; set; }


        [Display(Name = "大病工资")]
        public decimal? Wage_9 { get; set; }

        [Display(Name = "大病报增方式")]
        public string PoliceOperationName_9 { get; set; }

        [Display(Name = "大病起缴时间")]
        public DateTime? StartTime_9 { get; set; }

        [Display(Name = "大病社保月")]
        public DateTime? InsuranceMonth_9 { get; set; }

        [Display(Name = "大病是否单立户")]
        public string IsIndependentAccount_9 { get; set; }

        [Display(Name = "大病创建时间")]
        public DateTime? CreateTime_9 { get; set; }


        [Display(Name = "分公司名称（操作人所在的公司）")]
        public string Operator_CompanyName { get; set; }

    }
    #endregion

    #region 社保报增模板导入视图
    public class EmployeeAddExcelModel
    {
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }

        [Display(Name = "社保缴纳地")]
        public string City { get; set; }

        [Display(Name = "公司ID")]
        public string CompanyId { get; set; }

        [Display(Name = "社保缴纳地ID")]
        public string CityId { get; set; }

        [Display(Name = "员工ID")]
        public string EmployeeId { get; set; }

        [Display(Name = "员工姓名")]
        public string Name { get; set; }

        [Display(Name = "证件号码")]
        public string CertificateNumber { get; set; }

        [Display(Name = "证件类型")]
        public string CertificateType { get; set; }

        [Display(Name = "岗位")]
        public string Station { get; set; }

        [Display(Name = "社保户口性质")]
        public string PoliceAccountNatureName { get; set; }

        [Display(Name = "社保户口性质编号")]
        public string PoliceAccountNatureId { get; set; }

        [Display(Name = "社保起缴时间")]
        public string StartTime { get; set; }

        [Display(Name = "社保工资")]
        public string Wage { get; set; }

        [Display(Name = "公积金基数")]
        public string Wage_5 { get; set; }

        [Display(Name = "养老政策名称")]
        public string PoliceInsuranceName_1 { get; set; }

        [Display(Name = "养老报增方式")]
        public string PoliceOperationName_1 { get; set; }

        [Display(Name = "医疗政策名称")]
        public string PoliceInsuranceName_2 { get; set; }

        [Display(Name = "医疗报增方式")]
        public string PoliceOperationName_2 { get; set; }

        [Display(Name = "工伤政策名称")]
        public string PoliceInsuranceName_3 { get; set; }

        [Display(Name = "工伤报增方式")]
        public string PoliceOperationName_3 { get; set; }

        [Display(Name = "失业政策名称")]
        public string PoliceInsuranceName_4 { get; set; }

        [Display(Name = "失业报增方式")]
        public string PoliceOperationName_4 { get; set; }

        [Display(Name = "公积金政策名称")]
        public string PoliceInsuranceName_5 { get; set; }

        [Display(Name = "公积金报增方式")]
        public string PoliceOperationName_5 { get; set; }

        [Display(Name = "生育政策名称")]
        public string PoliceInsuranceName_6 { get; set; }

        [Display(Name = "生育报增方式")]
        public string PoliceOperationName_6 { get; set; }

        //[Display(Name = "养老政策ID")]
        //public string PoliceInsuranceId_1 { get; set; }

        //[Display(Name = "养老报增方式ID")]
        //public string PoliceOperationId_1 { get; set; }

        //public string PoliceInsuranceId_2 { get; set; }

        //public string PoliceOperationId_2 { get; set; }

        //public string PoliceInsuranceId_3 { get; set; }

        //public string PoliceOperationId_3 { get; set; }

        //public string PoliceInsuranceId_4 { get; set; }

        //public string PoliceOperationId_4 { get; set; }

        //public string PoliceInsuranceId_5 { get; set; }

        //public string PoliceOperationId_5 { get; set; }

        //public string PoliceInsuranceId_6 { get; set; }

        //public string PoliceOperationId_6 { get; set; }


        [Display(Name = "户口类型")]
        public string AccountType { get; set; }

        [Display(Name = "户口所在地")]
        public string AccountAddress { get; set; }

        [Display(Name = "联系电话")]
        public string MobilePhone { get; set; }

        [Display(Name = "联系地址")]
        public string Address { get; set; }

        [Display(Name = "社保报增信息")]
        public List<EmployeeAddItem> employeeAddList { get; set; }

        public string StartTime_5 { get; set; }
    }


    public class EmployeeAddItem
    {
        public string PoliceInsuranceId { get; set; }

        public string PoliceOperationId { get; set; }

        public string PoliceInsuranceName { get; set; }

        public string PoliceOperationName { get; set; }
    }

    public class EmployeeAddModel
    {
        public Employee employee { get; set; }

        public List<EmployeeAdd> employeeAddList { get; set; }

        public bool employeeMark { get; set; }

        public EmployeeContact employeeContact { get; set; }

        public bool employeeContactMark { get; set; }
    }
    #endregion

    #region 新报增
    public class EmployeeAddExcelModel1
    {
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }

        [Display(Name = "社保缴纳地")]
        public string City { get; set; }

        [Display(Name = "公司ID")]
        public string CompanyId { get; set; }

        [Display(Name = "社保缴纳地ID")]
        public string CityId { get; set; }

        [Display(Name = "员工ID")]
        public string EmployeeId { get; set; }

        [Display(Name = "员工姓名")]
        public string Name { get; set; }

        [Display(Name = "证件号码")]
        public string CertificateNumber { get; set; }

        [Display(Name = "证件类型")]
        public string CertificateType { get; set; }

        [Display(Name = "岗位")]
        public string Station { get; set; }

        [Display(Name = "社保户口性质")]
        public string PoliceAccountNatureName { get; set; }

        [Display(Name = "社保户口性质编号")]
        public string PoliceAccountNatureId { get; set; }

        [Display(Name = "社保起缴时间")]
        public string StartTime { get; set; }

        [Display(Name = "社保工资")]
        public string Wage { get; set; }

        [Display(Name = "公积金基数")]
        public string Wage_5 { get; set; }


        [Display(Name = "户口类型")]
        public string AccountType { get; set; }

        [Display(Name = "户口所在地")]
        public string AccountAddress { get; set; }

        [Display(Name = "联系电话")]
        public string MobilePhone { get; set; }

        [Display(Name = "联系地址")]
        public string Address { get; set; }

        [Display(Name = "社保报增信息")]
        public List<EmployeeAddItem> employeeAddList { get; set; }

        public string StartTime_5 { get; set; }
        public List<Insurance> Insurance { get; set; }
        public string SupplierName { get; set; }
    }
    public class Insurance
    {
        public int? Id { get; set; }//
        public string InsuranceKind { get; set; }//
        public decimal? Wage { get; set; }//养老工资
        public string StartTime { get; set; }//起缴时间
        public string SocialInsuranceMonth { get; set; }//社保月
        // public string Pension_Mode { get; set; }//社保报增方式

        public string InsuranceNumber { get; set; }//社保编号
        public int? PoliceInsurance { get; set; }//社保政策标示
        public string PoliceInsurancename { get; set; }//社保政策标示
        public int? PoliceOperation { get; set; }//政策手续
        public string PoliceOperationname { get; set; }//政策手续
        public string SupplierRemark { get; set; }//备注
    }
    #endregion


    public class EmployeeMiddleExcelModel
    {

        [Display(Name = "社保缴纳地")]
        public string City { get; set; }

        [Display(Name = "公司ID")]
        public string CompanyId { get; set; }

        [Display(Name = "社保缴纳地ID")]
        public string CityId { get; set; }

        [Display(Name = "员工ID")]
        public string EmployeeId { get; set; }

        [Display(Name = "员工姓名")]
        public string Name { get; set; }

        [Display(Name = "证件号码")]
        public string CertificateNumber { get; set; }

        [Display(Name = "证件类型")]
        public string CertificateType { get; set; }

        public string CompanyPayment { get; set; }

        public string EmployeePayment { get; set; }

        public string Remark { get; set; }
    }


}