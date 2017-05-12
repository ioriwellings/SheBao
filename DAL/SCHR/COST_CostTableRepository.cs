using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
using Langben.DAL.Model;
namespace Langben.DAL
{
    /// <summary>
    /// 费用表为主表的相关方法
    /// </summary>
    public partial class COST_CostTableRepository
    {

        /// <summary>
        /// 通过主键id，获取费用_费用表---查看详细，首次编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>费用_费用表</returns>
        public CostFeeModel GetCostFeeModelById(SysEntities db, int id)
        {
            var query = from cost in db.COST_CostTable
                        where cost.ID == id
                        join company in db.CRM_Company on cost.CRM_Company_ID equals company.ID
                        into fee
                        from l in fee.DefaultIfEmpty()
                        select new CostFeeModel()
                        {
                            ID = cost.ID,
                            CostTableType = cost.CostTableType,
                            YearMonth = cost.YearMonth,
                            SerialNumber = cost.SerialNumber,
                            ChargeCost = cost.ChargeCost,
                            ServiceCost = cost.ServiceCost,
                            Remark = cost.Remark,
                            Status = cost.Status,
                            CRM_Company_ID = cost.CRM_Company_ID,
                            CompanyName = l.CompanyName,
                            CompanyCode = l.CompanyCode,
                            CreateTime = cost.CreateTime,
                            CreateUserID = cost.CreateUserID,
                            CreateUserName = cost.CreateUserName,
                            BranchID = cost.BranchID
                        };
            return query.ToList().FirstOrDefault();
        }
        /// <summary>
        /// 查询的数据（费用自检列表）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <param name="dateTime">时间</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="costTableType">费用表类型</param>
        /// <param name="listQuery">额外的参数</param>
        /// <returns></returns>
        public IOrderedQueryable<CostFeeModel> GetCostFeeList(SysEntities db, string dateTime, List<int> companyId, int costTableType, int status)
        {
            var queryLeft = from cost in db.COST_CostTable
                            where cost.CreateFrom == (int)Common.CostTable_CreateFrom.本地费用
                            join company in db.CRM_Company on cost.CRM_Company_ID equals company.ID
                                into fee
                            from l in fee.DefaultIfEmpty()
                            select new CostFeeModel()
                            {
                                ID = cost.ID,
                                CostTableType = cost.CostTableType,
                                YearMonth = cost.YearMonth,
                                SerialNumber = cost.SerialNumber,
                                ChargeCost = cost.ChargeCost,
                                ServiceCost = cost.ServiceCost,
                                Remark = cost.Remark,
                                Status = cost.Status,
                                CRM_Company_ID = cost.CRM_Company_ID,
                                CompanyName = l.CompanyName,
                                CreateTime = cost.CreateTime,
                                CreateUserID = cost.CreateUserID,
                                CreateUserName = cost.CreateUserName,
                                BranchID = cost.BranchID
                            };
            if (!string.IsNullOrEmpty(dateTime))
            {
                var datetime_Int = Convert.ToInt32(dateTime);
                queryLeft = queryLeft.Where(x => x.YearMonth == datetime_Int);
            }
            if (companyId != null)
            {
                queryLeft = queryLeft.Where(x => companyId.Contains(x.CRM_Company_ID));
            }
            if (costTableType != 0)
            {
                queryLeft = queryLeft.Where(x => x.CostTableType == costTableType);
            }
            if (status != 0)
            {
                queryLeft = queryLeft.Where(x => x.Status == status);
            }
            return queryLeft.OrderBy(o => o.YearMonth).ThenBy(o => o.SerialNumber);
        }
        /// <summary>
        /// 获取费用明细表信息（包含五险一金、其他费用、服务费等）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <param name="costId">费用表ID</param>
        /// <returns></returns>
        public List<Cost_CostTableDetails> GetCostFeeDetailList(SysEntities db, int costId)
        {

            //查询分值机构
            // var CompanyEmployeeRelationn = db.CompanyEmployeeRelation.Where(o => true);//公司关系表
            //  var CRM_CompanyToBranchn = db.CRM_CompanyToBranch.Where(o => true);//公司对应责任客服
            var ORG_Departmentn = db.ORG_Department.Where(o => o.DepartmentType == 1);//公司部门
            // var ORG_Usern = db.ORG_User.Where(o => true);//人员信息表

            #region 获取相关数据源
            // 费用表社保明细
            var costInsuranceQuery = from insurance in db.COST_CostTableInsurance
                                     where insurance.COST_CostTable_ID == costId
                                     //orderby insurance.Employee_ID ascending, insurance.PaymentStyle ascending
                                     join city in db.City on insurance.CityId equals city.Id
                                    into temp
                                     from tt in temp.DefaultIfEmpty()
                                     join cost in db.COST_CostTable on insurance.COST_CostTable_ID equals cost.ID
                                     into temp1
                                     from tt1 in temp1.DefaultIfEmpty()
                                     join supplier in db.Supplier on tt1.Suppler_ID equals supplier.Id
                                     into temp2
                                     from tt2 in temp2.DefaultIfEmpty()

                                     join branch in ORG_Departmentn on insurance.BranchID equals branch.ID into temp3
                                     from tt3 in temp3.DefaultIfEmpty()

                                     select new
                                     {

                                         ID = insurance.ID,
                                         CreateFrom = tt1.CreateFrom,
                                         SupplierName = tt2.Name,
                                         COST_CostTable_ID = insurance.COST_CostTable_ID,
                                         Operator_CompanyName = tt3.DepartmentName,
                                         PaymentStyle = insurance.PaymentStyle,
                                         PaymentInterval = insurance.PaymentInterval,
                                         PaymentSocialMonth = insurance.PaymentSocialMonth,
                                         PaymentMonth = insurance.PaymentMonth,
                                         CompanyRadix = insurance.CompanyRadix,
                                         CompanyCost = insurance.CompanyCost,
                                         CompanyRatio = insurance.CompanyRatio,
                                         PersonRadix = insurance.PersonRadix,
                                         PersonCost = insurance.PersonCost,
                                         PersonRatio = insurance.PersonRatio,
                                         CostType = insurance.CostType,
                                         Employee_ID = insurance.Employee_ID,
                                         EmployName = insurance.EmployName,
                                         CertificateType = insurance.CertificateType,
                                         CertificateNumber = insurance.CertificateNumber,
                                         Status = insurance.Status,
                                         CRM_Company_ID = insurance.CRM_Company_ID,
                                         BranchID = insurance.BranchID,
                                         CityId = insurance.CityId,
                                         CityName = tt == null ? "" : tt.Name //这里主要第二个集合有可能为空。需要判断
                                     };
            // 费用表其他费用
            var costOtherQuery = from other in db.COST_CostTableOther
                                 where other.COST_CostTable_ID == costId
                                 orderby other.Employee_ID
                                 select other;
            // 费用表服务费
            var costServiceQuery = from service in db.COST_CostTableService
                                   where service.COST_CostTable_ID == costId
                                   orderby service.Employee_ID
                                   select service;
            var costInsuranceList = costInsuranceQuery.ToList().OrderBy(x => x.Employee_ID).ThenBy(x => x.PaymentStyle).ThenBy(x => x.CityId);
            List<COST_CostTableOther> costOtherList = costOtherQuery.OrderBy(x => x.Employee_ID).ThenBy(x => x.PaymentStyle).ToList();
            List<COST_CostTableService> costServiceList = costServiceQuery.OrderBy(x => x.Employee_ID).ThenBy(x => x.PaymentStyle).ToList();
            #endregion

            List<Cost_CostTableDetails> costDetailList = new List<Cost_CostTableDetails>();
            int no = 0;
            int employeeNow = -2;  // 循环的当前员工
            int styleNow = -2;     // 循环的当前类型
            string cityNow = "";   // 循环的当前缴纳地
            foreach (var insurance in costInsuranceList)
            {
                int employeeId = insurance.Employee_ID == null ? -1 : (int)insurance.Employee_ID;
                int companyId = insurance.CRM_Company_ID;
                int style = insurance.PaymentStyle;
                string city = insurance.CityId;

                if (employeeId != employeeNow || style != styleNow || city != cityNow)
                {
                    #region 新增一行数据
                    // 若员工ID变化，或缴费类型变化，则重新添加一行数据
                    Cost_CostTableDetails detailModel = new Cost_CostTableDetails();
                    detailModel.No = employeeId != employeeNow ? ++no : no;  // 若循环到另一个员工，则序号加1
                    detailModel.COST_CostTable_ID = insurance.COST_CostTable_ID;
                    detailModel.PaymentStyle = insurance.PaymentStyle;
                    detailModel.CRM_Company_ID = insurance.CRM_Company_ID;
                    detailModel.CityName = insurance.CityName;  // 缴纳地
                    detailModel.CreateFrom = insurance.CreateFrom;//费用来源
                    detailModel.SupplierName = insurance.SupplierName;
                    // 个人信息
                    detailModel.Employee_ID = insurance.Employee_ID;
                    detailModel.EmployName = insurance.EmployName;
                    detailModel.CertificateType = insurance.CertificateType;
                    detailModel.CertificateNumber = insurance.CertificateNumber;
                    detailModel.SupplierName = insurance.SupplierName;
                    detailModel.Operator_CompanyName = insurance.Operator_CompanyName;
                    // 金额字段都赋值为0
                    detailModel.YanglaoCompanyCost = 0;
                    detailModel.YanglaoPersonCost = 0;
                    detailModel.YiliaoCompanyCost = 0;
                    detailModel.YiliaoPersonCost = 0;
                    detailModel.ShengyuCompanyCost = 0;
                    detailModel.ShengyuPersonCost = 0;
                    detailModel.GongshangCompanyCost = 0;
                    detailModel.GongshangPersonCost = 0;
                    detailModel.ShiyeCompanyCost = 0;
                    detailModel.ShiyePersonCost = 0;
                    detailModel.GongjijinCompanyCost = 0;
                    detailModel.GongjijinPersonCost = 0;
                    detailModel.DaeCompanyCost = 0;
                    detailModel.DaePersonCost = 0;
                    detailModel.GongjijinBCCompanyCost = 0;
                    detailModel.GongjijinBCPersonCost = 0;

                    costDetailList.Add(detailModel);
                    #endregion

                    // 当前员工、类型、缴纳地变量赋值
                    employeeNow = employeeId;
                    styleNow = style;
                    cityNow = city;
                }

                #region 社保相关内容
                int index = costDetailList.Count;
                switch (insurance.CostType)
                {
                    case (int)Common.CostType.养老: // 养老
                        costDetailList[index - 1].YanglaoPaymentInterval = insurance.PaymentInterval;  // 养老缴费区间
                        costDetailList[index - 1].YanglaoPaymentMonth = insurance.PaymentMonth;  // 养老缴纳月数
                        costDetailList[index - 1].YanglaoCompanyRadix = insurance.CompanyRadix;  // 养老企业缴费基数
                        costDetailList[index - 1].YanglaoCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 养老企业费用
                        costDetailList[index - 1].YanglaoPersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 养老个人费用
                        break;
                    case (int)Common.CostType.医疗: // 医疗
                        costDetailList[index - 1].YiliaoPaymentInterval = insurance.PaymentInterval;  // 医疗缴费区间
                        costDetailList[index - 1].YiliaoPaymentMonth = insurance.PaymentMonth;  // 医疗缴纳月数
                        costDetailList[index - 1].YiliaoCompanyRadix = insurance.CompanyRadix;  // 医疗企业缴费基数
                        costDetailList[index - 1].YiliaoCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 医疗企业费用
                        costDetailList[index - 1].YiliaoPersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 医疗个人费用
                        break;
                    case (int)Common.CostType.工伤: // 工伤
                        costDetailList[index - 1].GongshangPaymentInterval = insurance.PaymentInterval;  // 工伤缴费区间
                        costDetailList[index - 1].GongshangPaymentMonth = insurance.PaymentMonth;  // 工伤缴纳月数
                        costDetailList[index - 1].GongshangCompanyRadix = insurance.CompanyRadix;  // 工伤企业缴费基数
                        costDetailList[index - 1].GongshangCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 工伤企业费用
                        costDetailList[index - 1].GongshangPersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 工伤个人费用
                        break;
                    case (int)Common.CostType.失业: // 失业
                        costDetailList[index - 1].ShiyePaymentInterval = insurance.PaymentInterval;  // 失业缴费区间
                        costDetailList[index - 1].ShiyePaymentMonth = insurance.PaymentMonth;  // 失业缴纳月数
                        costDetailList[index - 1].ShiyeCompanyRadix = insurance.CompanyRadix;  // 失业企业缴费基数
                        costDetailList[index - 1].ShiyeCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 失业企业费用
                        costDetailList[index - 1].ShiyePersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 失业个人费用
                        break;
                    case (int)Common.CostType.公积金: // 公积金
                        costDetailList[index - 1].GongjijinPaymentInterval = insurance.PaymentInterval;  // 公积金缴费区间
                        costDetailList[index - 1].GongjijinPaymentMonth = insurance.PaymentMonth;  // 公积金缴纳月数
                        costDetailList[index - 1].GongjijinCompanyRadix = insurance.CompanyRadix;  // 公积金企业缴费基数
                        costDetailList[index - 1].GongjijinCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 公积金企业费用
                        costDetailList[index - 1].GongjijinPersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 公积金个人费用
                        break;
                    case (int)Common.CostType.生育: // 生育
                        costDetailList[index - 1].ShengyuPaymentInterval = insurance.PaymentInterval;  // 生育缴费区间
                        costDetailList[index - 1].ShengyuPaymentMonth = insurance.PaymentMonth;  // 生育缴纳月数
                        costDetailList[index - 1].ShengyuCompanyRadix = insurance.CompanyRadix;  // 生育企业缴费基数
                        costDetailList[index - 1].ShengyuCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 生育企业费用
                        costDetailList[index - 1].ShengyuPersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 生育个人费用
                        break;
                    case (int)Common.CostType.大病: // 大额
                        costDetailList[index - 1].DaePaymentInterval = insurance.PaymentInterval;  // 大额缴费区间
                        costDetailList[index - 1].DaePaymentMonth = insurance.PaymentMonth;  // 大额缴纳月数
                        costDetailList[index - 1].DaeCompanyRadix = insurance.CompanyRadix;  // 大额企业缴费基数
                        costDetailList[index - 1].DaeCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 大额企业费用
                        costDetailList[index - 1].DaePersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 大额个人费用
                        break;
                    case (int)Common.CostType.补充公积金: // 补充公积金
                        costDetailList[index - 1].DaePaymentInterval = insurance.PaymentInterval;  // 补充公积金缴费区间
                        costDetailList[index - 1].DaePaymentMonth = insurance.PaymentMonth;  // 补充公积金缴纳月数
                        costDetailList[index - 1].DaeCompanyRadix = insurance.CompanyRadix;  // 补充公积金企业缴费基数
                        costDetailList[index - 1].DaeCompanyCost = insurance.CompanyCost == null ? 0 : insurance.CompanyCost;  // 补充公积金企业费用
                        costDetailList[index - 1].DaePersonCost = insurance.PersonCost == null ? 0 : insurance.PersonCost;   // 补充公积金个人费用
                        break;
                }
                #endregion
            }




            #region 服务费、其他费用
            //int noNow = 0; // 当前循环的no
            employeeNow = -2;  // 循环的当前员工
            styleNow = -2;     // 循环的当前类型
            foreach (Cost_CostTableDetails detailModel in costDetailList)
            {
                int employeeId = detailModel.Employee_ID == null ? -1 : (int)detailModel.Employee_ID;
                int style = detailModel.PaymentStyle;

                if (employeeId != employeeNow || style != styleNow)
                {
                    // 服务费
                    var serviceList = costServiceList.Where(x => x.Employee_ID == detailModel.Employee_ID && x.CRM_Company_ID == detailModel.CRM_Company_ID && x.PaymentStyle == detailModel.PaymentStyle);
                    decimal serviceSum = 0;
                    foreach (var service in serviceList)
                    {
                        serviceSum += service.ChargeCost;
                    }
                    detailModel.ServiceCost = serviceSum;

                    // 其他费用
                    var otherList = costOtherList.Where(x => x.Employee_ID == detailModel.Employee_ID && x.CRM_Company_ID == detailModel.CRM_Company_ID && x.PaymentStyle == detailModel.PaymentStyle);
                    decimal otherSum = 0;
                    decimal otherInsuranceSum = 0;
                    decimal productionSum = 0;
                    foreach (var other in otherList)
                    {
                        switch (other.CostType)
                        {
                            case 1: // 1：其他费用（残保金、工会费） 2：其他社保费用（滞纳金） 3：工本费
                                otherSum += (decimal)other.ChargeCost;
                                break;
                            case 2:
                                otherInsuranceSum += (decimal)other.ChargeCost;
                                break;
                            case 3:
                                productionSum += (decimal)other.ChargeCost;
                                break;
                        }
                    }
                    detailModel.OtherCost = otherSum;
                    detailModel.OtherInsuranceCost = otherInsuranceSum;
                    detailModel.ProductionCost = productionSum;

                    // 修改当前循环的员工、类型
                    employeeNow = employeeId;
                    styleNow = style;
                    //noNow = detailModel.No;
                }
                else
                {
                    detailModel.OtherCost = 0;
                    detailModel.OtherInsuranceCost = 0;
                    detailModel.ProductionCost = 0;
                    detailModel.ServiceCost = 0;
                }
            }
            #endregion

            #region 将服务费中存在，而明细列表中不存在的数据加到明细列表中
            foreach (var li in costServiceList)
            {
                int detailCount = costDetailList.Where(x => x.Employee_ID == li.Employee_ID && x.PaymentStyle == li.PaymentStyle).Count();

                if (detailCount == 0)
                {
                    Cost_CostTableDetails detailModel = new Cost_CostTableDetails();
                    Cost_CostTableDetails lastModel = costDetailList.LastOrDefault();
                    detailModel.No = lastModel == null ? 1 : lastModel.No + 1;
                    detailModel.COST_CostTable_ID = li.COST_CostTable_ID;
                    detailModel.PaymentStyle = li.PaymentStyle;
                    detailModel.CRM_Company_ID = li.CRM_Company_ID;
                    detailModel.CityName = "";  // 缴纳地
                    //detailModel.Operator_CompanyName = lastModel.Operator_CompanyName;//分支机构id

                    // 个人信息
                    detailModel.Employee_ID = li.Employee_ID;
                    detailModel.EmployName = li.EmployName;
                    detailModel.CertificateType = li.CertificateType;
                    detailModel.CertificateNumber = li.CertificateNumber;

                    // 金额字段都赋值为0
                    detailModel.YanglaoCompanyCost = 0;
                    detailModel.YanglaoPersonCost = 0;
                    detailModel.YiliaoCompanyCost = 0;
                    detailModel.YiliaoPersonCost = 0;
                    detailModel.ShengyuCompanyCost = 0;
                    detailModel.ShengyuPersonCost = 0;
                    detailModel.GongshangCompanyCost = 0;
                    detailModel.GongshangPersonCost = 0;
                    detailModel.ShiyeCompanyCost = 0;
                    detailModel.ShiyePersonCost = 0;
                    detailModel.GongjijinCompanyCost = 0;
                    detailModel.GongjijinPersonCost = 0;
                    detailModel.DaeCompanyCost = 0;
                    detailModel.DaePersonCost = 0;
                    detailModel.GongjijinBCCompanyCost = 0;
                    detailModel.GongjijinBCPersonCost = 0;

                    // 服务费
                    var serviceList = costServiceList.Where(x => x.Employee_ID == li.Employee_ID && x.PaymentStyle == li.PaymentStyle);
                    decimal serviceSum = 0;
                    foreach (var service in serviceList)
                    {
                        serviceSum += service.ChargeCost;
                    }
                    detailModel.ServiceCost = serviceSum;

                    // 其他费用
                    var otherList = costOtherList.Where(x => x.Employee_ID == li.Employee_ID && x.PaymentStyle == li.PaymentStyle);
                    decimal otherSum = 0;
                    decimal otherInsuranceSum = 0;
                    decimal productionSum = 0;
                    foreach (var other in otherList)
                    {
                        switch (other.CostType)
                        {
                            case 1: // 1：其他费用（残保金、工会费） 2：其他社保费用（滞纳金） 3：工本费
                                otherSum += (decimal)other.ChargeCost;
                                break;
                            case 2:
                                otherInsuranceSum += (decimal)other.ChargeCost;
                                break;
                            case 3:
                                productionSum += (decimal)other.ChargeCost;
                                break;
                        }
                    }
                    detailModel.OtherCost = otherSum;
                    detailModel.OtherInsuranceCost = otherInsuranceSum;
                    detailModel.ProductionCost = productionSum;

                    costDetailList.Add(detailModel);
                }
            }

            #endregion

            #region 将其他费用中存在，而明细列表中不存在的数据加到明细列表中
            foreach (var li in costOtherList)
            {
                int detailCount = costDetailList.Where(x => x.Employee_ID == li.Employee_ID && x.PaymentStyle == li.PaymentStyle).Count();

                if (detailCount == 0)
                {
                    Cost_CostTableDetails detailModel = new Cost_CostTableDetails();
                    Cost_CostTableDetails lastModel = costDetailList.LastOrDefault();
                    detailModel.No = lastModel == null ? 1 : lastModel.No + 1;
                    detailModel.COST_CostTable_ID = li.COST_CostTable_ID;
                    detailModel.PaymentStyle = li.PaymentStyle;
                    detailModel.CRM_Company_ID = li.CRM_Company_ID;
                    detailModel.CityName = "";  // 缴纳地
                    //detailModel.Operator_CompanyName = lastModel.Operator_CompanyName;//分支机构id

                    // 个人信息
                    detailModel.Employee_ID = li.Employee_ID;
                    detailModel.EmployName = li.EmployName;
                    detailModel.CertificateType = li.CertificateType;
                    detailModel.CertificateNumber = li.CertificateNumber;

                    // 金额字段都赋值为0
                    detailModel.YanglaoCompanyCost = 0;
                    detailModel.YanglaoPersonCost = 0;
                    detailModel.YiliaoCompanyCost = 0;
                    detailModel.YiliaoPersonCost = 0;
                    detailModel.ShengyuCompanyCost = 0;
                    detailModel.ShengyuPersonCost = 0;
                    detailModel.GongshangCompanyCost = 0;
                    detailModel.GongshangPersonCost = 0;
                    detailModel.ShiyeCompanyCost = 0;
                    detailModel.ShiyePersonCost = 0;
                    detailModel.GongjijinCompanyCost = 0;
                    detailModel.GongjijinPersonCost = 0;
                    detailModel.DaeCompanyCost = 0;
                    detailModel.DaePersonCost = 0;
                    detailModel.GongjijinBCCompanyCost = 0;
                    detailModel.GongjijinBCPersonCost = 0;

                    // 服务费
                    detailModel.ServiceCost = 0;

                    // 其他费用
                    var otherList = costOtherList.Where(x => x.Employee_ID == li.Employee_ID && x.PaymentStyle == li.PaymentStyle);
                    decimal otherSum = 0;
                    decimal otherInsuranceSum = 0;
                    decimal productionSum = 0;
                    foreach (var other in otherList)
                    {
                        switch (other.CostType)
                        {
                            case 1: // 1：其他费用（残保金、工会费） 2：其他社保费用（滞纳金） 3：工本费
                                otherSum += (decimal)other.ChargeCost;
                                break;
                            case 2:
                                otherInsuranceSum += (decimal)other.ChargeCost;
                                break;
                            case 3:
                                productionSum += (decimal)other.ChargeCost;
                                break;
                        }
                    }
                    detailModel.OtherCost = otherSum;
                    detailModel.OtherInsuranceCost = otherInsuranceSum;
                    detailModel.ProductionCost = productionSum;

                    costDetailList.Add(detailModel);
                }
            }

            #endregion

            return costDetailList;
        }


