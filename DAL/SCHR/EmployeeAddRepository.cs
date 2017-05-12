using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
using Langben.DAL.Model;
namespace Langben.DAL
{
    /// <summary>
    /// 增加员工
    /// </summary>
    public partial class EmployeeAddRepository : BaseRepository<EmployeeAdd>, IDisposable
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
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);
                    var user_zr = ent.CRM_CompanyToBranch.Where(a => a.UserID_ZR != null);
                    var user_yg = ent.UserCityCompany.Where(a => a.UserID_YG != null);
                    var comR = ent.CRM_CompanyToBranch.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
                        {
                            string str = queryDic["Name"];
                            emp = emp.Where(a => a.Name.Contains(str));
                        }

                        //if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
                        //{
                        //    string str = queryDic["CertificateNumber"];
                        //    emp = emp.Where(a => a.CertificateNumber == str);
                        //}
                        if (!string.IsNullOrEmpty(CertificateNumber))
                        {
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
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                        if (queryDic.ContainsKey("UserID_ZR") && !string.IsNullOrWhiteSpace(queryDic["UserID_ZR"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_ZR"]);
                            com = from c in com
                                  join g in ent.CRM_CompanyToBranch.Where(a => a.UserID_ZR == str) on c.ID equals g.CRM_Company_ID
                                  select c;
                        }
                        if (queryDic.ContainsKey("UserID_YG") && !string.IsNullOrWhiteSpace(queryDic["UserID_YG"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_YG"]);
                            user_yg = user_yg.Where(a => a.UserID_YG == uid);
                            com = from c in com
                                  join g in ent.UserCityCompany.Where(a => a.UserID_YG == uid) on c.ID equals g.CompanyId
                                  select c;
                            city = from a in city
                                   join b in ent.ORG_UserCity.Where(a => a.UserID == uid) on a.Id equals b.CityId
                                   select a;
                        }

                        if (queryDic.ContainsKey("UserID_SB") && !string.IsNullOrWhiteSpace(queryDic["UserID_SB"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_SB"]);
                            empAdd = from a in empAdd
                                     join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                     join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                     join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                     select a;
                        }

                        if (queryDic.ContainsKey("UserID_Supplier") && !string.IsNullOrWhiteSpace(queryDic["UserID_Supplier"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_Supplier"]);
                            empAdd = from a in empAdd
                                     join s in ent.Supplier on a.SuppliersId equals s.Id
                                     where s.CustomerServiceId == str
                                     select a;
                        }
                        if (queryDic.ContainsKey("SuppliersId") && !string.IsNullOrWhiteSpace(queryDic["SuppliersId"]))
                        {
                            int str = Convert.ToInt32(queryDic["SuppliersId"]);
                            empAdd = from a in empAdd
                                     where a.SuppliersId == str
                                     select a;
                        }
                    }
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                join s in ent.Supplier on b.SuppliersId equals s.Id into JoinedEmpDept
                                from sup in JoinedEmpDept.DefaultIfEmpty()
                                select new EmployeeApprove()
                               {
                                   CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                   CompanyId = r.CompanyId,
                                   CompanyName = c.CompanyName,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   City = e.Name,
                                   CityID = r.CityId,
                                   PoliceAccountNature = f.Name,
                                   YearMonth = b.YearMonth,
                                   SupplierID = b.SuppliersId ?? 0,
                                   SupplierName = sup.Name ?? ""
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


                    var aaa = empAdd.ToList();

                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from a in empAdd
                                join b in list on a.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                                //join d in ent.InsuranceKind on a.InsuranceKindId equals d.Id
                                where a.YearMonth == b.YearMonth
                                select new
                                {
                                    a.CompanyEmployeeRelationId,
                                    a.InsuranceKindId,
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
        #endregion

        #region 查询待责任客服审核列表
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
                    var empAdd = ent.EmployeeAdd.Where(a => true);
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
                            empAdd = empAdd.Where(a => states.Contains(a.State));
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
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                    }
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                select new EmployeeApprove()
                               {
                                   CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
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
                    var temp = (from a in empAdd
                                join b in list on a.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                                //join d in ent.InsuranceKind on a.InsuranceKindId equals d.Id
                                where a.YearMonth == b.YearMonth
                                select new
                                {
                                    a.CompanyEmployeeRelationId,
                                    a.InsuranceKindId,
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
        #endregion

        #region 审责任客服审核
        /// <summary>
        /// 审责任客服审核
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="StateOld">原状态</param>
        /// <param name="StateNew">新审核状态</param>
        /// <returns></returns>
        public bool EmployeeAddApproved(int?[] ApprovedId, string StateOld, string StateNew)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    if (ApprovedId == null || ApprovedId.Count() <= 0)
                    {
                        return false;
                    }
                    //var updateEmpAdd = ent.EmployeeAdd.Where(a => ApprovedId.Contains(a.CompanyEmployeeRelationId) && a.State == StateOld);
                    var updateEmpAdd = ent.EmployeeAdd.Where(a => ApprovedId.Contains(a.Id) && a.State == StateOld);
                    if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                    {
                        foreach (var item in updateEmpAdd)
                        {
                            item.State = StateNew;
                        }
                        ent.SaveChanges();
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
            //var BAOXIAN_JIGOU_SBTYPE = SHEBAO.BAOXIAN_JIGOU_SBTYPE.FirstOrDefault(o => o.BX_SB_TYPE == ZENG_TYPE);
            ////先判断此报增类型基数是否和政策表中的基数一致
            //if (BAOXIAN_JIGOU_SBTYPE.EQUAL_BASE == true)
            //{
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

            //基数四舍五入

            var ff = SHEBAO.PoliceInsurance_Four_Five.FirstOrDefault(x => x.PoliceInsuranceID == POL_ID);
            if (ff != null)
            {
                //企业
                if (CorP == 1)
                {
                    if (ff.C_BaseIS == (int)Common.IS_Four_Five.四舍五入)
                    {
                        JISHU_C = Common.MathEx.RoundEx(JISHU_C, ff.B_BaseDigit ?? 2);

                    }
                    if (ff.C_BaseIS == (int)Common.IS_Four_Five.进位)
                    {
                        //  JISHU_C = Math.Round(JISHU_C, ff.C_BaseDigit ?? 2, MidpointRounding.AwayFromZero);
                        JISHU_C = Common.MathEx.RoundUp(JISHU_C, ff.C_BaseDigit ?? 2);

                    }
                    if (ff.C_BaseIS == (int)Common.IS_Four_Five.不进位)
                    {
                        // JISHU_C = Math.Round(JISHU_C, ff.C_BaseDigit ?? 2, MidpointRounding.AwayFromZero);
                        JISHU_C = Common.MathEx.RoundDown(JISHU_C, ff.C_BaseDigit ?? 2);


                    }

                }
                //个人
                if (CorP == 2)
                {
                    if (ff.B_BaseIS == (int)Common.IS_Four_Five.四舍五入)
                    {
                        //  JISHU_P = Math.Round(JISHU_P, ff.B_BaseDigit ?? 2, MidpointRounding.AwayFromZero);
                        JISHU_P = Common.MathEx.RoundEx(JISHU_P, ff.B_BaseDigit ?? 2);
                    }
                    if (ff.C_BaseIS == (int)Common.IS_Four_Five.进位)
                    {
                        JISHU_P = Common.MathEx.RoundUp(JISHU_P, ff.C_BaseDigit ?? 2);

                    }
                    if (ff.C_BaseIS == (int)Common.IS_Four_Five.不进位)
                    {

                        JISHU_P = Common.MathEx.RoundDown(JISHU_P, ff.C_BaseDigit ?? 2);


                    }

                }

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

        #region 企业承担费用计算
        /// <summary>
        /// 企业承担费用计算
        /// </summary>
        /// <param name="jishu">企业基数</param>
        /// <param name="bili">企业比例</param>
        /// <param name="mouth">要承担</param>
        /// <param name="zhengceID"></param>
        /// <returns></returns>
        public static decimal Get_CompanyPayment(SysEntities SHEBAO, decimal jishu, decimal bili, int mouth, int zhengceID)
        {
            decimal d = 0;
            var ff = SHEBAO.PoliceInsurance_Four_Five.FirstOrDefault(x => x.PoliceInsuranceID == zhengceID);
            if (ff != null)
            {
                if (ff.C_BearIS == (int)Common.IS_Four_Five.四舍五入)
                {


                    if (ff.C_BearMonth == (int)Common.IS_BearMonth.先乘以月)
                    {
                        d = jishu * bili * mouth;
                        d = Common.MathEx.RoundEx(d, ff.C_BearDigit ?? 2);
                    }
                    else
                    {
                        d = jishu * bili;
                        d = d = Common.MathEx.RoundEx(d, ff.C_BearDigit ?? 2) * mouth;
                    }

                }
                else if (ff.C_BearIS == (int)Common.IS_Four_Five.不进位)
                {


                    if (ff.C_BearMonth == (int)Common.IS_BearMonth.先乘以月)
                    {
                        d = jishu * bili * mouth;
                        d = Common.MathEx.RoundDown(d, ff.C_BearDigit ?? 2);
                    }
                    else
                    {
                        d = jishu * bili;
                        d = d = Common.MathEx.RoundDown(d, ff.C_BearDigit ?? 2) * mouth;
                    }

                }
                else if (ff.C_BearIS == (int)Common.IS_Four_Five.进位)
                {


                    if (ff.C_BearMonth == (int)Common.IS_BearMonth.先乘以月)
                    {
                        d = jishu * bili * mouth;
                        d = Common.MathEx.RoundUp(d, ff.C_BearDigit ?? 2);
                    }
                    else
                    {
                        d = jishu * bili;
                        d = d = Common.MathEx.RoundUp(d, ff.C_BearDigit ?? 2) * mouth;
                    }

                }
                else
                {
                    d = jishu * bili * mouth;
                    d = Common.MathEx.RoundEx(d, 2);
                }


            }
            else
            {
                d = jishu * bili * mouth;
                d = Common.MathEx.RoundEx(d, 2);

            }
            return d;
        }

        #endregion

        #region 个人承担费用计算
        /// <summary>
        /// 企业承担费用计算
        /// </summary>
        /// <param name="jishu">企业基数</param>
        /// <param name="bili">企业比例</param>
        /// <param name="mouth">要承担</param>
        /// <param name="zhengceID"></param>
        /// <returns></returns>
        public static decimal Get_EmployeePayment(SysEntities SHEBAO, decimal jishu, decimal bili, int mouth, int zhengceID)
        {
            decimal d = 0;
            var ff = SHEBAO.PoliceInsurance_Four_Five.FirstOrDefault(x => x.PoliceInsuranceID == zhengceID);
            if (ff != null)
            {
                if (ff.B_BearIS == (int)Common.IS_Four_Five.四舍五入)
                {


                    if (ff.B_BearMonth == (int)Common.IS_BearMonth.后乘以月)
                    {
                        d = jishu * bili * mouth;
                        d = Math.Round(d, ff.B_BearDigit ?? 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        d = jishu * bili;
                        d = Math.Round(d, ff.B_BearDigit ?? 2, MidpointRounding.AwayFromZero) * mouth;
                    }

                }
                else
                {
                    d = jishu * bili * mouth;
                    d = Math.Round(d, 2, MidpointRounding.AwayFromZero);

                }


            }
            else
            {
                d = jishu * bili * mouth;
                d = Math.Round(d, 2, MidpointRounding.AwayFromZero);

            }
            return d;
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

        #region 查询待客服经理分配列表
        /// <summary>
        /// 查询待客服经理分配列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAllot> GetAllotList(SysEntities db, int page, int rows, string search, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var user_zr = ent.CRM_CompanyToBranch.Where(a => a.UserID_ZR != null);
                    var user_yg = ent.UserCityCompany.Where(a => a.UserID_YG != null);
                    var city = ent.City.Where(a => true);
                    var empAddServer = ent.EmployeeAdd.Where(a => true);
                    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                    if (queryDic != null && queryDic.Count > 0)
                    {
                        if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                        {
                            string str = queryDic["State"];
                            string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            empAdd = empAdd.Where(a => states.Contains(a.State));
                        }

                        if (queryDic.ContainsKey("ServerState") && !string.IsNullOrWhiteSpace(queryDic["ServerState"]))
                        {
                            string str = queryDic["ServerState"];
                            empAddServer = empAddServer.Where(a => a.State == str);
                        }

                        if (queryDic.ContainsKey("CompanyName") && !string.IsNullOrWhiteSpace(queryDic["CompanyName"]))
                        {
                            string str = queryDic["CompanyName"];
                            com = com.Where(a => a.CompanyName.Contains(str));
                        }
                        //未分配、已分配条件查询
                        if (queryDic.ContainsKey("AllotState") && !string.IsNullOrWhiteSpace(queryDic["AllotState"]))
                        {
                            string str = queryDic["AllotState"];
                            if (str.Equals(Common.AllotState.未分配.ToString()))
                            {
                                com = com.Where(a => !user_yg.Select(b => b.CompanyId).Contains(a.ID));

                            }
                            else if (str.Equals(Common.AllotState.已分配.ToString()))
                            {
                                if (queryDic.ContainsKey("UserID_YG") && !string.IsNullOrWhiteSpace(queryDic["UserID_YG"]))
                                {
                                    int uid = Convert.ToInt32(queryDic["UserID_YG"]);
                                    user_yg = user_yg.Where(a => a.UserID_YG == uid);
                                }
                                com = com.Where(a => user_yg.Select(b => b.CompanyId).Contains(a.ID));

                            }

                        }


                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                        if (queryDic.ContainsKey("UserID") && !string.IsNullOrWhiteSpace(queryDic["UserID"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID"]);
                            city = from a in city
                                   join b in ent.ORG_UserCity.Where(a => a.UserID == uid) on a.Id equals b.CityId
                                   select a;
                        }
                    }

                    var comEmpAdd = (from r in ent.CompanyEmployeeRelation
                                     join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                     join c in com on r.CompanyId equals c.ID
                                     //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                     join e in city on r.CityId equals e.Id
                                     //join f in userCity on e.Id equals f.CityId
                                     select new
                                     {
                                         CompanyId = r.CompanyId,
                                         CompanyName = c.CompanyName,
                                         CityId = e.Id,
                                         City = e.Name,
                                         EmployeeId = r.EmployeeId,
                                     }).Distinct();

                    var listAdd = from a in comEmpAdd
                                  group a by new { a.CompanyId, a.CompanyName, a.City, a.CityId } into b
                                  select new
                                  {
                                      CompanyId = b.Key.CompanyId,
                                      CompanyName = b.Key.CompanyName,
                                      City = b.Key.City,
                                      CityId = b.Key.CityId,
                                      EmployeeAddSum = b.Count()
                                  };
                    var comEmpServer = (from r in ent.CompanyEmployeeRelation
                                        join b in empAddServer on r.Id equals b.CompanyEmployeeRelationId
                                        join c in com on r.CompanyId equals c.ID
                                        //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                        join e in city on r.CityId equals e.Id
                                        select new
                                        {
                                            CompanyId = r.CompanyId,
                                            EmployeeId = r.EmployeeId,
                                        }).Distinct();

                    var listServer = from a in comEmpServer
                                     group a by a.CompanyId into b
                                     select new
                                     {
                                         CompanyId = b.Key,
                                         EmployeeServerSum = b.Count()
                                     };

                    var list = from a in listAdd
                               join b2 in listServer on a.CompanyId equals b2.CompanyId into bb
                               from b in bb.DefaultIfEmpty()
                               join f2 in user_zr on a.CompanyId equals f2.CRM_Company_ID into ff
                               from f in ff.DefaultIfEmpty()
                               join g2 in user_yg on new { a.CompanyId, a.CityId } equals new { g2.CompanyId, g2.CityId } into gg
                               from g in gg.DefaultIfEmpty()
                               select new EmployeeAllot()
                               {
                                   CompanyId = a.CompanyId,
                                   CompanyName = a.CompanyName,
                                   City = a.City,
                                   EmployeeAddSum = a.EmployeeAddSum,
                                   EmployeeServerSum = b.EmployeeServerSum == null ? 0 : b.EmployeeServerSum,
                                   UserID_YG = g.UserID_YG,
                                   UserID_ZR = f.UserID_ZR
                               };
                    count = 0;
                    if (list != null && list.Count() >= 1)
                    {
                        count = list.Count();
                        if (page > -1)
                        {
                            list = list.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyId).Skip((page - 1) * rows).Take(rows);
                        }
                    }

                    return list.ToList();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }
        #endregion

        #region 缴纳地户口性质
        /// <summary>
        /// 缴纳地户口性质 敬
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id">缴纳地</param>
        /// <returns></returns>
        public IQueryable<idname__> getPoliceAccountNatureid(SysEntities db, string id)
        {
            var w = db.City.FirstOrDefault(p => p.Id == id);
            if (w != null)
            {
                return (from c in w.PoliceAccountNature

                        select new idname__ { ID = c.Id, Name = c.Name }).AsQueryable();
            }
            else
            {
                return (from c in db.PoliceAccountNature

                        select new idname__ { ID = c.Id, Name = c.Name }).AsQueryable();
            }

        }
        #endregion

        #region 社保种类
        /// <summary>
        /// 社保种类 敬
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id">缴纳地</param>
        /// <returns></returns>
        public IQueryable<idname__> getInsuranceKindid(SysEntities db, string id)
        {
            string InsuranceKindStatus = Status.启用.ToString();
            return from c in db.InsuranceKind
                   where c.City == id && c.State == InsuranceKindStatus
                   select new idname__ { ID = c.Id, Name = c.Name };

        }
        #endregion

        #region 政策手续
        /// <summary>
        /// 政策手续 敬
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id">社保种类</param>
        /// <returns></returns>
        public IQueryable<idname__> getPoliceOperationid(SysEntities db, int id)
        {
            string InsuranceKindStatus = Status.启用.ToString();
            string Style = PoliceOperation_Style.报增.ToString();
            var w = db.InsuranceKind.FirstOrDefault(p => p.Id == id && p.State == InsuranceKindStatus);
            if (w != null)
            {
                return (from c in w.PoliceOperation
                        where c.Style == Style
                        select new idname__ { ID = c.Id, Name = c.Name }).AsQueryable();
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 社保报增查询
        /// <summary>
        /// 社保报增查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddList(SysEntities db, int page, int rows, string search, List<ORG_User> userList, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    #region 查询条件


                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);

                    var empAddLast = from a in empAdd
                                     group a by new
                                     {
                                         a.CompanyEmployeeRelationId,
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

                    List<int> userIdList = userList.Select(a => a.ID).ToList();
                    List<int?> userIdList2 = (from a in userList
                                              select new
                                              {
                                                  ID = (int?)a.ID
                                              }).Select(a => a.ID).ToList();

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
                        if (queryDic.ContainsKey("UserID_YG") && !string.IsNullOrWhiteSpace(queryDic["UserID_YG"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_YG"]);
                            com = from c in com
                                  join g in ent.UserCityCompany.Where(a => a.UserID_YG == uid) on c.ID equals g.CompanyId
                                  select c;
                            city = from a in city
                                   join b in ent.ORG_UserCity.Where(a => a.UserID == uid) on a.Id equals b.CityId
                                   select a;
                        }
                        if (queryDic.ContainsKey("UserID_ZR") && !string.IsNullOrWhiteSpace(queryDic["UserID_ZR"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_ZR"]);
                            com = from c in com
                                  join g in ent.CRM_CompanyToBranch.Where(a => a.UserID_ZR == uid) on c.ID equals g.CRM_Company_ID
                                  select c;
                        }
                        if (queryDic.ContainsKey("UserID_SB") && !string.IsNullOrWhiteSpace(queryDic["UserID_SB"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_SB"]);
                            empAdd = from a in empAdd
                                     join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                     join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                     join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == uid) on c.Id equals d.InsuranceKindId
                                     select a;
                        }
                    }
                    #endregion

                    #region 数据调取




                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                join g2 in ent.UserCityCompany.Where(a => userIdList2.Contains(a.UserID_YG)) on new { CityId = e.Id, r.CompanyId } equals new { g2.CityId, g2.CompanyId } into gg
                                from g in gg.DefaultIfEmpty()           //数据有无代表缴纳地是否属于此用户
                                join h2 in ent.CRM_CompanyToBranch.Where(a => userIdList2.Contains(a.UserID_ZR)) on c.ID equals h2.CRM_Company_ID into hh
                                from h in hh.DefaultIfEmpty()          //数据有无代表客户是否属于此用户

                                join i in ent.PoliceInsurance on b.PoliceInsuranceId equals i.Id
                                join j in ent.InsuranceKind on i.InsuranceKindId equals j.Id
                                join k2 in ent.ORG_UserInsuranceKind.Where(a => userIdList.Contains(a.UserID)) on j.Id equals k2.InsuranceKindId into kk
                                from k in kk.DefaultIfEmpty()          //数据有无代表险种是否属于此用户   
                                where (g.ID != null || h.ID != null || k.ID != null)

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
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    b.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    b.PoliceInsuranceId,
                                    b.YearMonth,
                                    b.InsuranceCode,
                                    b.Wage,
                                    d.Name,
                                    State = b.Remark == null ? b.State : b.State + ":" + b.Remark,
                                    b.StartTime,
                                    PoliceOperationName = e.Name,
                                    b.InsuranceMonth,
                                    b.IsIndependentAccount,
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
                               join bb9 in temp.Where(a => a.InsuranceKindId == 9) on a.CompanyEmployeeRelationId equals bb9.CompanyEmployeeRelationId into bt9
                               from b9 in bt9.DefaultIfEmpty()
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
                                                       + (b8 == null ? "" : b8.Id.ToString() + ",")
                                                     + (b9 == null ? "" : b9.Id.ToString()),

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


                                   InsuranceCode_9 = b9 == null ? null : b9.InsuranceCode,
                                   CompanyNumber_9 = b9 == null ? null : (decimal?)Get_Jishu(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, comMark),
                                   CompanyPercent_9 = b9 == null ? null : (decimal?)Get_BILI(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, comMark),
                                   EmployeeNumber_9 = b9 == null ? null : (decimal?)Get_Jishu(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, empMark),
                                   EmployeePercent_9 = b9 == null ? null : (decimal?)Get_BILI(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, empMark),
                                   State_9 = b9 == null ? null : b9.State,
                                   PoliceInsuranceName_9 = b9 == null ? null : b9.Name,
                                   YearMonth_9 = b9 == null ? null : b9.YearMonth,
                                   StartTime_9 = b9 == null ? null : b9.StartTime,
                                   Wage_9 = b9 == null ? null : b9.Wage,
                                   PoliceOperationName_9 = b9 == null ? null : b9.PoliceOperationName,

                                   InsuranceMonth_9 = b9 == null ? null : b9.InsuranceMonth,
                                   IsIndependentAccount_9 = b9 == null ? null : b9.IsIndependentAccount,
                                   CreateTime_9 = b9 == null ? null : b9.CreateTime,

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

        #region 社保报增查询 用以导出Excel 王帅
        /// <summary>
        /// 社保报增查询
        /// </summary>
        /// <param name="db"></param>
        /// <param name="search">查询条件</param>
        /// <param name="userList">用户信息</param>
        /// <param name="count">输出数量</param>
        /// <returns></returns>
        public List<EmployeeAddView> GetEmployeeAddListForExcel(SysEntities db, string search, List<ORG_User> userList, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    #region 查询条件  


                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);

                    var empAddLast = from a in empAdd
                                     group a by new
                                     {
                                         a.CompanyEmployeeRelationId,
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

                    List<int> userIdList = userList.Select(a => a.ID).ToList();
                    List<int?> userIdList2 = (from a in userList
                                              select new
                                              {
                                                  ID = (int?)a.ID
                                              }).Select(a => a.ID).ToList();

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
                        if (queryDic.ContainsKey("UserID_YG") && !string.IsNullOrWhiteSpace(queryDic["UserID_YG"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_YG"]);
                            com = from c in com
                                  join g in ent.UserCityCompany.Where(a => a.UserID_YG == uid) on c.ID equals g.CompanyId
                                  select c;
                            city = from a in city
                                   join b in ent.ORG_UserCity.Where(a => a.UserID == uid) on a.Id equals b.CityId
                                   select a;
                        }
                        if (queryDic.ContainsKey("UserID_ZR") && !string.IsNullOrWhiteSpace(queryDic["UserID_ZR"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_ZR"]);
                            com = from c in com
                                  join g in ent.CRM_CompanyToBranch.Where(a => a.UserID_ZR == uid) on c.ID equals g.CRM_Company_ID
                                  select c;
                        }
                        if (queryDic.ContainsKey("UserID_SB") && !string.IsNullOrWhiteSpace(queryDic["UserID_SB"]))
                        {
                            int uid = Convert.ToInt32(queryDic["UserID_SB"]);
                            empAdd = from a in empAdd
                                     join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                     join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                     join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == uid) on c.Id equals d.InsuranceKindId
                                     select a;
                        }
                    }
                    #endregion

                    #region 数据调取




                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                join g2 in ent.UserCityCompany.Where(a => userIdList2.Contains(a.UserID_YG)) on new { CityId = e.Id, r.CompanyId } equals new { g2.CityId, g2.CompanyId } into gg
                                from g in gg.DefaultIfEmpty()           //数据有无代表缴纳地是否属于此用户
                                join h2 in ent.CRM_CompanyToBranch.Where(a => userIdList2.Contains(a.UserID_ZR)) on c.ID equals h2.CRM_Company_ID into hh
                                from h in hh.DefaultIfEmpty()          //数据有无代表客户是否属于此用户

                                join i in ent.PoliceInsurance on b.PoliceInsuranceId equals i.Id
                                join j in ent.InsuranceKind on i.InsuranceKindId equals j.Id
                                join k2 in ent.ORG_UserInsuranceKind.Where(a => userIdList.Contains(a.UserID)) on j.Id equals k2.InsuranceKindId into kk
                                from k in kk.DefaultIfEmpty()          //数据有无代表险种是否属于此用户   
                                where (g.ID != null || h.ID != null || k.ID != null)

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
                    //if (list != null && list.Count() >= 1)
                    //{
                    //    count = list.Count();
                    //    if (page > -1)
                    //    {
                    //        list = list.OrderBy(a => a.CompanyId).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                    //    }
                    //}
                    #endregion

                    #region 在内存中进行数据拼接

                    List<EmployeeAddView> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    b.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    b.PoliceInsuranceId,
                                    b.YearMonth,
                                    b.InsuranceCode,
                                    b.Wage,
                                    d.Name,
                                    State = b.Remark == null ? b.State : b.State + ":" + b.Remark,
                                    b.StartTime,
                                    PoliceOperationName = e.Name,
                                    b.InsuranceMonth,
                                    b.IsIndependentAccount,
                                    b.CreateTime
                                }).ToList();

                    int comMark = 1;
                    int empMark = 2;
                    //查询分值机构
                    var CompanyEmployeeRelationn=db.CompanyEmployeeRelation.Where(o=>true);//公司关系表
                    var CRM_CompanyToBranchn=db.CRM_CompanyToBranch.Where(o=>true);//公司对应责任客服
                    var ORG_Departmentn=db.ORG_Department.Where(o=>true);//公司部门
                    var ORG_Usern=db.ORG_User.Where(o=>true);//人员信息表

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
                               join bb9 in temp.Where(a => a.InsuranceKindId == 9) on a.CompanyEmployeeRelationId equals bb9.CompanyEmployeeRelationId into bt9
                               from b9 in bt9.DefaultIfEmpty()
                               join coms in CompanyEmployeeRelationn on a.CompanyEmployeeRelationId equals coms.Id
                               join branch in CRM_CompanyToBranchn on coms.CompanyId equals branch.CRM_Company_ID
                               join user in ORG_Usern on branch.UserID_ZR equals user.ID
                               join depart in ORG_Departmentn on user.ORG_Department_ID equals depart.ID
                               join gsmc in ORG_Departmentn on depart.BranchID equals gsmc.ID



                            
                               select new EmployeeAddView()
                               {
                                   Operator_CompanyName = gsmc.DepartmentName,
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
                                                       + (b8 == null ? "" : b8.Id.ToString() + ",")
                                                     + (b9 == null ? "" : b9.Id.ToString()),

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


                                   InsuranceCode_9 = b9 == null ? null : b9.InsuranceCode,
                                   CompanyNumber_9 = b9 == null ? null : (decimal?)Get_Jishu(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, comMark),
                                   CompanyPercent_9 = b9 == null ? null : (decimal?)Get_BILI(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, comMark),
                                   EmployeeNumber_9 = b9 == null ? null : (decimal?)Get_Jishu(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, empMark),
                                   EmployeePercent_9 = b9 == null ? null : (decimal?)Get_BILI(db, (int)b9.PoliceInsuranceId, (decimal)b9.Wage, empMark),
                                   State_9 = b9 == null ? null : b9.State,
                                   PoliceInsuranceName_9 = b9 == null ? null : b9.Name,
                                   YearMonth_9 = b9 == null ? null : b9.YearMonth,
                                   StartTime_9 = b9 == null ? null : b9.StartTime,
                                   Wage_9 = b9 == null ? null : b9.Wage,
                                   PoliceOperationName_9 = b9 == null ? null : b9.PoliceOperationName,

                                   InsuranceMonth_9 = b9 == null ? null : b9.InsuranceMonth,
                                   IsIndependentAccount_9 = b9 == null ? null : b9.IsIndependentAccount,
                                   CreateTime_9 = b9 == null ? null : b9.CreateTime,

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

        #region  社保单人报增身份证号多行查询

        /// <summary>
        ///  社保单人报增身份证号多行查询
        /// </summary> 
        /// <param name="db">数据访问上下文</param>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="cardIds">身份证号（可多条根据换行符分割）</param> 
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="total">结果集的总数</param>
        /// <returns></returns>
        public List<Employee> GetEmployeeList(SysEntities db, int zrUserId, string cardIds, int page, int rows, ref int total)
        {
            List<Employee> view = new List<Employee>();
            //获取员工姓名查询员工信息
            IQueryable<Employee> employee = db.Employee.Where(o => true);

            if (!string.IsNullOrEmpty(cardIds))
            {
                string[] CARD_ID_LIST = cardIds.Split(Convert.ToChar(10));
                List<string> CARDLIST = new List<string>();
                //var CARD_ID_LIST = CARD_ID.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < CARD_ID_LIST.Length; i++)
                {
                    CARDLIST.Add(CARD_ID_LIST[i]);
                    CARDLIST.Add(CardCommon.CardIDTo15(CARD_ID_LIST[i]));
                    CARDLIST.Add(CardCommon.CardIDTo18(CARD_ID_LIST[i]));
                }
                CARDLIST = CARDLIST.Distinct().ToList();
                employee = employee.Where(o => CARDLIST.Contains(o.CertificateNumber));
            }
            total = 0;
            if (employee.Any())
            {
                total = employee.Count();
                if (page > -1)
                {
                    employee = employee.OrderBy(a => a.Id).Skip((page - 1) * rows).Take(rows);
                }
            }
            view = employee.ToList();


            return view;
        }

        #endregion

        #region 企业初始绑定
        /// <summary>
        /// 企业初始绑定 敬
        /// </summary> 
        /// <returns></returns>
        public IQueryable<idname__> getCompanyList(SysEntities db)
        {

            return from c in db.CRM_Company
                   select new idname__ { ID = c.ID, Name = c.CompanyName };

        }
        #endregion

        #region 缴纳地初始绑定
        /// <summary>
        /// 缴纳地初始绑定 敬
        /// </summary> 
        /// <returns></returns>
        public IQueryable<idname__> getCitylist(SysEntities db)
        {

            return from c in db.City
                   select new idname__ { Cityid = c.Id, Name = c.Name };

        }
        #endregion

        #region 查询社保专员列表
        /// <summary>
        /// 查询社保专员列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeApprove> GetCommissionerList(SysEntities db, string CertificateNumber, int page, int rows, string search, out int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeAdd.Where(a => true);
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
                        if (queryDic.ContainsKey("CityId") && !string.IsNullOrWhiteSpace(queryDic["CityId"]))
                        {
                            string str = queryDic["CityId"];
                            city = city.Where(a => a.Id == str);
                        }
                    }
                    //员工企业关系表
                    //var CompanyEmployeeRelationlist = (from a in emp
                    //                                   join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                    //                                   join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                    //                                   join c in com on r.CompanyId equals c.ID
                    //                                   //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                    //                                   join e in city on r.CityId equals e.Id
                    //                                   join f in ent.PoliceAccountNature on b.PoliceAccountNatureId equals f.Id
                    //                                   select new EmployeeApprove()
                    //                                   {
                    //                                       CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                    //                                       CompanyId = r.CompanyId,
                    //                                       Employee_ID = r.EmployeeId,

                    //                                       CompanyName = c.CompanyName,
                    //                                       Name = a.Name,
                    //                                       CertificateNumber = a.CertificateNumber,
                    //                                       City = e.Name,
                    //                                       PoliceAccountNature = f.Name,
                    //                                       YearMonth = b.YearMonth
                    //                                   }).Distinct();
                    //报增表中员工客服已确认或者已提取的
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                                select new EmployeeApprove()
                                {
                                    Id = b.Id,
                                    CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    Employee_ID = r.EmployeeId,
                                    InsuranceKindid = (int)b.InsuranceKindId,
                                    CompanyName = c.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = e.Name,
                                    PoliceAccountNature = f.Name,
                                    YearMonth = b.YearMonth
                                }).ToList();
                    //费用表中符合条件数据
                    int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                    int[] ZENG_STATUS = new int[] { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已支付 };
                    var COST_CostTable = ent.COST_CostTable.Where(a => true);
                    var list1 = (from a in COST_CostTable
                                 join b in ent.COST_CostTableInsurance on a.ID equals b.COST_CostTable_ID
                                 where ZENG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle
                                 select new EmployeeApprove
                                 {
                                     CompanyId = a.CRM_Company_ID,
                                     Employee_ID = b.Employee_ID,
                                     InsuranceKindid = b.CostType
                                 }).ToList();//费用表
                    //报增表与费用表联合查询
                    var uuuu = (from a in list
                                join b in list1 on new { a.CompanyId, a.Employee_ID, a.InsuranceKindid } equals new { b.CompanyId, b.Employee_ID, b.InsuranceKindid }
                                select new EmployeeApprove
                                {
                                    Id = a.Id,
                                    CompanyEmployeeRelationId = a.CompanyEmployeeRelationId,
                                    CompanyId = a.CompanyId,
                                    Employee_ID = a.Employee_ID,
                                    InsuranceKindid = (int)a.InsuranceKindid,
                                    CompanyName = a.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = a.City,
                                    PoliceAccountNature = a.Name,
                                    YearMonth = a.YearMonth
                                });
                    var dd = from b in uuuu
                             group b by new { b.CompanyId, b.CompanyName, b.Employee_ID, b.Name, b.CertificateNumber, b.YearMonth, b.City, b.CompanyEmployeeRelationId } into g
                             select new EmployeeApprove
                             {
                                 CompanyEmployeeRelationId = g.Key.CompanyEmployeeRelationId,
                                 CompanyId = g.Key.CompanyId,
                                 CompanyName = g.Key.CompanyName,
                                 CertificateNumber = g.Key.CertificateNumber,
                                 YearMonth = g.Key.YearMonth,
                                 Name = g.Key.Name,
                                 Employee_ID = g.Key.Employee_ID,
                                 City = g.Key.City,
                                 InsuranceKindname = string.Join(",", (g.Select(o => (Common.EmployeeAdd_InsuranceKindId)o.InsuranceKindid))),
                                 // InsuranceKindname = (string.Join(",", (g.Select(o => (Common.EmployeeAdd_InsuranceKindId)o.InsuranceKindid)))).Substring(0, (string.Join(",", (g.Select(o => (Common.EmployeeAdd_InsuranceKindId)o.InsuranceKindid)))).Length - 1),
                                 AddIds = string.Join(",", (g.Select(o => o.Id)))
                             };

                    //分页
                    count = 0;
                    count = dd.Count();
                    if (page > -1)
                    {
                        dd = dd.OrderBy(a => a.CompanyId).ThenBy(a => a.Employee_ID).Skip((page - 1) * rows).Take(rows);
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

        #region 社保专员提取报增数据
        /// <summary>
        /// 社保专员提取报增数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddExcelList(SysEntities db, int page, int rows, string search, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    #region 查询条件


                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);

                    var empAddLast = from a in empAdd
                                     group a by new
                                     {
                                         a.CompanyEmployeeRelationId,
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

                    //费用表中符合条件数据
                    int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                    int[] ZENG_STATUS = new int[] { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已支付 };
                    var COST_CostTable = ent.COST_CostTable.Where(a => true);
                    var list1 = (from a in COST_CostTable
                                 join b in ent.COST_CostTableInsurance on a.ID equals b.COST_CostTable_ID
                                 where ZENG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle
                                 select new
                                 {
                                     CompanyId = (int?)a.CRM_Company_ID,
                                     EmployeeId = b.Employee_ID,
                                     InsuranceKindId = (int?)b.CostType
                                 });//费用表

                    //通过匹配费用表同报增表找出已支付和待核销的报增信息
                    empAdd = from a in empAdd
                             join b in ent.CompanyEmployeeRelation on a.CompanyEmployeeRelationId equals b.Id
                             join c in list1 on new { a.InsuranceKindId, b.CompanyId, b.EmployeeId } equals new { c.InsuranceKindId, c.CompanyId, c.EmployeeId }
                             select a;


                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
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
                    //List<EmployeeAddView> appList1 = uuuu.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                //   join f in appList1  on b.Id equals f.Id
                                join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    b.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    b.PoliceInsuranceId,
                                    b.YearMonth,
                                    b.InsuranceCode,
                                    b.Wage,
                                    d.Name,
                                    b.State,
                                    b.StartTime,
                                    PoliceOperationName = e.Name,
                                    b.InsuranceMonth,
                                    b.IsIndependentAccount,
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

        #region 社保政策校验


        public List<EmployeeAddCheckModel> EmployeeAddCkeck(string City, int PoliceAccountNatureId, int? PoliceOperationId, int? PoliceInsuranceId, string InsuranceKind)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    var city = ent.City.Where(a => a.Id == City);

                    PoliceAccountNature pan = city.FirstOrDefault().PoliceAccountNature.Where(a => a.Id == PoliceAccountNatureId).FirstOrDefault();
                    int? panId = 0;
                    if (pan != null)
                    {
                        panId = pan.Id;
                    }
                    var insKind = ent.InsuranceKind.Where(a => a.Name == InsuranceKind);
                    var po = insKind.FirstOrDefault().PoliceOperation.Where(a => a.Id == PoliceOperationId).FirstOrDefault();
                    int? poId = 0;

                    if (po != null)
                    {
                        poId = po.Id;
                    }

                    var pi = ent.PoliceInsurance.Where(a => a.Id == PoliceInsuranceId);

                    var ppr = ent.PoliceOperationPoliceInsurancePoliceAccountNature.Where(a => a.PoliceAccountNatureId == PoliceAccountNatureId && a.PoliceInsuranceId == PoliceInsuranceId && a.PoliceOperationId == PoliceOperationId);



                    var list = (from a in city
                                join b2 in insKind on a.Id equals b2.City into bb
                                from b in bb.DefaultIfEmpty()
                                join c2 in pi on b.Id equals c2.InsuranceKindId into cc
                                from c in cc.DefaultIfEmpty()
                                join f2 in ppr on new { PoliceOperationId = poId, PoliceInsuranceId = c.Id, PoliceAccountNatureId = panId } equals new { PoliceOperationId = (int?)f2.PoliceOperationId, f2.PoliceInsuranceId, PoliceAccountNatureId = (int?)f2.PoliceAccountNatureId }
                                into ff
                                from f in ff.DefaultIfEmpty()
                                select new EmployeeAddCheckModel
                                {
                                    CityId = a.Id,
                                    PoliceAccountNatureId = panId,
                                    PoliceInsuranceId = c.Id,
                                    PoliceOperationId = f.PoliceOperationId
                                }).Distinct();
                    return list.ToList();

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        //新
        #region 社保专员提取报增数据
        /// <summary>
        /// 社保专员提取报增数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddExcelList1(SysEntities db, int page, int rows, string search, ref int count)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    #region 查询条件
                    var emp = ent.Employee.Where(a => true);
                    var empAdd = ent.EmployeeAdd.Where(a => true);
                    var com = ent.CRM_Company.Where(a => true);
                    var city = ent.City.Where(a => true);

                    var empAddLast = from a in empAdd
                                     group a by new
                                     {
                                         a.CompanyEmployeeRelationId,
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
                        if (queryDic.ContainsKey("UserID_SB") && !string.IsNullOrWhiteSpace(queryDic["UserID_SB"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_SB"]);
                            empAdd = from a in empAdd
                                     join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                     join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                     join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                     select a;
                        }
                    }
                    #endregion

                    #region 数据调取
                    ////费用表中符合条件数据
                    //int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                    //int[] ZENG_STATUS = new int[] { (int)Common.COST_Table_Status.待核销, (int)Common.COST_Table_Status.已支付 };
                    //var COST_CostTable = ent.COST_CostTable.Where(a => true);
                    //var list1 = (from a in COST_CostTable
                    //             join b in ent.COST_CostTableInsurance on a.ID equals b.COST_CostTable_ID
                    //             where ZENG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle
                    //             select new
                    //             {
                    //                 CompanyId = (int?)a.CRM_Company_ID,
                    //                 EmployeeId = b.Employee_ID,
                    //                 InsuranceKindId = (int?)b.CostType
                    //             });//费用表

                    ////通过匹配费用表同报增表找出已支付和待核销的报增信息
                    //empAdd = from a in empAdd
                    //         join b in ent.CompanyEmployeeRelation on a.CompanyEmployeeRelationId equals b.Id
                    //         join c in list1 on new { a.InsuranceKindId, b.CompanyId, b.EmployeeId } equals new { c.InsuranceKindId, c.CompanyId, c.EmployeeId }
                    //         select a;


                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
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
                                    CityID = e.Id,
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
                    //List<EmployeeAddView> appList1 = uuuu.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                //   join f in appList1  on b.Id equals f.Id
                                join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on b.PoliceOperationId equals e.Id
                                select new
                                {
                                    b.Id,
                                    b.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    b.PoliceInsuranceId,
                                    b.YearMonth,
                                    b.InsuranceCode,
                                    b.Wage,
                                    d.Name,
                                    b.State,
                                    b.StartTime,
                                    PoliceOperationName = e.Name,
                                    b.InsuranceMonth,
                                    b.IsIndependentAccount,
                                    b.CreateTime
                                }).ToList();

                    int comMark = 1;
                    int empMark = 2;
                    appList = (from a in appList
                               join bb1 in temp on a.CompanyEmployeeRelationId equals bb1.CompanyEmployeeRelationId into bt1
                               from b1 in bt1.DefaultIfEmpty()

                               select new EmployeeAddView()
                               {
                                   CompanyEmployeeRelationId = a.CompanyEmployeeRelationId,
                                   CompanyCode = a.CompanyCode,
                                   CompanyName = a.CompanyName,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   InsuranceKinds = b1.InsuranceKindId == null ? "" : b1.InsuranceKindId.ToString(),
                                   City = a.City,
                                   CityID = a.CityID,
                                   PoliceAccountNatureName = a.PoliceAccountNatureName,
                                   Station = a.Station,
                                   AddIds = b1.Id == null ? "" : b1.Id.ToString(),
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


        #region 社保失败
        public string ChangeServicecharge(SysEntities SysEntitiesO2O, EmployeeAdd item, string message, string LoginName)
        {
            try
            {
                string[] all_STATUS = new string[]{
                        Common.EmployeeAdd_State.待责任客服确认.ToString(),   Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                        Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString(),  Common.EmployeeAdd_State.社保专员已提取.ToString(),
                         Common.EmployeeAdd_State.申报成功.ToString(),
                        };
                string enable = Status.启用.ToString();


                var Model = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(o => o.Id == item.Id);

                Model.State = EmployeeAdd_State.申报失败.ToString();
                Model.Remark = message;
                Model.UpdateTime = DateTime.Now;
                Model.UpdatePerson = LoginName;
                var updateEmpAddmiddle = SysEntitiesO2O.EmployeeMiddle.Where(a => a.InsuranceKindId == item.InsuranceKindId && a.CompanyEmployeeRelationId == item.CompanyEmployeeRelationId && a.State == enable && (a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴));

                if (updateEmpAddmiddle != null && updateEmpAddmiddle.Count() >= 1)
                {
                    foreach (EmployeeMiddle itemn in updateEmpAddmiddle)
                    {
                        itemn.State = Status.停用.ToString();
                        itemn.UpdateTime = DateTime.Now;
                        itemn.UpdatePerson = LoginName;
                        SysEntitiesO2O.SaveChanges();


                    }
                    #region 失败是删除服务费
                    var EmployeeAddList = SysEntitiesO2O.EmployeeAdd.Where(a => a.CompanyEmployeeRelationId == item.CompanyEmployeeRelationId && all_STATUS.Contains(a.State));
                    if (EmployeeAddList.Count() <= 0)
                    {
                        var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(a => a.Id == item.CompanyEmployeeRelationId && a.State == "在职");
                        if (CompanyEmployeeRelation != null)
                        {
                            CompanyEmployeeRelation.State = "离职";
                        }
                        #region 添加服务费

                        #region 得出服务费

                        var aa = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(ce => ce.Id == item.CompanyEmployeeRelationId);
                        decimal payService_One = 0;
                        decimal payService_OneBJ = 0;
                        int count = 0;
                        //先找出企业交社保的人数
                        //var companyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.Where(ce => ce.State == "在职" && ce.CompanyId == aa.CompanyId);
                        //if (companyEmployeeRelation.Count() > 0)
                        //{
                        //    count = companyEmployeeRelation.Count() - 1;
                        //}
                        CRM_CompanyRepository api = new CRM_CompanyRepository();
                        count = api.getEmployee(SysEntitiesO2O, item.CompanyEmployeeRelation.CompanyId, item.YearMonth);

                        int[] cclpStatus = new int[] { (int)Common.Status.启用, (int)Common.Status.修改中 };
                        //找出单人服务费
                        var cRM_CompanyLadderPrice = SysEntitiesO2O.CRM_CompanyLadderPrice.Where(cclp => cclp.CRM_Company_ID == aa.CompanyId && cclpStatus.Contains(cclp.Status) && cclp.BeginLadder <= count && cclp.EndLadder >= count);
                        if (cRM_CompanyLadderPrice.Count() > 0)
                        {
                            payService_One = cRM_CompanyLadderPrice.OrderByDescending(o => o.EndLadder).FirstOrDefault().SinglePrice;

                        }
                        decimal payService_All = 0;
                        bool zhengHu = false;
                        //找出整户服务费
                        var cRM_CompanyPrice = SysEntitiesO2O.CRM_CompanyPrice.Where(ccp => cclpStatus.Contains(ccp.Status) && ccp.CRM_Company_ID == aa.CompanyId);
                        if (cRM_CompanyPrice.Count() > 0)
                        {
                            payService_All = cRM_CompanyPrice.FirstOrDefault().LowestPrice.Value;
                            payService_OneBJ = cRM_CompanyPrice.FirstOrDefault().AddPrice.Value;
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
                        #endregion
                        #region 得出需要修改的人和公司全部人
                        string[] ZENG_STATUS = new string[]{
                                Common.EmployeeAdd_State.待责任客服确认.ToString(),   Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                                Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString(),  Common.EmployeeAdd_State.社保专员已提取.ToString(),
                                 Common.EmployeeAdd_State.申报成功.ToString()   
                                
                        };

                        //var CompanyEmployeeRelationlist = from a in SysEntitiesO2O.EmployeeMiddle.Where(em => em.StartDate <= item.YearMonth && em.EndedDate >= item.YearMonth && em.State == enable)
                        //                                  join b in SysEntitiesO2O.CompanyEmployeeRelation.Where(x => x.CompanyId == item.CompanyEmployeeRelation.CompanyId) on a.CompanyEmployeeRelationId equals b.Id
                        //                                  where a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴
                        //                                  group new { b } by new
                        //                                  {
                        //                                      Employee_ID = b.EmployeeId,
                        //                                      CompanyId = (int)b.CompanyId,
                        //                                      YearMonth = a.UseBetween,
                        //                                  }
                        //                                      into s

                        //                                      select new
                        //                                      {
                        //                                          Employee_ID = s.Key.Employee_ID,

                        //                                          CompanyId = s.Key.CompanyId,
                        //                                          YearMonth = s.Key.YearMonth,

                        //                                      };
                        int[] emploees = api.getEmployeeIDs(SysEntitiesO2O, item.CompanyEmployeeRelation.CompanyId, item.YearMonth);
                        var CompanyEmployeeRelationlist = (from ce in SysEntitiesO2O.CompanyEmployeeRelation.Where(ce => ce.State == "在职")
                                                           join e in SysEntitiesO2O.EmployeeAdd on ce.Id equals e.CompanyEmployeeRelationId
                                                           where ce.CompanyId == aa.CompanyId && ZENG_STATUS.Contains(e.State) && emploees.Contains(ce.EmployeeId ?? 0)
                                                           select new
                                                           {
                                                               CompanyId = (int)ce.CompanyId,
                                                               Employee_ID = ce.EmployeeId,
                                                               YearMonth = e.YearMonth
                                                           }).Distinct().ToList();

                        #endregion


                        #endregion
                        #region 修改费用服务费表中的正常服务费
                        int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                        int[] FEIYONG_STATUS = new int[] { (int)Common.COST_Table_Status.财务作废, (int)Common.COST_Table_Status.客户作废, (int)Common.COST_Table_Status.责任客服作废 };
                        var COST_CostTable = SysEntitiesO2O.COST_CostTable.Where(a => true);
                        foreach (var item3 in CompanyEmployeeRelationlist)
                        {
                            var list1 = (from a in COST_CostTable
                                         join b in SysEntitiesO2O.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                         where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID == item.CompanyEmployeeRelation.EmployeeId
                                         select b).FirstOrDefault();//费用表
                            if (list1 != null)
                            {
                                if (zhengHu == true)//整户服务费
                                {
                                    list1.ServiceCoset = 0;

                                    var list2 = from a in COST_CostTable
                                                join b in SysEntitiesO2O.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID != item.CompanyEmployeeRelation.EmployeeId
                                                select b;//
                                    if (list2.Count() > 0)
                                    {
                                        foreach (var item4 in list2)
                                        {
                                            item4.ServiceCoset = 0;
                                        }
                                        list2.FirstOrDefault().ServiceCoset = payService_All;
                                    }
                                }
                                else//单人服务费
                                {
                                    list1.ServiceCoset = 0;
                                    var list2 = from a in COST_CostTable
                                                join b in SysEntitiesO2O.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID != item.CompanyEmployeeRelation.EmployeeId
                                                select b;//
                                    if (list2.Count() > 0)
                                    {
                                        foreach (var item4 in list2)
                                        {


                                            item4.ServiceCoset = payService_One;


                                        }
                                    }
                                }
                            }

                        }
                        #endregion
                        #region  修改补缴服务费
                        //取中间表的补缴数据
                        string Middlestate = Common.Status.启用.ToString();
                        var CompanyEmployeeRelationlistMiddle =
                            (from em in SysEntitiesO2O.EmployeeMiddle.Where(em => em.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴)
                             join cer in SysEntitiesO2O.CompanyEmployeeRelation on em.CompanyEmployeeRelationId equals cer.Id
                             join e in SysEntitiesO2O.Employee on cer.EmployeeId equals e.Id
                             where cer.CompanyId == aa.CompanyId && em.State == Middlestate && em.InsuranceKindId != item.InsuranceKindId
                             select new
                             {
                                 StartDate = em.StartDate,
                                 Employee_ID = e.Id,
                                 CompanyId = cer.CompanyId,
                                 PaymentMonth = em.PaymentMonth,

                             }).GroupBy(o => new { o.Employee_ID, o.CompanyId, o.PaymentMonth, o.StartDate }).Select(o => new
                             {
                                 StartDate = o.Key.StartDate,
                                 ServiceCoset = (decimal)o.Max(m => m.PaymentMonth) * payService_OneBJ,
                                 Employee_ID = o.Key.Employee_ID,
                                 CompanyId = aa.CompanyId,
                                 PaymentMonth = o.Key.PaymentMonth,
                             }).ToList();
                        #region 修改费用服务费表中的补缴服务费
                        int bujiaoPaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                        foreach (var item3 in CompanyEmployeeRelationlistMiddle)
                        {
                            var listooo = (from a in COST_CostTable
                                           join b in SysEntitiesO2O.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                           where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == bujiaoPaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.StartDate && b.Employee_ID == item.CompanyEmployeeRelation.EmployeeId
                                           select b).FirstOrDefault();//费用表
                            if (listooo != null)
                            {
                                listooo.ServiceCoset = 0;
                            }
                        }
                        #endregion
                        #endregion
                        SysEntitiesO2O.SaveChanges();
                    }
                    #endregion
                }
                return "";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
        #endregion

        #region 查询报增失败导入模板中的数据是否属于该社保客服负责的、待责任客服审核的 数据
        /// <summary>
        /// 查询报增失败导入模板中的数据是否属于该社保客服负责的、待责任客服审核的 数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <returns></returns>      
        public int CheckApprove(string certificateNumber, string name, int insuranceKindId, string cityName, int userID_SB, ref EmployeeAdd addModel)
        {
            using (SysEntities ent = new SysEntities())
            {
                try
                {
                    //员工  姓名 证件号码
                    var emp = ent.Employee.Where(a => a.Name.Equals(name) && a.CertificateNumber.Equals(certificateNumber));
                    //emp = emp.Where(a => a.State.Equals("在职"));//员工状态
                    int cnt = emp.Count();
                    if (cnt == 0)
                    {
                        return 1;
                    }
                    //增加员工（社保报增表）   状态(社保专员已提取)、险种
                    string AddState = Common.EmployeeAdd_State.社保专员已提取.ToString();
                    var empAdd = ent.EmployeeAdd.Where(a => a.State == AddState && a.InsuranceKindId == insuranceKindId);

                    //社保缴纳地 地名
                    var city = ent.City.Where(a => a.Name.Equals(cityName));

                    int addId = 0;

                    // 根据员工ID、缴纳地、员工在职状态确定企业员工关系
                    // 根据企业员工关系确定、险种、报增状态确定增加员工（报增）
                    var chkList1 = from a in emp
                                   join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                   join c in city on r.CityId equals c.Id
                                   join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                   select b;
                    cnt = chkList1.Count();
                    if (cnt == 0)
                    {
                        return 2;
                    }
                    else
                    {
                        addModel = chkList1.ToList().FirstOrDefault();
                        addId = addModel.Id;
                    }

                    // 根据报增表政策手续ID确定社保政策
                    // 根据社保政策表社保种类ID确定社保种类
                    // 根据社保种类ID确定客服社保种类
                    var chkList2 = from a in empAdd
                                   join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                   join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                   join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == userID_SB) on c.Id equals d.InsuranceKindId
                                   where a.Id == addId
                                   select a;

                    cnt = chkList2.Count();

                    if (cnt == 0)
                    {
                        return 3;
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }
        #endregion

        #region 供应商客服提取报增数据
        /// <summary>
        /// 供应商客服提取报增数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<SupplierAddView> GetEmployeeAddExcelListBySupplier(SysEntities db, string search)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    //string BranchName = "";

                    #region 查询条件

                    var emp = ent.Employee.Where(a => true);//员工
                    var empAdd = ent.EmployeeAdd.Where(a => true);//员工报增
                    var com = ent.CRM_Company.Where(a => true);//企业
                    var comb = ent.CRM_CompanyToBranch.Where(a => true);//企业所属分支机构
                    var city = ent.City.Where(a => true);//城市
                    var empPhone = ent.EmployeeContact.Where(a => true);//员工联系方式

                    //获取最后一条报增信息
                    var empAddLast = from a in empAdd
                                     group a by new
                                     {
                                         a.CompanyEmployeeRelationId,
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

                    //获取最后一条联系信息
                    var empPhoneLast = from a in empPhone
                                       group a by new
                                       {
                                           a.EmployeeId
                                       } into g
                                       select new
                                       {
                                           EmployeeId = g.Key.EmployeeId,
                                           Id = g.Max(a => a.Id)
                                       };

                    empPhone = from a in empPhone
                               join b in empPhoneLast on a.Id equals b.Id
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

                        if (queryDic.ContainsKey("UserID_SB") && !string.IsNullOrWhiteSpace(queryDic["UserID_SB"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_SB"]);
                            empAdd = from a in empAdd
                                     join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                     join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                     join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                     select a;
                        }

                        if (queryDic.ContainsKey("UserID_Supplier") && !string.IsNullOrWhiteSpace(queryDic["UserID_Supplier"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_Supplier"]);
                            empAdd = from a in empAdd
                                     join s in ent.Supplier on a.SuppliersId equals s.Id
                                     where s.CustomerServiceId == str
                                     select a;
                        }

                        if (queryDic.ContainsKey("SupplierID") && !string.IsNullOrWhiteSpace(queryDic["SupplierID"]))
                        {
                            int str = Convert.ToInt32(queryDic["SupplierID"]);
                            empAdd = from a in empAdd where a.SuppliersId == str select a;
                        }
                    }
                    #endregion

                    #region 数据调取
                    int comMark = 1;
                    int empMark = 2;

                    string[] stopStatus = new string[] { Common.EmployeeStopPayment_State.待供应商客服提取.ToString(), 
                                                Common.EmployeeStopPayment_State.待员工客服经理分配.ToString(),
                                                Common.EmployeeStopPayment_State.待员工客服确认.ToString(),
                                                Common.EmployeeStopPayment_State.待责任客服确认.ToString(),
                                                Common.EmployeeStopPayment_State.社保专员已提取.ToString(),
                                                Common.EmployeeStopPayment_State.已发送供应商.ToString(),
                                                Common.EmployeeStopPayment_State.员工客服已确认.ToString()
                                                //,Common.EmployeeStopPayment_State.责任客服未通过.ToString()
                    };

                    var list = from a in emp
                               join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                               join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                               join c in com on r.CompanyId equals c.ID
                               join d in comb on c.ID equals d.CRM_Company_ID
                               join e in city on r.CityId equals e.Id
                               join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                               join user in ent.ORG_User on d.UserID_ZR equals user.ID
                               join phone in empPhone on a.Id equals phone.EmployeeId into tmp
                               from userphone in tmp.DefaultIfEmpty()
                               join dep in ent.ORG_Department on user.ORG_Department_ID equals dep.ID
                               join branch in ent.ORG_Department on dep.BranchID equals branch.ID
                               join pi in ent.PoliceInsurance on b.PoliceInsuranceId equals pi.Id
                               //join po in ent.PoliceOperation on b.PoliceOperationId equals po.Id
                               //join s in tmpStop on new { insurancekindid = b.InsuranceKindId, cityid = r.CityId, employeeid = r.EmployeeId }
                               //               equals new { insurancekindid = s.InsuranceKindId, cityid = s.CityID, employeeid = s.EmployeeId } into tmp2
                               //from stop in tmp2.DefaultIfEmpty()
                               select new SupplierAddView()
                               {
                                   CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                   CompanyId = r.CompanyId,
                                   EmployeeId = a.Id,
                                   CompanyCode = c.CompanyCode,
                                   CompanyName = c.CompanyName,
                                   CertificateNumber = a.CertificateNumber,
                                   EmployeeName = a.Name,
                                   City = e.Name,
                                   CityID = e.Id,
                                   OperationTime = b.StartTime,
                                   CustomerName = user.RName,
                                   BranchName = branch.DepartmentName,
                                   Telephone = userphone == null ? "" : userphone.MobilePhone ?? userphone.Telephone,
                                   YearMonth = b.YearMonth,
                                   PoliceInsuranceId = b.PoliceInsuranceId,
                                   PoliceInsuranceName = pi.Name,
                                   Wage = b.Wage,
                                   EmployeeAddId = b.Id,
                                   SupplierRemark = b.SupplierRemark,
                                   InsuranceKindId = b.InsuranceKindId
                                   //State = stop == null ? "" : "1"
                               };

                    #endregion


                    //查看同一员工在同一地市同一险种是否也存在报减数据
                    var tmpStop = (from a in ent.EmployeeAdd
                                   join b in ent.CompanyEmployeeRelation on a.CompanyEmployeeRelationId equals b.Id
                                   join c in ent.EmployeeStopPayment on a.Id equals c.EmployeeAddId
                                   where b.State == "在职" && stopStatus.Contains(c.State)
                                   select new
                                   {
                                       a.InsuranceKindId,
                                       b.CityId,
                                       b.EmployeeId
                                   }).ToList();


                    #region 在内存中进行数据计算

                    var appList = list.ToList();

                    appList = (from a in appList
                               join s in tmpStop on new { insurancekindid = a.InsuranceKindId, cityid = a.CityID, employeeid = a.EmployeeId }
                                              equals new { insurancekindid = s.InsuranceKindId, cityid = s.CityId, employeeid = s.EmployeeId } into tmp2
                               from stop in tmp2.DefaultIfEmpty()
                               select new SupplierAddView()
                               {
                                   CompanyEmployeeRelationId = a.CompanyEmployeeRelationId,
                                   CompanyCode = a.CompanyCode,
                                   CompanyName = a.CompanyName,
                                   EmployeeName = a.EmployeeName,
                                   CertificateNumber = a.CertificateNumber,
                                   InsuranceKindId = a.InsuranceKindId,
                                   City = a.City,
                                   CityID = a.CityID,
                                   CompanyNumber = Get_Jishu(db, (int)a.PoliceInsuranceId, (decimal)a.Wage, comMark),
                                   CompanyPercent = Get_BILI(db, (int)a.PoliceInsuranceId, (decimal)a.Wage, comMark),
                                   EmployeeNumber = Get_Jishu(db, (int)a.PoliceInsuranceId, (decimal)a.Wage, empMark),
                                   EmployeePercent = Get_BILI(db, (int)a.PoliceInsuranceId, (decimal)a.Wage, empMark),
                                   State = stop == null ? "" : "1",
                                   PoliceInsuranceName = a.PoliceInsuranceName,
                                   OperationTime = a.OperationTime,
                                   Wage = a.Wage,
                                   CustomerName = a.CustomerName,
                                   BranchName = a.BranchName,
                                   Telephone = a.Telephone,
                                   YearMonth = a.YearMonth,
                                   SupplierRemark = a.SupplierRemark,
                                   EmployeeAddId = a.EmployeeAddId
                               }).ToList();
                    #endregion
                    return appList;
                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }

        #endregion

        #region 修改报增数据状态
        /// <summary>
        /// 修改报增数据状态
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ids">要修改数据的ID数组</param>
        /// <param name="oldStatus">原状态(可为空字符)</param>
        /// <param name="newStatus">新状态</param>
        /// <param name="name">修改人</param>
        /// <returns></returns>
        public bool ChangeStatus(SysEntities db, int[] ids, string oldStatus, string newStatus, string name)
        {
            try
            {
                var updateEmpAdd = db.EmployeeAdd.Where(a => ids.Contains(a.Id));
                if (!string.IsNullOrEmpty(oldStatus))
                {
                    updateEmpAdd = updateEmpAdd.Where(a => a.State == oldStatus);
                }
                if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                {
                    foreach (var item in updateEmpAdd)
                    {
                        item.State = newStatus;
                        item.UpdateTime = DateTime.Now;
                        item.UpdatePerson = name;
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}




