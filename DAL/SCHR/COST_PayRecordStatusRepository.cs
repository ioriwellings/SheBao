using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
using Langben.DAL.Model;
using System.Transactions;
using System.Text;
using System.Data.Entity.Validation;
namespace Langben.DAL
{
    /// <summary>
    /// 费用_社保支出导入汇总
    /// </summary>
    public partial class COST_PayRecordStatusRepository
    {
        /// <summary>
        /// 查询的数据(支出费用确认列表)
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="dateTime">年月</param>
        /// <param name="costType">险种</param>
        /// <param name="personId">客服ID</param>
        /// <returns></returns>      
        public IQueryable<COST_PayRecordConfirm> GetPayRecordStatusList(SysEntities db, string dateTime, int costType, List<int?> personId, string cityId, int currentUser)
        {
            var datetime_Int = Convert.ToInt32(dateTime);

            #region 权限代码
            // 险种权限控制（当前社保客服负责的险种）
            //var query = (from insuranceKind in db.InsuranceKind
            //             join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
            //             where userInsurance.UserID == currentUser
            //             select insuranceKind.Name).Distinct().ToList();
            //int[] costTypeList = new int[query.Count()];
            //for (int i = 0; i < query.Count(); i++)
            //{
            //    costTypeList[i] = EnumsCommon.GetInsuranceKindValue(query[i]);  // 险种对应的costType编号
            //}

            // 获取当前社保客服负责的险种及缴纳地
            var query = from insuranceKind in db.InsuranceKind
                        join userInsuranceKind in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsuranceKind.InsuranceKindId
                        where userInsuranceKind.UserID == currentUser
                        select insuranceKind;

            var queryLeft = from payRecord in db.COST_PayRecord.Where(x => x.YearMonth == datetime_Int)
                            //join userCity in db.ORG_UserCity on payRecord.CityId equals userCity.CityId
                            //where userCity.UserID == currentUser  // 缴纳地权限控制
                            join payRecordStatus in db.COST_PayRecordStatus on payRecord.ID equals payRecordStatus.COST_PayRecordId
                            join insuranceKind in query on new { costType = payRecordStatus.CostType, cityId = payRecord.CityId }
                                equals new { costType = insuranceKind.InsuranceKindId ?? 0, cityId = insuranceKind.City }
                            join city in db.City on payRecord.CityId equals city.Id
                            select new COST_PayRecordConfirm
                            {
                                ID = payRecordStatus.ID,
                                CostType = payRecordStatus.CostType,
                                Status = payRecordStatus.Status,
                                COST_PayRecordId = payRecordStatus.COST_PayRecordId,
                                CompanyCost = payRecordStatus.CompanyCost,
                                PersonCost = payRecordStatus.PersonCost,
                                AllCount = payRecordStatus.AllCount,
                                CreateTime = payRecordStatus.CreateTime,
                                CreateUserName = payRecordStatus.CreateUserName,
                                CreateUserID = payRecordStatus.CreateUserID,
                                CityId = city.Id,
                                CityName = city.Name
                            };
            //queryLeft = queryLeft.Where(x => costTypeList.Contains(x.CostType));  // 险种权限控制
            #endregion

            if (personId != null)
            {
                queryLeft = queryLeft.Where(x => personId.Contains(x.CreateUserID));
            }
            if (costType != 0)
            {
                queryLeft = queryLeft.Where(x => x.CostType == costType);
            }
            if (!String.IsNullOrEmpty(cityId) && cityId != "0")
            {
                queryLeft = queryLeft.Where(x => x.CityId == cityId);
            }
            return queryLeft.OrderBy(o => o.CityId).ThenBy(o => o.CostType).ThenBy(o => o.ID);
        }