        //#region 获取补收的费用
        //public List<Cost_CostTableDetails> GetBuShou(SysEntities db, List<Cost_CostTableDetails> list, List<COST_CostTableService> costServiceList)
        //{



        //    foreach (var li in costServiceList.Where(x => x.PaymentStyle == 4))
        //    {
        //        int a = list.Where(x => x.Employee_ID == li.Employee_ID && x.PaymentStyle == 4).Count();




        //        if (a == 0)
        //        {
        //            Cost_CostTableDetails detailModel = new Cost_CostTableDetails();
        //            Cost_CostTableDetails lastModel = list.LastOrDefault();
        //            detailModel.No = lastModel.No + 1;
        //            detailModel.COST_CostTable_ID = lastModel.COST_CostTable_ID;
        //            detailModel.PaymentStyle = 4;
        //            detailModel.CRM_Company_ID = lastModel.CRM_Company_ID;
        //            detailModel.CityName = "";  // 缴纳地
        //            //detailModel.Operator_CompanyName = lastModel.Operator_CompanyName;//分支机构id

        //            // 个人信息
        //            detailModel.Employee_ID = li.Employee_ID;
        //            detailModel.EmployName = li.EmployName;
        //            detailModel.CertificateType = li.CertificateType;
        //            detailModel.CertificateNumber = li.CertificateNumber;

