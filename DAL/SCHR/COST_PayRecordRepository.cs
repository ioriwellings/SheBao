using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Langben.DAL.Model;

namespace Langben.DAL
{
    public partial class COST_PayRecordRepository
    {
        public List<CostPayDuibi> GetCostPay(SysEntities db, int yearMonthSrart, int yearMonthEnd, int[] companyId, int costType)
        {
            IQueryable<CostPayDuibi> payList = null;
            IQueryable<CostPayDuibi> costList = null;
            #region 获取支出数据结果

            // 获取支出数据结果
            switch (costType)
            {
                case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                    // 费用_社保支出养老
                    // 获取支出数据结果
                    payList = from yanglao in db.COST_PayYangLao
                              join recordStatus in db.COST_PayRecordStatus on yanglao.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on yanglao.CompanyId equals company.ID
                              into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                              && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(yanglao.CompanyId)   //&& record.SuppliersId == suppliersId //供应商先不考虑
                              group new { yanglao, l } by new
                              {
                                  companyId = yanglao.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                              
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.yanglao.CompanyCost),
                                      PersonPay = s.Sum(p => p.yanglao.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                    // 费用_社保支出医疗
                    payList = from yiliao in db.COST_PayYiLiao
                              join recordStatus in db.COST_PayRecordStatus on yiliao.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on yiliao.CompanyId equals company.ID
                               into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                              && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(yiliao.CompanyId)  //&& record.SuppliersId == suppliersId  // 供应商先不考虑
                              group new { yiliao, l } by new
                              {
                                  companyId = yiliao.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                                 
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.yiliao.CompanyCost),
                                      PersonPay = s.Sum(p => p.yiliao.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                    // 费用_社保支出失业
                    payList = from shiye in db.COST_PayShiYe
                              join recordStatus in db.COST_PayRecordStatus on shiye.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on shiye.CompanyId equals company.ID into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(shiye.CompanyId) // && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { shiye, l } by new
                              {
                                  companyId = shiye.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                                
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.shiye.CompanyCost),
                                      PersonPay = s.Sum(p => p.shiye.PersonCost)
                                  };
                    var qqq = payList.ToList();
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                    // 费用_社保支出工伤
                    payList = from gongshang in db.COST_PayGongShang
                              join recordStatus in db.COST_PayRecordStatus on gongshang.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on gongshang.CompanyId equals company.ID
                               into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(gongshang.CompanyId)// && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { gongshang, l } by new
                              {
                                  companyId = gongshang.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.gongshang.CompanyCost),
                                      PersonPay = s.Sum(p => p.gongshang.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                    // 费用_社保支出工伤
                    payList = from shengyu in db.COST_PayShengYu
                              join recordStatus in db.COST_PayRecordStatus on shengyu.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on shengyu.CompanyId equals company.ID
                               into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(shengyu.CompanyId)// && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { shengyu, l } by new
                              {
                                  companyId = shengyu.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                                
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.shengyu.CompanyCost),
                                      PersonPay = s.Sum(p => p.shengyu.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                    // 费用_社保支出公积金
                    payList = from gongjijin in db.COST_PayGongJiJin
                              join recordStatus in db.COST_PayRecordStatus on gongjijin.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on gongjijin.CompanyId equals company.ID
                               into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(gongjijin.CompanyId)// && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { gongjijin, l } by new
                              {
                                  companyId = gongjijin.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                               
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.gongjijin.CompanyCost),
                                      PersonPay = s.Sum(p => p.gongjijin.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                    // 费用_社保支出医疗大额
                    payList = from dae in db.COST_PayYiLiaoDaE
                              join recordStatus in db.COST_PayRecordStatus on dae.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on dae.CompanyId equals company.ID
                               into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                                && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(dae.CompanyId)// && record.SuppliersId == suppliersId  //供应商先不考虑
                              group new { dae, l } by new
                              {
                                  companyId = dae.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                                
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.dae.CompanyCost),
                                      PersonPay = s.Sum(p => p.dae.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.补充公积金:
                    // 费用_社保支出补充公积金
                    payList = from buchong in db.COST_PayGongJiJinBC
                              join recordStatus in db.COST_PayRecordStatus on buchong.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on buchong.CompanyId equals company.ID
                               into gg
                              from l in gg.DefaultIfEmpty()
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId.Contains(buchong.CompanyId)// && record.SuppliersId == suppliersId // 供应商先不考虑
                              group new { buchong, l } by new
                              {
                                  companyId = buchong.CompanyId,
                                  companyname = l.CompanyName
                              }
                                  into s
                                  select new CostPayDuibi
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.buchong.CompanyCost),
                                      PersonPay = s.Sum(p => p.buchong.PersonCost)
                                  };
                    break;
            }

            #endregion

            #region 获取收入数据结果

            int[] status = { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已核销, (int)Common.COST_Table_Status.已支付 };
            // 获取收入数据结果
            costList = from insurance in db.COST_CostTableInsurance
                       join cost in db.COST_CostTable on insurance.COST_CostTable_ID equals cost.ID
                       join company in db.CRM_Company on insurance.CRM_Company_ID equals company.ID
                       where status.Contains(cost.Status) && insurance.CostType == costType
                        && cost.YearMonth >= yearMonthSrart && cost.YearMonth <= yearMonthEnd && companyId.Contains(cost.CRM_Company_ID)
                       group new { insurance, company } by new
                       {
                           companyId = insurance.CRM_Company_ID,
                           companyname = company.CompanyName
                       }
                           into s
                           select new CostPayDuibi
                           {
                               CompanyId = s.Key.companyId,
                               CompanyName = s.Key.companyname,
                               CompanyCost = s.Sum(p => p.insurance.CompanyCost),
                               PersonCost = s.Sum(p => p.insurance.PersonCost),
                               CompanyPay = 0,
                               PersonPay = 0
                           };

            #endregion

            //bool paybool = payList.Any();
            //bool costbool = costList.Any();
            //if ((!paybool) && (!costbool))
            //{
            //    return payList;
            //}
            //else if ((!paybool) && costbool)
            //{
            //    return costList;
            //}
            //else if (paybool && (!costbool))
            //{
            //    return payList;
            //}
            //else
            //{

            List<CostPayDuibi> payList1 = payList.ToList();
            List<CostPayDuibi> costList1 = costList.ToList();
            var costpay = from a in (payList1.Concat(costList1))
                          join b in db.CRM_Company on a.CompanyId equals b.ID
                          group new { a, b } by new { a.CompanyId, b.CompanyName } into s
                          select new CostPayDuibi
                          {
                              CompanyId = s.Key.CompanyId,
                              CompanyName = s.Key.CompanyName,
                              CompanyPay = s.Sum(p => p.a.CompanyPay),
                              PersonPay = s.Sum(p => p.a.PersonPay),
                              CompanyCost = s.Sum(p => p.a.CompanyCost),
                              PersonCost = s.Sum(p => p.a.PersonCost)
                          };
            var q111 = costpay.ToList();
            return costpay.ToList();
            // }
        }

        public List<CostPayDuibiDetails> GetCostPay(SysEntities db, int yearMonthSrart, int yearMonthEnd, int companyId, int costType)
        {
            IQueryable<CostPayDuibiDetails> payList = null;
            IQueryable<CostPayDuibiDetails> costList = null;
            #region 获取支出数据结果

            // 获取支出数据结果
            switch (costType)
            {
                case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                    // 费用_社保支出养老
                    // 获取支出数据结果
                    payList = from yanglao in db.COST_PayYangLao
                              join recordStatus in db.COST_PayRecordStatus on yanglao.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on yanglao.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                              && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == yanglao.CompanyId  //&& record.SuppliersId == suppliersId //供应商先不考虑
                              group new { yanglao, company } by new
                              {
                                  companyId = yanglao.CompanyId,
                                  yearmonth = yanglao.YearMonth,
                                  companyname = company.CompanyName
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.yanglao.CompanyCost),
                                      PersonPay = s.Sum(p => p.yanglao.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                    // 费用_社保支出医疗
                    payList = from yiliao in db.COST_PayYiLiao
                              join recordStatus in db.COST_PayRecordStatus on yiliao.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on yiliao.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                              && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == yiliao.CompanyId //&& record.SuppliersId == suppliersId  // 供应商先不考虑
                              group new { yiliao, company } by new
                              {
                                  companyId = yiliao.CompanyId,
                                  yearmonth = yiliao.YearMonth,
                                  companyname = company.CompanyName
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.yiliao.CompanyCost),
                                      PersonPay = s.Sum(p => p.yiliao.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                    // 费用_社保支出失业
                    payList = from shiye in db.COST_PayShiYe
                              join recordStatus in db.COST_PayRecordStatus on shiye.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on shiye.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == shiye.CompanyId // && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { shiye, company } by new
                              {
                                  companyId = shiye.CompanyId,
                                  yearmonth = shiye.YearMonth,
                                  companyname = company.CompanyName
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.shiye.CompanyCost),
                                      PersonPay = s.Sum(p => p.shiye.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                    // 费用_社保支出工伤
                    payList = from gongshang in db.COST_PayGongShang
                              join recordStatus in db.COST_PayRecordStatus on gongshang.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on gongshang.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == gongshang.CompanyId// && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { gongshang, company } by new
                              {
                                  companyId = gongshang.CompanyId,
                                  yearmonth = gongshang.YearMonth,
                                  companyname = company.CompanyName
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.gongshang.CompanyCost),
                                      PersonPay = s.Sum(p => p.gongshang.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                    // 费用_社保支出工伤
                    payList = from shengyu in db.COST_PayShengYu
                              join recordStatus in db.COST_PayRecordStatus on shengyu.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on shengyu.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == shengyu.CompanyId// && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { shengyu, company } by new
                              {
                                  companyId = shengyu.CompanyId,
                                  yearmonth = shengyu.YearMonth,
                                  companyname = company.CompanyName
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.shengyu.CompanyCost),
                                      PersonPay = s.Sum(p => p.shengyu.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                    // 费用_社保支出公积金
                    payList = from gongjijin in db.COST_PayGongJiJin
                              join recordStatus in db.COST_PayRecordStatus on gongjijin.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on gongjijin.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == gongjijin.CompanyId// && record.SuppliersId == suppliersId //供应商先不考虑
                              group new { gongjijin, company } by new
                              {
                                  companyId = gongjijin.CompanyId,
                                  yearmonth = gongjijin.YearMonth,
                                  companyname = company.CompanyName
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.gongjijin.CompanyCost),
                                      PersonPay = s.Sum(p => p.gongjijin.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                    // 费用_社保支出医疗大额
                    payList = from dae in db.COST_PayYiLiaoDaE
                              join recordStatus in db.COST_PayRecordStatus on dae.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on dae.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                                && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == dae.CompanyId// && record.SuppliersId == suppliersId  //供应商先不考虑
                              group new { dae, company } by new
                              {
                                  companyId = dae.CompanyId,
                                  companyname = company.CompanyName,
                                  yearmonth = dae.YearMonth
                              }
                                  into s
                                  from l in s.DefaultIfEmpty()
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.dae.CompanyCost),
                                      PersonPay = s.Sum(p => p.dae.PersonCost)
                                  };
                    break;
                case (int)Common.EmployeeAdd_InsuranceKindId.补充公积金:
                    // 费用_社保支出补充公积金
                    payList = from buchong in db.COST_PayGongJiJinBC
                              join recordStatus in db.COST_PayRecordStatus on buchong.COST_PayRecordStatusID equals recordStatus.ID
                              join record in db.COST_PayRecord on recordStatus.COST_PayRecordId equals record.ID
                              join company in db.CRM_Company on buchong.CompanyId equals company.ID
                              where recordStatus.Status != (int)Common.COST_PayRecord_Status.未锁定 && recordStatus.CostType == costType
                               && record.YearMonth >= yearMonthSrart && record.YearMonth <= yearMonthEnd && companyId == buchong.CompanyId// && record.SuppliersId == suppliersId // 供应商先不考虑
                              group new { buchong, companyId } by new
                              {
                                  companyId = buchong.CompanyId,
                                  companyname = company.CompanyName,
                                  yearmonth = buchong.YearMonth
                              }
                                  into s
                                  select new CostPayDuibiDetails
                                  {
                                      CompanyId = s.Key.companyId,
                                      CompanyName = s.Key.companyname,
                                      YearMonth = s.Key.yearmonth,
                                      CompanyCost = 0,
                                      PersonCost = 0,
                                      CompanyPay = s.Sum(p => p.buchong.CompanyCost),
                                      PersonPay = s.Sum(p => p.buchong.PersonCost)
                                  };
                    break;
            }

            #endregion

            #region 获取收入数据结果

            int[] status = { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已核销, (int)Common.COST_Table_Status.已支付 };
            // 获取收入数据结果
            costList = from insurance in db.COST_CostTableInsurance
                       join cost in db.COST_CostTable on insurance.COST_CostTable_ID equals cost.ID
                       join company in db.CRM_Company on insurance.CRM_Company_ID equals company.ID
                       where status.Contains(cost.Status) && insurance.CostType == costType
                        && cost.YearMonth >= yearMonthSrart && cost.YearMonth <= yearMonthEnd && companyId == cost.CRM_Company_ID
                       group new { insurance, cost, company } by new
                       {
                           companyId = insurance.CRM_Company_ID,
                           companyname = company.CompanyName,
                           yearmonth = cost.YearMonth
                       }
                           into s
                           select new CostPayDuibiDetails
                           {
                               CompanyId = s.Key.companyId,
                               CompanyName = s.Key.companyname,
                               YearMonth = s.Key.yearmonth,
                               CompanyCost = s.Sum(p => p.insurance.CompanyCost),
                               PersonCost = s.Sum(p => p.insurance.PersonCost),
                               CompanyPay = 0,
                               PersonPay = 0
                           };

            #endregion

            //bool paybool = payList.Any();
            //bool costbool = costList.Any();
            //if ((!paybool) && (!costbool))
            //{
            //    return payList.ToList();
            //}
            //else if ((!paybool) && costbool)
            //{
            //    return costList.ToList();
            //}
            //else if (paybool && (!costbool))
            //{
            //    return payList.ToList();
            //}
            //else
            //{
            List<CostPayDuibiDetails> payList1 = payList.ToList();
            List<CostPayDuibiDetails> costList1 = costList.ToList();
            var costpay = (from a in (payList1.Union(costList1))
                           join b in db.CRM_Company on a.CompanyId equals b.ID
                           group new { a, b } by new { a.CompanyId, b.CompanyName, a.YearMonth } into s
                           select new CostPayDuibiDetails
                           {
                               CompanyId = s.Key.CompanyId,
                               CompanyName = s.Key.CompanyName,
                               YearMonth = s.Key.YearMonth,
                               CompanyPay = s.Sum(p => p.a.CompanyPay),
                               PersonPay = s.Sum(p => p.a.PersonPay),
                               CompanyCost = s.Sum(p => p.a.CompanyCost),
                               PersonCost = s.Sum(p => p.a.PersonCost)
                           }).ToList();
            return costpay.ToList();
            // }
        }
    }
}