        /// <summary>
        /// 查询要加入对比的支出数据汇总
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="costType">险种</param>
        /// <param name="cityId">缴纳地</param>
        /// <returns></returns>      
        public List<COST_PayRecordSummary> GetPayRecordContrastedList(SysEntities db, string yearMonth, int costType, string cityId)
        {
            var datetime_Int = Convert.ToInt32(yearMonth);

            var queryLeft = from payRecord in db.COST_PayRecord
                            where payRecord.YearMonth == datetime_Int && payRecord.CityId == cityId
                            join city in db.City on payRecord.CityId equals city.Id
                            join payRecordStatus in db.COST_PayRecordStatus on payRecord.ID equals payRecordStatus.COST_PayRecordId
                            where payRecordStatus.CostType == costType
                            select new
                            {
                                ID = payRecordStatus.ID,
                                CompanyCost = payRecordStatus.CompanyCost,
                                PersonCost = payRecordStatus.PersonCost,
                                Status = payRecordStatus.Status,
                                CityName = city.Name
                            };

            decimal? companyCost = 0;
            decimal? personCost = 0;
            int count = 0;
            int isLocked = 0;
            var statusList = queryLeft.ToList();

            #region 金额总额计算
            companyCost = (from record in statusList select record.CompanyCost).Sum();
            personCost = (from record in statusList select record.PersonCost).Sum();
            #endregion

            #region 是否可以进行对比判断
            int unLockedCount = statusList.Where(x => x.Status != (int)Common.COST_PayRecord_Status.已锁定).Count();

            if (unLockedCount > 0)
            {
                // 存在未锁定或已对比的支出数据
                isLocked = 1;  // 0:可对比；1：不可对比
            }
            #endregion

            #region 人数计算
            var list = from status in statusList select status.ID;
            switch (costType)
            {
                case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                    count = (from yanglao in db.COST_PayYangLao
                             where list.Contains(yanglao.COST_PayRecordStatusID)
                             //where (from status in statusList select status.ID).Contains(yanglao.COST_PayRecordStatusID)
                             select yanglao.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                    count = (from yiliao in db.COST_PayYiLiao
                             where list.Contains(yiliao.COST_PayRecordStatusID)
                             select yiliao.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                    count = (from shiye in db.COST_PayShiYe
                             where list.Contains(shiye.COST_PayRecordStatusID)
                             select shiye.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                    count = (from shengyu in db.COST_PayShengYu
                             where list.Contains(shengyu.COST_PayRecordStatusID)
                             select shengyu.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                    count = (from gongshang in db.COST_PayGongShang
                             where list.Contains(gongshang.COST_PayRecordStatusID)
                             select gongshang.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                    count = (from gongjijin in db.COST_PayGongJiJin
                             where list.Contains(gongjijin.COST_PayRecordStatusID)
                             select gongjijin.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                    count = (from dae in db.COST_PayYiLiaoDaE
                             where list.Contains(dae.COST_PayRecordStatusID)
                             select dae.EmployeeId).Distinct().Count();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.补充公积金:
                    count = (from buchong in db.COST_PayGongJiJinBC
                             where list.Contains(buchong.COST_PayRecordStatusID)
                             select buchong.EmployeeId).Distinct().Count();
                    break;
            }
            #endregion

            List<COST_PayRecordSummary> recordAllList = new List<COST_PayRecordSummary>();
            if (statusList.Count() > 0)
            {
                COST_PayRecordSummary payRecordSummary = new COST_PayRecordSummary();
                payRecordSummary.YearMonth = datetime_Int;
                payRecordSummary.CostType = costType;
                payRecordSummary.CompanyCost = companyCost;
                payRecordSummary.PersonCost = personCost;
                payRecordSummary.Status = isLocked;  // 用status来记录是否可进行对比
                payRecordSummary.Count = count;
                payRecordSummary.CityId = cityId;
                payRecordSummary.CityName = statusList != null ? statusList[0].CityName : "";

                recordAllList.Add(payRecordSummary);
            }

            return recordAllList;
        }

        /// <summary>
        /// 删除支出费用
        /// </summary>
        /// <param name="db">实体数据</param>
        /// <param name="id">费用_社保支出导入汇总 主键</param>
        /// <returns></returns>
        public void DeletePayRecord(SysEntities db, int id)
        {
            // 删除COST_PayRecordStatus表中记录
            COST_PayRecordStatus payRecordItem = GetById(db, id);

            if (payRecordItem != null)
            {
                int costType = payRecordItem.CostType;
                db.COST_PayRecordStatus.Remove(payRecordItem);

                #region 删除明细表中数据
                switch (costType)
                {
                    case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                        // 费用_社保支出养老
                        var yanglao = from f in db.COST_PayYangLao
                                      where f.COST_PayRecordStatusID == id
                                      select f;
                        foreach (var item in yanglao)
                        {
                            db.COST_PayYangLao.Remove(item);
                        }
                        break;
                    case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                        // 费用_社保支出医疗
                        var yiliao = from f in db.COST_PayYiLiao
                                     where f.COST_PayRecordStatusID == id
                                     select f;
                        foreach (var item in yiliao)
                        {
                            db.COST_PayYiLiao.Remove(item);
                        }
                        break;
                    case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                        // 费用_社保支出失业
                        var shiye = from f in db.COST_PayShiYe
                                    where f.COST_PayRecordStatusID == id
                                    select f;
                        foreach (var item in shiye)
                        {
                            db.COST_PayShiYe.Remove(item);
                        }
                        break;
                    case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                        // 费用_社保支出工伤
                        var gongshang = from f in db.COST_PayGongShang
                                        where f.COST_PayRecordStatusID == id
                                        select f;
                        foreach (var item in gongshang)
                        {
                            db.COST_PayGongShang.Remove(item);
                        }
                        break;
                    case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                        // 费用_社保支出公积金
                        var gongjijin = from f in db.COST_PayGongJiJin
                                        where f.COST_PayRecordStatusID == id
                                        select f;
                        foreach (var item in gongjijin)
                        {
                            db.COST_PayGongJiJin.Remove(item);
                        }
                        break;
                    case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                        // 费用_社保支出医疗大额
                        var dae = from f in db.COST_PayYiLiaoDaE
                                  where f.COST_PayRecordStatusID == id
                                  select f;
                        foreach (var item in dae)
                        {
                            db.COST_PayYiLiaoDaE.Remove(item);
                        }
                        break;
                    case (int)Common.EmployeeAdd_InsuranceKindId.补充公积金:
                        // 费用_社保支出补充公积金
                        var bcGongjijin = from f in db.COST_PayGongJiJinBC
                                          where f.COST_PayRecordStatusID == id
                                          select f;
                        foreach (var item in bcGongjijin)
                        {
                            db.COST_PayGongJiJinBC.Remove(item);
                        }
                        break;
                }

                #endregion
            }
        }

        /// <summary>
        /// 更新支出费用表状态
        /// </summary>
        /// <param name="id">费用表主键</param>
        /// <param name="status">要更新成的状态值</param>
        /// <returns></returns>
        public int UpdatePayRecordStatus(int id, int status)
        {
            using (SysEntities db = new SysEntities())
            {
                COST_PayRecordStatus updateItem = GetById(db, id);
                if (updateItem != null)
                {
                    updateItem.Status = status;
                }
                return Save(db);
            }
        }

        /// <summary>
        /// 收支对比
        /// </summary>
        /// <param name="db">实体数据</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="cityId">缴纳地</param>
        /// <param name="costType">险种</param>
        /// <param name="1liersId">供应商</param>
        /// <param name="userName">当前操作用户</param>
        /// <returns></returns>
        public void ContrastedInsurance(SysEntities db, int yearMonth, string cityId, int costType, string userName)
        {
            List<CostPayInsurance> payList = new List<CostPayInsurance>();

            #region 获取支出数据结果

            // 获取支出数据结果
            switch (costType)
            {
                case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                    #region 费用_社保支出养老
                    var yangLaoList = from yanglao in db.COST_PayYangLao
                                      join recordStatus in db.COST_PayRecordStatus on yanglao.COST_PayRecordStatusID equals recordStatus.ID
                                      join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                      where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  //&& record.SuppliersId == suppliersId //供应商先不考虑
                                      group yanglao by new
                                      {
                                          employeeId = yanglao.EmployeeId,
                                          companyId = yanglao.CompanyId
                                      }
                                          into s
                                          select new CostPayInsurance
                                          {
                                              EmployeeId = s.Key.employeeId,
                                              CompanyId = s.Key.companyId,
                                              PayCompanyCost = s.Sum(p => p.CompanyCost),
                                              PayPersonCost = s.Sum(p => p.PersonCost),
                                              CostCompanyCost = 0,
                                              CostPersonCost = 0
                                          };
                    payList = yangLaoList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                    #region 费用_社保支出医疗
                    var yiLiaoList = from yiliao in db.COST_PayYiLiao
                                     join recordStatus in db.COST_PayRecordStatus on yiliao.COST_PayRecordStatusID equals recordStatus.ID
                                     join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                     where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  //&& record.SuppliersId == suppliersId  // 供应商先不考虑
                                     group yiliao by new
                                     {
                                         employeeId = yiliao.EmployeeId,
                                         companyId = yiliao.CompanyId
                                     }
                                         into s
                                         select new CostPayInsurance
                                         {
                                             EmployeeId = s.Key.employeeId,
                                             CompanyId = s.Key.companyId,
                                             PayCompanyCost = s.Sum(p => p.CompanyCost),
                                             PayPersonCost = s.Sum(p => p.PersonCost),
                                             CostCompanyCost = 0,
                                             CostPersonCost = 0
                                         };
                    payList = yiLiaoList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                    #region 费用_社保支出失业
                    var shiYeList = from shiye in db.COST_PayShiYe
                                    join recordStatus in db.COST_PayRecordStatus on shiye.COST_PayRecordStatusID equals recordStatus.ID
                                    join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                    where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  // && record.SuppliersId == suppliersId //供应商先不考虑
                                    group shiye by new
                                    {
                                        employeeId = shiye.EmployeeId,
                                        companyId = shiye.CompanyId
                                    }
                                        into s
                                        select new CostPayInsurance
                                        {
                                            EmployeeId = s.Key.employeeId,
                                            CompanyId = s.Key.companyId,
                                            PayCompanyCost = s.Sum(p => p.CompanyCost),
                                            PayPersonCost = s.Sum(p => p.PersonCost),
                                            CostCompanyCost = 0,
                                            CostPersonCost = 0
                                        };
                    payList = shiYeList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                    #region 费用_社保支出工伤
                    var gongShangList = from gongshang in db.COST_PayGongShang
                                        join recordStatus in db.COST_PayRecordStatus on gongshang.COST_PayRecordStatusID equals recordStatus.ID
                                        join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                        where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  // && record.SuppliersId == suppliersId //供应商先不考虑
                                        group gongshang by new
                                      {
                                          employeeId = gongshang.EmployeeId,
                                          companyId = gongshang.CompanyId
                                      }
                                            into s
                                            select new CostPayInsurance
                                            {
                                                EmployeeId = s.Key.employeeId,
                                                CompanyId = s.Key.companyId,
                                                PayCompanyCost = s.Sum(p => p.CompanyCost),
                                                PayPersonCost = s.Sum(p => p.PersonCost),
                                                CostCompanyCost = 0,
                                                CostPersonCost = 0
                                            };
                    payList = gongShangList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                    #region 费用_社保支出生育
                    var shangyuList = from shengyu in db.COST_PayShengYu
                                      join recordStatus in db.COST_PayRecordStatus on shengyu.COST_PayRecordStatusID equals recordStatus.ID
                                      join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                      where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  // && record.SuppliersId == suppliersId //供应商先不考虑
                                      group shengyu by new
                                      {
                                          employeeId = shengyu.EmployeeId,
                                          companyId = shengyu.CompanyId
                                      }
                                          into s
                                          select new CostPayInsurance
                                          {
                                              EmployeeId = s.Key.employeeId,
                                              CompanyId = s.Key.companyId,
                                              PayCompanyCost = s.Sum(p => p.CompanyCost),
                                              PayPersonCost = s.Sum(p => p.PersonCost),
                                              CostCompanyCost = 0,
                                              CostPersonCost = 0
                                          };
                    payList = shangyuList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                    #region 费用_社保支出公积金
                    var gongJiJinList = from gongjijin in db.COST_PayGongJiJin
                                        join recordStatus in db.COST_PayRecordStatus on gongjijin.COST_PayRecordStatusID equals recordStatus.ID
                                        join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                        where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  // && record.SuppliersId == suppliersId //供应商先不考虑
                                        group gongjijin by new
                                        {
                                            employeeId = gongjijin.EmployeeId,
                                            companyId = gongjijin.CompanyId
                                        }
                                            into s
                                            select new CostPayInsurance
                                            {
                                                EmployeeId = s.Key.employeeId,
                                                CompanyId = s.Key.companyId,
                                                PayCompanyCost = s.Sum(p => p.CompanyCost),
                                                PayPersonCost = s.Sum(p => p.PersonCost),
                                                CostCompanyCost = 0,
                                                CostPersonCost = 0
                                            };
                    payList = gongJiJinList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                    #region 费用_社保支出医疗大额
                    var daEList = from dae in db.COST_PayYiLiaoDaE
                                  join recordStatus in db.COST_PayRecordStatus on dae.COST_PayRecordStatusID equals recordStatus.ID
                                  join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                  where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth   // && record.SuppliersId == suppliersId  //供应商先不考虑
                                  group dae by new
                                  {
                                      employeeId = dae.EmployeeId,
                                      companyId = dae.CompanyId
                                  }
                                      into s
                                      select new CostPayInsurance
                                      {
                                          EmployeeId = s.Key.employeeId,
                                          CompanyId = s.Key.companyId,
                                          PayCompanyCost = s.Sum(p => p.CompanyCost),
                                          PayPersonCost = s.Sum(p => p.PersonCost),
                                          CostCompanyCost = 0,
                                          CostPersonCost = 0
                                      };
                    payList = daEList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.补充公积金:
                    #region 费用_社保支出补充公积金
                    var buChongList = from buchong in db.COST_PayGongJiJinBC
                                      join recordStatus in db.COST_PayRecordStatus on buchong.COST_PayRecordStatusID equals recordStatus.ID
                                      join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                      where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth  // && record.SuppliersId == suppliersId // 供应商先不考虑
                                      group buchong by new
                                      {
                                          employeeId = buchong.EmployeeId,
                                          companyId = buchong.CompanyId
                                      }
                                          into s
                                          select new CostPayInsurance
                                          {
                                              EmployeeId = s.Key.employeeId,
                                              CompanyId = s.Key.companyId,
                                              PayCompanyCost = s.Sum(p => p.CompanyCost),
                                              PayPersonCost = s.Sum(p => p.PersonCost),
                                              CostCompanyCost = 0,
                                              CostPersonCost = 0
                                          };
                    payList = buChongList.ToList();
                    #endregion
                    break;
            }

            #endregion

            #region 获取收入数据结果

            int[] status = { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已核销, (int)Common.COST_Table_Status.已支付 };
            // 获取收入数据结果
            var costQuery = from insurance in db.COST_CostTableInsurance
                            join cost in db.COST_CostTable on insurance.COST_CostTable_ID equals cost.ID
                            where cost.CreateFrom == (int)Common.CostTable_CreateFrom.本地费用 && status.Contains(cost.Status) && insurance.CostType == costType && insurance.CityId == cityId && cost.YearMonth <= yearMonth
                            group insurance by new
                            {
                                employeeId = insurance.Employee_ID,
                                companyId = insurance.CRM_Company_ID
                            }
                                into s
                                select new CostPayInsurance
                                {
                                    EmployeeId = (int)s.Key.employeeId,
                                    CompanyId = s.Key.companyId,
                                    PayCompanyCost = 0,
                                    PayPersonCost = 0,
                                    CostCompanyCost = s.Sum(p => p.CompanyCost),
                                    CostPersonCost = s.Sum(p => p.PersonCost)
                                };
            List<CostPayInsurance> costList = costQuery.ToList();

            #endregion

            #region 进行收支对比，并将对比结果存到中间表中
            // 进行收支对比，并将对比结果存到中间表中
            var fullData = from full in payList.Union(costList)
                           group full by new { full.EmployeeId, full.CompanyId }
                               into g
                               select new
                               {
                                   EmployeeId = g.Key.EmployeeId,
                                   CompanyId = g.Key.CompanyId,
                                   PayCompanyCost = g.Sum(p => p.PayCompanyCost),
                                   PayPersonCost = g.Sum(p => p.PayPersonCost),
                                   CostCompanyCost = g.Sum(p => p.CostCompanyCost),
                                   CostPersonCost = g.Sum(p => p.CostPersonCost)
                               };
            var fullJoinData = fullData.Where(x => x.PayCompanyCost != x.CostCompanyCost || x.PayPersonCost != x.CostPersonCost).ToList();  // 排除收支相等的数据

            // 将收支不相等的数据，存到费用中间表中
            foreach (var item in fullJoinData)
            {
                int employeeId = item.EmployeeId;
                int companyId = item.CompanyId;

                // 获取员工企业关系编号
                var relationList = from c in db.CompanyEmployeeRelation
                                   where c.EmployeeId == employeeId
                                   where c.CompanyId == companyId
                                   select new
                                   {
                                       ID = c.Id
                                   };
                var relationModel = relationList.ToList().FirstOrDefault();

                // 获取费用类型
                decimal companyCost = (decimal)item.PayCompanyCost - (decimal)item.CostCompanyCost;
                decimal personCost = (decimal)item.PayPersonCost - (decimal)item.CostPersonCost;
                int style = (int)Common.EmployeeMiddle_PaymentStyle.补收;

                if (companyCost != 0)
                {
                    style = companyCost > 0 ? (int)Common.EmployeeMiddle_PaymentStyle.补收 : (int)Common.EmployeeMiddle_PaymentStyle.退费;
                }
                else
                {
                    style = personCost > 0 ? (int)Common.EmployeeMiddle_PaymentStyle.补收 : (int)Common.EmployeeMiddle_PaymentStyle.退费;
                }

                // 费用中间表中添加数据
                EmployeeMiddle middle = new EmployeeMiddle();
                middle.InsuranceKindId = costType;
                if (relationModel != null)
                {
                    middle.CompanyEmployeeRelationId = relationModel.ID;
                }
                middle.PaymentStyle = style;
                middle.CompanyPayment = companyCost;
                middle.EmployeePayment = personCost;
                middle.PaymentMonth = 1;
                int date = Convert.ToInt32((DateTime.Now.ToString("yyyy-MM")).Replace("-", ""));
                middle.StartDate = date;
                middle.EndedDate = date; // 开始、结束时间段为当前月
                middle.State = Common.Status.启用.ToString();
                middle.CreateTime = DateTime.Now;
                middle.CreatePerson = userName;
                middle.CityId = cityId;

                db.EmployeeMiddle.Add(middle);
            }
            #endregion

            #region 修改费用_社保支出导入汇总表的状态

            // 将参与对比的“已锁定”的数据，状态改为“已对比”（状态为已对比的没有取，因为不需要改状态）
            var recordStatusList = (from recordStatus in db.COST_PayRecordStatus
                                    join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                    where recordStatus.Status == (int)Common.COST_PayRecord_Status.已锁定 && recordStatus.CostType == costType && record.CityId == cityId && record.YearMonth <= yearMonth //&& record.SuppliersId == suppliersId //供应商先不考虑
                                    select recordStatus.ID).Distinct();

            foreach (var item in recordStatusList)
            {
                COST_PayRecordStatus updateItem = GetById(db, item);
                if (updateItem != null)
                {
                    updateItem.Status = (int)Common.COST_PayRecord_Status.已对比;
                }
            }
            #endregion
        }

        /// <summary>
        /// 查询的数据(员工收支对比列表)
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="yearMonthStart">起始年月</param>
        /// <param name="yearMonthEnd">结束年月</param>
        /// <param name="costType">险种(单一险种的查询)</param>
        /// <param name="companyId">公司ID</param>
        /// <param name="certificate">身份证号(支持多行查询)</param>
        /// <param name="employeeName">员工姓名（模糊查询）</param>
        /// <returns></returns>      
        public List<CostPayPersonContrasted> GetPayPersonContrastedList(SysEntities db, int yearMonthStart, int yearMonthEnd, int costType, List<int> companyId, List<string> cityId, string certificate, string employeeName)
        {
            List<CostPayPersonContrasted> payList = new List<CostPayPersonContrasted>();

            #region 身份证号数组拆分
            List<string> cardList = new List<string>();
            if (!string.IsNullOrEmpty(certificate))
            {
                string[] certificateList = certificate.Split(Convert.ToChar(10));
                for (int i = 0; i < certificateList.Length; i++)
                {
                    cardList.Add(certificateList[i]);
                    cardList.Add(CardCommon.CardIDTo15(certificateList[i]));
                    cardList.Add(CardCommon.CardIDTo18(certificateList[i]));
                }
                cardList = cardList.Distinct().ToList();
            }
            #endregion

            #region 获取支出数据结果

            // 获取支出数据结果
            switch (costType)
            {
                case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                    #region 费用_社保支出养老
                    // 获取支出数据结果
                    var yangLaoList = from yanglao in db.COST_PayYangLao
                                      join recordStatus in db.COST_PayRecordStatus on yanglao.COST_PayRecordStatusID equals recordStatus.ID
                                      join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                      where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && yanglao.YearMonth >= yearMonthStart && yanglao.YearMonth <= yearMonthEnd
                                      group yanglao by new
                                      {
                                          employeeId = yanglao.EmployeeId,
                                          companyId = yanglao.CompanyId,
                                          cityId = yanglao.CityId
                                      }
                                          into s
                                          select new CostPayPersonContrasted
                                          {
                                              EmployeeId = s.Key.employeeId,
                                              CompanyId = s.Key.companyId,
                                              CityId = s.Key.cityId,
                                              EmployeeName = s.Max(p => p.EmployeeName),
                                              Certificate = s.Max(p => p.CertificateNumber),
                                              PayCompanyCost = s.Sum(p => p.CompanyCost),
                                              PayPersonCost = s.Sum(p => p.PersonCost),
                                              CostCompanyCost = 0,
                                              CostPersonCost = 0
                                          };
                    payList = yangLaoList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                    #region 费用_社保支出医疗
                    var yiliaoList = from yiliao in db.COST_PayYiLiao
                                     join recordStatus in db.COST_PayRecordStatus on yiliao.COST_PayRecordStatusID equals recordStatus.ID
                                     join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                     where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && yiliao.YearMonth >= yearMonthStart && yiliao.YearMonth <= yearMonthEnd
                                     group yiliao by new
                                      {
                                          employeeId = yiliao.EmployeeId,
                                          companyId = yiliao.CompanyId,
                                          cityId = yiliao.CityId
                                      }
                                         into s
                                         select new CostPayPersonContrasted
                                         {
                                             EmployeeId = s.Key.employeeId,
                                             CompanyId = s.Key.companyId,
                                             CityId = s.Key.cityId,
                                             EmployeeName = s.Max(p => p.EmployeeName),
                                             Certificate = s.Max(p => p.CertificateNumber),
                                             PayCompanyCost = s.Sum(p => p.CompanyCost),
                                             PayPersonCost = s.Sum(p => p.PersonCost),
                                             CostCompanyCost = 0,
                                             CostPersonCost = 0
                                         };
                    payList = yiliaoList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                    #region 费用_社保支出失业
                    var shiyeList = from shiye in db.COST_PayShiYe
                                    join recordStatus in db.COST_PayRecordStatus on shiye.COST_PayRecordStatusID equals recordStatus.ID
                                    join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                    where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && shiye.YearMonth >= yearMonthStart && shiye.YearMonth <= yearMonthEnd
                                    group shiye by new
                                      {
                                          employeeId = shiye.EmployeeId,
                                          companyId = shiye.CompanyId,
                                          cityId = shiye.CityId
                                      }
                                        into s
                                        select new CostPayPersonContrasted
                                        {
                                            EmployeeId = s.Key.employeeId,
                                            CompanyId = s.Key.companyId,
                                            CityId = s.Key.cityId,
                                            EmployeeName = s.Max(p => p.EmployeeName),
                                            Certificate = s.Max(p => p.CertificateNumber),
                                            PayCompanyCost = s.Sum(p => p.CompanyCost),
                                            PayPersonCost = s.Sum(p => p.PersonCost),
                                            CostCompanyCost = 0,
                                            CostPersonCost = 0
                                        };
                    payList = shiyeList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                    #region 费用_社保支出工伤
                    var gongshangList = from gongshang in db.COST_PayGongShang
                                        join recordStatus in db.COST_PayRecordStatus on gongshang.COST_PayRecordStatusID equals recordStatus.ID
                                        join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                        where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && gongshang.YearMonth >= yearMonthStart && gongshang.YearMonth <= yearMonthEnd
                                        group gongshang by new
                                      {
                                          employeeId = gongshang.EmployeeId,
                                          companyId = gongshang.CompanyId,
                                          cityId = gongshang.CityId
                                      }
                                            into s
                                            select new CostPayPersonContrasted
                                            {
                                                EmployeeId = s.Key.employeeId,
                                                CompanyId = s.Key.companyId,
                                                CityId = s.Key.cityId,
                                                EmployeeName = s.Max(p => p.EmployeeName),
                                                Certificate = s.Max(p => p.CertificateNumber),
                                                PayCompanyCost = s.Sum(p => p.CompanyCost),
                                                PayPersonCost = s.Sum(p => p.PersonCost),
                                                CostCompanyCost = 0,
                                                CostPersonCost = 0
                                            };
                    payList = gongshangList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                    #region 费用_社保支出生育
                    var shengyuList = from shengyu in db.COST_PayShengYu
                                      join recordStatus in db.COST_PayRecordStatus on shengyu.COST_PayRecordStatusID equals recordStatus.ID
                                      join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                      where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && shengyu.YearMonth >= yearMonthStart && shengyu.YearMonth <= yearMonthEnd
                                      group shengyu by new
                                      {
                                          employeeId = shengyu.EmployeeId,
                                          companyId = shengyu.CompanyId,
                                          cityId = shengyu.CityId
                                      }
                                          into s
                                          select new CostPayPersonContrasted
                                          {
                                              EmployeeId = s.Key.employeeId,
                                              CompanyId = s.Key.companyId,
                                              CityId = s.Key.cityId,
                                              EmployeeName = s.Max(p => p.EmployeeName),
                                              Certificate = s.Max(p => p.CertificateNumber),
                                              PayCompanyCost = s.Sum(p => p.CompanyCost),
                                              PayPersonCost = s.Sum(p => p.PersonCost),
                                              CostCompanyCost = 0,
                                              CostPersonCost = 0
                                          };
                    payList = shengyuList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                    #region 费用_社保支出公积金
                    var gongjijinList = from gongjijin in db.COST_PayGongJiJin
                                        join recordStatus in db.COST_PayRecordStatus on gongjijin.COST_PayRecordStatusID equals recordStatus.ID
                                        join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                        where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && gongjijin.YearMonth >= yearMonthStart && gongjijin.YearMonth <= yearMonthEnd
                                        group gongjijin by new
                                      {
                                          employeeId = gongjijin.EmployeeId,
                                          companyId = gongjijin.CompanyId,
                                          cityId = gongjijin.CityId
                                      }
                                            into s
                                            select new CostPayPersonContrasted
                                            {
                                                EmployeeId = s.Key.employeeId,
                                                CompanyId = s.Key.companyId,
                                                CityId = s.Key.cityId,
                                                EmployeeName = s.Max(p => p.EmployeeName),
                                                Certificate = s.Max(p => p.CertificateNumber),
                                                PayCompanyCost = s.Sum(p => p.CompanyCost),
                                                PayPersonCost = s.Sum(p => p.PersonCost),
                                                CostCompanyCost = 0,
                                                CostPersonCost = 0
                                            };
                    payList = gongjijinList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                    #region 费用_社保支出医疗大额
                    var daeList = from dae in db.COST_PayYiLiaoDaE
                                  join recordStatus in db.COST_PayRecordStatus on dae.COST_PayRecordStatusID equals recordStatus.ID
                                  join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                  where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && dae.YearMonth >= yearMonthStart && dae.YearMonth <= yearMonthEnd
                                  group dae by new
                                     {
                                         employeeId = dae.EmployeeId,
                                         companyId = dae.CompanyId,
                                         cityId = dae.CityId
                                     }
                                      into s
                                      select new CostPayPersonContrasted
                                      {
                                          EmployeeId = s.Key.employeeId,
                                          CompanyId = s.Key.companyId,
                                          CityId = s.Key.cityId,
                                          EmployeeName = s.Max(p => p.EmployeeName),
                                          Certificate = s.Max(p => p.CertificateNumber),
                                          PayCompanyCost = s.Sum(p => p.CompanyCost),
                                          PayPersonCost = s.Sum(p => p.PersonCost),
                                          CostCompanyCost = 0,
                                          CostPersonCost = 0
                                      };
                    payList = daeList.ToList();
                    #endregion
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.补充公积金:
                    #region 费用_社保支出补充公积金
                    var buchongList = from buchong in db.COST_PayGongJiJinBC
                                      join recordStatus in db.COST_PayRecordStatus on buchong.COST_PayRecordStatusID equals recordStatus.ID
                                      join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                                      where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType && buchong.YearMonth >= yearMonthStart && buchong.YearMonth <= yearMonthEnd
                                      group buchong by new
                                      {
                                          employeeId = buchong.EmployeeId,
                                          companyId = buchong.CompanyId,
                                          cityId = buchong.CityId
                                      }
                                          into s
                                          select new CostPayPersonContrasted
                                          {
                                              EmployeeId = s.Key.employeeId,
                                              CompanyId = s.Key.companyId,
                                              CityId = s.Key.cityId,
                                              EmployeeName = s.Max(p => p.EmployeeName),
                                              Certificate = s.Max(p => p.CertificateNumber),
                                              PayCompanyCost = s.Sum(p => p.CompanyCost),
                                              PayPersonCost = s.Sum(p => p.PersonCost),
                                              CostCompanyCost = 0,
                                              CostPersonCost = 0
                                          };
                    payList = buchongList.ToList();
                    #endregion
                    break;
            }
            if (companyId != null)
            {
                payList = payList.Where(x => companyId.Contains(x.CompanyId)).ToList();
            }
            if (cityId != null)
            {
                payList = payList.Where(x => cityId.Contains(x.CityId)).ToList();
            }
            if (certificate != "" && cardList.Count() > 0)
            {
                payList = payList.Where(x => cardList.Contains(x.Certificate)).ToList();
            }
            if (employeeName != "")
            {
                payList = payList.Where(x => x.EmployeeName.Contains(employeeName)).ToList();
            }

            #endregion

            #region 获取收入数据结果

            int[] status = { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已核销, (int)Common.COST_Table_Status.已支付 };
            // 获取收入数据结果
            var costQuery = from insurance in db.COST_CostTableInsurance
                            join cost in db.COST_CostTable on insurance.COST_CostTable_ID equals cost.ID
                            where cost.CreateFrom == (int)Common.CostTable_CreateFrom.本地费用 && status.Contains(cost.Status) && insurance.CostType == costType && cost.YearMonth >= yearMonthStart && cost.YearMonth <= yearMonthEnd
                            group new { insurance, cost } by new
                            {
                                employeeId = insurance.Employee_ID,
                                companyId = insurance.CRM_Company_ID,
                                cityId = insurance.CityId
                            }
                                into s
                                select new CostPayPersonContrasted
                                {
                                    EmployeeId = (int)s.Key.employeeId,
                                    CompanyId = s.Key.companyId,
                                    CityId = s.Key.cityId,
                                    EmployeeName = s.Max(p => p.insurance.EmployName),
                                    Certificate = s.Max(p => p.insurance.CertificateNumber),
                                    PayCompanyCost = 0,
                                    PayPersonCost = 0,
                                    CostCompanyCost = s.Sum(p => p.insurance.CompanyCost),
                                    CostPersonCost = s.Sum(p => p.insurance.PersonCost)
                                };
            if (companyId != null)
            {
                costQuery = costQuery.Where(x => companyId.Contains(x.CompanyId));
            }
            if (cityId != null)
            {
                costQuery = costQuery.Where(x => cityId.Contains(x.CityId));
            }
            if (certificate != "" && cardList.Count() > 0)
            {
                costQuery = costQuery.Where(x => cardList.Contains(x.Certificate));
            }
            if (employeeName != "")
            {
                costQuery = costQuery.Where(x => x.EmployeeName.Contains(employeeName));
            }
            List<CostPayPersonContrasted> costList = costQuery.ToList();

            #endregion

            #region 进行收支对比
            // 进行收支对比
            var fullData = (from full in payList.Union(costList)
                            group full by new { full.EmployeeId, full.CompanyId, full.CityId } into g
                            select new CostPayPersonContrasted
                            {
                                EmployeeId = g.Key.EmployeeId,
                                CompanyId = g.Key.CompanyId,
                                CityId = g.Key.CityId,
                                EmployeeName = g.Max(p => p.EmployeeName),
                                Certificate = g.Max(p => p.Certificate),
                                PayCompanyCost = g.Sum(p => p.PayCompanyCost),
                                PayPersonCost = g.Sum(p => p.PayPersonCost),
                                CostCompanyCost = g.Sum(p => p.CostCompanyCost),
                                CostPersonCost = g.Sum(p => p.CostPersonCost)
                            });

            List<CostPayPersonContrasted> fullJoinData = (from full in fullData
                                                          join company in db.CRM_Company on full.CompanyId equals company.ID
                                                          into g
                                                          from l in g.DefaultIfEmpty()  // 左连接CRM_Company表
                                                          join city in db.City on full.CityId equals city.Id
                                                          into m
                                                          from n in m.DefaultIfEmpty() // 左连接City表
                                                          select new CostPayPersonContrasted
                                                          {
                                                              EmployeeId = full.EmployeeId,
                                                              CompanyId = full.CompanyId,
                                                              CityId = full.CityId,
                                                              CityName = n.Name,
                                                              EmployeeName = full.EmployeeName,
                                                              Certificate = full.Certificate,
                                                              CompanyName = l.CompanyName,
                                                              CompanyCode = l.CompanyCode,
                                                              PayCompanyCost = full.PayCompanyCost,
                                                              PayPersonCost = full.PayPersonCost,
                                                              CostCompanyCost = full.CostCompanyCost,
                                                              CostPersonCost = full.CostPersonCost
                                                          }).ToList();
            #endregion

            return fullJoinData;
        }


        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断）
        /// </summary>
        /// 责任客服可查询：自己负责的企业
        /// 员工客服可查询：自己负责的企业
        /// 社保客服可查询：所有企业
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        public List<CRM_Company> GetCompanyListByGroup(int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                string groupUser_SBKF = "SBKF";  // 用户组中“社保客服”的编码
                string groupUser_ZRKF = "ZRKF";  // 用户组中“责任客服”的编码
                string groupUser_YGKF = "YGKF";  // 用户组中“员工客服”的编码

                bool isUser_ZRKF = IsUserGroup(groupUser_ZRKF, userID);
                bool isUser_SBKF = IsUserGroup(groupUser_SBKF, userID);
                bool isUser_YGKF = IsUserGroup(groupUser_YGKF, userID);

                var query = db.CRM_Company.Where(o => true);

                if (isUser_SBKF)  // 社保客服可以看所有的企业
                {
                    // 企业不需要做任何过滤
                    return query.ToList();
                }

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

                #region 获取符合条件的企业信息
                if (isUser_ZRKF && isUser_YGKF)
                {
                    // 获取责任客服及员工客服负责的企业集合（取并集）
                    query = (from cc in query
                             join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                             join f in people on cctb.UserID_ZR equals f.ID
                             select cc)
                             .Union
                             (from cc in query
                              join ucc in db.UserCityCompany on cc.ID equals ucc.CompanyId
                              join f in people on ucc.UserID_YG equals f.ID
                              //join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                              //join f in people on cctb.UserID_YG equals f.ID
                              select cc);
                }
                else if (isUser_ZRKF)
                {
                    // 获取责任客服负责的企业
                    query = (from cc in query
                             join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                             join f in people on cctb.UserID_ZR equals f.ID
                             select cc);
                }
                else if (isUser_YGKF)
                {
                    // 获取员工客服负责的企业
                    query = (from cc in query
                             join ucc in db.UserCityCompany on cc.ID equals ucc.CompanyId
                             join f in people on ucc.UserID_YG equals f.ID
                             //join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                             //join f in people on cctb.UserID_YG equals f.ID
                             select cc);
                }
                #endregion

                return query.ToList();
            }
        }

        /// <summary>
        /// 根据用户组权限、险种类型，获取缴纳地列表（员工收支对比时用）
        /// </summary>
        /// 责任客服可查询：所有缴纳地
        /// 员工客服可查询：自己负责的缴纳地
        /// 社保客服可查询：自己负责的缴纳地
        /// <param name="userID">登录人ID</param>
        public List<City> GetCityListByGroup(int userID, int costType)
        {
            using (SysEntities db = new SysEntities())
            {
                string groupUser_SBKF = "SBKF";  // 用户组中“社保客服”的编码
                string groupUser_ZRKF = "ZRKF";  // 用户组中“责任客服”的编码
                string groupUser_YGKF = "YGKF";  // 用户组中“员工客服”的编码

                bool isUser_ZRKF = IsUserGroup(groupUser_ZRKF, userID);
                bool isUser_SBKF = IsUserGroup(groupUser_SBKF, userID);
                bool isUser_YGKF = IsUserGroup(groupUser_YGKF, userID);

                var query = db.City.Where(o => true);

                #region 权限代码
                if (isUser_ZRKF)  // 责任客服可以看所有的缴纳地
                {
                    // 缴纳地不需要做任何过滤
                    return query.ToList();
                }
                if (isUser_YGKF && isUser_SBKF)
                {
                    // 员工客服可以查看负责的缴纳地
                    var queryCityList = (from city in db.City
                                         join userCity in db.ORG_UserCity on city.Id equals userCity.CityId
                                         where userCity.UserID == userID
                                         select city)
                            .Union
                            (from city in db.City
                             join insuranceKind in db.InsuranceKind on city.Id equals insuranceKind.City
                             join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
                             where userInsurance.UserID == userID && insuranceKind.InsuranceKindId == costType
                             select city
                            ).Distinct().ToList();
                    return queryCityList;
                }
                else if (isUser_YGKF)
                {
                    return GetYGKFCityListByUser(userID);
                }

                if (isUser_SBKF)
                {
                    // 社保客服可以查看负责的缴纳地
                    var queryCityList = (from city in db.City
                                         join insuranceKind in db.InsuranceKind on city.Id equals insuranceKind.City
                                         join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
                                         where userInsurance.UserID == userID && insuranceKind.InsuranceKindId == costType
                                         select city).Distinct().ToList();
                    return queryCityList;
                }
                #endregion

                return query.ToList();
            }
        }

        /// <summary>
        /// 根据用户编号获取所负责的缴纳地(员工客服)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<City> GetYGKFCityListByUser(int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                #region 权限代码
                var query = from city in db.City
                            join userCity in db.ORG_UserCity on city.Id equals userCity.CityId
                            where userCity.UserID == userID
                            select city;

                #endregion
                return query.ToList();
            }
        }

        /// <summary>
        /// 根据用户编号获取所负责的缴纳地(社保客服)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<City> GetSBKFCityListByUser(int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                #region 权限代码
                // 社保客服只能查询自己负责的缴纳地
                var query = (from city in db.City
                             join insuranceKind in db.InsuranceKind on city.Id equals insuranceKind.City
                             join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
                             where userInsurance.UserID == userID
                             select city).Distinct().ToList();

                #endregion
                return query;
            }
        }

        /// <summary>
        /// 根据用户组权限获取险种
        /// </summary>
        /// <param name="userID">当前用户编号</param>
        /// <returns></returns>
        public List<EnumsCommon.EnumsListModel> GetCostTypeByGroup(int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                string groupUser_SBKF = "SBKF";  // 用户组中“社保客服”的编码
                string groupUser_ZRKF = "ZRKF";  // 用户组中“责任客服”的编码
                string groupUser_YGKF = "YGKF";  // 用户组中“员工客服”的编码

                bool isUser_ZRKF = IsUserGroup(groupUser_ZRKF, userID);
                bool isUser_SBKF = IsUserGroup(groupUser_SBKF, userID);
                bool isUser_YGKF = IsUserGroup(groupUser_YGKF, userID);

                List<EnumsCommon.EnumsListModel> enumsList = new List<EnumsCommon.EnumsListModel>();

                #region 权限代码
                if (isUser_ZRKF || isUser_YGKF)
                {
                    // 若为责任客服或员工客服，则可以查看所有险种
                    enumsList = EnumsCommon.GetEnumList(typeof(Common.EmployeeAdd_InsuranceKindId));
                }
                else if (isUser_SBKF)
                {
                    // 社保客服只能查询自己负责的险种
                    var query = (from insuranceKind in db.InsuranceKind
                                 join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
                                 where userInsurance.UserID == userID
                                 select insuranceKind.Name).Distinct().ToList();
                    foreach (var item in query)
                    {
                        EnumsCommon.EnumsListModel model = new EnumsCommon.EnumsListModel();
                        model.Code = EnumsCommon.GetInsuranceKindValue(item);   // 险种对应的costType编号
                        model.Name = item;
                        enumsList.Add(model);
                    }
                }

                enumsList = EnumsCommon.GetEnumList(typeof(Common.EmployeeAdd_InsuranceKindId));
                #endregion

                return enumsList;
            }
        }