        //            // 金额字段都赋值为0
        //            detailModel.YanglaoCompanyCost = 0;
        //            detailModel.YanglaoPersonCost = 0;
        //            detailModel.YiliaoCompanyCost = 0;
        //            detailModel.YiliaoPersonCost = 0;
        //            detailModel.ShengyuCompanyCost = 0;
        //            detailModel.ShengyuPersonCost = 0;
        //            detailModel.GongshangCompanyCost = 0;
        //            detailModel.GongshangPersonCost = 0;
        //            detailModel.ShiyeCompanyCost = 0;
        //            detailModel.ShiyePersonCost = 0;
        //            detailModel.GongjijinCompanyCost = 0;
        //            detailModel.GongjijinPersonCost = 0;
        //            detailModel.DaeCompanyCost = 0;
        //            detailModel.DaePersonCost = 0;
        //            detailModel.GongjijinBCCompanyCost = 0;
        //            detailModel.GongjijinBCPersonCost = 0;

        //            // 服务费
        //            detailModel.OtherCost = 0;
        //            detailModel.OtherInsuranceCost = 0;
        //            detailModel.ProductionCost = 0;
        //            detailModel.ServiceCost = li.ChargeCost;

        //            list.Add(detailModel);
        //        }
        //    }
        //    return list;
        //}
        //#endregion




