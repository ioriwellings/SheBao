using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using Langben.App.Models;

namespace Langben.App.Controllers
{
    public class AddEmployeeApiController : BaseApiController
    {
        //  报增
        #region


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/addemployee/{id}")]
        public IHttpActionResult Index(int id, PostInfo postinfo)
        {

            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }
            try
            {
                var postinfos = TransDate(postinfo);
                var CompanyId = id;
                using (SysEntities SysEntitiesO2O = new SysEntities())
                {
                    Employee employee = new Employee();//员工表
                    EmployeeContact employeeContact = new EmployeeContact();//联系人表
                    CompanyEmployeeRelation companyEmployeeRelation = new CompanyEmployeeRelation();//员工企业关系表
                    //员工
                    employee.Name = postinfos.Name;
                    employee.CertificateType = postinfos.IDType;
                    employee.CertificateNumber = postinfos.IDNumber;
                    employee.AccountType = postinfos.ResidentType;
                    SysEntitiesO2O.Employee.Add(employee);
                    //联系人
                    employeeContact.MobilePhone = postinfos.Telephone;
                    employeeContact.Address = postinfos.ResidentLocation;
                    SysEntitiesO2O.EmployeeContact.Add(employeeContact);
                    SysEntitiesO2O.SaveChanges();

                    //员工关系表
                    companyEmployeeRelation.CityId = postinfos.City;
                    companyEmployeeRelation.CompanyId = CompanyId;
                    companyEmployeeRelation.EmployeeId = employee.Id;
                    companyEmployeeRelation.State = "在职";
                    companyEmployeeRelation.CreateTime = DateTime.Now;
                    SysEntitiesO2O.CompanyEmployeeRelation.Add(companyEmployeeRelation);
                    SysEntitiesO2O.SaveChanges();
                    DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
                    DateTime QJ_Yanglao = DateTime.MinValue;
                    DateTime QJ_Yiliao = DateTime.MinValue;
                    DateTime QJ_Gongshang = DateTime.MinValue;
                    DateTime QJ_Shiye = DateTime.MinValue;
                    DateTime QJ_Gongjijin = DateTime.MinValue;
                    DateTime QJ_Shengyu = DateTime.MinValue;
                    DateTime.TryParse(postinfos.Pension_PaymentTime, out QJ_Yanglao);
                    DateTime.TryParse(postinfos.Medical_PaymentTime, out QJ_Yiliao);
                    DateTime.TryParse(postinfos.WorkInjury_PaymentTime, out QJ_Gongshang);
                    DateTime.TryParse(postinfos.Unemployment_PaymentTime, out QJ_Shiye);
                    DateTime.TryParse(postinfos.HousingFund_PaymentTime, out QJ_Gongjijin);
                    DateTime.TryParse(postinfos.Maternity_PaymentTime, out QJ_Shengyu);



                    #region 养老
                    if (QJ_Yanglao != DateTime.MinValue)
                    {
                        decimal GZ_Yanglao = postinfos.Pension_Wage;
                        int ZC_Yanglao_ID = postinfos.Pension_Percentage;
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                        EmployeeAdd employeeAdd = new EmployeeAdd();
                        employeeAdd.Wage = GZ_Yanglao;//工资

                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Yanglao_ID);
                        employeeAdd.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;//社保种类
                        employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelation.Id;//员工关系
                        employeeAdd.PoliceInsuranceId = ZC_Yanglao_ID;//社保政策
                        employeeAdd.InsuranceCode = postinfos.Pension_InsuranceNumber;//社保编号
                        employeeAdd.PoliceOperationId = postinfos.Pension_PoliceOperation;//政策手续
                        employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                        employeeAdd.StartTime = QJ_Yanglao;//起缴时间
                        employeeAdd.CreateTime = DateTime.Now;
                        // employeeAdd.CreatePerson = UID;
                        employeeAdd.PoliceAccountNatureId = postinfos.Pension_PoliceAccountNature;//户口性质
                        // employeeAdd.SuppliersId = Enums.SHEBAO_STATUS.客服已确认;供应商
                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                        employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                        SysEntitiesO2O.EmployeeAdd.Add(employeeAdd);


                        #region 正常
                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                        employeeMiddle.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                        employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelation.Id;
                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        employeeMiddle.CompanyBasePayment = JISHU_C;
                        employeeMiddle.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                        employeeMiddle.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                        employeeMiddle.State = "1";//正常
                        employeeMiddle.CityId = postinfos.City;
                        SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle);
                        #endregion
                        #region 补缴
                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Yanglao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                        {
                            Int32 Months = Business.CHA_Months(QJ_Yanglao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                            // FWF_Months = Months > FWF_Months ? Months : FWF_Months; //服务费补缴月数
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                            //employeeMiddle.CompanyEmployeeRelationId = P_BAOXIAN_POLICY.CID;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = "1";//正常
                            employeeMiddle.CityId = postinfos.City;
                            SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle_BJ);
                        }
                        #endregion


                    }
                    #endregion