        /// <summary>
        /// 获取社保客服负责的险种及缴纳地信息
        /// </summary>
        /// <param name="userID">当前用户编号</param>
        /// <returns></returns>
        public List<InsuranceKind> GetSBKFInsuranceKindByUser(int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                // 社保客服只能查询自己负责的险种
                var query = (from insuranceKind in db.InsuranceKind
                             join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
                             where userInsurance.UserID == userID
                             select insuranceKind).ToList();

                return query;
            }
        }

        /// <summary>
        /// 根据缴纳地获取社保客服负责的险种
        /// </summary>
        /// <param name="userID">当前用户编号</param>
        /// <returns></returns>
        public List<Langben.DAL.Model.CostType> GetSBKFCostTypeByCity(int userID, string cityId)
        {
            using (SysEntities db = new SysEntities())
            {
                // 社保客服只能查询自己负责的险种
                var query = (from insuranceKind in db.InsuranceKind.Where(x => x.City == cityId)
                             join userInsurance in db.ORG_UserInsuranceKind on insuranceKind.Id equals userInsurance.InsuranceKindId
                             where userInsurance.UserID == userID
                             select new Langben.DAL.Model.CostType
                             {
                                 Code = insuranceKind.InsuranceKindId ?? 0,
                                 Name = insuranceKind.Name
                             }).Distinct().ToList();

                return query;
            }
        }