        /// <summary>
        /// 查询的数据（财务确认费用表列表）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <returns></returns>
        public IOrderedQueryable<CostFeeModel> GetCostFeeFinanceAduitList(SysEntities db, string dateTime, int status)
        {
            var queryLeft = from cost in db.COST_CostTable
                            where cost.CreateFrom == (int)Common.CostTable_CreateFrom.本地费用
                            join companyToBranch in db.CRM_CompanyToBranch on cost.CRM_Company_ID equals companyToBranch.CRM_Company_ID
                            join user in db.ORG_User on companyToBranch.UserID_ZR equals user.ID
                            into costuser
                            from costZR in costuser.DefaultIfEmpty()
                            join company in db.CRM_Company on cost.CRM_Company_ID equals company.ID
                                into fee
                            from l in fee.DefaultIfEmpty()
                            select new CostFeeModel()
                            {
                                ID = cost.ID,
                                CostTableType = cost.CostTableType,
                                YearMonth = cost.YearMonth,
                                SerialNumber = cost.SerialNumber,
                                ChargeCost = cost.ChargeCost,
                                ServiceCost = cost.ServiceCost,
                                Remark = cost.Remark,
                                Status = cost.Status,
                                CRM_Company_ID = cost.CRM_Company_ID,
                                CompanyName = l.CompanyName,  // 公司名称
                                UserID_ZR = companyToBranch.UserID_ZR,  // 责任客服
                                UserName_ZR = costZR.RName,  // 责任客服名称
                                CreateTime = cost.CreateTime,
                                CreateUserID = cost.CreateUserID,
                                CreateUserName = cost.CreateUserName,
                                BranchID = cost.BranchID
                            };
            if (!string.IsNullOrEmpty(dateTime))
            {
                var datetime_Int = Convert.ToInt32(dateTime);
                queryLeft = queryLeft.Where(x => x.YearMonth == datetime_Int);
            }
            if (status != 0)
            {
                queryLeft = queryLeft.Where(x => x.Status == status);
            }
            return queryLeft.OrderBy(o => o.YearMonth).ThenBy(o => o.SerialNumber);
        }
        /// <summary>
        /// 作废费用表
        /// </summary>
        /// <param name="db">实体数据</param>
        /// <param name="id">费用表主键</param>
        /// <param name="status">作废后的状态</param>
        /// <returns></returns>
        public void CancelCostTable(SysEntities db, int id, int status)
        {
            COST_CostTable costItem = GetById(db, id);
            if (costItem != null)
            {
                costItem.Status = status;
            }

            int yearMonth = costItem.YearMonth - 1;
            string billId = id.ToString();
            IQueryable<EmployeeMiddle> collection = from f in db.EmployeeMiddle
                                                    where f.BillId == billId
                                                    select f;
            foreach (var item in collection)
            {
                //item.UseBetween = yearMonth;
                item.UseBetween = 0;
                item.BillId = "";
            }
        }
        /// <summary>
        /// 更新费用表状态
        /// </summary>
        /// <param name="id">费用表主键</param>
        /// <param name="status">要更新成的状态值</param>
        /// <returns></returns>
        public int UpdateCostTableStatus(int id, int status)
        {
            using (SysEntities db = new SysEntities())
            {
                COST_CostTable updateItem = GetById(db, id);
                if (updateItem != null)
                {
                    updateItem.Status = status;
                }
                return Save(db);
            }
        }
        /// <summary>
        /// 更新费用表状态
        /// </summary>
        /// <param name="db">实体数据</param>
        /// <param name="ids">费用表主键</param>
        /// <param name="status">要更新成的状态值</param>
        public void UpdateCostTableStatus(SysEntities db, int[] ids, int status)
        {
            IQueryable<COST_CostTable> collection = from f in db.COST_CostTable
                                                    where ids.Contains(f.ID)
                                                    select f;
            foreach (var item in collection)
            {
                item.Status = status;
            }
        }