                    #region 医疗
                    if (QJ_Yiliao != DateTime.MinValue)
                    {
                        decimal GZ_Yiliao = postinfos.Medical_Wage;
                        int ZC_Yiliao_ID = postinfos.Medical_Percentage;
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                        EmployeeAdd employeeAdd = new EmployeeAdd();
                        employeeAdd.Wage = GZ_Yiliao;//工资

                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Yiliao_ID);
                        employeeAdd.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;//社保种类
                        employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelation.Id;//员工关系
                        employeeAdd.PoliceInsuranceId = ZC_Yiliao_ID;//社保政策
                        employeeAdd.InsuranceCode = postinfos.Medical_InsuranceNumber;//社保编号
                        employeeAdd.PoliceOperationId = postinfos.Medical_PoliceOperation;//政策手续
                        employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                        employeeAdd.StartTime = QJ_Yiliao;//起缴时间
                        employeeAdd.CreateTime = DateTime.Now;
                        // employeeAdd.CreatePerson = UID;
                        employeeAdd.PoliceAccountNatureId = postinfos.Medical_PoliceAccountNature;//户口性质
                        // employeeAdd.SuppliersId = Enums.SHEBAO_STATUS.客服已确认;供应商
                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                        employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                        SysEntitiesO2O.EmployeeAdd.Add(employeeAdd);


