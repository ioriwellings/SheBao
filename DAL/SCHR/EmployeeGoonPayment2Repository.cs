using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
using Langben.DAL.Model;
using System.Text;
namespace Langben.DAL
{
    /// <summary>
    /// 员工调基
    /// </summary>
    public partial class EmployeeGoonPayment2Repository : BaseRepository<EmployeeGoonPayment2>, IDisposable
    {
        #region 正常服务中的员工列表
        /// <summary>
        /// 正常服务中的员工列表
        /// </summary>
        /// <param name="db">数据访问上下文</param>
        /// <param name="page">页数</param>
        /// <param name="rows">行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="departmentScope">数据范围权限</param>
        /// <param name="departments">数据部门权限</param>
        /// <param name="branchID">登录人所属分支机构ID</param>
        /// <param name="departmentID">登录人所属部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <param name="count">数据总条数</param>
        /// <returns></returns>
        public List<EmployeeGoonPayment2View> GetEmployeeList(SysEntities db, int page, int rows, string search, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int count)
        {
            using (db)
            {
                try
                {
                    string StateName = Common.EmployeeAdd_State.申报成功.ToString();
                    var emp = db.Employee.Where(a => true);
                    var empAdd = db.EmployeeAdd.Where(a => a.State == StateName);
                    var com = db.CRM_Company.Where(a => true);
                    //var city = db.City.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        //员工姓名
                        if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
                        {
                            string str = queryDic["Name"];
                            emp = emp.Where(a => a.Name.Contains(str));
                        }
                        //身份证号
                        if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
                        {
                            string CertificateNumber = queryDic["CertificateNumber"];
                            string[] CARD_ID_LIST = CertificateNumber.Split(Convert.ToChar(10));
                            List<string> CARDLIST = new List<string>();
                            //var CARD_ID_LIST = CARD_ID.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < CARD_ID_LIST.Length; i++)
                            {
                                CARDLIST.Add(CARD_ID_LIST[i]);
                                CARDLIST.Add(CardCommon.CardIDTo15(CARD_ID_LIST[i]));
                                CARDLIST.Add(CardCommon.CardIDTo18(CARD_ID_LIST[i]));
                            }
                            CARDLIST = CARDLIST.Distinct().ToList();
                            emp = emp.Where(o => CARDLIST.Contains(o.CertificateNumber));
                        }
                        ////状态
                        //if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        //{
                        //    string str = queryDic["State"];
                        //    string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //    empAdd = empAdd.Where(a => states.Contains(a.State));
                        //}
                        //if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        //{
                        //    string str = queryDic["InsuranceKinds"];
                        //    int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                        //    empAdd = empAdd.Where(a => Ids.Contains(a.InsuranceKindId));
                        //}

                        //if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        //{
                        //    int str = Convert.ToInt32(queryDic["YearMonth"]);
                        //    empAdd = empAdd.Where(a => a.YearMonth == str);
                        //}
                        //企业名称
                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        //if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        //{
                        //    string str = queryDic["CityId"];
                        //    city = city.Where(a => a.Id == str);
                        //}
                    }

                    List<string> States = new List<string>();
                    States.Add(Common.EmployeeGoonPayment2_STATUS.待员工客服确认.ToString());
                    States.Add(Common.EmployeeGoonPayment2_STATUS.待责任客服确认.ToString());
                    States.Add(Common.EmployeeGoonPayment2_STATUS.社保专员已提取.ToString());
                    States.Add(Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString());

                    var goon = db.EmployeeGoonPayment2.Where(c => States.Contains(c.State));

                    var list = from a in emp
                               join r in db.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                               join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                               join c in com on r.CompanyId equals c.ID
                               //join d in db.InsuranceKind on b.InsuranceKindId equals d.Id
                               //join e in city on r.CityId equals e.Id
                               //join f in db.PoliceAccountNature on b.PoliceAccountNatureId equals f.Id
                               join g in db.CRM_CompanyToBranch on c.ID equals g.CRM_Company_ID
                               select new EmployeeGoonPayment2View()
                               {
                                   Id = b.Id,
                                   CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                   CompanyId = r.CompanyId,
                                   CompanyName = c.CompanyName,
                                   EmployeeID = a.Id,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   //City = e.Name,
                                   //CityID = r.CityId,
                                   //PoliceAccountNature = f.Name,
                                   //YearMonth = b.YearMonth,
                                   UserID_ZR = g.UserID_ZR,
                                   BranchID = g.BranchID,
                                   InsuranceKindId = b.InsuranceKindId,
                                   InsuranceKindName = ""
                               };

                    #region 数据范围权限配置

                    if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
                    {
                        if (!string.IsNullOrEmpty(departments))
                        {
                            //获取自定义限制的部门所有员工
                            var people = from a in db.ORG_User
                                         where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                         select a;

                            //获取数据源中责任客服为以上取得员工列表的数据
                            list = from a in list
                                   join b in people on a.UserID_ZR equals b.ID
                                   select a;

                        }
                    }
                    else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
                    {
                        //本机构
                        var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);

                        //根据机构左右值获取本机构及下属机构所有员工
                        var people = from a in db.ORG_User
                                     join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                     where b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        list = from a in list
                               join b in people on a.UserID_ZR equals b.ID
                               select a;
                    }
                    else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
                    {
                        list = from a in list
                               where a.BranchID == branchID
                               select a;
                    }
                    else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
                    {
                        //当前用户所属部门
                        ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);

                        //查询本部门及下属部门所有部门数据
                        var people = from a in db.ORG_User
                                     join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                     where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        list = from a in list
                               join b in people on a.UserID_ZR equals b.ID
                               select a;
                    }
                    else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
                    {
                        //查询本部门所有用户数据
                        var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                     join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.ID == departmentID) on a.ORG_Department_ID equals b.ID
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        list = from a in list
                               join b in people on a.UserID_ZR equals b.ID
                               select a;
                    }
                    else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
                    {
                        list = list.Where(c => c.UserID_ZR == userID);
                    }

                    #endregion

                    List<EmployeeGoonPayment2View> appList = list.ToList();

                    //判断该社保记录有无正在进行中个调基数据
                    foreach (var m in appList)
                    {
                        var AddList = goon.Where(c => c.EmployeeAddId == m.Id);

                        int cnt = AddList.Count();
                        m.FLG = cnt > 0 ? 1 : 0; ;
                    }

                    var res = from b in appList
                              group b by new { b.CompanyId, b.CompanyName, b.EmployeeID, b.Name, b.CertificateNumber, b.CompanyEmployeeRelationId } into g
                              select new EmployeeGoonPayment2View
                              {
                                  CompanyEmployeeRelationId = g.Key.CompanyEmployeeRelationId,
                                  CompanyId = g.Key.CompanyId,
                                  CompanyName = g.Key.CompanyName,
                                  EmployeeID = g.Key.EmployeeID,
                                  Name = g.Key.Name,
                                  CertificateNumber = g.Key.CertificateNumber,
                                  InsuranceKindName = string.Join(",", (g.Select(o => (Common.EmployeeAdd_InsuranceKindId)o.InsuranceKindId))),
                                  FLG = g.Sum(c => c.FLG)
                                  //,AddIds = string.Join(",", (g.Select(o => o.Id)))
                              };

                    count = 0;
                    if (res != null && res.Count() > 0)
                    {
                        count = res.Count();
                        if (page > -1)
                        {
                            res = res.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                        }
                    }