        /// <summary>
        /// 判断当前用户是否在指定的用户组中
        /// </summary>
        /// <param name="groupCode">用户组编码</param>
        /// <param name="userId">当前用户编号</param>
        /// <returns></returns>
        public bool IsUserGroup(string groupCode, int userId)
        {
            using (SysEntities db = new SysEntities())
            {
                var query = from userGroup in db.ORG_GroupUser
                            join gro in db.ORG_Group on userGroup.ORG_Group_ID equals gro.ID
                            where gro.Code == groupCode && userGroup.ORG_User_ID == userId
                            select userGroup;
                return query.Count() > 0;
            }
        }

        /// <summary>
        /// 获取用户组列表
        /// </summary>
        public List<ORG_User> GetPersonListByGroupCode(string groupCode, int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                var query = from user in db.ORG_User
                            join groupUser in db.ORG_GroupUser on user.ID equals groupUser.ORG_User_ID
                            join gro in db.ORG_Group on groupUser.ORG_Group_ID equals gro.ID
                            where gro.Code == groupCode  // 用户组为groupCode的用户组编码
                            select user;

                switch (departmentScope)
                {
                    case (int)Common.DepartmentScopeAuthority.无限制: // 无限制
                        //不做任何逻辑判断，查询所有部门数据
                        if (!string.IsNullOrEmpty(departments))
                        {
                            // 获取特定部门所有人员
                            if (departments != "")
                            {
                                int[] departmentList = Array.ConvertAll<string, int>(departments.Split(','), delegate(string s) { return int.Parse(s); });
                                query = from a in query
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

                        query = from a in query
                                join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                select a;
                        break;
                    case (int)Common.DepartmentScopeAuthority.本机构:
                        //查询本机构所有部门数据
                        query = from a in query
                                join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                                select a;
                        break;
                    case (int)Common.DepartmentScopeAuthority.本部门及其下属部门:
                        //当前用户所属部门
                        ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);
                        //查询本部门及下属部门所有部门数据
                        query = from a in query
                                join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                select a;
                        break;
                    case (int)Common.DepartmentScopeAuthority.本部门:
                        //查询本部门所有用户数据
                        query = from a in query.Where(o => o.XYBZ == "Y")
                                join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == departmentID) on a.ORG_Department_ID equals b.ID
                                select a;

                        break;
                    case (int)Common.DepartmentScopeAuthority.本人:
                        query = from a in query.Where(o => o.ID == userID) select a;
                        break;
                }

                return query.ToList();
            }
        }




        #region 数据分割单
        public List<CostPayFenGe> GetCostPayFenGeList(int Kinds, int QIJIAN, int? CompanyId, bool Page, int PageSize, int CurPage, out int Tatal_Count)
        {
            int TatalCount = 0;
            List<CostPayFenGe> list = new List<CostPayFenGe>();
            using (SysEntities db = new SysEntities())
            {
                switch (Kinds)
                {
                    case (int)CostPay_InsuranceKind.养老:

                        var yanglao = db.COST_PayYangLao.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            yanglao = yanglao.Where(x => x.CompanyId == CompanyId);
                        }

                        var yllist = from a in yanglao
                                     join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                     from dept in JoinedEmpDept.DefaultIfEmpty()
                                     group a by new { a.CompanyId, a.CompanyName } into g
                                     select new CostPayFenGe
                                     {
                                         C_NAME = g.Key.CompanyName,
                                         CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                         CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                         CID = g.Key.CompanyId,
                                         H_Sum = g.Sum(x => (x.Total ?? 0)) + g.Sum(x => (x.PayFee ?? 0)),
                                         QIJIAN = QIJIAN,
                                         工本费 = g.Sum(x => (x.PayFee ?? 0)),

                                     };
                        TatalCount = yllist.Count();
                        if (Page)
                        {
                            list = yllist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = yllist.ToList();
                        }

                        break;
                    case (int)CostPay_InsuranceKind.医疗:
                        var yiliao = db.COST_PayYiLiao.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            yiliao = yiliao.Where(x => x.CompanyId == CompanyId);
                        }
                        var yiliaolist = from a in yiliao
                                         join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                         from dept in JoinedEmpDept.DefaultIfEmpty()
                                         group a by new { a.CompanyId, a.CompanyName } into g
                                         select new CostPayFenGe
                                         {

                                             C_NAME = g.Key.CompanyName,
                                             CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                             CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                             CID = g.Key.CompanyId,
                                             H_Sum = g.Sum(x => (x.Total ?? 0)) + g.Sum(x => (x.PayFee ?? 0)),
                                             QIJIAN = QIJIAN,
                                             工本费 = g.Sum(x => (x.PayFee ?? 0)),


                                         };
                        TatalCount = yiliaolist.Count();
                        if (Page)
                        {
                            list = yiliaolist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = yiliaolist.ToList();
                        }
                        break;
                    case (int)CostPay_InsuranceKind.失业:
                        var shiye = db.COST_PayShiYe.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            shiye = shiye.Where(x => x.CompanyId == CompanyId);
                        }
                        var shiyelist = from a in shiye
                                        join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                        from dept in JoinedEmpDept.DefaultIfEmpty()
                                        group a by new { a.CompanyId, a.CompanyName } into g
                                        select new CostPayFenGe
                                        {
                                            //C_NAME = a.CompanyName,
                                            //CHARGE_C = a.CompanyCost ?? 0,
                                            //CHARGE_P = a.PersonCost ?? 0,
                                            //CID = a.CompanyId,
                                            //H_Sum = a.Total ?? 0,
                                            //P_ID = a.EmployeeId,
                                            //QIJIAN = a.YearMonth,
                                            //SB_CARD_ID = a.CertificateNumberSB,
                                            //YANGLAO_CARD_ID = a.CertificateNumber,
                                            //工本费 = a.PayFee ?? 0,
                                            //CANBAODI = dept.Name,
                                            C_NAME = g.Key.CompanyName,
                                            CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                            CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                            CID = g.Key.CompanyId,
                                            H_Sum = g.Sum(x => (x.Total ?? 0)) + g.Sum(x => (x.PayFee ?? 0)),
                                            QIJIAN = QIJIAN,
                                            工本费 = g.Sum(x => (x.PayFee ?? 0)),
                                        };
                        TatalCount = shiyelist.Count();
                        if (Page)
                        {
                            list = shiyelist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = shiyelist.ToList();
                        }
                        break;
                    case (int)CostPay_InsuranceKind.工伤:
                        var gs = db.COST_PayGongShang.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            gs = gs.Where(x => x.CompanyId == CompanyId);
                        }
                        var gongshang = from a in gs
                                        join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                        from dept in JoinedEmpDept.DefaultIfEmpty()
                                        group a by new { a.CompanyId, a.CompanyName } into g
                                        select new CostPayFenGe
                                        {
                                            C_NAME = g.Key.CompanyName,
                                            CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                            CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                            CID = g.Key.CompanyId,
                                            H_Sum = g.Sum(x => (x.Total ?? 0)) + g.Sum(x => (x.PayFee ?? 0)),
                                            QIJIAN = QIJIAN,
                                            工本费 = g.Sum(x => (x.PayFee ?? 0)),
                                            //C_NAME = a.CompanyName,
                                            //CHARGE_C = a.CompanyCost ?? 0,
                                            //CHARGE_P = a.PersonCost ?? 0,
                                            //CID = a.CompanyId,
                                            //H_Sum = a.Total ?? 0,
                                            //P_ID = a.EmployeeId,
                                            //QIJIAN = a.YearMonth,
                                            //SB_CARD_ID = a.CertificateNumberSB,
                                            //YANGLAO_CARD_ID = a.CertificateNumber,
                                            //工本费 = a.PayFee ?? 0,
                                            //CANBAODI = dept.Name,
                                        };
                        TatalCount = gongshang.Count();
                        if (Page)
                        {
                            list = gongshang.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = gongshang.ToList();
                        }
                        break;
                    case (int)CostPay_InsuranceKind.公积金:
                        var gjj = db.COST_PayGongJiJin.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            gjj = gjj.Where(x => x.CompanyId == CompanyId);
                        }
                        var gongjijinlist = from a in gjj
                                            join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                            from dept in JoinedEmpDept.DefaultIfEmpty()
                                            group a by new { a.CompanyId, a.CompanyName } into g
                                            select new CostPayFenGe
                                            {
                                                //C_NAME = a.CompanyName,
                                                //CHARGE_C = a.CompanyCost ?? 0,
                                                //CHARGE_P = a.PersonCost ?? 0,
                                                //CID = a.CompanyId,
                                                //H_Sum = a.Total ?? 0,
                                                //P_ID = a.EmployeeId,
                                                //QIJIAN = a.YearMonth,
                                                //SB_CARD_ID = a.CertificateNumberSB,
                                                //YANGLAO_CARD_ID = a.CertificateNumber,
                                                //工本费 = a.PayFee ?? 0,
                                                //CANBAODI = dept.Name,
                                                C_NAME = g.Key.CompanyName,
                                                CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                                CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                                CID = g.Key.CompanyId,
                                                H_Sum = g.Sum(x => (x.Total ?? 0)) + g.Sum(x => (x.PayFee ?? 0)),
                                                QIJIAN = QIJIAN,
                                                工本费 = g.Sum(x => (x.PayFee ?? 0)),
                                            };
                        TatalCount = gongjijinlist.Count();
                        if (Page)
                        {
                            list = gongjijinlist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = gongjijinlist.ToList();
                        }
                        break;
                    case (int)CostPay_InsuranceKind.补充公积金:
                        var bcgjj = db.COST_PayGongJiJinBC.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            bcgjj = bcgjj.Where(x => x.CompanyId == CompanyId);
                        }
                        var bcgjjlist = from a in bcgjj
                                        join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                        from dept in JoinedEmpDept.DefaultIfEmpty()
                                        group a by new { a.CompanyId, a.CompanyName } into g
                                        select new CostPayFenGe
                                        {
                                            C_NAME = g.Key.CompanyName,
                                            CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                            CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                            CID = g.Key.CompanyId,
                                            H_Sum = g.Sum(x => (x.Total ?? 0)),
                                            QIJIAN = QIJIAN,
                                            工本费 = 0,
                                            //C_NAME = a.CompanyName,
                                            //CHARGE_C = a.CompanyCost ?? 0,
                                            //CHARGE_P = a.PersonCost ?? 0,
                                            //CID = a.CompanyId,
                                            //H_Sum = a.Total ?? 0,
                                            //P_ID = a.EmployeeId,
                                            //QIJIAN = a.YearMonth,
                                            //SB_CARD_ID = a.CertificateNumberSB,
                                            //YANGLAO_CARD_ID = a.CertificateNumber,
                                            //工本费 = 0,
                                            //CANBAODI = dept.Name,
                                        };
                        TatalCount = bcgjjlist.Count();
                        if (Page)
                        {
                            list = bcgjjlist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = bcgjjlist.ToList();
                        }
                        break;
                    case (int)CostPay_InsuranceKind.生育:
                        var sy = db.COST_PayShengYu.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            sy = sy.Where(x => x.CompanyId == CompanyId);
                        }
                        var shengyulist = from a in sy
                                          join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                          from dept in JoinedEmpDept.DefaultIfEmpty()
                                          group a by new { a.CompanyId, a.CompanyName } into g
                                          select new CostPayFenGe
                                          {
                                              C_NAME = g.Key.CompanyName,
                                              CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                              CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                              CID = g.Key.CompanyId,
                                              H_Sum = g.Sum(x => (x.Total ?? 0)) + g.Sum(x => (x.PayFee ?? 0)),
                                              QIJIAN = QIJIAN,
                                              工本费 = g.Sum(x => (x.PayFee ?? 0)),
                                              //C_NAME = a.CompanyName,
                                              //CHARGE_C = a.CompanyCost ?? 0,
                                              //CHARGE_P = a.PersonCost ?? 0,
                                              //CID = a.CompanyId,
                                              //H_Sum = a.Total ?? 0,
                                              //P_ID = a.EmployeeId,
                                              //QIJIAN = a.YearMonth,
                                              //SB_CARD_ID = a.CertificateNumberSB,
                                              //YANGLAO_CARD_ID = a.CertificateNumber,
                                              //工本费 = a.PayFee ?? 0,
                                              //CANBAODI = dept.Name,
                                          };
                        TatalCount = shengyulist.Count();
                        if (Page)
                        {
                            list = shengyulist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = shengyulist.ToList();
                        }
                        break;
                    case (int)CostPay_InsuranceKind.医疗大额:
                        var de = db.COST_PayYiLiaoDaE.Where(x => x.COST_PayRecordStatusID != (int)COST_PayRecord_Status.未锁定 && x.YearMonth == QIJIAN);
                        if (CompanyId != null && CompanyId != 0)
                        {
                            de = de.Where(x => x.CompanyId == CompanyId);
                        }
                        var daelist = from a in de
                                      join b in db.City on a.CityId equals b.Id into JoinedEmpDept
                                      from dept in JoinedEmpDept.DefaultIfEmpty()
                                      group a by new { a.CompanyId, a.CompanyName } into g
                                      select new CostPayFenGe
                                      {
                                          C_NAME = g.Key.CompanyName,
                                          CHARGE_C = g.Sum(x => (x.CompanyCost ?? 0)),
                                          CHARGE_P = g.Sum(x => (x.PersonCost ?? 0)),
                                          CID = g.Key.CompanyId,
                                          H_Sum = g.Sum(x => (x.Total ?? 0)),
                                          QIJIAN = QIJIAN,
                                          工本费 = 0,
                                          //    C_NAME = a.CompanyName,
                                          //    CHARGE_C = a.CompanyCost ?? 0,
                                          //    CHARGE_P = a.PersonCost ?? 0,
                                          //    CID = a.CompanyId,
                                          //    H_Sum = a.Total ?? 0,
                                          //    P_ID = a.EmployeeId,
                                          //    QIJIAN = a.YearMonth,
                                          //    SB_CARD_ID = a.CertificateNumberSB,
                                          //    YANGLAO_CARD_ID = a.CertificateNumber,
                                          //    工本费 = 0,
                                          //    CANBAODI = dept.Name,
                                      };
                        TatalCount = daelist.Count();
                        if (Page)
                        {
                            list = daelist.OrderBy(x => x.CID).Take(PageSize * CurPage).Skip(PageSize * (CurPage - 1)).ToList();
                        }
                        else
                        {
                            list = daelist.ToList();
                        }
                        break;


                }
                Tatal_Count = TatalCount;
                return list;
            }


        }
        #endregion

        #region 供应商费用表导入（供应商费用，供应商预算费用）
        public string ImportExcelForGYS(DataTable dt, int CostTable_CreateFrom, int YearMouth, int Suppler_ID, int UserID, int BranchID, string UserName)
        {
            decimal? ChargeCost = 0;
            try
            {


                using (TransactionScope tran = new TransactionScope())
                {
                    List<COST_Pay_Suppler> list = new List<COST_Pay_Suppler>();
                    using (SysEntities db = new SysEntities())
                    {
                        string error = verification(db, dt, YearMouth, out list, Suppler_ID, UserID, BranchID, UserName);
                        if (error != "")
                        {
                            return error;

                        }
                        //费用表主表
                        #region 费用表主表
                        COST_CostTable cost = new COST_CostTable();
                        cost.BranchID = BranchID;
                        cost.ChargeCost = 0;
                        cost.CostTableType = (int)COST_Table_CostTableType.大户代理;
                        cost.CreateFrom = CostTable_CreateFrom;
                        cost.CreateTime = DateTime.Now;
                        cost.CreateUserID = UserID;
                        cost.CreateUserName = UserName;
                        cost.CRM_Company_ID = 0;//只要不是本地的，公司的ID都是-1
                        cost.SerialNumber = new COST_CostTableRepository().GetSerialNumber(YearMouth);
                        cost.Status = (int)Common.COST_Table_Status.供应商客服导入;//待责任客服确认
                        cost.Suppler_ID = Suppler_ID;
                        cost.ServiceCost = 0;
                        cost.YearMonth = YearMouth;
                        cost.Remark = "";
                        cost.IsLadderPrice = "否";
                        db.COST_CostTable.Add(cost);
                        db.SaveChanges();
                        #endregion


                        //社保明细表


                        foreach (var li in list)
                        {
                            //养老

                            if (li.CompanyCosYL != null || li.PersonCostYL != null)
                            {
                                COST_CostTableInsurance yanglao = new COST_CostTableInsurance();
                                yanglao.BranchID = li.BranchID;
                                yanglao.CertificateNumber = li.CardId;
                                yanglao.CertificateType = "";
                                yanglao.CityId = li.CityId;
                                yanglao.CompanyCost = li.CompanyCosYL;
                                yanglao.CompanyRadix = li.RadixYL;
                                // yanglao.CompanyRatio = null;
                                yanglao.COST_CostTable_ID = cost.ID;
                                yanglao.CostType = (int)Common.CostType.养老;
                                yanglao.CreateTime = DateTime.Now;
                                yanglao.CreateUserID = UserID;
                                yanglao.CreateUserName = UserName;
                                yanglao.CRM_Company_ID = li.CompanyId ?? 0;
                                yanglao.Employee_ID = li.PersonId;
                                yanglao.EmployName = li.PersonName;
                                //yanglao.ParentID = null;
                                yanglao.PaymentInterval = li.PaymentSocialMonthYL;
                                yanglao.PaymentMonth = 1;
                                yanglao.PaymentStyle = li.PaymentStyle ?? 0;
                                yanglao.PersonCost = li.PersonCostYL;
                                yanglao.PersonRadix = li.RadixYL;
                                yanglao.PersonRatio = 0;
                                yanglao.Status = 0;
                                ChargeCost = ChargeCost + yanglao.CompanyCost + yanglao.PersonCost;
                                db.COST_CostTableInsurance.Add(yanglao);

                            }
                            //医疗

                            if (li.CompanyCostYil != null || li.PersonCostYil != null)
                            {
                                COST_CostTableInsurance yiliao = new COST_CostTableInsurance();
                                yiliao.BranchID = li.BranchID;
                                yiliao.CertificateNumber = li.CardId;
                                yiliao.CertificateType = "";
                                yiliao.CityId = li.CityId;

                                yiliao.CompanyCost = li.CompanyCostYil;


                                yiliao.CompanyRadix = li.RadixYil;
                                // yiliao.CompanyRatio = null;
                                yiliao.COST_CostTable_ID = cost.ID;

                                yiliao.CostType = (int)Common.CostType.医疗; //*
                                yiliao.CreateTime = DateTime.Now;
                                yiliao.CreateUserID = UserID;
                                yiliao.CreateUserName = UserName;
                                yiliao.CRM_Company_ID = li.CompanyId ?? 0; ;
                                yiliao.Employee_ID = li.PersonId;
                                yiliao.EmployName = li.PersonName;
                                //yanglao.ParentID = null;

                                yiliao.PaymentInterval = li.PaymentSocialMonthYiL;
                                yiliao.PaymentMonth = 1;
                                yiliao.PaymentStyle = li.PaymentStyle ?? 0;

                                yiliao.PersonCost = li.PersonCostYil;
                                yiliao.PersonRadix = li.RadixYil;
                                yiliao.PersonRatio = 0;
                                yiliao.Status = 0;
                                ChargeCost = ChargeCost + yiliao.CompanyCost + yiliao.PersonCost;
                                db.COST_CostTableInsurance.Add(yiliao);
                            }

                            //失业

                            if (li.CompanyCostSY != null || li.PersonCostSY != null)
                            {
                                COST_CostTableInsurance m = new COST_CostTableInsurance();
                                m.BranchID = li.BranchID;
                                m.CertificateNumber = li.CardId;
                                m.CertificateType = "";
                                m.CityId = li.CityId;

                                m.CompanyCost = li.CompanyCostSY;


                                m.CompanyRadix = li.RadixSY;
                                // yiliao.CompanyRatio = null;
                                m.COST_CostTable_ID = cost.ID;

                                m.CostType = (int)Common.CostType.失业; //*
                                m.CreateTime = DateTime.Now;
                                m.CreateUserID = UserID;
                                m.CreateUserName = UserName;
                                m.CRM_Company_ID = li.CompanyId ?? 0; ;
                                m.Employee_ID = li.PersonId;
                                m.EmployName = li.PersonName;
                                //yanglao.ParentID = null;

                                m.PaymentInterval = li.PaymentSocialMonthSY;
                                m.PaymentMonth = 1;
                                m.PaymentStyle = li.PaymentStyle ?? 0;

                                m.PersonCost = li.PersonCostSY;
                                m.PersonRadix = li.RadixSY;
                                m.PersonRatio = 0;
                                m.Status = 0;
                                ChargeCost = ChargeCost + m.CompanyCost + m.PersonCost;
                                db.COST_CostTableInsurance.Add(m);
                            }
                            //工伤

                            if (li.CompanyCostGS != null)
                            {
                                COST_CostTableInsurance m = new COST_CostTableInsurance();
                                m.BranchID = li.BranchID;
                                m.CertificateNumber = li.CardId;
                                m.CertificateType = "";
                                m.CityId = li.CityId;

                                m.CompanyCost = li.CompanyCostGS;


                                m.CompanyRadix = li.RadixGS;
                                // yiliao.CompanyRatio = null;
                                m.COST_CostTable_ID = cost.ID;

                                m.CostType = (int)Common.CostType.工伤; //*
                                m.CreateTime = DateTime.Now;
                                m.CreateUserID = UserID;
                                m.CreateUserName = UserName;
                                m.CRM_Company_ID = li.CompanyId ?? 0; ;
                                m.Employee_ID = li.PersonId;
                                m.EmployName = li.PersonName;
                                //yanglao.ParentID = null;

                                m.PaymentInterval = li.PaymentSocialMonthGS;
                                m.PaymentMonth = 1;
                                m.PaymentStyle = li.PaymentStyle ?? 0;

                                m.PersonCost = 0;//工伤没有个人部分
                                m.PersonRadix = li.RadixGS;
                                m.PersonRatio = 0;
                                m.Status = 0;
                                ChargeCost = ChargeCost + m.CompanyCost + m.PersonCost;
                                db.COST_CostTableInsurance.Add(m);
                            }

                            //公积金

                            if (li.CompanyCostGJJ != null || li.PersonCostGJJ != null)
                            {
                                COST_CostTableInsurance m = new COST_CostTableInsurance();
                                m.BranchID = li.BranchID;
                                m.CertificateNumber = li.CardId;
                                m.CertificateType = "";
                                m.CityId = li.CityId;

                                m.CompanyCost = li.CompanyCostGJJ;


                                m.CompanyRadix = li.RadixGS;
                                // yiliao.CompanyRatio = null;
                                m.COST_CostTable_ID = cost.ID;

                                m.CostType = (int)Common.CostType.公积金; //*
                                m.CreateTime = DateTime.Now;
                                m.CreateUserID = UserID;
                                m.CreateUserName = UserName;
                                m.CRM_Company_ID = li.CompanyId ?? 0; ;
                                m.Employee_ID = li.PersonId;
                                m.EmployName = li.PersonName;
                                //yanglao.ParentID = null;

                                m.PaymentInterval = li.PaymentSocialMonthGJJ;
                                m.PaymentMonth = 1;
                                m.PaymentStyle = li.PaymentStyle ?? 0;

                                m.PersonCost = li.PersonCostGJJ;
                                m.PersonRadix = li.RadixGJJ;
                                m.PersonRatio = 0;
                                m.Status = 0;
                                ChargeCost = ChargeCost + m.CompanyCost + m.PersonCost;
                                db.COST_CostTableInsurance.Add(m);
                            }
                            //大额

                            if (li.CompanyCostYilMax != null || li.PersonCostYilMax != null)
                            {
                                COST_CostTableInsurance m = new COST_CostTableInsurance();
                                m.BranchID = li.BranchID;
                                m.CertificateNumber = li.CardId;
                                m.CertificateType = "";
                                m.CityId = li.CityId;

                                m.CompanyCost = li.CompanyCostYilMax;


                                m.CompanyRadix = li.RadixYil;
                                // yiliao.CompanyRatio = null;
                                m.COST_CostTable_ID = cost.ID;

                                m.CostType = (int)Common.CostType.大病; //*
                                m.CreateTime = DateTime.Now;
                                m.CreateUserID = UserID;
                                m.CreateUserName = UserName;
                                m.CRM_Company_ID = li.CompanyId ?? 0; ;
                                m.Employee_ID = li.PersonId;
                                m.EmployName = li.PersonName;
                                //yanglao.ParentID = null;

                                m.PaymentInterval = li.PaymentSocialMonthYiL;
                                m.PaymentMonth = 1;
                                m.PaymentStyle = li.PaymentStyle ?? 0;

                                m.PersonCost = li.PersonCostYilMax;
                                m.PersonRadix = li.RadixYil;
                                m.PersonRatio = 0;
                                m.Status = 0;
                                ChargeCost = ChargeCost + m.CompanyCost + m.PersonCost;
                                db.COST_CostTableInsurance.Add(m);
                            }
                            //生育

                            if (li.CompanyCostShengY != null || li.CompanyCostShengY != null)
                            {
                                COST_CostTableInsurance m = new COST_CostTableInsurance();
                                m.BranchID = li.BranchID;
                                m.CertificateNumber = li.CardId;
                                m.CertificateType = "";
                                m.CityId = li.CityId;

                                m.CompanyCost = li.CompanyCostShengY;


                                m.CompanyRadix = li.RadixSY;
                                // yiliao.CompanyRatio = null;
                                m.COST_CostTable_ID = cost.ID;

                                m.CostType = (int)Common.CostType.生育; //*
                                m.CreateTime = DateTime.Now;
                                m.CreateUserID = UserID;
                                m.CreateUserName = UserName;
                                m.CRM_Company_ID = li.CompanyId ?? 0; ;
                                m.Employee_ID = li.PersonId;
                                m.EmployName = li.PersonName;
                                //yanglao.ParentID = null;

                                m.PaymentInterval = li.PaymentSocialMonthSY;
                                m.PaymentMonth = 1;
                                m.PaymentStyle = li.PaymentStyle ?? 0;

                                m.PersonCost = 0;
                                m.PersonRadix = li.RadixSY;
                                m.PersonRatio = 0;
                                m.Status = 0;
                                ChargeCost = ChargeCost + m.CompanyCost + m.PersonCost;
                                db.COST_CostTableInsurance.Add(m);
                            }

                            #region 服务费
                            if (li.ServiceCost != null)
                            {
                                COST_CostTableService service = new COST_CostTableService();
                                service.BranchID = BranchID;
                                service.CertificateNumber = li.CardId;
                                service.CertificateType = "";
                                service.ChargeCost = li.ServiceCost ?? 0;
                                service.COST_CostTable_ID = cost.ID;
                                service.CreateTime = DateTime.Now;
                                service.CreateUserID = UserID;
                                service.CreateUserName = UserName;
                                service.CRM_Company_ID = li.CompanyId ?? 0; ;
                                service.Employee_ID = li.PersonId;
                                service.EmployName = li.PersonName;
                                service.PaymentStyle = li.PaymentStyle ?? 0;
                                ChargeCost = ChargeCost + service.ChargeCost;
                                db.COST_CostTableService.Add(service);

                            }

                            #endregion

                            #region 其他费用
                            if (li.PayOther != null)
                            {
                                COST_CostTableOther service = new COST_CostTableOther();
                                service.BranchID = BranchID;
                                service.CertificateNumber = li.CardId;
                                service.CertificateType = "";
                                service.ChargeCost = li.PayOther ?? 0;
                                service.COST_CostTable_ID = cost.ID;
                                service.CostType = 1;
                                service.CreateTime = DateTime.Now;
                                service.CreateUserID = UserID;
                                service.CreateUserName = UserName;
                                service.CRM_Company_ID = li.CompanyId ?? 0; ;
                                service.Employee_ID = li.PersonId;
                                service.EmployName = li.PersonName;
                                service.PaymentStyle = li.PaymentStyle ?? 0;
                                ChargeCost = ChargeCost + service.ChargeCost;
                                db.COST_CostTableOther.Add(service);

                            }

                            #endregion

                            #region 其他社保费用
                            if (li.PayOtherSocial != null)
                            {
                                COST_CostTableOther service = new COST_CostTableOther();
                                service.BranchID = BranchID;
                                service.CertificateNumber = li.CardId;
                                service.CertificateType = "";
                                service.ChargeCost = li.PayOtherSocial ?? 0;
                                service.COST_CostTable_ID = cost.ID;
                                service.CostType = 2;
                                service.CreateTime = DateTime.Now;
                                service.CreateUserID = UserID;
                                service.CreateUserName = UserName;
                                service.CRM_Company_ID = li.CompanyId ?? 0; ;
                                service.Employee_ID = li.PersonId;
                                service.EmployName = li.PersonName;
                                service.PaymentStyle = li.PaymentStyle ?? 0;
                                ChargeCost = ChargeCost + service.ChargeCost;
                                db.COST_CostTableOther.Add(service);

                            }

                            #endregion
                            #region 工本费
                            if (li.PayFee != null)
                            {
                                COST_CostTableOther service = new COST_CostTableOther();
                                service.BranchID = BranchID;
                                service.CertificateNumber = li.CardId;
                                service.CertificateType = "";
                                service.ChargeCost = li.PayFee ?? 0;
                                service.COST_CostTable_ID = cost.ID;
                                service.CostType = 3;
                                service.CreateTime = DateTime.Now;
                                service.CreateUserID = UserID;
                                service.CreateUserName = UserName;
                                service.CRM_Company_ID = li.CompanyId ?? 0; ;
                                service.Employee_ID = li.PersonId;
                                service.EmployName = li.PersonName;
                                service.PaymentStyle = li.PaymentStyle ?? 0;
                                ChargeCost = ChargeCost + service.ChargeCost;
                                db.COST_CostTableOther.Add(service);

                            }

                            #endregion
                        }
                        //更改金额
                        cost.ChargeCost = ChargeCost ?? 0;
                        db.SaveChanges();
                        tran.Complete();


                    }

                    return "";
                }
            }
            catch (DbEntityValidationException ex)
            {
                string s = "";
                foreach (var errors in ex.EntityValidationErrors)
                {
                    foreach (var item in errors.ValidationErrors)
                    {
                        s = s + item.ErrorMessage + item.PropertyName + ",";
                    }
                }
                return "";
            }


        }
        #region 导入验证
        private string verification(SysEntities db, DataTable dt, int yearMonth, out List<COST_Pay_Suppler> list, int Supper_ID, int UserID, int BranchID, string UserName)
        {
            List<COST_Pay_Suppler> cpt_List = new List<COST_Pay_Suppler>();

            StringBuilder ErrorList = new StringBuilder();

            try
            {
                string cityFirst = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder Error = new StringBuilder();
                    DataRow dr = dt.Rows[i];
                    COST_Pay_Suppler cpt = new COST_Pay_Suppler();

                    string hanghao = "(第" + (i + 1) + "行)";
                    var cardId = dr["身份证号"].ToString().Trim();
                    //var personName = dr["姓名"].ToString().Trim();
                    //var cityName = dr["保险缴纳地"].ToString().Trim();
                    var PaymentStyle = dr["缴费类型"].ToString().Trim();
                    var BranchName = dr["分公司"].ToString().Trim();
                    var PaymentSocialMonthYL = dr["缴费区间"].ToString().Trim();
                    var RadixYL = dr["养老保险缴费基数"].ToString().Trim();
                    var CompanyCosYL = dr["养老保险单位"].ToString().Trim();
                    var PersonCostYL = dr["养老保险个人"].ToString().Trim();
                    var PaymentMonthYL = dr["养老缴纳月数"].ToString().Trim();

                    var PaymentSocialMonthSY = dr["失业缴费区间"].ToString().Trim();
                    var PaymentMonthSY = dr["失业缴纳月数"].ToString().Trim();
                    var RadixSY = dr["失业缴费基数"].ToString().Trim();
                    var CompanyCostSY = dr["失业单位"].ToString().Trim();
                    var PersonCostSY = dr["失业个人"].ToString().Trim();

                    var PaymentSocialMonthGS = dr["工伤缴费区间"].ToString().Trim();
                    var PaymentMonthGS = dr["工伤缴纳月数"].ToString().Trim();
                    var RadixGS = dr["工伤缴费基数"].ToString().Trim();
                    var CompanyCostGS = dr["工伤单位"].ToString().Trim();

                    var PaymentSocialMonthYiL = dr["医疗缴费区间"].ToString().Trim();
                    var PaymentMonthYiL = dr["医疗缴纳月数"].ToString().Trim();
                    var RadixYil = dr["医疗缴费基数"].ToString().Trim();
                    var CompanyCostYil = dr["医疗单位"].ToString().Trim();
                    var PersonCostYil = dr["医疗个人"].ToString().Trim();
                    var CompanyCostYilMax = dr["医疗大额医疗单位"].ToString().Trim();
                    var PersonCostYilMax = dr["医疗大额医疗个人"].ToString().Trim();
                    var CompanyCostShengY = dr["医疗生育保险单位"].ToString().Trim();

                    var PaymentSocialMonthGJJ = dr["公积金缴费区间"].ToString().Trim();
                    var PaymentMonthGJJ = dr["公积金缴纳月数"].ToString().Trim();
                    var RadixGJJ = dr["公积金缴费基数"].ToString().Trim();
                    var CompanyCostGJJ = dr["公积金单位"].ToString().Trim();
                    var PersonCostGJJ = dr["公积金个人"].ToString().Trim();

                    var PaymentSocialMonthBCGJJ = "";
                    var RadixBCGJJ = "";
                    var CompanyCostBCGJJ = "";
                    var PersonCostBCGJJ = "";

                    //var PayOtherSocial = dr["其他费用"].ToString().Trim();
                    var PayOther = dr["其他费用"].ToString().Trim();
                    var PayFee = dr["工本费"].ToString().Trim();
                    var serviceCost = dr["服务费"].ToString().Trim();
                    var yanglaoPayFee = "";
                    var yiliaoPayFee = "";
                    var gongshangPayFee = "";
                    var shiyePayFee = "";
                    var gongjijinPayFee = "";
                    var shengyuPayFee = "";

                    cpt.PaymentStyle = (int)(Common.EmployeeMiddle_PaymentStyle)Enum.Parse(typeof(Common.EmployeeMiddle_PaymentStyle), PaymentStyle);



                    #region 判断身份证号是否合法
                    //判断身份证号是否合法
                    if (!Common.CardCommon.CheckCardID(cardId))
                    {
                        Error.Append("身份证号不合法！\r\n");
                    }
                    else
                    {
                        //判断身份证号是否存在


                        var trueEmployee = from a in db.EmployeeAdd.Where(x => x.SuppliersId == Supper_ID)
                                           join b in db.Employee on a.CompanyEmployeeRelation.EmployeeId equals b.Id
                                           join c in db.CRM_Company on a.CompanyEmployeeRelation.CompanyId equals c.ID
                                           join d in db.City on a.CompanyEmployeeRelation.CityId equals d.Id
                                           join e in db.CRM_CompanyToBranch on a.CompanyEmployeeRelation.CompanyId equals e.CRM_Company_ID
                                           where b.CertificateNumber == cardId
                                           select new
                                           {
                                               c.CompanyName,
                                               e.BranchID,
                                               a.CompanyEmployeeRelation.CityId,
                                               a.CompanyEmployeeRelation.CompanyId,
                                               EmployeeAddid = a.Id,
                                               b.Id,
                                               b.Name
                                           };
                        if (trueEmployee.Count() > 0)
                        {//找出对应的员工编号和员工姓名
                            var oneTrueEmployee = trueEmployee.OrderByDescending(x => x.EmployeeAddid).FirstOrDefault();
                            cpt.PersonId = oneTrueEmployee.Id;
                            cpt.CardId = cardId;
                            cpt.PersonName = oneTrueEmployee.Name;
                            cpt.CompanyId = oneTrueEmployee.CompanyId;
                            cpt.BranchID = oneTrueEmployee.BranchID;
                            cpt.CityId = oneTrueEmployee.CityId;





                        }
                        else
                        {
                            Error.Append("报增记录不存在此人！\r\n");
                        }
                    }
                    #endregion



                    #region 判断养老基数，和金额
                    //如果单位承担或个人承担不为空则认为缴纳此险种
                    if (!string.IsNullOrEmpty(CompanyCosYL) || !string.IsNullOrEmpty(PersonCostYL))
                    {
                        cpt.PaymentSocialMonthYL = PaymentSocialMonthYL;

                        if (!string.IsNullOrEmpty(RadixYL))
                        {
                            if (Common.Business.Is_Decimal(RadixYL))
                            {
                                cpt.RadixYL = Convert.ToDecimal(RadixYL);
                            }
                            else
                            {
                                Error.Append("养老基数格式不正确，请修改后再导入！\r\n");
                            }
                        }
                        else { cpt.RadixYL = (decimal)0; }
                        if (Common.Business.Is_Decimal(CompanyCosYL))
                        {
                            cpt.CompanyCosYL = Convert.ToDecimal(CompanyCosYL);
                        }
                        else { Error.Append("养老单位承担金额格式不正确！\r\n"); }
                        if (Common.Business.Is_Decimal(PersonCostYL))
                        {
                            cpt.PersonCostYL = Convert.ToDecimal(PersonCostYL);
                        }
                        else { Error.Append("养老个人承担金额格式不正确！\r\n"); }
                    }
                    #endregion
                    cpt.SuppliersId = Supper_ID;


                    #region 判断医疗，生育，大病
                    // 判断医疗
                    if (!string.IsNullOrEmpty(CompanyCostYil) || !string.IsNullOrEmpty(PersonCostYil))
                    {
                        cpt.PaymentSocialMonthYiL = PaymentSocialMonthYiL;

                        //基数
                        if (!string.IsNullOrEmpty(RadixYil))
                        {
                            if (Common.Business.Is_Decimal(RadixYil))
                            {
                                cpt.RadixYil = Convert.ToDecimal(RadixYil);
                            }
                            else { Error.Append("医疗基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixYil = (decimal)0; }
                        //医疗单位
                        if (Common.Business.Is_Decimal(CompanyCostYil))
                        {
                            cpt.CompanyCostYil = Convert.ToDecimal(CompanyCostYil);
                        }
                        else { Error.Append("医疗单位承担金额格式不正确！\r\n"); }
                        //医疗个人
                        if (Common.Business.Is_Decimal(PersonCostYil))
                        {
                            cpt.PersonCostYil = Convert.ToDecimal(PersonCostYil);
                        }
                        else { Error.Append("医疗个人承担金额不正确！\r\n"); }
                    }
                    // 判断生育
                    if (!string.IsNullOrEmpty(CompanyCostShengY))
                    {
                        //生育单位
                        if (!string.IsNullOrEmpty(CompanyCostShengY))
                        {
                            if (Common.Business.Is_Decimal(CompanyCostShengY))
                            {
                                cpt.CompanyCostShengY = Convert.ToDecimal(CompanyCostShengY);
                            }
                            else { Error.Append("生育金额格式不正确！\r\n"); }
                        }
                        else { cpt.CompanyCostShengY = (decimal)0; }
                    }
                    // 判断大病
                    if (!string.IsNullOrEmpty(CompanyCostYilMax) || !string.IsNullOrEmpty(PersonCostYilMax))
                    {
                        //大病单位
                        if (!string.IsNullOrEmpty(CompanyCostYilMax))
                        {
                            if (Common.Business.Is_Decimal(CompanyCostYilMax))
                            {
                                cpt.CompanyCostYilMax = Convert.ToDecimal(CompanyCostYilMax);
                            }
                            else { Error.Append("大病单位金额格式不正确！\r\n"); }
                        }
                        else { cpt.CompanyCostYilMax = (decimal)0; }
                        //大病个人
                        if (!string.IsNullOrEmpty(PersonCostYilMax))
                        {
                            if (Common.Business.Is_Decimal(PersonCostYilMax))
                            {
                                cpt.PersonCostYilMax = Convert.ToDecimal(PersonCostYilMax);
                            }
                            else { Error.Append("大病个人金额格式不正确！\r\n"); }
                        }
                        else { cpt.PersonCostYilMax = (decimal)0; }
                    }
                    #endregion

                    #region 判断工伤基数，金额
                    if (!string.IsNullOrEmpty(CompanyCostGS))
                    {
                        cpt.PaymentSocialMonthGS = PaymentSocialMonthGS;
                        //判断基数
                        if (!string.IsNullOrEmpty(RadixGS))
                        {
                            if (Common.Business.Is_Decimal(RadixGS))
                            {
                                cpt.RadixGS = Convert.ToDecimal(RadixGS);
                            }
                            else { Error.Append("工伤基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixGS = (decimal)0; }
                        //判断金额
                        if (Common.Business.Is_Decimal(CompanyCostGS))
                        {
                            cpt.CompanyCostGS = Convert.ToDecimal(CompanyCostGS);
                        }
                        else { Error.Append("工伤单位金额格式不正确！\r\n"); }
                    }
                    #endregion

                    #region 判断失业基数、金额
                    if (!string.IsNullOrEmpty(CompanyCostSY) || !string.IsNullOrEmpty(PersonCostSY))
                    {
                        cpt.PaymentSocialMonthSY = PaymentSocialMonthSY;
                        //判断基数
                        if (!string.IsNullOrEmpty(RadixSY))
                        {
                            if (Common.Business.Is_Decimal(RadixSY))
                            {
                                cpt.RadixSY = Convert.ToDecimal(RadixSY);
                            }
                            else { Error.Append("失业基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixSY = (decimal)0; }
                        //判断单位金额
                        if (Common.Business.Is_Decimal(CompanyCostSY))
                        {
                            cpt.CompanyCostSY = Convert.ToDecimal(CompanyCostSY);
                        }
                        else { Error.Append("失业单位金额格式不正确！\r\n"); }
                        //判断个人金额
                        if (Common.Business.Is_Decimal(PersonCostSY))
                        {
                            cpt.PersonCostSY = Convert.ToDecimal(PersonCostSY);
                        }
                        else { Error.Append("失业个人金额格式不正确！\r\n"); }
                    }
                    #endregion

                    #region 判断公积金基数、金额，补充公积金基数，金额
                    //公积金基数
                    if (!string.IsNullOrEmpty(CompanyCostGJJ) || !string.IsNullOrEmpty(PersonCostGJJ))
                    {
                        cpt.PaymentSocialMonthGJJ = PaymentSocialMonthGJJ;

                        if (!string.IsNullOrEmpty(RadixGJJ))
                        {
                            if (Common.Business.Is_Decimal(RadixGJJ))
                            {
                                cpt.RadixGJJ = Convert.ToDecimal(RadixGJJ);
                            }
                            else { Error.Append("公积金基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixGJJ = (decimal)0; }

                        //公积金单位承担
                        if (Common.Business.Is_Decimal(CompanyCostGJJ))
                        {
                            cpt.CompanyCostGJJ = Convert.ToDecimal(CompanyCostGJJ);
                        }
                        else { Error.Append("公积金单位承担金客格式不正确！\r\n"); }
                        //公积金个人承担
                        if (Common.Business.Is_Decimal(PersonCostGJJ))
                        {
                            cpt.PersonCostGJJ = Convert.ToDecimal(PersonCostGJJ);
                        }
                        else { Error.Append("公积金个人承担金额格式不正确！\r\n"); }
                    }
                    //判断补充公积金
                    if (!string.IsNullOrEmpty(CompanyCostBCGJJ) || !string.IsNullOrEmpty(PersonCostBCGJJ))
                    {
                        //补充公积金基数
                        if (!string.IsNullOrEmpty(RadixBCGJJ))
                        {
                            if (Common.Business.Is_Decimal(RadixBCGJJ))
                            {
                                cpt.RadixBCGJJ = Convert.ToDecimal(RadixBCGJJ);
                            }
                            else { Error.Append("补充公积金基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixBCGJJ = (decimal)0; }
                        //补充公积金单位承担
                        if (Common.Business.Is_Decimal(CompanyCostBCGJJ))
                        {
                            cpt.CompanyCostBCGJJ = Convert.ToDecimal(CompanyCostBCGJJ);
                        }
                        else { Error.Append("补充公积金单位承担金额格式不正确！\r\n"); }

                        //补充公积金个人承担
                        if (Common.Business.Is_Decimal(PersonCostBCGJJ))
                        {
                            cpt.PersonCostBCGJJ = Convert.ToDecimal(PersonCostBCGJJ);
                        }
                        else { Error.Append("补充公积金个人承担金额格式不正确！\r\n"); }
                    }

                    #endregion

                    #region 判断其他社保费、其他费用、工本费
                    ////其他社保费
                    //if (!string.IsNullOrEmpty(PayOtherSocial))
                    //{
                    //    if (Common.Business.Is_Decimal(PayOtherSocial))
                    //    {
                    //        cpt.PayOtherSocial = Convert.ToDecimal(PayOtherSocial);
                    //    }
                    //    else { Error.Append("其他社保费格式不正确！\r\n"); }
                    //}
                    //else { cpt.PayOtherSocial = (decimal)0; }
                    ////其他费用
                    //if (!string.IsNullOrEmpty(PayOther))
                    //{
                    //    if (Common.Business.Is_Decimal(PayOther))
                    //    {
                    //        cpt.PayOther = Convert.ToDecimal(PayOther);
                    //    }
                    //    else { Error.Append("其他费用格式不正确！\r\n"); }
                    //}
                    //else { cpt.PayOther = (decimal)0; }

                    //养老工本费
                    if (!string.IsNullOrEmpty(yanglaoPayFee))
                    {
                        if (Common.Business.Is_Decimal(yanglaoPayFee))
                        {
                            cpt.YanglaoPayFee = Convert.ToDecimal(yanglaoPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.YanglaoPayFee = (decimal)0; }
                    //医疗工本费
                    if (!string.IsNullOrEmpty(yiliaoPayFee))
                    {
                        if (Common.Business.Is_Decimal(yiliaoPayFee))
                        {
                            cpt.YiliaoPayFee = Convert.ToDecimal(yiliaoPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.YiliaoPayFee = (decimal)0; }
                    //工伤工本费
                    if (!string.IsNullOrEmpty(gongshangPayFee))
                    {
                        if (Common.Business.Is_Decimal(gongshangPayFee))
                        {
                            cpt.GongshangPayFee = Convert.ToDecimal(gongshangPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.GongshangPayFee = (decimal)0; }
                    //公积金工本费
                    if (!string.IsNullOrEmpty(gongjijinPayFee))
                    {
                        if (Common.Business.Is_Decimal(gongjijinPayFee))
                        {
                            cpt.GongjijinPayFee = Convert.ToDecimal(gongjijinPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.GongjijinPayFee = (decimal)0; }
                    //失业工本费
                    if (!string.IsNullOrEmpty(shiyePayFee))
                    {
                        if (Common.Business.Is_Decimal(shiyePayFee))
                        {
                            cpt.ShiyePayFee = Convert.ToDecimal(shiyePayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.ShiyePayFee = (decimal)0; }
                    //生育工本费
                    if (!string.IsNullOrEmpty(shengyuPayFee))
                    {
                        if (Common.Business.Is_Decimal(shengyuPayFee))
                        {
                            cpt.ShengYuPayFee = Convert.ToDecimal(shengyuPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.ShengYuPayFee = (decimal)0; }


                    //工本费
                    if (!string.IsNullOrEmpty(PayFee))
                    {
                        if (Common.Business.Is_Decimal(PayFee))
                        {
                            cpt.PayFee = Convert.ToDecimal(PayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.PayFee = (decimal)0; }
                    //服务费
                    if (!string.IsNullOrEmpty(serviceCost))
                    {
                        if (Common.Business.Is_Decimal(serviceCost))
                        {
                            cpt.ServiceCost = Convert.ToDecimal(serviceCost);
                        }
                        else { Error.Append("服务费格式不正确！\r\n"); }
                    }
                    else { cpt.PayFee = (decimal)0; }
                    #endregion

                    #region 判断有没有错误，没有的话加入创建信息
                    if (Error.ToString() != "")
                    {
                        Error.Insert(0, hanghao);
                        ErrorList.Append(Error);
                    }
                    else
                    {
                        cpt.YearMonth = yearMonth;
                        cpt.CreateTime = DateTime.Now;
                        cpt.CreateUserID = UserID;
                        cpt.CreateUserName = UserName;
                        cpt.BranchID = BranchID;
                        //cpt.CardIdSB = cardIdSB;
                        cpt_List.Add(cpt);
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                ErrorList.Append(ex.ToString());
            }
            list = cpt_List;
            return ErrorList.ToString();
        }
        #endregion

        #endregion
      
    }
}