        #region 获取生成费用表需要的数据

        #region 获取中间表里的社保费用（含正常和对比）
        /// <summary>
        /// 获取中间表里的社保费用（含正常和对比）
        /// </summary>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        public List<COST_CostTableInsurance> Get_List_COST_CostTableInsurance(SysEntities db, int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID, out IQueryable<EmployeeMiddle> data)
        {
            int[] costType = new int[]{(int)Common.CostType.养老, (int)Common.CostType.医疗, (int)Common.CostType.工伤,(int)Common.CostType.失业,(int)Common.CostType.公积金
                ,(int)Common.CostType.生育,(int)Common.CostType.补充公积金,(int)Common.CostType.大病};
            string state = Common.Status.启用.ToString();
            //首先取出中间表中可以提取的数据
            var trueData = from a in db.EmployeeMiddle.Where(em => em.StartDate <= yearMonth && em.EndedDate >= yearMonth && (costType.Contains(em.InsuranceKindId.Value)) &&
                em.State == state && em.UseBetween < yearMonth)
                           join b in db.CompanyEmployeeRelation.Where(x => x.CompanyId == CRM_Company_ID) on a.CompanyEmployeeRelationId equals b.Id
                           select a;
            //把取出来的数据作标记
            data = trueData;

            //正常和对比的社保费用
            var query = (from em in trueData
                         join c in db.CompanyEmployeeRelation.Where(c => c.CompanyId == CRM_Company_ID && c.EmployeeAdd.Any(x => x.SuppliersId == null)) on em.CompanyEmployeeRelationId equals c.Id
                         join e in db.Employee on c.EmployeeId equals e.Id
                         where c.CompanyId == CRM_Company_ID
                         orderby e.Id descending
                         select new
                         {
                             COST_CostTable_ID = COST_CostTable_ID,
                             PaymentStyle = (int)em.PaymentStyle,
                             PaymentInterval = em.PaymentBetween,
                             PaymentSocialMonth = "",
                             PaymentMonth = em.PaymentMonth,
                             CompanyRadix = em.CompanyBasePayment,
                             CompanyCost = em.CompanyPayment,
                             CompanyRatio = 0,
                             PersonRadix = em.EmployeeBasePayment,
                             PersonCost = em.EmployeePayment,
                             PersonRatio = 0,
                             CostType = (int)em.InsuranceKindId,
                             Employee_ID = e.Id,
                             EmployName = e.Name,
                             CertificateType = e.CertificateType,
                             CertificateNumber = e.CertificateNumber,
                             Remark = em.Remark,
                             CRM_Company_ID = (int)c.CompanyId,
                             CreateTime = DateTime.Now,
                             CreateUserID = CreateUserID,
                             CreateUserName = CreateUserName,
                             BranchID = BranchID,
                             CityId = em.CityId

                         }).ToList().Select(o => new COST_CostTableInsurance
                         {
                             COST_CostTable_ID = o.COST_CostTable_ID,
                             PaymentStyle = o.PaymentStyle,
                             PaymentInterval = o.PaymentInterval,
                             PaymentSocialMonth = o.PaymentSocialMonth,
                             PaymentMonth = o.PaymentMonth,
                             CompanyRadix = o.CompanyRadix,
                             CompanyCost = o.CompanyCost,
                             CompanyRatio = o.CompanyRatio,
                             PersonRadix = o.PersonRadix,
                             PersonCost = o.PersonCost,
                             PersonRatio = o.PersonRatio,
                             CostType = o.CostType,
                             Employee_ID = o.Employee_ID,
                             EmployName = o.EmployName,
                             CertificateType = o.CertificateType,
                             CertificateNumber = o.CertificateNumber,
                             Remark = o.Remark,
                             CRM_Company_ID = (int)o.CRM_Company_ID,
                             CreateTime = o.CreateTime,
                             CreateUserID = o.CreateUserID,
                             CreateUserName = o.CreateUserName,
                             BranchID = o.BranchID,
                             CityId = o.CityId
                         });


         
            var listm = (from em in trueData
                         join c in db.CompanyEmployeeRelation.Where(c => c.CompanyId == CRM_Company_ID && c.EmployeeAdd.Any(x => x.SuppliersId == null)) on em.CompanyEmployeeRelationId equals c.Id
                         join e in db.Employee on c.EmployeeId equals e.Id
                         select new
                         {
                             e.Id
                         }).Distinct();
            var listSuppler = from a in db.COST_CostTableInsurance.Where(x => x.CRM_Company_ID == CRM_Company_ID)
                              join b in db.COST_CostTable.Where(x => x.YearMonth == yearMonth && x.CreateFrom == (int)Common.CostTable_CreateFrom.供应商费用 && x.Status == (int)Common.COST_Table_Status.待核销) on a.COST_CostTable_ID equals b.ID
                              join c in listm on a.Employee_ID equals c.Id
                              select a;


            return query.Union(listSuppler).ToList();
        }
        #endregion