                    return res.ToList();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }
        #endregion

        #region 员工客服确认调基数据
        /// <summary>
        /// 员工客服确认调基数据
        /// </summary>
        /// <param name="db">数据访问上下文</param>
        /// <param name="page">页数</param>
        /// <param name="rows">行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="departmentScope">数据范围权限</param>
        /// <param name="departments">数据部门权限</param>
        /// <param name="branchID">登录人所属分支机构ID</param>
        /// <param name="departmentID">登录人所属部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <param name="count">数据总条数</param>
        /// <returns></returns>      
        public List<EmployeeGoonPayment2View> GetApproveList(SysEntities db, int page, int rows, string search, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int count)
        {
            using (db)
            {
                try
                {
                    string StateName = Common.EmployeeAdd_State.申报成功.ToString();
                    string StateName2 = Common.EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    var emp = db.Employee.Where(a => true);
                    var empAdd = db.EmployeeAdd.Where(a => a.State == StateName);
                    var com = db.CRM_Company.Where(a => true);
                    var comB = db.CRM_CompanyToBranch.Where(a => true);
                    var empGoon2 = db.EmployeeGoonPayment2.Where(a => a.State == StateName2);
                    var city = db.City.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        //身份证号
                        if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
                        {
                            string CertificateNumber = queryDic["CertificateNumber"];
                            string[] CARD_ID_LIST = CertificateNumber.Split(Convert.ToChar(10));
                            List<string> CARDLIST = new List<string>();
                            //var CARD_ID_LIST = CARD_ID.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < CARD_ID_LIST.Length; i++)
                            {
                                CARDLIST.Add(CARD_ID_LIST[i]);
                                CARDLIST.Add(CardCommon.CardIDTo15(CARD_ID_LIST[i]));
                                CARDLIST.Add(CardCommon.CardIDTo18(CARD_ID_LIST[i]));
                            }
                            CARDLIST = CARDLIST.Distinct().ToList();
                            emp = emp.Where(o => CARDLIST.Contains(o.CertificateNumber));
                        }

                        //申报时间
                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            int str = Convert.ToInt32(queryDic["YearMonth"]);
                            //DateTime dtStart = Convert.ToDateTime(str.Substring(0, 4) + "/" + str.Substring(4, 2) + "/01");
                            //DateTime dtEnd = dtStart.AddMonths(1);
                            //empGoon2 = empGoon2.Where(a => a.CreateTime >= dtStart && a.CreateTime < dtEnd);
                            empGoon2 = empGoon2.Where(a => a.YearMonth == str);
                        }

                        //企业名称
                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }

                        //责任客服
                        if (queryDic.ContainsKey("UserID_ZR") && !string.IsNullOrWhiteSpace(queryDic["UserID_ZR"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_ZR"]);
                            comB = comB.Where(a => a.UserID_ZR == str);
                        }
                        //if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        //{
                        //    string str = queryDic["CityId"];
                        //    city = city.Where(a => a.Id == str);
                        //}
                    }
                    var list = from a in emp
                               join r in db.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                               join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                               join m in empGoon2 on b.Id equals m.EmployeeAddId
                               join c in com on r.CompanyId equals c.ID
                               join g in comB on c.ID equals g.CRM_Company_ID
                               //join d in db.InsuranceKind on b.InsuranceKindId equals d.Id
                               join e in city on m.CityId equals e.Id
                               join f in db.PoliceAccountNature on b.PoliceAccountNatureId equals f.Id
                               select new EmployeeGoonPayment2View()
                               {
                                   Id = m.Id,
                                   CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                   CompanyId = r.CompanyId,
                                   CompanyName = c.CompanyName,
                                   EmployeeID = a.Id,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   City = e.Name,
                                   CityID = m.CityId,
                                   PoliceAccountNature = f.Name,
                                   //CreateTime = m.CreateTime,
                                   YearMonth = m.YearMonth,
                                   UserID_ZR = g.UserID_ZR,
                                   BranchID = g.BranchID,
                                   InsuranceKindId = b.InsuranceKindId,
                                   InsuranceKindName = ""
                               };

                    #region 数据范围权限配置

                    if (departmentScope == 0)//无限制
                    {
                        if (!string.IsNullOrEmpty(departments))
                        {
                            //获取自定义限制的部门所有员工
                            var people = from a in db.ORG_User
                                         where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                         select a;

                            //获取数据源中责任客服为以上取得员工列表的数据
                            list = from a in list
                                   join b in people on a.UserID_ZR equals b.ID
                                   select a;

                        }
                    }
                    else if (departmentScope == 1)//本机构及下属机构
                    {
                        //本机构
                        var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);

                        //根据机构左右值获取本机构及下属机构所有员工
                        var people = from a in db.ORG_User
                                     join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                     where b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        list = from a in list
                               join b in people on a.UserID_ZR equals b.ID
                               select a;
                    }
                    else if (departmentScope == 2) //本机构
                    {
                        list = from a in list
                               where a.BranchID == branchID
                               select a;
                    }
                    else if (departmentScope == 3)//本部门及其下属部门
                    {
                        //当前用户所属部门
                        ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);

                        //查询本部门及下属部门所有部门数据
                        var people = from a in db.ORG_User
                                     join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                     where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        list = from a in list
                               join b in people on a.UserID_ZR equals b.ID
                               select a;
                    }
                    else if (departmentScope == 4) //本部门
                    {
                        //查询本部门所有用户数据
                        var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                     join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.ID == departmentID) on a.ORG_Department_ID equals b.ID
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        list = from a in list
                               join b in people on a.UserID_ZR equals b.ID
                               select a;
                    }
                    else if (departmentScope == 5) //本人
                    {
                        list = list.Where(c => c.UserID_ZR == userID);
                    }

                    #endregion

                    List<EmployeeGoonPayment2View> appList = list.ToList();

                    //foreach (var m in appList)
                    //{
                    //    m.YearMonth = Convert.ToInt32(Convert.ToDateTime(m.CreateTime).Year.ToString().PadLeft(4, '0') + Convert.ToDateTime(m.CreateTime).Month.ToString().PadLeft(2, '0'));
                    //}

                    var res = from b in appList
                              group b by new
                              {
                                  b.CompanyId,
                                  b.CompanyName,
                                  b.EmployeeID,
                                  b.Name,
                                  b.CertificateNumber,
                                  b.CompanyEmployeeRelationId,
                                  b.City,
                                  b.PoliceAccountNature,
                                  b.YearMonth
                              } into g
                              select new EmployeeGoonPayment2View
                              {
                                  CompanyEmployeeRelationId = g.Key.CompanyEmployeeRelationId,
                                  CompanyId = g.Key.CompanyId,
                                  CompanyName = g.Key.CompanyName,
                                  EmployeeID = g.Key.EmployeeID,
                                  Name = g.Key.Name,
                                  CertificateNumber = g.Key.CertificateNumber,
                                  City = g.Key.City,
                                  PoliceAccountNature = g.Key.PoliceAccountNature,
                                  YearMonth = g.Key.YearMonth,
                                  InsuranceKindName = string.Join(",", (g.Select(o => (Common.EmployeeAdd_InsuranceKindId)o.InsuranceKindId))),
                                  AddIds = string.Join(",", (g.Select(o => o.Id)))
                              };

                    count = 0;
                    if (res != null && res.Count() > 0)
                    {
                        count = res.Count();
                        if (page > -1)
                        {
                            res = res.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                        }
                    }


                    return res.ToList();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #region 调基数据状态变更
        /// <summary>
        /// 调基数据状态变更
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="StateNew">新审核状态</param>
        /// <returns></returns>
        public bool EmployeeGoonPayment2Approved(SysEntities db, int?[] ApprovedId, string StateNew, string UserName)
        {
            using (db)
            {
                try
                {
                    if (ApprovedId == null || ApprovedId.Count() <= 0)
                    {
                        return false;
                    }

                    var updList = db.EmployeeGoonPayment2.Where(a => ApprovedId.Contains(a.Id));
                    if (updList.Any())
                    {
                        foreach (var item in updList)
                        {
                            item.State = StateNew;
                            item.UpdatePerson = UserName;
                            item.UpdateTime = DateTime.Now;
                        }
                        db.SaveChanges();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #region 员工客服终止调基
        /// <summary>
        /// 员工客服终止调基
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <returns></returns>
        public bool EmployeeGoonPayment2End(SysEntities db, int?[] ApprovedId, string UserName)
        {
            using (db)
            {
                try
                {
                    if (ApprovedId == null || ApprovedId.Count() <= 0)
                    {
                        return false;
                    }
                    string Enable = Status.启用.ToString();
                    var updList = db.EmployeeGoonPayment2.Where(a => ApprovedId.Contains(a.Id));
                    if (updList.Any())
                    {
                        foreach (var item in updList)
                        {
                            item.State = Common.EmployeeGoonPayment2_STATUS.终止.ToString();
                            item.UpdatePerson = UserName;
                            item.UpdateTime = DateTime.Now;

                            //更新增加员工表
                            var empAdd = db.EmployeeAdd.FirstOrDefault(c => c.Id == item.EmployeeAddId);
                            empAdd.Wage = item.OldWage;
                            empAdd.UpdateTime = DateTime.Now;


                            //更新中间表
                            var Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == empAdd.CompanyEmployeeRelationId && e.InsuranceKindId == empAdd.InsuranceKindId && e.State == Enable);

                            if (Middle.Any())
                            {
                                var JISHU_C = EmployeeAddRepository.Get_Jishu(db, (int)empAdd.PoliceInsuranceId, (decimal)item.OldWage, 1);//公司基数
                                var JISHU_P = EmployeeAddRepository.Get_Jishu(db, (int)empAdd.PoliceInsuranceId, (decimal)item.OldWage, 2);//个人基数
                                var PERCENT_C = EmployeeAddRepository.Get_BILI(db, (int)empAdd.PoliceInsuranceId, (decimal)item.OldWage, 1);//公司比例
                                var PERCENT_P = EmployeeAddRepository.Get_BILI(db, (int)empAdd.PoliceInsuranceId, (decimal)item.OldWage, 2);//个人比例
                                foreach (var order in Middle)
                                {
                                    order.CompanyBasePayment = JISHU_C;
                                    order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, (int)empAdd.PoliceInsuranceId);
                                    order.EmployeeBasePayment = JISHU_P;
                                    order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, (int)empAdd.PoliceInsuranceId);
                                    order.Remark = "调基终止";
                                    order.UpdatePerson = UserName;
                                    order.UpdateTime = DateTime.Now;
                                }
                            }
                        }

                        db.SaveChanges();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #region 王骁健
        #region 获取调基员工的信息
        /// <summary>
        /// 获取调基员工的信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyEmployeeRelationId"></param>
        /// <returns></returns>
        public EmployeeGoonPayment2View GetChangeWageEmployeeInfo(SysEntities db, int companyEmployeeRelationId)
        {
            EmployeeGoonPayment2View info = new EmployeeGoonPayment2View();

            var cer =
                db.CompanyEmployeeRelation.Where(o => o.Id == companyEmployeeRelationId && o.State == "在职")
                    .OrderByDescending(o => o.Id);
            if (cer.Any())
            {
                var query1 = (from a in cer
                              join b in db.Employee on a.EmployeeId equals b.Id
                              join c in db.CRM_Company on a.CompanyId equals c.ID
                              where a.Id == companyEmployeeRelationId && a.State == "在职"
                              select new
                              {
                                  EmployeeName = b.Name,
                                  CardId = b.CertificateNumber,
                                  CompanyName = c.CompanyName,
                                  Station = a.Station,
                              }).First();
                //获取申报成功状态的报增信息
                string strState = Common.EmployeeAdd_State.申报成功.ToString();
                List<string> stopingState = new List<string>()
            {
                Common.EmployeeStopPayment_State.待责任客服确认.ToString(),
                Common.EmployeeStopPayment_State.待员工客服经理分配.ToString(),
                Common.EmployeeStopPayment_State.待员工客服确认.ToString(),
                Common.EmployeeStopPayment_State.员工客服已确认.ToString(),
                Common.EmployeeStopPayment_State.社保专员已提取.ToString(),
            };
                var query2 = from d in db.EmployeeAdd
                             where d.CompanyEmployeeRelationId == companyEmployeeRelationId && d.State == "申报成功" &&
                                  !stopingState.Contains(
                                      d.EmployeeStopPayment.OrderByDescending(o => o.CreateTime).FirstOrDefault().State)
                             select new
                             {
                                 CityID = d.PoliceInsurance.InsuranceKind.City1.Id,
                                 CityName = d.PoliceInsurance.InsuranceKind.City1.Name,
                                 PoliceAccountNatureName = d.PoliceAccountNature.Name,
                                 d.Wage,
                                 EmployeeAddId = d.Id,
                                 InsuranceKindId = d.PoliceInsurance.InsuranceKind.Id,
                                 InsuranceKindName = d.PoliceInsurance.InsuranceKind.Name,
                                 PoliceInsuranceId = d.PoliceInsuranceId,
                                 CompanyEmployeeRelationId = d.CompanyEmployeeRelationId,
                             };

                info.Name = query1.EmployeeName;
                info.CertificateNumber = query1.CardId;
                info.CompanyName = query1.CompanyName;
                info.Station = query1.Station;
                info.City = query2.First().CityName;
                info.CityID = query2.First().CityID;
                info.PoliceAccountNature = query2.First().PoliceAccountNatureName;
                info.SB_Wage = 0;
                info.GJJ_Wage = 0;
                var sb = query2.FirstOrDefault(o => !o.InsuranceKindName.Contains("公积金"));
                if (sb != null)
                {
                    info.SB_Wage = sb.Wage ?? 0;
                }
                var gjj = query2.FirstOrDefault(o => o.InsuranceKindName.Contains("公积金"));
                if (gjj != null)
                {
                    info.GJJ_Wage = gjj.Wage ?? 0;
                }
                List<ChangeWageInsuranceKindInfo> lstkindInfo = new List<ChangeWageInsuranceKindInfo>();

                foreach (var q in query2)
                {
                    ChangeWageInsuranceKindInfo kindInfo = new ChangeWageInsuranceKindInfo()
                    {
                        EmployeeAddId = q.EmployeeAddId,
                        InsuranceKindId = q.InsuranceKindId,
                        InsuranceKindName = q.InsuranceKindName,
                        PoliceInsuranceId = q.PoliceInsuranceId ?? 0,
                        Wage = q.Wage ?? 0,
                        CompanyEmployeeRelationId = q.CompanyEmployeeRelationId ?? 0,
                    };
                    lstkindInfo.Add(kindInfo);

                }
                info.LastChangeWageInsuranceKindInfo = lstkindInfo;
            }
            return info;
        }
        #endregion
        /// <summary>
        /// 调基（新社保工资）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="yanglaoID"></param>
        /// <param name="yiliaoID"></param>
        /// <param name="gongshangID"></param>
        /// <param name="shiyeID"></param>
        /// <param name="shengyuID"></param>
        /// <param name="gongjijinID"></param>
        /// <returns></returns>
        public int ChangeWage(SysEntities db, AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID, string userName)
        {
            List<EmployeeGoonPayment2> changeWageList = new List<EmployeeGoonPayment2>();
            string Enable = Status.启用.ToString();
            try
            {
                #region 养老
                if (entity.Pension_Wage != 0 && entity.Pension_Wage != null)
                {
                    EmployeeAdd yanglao_EmployeeAdd = db.EmployeeAdd.Where(e => e.Id == yanglaoID).FirstOrDefault();
                    decimal GZ_Yanglao = (decimal)entity.Pension_Wage;//工资
                    int ZC_Yanglao_ID = (int)yanglao_EmployeeAdd.PoliceInsuranceId;//政策id
                    //添加调基数据
                    EmployeeGoonPayment2 yanglaoModel = new EmployeeGoonPayment2();
                    yanglaoModel.NewWage = GZ_Yanglao;
                    yanglaoModel.OldWage = yanglao_EmployeeAdd.Wage;
                    yanglaoModel.InsuranceMonth = DateTime.Now;
                    yanglaoModel.EmployeeAddId = yanglaoID;
                    yanglaoModel.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                    yanglaoModel.State = EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    yanglaoModel.IsChangeMonth = "N";
                    yanglaoModel.CityId = entity.CityID;
                    yanglaoModel.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                    yanglaoModel.CreatePerson = userName;
                    yanglaoModel.CreateTime = DateTime.Now;
                    changeWageList.Add(yanglaoModel);
                    //更新报增表工资
                    yanglao_EmployeeAdd.Wage = GZ_Yanglao;
                    yanglao_EmployeeAdd.UpdatePerson = userName;
                    yanglao_EmployeeAdd.UpdateTime = DateTime.Now;
                    //更新中间表
                    var yanglao_Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == yanglao_EmployeeAdd.CompanyEmployeeRelationId && e.InsuranceKindId == yanglao_EmployeeAdd.InsuranceKindId && e.State == Enable);
                    if (yanglao_Middle.Count() > 0)
                    {
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(db, ZC_Yanglao_ID, GZ_Yanglao, 1);//公司基数
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(db, ZC_Yanglao_ID, GZ_Yanglao, 2);//个人基数
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(db, ZC_Yanglao_ID, GZ_Yanglao, 1);//公司比例
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(db, ZC_Yanglao_ID, GZ_Yanglao, 2);//个人比例
                        foreach (var order in yanglao_Middle)
                        {
                            order.CompanyBasePayment = JISHU_C;
                            order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, ZC_Yanglao_ID);
                            order.EmployeeBasePayment = JISHU_P;
                            order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, ZC_Yanglao_ID);
                            order.Remark = "调基";
                            order.UpdatePerson = userName;
                            order.UpdateTime = DateTime.Now;
                        }
                    }
                }
                #endregion

                #region 医疗
                if (entity.Medical_Wage != 0 && entity.Medical_Wage != null)
                {
                    EmployeeAdd yiliao_EmployeeAdd = db.EmployeeAdd.Where(e => e.Id == yiliaoID).FirstOrDefault();
                    decimal GZ_Yiliao = (decimal)entity.Medical_Wage;//工资
                    int ZC_Yiliao_ID = (int)yiliao_EmployeeAdd.PoliceInsuranceId;//政策id
                    //添加调基数据
                    EmployeeGoonPayment2 yiliaoModel = new EmployeeGoonPayment2();
                    yiliaoModel.NewWage = GZ_Yiliao;
                    yiliaoModel.OldWage = yiliao_EmployeeAdd.Wage;
                    yiliaoModel.InsuranceMonth = DateTime.Now;
                    yiliaoModel.EmployeeAddId = yiliaoID;
                    yiliaoModel.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                    yiliaoModel.State = EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    yiliaoModel.IsChangeMonth = "N";
                    yiliaoModel.CityId = entity.CityID;
                    yiliaoModel.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                    yiliaoModel.CreatePerson = userName;
                    yiliaoModel.CreateTime = DateTime.Now;
                    changeWageList.Add(yiliaoModel);
                    //更新报增表工资
                    yiliao_EmployeeAdd.Wage = GZ_Yiliao;
                    yiliao_EmployeeAdd.UpdatePerson = userName;
                    yiliao_EmployeeAdd.UpdateTime = DateTime.Now;
                    //更新中间表
                    var yiliao_Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == yiliao_EmployeeAdd.CompanyEmployeeRelationId && e.InsuranceKindId == yiliao_EmployeeAdd.InsuranceKindId && e.State == Enable);
                    if (yiliao_Middle.Count() > 0)
                    {
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(db, ZC_Yiliao_ID, GZ_Yiliao, 1);//公司基数
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(db, ZC_Yiliao_ID, GZ_Yiliao, 2);//个人基数
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(db, ZC_Yiliao_ID, GZ_Yiliao, 1);//公司比例
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(db, ZC_Yiliao_ID, GZ_Yiliao, 2);//个人比例
                        foreach (var order in yiliao_Middle)
                        {
                            order.CompanyBasePayment = JISHU_C;
                            order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, ZC_Yiliao_ID);
                            order.EmployeeBasePayment = JISHU_P;
                            order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, ZC_Yiliao_ID);
                            order.Remark = "调基";
                            order.UpdatePerson = userName;
                            order.UpdateTime = DateTime.Now;
                        }
                    }
                }
                #endregion

                #region 工伤
                if (entity.WorkInjury_Wage != 0 && entity.WorkInjury_Wage != null)
                {
                    EmployeeAdd gongshang_EmployeeAdd = db.EmployeeAdd.Where(e => e.Id == gongshangID).FirstOrDefault();
                    decimal GZ_gongshang = (decimal)entity.WorkInjury_Wage;//工资
                    int ZC_gongshang_ID = (int)gongshang_EmployeeAdd.PoliceInsuranceId;//政策id
                    //添加调基数据
                    EmployeeGoonPayment2 gongshangModel = new EmployeeGoonPayment2();
                    gongshangModel.NewWage = GZ_gongshang;
                    gongshangModel.OldWage = gongshang_EmployeeAdd.Wage;
                    gongshangModel.InsuranceMonth = DateTime.Now;
                    gongshangModel.EmployeeAddId = gongshangID;
                    gongshangModel.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                    gongshangModel.State = EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    gongshangModel.IsChangeMonth = "N";
                    gongshangModel.CityId = entity.CityID;
                    gongshangModel.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                    gongshangModel.CreatePerson = userName;
                    gongshangModel.CreateTime = DateTime.Now;
                    changeWageList.Add(gongshangModel);
                    //更新报增表工资
                    gongshang_EmployeeAdd.Wage = GZ_gongshang;
                    gongshang_EmployeeAdd.UpdatePerson = userName;
                    gongshang_EmployeeAdd.UpdateTime = DateTime.Now;
                    //更新中间表
                    var gongshang_Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == gongshang_EmployeeAdd.CompanyEmployeeRelationId && e.InsuranceKindId == gongshang_EmployeeAdd.InsuranceKindId && e.State == Enable);
                    if (gongshang_Middle.Count() > 0)
                    {
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(db, ZC_gongshang_ID, GZ_gongshang, 1);//公司基数
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(db, ZC_gongshang_ID, GZ_gongshang, 2);//个人基数
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(db, ZC_gongshang_ID, GZ_gongshang, 1);//公司比例
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(db, ZC_gongshang_ID, GZ_gongshang, 2);//个人比例
                        foreach (var order in gongshang_Middle)
                        {
                            order.CompanyBasePayment = JISHU_C;
                            order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, ZC_gongshang_ID);
                            order.EmployeeBasePayment = JISHU_P;
                            order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, ZC_gongshang_ID);
                            order.Remark = "调基";
                            order.UpdatePerson = userName;
                            order.UpdateTime = DateTime.Now;
                        }
                    }
                }
                #endregion

                #region 失业
                if (entity.Unemployment_Wage != 0 && entity.Unemployment_Wage != null)
                {
                    EmployeeAdd shiye_EmployeeAdd = db.EmployeeAdd.Where(e => e.Id == shiyeID).FirstOrDefault();
                    decimal GZ_shiye = (decimal)entity.Unemployment_Wage;//工资
                    int ZC_shiye_ID = (int)shiye_EmployeeAdd.PoliceInsuranceId;//政策id
                    //添加调基数据
                    EmployeeGoonPayment2 shiyeModel = new EmployeeGoonPayment2();
                    shiyeModel.NewWage = GZ_shiye;
                    shiyeModel.OldWage = shiye_EmployeeAdd.Wage;
                    shiyeModel.InsuranceMonth = DateTime.Now;
                    shiyeModel.EmployeeAddId = shiyeID;
                    shiyeModel.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;
                    shiyeModel.State = EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    shiyeModel.IsChangeMonth = "N";
                    shiyeModel.CityId = entity.CityID;
                    shiyeModel.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                    shiyeModel.CreatePerson = userName;
                    shiyeModel.CreateTime = DateTime.Now;
                    changeWageList.Add(shiyeModel);
                    //更新报增表工资
                    shiye_EmployeeAdd.Wage = GZ_shiye;
                    shiye_EmployeeAdd.UpdatePerson = userName;
                    shiye_EmployeeAdd.UpdateTime = DateTime.Now;
                    //更新中间表
                    var shiye_Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == shiye_EmployeeAdd.CompanyEmployeeRelationId && e.InsuranceKindId == shiye_EmployeeAdd.InsuranceKindId && e.State == Enable);
                    if (shiye_Middle.Count() > 0)
                    {
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(db, ZC_shiye_ID, GZ_shiye, 1);//公司基数
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(db, ZC_shiye_ID, GZ_shiye, 2);//个人基数
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(db, ZC_shiye_ID, GZ_shiye, 1);//公司比例
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(db, ZC_shiye_ID, GZ_shiye, 2);//个人比例
                        foreach (var order in shiye_Middle)
                        {
                            order.CompanyBasePayment = JISHU_C;
                            order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, ZC_shiye_ID);
                            order.EmployeeBasePayment = JISHU_P;
                            order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, ZC_shiye_ID);
                            order.Remark = "调基";
                            order.UpdatePerson = userName;
                            order.UpdateTime = DateTime.Now;
                        }
                    }
                }
                #endregion

                #region 生育
                if (entity.Maternity_Wage != 0 && entity.Maternity_Wage != null)
                {
                    EmployeeAdd shengyu_EmployeeAdd = db.EmployeeAdd.Where(e => e.Id == shengyuID).FirstOrDefault();
                    decimal GZ_shengyu = (decimal)entity.Maternity_Wage;//工资
                    int ZC_shengyu_ID = (int)shengyu_EmployeeAdd.PoliceInsuranceId;//政策id
                    //添加调基数据
                    EmployeeGoonPayment2 shengyuModel = new EmployeeGoonPayment2();
                    shengyuModel.NewWage = GZ_shengyu;
                    shengyuModel.OldWage = shengyu_EmployeeAdd.Wage;
                    shengyuModel.InsuranceMonth = DateTime.Now;
                    shengyuModel.EmployeeAddId = shengyuID;
                    shengyuModel.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                    shengyuModel.State = EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    shengyuModel.IsChangeMonth = "N";
                    shengyuModel.CityId = entity.CityID;
                    shengyuModel.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                    shengyuModel.CreatePerson = userName;
                    shengyuModel.CreateTime = DateTime.Now;
                    changeWageList.Add(shengyuModel);
                    //更新报增表工资
                    shengyu_EmployeeAdd.Wage = GZ_shengyu;
                    shengyu_EmployeeAdd.UpdatePerson = userName;
                    shengyu_EmployeeAdd.UpdateTime = DateTime.Now;
                    //更新中间表
                    var shengyu_Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == shengyu_EmployeeAdd.CompanyEmployeeRelationId && e.InsuranceKindId == shengyu_EmployeeAdd.InsuranceKindId && e.State == Enable);
                    if (shengyu_Middle.Count() > 0)
                    {
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(db, ZC_shengyu_ID, GZ_shengyu, 1);//公司基数
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(db, ZC_shengyu_ID, GZ_shengyu, 2);//个人基数
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(db, ZC_shengyu_ID, GZ_shengyu, 1);//公司比例
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(db, ZC_shengyu_ID, GZ_shengyu, 2);//个人比例
                        foreach (var order in shengyu_Middle)
                        {
                            order.CompanyBasePayment = JISHU_C;
                            order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, ZC_shengyu_ID);
                            order.EmployeeBasePayment = JISHU_P;
                            order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, ZC_shengyu_ID);
                            order.Remark = "调基";
                            order.UpdatePerson = userName;
                            order.UpdateTime = DateTime.Now;
                        }
                    }
                }
                #endregion

                #region 公积金
                if (entity.HousingFund_Wage != 0 && entity.HousingFund_Wage != null)
                {
                    EmployeeAdd gongjijin_EmployeeAdd = db.EmployeeAdd.Where(e => e.Id == gongjijinID).FirstOrDefault();
                    decimal GZ_gongjijin = (decimal)entity.HousingFund_Wage;//工资
                    int ZC_gongjijin_ID = (int)gongjijin_EmployeeAdd.PoliceInsuranceId;//政策id
                    //添加调基数据
                    EmployeeGoonPayment2 gongjijinModel = new EmployeeGoonPayment2();
                    gongjijinModel.NewWage = GZ_gongjijin;
                    gongjijinModel.OldWage = gongjijin_EmployeeAdd.Wage;
                    gongjijinModel.InsuranceMonth = DateTime.Now;
                    gongjijinModel.EmployeeAddId = gongjijinID;
                    gongjijinModel.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                    gongjijinModel.State = EmployeeGoonPayment2_STATUS.待员工客服确认.ToString();
                    gongjijinModel.IsChangeMonth = "N";
                    gongjijinModel.CityId = entity.CityID;
                    gongjijinModel.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                    gongjijinModel.CreatePerson = userName;
                    gongjijinModel.CreateTime = DateTime.Now;
                    changeWageList.Add(gongjijinModel);
                    //更新报增表工资
                    gongjijin_EmployeeAdd.Wage = GZ_gongjijin;
                    gongjijin_EmployeeAdd.UpdatePerson = userName;
                    gongjijin_EmployeeAdd.UpdateTime = DateTime.Now;
                    //更新中间表
                    var gongjijin_Middle = db.EmployeeMiddle.Where(e => e.CompanyEmployeeRelationId == gongjijin_EmployeeAdd.CompanyEmployeeRelationId && e.InsuranceKindId == gongjijin_EmployeeAdd.InsuranceKindId && e.State == Enable);
                    if (gongjijin_Middle.Count() > 0)
                    {
                        var JISHU_C = EmployeeAddRepository.Get_Jishu(db, ZC_gongjijin_ID, GZ_gongjijin, 1);//公司基数
                        var JISHU_P = EmployeeAddRepository.Get_Jishu(db, ZC_gongjijin_ID, GZ_gongjijin, 2);//个人基数
                        var PERCENT_C = EmployeeAddRepository.Get_BILI(db, ZC_gongjijin_ID, GZ_gongjijin, 1);//公司比例
                        var PERCENT_P = EmployeeAddRepository.Get_BILI(db, ZC_gongjijin_ID, GZ_gongjijin, 2);//个人比例
                        foreach (var order in gongjijin_Middle)
                        {
                            order.CompanyBasePayment = JISHU_C;
                            order.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(db, JISHU_C, PERCENT_C, 1, ZC_gongjijin_ID);
                            order.EmployeeBasePayment = JISHU_P;
                            order.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(db, JISHU_P, PERCENT_P, 1, ZC_gongjijin_ID);
                            order.Remark = "调基";
                            order.UpdatePerson = userName;
                            order.UpdateTime = DateTime.Now;
                        }
                    }
                }
                #endregion

                db.EmployeeGoonPayment2.AddRange(changeWageList);
                db.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        // 检查是否有险种正在调基处理中
        public string Verification(SysEntities db, AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID)
        {
            string[] arrayStatus = new string[] { 
                Common.EmployeeGoonPayment2_STATUS.待责任客服确认.ToString(),
                Common.EmployeeGoonPayment2_STATUS.待员工客服确认.ToString(),
                Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString(),
                Common.EmployeeGoonPayment2_STATUS.社保专员已提取.ToString()
            };
            StringBuilder Error = new StringBuilder();
            //养老
            if (yanglaoID != null)
            {
                var yanglaoList = db.EmployeeGoonPayment2.Where(e => e.EmployeeAddId == yanglaoID && arrayStatus.Contains(e.State));
                if (yanglaoList.Count() > 0)
                {
                    Error.Append("养老 ");
                }
            }
            //医疗
            if (yiliaoID != null)
            {
                var yiliaoList = db.EmployeeGoonPayment2.Where(e => e.EmployeeAddId == yiliaoID && arrayStatus.Contains(e.State));
                if (yiliaoList.Count() > 0)
                {
                    Error.Append("医疗 ");
                }
            }
            //生育
            if (shengyuID != null)
            {
                var shengyuList = db.EmployeeGoonPayment2.Where(e => e.EmployeeAddId == shengyuID && arrayStatus.Contains(e.State));
                if (shengyuList.Count() > 0)
                {
                    Error.Append("生育 ");
                }
            }
            //工伤
            if (gongshangID != null)
            {
                var gongshangList = db.EmployeeGoonPayment2.Where(e => e.EmployeeAddId == gongshangID && arrayStatus.Contains(e.State));
                if (gongshangList.Count() > 0)
                {
                    Error.Append("工伤 ");
                }
            }
            //失业
            if (shiyeID != null)
            {
                var shiyeList = db.EmployeeGoonPayment2.Where(e => e.EmployeeAddId == shiyeID && arrayStatus.Contains(e.State));
                if (shiyeList.Count() > 0)
                {
                    Error.Append("失业 ");
                }
            }
            //公积金
            if (gongjijinID != null)
            {
                var gongjijinList = db.EmployeeGoonPayment2.Where(e => e.EmployeeAddId == gongjijinID && arrayStatus.Contains(e.State));
                if (gongjijinList.Count() > 0)
                {
                    Error.Append("公积金 ");
                }
            }
            if (!string.IsNullOrEmpty(Error.ToString()))
            {
                Error.Append("正在调基处理中，暂不能处理！");
            }
            return Error.ToString();
        }
        #endregion

        #region 信伟青
        #region 查询社保专员列表
        /// <summary>
        /// 查询社保专员列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeGoonPayment2View> GetCommissionerList(SysEntities db, string CertificateNumber, int page, int rows, string search, out int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var emp = ent.Employee.Where(a => true);
                    var empadd = ent.EmployeeAdd.Where(a => true);
                    var emppay = ent.EmployeeGoonPayment2.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);
                    #region 条件
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
                            string[] CARD_ID_LIST = CertificateNumber.Split(Convert.ToChar(10));
                            List<string> CARDLIST = new List<string>();
                            for (int i = 0; i < CARD_ID_LIST.Length; i++)
                            {
                                CARDLIST.Add(CARD_ID_LIST[i]);
                                CARDLIST.Add(CardCommon.CardIDTo15(CARD_ID_LIST[i]));
                                CARDLIST.Add(CardCommon.CardIDTo18(CARD_ID_LIST[i]));
                            }
                            CARDLIST = CARDLIST.Distinct().ToList();
                            emp = emp.Where(o => CARDLIST.Contains(o.CertificateNumber));
                        }

                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            emppay = emppay.Where(a => states.Contains(a.State));
                        }
                        if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        {
                            string str = queryDic["InsuranceKinds"];
                            int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                            emppay = emppay.Where(a => Ids.Contains(a.InsuranceKindId));
                        }

                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            string str = queryDic["YearMonth"];
                            DateTime dtStart = Convert.ToDateTime(str.Substring(0, 4) + "/" + str.Substring(4, 2) + "/01");
                            DateTime dtEnd = dtStart.AddMonths(1);
                            emppay = emppay.Where(a => a.CreateTime >= dtStart && a.CreateTime < dtEnd);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                    }
                    #endregion
                    //调基表中员工客服已确认或者已提取的
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empadd on r.Id equals b.CompanyEmployeeRelationId
                                join d in emppay on b.Id equals d.EmployeeAddId
                                join c in com on r.CompanyId equals c.ID
                                join e in city on r.CityId equals e.Id
                                select new EmployeeGoonPayment2View()
                                {
                                    Id = d.Id,
                                    CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    EmployeeID = r.EmployeeId,
                                    InsuranceKindId = d.InsuranceKindId,
                                    InsuranceKindName = "",
                                    CompanyName = c.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = e.Name,
                                    YearMonth = d.YearMonth
                                }).ToList();

                    var dd = from b in list
                             group b by new { b.CompanyId, b.CompanyName, b.EmployeeID, b.Name, b.CertificateNumber, b.YearMonth, b.City, b.CompanyEmployeeRelationId } into g
                             select new EmployeeGoonPayment2View
                             {
                                 CompanyEmployeeRelationId = g.Key.CompanyEmployeeRelationId,
                                 CompanyId = g.Key.CompanyId,
                                 CompanyName = g.Key.CompanyName,
                                 CertificateNumber = g.Key.CertificateNumber,
                                 YearMonth = g.Key.YearMonth,
                                 Name = g.Key.Name,
                                 EmployeeID = g.Key.EmployeeID,
                                 City = g.Key.City,
                                 InsuranceKindName = string.Join(",", (g.Select(o => (Common.EmployeeAdd_InsuranceKindId)o.InsuranceKindId))),
                                 AddIds = string.Join(",", (g.Select(o => o.Id)))
                             };

                    //分页
                    count = 0;
                    count = dd.Count();
                    if (page > -1)
                    {
                        dd = dd.OrderBy(a => a.CompanyId).ThenBy(a => a.EmployeeID).Skip((page - 1) * rows).Take(rows);
                    }
                    return dd.ToList();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #region 社保专员提取调基数据
        /// <summary>
        /// 社保专员提取调基数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddExcelList(SysEntities db, int page, int rows, string search, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var emppay = ent.EmployeeGoonPayment2.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);

                    #region 查询条件
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
                            string[] numList = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            emp = emp.Where(a => numList.Contains(a.CertificateNumber));
                        }

                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            emppay = emppay.Where(a => states.Contains(a.State));
                        }
                        if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        {
                            string str = queryDic["InsuranceKinds"];
                            int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                            emppay = emppay.Where(a => Ids.Contains(a.InsuranceKindId));
                        }
                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            string str = queryDic["YearMonth"];
                            DateTime dtStart = Convert.ToDateTime(str.Substring(0, 4) + "/" + str.Substring(4, 2) + "/01");
                            DateTime dtEnd = dtStart.AddMonths(1);
                            emppay = emppay.Where(a => a.CreateTime >= dtStart && a.CreateTime < dtEnd);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        //if (queryDic.ContainsKey("CompanyCode") && !string.IsNullOrWhiteSpace(queryDic["CompanyCode"]))
                        //{
                        //    string str = queryDic["CompanyCode"];
                        //    com = com.Where(a => a.CompanyCode.Contains(str));
                        //}
                    }
                    #endregion

                    #region 数据调取
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join d in emppay on r.Id equals d.EmployeeAdd.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                select new EmployeeAddView()
                                {
                                    CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    CompanyCode = c.CompanyCode,
                                    CompanyName = c.CompanyName,
                                    CertificateNumber = a.CertificateNumber,
                                    Name = a.Name,
                                    City = e.Name,
                                    PoliceAccountNatureName = f.Name,
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
                    #endregion

                    #region 在内存中进行数据拼接

                    List<EmployeeAddView> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的险种信息全部查询并放入系统内存中;
                    var temp = (from b in emppay
                                join c in list on b.EmployeeAdd.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.EmployeeAdd.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.EmployeeAdd.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    c.CompanyEmployeeRelationId,//企业员工关系
                                    b.InsuranceKindId,//社保种类
                                    b.EmployeeAdd.PoliceInsuranceId,//社保政策
                                    b.YearMonth,//创建时间
                                    b.EmployeeAdd.InsuranceCode,//社保编号
                                    b.NewWage,//新工资
                                    d.Name,//社保名称
                                    b.State,//调基状态
                                    b.EmployeeAdd.StartTime,//起缴时间
                                    PoliceOperationName = e.Name,//政策手续名称
                                    b.InsuranceMonth,//社保月
                                    b.EmployeeAdd.IsIndependentAccount,//是否单立户
                                    b.CreateTime
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
                               select new EmployeeAddView()
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
                                   CompanyNumber_1 = b1 == null ? null : (decimal?)Get_Jishu(db, (int)b1.PoliceInsuranceId, (decimal)b1.NewWage, comMark),
                                   CompanyPercent_1 = b1 == null ? null : (decimal?)Get_BILI(db, (int)b1.PoliceInsuranceId, (decimal)b1.NewWage, comMark),
                                   EmployeeNumber_1 = b1 == null ? null : (decimal?)Get_Jishu(db, (int)b1.PoliceInsuranceId, (decimal)b1.NewWage, empMark),
                                   EmployeePercent_1 = b1 == null ? null : (decimal?)Get_BILI(db, (int)b1.PoliceInsuranceId, (decimal)b1.NewWage, empMark),
                                   State_1 = b1 == null ? null : b1.State,
                                   PoliceInsuranceName_1 = b1 == null ? null : b1.Name,
                                   YearMonth_1 = b1 == null ? null : b1.YearMonth,
                                   StartTime_1 = b1 == null ? null : b1.StartTime,
                                   Wage_1 = b1 == null ? null : b1.NewWage,
                                   PoliceOperationName_1 = b1 == null ? null : b1.PoliceOperationName,

                                   InsuranceMonth_1 = b1 == null ? null : b1.InsuranceMonth,
                                   IsIndependentAccount_1 = b1 == null ? null : b1.IsIndependentAccount,
                                   CreateTime_1 = b1 == null ? null : b1.CreateTime,

                                   InsuranceCode_2 = b2 == null ? null : b2.InsuranceCode,
                                   CompanyNumber_2 = b2 == null ? null : (decimal?)Get_Jishu(db, (int)b2.PoliceInsuranceId, (decimal)b2.NewWage, comMark),
                                   CompanyPercent_2 = b2 == null ? null : (decimal?)Get_BILI(db, (int)b2.PoliceInsuranceId, (decimal)b2.NewWage, comMark),
                                   EmployeeNumber_2 = b2 == null ? null : (decimal?)Get_Jishu(db, (int)b2.PoliceInsuranceId, (decimal)b2.NewWage, empMark),
                                   EmployeePercent_2 = b2 == null ? null : (decimal?)Get_BILI(db, (int)b2.PoliceInsuranceId, (decimal)b2.NewWage, empMark),
                                   State_2 = b2 == null ? null : b2.State,
                                   PoliceInsuranceName_2 = b2 == null ? null : b2.Name,
                                   YearMonth_2 = b2 == null ? null : b2.YearMonth,
                                   StartTime_2 = b2 == null ? null : b2.StartTime,
                                   Wage_2 = b2 == null ? null : b2.NewWage,
                                   PoliceOperationName_2 = b2 == null ? null : b2.PoliceOperationName,

                                   InsuranceMonth_2 = b2 == null ? null : b2.InsuranceMonth,
                                   IsIndependentAccount_2 = b2 == null ? null : b2.IsIndependentAccount,
                                   CreateTime_2 = b2 == null ? null : b2.CreateTime,

                                   InsuranceCode_3 = b3 == null ? null : b3.InsuranceCode,
                                   CompanyNumber_3 = b3 == null ? null : (decimal?)Get_Jishu(db, (int)b3.PoliceInsuranceId, (decimal)b3.NewWage, comMark),
                                   CompanyPercent_3 = b3 == null ? null : (decimal?)Get_BILI(db, (int)b3.PoliceInsuranceId, (decimal)b3.NewWage, comMark),
                                   EmployeeNumber_3 = b3 == null ? null : (decimal?)Get_Jishu(db, (int)b3.PoliceInsuranceId, (decimal)b3.NewWage, empMark),
                                   EmployeePercent_3 = b3 == null ? null : (decimal?)Get_BILI(db, (int)b3.PoliceInsuranceId, (decimal)b3.NewWage, empMark),
                                   State_3 = b3 == null ? null : b3.State,
                                   PoliceInsuranceName_3 = b3 == null ? null : b3.Name,
                                   YearMonth_3 = b3 == null ? null : b3.YearMonth,
                                   StartTime_3 = b3 == null ? null : b3.StartTime,
                                   Wage_3 = b3 == null ? null : b3.NewWage,
                                   PoliceOperationName_3 = b3 == null ? null : b3.PoliceOperationName,

                                   InsuranceMonth_3 = b3 == null ? null : b3.InsuranceMonth,
                                   IsIndependentAccount_3 = b3 == null ? null : b3.IsIndependentAccount,
                                   CreateTime_3 = b3 == null ? null : b3.CreateTime,

                                   InsuranceCode_4 = b4 == null ? null : b4.InsuranceCode,
                                   CompanyNumber_4 = b4 == null ? null : (decimal?)Get_Jishu(db, (int)b4.PoliceInsuranceId, (decimal)b4.NewWage, comMark),
                                   CompanyPercent_4 = b4 == null ? null : (decimal?)Get_BILI(db, (int)b4.PoliceInsuranceId, (decimal)b4.NewWage, comMark),
                                   EmployeeNumber_4 = b4 == null ? null : (decimal?)Get_Jishu(db, (int)b4.PoliceInsuranceId, (decimal)b4.NewWage, empMark),
                                   EmployeePercent_4 = b4 == null ? null : (decimal?)Get_BILI(db, (int)b4.PoliceInsuranceId, (decimal)b4.NewWage, empMark),
                                   State_4 = b4 == null ? null : b4.State,
                                   PoliceInsuranceName_4 = b4 == null ? null : b4.Name,
                                   YearMonth_4 = b4 == null ? null : b4.YearMonth,
                                   StartTime_4 = b4 == null ? null : b4.StartTime,
                                   Wage_4 = b4 == null ? null : b4.NewWage,
                                   PoliceOperationName_4 = b4 == null ? null : b4.PoliceOperationName,

                                   InsuranceMonth_4 = b4 == null ? null : b4.InsuranceMonth,
                                   IsIndependentAccount_4 = b4 == null ? null : b4.IsIndependentAccount,
                                   CreateTime_4 = b4 == null ? null : b4.CreateTime,


                                   InsuranceCode_5 = b5 == null ? null : b5.InsuranceCode,
                                   CompanyNumber_5 = b5 == null ? null : (decimal?)Get_Jishu(db, (int)b5.PoliceInsuranceId, (decimal)b5.NewWage, comMark),
                                   CompanyPercent_5 = b5 == null ? null : (decimal?)Get_BILI(db, (int)b5.PoliceInsuranceId, (decimal)b5.NewWage, comMark),
                                   EmployeeNumber_5 = b5 == null ? null : (decimal?)Get_Jishu(db, (int)b5.PoliceInsuranceId, (decimal)b5.NewWage, empMark),
                                   EmployeePercent_5 = b5 == null ? null : (decimal?)Get_BILI(db, (int)b5.PoliceInsuranceId, (decimal)b5.NewWage, empMark),
                                   State_5 = b5 == null ? null : b5.State,
                                   PoliceInsuranceName_5 = b5 == null ? null : b5.Name,
                                   YearMonth_5 = b5 == null ? null : b5.YearMonth,
                                   StartTime_5 = b5 == null ? null : b5.StartTime,
                                   Wage_5 = b5 == null ? null : b5.NewWage,
                                   PoliceOperationName_5 = b5 == null ? null : b5.PoliceOperationName,

                                   InsuranceMonth_5 = b5 == null ? null : b5.InsuranceMonth,
                                   IsIndependentAccount_5 = b5 == null ? null : b5.IsIndependentAccount,
                                   CreateTime_5 = b5 == null ? null : b5.CreateTime,


                                   InsuranceCode_6 = b6 == null ? null : b6.InsuranceCode,
                                   CompanyNumber_6 = b6 == null ? null : (decimal?)Get_Jishu(db, (int)b6.PoliceInsuranceId, (decimal)b6.NewWage, comMark),
                                   CompanyPercent_6 = b6 == null ? null : (decimal?)Get_BILI(db, (int)b6.PoliceInsuranceId, (decimal)b6.NewWage, comMark),
                                   EmployeeNumber_6 = b6 == null ? null : (decimal?)Get_Jishu(db, (int)b6.PoliceInsuranceId, (decimal)b6.NewWage, empMark),
                                   EmployeePercent_6 = b6 == null ? null : (decimal?)Get_BILI(db, (int)b6.PoliceInsuranceId, (decimal)b6.NewWage, empMark),
                                   State_6 = b6 == null ? null : b6.State,
                                   PoliceInsuranceName_6 = b6 == null ? null : b6.Name,
                                   YearMonth_6 = b6 == null ? null : b6.YearMonth,
                                   StartTime_6 = b6 == null ? null : b6.StartTime,
                                   Wage_6 = b6 == null ? null : b6.NewWage,
                                   PoliceOperationName_6 = b6 == null ? null : b6.PoliceOperationName,

                                   InsuranceMonth_6 = b6 == null ? null : b6.InsuranceMonth,
                                   IsIndependentAccount_6 = b6 == null ? null : b6.IsIndependentAccount,
                                   CreateTime_6 = b6 == null ? null : b6.CreateTime,



                                   InsuranceCode_7 = b7 == null ? null : b7.InsuranceCode,
                                   CompanyNumber_7 = b7 == null ? null : (decimal?)Get_Jishu(db, (int)b7.PoliceInsuranceId, (decimal)b7.NewWage, comMark),
                                   CompanyPercent_7 = b7 == null ? null : (decimal?)Get_BILI(db, (int)b7.PoliceInsuranceId, (decimal)b7.NewWage, comMark),
                                   EmployeeNumber_7 = b7 == null ? null : (decimal?)Get_Jishu(db, (int)b7.PoliceInsuranceId, (decimal)b7.NewWage, empMark),
                                   EmployeePercent_7 = b7 == null ? null : (decimal?)Get_BILI(db, (int)b7.PoliceInsuranceId, (decimal)b7.NewWage, empMark),
                                   State_7 = b7 == null ? null : b7.State,
                                   PoliceInsuranceName_7 = b7 == null ? null : b7.Name,
                                   YearMonth_7 = b7 == null ? null : b7.YearMonth,
                                   StartTime_7 = b7 == null ? null : b7.StartTime,
                                   Wage_7 = b7 == null ? null : b7.NewWage,
                                   PoliceOperationName_7 = b7 == null ? null : b7.PoliceOperationName,

                                   InsuranceMonth_7 = b7 == null ? null : b7.InsuranceMonth,
                                   IsIndependentAccount_7 = b7 == null ? null : b7.IsIndependentAccount,
                                   CreateTime_7 = b7 == null ? null : b7.CreateTime,


                                   InsuranceCode_8 = b8 == null ? null : b8.InsuranceCode,
                                   CompanyNumber_8 = b8 == null ? null : (decimal?)Get_Jishu(db, (int)b8.PoliceInsuranceId, (decimal)b8.NewWage, comMark),
                                   CompanyPercent_8 = b8 == null ? null : (decimal?)Get_BILI(db, (int)b8.PoliceInsuranceId, (decimal)b8.NewWage, comMark),
                                   EmployeeNumber_8 = b8 == null ? null : (decimal?)Get_Jishu(db, (int)b8.PoliceInsuranceId, (decimal)b8.NewWage, empMark),
                                   EmployeePercent_8 = b8 == null ? null : (decimal?)Get_BILI(db, (int)b8.PoliceInsuranceId, (decimal)b8.NewWage, empMark),
                                   State_8 = b8 == null ? null : b8.State,
                                   PoliceInsuranceName_8 = b8 == null ? null : b8.Name,
                                   YearMonth_8 = b8 == null ? null : b8.YearMonth,
                                   StartTime_8 = b8 == null ? null : b8.StartTime,
                                   Wage_8 = b8 == null ? null : b8.NewWage,
                                   PoliceOperationName_8 = b8 == null ? null : b8.PoliceOperationName,

                                   InsuranceMonth_8 = b8 == null ? null : b8.InsuranceMonth,
                                   IsIndependentAccount_8 = b8 == null ? null : b8.IsIndependentAccount,
                                   CreateTime_8 = b8 == null ? null : b8.CreateTime,

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

        #region 获取基数
        /// <summary>
        /// 获取基数
        /// </summary>
        /// <param name="SHEBAO"></param>
        /// <param name="POL_ID">政策ID</param>
        /// <param name="ZENG_TYPE">报增类型编号</param>
        /// <param name="GONGZI">工资</param>
        /// <param name="CorP">单位还是个人（1单位，2个人）</param>
        /// <returns></returns>
        public static decimal Get_Jishu(SysEntities SHEBAO, int POL_ID, decimal GONGZI, int CorP)
        {
            decimal JISHU = (decimal)0;
            decimal JISHU_C = GONGZI, JISHU_P = GONGZI;
            //查找政策表

            var BAOXIAN_PoliceInsurance = SHEBAO.PoliceInsurance.FirstOrDefault(o => o.Id == POL_ID);
            JISHU_C = (decimal)BAOXIAN_PoliceInsurance.CompanyLowestNumber > JISHU_C ? (decimal)BAOXIAN_PoliceInsurance.CompanyLowestNumber : JISHU_C;
            if (BAOXIAN_PoliceInsurance.CompanyHighestNumber != (decimal)0)
            {
                JISHU_C = JISHU_C > (decimal)BAOXIAN_PoliceInsurance.CompanyHighestNumber ? (decimal)BAOXIAN_PoliceInsurance.CompanyHighestNumber : JISHU_C;
            }
            JISHU_P = (decimal)BAOXIAN_PoliceInsurance.EmployeeLowestNumber > JISHU_P ? (decimal)BAOXIAN_PoliceInsurance.EmployeeLowestNumber : JISHU_P;
            if (BAOXIAN_PoliceInsurance.EmployeeHighestNumber != (decimal)0)
            {
                JISHU_P = JISHU_P > (decimal)BAOXIAN_PoliceInsurance.EmployeeHighestNumber ? (decimal)BAOXIAN_PoliceInsurance.EmployeeHighestNumber : JISHU_P;
            }

            if (CorP == 1)
            {
                JISHU = JISHU_C;
            }
            else
                JISHU = JISHU_P;
            return JISHU;
        }
        #endregion

        #region 获取比例
        /// <summary>
        /// 获取比例
        /// </summary>
        /// <param name="SHEBAO"></param>
        /// <param name="POL_ID">政策ID</param>
        /// <param name="ZENG_TYPE">报增类型编号</param>
        /// <param name="GONGZI">工资</param>
        /// <param name="CorP">单位还是个人（1单位，2个人）</param>
        /// <returns></returns>
        public static decimal Get_BILI(SysEntities SHEBAO, int POL_ID, decimal GONGZI, int CorP)
        {
            decimal BILI = (decimal)0;
            decimal PERCENT_C = (decimal)0, PERCENT_P = (decimal)0;
            //查找政策表
            var BAOXIAN_POLICY_PoliceInsurance = SHEBAO.PoliceInsurance.FirstOrDefault(o => o.Id == POL_ID);
            PERCENT_C = (decimal)BAOXIAN_POLICY_PoliceInsurance.CompanyPercent;
            PERCENT_P = (decimal)BAOXIAN_POLICY_PoliceInsurance.EmployeePercent;
            if (CorP == 1)
            {
                BILI = PERCENT_C;
            }
            else
                BILI = PERCENT_P;
            return BILI;
        }
        #endregion
        #endregion

        #region 社保调基查询
        /// <summary>
        /// 社保报增查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeGoonPayment2List(SysEntities db, int page, int rows, string search, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    #region 查询条件


                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeGoonPayment2.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);

                    var empAddLast = from a in empAdd
                                     group a by new
                                     {
                                         a.EmployeeAdd.CompanyEmployeeRelationId,
                                         a.InsuranceKindId,
                                     } into g
                                     select new
                                     {
                                         CompanyEmployeeRelationId = g.Key.CompanyEmployeeRelationId,
                                         InsuranceKindId = g.Key.CompanyEmployeeRelationId,
                                         Id = g.Max(a => a.Id)
                                     };
                    empAdd = from a in empAdd
                             join b in empAddLast on a.Id equals b.Id
                             select a;

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
                            string[] numList = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            emp = emp.Where(a => numList.Contains(a.CertificateNumber));
                        }

                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            empAdd = empAdd.Where(a => states.Contains(a.State));
                        }
                        if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        {
                            string str = queryDic["InsuranceKinds"];
                            int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                            empAdd = empAdd.Where(a => Ids.Contains(a.InsuranceKindId));
                        }
                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            int str = Convert.ToInt32(queryDic["YearMonth"]);
                            empAdd = empAdd.Where(a => a.YearMonth == str);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        if (queryDic.ContainsKey("CompanyCode") && !string.IsNullOrWhiteSpace(queryDic["CompanyCode"]))
                        {
                            string str = queryDic["CompanyCode"];
                            com = com.Where(a => a.CompanyCode.Contains(str));
                        }
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                    }
                    #endregion

                    #region 数据调取




                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.EmployeeAdd.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                select new EmployeeAddView()
                                {
                                    CompanyEmployeeRelationId = b.EmployeeAdd.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    CompanyCode = c.CompanyCode,
                                    CompanyName = c.CompanyName,
                                    CertificateNumber = a.CertificateNumber,
                                    Name = a.Name,
                                    City = e.Name,
                                    PoliceAccountNatureName = f.Name,
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
                    #endregion

                    #region 在内存中进行数据拼接

                    List<EmployeeAddView> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                join c in list on b.EmployeeAdd.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.EmployeeAdd.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.EmployeeAdd.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    b.EmployeeAdd.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    b.EmployeeAdd.PoliceInsuranceId,
                                    b.YearMonth,
                                    b.EmployeeAdd.InsuranceCode,
                                    b.EmployeeAdd.Wage,
                                    d.Name,
                                    b.State,
                                    b.EmployeeAdd.StartTime,
                                    PoliceOperationName = e.Name,
                                    b.InsuranceMonth,
                                    b.EmployeeAdd.IsIndependentAccount,
                                    b.CreateTime
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
                               select new EmployeeAddView()
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
                                   CompanyNumber_1 = b1 == null ? null : (decimal?)Get_Jishu(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, comMark),
                                   CompanyPercent_1 = b1 == null ? null : (decimal?)Get_BILI(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, comMark),
                                   EmployeeNumber_1 = b1 == null ? null : (decimal?)Get_Jishu(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, empMark),
                                   EmployeePercent_1 = b1 == null ? null : (decimal?)Get_BILI(db, (int)b1.PoliceInsuranceId, (decimal)b1.Wage, empMark),
                                   State_1 = b1 == null ? null : b1.State,
                                   PoliceInsuranceName_1 = b1 == null ? null : b1.Name,
                                   YearMonth_1 = b1 == null ? null : b1.YearMonth,
                                   StartTime_1 = b1 == null ? null : b1.StartTime,
                                   Wage_1 = b1 == null ? null : b1.Wage,
                                   PoliceOperationName_1 = b1 == null ? null : b1.PoliceOperationName,

                                   InsuranceMonth_1 = b1 == null ? null : b1.InsuranceMonth,
                                   IsIndependentAccount_1 = b1 == null ? null : b1.IsIndependentAccount,
                                   CreateTime_1 = b1 == null ? null : b1.CreateTime,

                                   InsuranceCode_2 = b2 == null ? null : b2.InsuranceCode,
                                   CompanyNumber_2 = b2 == null ? null : (decimal?)Get_Jishu(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, comMark),
                                   CompanyPercent_2 = b2 == null ? null : (decimal?)Get_BILI(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, comMark),
                                   EmployeeNumber_2 = b2 == null ? null : (decimal?)Get_Jishu(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, empMark),
                                   EmployeePercent_2 = b2 == null ? null : (decimal?)Get_BILI(db, (int)b2.PoliceInsuranceId, (decimal)b2.Wage, empMark),
                                   State_2 = b2 == null ? null : b2.State,
                                   PoliceInsuranceName_2 = b2 == null ? null : b2.Name,
                                   YearMonth_2 = b2 == null ? null : b2.YearMonth,
                                   StartTime_2 = b2 == null ? null : b2.StartTime,
                                   Wage_2 = b2 == null ? null : b2.Wage,
                                   PoliceOperationName_2 = b2 == null ? null : b2.PoliceOperationName,

                                   InsuranceMonth_2 = b2 == null ? null : b2.InsuranceMonth,
                                   IsIndependentAccount_2 = b2 == null ? null : b2.IsIndependentAccount,
                                   CreateTime_2 = b2 == null ? null : b2.CreateTime,

                                   InsuranceCode_3 = b3 == null ? null : b3.InsuranceCode,
                                   CompanyNumber_3 = b3 == null ? null : (decimal?)Get_Jishu(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, comMark),
                                   CompanyPercent_3 = b3 == null ? null : (decimal?)Get_BILI(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, comMark),
                                   EmployeeNumber_3 = b3 == null ? null : (decimal?)Get_Jishu(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, empMark),
                                   EmployeePercent_3 = b3 == null ? null : (decimal?)Get_BILI(db, (int)b3.PoliceInsuranceId, (decimal)b3.Wage, empMark),
                                   State_3 = b3 == null ? null : b3.State,
                                   PoliceInsuranceName_3 = b3 == null ? null : b3.Name,
                                   YearMonth_3 = b3 == null ? null : b3.YearMonth,
                                   StartTime_3 = b3 == null ? null : b3.StartTime,
                                   Wage_3 = b3 == null ? null : b3.Wage,
                                   PoliceOperationName_3 = b3 == null ? null : b3.PoliceOperationName,

                                   InsuranceMonth_3 = b3 == null ? null : b3.InsuranceMonth,
                                   IsIndependentAccount_3 = b3 == null ? null : b3.IsIndependentAccount,
                                   CreateTime_3 = b3 == null ? null : b3.CreateTime,

                                   InsuranceCode_4 = b4 == null ? null : b4.InsuranceCode,
                                   CompanyNumber_4 = b4 == null ? null : (decimal?)Get_Jishu(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, comMark),
                                   CompanyPercent_4 = b4 == null ? null : (decimal?)Get_BILI(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, comMark),
                                   EmployeeNumber_4 = b4 == null ? null : (decimal?)Get_Jishu(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, empMark),
                                   EmployeePercent_4 = b4 == null ? null : (decimal?)Get_BILI(db, (int)b4.PoliceInsuranceId, (decimal)b4.Wage, empMark),
                                   State_4 = b4 == null ? null : b4.State,
                                   PoliceInsuranceName_4 = b4 == null ? null : b4.Name,
                                   YearMonth_4 = b4 == null ? null : b4.YearMonth,
                                   StartTime_4 = b4 == null ? null : b4.StartTime,
                                   Wage_4 = b4 == null ? null : b4.Wage,
                                   PoliceOperationName_4 = b4 == null ? null : b4.PoliceOperationName,

                                   InsuranceMonth_4 = b4 == null ? null : b4.InsuranceMonth,
                                   IsIndependentAccount_4 = b4 == null ? null : b4.IsIndependentAccount,
                                   CreateTime_4 = b4 == null ? null : b4.CreateTime,


                                   InsuranceCode_5 = b5 == null ? null : b5.InsuranceCode,
                                   CompanyNumber_5 = b5 == null ? null : (decimal?)Get_Jishu(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, comMark),
                                   CompanyPercent_5 = b5 == null ? null : (decimal?)Get_BILI(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, comMark),
                                   EmployeeNumber_5 = b5 == null ? null : (decimal?)Get_Jishu(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, empMark),
                                   EmployeePercent_5 = b5 == null ? null : (decimal?)Get_BILI(db, (int)b5.PoliceInsuranceId, (decimal)b5.Wage, empMark),
                                   State_5 = b5 == null ? null : b5.State,
                                   PoliceInsuranceName_5 = b5 == null ? null : b5.Name,
                                   YearMonth_5 = b5 == null ? null : b5.YearMonth,
                                   StartTime_5 = b5 == null ? null : b5.StartTime,
                                   Wage_5 = b5 == null ? null : b5.Wage,
                                   PoliceOperationName_5 = b5 == null ? null : b5.PoliceOperationName,

                                   InsuranceMonth_5 = b5 == null ? null : b5.InsuranceMonth,
                                   IsIndependentAccount_5 = b5 == null ? null : b5.IsIndependentAccount,
                                   CreateTime_5 = b5 == null ? null : b5.CreateTime,


                                   InsuranceCode_6 = b6 == null ? null : b6.InsuranceCode,
                                   CompanyNumber_6 = b6 == null ? null : (decimal?)Get_Jishu(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, comMark),
                                   CompanyPercent_6 = b6 == null ? null : (decimal?)Get_BILI(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, comMark),
                                   EmployeeNumber_6 = b6 == null ? null : (decimal?)Get_Jishu(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, empMark),
                                   EmployeePercent_6 = b6 == null ? null : (decimal?)Get_BILI(db, (int)b6.PoliceInsuranceId, (decimal)b6.Wage, empMark),
                                   State_6 = b6 == null ? null : b6.State,
                                   PoliceInsuranceName_6 = b6 == null ? null : b6.Name,
                                   YearMonth_6 = b6 == null ? null : b6.YearMonth,
                                   StartTime_6 = b6 == null ? null : b6.StartTime,
                                   Wage_6 = b6 == null ? null : b6.Wage,
                                   PoliceOperationName_6 = b6 == null ? null : b6.PoliceOperationName,

                                   InsuranceMonth_6 = b6 == null ? null : b6.InsuranceMonth,
                                   IsIndependentAccount_6 = b6 == null ? null : b6.IsIndependentAccount,
                                   CreateTime_6 = b6 == null ? null : b6.CreateTime,



                                   InsuranceCode_7 = b7 == null ? null : b7.InsuranceCode,
                                   CompanyNumber_7 = b7 == null ? null : (decimal?)Get_Jishu(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, comMark),
                                   CompanyPercent_7 = b7 == null ? null : (decimal?)Get_BILI(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, comMark),
                                   EmployeeNumber_7 = b7 == null ? null : (decimal?)Get_Jishu(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, empMark),
                                   EmployeePercent_7 = b7 == null ? null : (decimal?)Get_BILI(db, (int)b7.PoliceInsuranceId, (decimal)b7.Wage, empMark),
                                   State_7 = b7 == null ? null : b7.State,
                                   PoliceInsuranceName_7 = b7 == null ? null : b7.Name,
                                   YearMonth_7 = b7 == null ? null : b7.YearMonth,
                                   StartTime_7 = b7 == null ? null : b7.StartTime,
                                   Wage_7 = b7 == null ? null : b7.Wage,
                                   PoliceOperationName_7 = b7 == null ? null : b7.PoliceOperationName,

                                   InsuranceMonth_7 = b7 == null ? null : b7.InsuranceMonth,
                                   IsIndependentAccount_7 = b7 == null ? null : b7.IsIndependentAccount,
                                   CreateTime_7 = b7 == null ? null : b7.CreateTime,


                                   InsuranceCode_8 = b8 == null ? null : b8.InsuranceCode,
                                   CompanyNumber_8 = b8 == null ? null : (decimal?)Get_Jishu(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, comMark),
                                   CompanyPercent_8 = b8 == null ? null : (decimal?)Get_BILI(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, comMark),
                                   EmployeeNumber_8 = b8 == null ? null : (decimal?)Get_Jishu(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, empMark),
                                   EmployeePercent_8 = b8 == null ? null : (decimal?)Get_BILI(db, (int)b8.PoliceInsuranceId, (decimal)b8.Wage, empMark),
                                   State_8 = b8 == null ? null : b8.State,
                                   PoliceInsuranceName_8 = b8 == null ? null : b8.Name,
                                   YearMonth_8 = b8 == null ? null : b8.YearMonth,
                                   StartTime_8 = b8 == null ? null : b8.StartTime,
                                   Wage_8 = b8 == null ? null : b8.Wage,
                                   PoliceOperationName_8 = b8 == null ? null : b8.PoliceOperationName,

                                   InsuranceMonth_8 = b8 == null ? null : b8.InsuranceMonth,
                                   IsIndependentAccount_8 = b8 == null ? null : b8.IsIndependentAccount,
                                   CreateTime_8 = b8 == null ? null : b8.CreateTime,

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

