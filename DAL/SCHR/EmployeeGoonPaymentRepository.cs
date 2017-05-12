using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Langben.DAL.Model;
namespace Langben.DAL
{
    public partial class EmployeeGoonPaymentRepository
    {
        #region 查询待责任客服审核列表
        /// <summary>
        /// 查询待责任客服审核列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeApprove> GetApproveList(SysEntities db, string CertificateNumber, int page, int rows, string search, out int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var emp = ent.Employee.Where(a => true);
                    var empGoon = ent.EmployeeGoonPayment.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
                        {
                            string str = queryDic["Name"];
                            emp = emp.Where(a => a.Name.Contains(str));
                        }

                        if (!string.IsNullOrEmpty(CertificateNumber))
                        {
                            string[] cardIdList = CertificateNumber.Split(Convert.ToChar(10));
                            List<string> cardList = new List<string>();
                            for (int i = 0; i < cardIdList.Length; i++)
                            {
                                cardList.Add(cardIdList[i]);
                                cardList.Add(CardCommon.CardIDTo15(cardIdList[i]));
                                cardList.Add(CardCommon.CardIDTo18(cardIdList[i]));
                            }
                            cardList = cardList.Distinct().ToList();
                            emp = emp.Where(o => cardList.Contains(o.CertificateNumber));
                        }

                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            empGoon = empGoon.Where(a => states.Contains(a.State));
                        }
                        if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        {
                            string str = queryDic["InsuranceKinds"];
                            int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                            empGoon = empGoon.Where(a => Ids.Contains(a.EmployeeAdd.InsuranceKindId));
                        }

                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            int str = Convert.ToInt32(queryDic["YearMonth"]);
                            empGoon = empGoon.Where(a => a.YearMonth == str);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                    }
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empGoon on r.Id equals b.EmployeeAdd.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on b.EmployeeAdd.PoliceAccountNatureId equals f.Id
                                select new EmployeeApprove()
                                {
                                    CompanyEmployeeRelationId = b.EmployeeAdd.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    CompanyName = c.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = e.Name,
                                    CityID = r.CityId,
                                    PoliceAccountNature = f.Name,
                                    YearMonth = b.YearMonth
                                }).Distinct();
                    count = 0;
                    if (list != null && list.Count() >= 1)
                    {
                        count = list.Count();
                        if (page > -1)
                        {
                            list = list.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                        }
                    }

                    List<EmployeeApprove> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的补缴险种信息全部查询并放入系统内存中;
                    var temp = (from a in empGoon
                                join b in list on a.EmployeeAdd.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                                //join d in ent.InsuranceKind on a.InsuranceKindId equals d.Id
                                where a.YearMonth == b.YearMonth
                                select new
                                {
                                    a.EmployeeAdd.CompanyEmployeeRelationId,
                                    a.EmployeeAdd.InsuranceKindId,
                                    a.YearMonth,
                                    a.Id
                                }).ToList();
                    for (int i = 0; i < appList.Count(); i++)
                    {
                        int? Id = appList[i].CompanyEmployeeRelationId;
                        int? YearMonth = appList[i].YearMonth;
                        if (Id != null)
                        {
                            var kinds = temp.Where(a => a.CompanyEmployeeRelationId == Id && a.YearMonth == YearMonth).ToList();
                            //var kinds = temp.Where(a => a.CompanyEmployeeRelationId == Id).ToList();
                            string kindsName = "";
                            string addIds = "";
                            foreach (var a in kinds)
                            {
                                kindsName += ((Common.EmployeeAdd_InsuranceKindId)a.InsuranceKindId + ",");
                                addIds += (a.Id + ",");
                            }
                            kindsName = kindsName.Substring(0, kindsName.Length - 1);
                            appList[i].InsuranceKinds = kindsName;
                            appList[i].AddIds = addIds;
                        }
                    }
                    return appList;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }

        /// <summary>
        /// 查询待责任客服审核列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <returns></returns>      
        public List<EmployeeApprove> GetApproveList(SysEntities db, string search)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var emp = ent.Employee.Where(a => true);
                    var empGoon = ent.EmployeeGoonPayment.Where(a => true);  // 补缴
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
                        {
                            string str = queryDic["Name"];
                            emp = emp.Where(a => a.Name.Contains(str));
                        }

                        if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
                        {
                            string str = queryDic["CertificateNumber"];
                            emp = emp.Where(a => a.CertificateNumber.Contains(str));
                        }

                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            empGoon = empGoon.Where(a => states.Contains(a.State));
                        }

                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            int str = Convert.ToInt32(queryDic["YearMonth"]);
                            empGoon = empGoon.Where(a => a.YearMonth == str);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                    }
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empGoon on r.Id equals b.EmployeeAdd.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on b.EmployeeAdd.PoliceAccountNatureId equals f.Id
                                select new EmployeeApprove()
                                {
                                    CompanyEmployeeRelationId = b.EmployeeAdd.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    CompanyName = c.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = e.Name,
                                    PoliceAccountNature = f.Name,
                                    YearMonth = b.YearMonth
                                }).Distinct();

                    if (list != null && list.Count() >= 1)
                    {

                        list = list.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyEmployeeRelationId);

                    }

                    List<EmployeeApprove> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from a in empGoon
                                join b in list on a.EmployeeAdd.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                                //join d in ent.InsuranceKind on a.InsuranceKindId equals d.Id
                                where a.YearMonth == b.YearMonth
                                select new
                                {
                                    a.EmployeeAdd.CompanyEmployeeRelationId,
                                    a.EmployeeAdd.InsuranceKindId,
                                    a.YearMonth,
                                    a.Id
                                }).ToList();
                    for (int i = 0; i < appList.Count(); i++)
                    {
                        int? Id = appList[i].CompanyEmployeeRelationId;
                        int? YearMonth = appList[i].YearMonth;
                        if (Id != null)
                        {
                            var kinds = temp.Where(a => a.CompanyEmployeeRelationId == Id && a.YearMonth == YearMonth).ToList();
                            //var kinds = temp.Where(a => a.CompanyEmployeeRelationId == Id).ToList();
                            string kindsName = "";
                            string addIds = "";
                            foreach (var a in kinds)
                            {
                                kindsName += ((Common.EmployeeAdd_InsuranceKindId)a.InsuranceKindId + ",");
                                addIds += (a.Id + ",");
                            }
                            appList[i].InsuranceKinds = kindsName;
                            appList[i].AddIds = addIds;
                        }
                    }
                    return appList;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }

        #endregion

        #region 责任客服审核补缴数据
        /// <summary>
        /// 责任客服审核
        /// </summary>
        /// <param name="approvedId">待审核补缴数据ID</param>
        /// <param name="stateOld">原状态</param>
        /// <param name="stateNew">新审核状态</param>
        /// <returns></returns>
        public void EmployeeGoonPaymentApproved(SysEntities db, int?[] approvedId, string stateOld, string stateNew)
        {
            var updateEmpGoon = db.EmployeeGoonPayment.Where(a => approvedId.Contains(a.Id) && a.State == stateOld);
            if (updateEmpGoon != null && updateEmpGoon.Count() >= 1)
            {
                foreach (var item in updateEmpGoon)
                {
                    item.State = stateNew;
                }
            }
        }
        #endregion

        #region 社保专员提取报增数据
        /// <summary>
        /// 社保专员提取报增数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeGoonPaymentView> GetEmployeeGoonPaymentExcelList(SysEntities db, int page, int rows, string search, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    #region 查询条件


                    var emp = ent.Employee.Where(a => true);
                    var empGoon = ent.EmployeeGoonPayment.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
                        {
                            string str = queryDic["Name"];
                            emp = emp.Where(a => a.Name.Contains(str));
                        }
                        if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
                        {
                            string[] cardIdList = queryDic["CertificateNumber"].Split(Convert.ToChar(10));
                            List<string> cardList = new List<string>();
                            for (int i = 0; i < cardIdList.Length; i++)
                            {
                                cardList.Add(cardIdList[i]);
                                cardList.Add(CardCommon.CardIDTo15(cardIdList[i]));
                                cardList.Add(CardCommon.CardIDTo18(cardIdList[i]));
                            }
                            cardList = cardList.Distinct().ToList();
                            emp = emp.Where(o => cardList.Contains(o.CertificateNumber));
                        }

                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            empGoon = empGoon.Where(a => states.Contains(a.State));
                        }
                        if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        {
                            string str = queryDic["InsuranceKinds"];
                            int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                            empGoon = empGoon.Where(a => Ids.Contains(a.EmployeeAdd.InsuranceKindId));
                        }

                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            int str = Convert.ToInt32(queryDic["YearMonth"]);
                            empGoon = empGoon.Where(a => a.YearMonth == str);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                    }
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empGoon on r.Id equals b.EmployeeAdd.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on b.EmployeeAdd.PoliceAccountNatureId equals f.Id
                                select new EmployeeGoonPaymentView()
                                {
                                    CompanyEmployeeRelationId = b.EmployeeAdd.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    CompanyName = c.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = e.Name,
                                  //  CityID = r.CityId,
                                    PoliceAccountNatureName = f.Name,
                                    YearMonth = (int)b.YearMonth,  
                                    CompanyCode = c.CompanyCode,            
                                    Station = r.Station
                                }).Distinct();
                    count = 0;
                    if (list != null && list.Count() >= 1)
                    {
                        count = list.Count();
                        if (page > -1)
                        {
                            list = list.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                        }
                    }

                    List<EmployeeGoonPaymentView> appList = list.ToList();
                    //List<EmployeeAddView> appList1 = uuuu.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empGoon
                                //   join f in appList1  on b.Id equals f.Id
                              //  join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.EmployeeAdd.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.EmployeeAdd.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    b.EmployeeAdd.CompanyEmployeeRelationId,
                                    b.EmployeeAdd.InsuranceKindId,
                                    b.EmployeeAdd.PoliceInsuranceId,
                                    b.YearMonth,
                                    b.EmployeeAdd.InsuranceCode,
                                    b.EmployeeAdd.Wage,
                                    d.Name,
                                    b.State,
                                    b.StartTime,
                                    b.EndTime,
                                    PoliceOperationName = e.Name,
                                    b.InsuranceMonth,
                                    b.EmployeeAdd.IsIndependentAccount,
                                    b.CreateTime,
                                    PoliceAccountNaturename= b.EmployeeAdd.PoliceAccountNature.Name
                                }).ToList();
                    int comMark = 1;
                    int empMark = 2;
                    appList = (from a in appList
                               join bb1 in temp.Where(a => a.InsuranceKindId == 1) on a.CompanyEmployeeRelationId equals bb1.CompanyEmployeeRelationId into bt1
                               from b1 in bt1.DefaultIfEmpty()
                               join bb2 in temp.Where(a => a.InsuranceKindId == 2) on a.CompanyEmployeeRelationId equals bb2.CompanyEmployeeRelationId into bt2
                               from b2 in bt2.DefaultIfEmpty()
                               join bb3 in temp.Where(a => a.InsuranceKindId == 3) on a.CompanyEmployeeRelationId equals bb3.CompanyEmployeeRelationId into bt3
                               from b3 in bt3.DefaultIfEmpty()
                               join bb4 in temp.Where(a => a.InsuranceKindId == 4) on a.CompanyEmployeeRelationId equals bb4.CompanyEmployeeRelationId into bt4
                               from b4 in bt4.DefaultIfEmpty()
                               join bb5 in temp.Where(a => a.InsuranceKindId == 5) on a.CompanyEmployeeRelationId equals bb5.CompanyEmployeeRelationId into bt5
                               from b5 in bt5.DefaultIfEmpty()
                               join bb6 in temp.Where(a => a.InsuranceKindId == 6) on a.CompanyEmployeeRelationId equals bb6.CompanyEmployeeRelationId into bt6
                               from b6 in bt6.DefaultIfEmpty()
                               join bb7 in temp.Where(a => a.InsuranceKindId == 7) on a.CompanyEmployeeRelationId equals bb7.CompanyEmployeeRelationId into bt7
                               from b7 in bt7.DefaultIfEmpty()
                               join bb8 in temp.Where(a => a.InsuranceKindId == 8) on a.CompanyEmployeeRelationId equals bb8.CompanyEmployeeRelationId into bt8
                               from b8 in bt8.DefaultIfEmpty()
                               select new EmployeeGoonPaymentView()
                               {
                                   CompanyEmployeeRelationId = a.CompanyEmployeeRelationId,
                                   CompanyCode = a.CompanyCode,
                                   CompanyName = a.CompanyName,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   City = a.City,
                                   PoliceAccountNatureName = a.PoliceAccountNatureName,
                                   Station = a.Station,
                                   AddIds = (b1 == null ? "" : b1.Id.ToString() + ",")
                                                     + (b2 == null ? "" : b2.Id.ToString() + ",")
                                                     + (b3 == null ? "" : b3.Id.ToString() + ",")
                                                     + (b4 == null ? "" : b4.Id.ToString() + ",")
                                                     + (b5 == null ? "" : b5.Id.ToString() + ",")
                                                     + (b6 == null ? "" : b6.Id.ToString() + ",")
                                                     + (b7 == null ? "" : b7.Id.ToString() + ",")
                                                     + (b8 == null ? "" : b8.Id.ToString()),

                                   InsuranceCode_1 = b1 == null ? null : b1.InsuranceCode,
                                   CompanyNumber_1 = b1 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, comMark),
                                   CompanyPercent_1 = b1 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, comMark),
                                   EmployeeNumber_1 = b1 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, empMark),
                                   EmployeePercent_1 = b1 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, empMark),
                                   State_1 = b1 == null ? null : b1.State,
                                   PoliceInsuranceName_1 = b1 == null ? null : b1.Name,
                                   YearMonth_1 = b1 == null ? null : b1.YearMonth,
                                   StartTime_1 = b1 == null ? null : b1.StartTime,
                                   EndTime_1 = b1 == null ? null : b1.EndTime,
                                   Wage_1 = b1 == null ? null : b1.Wage,
                                   PoliceOperationName_1 = b1 == null ? null : b1.PoliceOperationName,

                                   InsuranceMonth_1 = b1 == null ? null : b1.InsuranceMonth,
                                   IsIndependentAccount_1 = b1 == null ? null : b1.IsIndependentAccount,
                                   CreateTime_1 = b1 == null ? null : b1.CreateTime,
                                   PoliceAccountNatureName_1 = b1 == null ? null : b1.PoliceAccountNaturename,
                                   



                                   InsuranceCode_2 = b2 == null ? null : b2.InsuranceCode,
                                   CompanyNumber_2 = b2 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, comMark),
                                   CompanyPercent_2 = b2 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, comMark),
                                   EmployeeNumber_2 = b2 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, empMark),
                                   EmployeePercent_2 = b2 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, empMark),
                                   State_2 = b2 == null ? null : b2.State,
                                   PoliceInsuranceName_2 = b2 == null ? null : b2.Name,
                                   YearMonth_2 = b2 == null ? null : b2.YearMonth,
                                   StartTime_2 = b2 == null ? null : b2.StartTime,
                                   EndTime_2 = b2 == null ? null : b2.EndTime,
                                   Wage_2 = b2 == null ? null : b2.Wage,
                                   PoliceOperationName_2 = b2 == null ? null : b2.PoliceOperationName,

                                   InsuranceMonth_2 = b2 == null ? null : b2.InsuranceMonth,
                                   IsIndependentAccount_2 = b2 == null ? null : b2.IsIndependentAccount,
                                   CreateTime_2 = b2 == null ? null : b2.CreateTime,
                                   PoliceAccountNatureName_2 = b2 == null ? null : b2.PoliceAccountNaturename,

                                   InsuranceCode_3 = b3 == null ? null : b3.InsuranceCode,
                                   CompanyNumber_3 = b3 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, comMark),
                                   CompanyPercent_3 = b3 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, comMark),
                                   EmployeeNumber_3 = b3 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, empMark),
                                   EmployeePercent_3 = b3 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, empMark),
                                   State_3 = b3 == null ? null : b3.State,
                                   PoliceInsuranceName_3 = b3 == null ? null : b3.Name,
                                   YearMonth_3 = b3 == null ? null : b3.YearMonth,
                                   StartTime_3 = b3 == null ? null : b3.StartTime,
                                   EndTime_3 = b3 == null ? null : b3.EndTime,
                                   Wage_3 = b3 == null ? null : b3.Wage,
                                   PoliceOperationName_3 = b3 == null ? null : b3.PoliceOperationName,

                                   InsuranceMonth_3 = b3 == null ? null : b3.InsuranceMonth,
                                   IsIndependentAccount_3 = b3 == null ? null : b3.IsIndependentAccount,
                                   CreateTime_3 = b3 == null ? null : b3.CreateTime,
                                   PoliceAccountNatureName_3 = b3 == null ? null : b3.PoliceAccountNaturename,


                                   InsuranceCode_4 = b4 == null ? null : b4.InsuranceCode,
                                   CompanyNumber_4 = b4 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, comMark),
                                   CompanyPercent_4 = b4 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, comMark),
                                   EmployeeNumber_4 = b4 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, empMark),
                                   EmployeePercent_4 = b4 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, empMark),
                                   State_4 = b4 == null ? null : b4.State,
                                   PoliceInsuranceName_4 = b4 == null ? null : b4.Name,
                                   YearMonth_4 = b4 == null ? null : b4.YearMonth,
                                   StartTime_4 = b4 == null ? null : b4.StartTime,
                                   EndTime_4 = b4 == null ? null : b4.EndTime,
                                   Wage_4 = b4 == null ? null : b4.Wage,
                                   PoliceOperationName_4 = b4 == null ? null : b4.PoliceOperationName,

                                   InsuranceMonth_4 = b4 == null ? null : b4.InsuranceMonth,
                                   IsIndependentAccount_4 = b4 == null ? null : b4.IsIndependentAccount,
                                   CreateTime_4 = b4 == null ? null : b4.CreateTime,
                                   PoliceAccountNatureName_4 = b4 == null ? null : b4.PoliceAccountNaturename,


                                   InsuranceCode_5 = b5 == null ? null : b5.InsuranceCode,
                                   CompanyNumber_5 = b5 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, comMark),
                                   CompanyPercent_5 = b5 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, comMark),
                                   EmployeeNumber_5 = b5 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, empMark),
                                   EmployeePercent_5 = b5 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, empMark),
                                   State_5 = b5 == null ? null : b5.State,
                                   PoliceInsuranceName_5 = b5 == null ? null : b5.Name,
                                   YearMonth_5 = b5 == null ? null : b5.YearMonth,
                                   StartTime_5 = b5 == null ? null : b5.StartTime,
                                   EndTime_5 = b5 == null ? null : b5.EndTime,
                                   Wage_5 = b5 == null ? null : b5.Wage,
                                   PoliceOperationName_5 = b5 == null ? null : b5.PoliceOperationName,

                                   InsuranceMonth_5 = b5 == null ? null : b5.InsuranceMonth,
                                   IsIndependentAccount_5 = b5 == null ? null : b5.IsIndependentAccount,
                                   CreateTime_5 = b5 == null ? null : b5.CreateTime,
                                   PoliceAccountNatureName_5 = b5 == null ? null : b5.PoliceAccountNaturename,


                                   InsuranceCode_6 = b6 == null ? null : b6.InsuranceCode,
                                   CompanyNumber_6 = b6 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, comMark),
                                   CompanyPercent_6 = b6 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, comMark),
                                   EmployeeNumber_6 = b6 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, empMark),
                                   EmployeePercent_6 = b6 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, empMark),
                                   State_6 = b6 == null ? null : b6.State,
                                   PoliceInsuranceName_6 = b6 == null ? null : b6.Name,
                                   YearMonth_6 = b6 == null ? null : b6.YearMonth,
                                   StartTime_6 = b6 == null ? null : b6.StartTime,
                                   EndTime_6 = b6 == null ? null : b6.EndTime,
                                   Wage_6 = b6 == null ? null : b6.Wage,
                                   PoliceOperationName_6 = b6 == null ? null : b6.PoliceOperationName,

                                   InsuranceMonth_6 = b6 == null ? null : b6.InsuranceMonth,
                                   IsIndependentAccount_6 = b6 == null ? null : b6.IsIndependentAccount,
                                   CreateTime_6 = b6 == null ? null : b6.CreateTime,
                                   PoliceAccountNatureName_6 = b6 == null ? null : b6.PoliceAccountNaturename,


                                   InsuranceCode_7 = b7 == null ? null : b7.InsuranceCode,
                                   CompanyNumber_7 = b7 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, comMark),
                                   CompanyPercent_7 = b7 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, comMark),
                                   EmployeeNumber_7 = b7 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, empMark),
                                   EmployeePercent_7 = b7 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, empMark),
                                   State_7 = b7 == null ? null : b7.State,
                                   PoliceInsuranceName_7 = b7 == null ? null : b7.Name,
                                   YearMonth_7 = b7 == null ? null : b7.YearMonth,
                                   StartTime_7 = b7 == null ? null : b7.StartTime,
                                   EndTime_7 = b7 == null ? null : b7.EndTime,
                                   Wage_7 = b7 == null ? null : b7.Wage,
                                   PoliceOperationName_7 = b7 == null ? null : b7.PoliceOperationName,

                                   InsuranceMonth_7 = b7 == null ? null : b7.InsuranceMonth,
                                   IsIndependentAccount_7 = b7 == null ? null : b7.IsIndependentAccount,
                                   CreateTime_7 = b7 == null ? null : b7.CreateTime,
                                   PoliceAccountNatureName_7 = b7 == null ? null : b7.PoliceAccountNaturename,


                                   InsuranceCode_8 = b8 == null ? null : b8.InsuranceCode,
                                   CompanyNumber_8 = b8 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, comMark),
                                   CompanyPercent_8 = b8 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, comMark),
                                   EmployeeNumber_8 = b8 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_Jishu(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, empMark),
                                   EmployeePercent_8 = b8 == null ? null : (decimal?)Langben.DAL.EmployeeAddRepository.Get_BILI(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, empMark),
                                   State_8 = b8 == null ? null : b8.State,
                                   PoliceInsuranceName_8 = b8 == null ? null : b8.Name,
                                   YearMonth_8 = b8 == null ? null : b8.YearMonth,
                                   StartTime_8 = b8 == null ? null : b8.StartTime,
                                   EndTime_8 = b8 == null ? null : b8.EndTime,
                                   Wage_8 = b8 == null ? null : b8.Wage,
                                   PoliceOperationName_8 = b8 == null ? null : b8.PoliceOperationName,

                                   InsuranceMonth_8 = b8 == null ? null : b8.InsuranceMonth,
                                   IsIndependentAccount_8 = b8 == null ? null : b8.IsIndependentAccount,
                                   CreateTime_8 = b8 == null ? null : b8.CreateTime,
                                   PoliceAccountNatureName_8 = b8 == null ? null : b8.PoliceAccountNaturename,

                               }).ToList();
                    #endregion
                    return appList;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }

        #endregion
    }
}