        #region 获取服务费用
        /// <summary>
        /// 获取服务费用
        /// </summary>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        public List<COST_CostTableService> Get_List_COST_CostTableService(SysEntities db, int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID)
        {
            decimal payService_One = 0;
            decimal AddPrice = 0;
            CRM_CompanyRepository c = new CRM_CompanyRepository();
            int count = c.getEmployee(db, CRM_Company_ID, yearMonth);
            int[] emp = c.getEmployeeIDs(db, CRM_Company_ID, yearMonth);
            ////先找出企业交社保的人数
            //var companyEmployeeRelation = db.CompanyEmployeeRelation.Where(ce => ce.State == "在职" && ce.CompanyId == CRM_Company_ID).GroupBy(x => x.EmployeeId);
            //if (companyEmployeeRelation.Count() > 0)
            //{
            //    count = companyEmployeeRelation.Count();
            //}
            //找出单人服务费
            var cRM_CompanyLadderPrice = db.CRM_CompanyLadderPrice.Where(cclp => cclp.CRM_Company_ID == CRM_Company_ID && (new[] { (int)Common.Status.启用, (int)Common.Status.修改中 }).Contains(cclp.Status) && cclp.BeginLadder <= count && cclp.EndLadder >= count);
            if (cRM_CompanyLadderPrice.Count() > 0)
            {
                payService_One = cRM_CompanyLadderPrice.OrderByDescending(o => o.EndLadder).FirstOrDefault().SinglePrice;
            }
            //补缴服务费
            var cRM_CompanyPrice = db.CRM_CompanyPrice.Where(ccp => (new[] { (int)Common.Status.启用, (int)Common.Status.修改中 }).Contains(ccp.Status) && ccp.CRM_Company_ID == CRM_Company_ID);
            if (cRM_CompanyPrice.Count() > 0)
            {
                AddPrice = cRM_CompanyPrice.FirstOrDefault().AddPrice.Value;
            }

            var query =
                //政策服务费
                Get_List_COST_CostTableService_Z(db, count, payService_One, COST_CostTable_ID, CRM_Company_ID, CreateUserID, CreateUserName, BranchID, emp)
                //补缴服务费
                .Union(Get_List_COST_CostTableService_B(db, AddPrice, COST_CostTable_ID, CRM_Company_ID, CreateUserID, CreateUserName, BranchID))
                //历史服务费对比差
                .Union(Get_List_COST_CostTableService_C(db, COST_CostTable_ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID));
            return query.ToList();
        }

        /// <summary>
        /// 获取正常服务费用
        /// </summary>
        /// <param name="costTable">人数</param>
        /// <param name="costTable">单人服务费</param>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        public List<COST_CostTableService> Get_List_COST_CostTableService_Z(SysEntities db, int count, decimal payService_One, int COST_CostTable_ID, int CRM_Company_ID, int CreateUserID, string CreateUserName, int BranchID, int[] employeeIDs)
        {
            decimal payService_All = 0;
            bool zhengHu = false;

            //找出整户服务费
            var cRM_CompanyPrice = db.CRM_CompanyPrice.Where(ccp => (new[] { (int)Common.Status.启用, (int)Common.Status.修改中 }).Contains(ccp.Status) && ccp.CRM_Company_ID == CRM_Company_ID);
            if (cRM_CompanyPrice.Count() > 0)
            {
                payService_All = cRM_CompanyPrice.FirstOrDefault().LowestPrice.Value;
            }
            //判断取单人服务费还是整户服务费
            if (payService_One * count > payService_All)
            {
                zhengHu = false;
                payService_All = 0;
            }
            else
            {
                zhengHu = true;
                payService_One = 0;
            }
            CRM_CompanyRepository c = new CRM_CompanyRepository();


            var query = (from ce in db.CompanyEmployeeRelation.Where(ce => ce.State == "在职")
                         join e in db.Employee on ce.EmployeeId equals e.Id
                         where ce.CompanyId == CRM_Company_ID && employeeIDs.Contains(ce.EmployeeId ?? 0)
                         select new
                         {
                             COST_CostTable_ID = COST_CostTable_ID,
                             PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常,
                             ChargeCost = payService_One,
                             Employee_ID = e.Id,
                             EmployName = e.Name,
                             CertificateType = e.CertificateType,
                             CertificateNumber = e.CertificateNumber,
                             CRM_Company_ID = CRM_Company_ID,
                             CreateTime = DateTime.Now,
                             CreateUserID = CreateUserID,

                             CreateUserName = CreateUserName,
                             BranchID = BranchID
                         }).OrderBy(o => o.Employee_ID).ToList().Select(o => new COST_CostTableService
                         {
                             COST_CostTable_ID = o.COST_CostTable_ID,
                             PaymentStyle = o.PaymentStyle,
                             ChargeCost = o.ChargeCost,
                             Employee_ID = o.Employee_ID,
                             EmployName = o.EmployName,
                             CertificateType = o.CertificateType,
                             CertificateNumber = o.CertificateNumber,
                             CRM_Company_ID = o.CRM_Company_ID,
                             CreateTime = o.CreateTime,
                             CreateUserID = o.CreateUserID,
                             CreateUserName = o.CreateUserName,
                             BranchID = o.BranchID,
                             ServiceCoset = (decimal)0
                         }).ToList();
            if (zhengHu == true)
            {
                query.FirstOrDefault().ChargeCost = payService_All;
            }
            return query.ToList();

        }
        /// <summary>
        /// 获取补缴服务费用
        /// </summary>
        ///  /// <param name="costTable">单人服务费</param>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        public List<COST_CostTableService> Get_List_COST_CostTableService_B(SysEntities db, decimal payService_One, int COST_CostTable_ID, int CRM_Company_ID, int CreateUserID, string CreateUserName, int BranchID)
        {
            string state = Common.Status.启用.ToString();
            //取中间表的补缴数据
            var queryData =
                (from em in db.EmployeeMiddle
                 join cer in db.CompanyEmployeeRelation.Where(cer => cer.CompanyId == CRM_Company_ID) on em.CompanyEmployeeRelationId equals cer.Id
                 join e in db.Employee on cer.EmployeeId equals e.Id
                 where cer.CompanyId == CRM_Company_ID && em.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴 && em.State == state
                 select new
                 {
                     Employee_ID = e.Id,
                     EmployName = e.Name,
                     CertificateType = e.CertificateType,
                     CertificateNumber = e.CertificateNumber,
                     PaymentMonth = em.PaymentMonth
                 }
            ).GroupBy(o => new { o.Employee_ID, o.EmployName, o.CertificateType, o.CertificateNumber })
            .Select(o => new
            {
                COST_CostTable_ID = COST_CostTable_ID,
                PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴,
                ChargeCost = (decimal)o.Max(m => m.PaymentMonth) * payService_One,
                Employee_ID = o.Key.Employee_ID,
                EmployName = o.Key.EmployName,
                CertificateType = o.Key.CertificateType,
                CertificateNumber = o.Key.CertificateNumber,
                CRM_Company_ID = CRM_Company_ID,
                CreateTime = DateTime.Now,
                CreateUserID = CreateUserID,
                CreateUserName = CreateUserName,
                BranchID = BranchID
            }).ToList().Select(o => new COST_CostTableService
            {
                COST_CostTable_ID = o.COST_CostTable_ID,
                PaymentStyle = o.PaymentStyle,
                ChargeCost = o.ChargeCost,
                ServiceCoset = (decimal)0,
                Employee_ID = o.Employee_ID,
                EmployName = o.EmployName,
                CertificateType = o.CertificateType,
                CertificateNumber = o.CertificateNumber,
                CRM_Company_ID = o.CRM_Company_ID,
                CreateTime = o.CreateTime,
                CreateUserID = o.CreateUserID,
                CreateUserName = o.CreateUserName,
                BranchID = o.BranchID
            });

            return queryData.ToList();
        }
        /// <summary>
        /// 获取补收、退费服务费用
        /// </summary>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        public List<COST_CostTableService> Get_List_COST_CostTableService_C(SysEntities db, int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID)
        {

            int[] zuofeiStatus = { (int)Common.COST_Table_Status.财务作废, (int)Common.COST_Table_Status.供应商客服作废, (int)Common.COST_Table_Status.责任客服作废 };

            var queryData = (from ccts in db.COST_CostTableService.Where(ccts => ccts.CRM_Company_ID == CRM_Company_ID)
                             join cct in db.COST_CostTable.Where(cct => cct.YearMonth < yearMonth && !zuofeiStatus.Contains(cct.Status)) on ccts.COST_CostTable_ID equals cct.ID
                             join e in db.Employee on ccts.Employee_ID equals e.Id
                             where ccts.CRM_Company_ID == CRM_Company_ID
                             select new
                             {
                                 Employee_ID = e.Id,
                                 EmployName = e.Name,
                                 CertificateType = e.CertificateType,
                                 CertificateNumber = e.CertificateNumber,
                                 serviceMoney = ccts.ChargeCost - ccts.ServiceCoset
                             }).GroupBy(o => new { o.Employee_ID, o.EmployName, o.CertificateType, o.CertificateNumber })
                             .Select(o => new
                             {
                                 COST_CostTable_ID = COST_CostTable_ID,
                                 PaymentStyle = o.Sum(m => m.serviceMoney??0) > (decimal)0 ? (int)Common.EmployeeMiddle_PaymentStyle.退费 : (int)Common.EmployeeMiddle_PaymentStyle.补收,
                                 ChargeCost = -(decimal)o.Max(m => m.serviceMoney??0),
                                 Employee_ID = o.Key.Employee_ID,
                                 EmployName = o.Key.EmployName,
                                 CertificateType = o.Key.CertificateType,
                                 CertificateNumber = o.Key.CertificateNumber,
                                 CRM_Company_ID = CRM_Company_ID,
                                 CreateTime = DateTime.Now,
                                 CreateUserID = CreateUserID,
                                 CreateUserName = CreateUserName,
                                 BranchID = BranchID
                             }).Where(o => o.ChargeCost != 0).ToList().Select(o => new COST_CostTableService
                             {
                                 COST_CostTable_ID = o.COST_CostTable_ID,
                                 PaymentStyle = o.PaymentStyle,
                                 ChargeCost = o.ChargeCost,
                                 Employee_ID = o.Employee_ID,
                                 EmployName = o.EmployName,
                                 CertificateType = o.CertificateType,
                                 CertificateNumber = o.CertificateNumber,
                                 CRM_Company_ID = o.CRM_Company_ID,
                                 CreateTime = o.CreateTime,
                                 CreateUserID = o.CreateUserID,
                                 CreateUserName = o.CreateUserName,
                                 ServiceCoset = (decimal)0,
                                 BranchID = o.BranchID
                             });

            return queryData.ToList();
        }