                        #region 正常
                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                        employeeMiddle.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                        employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelation.Id;
                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        employeeMiddle.CompanyBasePayment = JISHU_C;
                        employeeMiddle.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                        employeeMiddle.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                        employeeMiddle.State = "1";//正常
                        employeeMiddle.CityId = postinfos.City;
                        SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle);
                        #endregion
                        #region 补缴
                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Yiliao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                        {
                            Int32 Months = Business.CHA_Months(QJ_Yiliao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                            // FWF_Months = Months > FWF_Months ? Months : FWF_Months; //服务费补缴月数
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                            //employeeMiddle.CompanyEmployeeRelationId = P_BAOXIAN_POLICY.CID;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = "1";//正常
                            employeeMiddle.CityId = postinfos.City;
                            SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle_BJ);
                        }
                        #endregion


                    }
                    #endregion

                    #region 工伤
                    if (QJ_Gongshang != DateTime.MinValue)
                    {
                        decimal GZ_Gongshang = postinfos.WorkInjury_Wage;
                        int ZC_Gongshang_ID = postinfos.WorkInjury_Percentage;
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                        EmployeeAdd employeeAdd = new EmployeeAdd();
                        employeeAdd.Wage = GZ_Gongshang;//工资

                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Gongshang_ID);
                        employeeAdd.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;//社保种类
                        employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelation.Id;//员工关系
                        employeeAdd.PoliceInsuranceId = ZC_Gongshang_ID;//社保政策
                        employeeAdd.InsuranceCode = postinfos.WorkInjury_InsuranceNumber;//社保编号
                        employeeAdd.PoliceOperationId = postinfos.WorkInjury_PoliceOperation;//政策手续
                        employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                        employeeAdd.StartTime = QJ_Gongshang;//起缴时间
                        employeeAdd.CreateTime = DateTime.Now;
                        // employeeAdd.CreatePerson = UID;
                        employeeAdd.PoliceAccountNatureId = postinfos.WorkInjury_PoliceAccountNature;//户口性质
                        // employeeAdd.SuppliersId = Enums.SHEBAO_STATUS.客服已确认;供应商
                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                        employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                        SysEntitiesO2O.EmployeeAdd.Add(employeeAdd);


                        #region 正常
                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                        employeeMiddle.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                        employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelation.Id;
                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        employeeMiddle.CompanyBasePayment = JISHU_C;
                        employeeMiddle.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                        employeeMiddle.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                        employeeMiddle.State = "1";//正常
                        employeeMiddle.CityId = postinfos.City;
                        SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle);
                        #endregion
                        #region 补缴
                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Gongshang, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                        {
                            Int32 Months = Business.CHA_Months(QJ_Gongshang, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                            // FWF_Months = Months > FWF_Months ? Months : FWF_Months; //服务费补缴月数
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                            //employeeMiddle.CompanyEmployeeRelationId = P_BAOXIAN_POLICY.CID;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = "1";//正常
                            employeeMiddle.CityId = postinfos.City;
                            SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle_BJ);
                        }
                        #endregion


                    }
                    #endregion

                    #region 失业
                    if (QJ_Shiye != DateTime.MinValue)
                    {
                        decimal GZ_Shiye = postinfos.Unemployment_Wage;
                        int ZC_Shiye_ID = postinfos.Unemployment_Percentage;
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                        EmployeeAdd employeeAdd = new EmployeeAdd();
                        employeeAdd.Wage = GZ_Shiye;//工资

                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Shiye_ID);
                        employeeAdd.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;//社保种类
                        employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelation.Id;//员工关系
                        employeeAdd.PoliceInsuranceId = ZC_Shiye_ID;//社保政策
                        employeeAdd.InsuranceCode = postinfos.Unemployment_InsuranceNumber;//社保编号
                        employeeAdd.PoliceOperationId = postinfos.Unemployment_PoliceOperation;//政策手续
                        employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                        employeeAdd.StartTime = QJ_Shiye;//起缴时间
                        employeeAdd.CreateTime = DateTime.Now;
                        // employeeAdd.CreatePerson = UID;
                        employeeAdd.PoliceAccountNatureId = postinfos.Unemployment_PoliceAccountNature;//户口性质
                        // employeeAdd.SuppliersId = Enums.SHEBAO_STATUS.客服已确认;供应商
                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                        employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                        SysEntitiesO2O.EmployeeAdd.Add(employeeAdd);


                        #region 正常
                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                        employeeMiddle.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                        employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelation.Id;
                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        employeeMiddle.CompanyBasePayment = JISHU_C;
                        employeeMiddle.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                        employeeMiddle.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                        employeeMiddle.State = "1";//正常
                        employeeMiddle.CityId = postinfos.City;
                        SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle);
                        #endregion
                        #region 补缴
                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Shiye, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                        {
                            Int32 Months = Business.CHA_Months(QJ_Shiye, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                            // FWF_Months = Months > FWF_Months ? Months : FWF_Months; //服务费补缴月数
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤; ;
                            //employeeMiddle.CompanyEmployeeRelationId = P_BAOXIAN_POLICY.CID;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = "1";//正常
                            employeeMiddle.CityId = postinfos.City;
                            SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle_BJ);
                        }
                        #endregion


                    }
                    #endregion

                    #region 公积金
                    if (QJ_Gongjijin != DateTime.MinValue)
                    {
                        decimal GZ_Gongjijin = postinfos.HousingFund_Wage;
                        int ZC_Gongjijin_ID = postinfos.HousingFund_Percentage;
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                        EmployeeAdd employeeAdd = new EmployeeAdd();
                        employeeAdd.Wage = GZ_Gongjijin;//工资

                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Gongjijin_ID);
                        employeeAdd.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;//社保种类
                        employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelation.Id;//员工关系
                        employeeAdd.PoliceInsuranceId = ZC_Gongjijin_ID;//社保政策
                        employeeAdd.InsuranceCode = postinfos.HousingFund_InsuranceNumber;//社保编号
                        employeeAdd.PoliceOperationId = postinfos.HousingFund_PoliceOperation;//政策手续
                        employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                        employeeAdd.StartTime = QJ_Gongjijin;//起缴时间
                        employeeAdd.CreateTime = DateTime.Now;
                        // employeeAdd.CreatePerson = UID;
                        employeeAdd.PoliceAccountNatureId = postinfos.HousingFund_PoliceAccountNature;//户口性质
                        // employeeAdd.SuppliersId = Enums.SHEBAO_STATUS.客服已确认;供应商
                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                        employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                        SysEntitiesO2O.EmployeeAdd.Add(employeeAdd);


                        #region 正常
                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                        employeeMiddle.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                        employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelation.Id;
                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        employeeMiddle.CompanyBasePayment = JISHU_C;
                        employeeMiddle.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                        employeeMiddle.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                        employeeMiddle.State = "1";//正常
                        employeeMiddle.CityId = postinfos.City;
                        SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle);
                        #endregion
                        #region 补缴
                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Gongjijin, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                        {
                            Int32 Months = Business.CHA_Months(QJ_Gongjijin, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                            // FWF_Months = Months > FWF_Months ? Months : FWF_Months; //服务费补缴月数
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                            //employeeMiddle.CompanyEmployeeRelationId = P_BAOXIAN_POLICY.CID;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = "1";//正常
                            employeeMiddle.CityId = postinfos.City;
                            SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle_BJ);
                        }
                        #endregion


                    }
                    #endregion

                    #region 生育
                    if (QJ_Shengyu != DateTime.MinValue)
                    {
                        decimal GZ_Shengyu = postinfos.Maternity_Wage;
                        int ZC_Shengyu_ID = postinfos.Maternity_Percentage;
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                        EmployeeAdd employeeAdd = new EmployeeAdd();
                        employeeAdd.Wage = GZ_Shengyu;//工资

                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Shengyu_ID);
                        employeeAdd.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;//社保种类
                        employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelation.Id;//员工关系
                        employeeAdd.PoliceInsuranceId = ZC_Shengyu_ID;//社保政策
                        employeeAdd.InsuranceCode = postinfos.Maternity_InsuranceNumber;//社保编号
                        employeeAdd.PoliceOperationId = postinfos.Maternity_PoliceOperation;//政策手续
                        employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                        employeeAdd.StartTime = QJ_Shengyu;//起缴时间
                        employeeAdd.CreateTime = DateTime.Now;
                        // employeeAdd.CreatePerson = UID;
                        employeeAdd.PoliceAccountNatureId = postinfos.Maternity_PoliceAccountNature;//户口性质
                        // employeeAdd.SuppliersId = Enums.SHEBAO_STATUS.客服已确认;供应商
                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                        employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                        SysEntitiesO2O.EmployeeAdd.Add(employeeAdd);


                        #region 正常
                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                        employeeMiddle.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                        employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelation.Id;
                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        employeeMiddle.CompanyBasePayment = JISHU_C;
                        employeeMiddle.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                        employeeMiddle.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                        employeeMiddle.State = "1";//正常
                        employeeMiddle.CityId = postinfos.City;
                        SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle);
                        #endregion
                        #region 补缴
                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Shengyu, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                        {
                            Int32 Months = Business.CHA_Months(QJ_Shengyu, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                            // FWF_Months = Months > FWF_Months ? Months : FWF_Months; //服务费补缴月数
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                            //employeeMiddle.CompanyEmployeeRelationId = P_BAOXIAN_POLICY.CID;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = "1";//正常
                            employeeMiddle.CityId = postinfos.City;
                            SysEntitiesO2O.EmployeeMiddle.Add(employeeMiddle_BJ);
                        }
                        #endregion


                    }
                    #endregion

                    SysEntitiesO2O.SaveChanges();

                    return Json(new { code = 0, message = "成功" });
                }
            }
            catch (Exception ee)
            {

                return Json(new { code = -1, message = ee.ToString() });
            }
        }


        public PostInfo2 TransDate(PostInfo postinfo)
        {
            var postInfo2 = new PostInfo2();

            postInfo2.Name = postinfo.Name;
            postInfo2.Name = postinfo.Name;
            postInfo2.IDNumber = postinfo.IDNumber;
            postInfo2.Telephone = postinfo.Telephone;
            postInfo2.ResidentType = postinfo.ResidentType;
            postInfo2.ResidentLocation = postinfo.ResidentLocation;
            postInfo2.SSResidentType = postinfo.SSResidentType;
            postInfo2.City = postinfo.Cityid;
            postInfo2.attachments = postinfo.attachments;

            if (postinfo.InsuranceType.Where(a => a.Insurance == "养老").FirstOrDefault() != null)
            {
                var Pension = postinfo.InsuranceType.Where(a => a.Insurance == "养老").FirstOrDefault();

                postInfo2.Pension_Wage = Convert.ToDecimal(Pension.Wage);
                postInfo2.Pension_PaymentTime = Pension.PaymentTime;
                postInfo2.Pension_SocialInsuranceMonth = Pension.SocialInsuranceMonth;
                postInfo2.Pension_Mode = Pension.Mode;
                postInfo2.Pension_InsuranceNumber = Pension.InsuranceNumber;
                postInfo2.Pension_Percentage = Pension.PoliceInsuranceid;
                postInfo2.Pension_PoliceOperation = Pension.PoliceOperationId;
                postInfo2.Pension_PoliceAccountNature = Pension.PoliceAccountNatureId;
            }

            if (postinfo.InsuranceType.Where(a => a.Insurance == "医疗").FirstOrDefault() != null)
            {
                var Medical = postinfo.InsuranceType.Where(a => a.Insurance == "医疗").FirstOrDefault();

                postInfo2.Medical_Wage = Convert.ToDecimal(Medical.Wage);
                postInfo2.Medical_PaymentTime = Medical.PaymentTime;
                postInfo2.Medical_SocialInsuranceMonth = Medical.SocialInsuranceMonth;
                postInfo2.Medical_Mode = Medical.Mode;
                postInfo2.Medical_InsuranceNumber = Medical.InsuranceNumber;
                postInfo2.Medical_Percentage = Medical.PoliceInsuranceid;
                postInfo2.Medical_PoliceOperation = Medical.PoliceOperationId;
                postInfo2.Medical_PoliceAccountNature = Medical.PoliceAccountNatureId;
            }

            if (postinfo.InsuranceType.Where(a => a.Insurance == "工伤").FirstOrDefault() != null)
            {
                var WorkInjury = postinfo.InsuranceType.Where(a => a.Insurance == "工伤").FirstOrDefault();

                postInfo2.WorkInjury_Wage = Convert.ToDecimal(WorkInjury.Wage);
                postInfo2.WorkInjury_PaymentTime = WorkInjury.PaymentTime;
                postInfo2.WorkInjury_SocialInsuranceMonth = WorkInjury.SocialInsuranceMonth;
                postInfo2.WorkInjury_Mode = WorkInjury.Mode;
                postInfo2.WorkInjury_InsuranceNumber = WorkInjury.InsuranceNumber;
                postInfo2.WorkInjury_Percentage = WorkInjury.PoliceInsuranceid;
                postInfo2.WorkInjury_PoliceOperation = WorkInjury.PoliceOperationId;
                postInfo2.WorkInjury_PoliceAccountNature = WorkInjury.PoliceAccountNatureId;
            }

            if (postinfo.InsuranceType.Where(a => a.Insurance == "失业").FirstOrDefault() != null)
            {
                var Unemployment = postinfo.InsuranceType.Where(a => a.Insurance == "失业").FirstOrDefault();

                postInfo2.Unemployment_Wage = Convert.ToDecimal(Unemployment.Wage);
                postInfo2.Unemployment_PaymentTime = Unemployment.PaymentTime;
                postInfo2.Unemployment_SocialInsuranceMonth = Unemployment.SocialInsuranceMonth;
                postInfo2.Unemployment_Mode = Unemployment.Mode;
                postInfo2.Unemployment_InsuranceNumber = Unemployment.InsuranceNumber;
                postInfo2.Unemployment_Percentage = Unemployment.PoliceInsuranceid;
                postInfo2.Unemployment_PoliceOperation = Unemployment.PoliceOperationId;
                postInfo2.Unemployment_PoliceAccountNature = Unemployment.PoliceAccountNatureId;
            }

            if (postinfo.InsuranceType.Where(a => a.Insurance == "公积金").FirstOrDefault() != null)
            {

                var HousingFund = postinfo.InsuranceType.Where(a => a.Insurance == "公积金").FirstOrDefault();

                postInfo2.HousingFund_Wage = Convert.ToDecimal(HousingFund.Wage);
                postInfo2.HousingFund_PaymentTime = HousingFund.PaymentTime;
                postInfo2.HousingFund_SocialInsuranceMonth = HousingFund.SocialInsuranceMonth;
                postInfo2.HousingFund_Mode = HousingFund.Mode;
                postInfo2.HousingFund_InsuranceNumber = HousingFund.InsuranceNumber;
                postInfo2.HousingFund_Percentage = HousingFund.PoliceInsuranceid;
                postInfo2.HousingFund_PoliceOperation = HousingFund.PoliceOperationId;
                postInfo2.HousingFund_PoliceAccountNature = HousingFund.PoliceAccountNatureId;
            }
            if (postinfo.InsuranceType.Where(a => a.Insurance == "生育").FirstOrDefault() != null)
            {
                var Maternity = postinfo.InsuranceType.Where(a => a.Insurance == "生育").FirstOrDefault();

                postInfo2.Maternity_Wage = Convert.ToDecimal(Maternity.Wage);
                postInfo2.Maternity_PaymentTime = Maternity.PaymentTime;
                postInfo2.Maternity_SocialInsuranceMonth = Maternity.SocialInsuranceMonth;
                postInfo2.Maternity_Mode = Maternity.Mode;
                postInfo2.Maternity_InsuranceNumber = Maternity.InsuranceNumber;
                postInfo2.Maternity_Percentage = Maternity.PoliceInsuranceid;
                postInfo2.Maternity_PoliceOperation = Maternity.PoliceOperationId;
                postInfo2.Maternity_PoliceAccountNature = Maternity.PoliceAccountNatureId;
            }

            return postInfo2;
        }

        #region 保留两位小数，四舍五入
        /// <summary>
        /// 保留两位小数，四舍五入
        /// </summary>
        /// <param name="SHU"></param>
        /// <returns></returns>
        public static decimal Get_TwoXiaoshu(decimal SHU)
        {
            return Math.Round(SHU, 2, MidpointRounding.AwayFromZero);
        }
        public static int CHA_Months(DateTime dt1, DateTime dt2)
        {
            int cha = 0;
            cha = (dt2.Year - dt1.Year) * 12 + dt2.Month - dt1.Month;
            return cha;
        }
        #endregion

        //{
        //    "Name": "刘腾飞", 
        //    "IDType": "身份证", 
        //    "IDNumber": "123456789012345678", 
        //    "Telephone": "13876543210", 
        //    " ResidentType ": "农业", 
        //    " ResidentLocation ": "山东省威海市乳山县XX镇派出所", 
        //    "Position": "工程师", 
        //    "SSResidentType": "本地城镇", 
        //    "Province": "山东省", 
        //    "City": "威海市", 
        //    "InsuranceType": [
        //        {
        //            "Insurance": "Pension", 
        //            "Wage": "5565", 
        //            "PaymentTime": "2015-7", 
        //            "SocialInsuranceMonth": "2015-8", 
        //            "Mode": "新增", 
        //            "InsuranceNumber": "55121365"
        //        }, 
        //        {
        //            "Insurance": "medical", 
        //            "Wage": "5565", 
        //            "PaymentTime": "2015-7", 
        //            "SocialInsuranceMonth": "2015-8", 
        //            "Mode": "新增", 
        //            "InsuranceNumber": "55121365"
        //        }, 
        //        {
        //            "Insurance": "WorkInjury", 
        //            "Wage": "5565", 
        //            "PaymentTime": "2015-7", 
        //            "SocialInsuranceMonth": "2015-9", 
        //            "Mode": "新增", 
        //            "InsuranceNumber": "55121365"
        //        }, 
        //        {
        //            "Insurance": "Unemployment ", 
        //            "Wage": "5565", 
        //            "PaymentTime": "2015-7", 
        //            "SocialInsuranceMonth": "2015-8", 
        //            "Mode": "新增", 
        //            "InsuranceNumber": "55121365"
        //        }, 
        //        {
        //            "Insurance": "Maternity", 
        //            "Wage": "5565", 
        //            "PaymentTime": "2015-7", 
        //            "SocialInsuranceMonth": "2015-8", 
        //            "Mode": "新增", 
        //            "InsuranceNumber": "55121365"
        //        }, 
        //        {
        //            "Insurance": "HousingFund", 
        //            "Wage": "5565", 
        //            "PaymentTime": "2015-7", 
        //            "SocialInsuranceMonth": "2015-8", 
        //            "Mode": "新增", 
        //            "InsuranceNumber": "55121365"
        //        }
        //    ], 
        //    " attachments ": "form.xls；photo.jpg;cardID.jpg"
        //}
        public class PostInfo2
        {

            public string Name { get; set; }
            public string IDType { get; set; }
            public string IDNumber { get; set; }

            public string Telephone { get; set; }
            public string ResidentType { get; set; }
            public string ResidentLocation { get; set; }

            public string SSResidentType { get; set; }
            public string City { get; set; }
            public string attachments { get; set; }


            #region 养老
            public decimal Pension_Wage { get; set; }//养老工资
            public string Pension_PaymentTime { get; set; }//起缴时间
            public string Pension_SocialInsuranceMonth { get; set; }//社保月
            public string Pension_Mode { get; set; }//社保报增方式

            public string Pension_InsuranceNumber { get; set; }//社保编号
            public int Pension_Percentage { get; set; }//社保政策标示
            public int Pension_PoliceOperation { get; set; }//政策手续
            public int Pension_PoliceAccountNature { get; set; }//户口性质

            #endregion

            #region
            public decimal Medical_Wage { get; set; }
            public string Medical_PaymentTime { get; set; }
            public string Medical_SocialInsuranceMonth { get; set; }
            public string Medical_Mode { get; set; }
            public string Medical_InsuranceNumber { get; set; }
            public int Medical_Percentage { get; set; }//社保政策标示
            public int Medical_PoliceOperation { get; set; }//政策手续
            public int Medical_PoliceAccountNature { get; set; }//户口性质



            public decimal WorkInjury_Wage { get; set; }
            public string WorkInjury_PaymentTime { get; set; }
            public string WorkInjury_SocialInsuranceMonth { get; set; }
            public string WorkInjury_Mode { get; set; }
            public string WorkInjury_InsuranceNumber { get; set; }
            public int WorkInjury_Percentage { get; set; }//社保政策标示
            public int WorkInjury_PoliceOperation { get; set; }//政策手续
            public int WorkInjury_PoliceAccountNature { get; set; }//户口性质

            public decimal Unemployment_Wage { get; set; }
            public string Unemployment_PaymentTime { get; set; }
            public string Unemployment_SocialInsuranceMonth { get; set; }
            public string Unemployment_Mode { get; set; }
            public string Unemployment_InsuranceNumber { get; set; }
            public int Unemployment_Percentage { get; set; }//社保政策标示
            public int Unemployment_PoliceOperation { get; set; }//政策手续
            public int Unemployment_PoliceAccountNature { get; set; }//户口性质


            public decimal HousingFund_Wage { get; set; }
            public string HousingFund_PaymentTime { get; set; }
            public string HousingFund_SocialInsuranceMonth { get; set; }
            public string HousingFund_Mode { get; set; }
            public string HousingFund_InsuranceNumber { get; set; }
            public int HousingFund_Percentage { get; set; }//社保政策标示
            public int HousingFund_PoliceOperation { get; set; }//政策手续
            public int HousingFund_PoliceAccountNature { get; set; }//户口性质

            public decimal Maternity_Wage { get; set; }
            public string Maternity_PaymentTime { get; set; }
            public string Maternity_SocialInsuranceMonth { get; set; }
            public string Maternity_Mode { get; set; }
            public string Maternity_InsuranceNumber { get; set; }
            public int Maternity_Percentage { get; set; }//社保政策标示
            public int Maternity_PoliceOperation { get; set; }//政策手续
            public int Maternity_PoliceAccountNature { get; set; }//户口性质
            #endregion
        }

        public class PostInfo
        {

            public string Name { get; set; }
            public string IDType { get; set; }
            public string IDNumber { get; set; }

            public string Telephone { get; set; }
            public string ResidentType { get; set; }
            public string ResidentLocation { get; set; }
            public string SSResidentType { get; set; }
            public string Cityid { get; set; }
            public string attachments { get; set; }

            public List<InsuranceType> InsuranceType { get; set; }
        }

        public class InsuranceType
        {
            public string Insurance { get; set; }
            public string Wage { get; set; }
            public string PaymentTime { get; set; }
            public string SocialInsuranceMonth { get; set; }
            public string Mode { get; set; }
            public string InsuranceNumber { get; set; }

            public int PoliceInsuranceid { get; set; }//社保政策标示
            public int PoliceOperationId { get; set; }//政策手续
            public int PoliceAccountNatureId { get; set; }//户口性质
        }

        #endregion

    }
}