        #endregion

        #region 获取其他费用和其他社保费用

        /// <summary>
        /// 获取中间表里的其他费用和其他社保费用
        /// </summary>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        public List<COST_CostTableOther> Get_List_COST_CostTableOther(SysEntities db, int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID, out IQueryable<EmployeeMiddle> data)
        {
            int[] costType = new int[] { (int)Common.CostType.其他费用, (int)Common.CostType.其他社保费用 };
            string state = Common.Status.启用.ToString();
            //首先取出中间表中可以提取的数据
            var trueData = from a in db.EmployeeMiddle.Where(em => em.StartDate <= yearMonth && em.EndedDate >= yearMonth && (costType.Contains(em.InsuranceKindId.Value)) &&
                 em.State == state && em.UseBetween < yearMonth)
                           join b in db.CompanyEmployeeRelation.Where(x => x.CompanyId == CRM_Company_ID) on a.CompanyEmployeeRelationId equals b.Id
                           select a;
            //把取出来的数据作标记
            data = trueData;

            //正常和对比的社保费用
            var query = (from em in trueData
                         join c in db.CompanyEmployeeRelation.Where(c => c.CompanyId == CRM_Company_ID&&c.EmployeeAdd.Any(x=>x.SuppliersId==null)) on em.CompanyEmployeeRelationId equals c.Id
                         join e in db.Employee on c.EmployeeId equals e.Id
                         where c.CompanyId == CRM_Company_ID
                         orderby e.Id descending
                         select new
                         {
                             COST_CostTable_ID = COST_CostTable_ID,
                             PaymentStyle = (int)em.PaymentStyle,
                             ChargeCost = em.CompanyPayment,
                             CostType = em.InsuranceKindId.Value,
                             Employee_ID = e.Id,
                             EmployName = e.Name,
                             CertificateType = e.CertificateType,
                             CertificateNumber = e.CertificateNumber,
                             Remark = em.Remark,
                             CRM_Company_ID = (int)c.CompanyId,
                             CreateTime = DateTime.Now,
                             CreateUserID = CreateUserID,
                             CreateUserName = CreateUserName,
                             BranchID = BranchID
                         }).ToList().Select(o => new COST_CostTableOther
                         {
                             COST_CostTable_ID = o.COST_CostTable_ID,
                             PaymentStyle = o.PaymentStyle,
                             ChargeCost = o.ChargeCost,
                             CostType = o.CostType,
                             Employee_ID = o.Employee_ID,
                             EmployName = o.EmployName,
                             CertificateType = o.CertificateType,
                             CertificateNumber = o.CertificateNumber,
                             Remark = o.Remark,
                             CRM_Company_ID = o.CRM_Company_ID,
                             CreateTime = o.CreateTime,
                             CreateUserID = o.CreateUserID,
                             CreateUserName = o.CreateUserName,
                             BranchID = o.BranchID
                         });
            var listm = (from em in trueData
                         join c in db.CompanyEmployeeRelation.Where(c => c.CompanyId == CRM_Company_ID && c.EmployeeAdd.Any(x => x.SuppliersId == null)) on em.CompanyEmployeeRelationId equals c.Id
                         join e in db.Employee on c.EmployeeId equals e.Id
                         select new
                         {
                             e.Id
                         }).Distinct();
            var listSuppler = from a in db.COST_CostTableOther.Where(x => x.CRM_Company_ID == CRM_Company_ID)
                              join b in db.COST_CostTable.Where(x => x.YearMonth == yearMonth && x.CreateFrom == (int)Common.CostTable_CreateFrom.供应商费用 && x.Status == (int)Common.COST_Table_Status.待核销) on a.COST_CostTable_ID equals b.ID
                              join c in listm on a.Employee_ID equals c.Id
                              select a;
            return query.Union(listSuppler).ToList();
        }

        #endregion

        #endregion


        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断,责任客服权限）
        /// </summary>
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        public List<CRM_Company> GetCompanyList(int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                var query = db.CRM_Company.Where(o => true);

                #region 权限获取
                var people = db.ORG_User.Where(o => true);  // 所有员工
                switch (departmentScope)
                {
                    case (int)Common.DepartmentScopeAuthority.无限制: // 无限制
                        //不做任何逻辑判断，查询所有部门数据
                        if (!string.IsNullOrEmpty(departments))
                        {
                            // 获取特定用户组所有人员
                            if (departments != "")
                            {
                                int[] departmentList = Array.ConvertAll<string, int>(departments.Split(','), delegate(string s) { return int.Parse(s); });
                                people = from a in db.ORG_User
                                         join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                         where a.XYBZ == "Y" && b.XYBZ == "Y" && departmentList.Contains(b.ID)
                                         select a;
                            }
                        }
                        break;
                    case (int)Common.DepartmentScopeAuthority.本机构及下属机构:
                        //当前用户直属机构
                        //查询本机构及下属机构所有部门数据
                        var branch = db.ORG_Department.FirstOrDefault(o => o.ID == userID);

                        people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                 select a;
                        break;
                    case (int)Common.DepartmentScopeAuthority.本机构:
                        //查询本机构所有部门数据
                        people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                                 select a;
                        break;
                    case (int)Common.DepartmentScopeAuthority.本部门及其下属部门:
                        //当前用户所属部门
                        ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);
                        //查询本部门及下属部门所有部门数据
                        people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                 select a;
                        break;
                    case (int)Common.DepartmentScopeAuthority.本部门:
                        //查询本部门所有用户数据
                        people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                 join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == departmentID) on a.ORG_Department_ID equals b.ID
                                 select a;

                        break;
                    case (int)Common.DepartmentScopeAuthority.本人:
                        people = from a in db.ORG_User.Where(o => o.ID == userID) select a;
                        break;
                }
                #endregion

                // 获取责任客服负责的企业
                query = (from cc in query
                         join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                         join f in people on cctb.UserID_ZR equals f.ID
                         select cc);

                return query.ToList();
            }
        }


        #region 根据费用表编号，获取费用表信息
        public List<Cost_Cost_Company> GetCost_Cost_Company(int Cost_TableID)
        {
            using (SysEntities db = new SysEntities())
            {
                var list = (from a in db.COST_CostTable.Where(x => x.ID == Cost_TableID)
                            join c in db.CRM_CompanyFinance_Bill on a.CRM_Company_ID equals c.CRM_Company_ID
                             into Joine1dEmpDept
                            from dept1 in Joine1dEmpDept.DefaultIfEmpty()

                            join b in db.CRM_Company on a.CRM_Company_ID equals b.ID

                            into JoinedEmpDept
                            from dept in JoinedEmpDept.DefaultIfEmpty()
                            select new Cost_Cost_Company
                            {
                                CompanyShuiHao = dept1 == null ? "" : dept1.TaxRegistryNumber,
                                CompanyID = a.CRM_Company_ID,
                                CompanyName = dept.CompanyName,

                                SerialNumber = a.SerialNumber,
                                YearMouth = a.YearMonth

                            }).ToList();
                return list;

            }

        }
        #endregion


        #region 获取费用表列表
        /// <summary>
        /// 获取费用表列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="search">查询条件</param>
        /// <param name="UserType">1 责任客服 2 供应商客服</param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IOrderedQueryable<CostFeeModel> GetAllCostFeeList(SysEntities db, string search, int UserType, List<int> companyId)
        {
            List<int> CreateFrom = new List<int>();
            if (UserType == 1)
            {
                CreateFrom.Add((int)Common.CostTable_CreateFrom.本地费用);
            }
            else
            {
                CreateFrom.Add((int)Common.CostTable_CreateFrom.供应商费用);
                CreateFrom.Add((int)Common.CostTable_CreateFrom.供应商预算费用);
            }

            var queryLeft = from cost in db.COST_CostTable.Where(x => CreateFrom.Contains(x.CreateFrom ?? 0))
                            join s in db.Supplier on cost.Suppler_ID equals s.Id
                            join company in db.CRM_Company on cost.CRM_Company_ID equals company.ID
                                into fee
                            from l in fee.DefaultIfEmpty()
                            select new CostFeeModel()
                            {
                                ID = cost.ID,
                                CostTableType = cost.CostTableType,
                                YearMonth = cost.YearMonth,
                                SerialNumber = cost.SerialNumber,
                                ChargeCost = cost.ChargeCost,
                                ServiceCost = cost.ServiceCost,
                                Remark = cost.Remark,
                                Status = cost.Status,
                                CRM_Company_ID = cost.CRM_Company_ID,
                                CompanyName = l.CompanyName,
                                CreateTime = cost.CreateTime,
                                CreateUserID = cost.CreateUserID,
                                CreateUserName = cost.CreateUserName,
                                BranchID = cost.BranchID,
                                CreateFrom = cost.CreateFrom,
                                Suppler = s.Name,
                                Suppler_ID = cost.Suppler_ID
                            };

            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                if (queryDic.ContainsKey("yearMonth$") && !string.IsNullOrWhiteSpace(queryDic["yearMonth$"]))
                {
                    var datetime_Int = Convert.ToInt32(queryDic["yearMonth$"]);
                    queryLeft = queryLeft.Where(x => x.YearMonth == datetime_Int);
                }

                //if (queryDic.ContainsKey("yearMonth$") && !string.IsNullOrWhiteSpace(queryDic["yearMonth$"]))
                //{
                //    var datetime_Int = Convert.ToInt32(queryDic["yearMonth$"]);
                //    queryLeft = queryLeft.Where(x => x.YearMonth == datetime_Int);
                //}
                if (queryDic.ContainsKey("costTableType") && !string.IsNullOrWhiteSpace(queryDic["costTableType"]))
                {
                    var costTableType = Convert.ToInt32(queryDic["costTableType"]);
                    queryLeft = queryLeft.Where(x => x.CostTableType == costTableType);
                }
                if (queryDic.ContainsKey("status") && !string.IsNullOrWhiteSpace(queryDic["status"]))
                {
                    var status = Convert.ToInt32(queryDic["status"]);
                    queryLeft = queryLeft.Where(x => x.Status == status);
                }
                if (queryDic.ContainsKey("Suppler_ID") && !string.IsNullOrWhiteSpace(queryDic["Suppler_ID"]))
                {
                    var Suppler_ID = Convert.ToInt32(queryDic["Suppler_ID"]);
                    queryLeft = queryLeft.Where(x => x.Suppler_ID == Suppler_ID);
                }
            }
            if (companyId != null)
            {
                queryLeft = queryLeft.Where(x => companyId.Contains(x.CRM_Company_ID));
            }

            return queryLeft.OrderBy(o => o.YearMonth).ThenBy(o => o.SerialNumber);
        }

        #endregion

        #region 获取费用表流水号（每月从1开始）


        /// <summary>
        /// 获取费用表流水号（每月从1开始）
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <returns></returns>
        public string GetSerialNumber(int yearMonth)
        {
            using (SysEntities db = new SysEntities())
            {
                var query = db.COST_CostTable.Where(cct => cct.YearMonth == yearMonth);
                if (query.Count() > 0)
                {
                    var serinlNumber = query.OrderByDescending(o => o.ID).FirstOrDefault().SerialNumber;
                    return yearMonth.ToString() + (Convert.ToInt32(serinlNumber.Substring(6)) + 1).ToString("000000");
                }
                else return yearMonth.ToString() + "000001";
            }
        }
        #endregion
    }
}

