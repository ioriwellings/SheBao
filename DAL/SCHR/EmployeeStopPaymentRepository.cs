using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Common;
using Langben.DAL.Model;
using System.Web;


namespace Langben.DAL
{
    public partial class EmployeeStopPaymentRepository
    {
        #region 责任客服审核平台数据
        /// <summary>
        /// 责任客服审核平台数据（待责任客服审核）
        /// </summary>
        /// <param name="db">数据访问上下文</param>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="cardIds">身份证号（可多条根据换行符分割）</param> 
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="total">结果集的总数</param>
        /// <returns></returns>
        public List<SingleStopPaymentView> GetListFromP(SysEntities db, int zrUserId,
            int companyId, string employeeName, string cardIds, int page, int rows, ref int total)
        {

            List<SingleStopPaymentView> view = null;
            using (var ent = new SysEntities())
            {
                //获取责任客服所负责的企业
                IQueryable<CRM_CompanyToBranch> companyToBranch = db.CRM_CompanyToBranch.Where(o => true);
                if (zrUserId > 0)
                {
                    companyToBranch = db.CRM_CompanyToBranch.Where(o => o.UserID_ZR == zrUserId);
                }
                //获取员工姓名查询员工信息
                IQueryable<Employee> employee = db.Employee.Where(o => true);
                if (!string.IsNullOrEmpty(employeeName))
                {
                    employee = employee.Where(o => o.Name.Contains(employeeName));
                }
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
                //获取申报成功状态的报增信息
                string strState = Common.EmployeeStopPayment_State.待责任客服确认.ToString();
                var EmployeeStopPayment = db.EmployeeStopPayment.Where(o => o.State == strState);
                if (companyId > 0)
                {
                    EmployeeStopPayment = EmployeeStopPayment.Where(o => o.EmployeeAdd.CompanyEmployeeRelation.CompanyId == companyId);
                }

                //根据状态提取待审核的数据（五个险种）
                //InsuranceKindId 要筛选的险种的数据

                var list = (from a in EmployeeStopPayment
                            join e in employee on a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId equals e.Id
                            join d in db.CRM_Company on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals d.ID
                            join f in db.City on a.EmployeeAdd.CompanyEmployeeRelation.CityId equals f.Id
                            join ctb in companyToBranch on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals ctb.CRM_Company_ID
                            select new SingleStopPaymentView
                            {
                                EmployeeID = a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId ?? 0,
                                CompanyID = a.EmployeeAdd.CompanyEmployeeRelation.CompanyId ?? 0,
                                YearMonth = a.EmployeeAdd.YearMonth,
                                EmployeeName = e.Name,
                                CertificateNumber = e.CertificateNumber,
                                CompanyName = d.CompanyName,
                                CityName = f.Name,
                                CityID = f.Id,
                                CompanyEmployeeRelationId = a.EmployeeAdd.CompanyEmployeeRelationId

                            }).Distinct();

                total = 0;
                if (list.Any())
                {
                    total = list.Count();
                    if (page > -1)
                    {
                        list = list.OrderBy(a => a.CompanyID).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                    }
                }
                view = list.ToList();

                //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                var temp = (from a in EmployeeStopPayment
                            join b in list on a.EmployeeAdd.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                            join d in db.InsuranceKind on a.EmployeeAdd.InsuranceKindId equals d.Id
                            select new
                            {
                                a.EmployeeAdd.CompanyEmployeeRelationId,
                                d.Name,
                                a.Id

                            }).ToList().Distinct();


                foreach (SingleStopPaymentView v in view)
                {
                    var kinds = temp.Where(a => a.CompanyEmployeeRelationId == v.CompanyEmployeeRelationId).ToList();
                    string kindsName = kinds.Aggregate("", (current, a) => current + (a.Name + ","));
                    string kindsID = kinds.Aggregate("", (current, a) => current + (a.Id + ","));
                    v.CanSotpInsuranceKindName = kindsName;
                    v.CanSotpInsuranceKindIDs = kindsID;
                }

                return view;

            }
        }

        #endregion

        #region 责任客服审核平台数据，通过，或者退回
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">数据访问上下文</param>
        /// <param name="kindsIDS">被操作的险种的iD</param>
        /// <param name="IsPass">1 通过 2 不通过</param>
        /// <returns></returns>
        public bool PassStopPaymentRepository(SysEntities db, string kindsIDS, int IsPass)
        {
            bool rbool = false;

            string[] Str_kindsIDS = kindsIDS.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] Int_kindsIDS = new int[Str_kindsIDS.Count()];
            int i = 0;
            foreach (var x in Str_kindsIDS)
            {
                Int_kindsIDS[i] = Convert.ToInt32(x);
                i++;
            }
            var list = db.EmployeeStopPayment.Where(x => Int_kindsIDS.Contains(x.Id)).ToList();
            foreach (var li in list)
            {
                if (IsPass == 1)
                {
                    li.State = Common.EmployeeStopPayment_State.待员工客服经理分配.ToString();
                }
                else
                {
                    li.State = Common.EmployeeStopPayment_State.责任客服未通过.ToString();
                }

            }

            db.SaveChanges();





            return rbool;

        }
        #endregion

        #region 员工客服审核报减数据
        public List<SingleStopPaymentView> GetEmployeeStopForCustomerList(SysEntities db, int zrUserId,
         int companyId, string employeeName, string cardIds, int page, int rows, ref int total, int yguserid)
        {

            List<SingleStopPaymentView> view = null;
            using (var ent = new SysEntities())
            {
                //获取责任客服所负责的企业
                var companyToBranch = db.UserCityCompany.Where(o => true);
                //if (zrUserId > 0)
                //{
                //    companyToBranch = db.CRM_CompanyToBranch.Where(o => true);
                //}


                if (yguserid > 0)
                {
                    companyToBranch = db.UserCityCompany.Where(o => o.UserID_YG == yguserid);
                }

                //获取员工姓名查询员工信息
                IQueryable<Employee> employee = db.Employee.Where(o => true);
                if (!string.IsNullOrEmpty(employeeName))
                {
                    employee = employee.Where(o => o.Name.Contains(employeeName));
                }
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
                //获取申报成功状态的报增信息
                string strState = Common.EmployeeStopPayment_State.待员工客服确认.ToString();
                var EmployeeStopPayment = db.EmployeeStopPayment.Where(o => o.State == strState);

                List<EmployeeStopPayment> aa = EmployeeStopPayment.ToList();
                if (companyId > 0)
                {
                    EmployeeStopPayment = EmployeeStopPayment.Where(o => o.EmployeeAdd.CompanyEmployeeRelation.CompanyId == companyId);
                }

                //根据状态提取待审核的数据（五个险种）
                //InsuranceKindId 要筛选的险种的数据

                var list = (from a in EmployeeStopPayment
                            join e in employee on a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId equals e.Id
                            join d in db.CRM_Company on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals d.ID
                            join f in db.City on a.EmployeeAdd.CompanyEmployeeRelation.CityId equals f.Id
                            // join g in db.ORG_UserCity.Where(o => true) on f.Id equals g.CityId
                            join g in db.ORG_UserCity.Where(o => o.UserID == yguserid) on f.Id equals g.CityId
                            join ctb in companyToBranch on new{ a.EmployeeAdd.CompanyEmployeeRelation.CompanyId,a.EmployeeAdd.CompanyEmployeeRelation.CityId} equals new { ctb.CompanyId,ctb.CityId}
                            select new SingleStopPaymentView
                            {

                                EmployeeID = a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId ?? 0,
                                CompanyID = a.EmployeeAdd.CompanyEmployeeRelation.CompanyId ?? 0,
                                YearMonth = a.EmployeeAdd.YearMonth,
                                EmployeeName = e.Name,
                                CertificateNumber = e.CertificateNumber,
                                CompanyName = d.CompanyName,
                                CityName = f.Name,
                                CityID = f.Id,
                                CompanyEmployeeRelationId = a.EmployeeAdd.CompanyEmployeeRelationId

                            }).Distinct();
                var ff = list.ToList();

                total = 0;
                if (list.Any())
                {
                    total = list.Count();
                    if (page > -1)
                    {
                        list = list.OrderBy(a => a.CompanyID).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                    }
                }
                view = list.ToList();

                //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                var temp = (from a in EmployeeStopPayment
                            join b in list on a.EmployeeAdd.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                            //join d in db.InsuranceKind on a.EmployeeAdd.InsuranceKindId equals d.Id
                            select new
                            {
                                a.EmployeeAdd.CompanyEmployeeRelationId,
                                a.EmployeeAdd.InsuranceKindId,
                                a.Id

                            }).ToList().Distinct();


                foreach (SingleStopPaymentView v in view)
                {

                    var kinds = temp.Where(a => a.CompanyEmployeeRelationId == v.CompanyEmployeeRelationId).ToList();
                    string kindsName = "";// kinds.Aggregate("", (current, a) => current + (a.Name + ","));
                    string kindsID = "";//kinds.Aggregate("", (current, a) => current + (a.Id + ","));
                    foreach (var k in kinds)
                    {
                        kindsName = kindsName + ((Common.EmployeeAdd_InsuranceKindId)k.InsuranceKindId).ToString() + ",";
                        kindsID = kindsID + k.Id + ",";
                    }



                    v.CanSotpInsuranceKindName = kindsName.TrimEnd(',');
                    v.CanSotpInsuranceKindIDs = kindsID.TrimEnd(',');
                }

                return view;

            }
        }

        #endregion


        #region 责任客服修改报减 数据 修改社保月
        public List<SingleStopPaymentViewDuty> GetEmployeeStopForDutyList(SysEntities db, int zrUserId,
         int companyId, string employeeName, string YearMonth, string cardIds, int page, int rows, ref int total)
        {

            //List<SingleStopPaymentViewDuty> view = null;
            //using (var ent = new SysEntities())
            //{
            //    //获取责任客服所负责的企业
            //    IQueryable<CRM_CompanyToBranch> companyToBranch = db.CRM_CompanyToBranch.Where(o => true);
            //    if (zrUserId > 0)
            //    {
            //        companyToBranch = db.CRM_CompanyToBranch.Where(o => o.UserID_ZR == zrUserId);
            //    }
            //    //获取员工姓名查询员工信息
            //    IQueryable<Employee> employee = db.Employee.Where(o => true);
            //    if (!string.IsNullOrEmpty(employeeName))
            //    {
            //        employee = employee.Where(o => o.Name.Contains(employeeName));
            //    }
            //    if (!string.IsNullOrEmpty(cardIds))
            //    {
            //        string[] CARD_ID_LIST = cardIds.Split(Convert.ToChar(10));
            //        List<string> CARDLIST = new List<string>();
            //        //var CARD_ID_LIST = CARD_ID.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //        for (int i = 0; i < CARD_ID_LIST.Length; i++)
            //        {
            //            CARDLIST.Add(CARD_ID_LIST[i]);
            //            CARDLIST.Add(CardCommon.CardIDTo15(CARD_ID_LIST[i]));
            //            CARDLIST.Add(CardCommon.CardIDTo18(CARD_ID_LIST[i]));
            //        }
            //        CARDLIST = CARDLIST.Distinct().ToList();
            //        employee = employee.Where(o => CARDLIST.Contains(o.CertificateNumber));
            //    }
            //    //获取申报成功状态的报增信息
            //    string strState = Common.EmployeeStopPayment_State.待员工客服确认.ToString();
            //    string strStateadd = Common.EmployeeStopPayment_State.申报成功.ToString();
            //    var EmployeeStopPayment = db.EmployeeStopPayment.Where(o => o.State == strState && o.EmployeeAdd.State == strStateadd);
            //    if (companyId > 0)
            //    {
            //        EmployeeStopPayment = EmployeeStopPayment.Where(o => o.EmployeeAdd.CompanyEmployeeRelation.CompanyId == companyId);
            //    }


            //    if (!string.IsNullOrEmpty(YearMonth))
            //    {
            //        int yearmonth = int.Parse(YearMonth);
            //        EmployeeStopPayment = EmployeeStopPayment.Where(o => o.YearMonth == yearmonth);
            //    }

            //    //根据状态提取待审核的数据（五个险种）
            //    //InsuranceKindId 要筛选的险种的数据

            //    var list = from a in EmployeeStopPayment
            //               join e in employee on a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId equals e.Id
            //               join d in db.CRM_Company on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals d.ID
            //               join f in db.City on a.EmployeeAdd.CompanyEmployeeRelation.CityId equals f.Id
            //               join ctb in companyToBranch on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals ctb.CRM_Company_ID
            //               select new SingleStopPaymentViewDuty
            //               {
            //                   ID = a.Id,
            //                   EmployeeID = a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId ?? 0,
            //                   CompanyID = a.EmployeeAdd.CompanyEmployeeRelation.CompanyId ?? 0,
            //                   YearMonth = a.EmployeeAdd.YearMonth,
            //                   EmployeeName = e.Name,
            //                   CertificateNumber = e.CertificateNumber,
            //                   CompanyName = d.CompanyName,
            //                   CityName = f.Name,
            //                   CityID = f.Id,
            //                   CompanyEmployeeRelationId = a.EmployeeAdd.CompanyEmployeeRelationId

            //               };

            //    total = 0;
            //    if (list.Any())
            //    {
            //        total = list.Count();
            //        if (page > -1)
            //        {
            //            list = list.OrderBy(a => a.CompanyID).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
            //        }
            //    }
            //    view = list.ToList();

            //    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
            //    var temp = (from a in EmployeeStopPayment
            //                join b in list on a.EmployeeAdd.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
            //                join d in db.InsuranceKind on a.EmployeeAdd.InsuranceKindId equals d.Id
            //                select new
            //                {
            //                    a.EmployeeAdd.CompanyEmployeeRelationId,
            //                    d.Name,
            //                    d.Id

            //                }).ToList();


            //    foreach (SingleStopPaymentViewDuty v in view)
            //    {
            //        var kinds = temp.Where(a => a.CompanyEmployeeRelationId == v.CompanyEmployeeRelationId).ToList();
            //        string kindsName = kinds.Aggregate("", (current, a) => current + (a.Name + ","));
            //        string kindsID = kinds.Aggregate("", (current, a) => current + (a.Id + ","));
            //        v.CanSotpInsuranceKindName = kindsName;
            //        v.CanSotpInsuranceKindIDs = kindsID;
            //    }

            //    return view;

            //}
            List<SingleStopPaymentViewDuty> view = null;
            using (var ent = new SysEntities())
            {
                //获取责任客服所负责的企业
                IQueryable<CRM_CompanyToBranch> companyToBranch = db.CRM_CompanyToBranch.Where(o => true);
                if (zrUserId > 0)
                {
                    companyToBranch = db.CRM_CompanyToBranch.Where(o => o.UserID_ZR == zrUserId);
                }
                //获取员工姓名查询员工信息
                IQueryable<Employee> employee = db.Employee.Where(o => true);
                if (!string.IsNullOrEmpty(employeeName))
                {
                    employee = employee.Where(o => o.Name.Contains(employeeName));
                }
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
                //获取申报成功状态的报增信息
                string strState = Common.EmployeeStopPayment_State.待员工客服确认.ToString();
                var EmployeeStopPayment = db.EmployeeStopPayment.Where(o => o.State == strState);
                if (companyId > 0)
                {
                    EmployeeStopPayment = EmployeeStopPayment.Where(o => o.EmployeeAdd.CompanyEmployeeRelation.CompanyId == companyId);
                }
                if (!string.IsNullOrEmpty(YearMonth))
                {
                    int yearmonth = int.Parse(YearMonth);
                    EmployeeStopPayment = EmployeeStopPayment.Where(o => o.YearMonth == yearmonth);
                }
                //根据状态提取待审核的数据（五个险种）
                //InsuranceKindId 要筛选的险种的数据

                var list = (from a in EmployeeStopPayment
                            join e in employee on a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId equals e.Id
                            join d in db.CRM_Company on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals d.ID
                            join f in db.City on a.EmployeeAdd.CompanyEmployeeRelation.CityId equals f.Id
                            join ctb in companyToBranch on a.EmployeeAdd.CompanyEmployeeRelation.CompanyId equals ctb.CRM_Company_ID
                            select new SingleStopPaymentViewDuty
                            {

                                EmployeeID = a.EmployeeAdd.CompanyEmployeeRelation.EmployeeId ?? 0,
                                CompanyID = a.EmployeeAdd.CompanyEmployeeRelation.CompanyId ?? 0,
                                YearMonth = a.EmployeeAdd.YearMonth,
                                EmployeeName = e.Name,
                                CertificateNumber = e.CertificateNumber,
                                CompanyName = d.CompanyName,
                                CityName = f.Name,
                                CityID = f.Id,
                                CompanyEmployeeRelationId = a.EmployeeAdd.CompanyEmployeeRelationId

                            }).Distinct();

                total = 0;
                if (list.Any())
                {
                    total = list.Count();
                    if (page > -1)
                    {
                        list = list.OrderBy(a => a.CompanyID).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                    }
                }
                view = list.ToList();

                //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                var temp = (from a in EmployeeStopPayment
                            join b in list on a.EmployeeAdd.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                            //join d in db.InsuranceKind on a.EmployeeAdd.InsuranceKindId equals d.Id
                            select new
                            {
                                a.EmployeeAdd.CompanyEmployeeRelationId,
                                a.EmployeeAdd.InsuranceKindId,
                                a.Id

                            }).ToList().Distinct();


                foreach (SingleStopPaymentViewDuty v in view)
                {

                    var kinds = temp.Where(a => a.CompanyEmployeeRelationId == v.CompanyEmployeeRelationId).ToList();
                    string kindsName = "";// kinds.Aggregate("", (current, a) => current + (a.Name + ","));
                    string kindsID = "";//kinds.Aggregate("", (current, a) => current + (a.Id + ","));
                    foreach (var k in kinds)
                    {
                        kindsName = kindsName + ((Common.EmployeeAdd_InsuranceKindId)k.InsuranceKindId).ToString() + ",";
                        kindsID = kindsID + ",";
                    }



                    v.CanSotpInsuranceKindName = kindsName;
                    v.CanSotpInsuranceKindIDs = kindsID;
                }


                return view;

            }
        }

        #endregion

        #region 员工客服审核平台数据，确认，或者终止
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">数据访问上下文</param>
        /// <param name="kindsIDS">被操作的险种的iD</param>
        /// <param name="IsPass">1 确认 2 终止</param>
        /// <returns></returns>
        public bool PassStopPaymentRepositoryForCustomer(SysEntities db, string kindsIDS, int IsPass, string remark)
        {
            bool rbool = false;

            string[] Str_kindsIDS = kindsIDS.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] Int_kindsIDS = new int[Str_kindsIDS.Count()];
            int i = 0;
            foreach (var x in Str_kindsIDS)
            {
                Int_kindsIDS[i] = Convert.ToInt32(x);
                i++;
            }
            var list = db.EmployeeStopPayment.Where(x => Int_kindsIDS.Contains(x.Id)).ToList();
            foreach (var li in list)
            {
                if (IsPass == 1)
                {
                    li.State = Common.EmployeeStopPayment_State.员工客服已确认.ToString();
                }
                else
                {
                    li.State = Common.EmployeeStopPayment_State.终止.ToString();
                    li.Remark = remark;

                    //更新中间表
                    var middle =
                    db.EmployeeMiddle.FirstOrDefault(
                        o =>
                            o.InsuranceKindId == li.EmployeeAdd.InsuranceKindId &&
                            o.CompanyEmployeeRelationId == li.EmployeeAdd.CompanyEmployeeRelationId && o.State == "启用" &&
                            o.PaymentStyle == 1);
                    if (middle != null)
                    {

                        middle.EndedDate = 999912;
                        middle.UpdateTime = DateTime.Now;

                    }
                }

            }

            db.SaveChanges();





            return rbool;

        }
        #endregion

        #region 修改报减方式
        /// <summary>
        /// 修改报减方式
        /// </summary>
        /// <param name="db"></param>
        /// <param name="EmployeeStopPaymentID"></param>
        /// <returns></returns>
        public bool EmploeeStopInfo(SysEntities db, int EmployeeStopPaymentID, int StopStyle)
        {
            bool f = false;
            try
            {
                var stop = db.EmployeeStopPayment.FirstOrDefault(x => x.Id == EmployeeStopPaymentID);
                stop.PoliceOperationId = StopStyle;
                db.SaveChanges();
                f = true;
            }
            catch (Exception)
            {

                f = false;
            }
            return f;

        }

        public bool EditStopPaymentOperation(SysEntities db, List<EmployeeStopPayment> lstStopPayments)
        {
            bool result = false;

            try
            {
                foreach (EmployeeStopPayment stopPayment in lstStopPayments)
                {
                    var stop = db.EmployeeStopPayment.FirstOrDefault(x => x.Id == stopPayment.Id);
                    if (stop != null) stop.PoliceOperationId = stopPayment.PoliceOperationId;
                }
                db.SaveChanges();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        #endregion


        #region 报减详情
        /// <summary>
        /// 获取报减详情
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="companyEmployeeRelationId">companyEmployeeRelationId</param>
        /// <param name="State">停缴的状态</param>
        /// <returns></returns>
        public StopPaymentEmployeeInfo EmploeeStopInfo(SysEntities db, int companyEmployeeRelationId, string State)
        {


           
            State = HttpUtility.HtmlDecode(State);
            string Addstate = EmployeeAdd_State.申报成功.ToString();
            StopPaymentEmployeeInfo info = new StopPaymentEmployeeInfo();

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
                var query2 = from d in db.EmployeeStopPayment
                             where d.EmployeeAdd.CompanyEmployeeRelationId == companyEmployeeRelationId && d.State == State && d.EmployeeAdd.State == Addstate
                             select new
                             {
                                 CityName = d.EmployeeAdd.PoliceInsurance.InsuranceKind.City1.Name,
                                 PoliceAccountNatureName = d.EmployeeAdd.PoliceAccountNature.Name,
                                 d.EmployeeAdd.Wage,
                                 EmployeeStopId = d.Id,
                                 d.EmployeeAdd.InsuranceKindId,
                                 InsuranceKindName = d.EmployeeAdd.PoliceInsurance.InsuranceKind.Name,
                                 PoliceInsuranceId = d.EmployeeAdd.PoliceInsuranceId,
                                 CompanyEmployeeRelationId = d.EmployeeAdd.CompanyEmployeeRelationId,
                                 stopdate = d.InsuranceMonth,
                                 stopstyle = d.PoliceOperation.Name

                             };

                info.EmployeeName = query1.EmployeeName;
                info.CardId = query1.CardId;
                info.CompanyName = query1.CompanyName;
                info.Station = query1.Station;
                info.CityName = query2.First().CityName;
                info.PoliceAccountNatureName = query2.First().PoliceAccountNatureName;
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
                List<StopPaymentInsuranceKindInfo> lstkindInfo = new List<StopPaymentInsuranceKindInfo>();

                foreach (var q in query2)
                {
                    StopPaymentInsuranceKindInfo kindInfo = new StopPaymentInsuranceKindInfo()
                    {
                        EmployeeStopPaymentID = q.EmployeeStopId,
                        InsuranceKindId = q.InsuranceKindId ?? 0,
                        InsuranceKindName = q.InsuranceKindName,
                        PoliceInsuranceId = q.PoliceInsuranceId ?? 0,
                        Wage = q.Wage ?? 0,
                        CompanyEmployeeRelationId = q.CompanyEmployeeRelationId ?? 0,
                        PoliceOperationName = q.stopstyle,
                        StopDate = q.stopdate

                    };
                    lstkindInfo.Add(kindInfo);

                }
                info.LstStopPaymentInsuranceKindInfos = lstkindInfo;
            }
            return info;

        }
        #endregion


        #region 获取可单人报减员工信息

        /// <summary>
        /// 获取可单人报减员工信息
        /// </summary> 
        /// <param name="db">数据访问上下文</param>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="cardIds">身份证号（可多条根据换行符分割）</param> 
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="total">结果集的总数</param>
        /// <returns></returns>
        public List<SingleStopPaymentView> GetSingleStopPaymentInfo(SysEntities db, int zrUserId,
            int companyId, string employeeName, string cardIds, int page, int rows, ref int total)
        {
            List<SingleStopPaymentView> view = new List<SingleStopPaymentView>();


            //获取责任客服所负责的企业
            IQueryable<CRM_CompanyToBranch> companyToBranch = db.CRM_CompanyToBranch.Where(o => true);
            if (zrUserId > 0)
            {
                companyToBranch = db.CRM_CompanyToBranch.Where(o => o.UserID_ZR == zrUserId);
            }
            //获取员工姓名查询员工信息
            IQueryable<Employee> employee = db.Employee.Where(o => true);
            if (!string.IsNullOrEmpty(employeeName))
            {
                employee = employee.Where(o => o.Name.Contains(employeeName));
            }
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
            //获取申报成功状态的报增信息
            string strState = Common.EmployeeAdd_State.申报成功.ToString();
            List<string> stopingState = new List<string>()
            {
                Common.EmployeeStopPayment_State.待责任客服确认.ToString(),
                Common.EmployeeStopPayment_State.待员工客服经理分配.ToString(),
                Common.EmployeeStopPayment_State.待员工客服确认.ToString(),
                Common.EmployeeStopPayment_State.员工客服已确认.ToString(),
                Common.EmployeeStopPayment_State.社保专员已提取.ToString(),
                Common.EmployeeStopPayment_State.待供应商客服提取.ToString(),
                Common.EmployeeStopPayment_State.已发送供应商.ToString(),
            };

            var employeeAdd = from a in db.EmployeeAdd
                              join b in employee on a.CompanyEmployeeRelation.EmployeeId equals b.Id
                              where
                                  a.State == strState &&
                                  !stopingState.Contains(
                                      a.EmployeeStopPayment.OrderByDescending(o => o.CreateTime).FirstOrDefault().State)
                              select a;

            if (companyId > 0)
            {
                employeeAdd = employeeAdd.Where(o => o.CompanyEmployeeRelation.CompanyId == companyId);
            }
            var q = employeeAdd.ToList();
            var qd = companyToBranch.ToList();
            var qsd = companyToBranch.ToList();
            var po= db.InsuranceKind.ToList();
            var pod = db.CRM_Company.ToList();
            var queryd = (from ea in employeeAdd
                         join ctb in companyToBranch on ea.CompanyEmployeeRelation.CompanyId equals ctb.CRM_Company_ID
                          join e in employee on ea.CompanyEmployeeRelation.EmployeeId equals e.Id
                          join c in db.CRM_Company on ea.CompanyEmployeeRelation.CompanyId equals c.ID
                          select new SingleStopPaymentView()
                         {
                             CompanyEmployeeRelationId = ea.CompanyEmployeeRelationId,
                              CompanyID = c.ID,
                              CompanyName = c.CompanyName,
                              EmployeeID = e.Id,
                              CertificateNumber = e.CertificateNumber,
                              EmployeeName = e.Name
                          }).Distinct().ToList();
            var query = (from ea in employeeAdd
                         join ctb in companyToBranch on ea.CompanyEmployeeRelation.CompanyId equals ctb.CRM_Company_ID
                         join e in employee on ea.CompanyEmployeeRelation.EmployeeId equals e.Id
                         join c in db.CRM_Company on ea.CompanyEmployeeRelation.CompanyId equals c.ID
                        // join ik in db.InsuranceKind on ea.InsuranceKindId.Value equals ik.Id
                         select new SingleStopPaymentView()
                         {
                             CompanyEmployeeRelationId = ea.CompanyEmployeeRelationId,
                             CompanyID = c.ID,
                             CompanyName = c.CompanyName,
                             EmployeeID = e.Id,
                             CertificateNumber = e.CertificateNumber,
                             EmployeeName = e.Name
                             //EmployeeAddId = ea.Id,
                         }).Distinct();
            total = 0;
            if (query.Any())
            {
                total = query.Count();
                if (page > -1)
                {
                    query = query.OrderBy(a => a.CompanyID).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
                }
            }
            view = query.ToList();
            //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
            var temp = (from a in employeeAdd
                        join b in query on a.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                        join d in db.InsuranceKind on a.PoliceInsurance.InsuranceKind.Id equals d.Id
                        select new
                        {
                            a.CompanyEmployeeRelationId,
                            d.Name
                        }).Distinct().ToList();


            foreach (SingleStopPaymentView v in view)
            {
                var kinds = temp.Where(a => a.CompanyEmployeeRelationId == v.CompanyEmployeeRelationId).ToList();
                string kindsName = kinds.Aggregate("", (current, a) => current + (a.Name + ","));
                v.CanSotpInsuranceKindName = kindsName.TrimEnd(',');

            }

            return view;
        }

        #endregion

        #region 平台Api--批量添加停缴信息
        /// <summary>
        /// 平台Api--批量添加停缴信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="lstStopPaymentEmployeeInfo"></param>
        /// <returns></returns>
        public string InsertStopPaymentInfo(SysEntities db, List<StopPaymentEmployeeInfoForApi> lstStopPaymentEmployeeInfo)
        {
            string result = string.Empty;
            StringBuilder error = new StringBuilder();
            List<EmployeeStopPayment> lstStopPayment = new List<EmployeeStopPayment>();
            DateTime now = DateTime.Now;
            List<string> lstStopingKind = new List<string>()
            {
                Common.EmployeeStopPayment_State.待责任客服确认.ToString(),
                Common.EmployeeStopPayment_State.待员工客服经理分配.ToString(),
                Common.EmployeeStopPayment_State.待员工客服确认.ToString(),
                Common.EmployeeStopPayment_State.员工客服已确认.ToString(),
                Common.EmployeeStopPayment_State.社保专员已提取.ToString(),
                Common.EmployeeStopPayment_State.待供应商客服提取.ToString(),
                Common.EmployeeStopPayment_State.已发送供应商.ToString(),
            };

            #region 验证

            try
            {
                foreach (StopPaymentEmployeeInfoForApi emp in lstStopPaymentEmployeeInfo)
                {
                    StringBuilder err = new StringBuilder();
                    var e = db.Employee.FirstOrDefault(o => o.CertificateNumber == emp.CardId);
                    if (e == null)
                    {
                        err.AppendLine(string.Format("不存在员工 {0} - {1}，不能报减。", emp.EmployeeName, emp.CardId));
                        continue;
                    }

                    emp.EmployeeId = e.Id;

                    var cer =
                        db.CompanyEmployeeRelation.Where(
                            o => o.CompanyId == emp.CompanyId && o.EmployeeId == emp.EmployeeId && o.State == "在职")
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault();
                    if (cer == null)
                    {
                        err.AppendLine(string.Format("{0} 的员工 {1} 没有任何申报成功的社保或公积金，不能报减。", emp.CompanyName, emp.EmployeeName));
                        continue;
                    }

                    var empAdd = db.EmployeeAdd.Where(o => o.CompanyEmployeeRelationId == cer.Id && o.State == "申报成功");
                    if (!empAdd.Any())
                    {
                        err.AppendLine(string.Format("{0} 的员工 {1} 没有任何申报成功的社保或公积金，不能报减。", emp.CompanyName, emp.EmployeeName));
                        continue;
                    }
                    foreach (StopPaymentInsuranceKindInfoForApi k in emp.LstInsuranceKindInfo)
                    {

                        var add = empAdd.FirstOrDefault(o => o.PoliceInsuranceId == k.PoliceInsuranceId);
                        if (add == null)
                        {
                            err.AppendLine(string.Format("{0} 的员工 {1} 没有申报成功 {2}，不能报减。", emp.CompanyName, emp.EmployeeName,
                                k.InsuranceKindName));
                        }
                        else
                        {
                            var stopingKind = add.EmployeeGoonPayment.Where(o => lstStopingKind.Contains(o.State));
                            if (stopingKind.Any())
                            {
                                err.AppendLine(string.Format("{0} 的员工 {1}  {2} 正在报减，不能重复报减。", emp.CompanyName,
                                    emp.EmployeeName, k.InsuranceKindName));
                            }
                            else
                            {
                                int insuranceAddMonths = 0;
                                var firstOrDefault = db.PoliceInsurance.FirstOrDefault(o => o.Id == k.PoliceInsuranceId);
                                if (firstOrDefault != null)
                                {
                                    insuranceAddMonths = firstOrDefault.InsuranceAdd ?? 0;
                                }
                                EmployeeStopPayment stopPayment = new EmployeeStopPayment();
                                stopPayment.EmployeeAddId = add.Id;
                                stopPayment.InsuranceMonth = DateTime.Now.AddMonths(insuranceAddMonths);
                                stopPayment.PoliceOperationId = k.PoliceOperationId;
                                stopPayment.Remark = k.Remark;
                                stopPayment.State = Common.EmployeeStopPayment_State.待责任客服确认.ToString();
                                stopPayment.CreateTime = now;
                                stopPayment.CreatePerson = k.CreatePerson;
                                stopPayment.UpdateTime = now;
                                stopPayment.UpdatePerson = k.CreatePerson;
                                stopPayment.YearMonth = Convert.ToInt32(now.Year.ToString() + now.Month.ToString());

                                lstStopPayment.Add(stopPayment);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(err.ToString()))
                    {
                        error.AppendLine(err.ToString());
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            #endregion
            result = error.ToString();
            try
            {
                if (lstStopPayment.Any() && string.IsNullOrEmpty(result))
                {
                    foreach (EmployeeStopPayment stopPayment in lstStopPayment)
                    {
                        Create(db, stopPayment);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        #endregion

        #region 获取停缴时员工的信息
        /// <summary>
        /// 获取停缴时员工的信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyEmployeeRelationId"></param>
        /// <returns></returns>
        public StopPaymentEmployeeInfo GetStopPaymentEmployeeInfo(SysEntities db, int companyEmployeeRelationId)
        {
            StopPaymentEmployeeInfo info = new StopPaymentEmployeeInfo();

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
                Common.EmployeeStopPayment_State.待供应商客服提取.ToString(),
                Common.EmployeeStopPayment_State.已发送供应商.ToString(),
            };
                var query2 = from d in db.EmployeeAdd
                             where d.CompanyEmployeeRelationId == companyEmployeeRelationId && d.State == "申报成功" &&
                                  !stopingState.Contains(
                                      d.EmployeeStopPayment.OrderByDescending(o => o.CreateTime).FirstOrDefault().State)
                             select new
                             {
                                 CityName = d.PoliceInsurance.InsuranceKind.City1.Name,
                                 PoliceAccountNatureName = d.PoliceAccountNature.Name,
                                 d.Wage,
                                 EmployeeAddId = d.Id,
                                 InsuranceKindId = d.InsuranceKindId,// d.PoliceInsurance.InsuranceKind.Id,
                                 InsuranceKindName = d.PoliceInsurance.InsuranceKind.Name,
                                 PoliceInsuranceId = d.PoliceInsuranceId,
                                 CompanyEmployeeRelationId = d.CompanyEmployeeRelationId,
                             };

                info.EmployeeName = query1.EmployeeName;
                info.CardId = query1.CardId;
                info.CompanyName = query1.CompanyName;
                info.Station = query1.Station;
                info.CityName = query2.First().CityName;
                info.PoliceAccountNatureName = query2.First().PoliceAccountNatureName;
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
                List<StopPaymentInsuranceKindInfo> lstkindInfo = new List<StopPaymentInsuranceKindInfo>();

                foreach (var q in query2)
                {
                    StopPaymentInsuranceKindInfo kindInfo = new StopPaymentInsuranceKindInfo()
                    {
                        EmployeeAddId = q.EmployeeAddId,
                        InsuranceKindId = q.InsuranceKindId ?? 0,
                        InsuranceKindName = q.InsuranceKindName,
                        PoliceInsuranceId = q.PoliceInsuranceId ?? 0,
                        Wage = q.Wage ?? 0,
                        CompanyEmployeeRelationId = q.CompanyEmployeeRelationId ?? 0,
                    };
                    lstkindInfo.Add(kindInfo);

                }
                info.LstStopPaymentInsuranceKindInfos = lstkindInfo;
            }
            return info;
        }
        #endregion

        #region 保存单人报减信息并修改员工费用中间表的相关信息

        /// <summary>
        /// 保存单人报减信息并修改员工费用中间表的相关信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool InsertStopPayment(SysEntities db, List<EmployeeStopPaymentSingle> list)
        {
            bool result = false;
            try
            {
                foreach (EmployeeStopPaymentSingle single in list)
                {
                    Create(db, single.StopPayment);
                    var middle =
                        db.EmployeeMiddle.FirstOrDefault(
                            o =>
                                o.InsuranceKindId == single.InsuranceKindId &&
                                o.CompanyEmployeeRelationId == single.CompanyEmployeeRelationId && o.State == "启用" &&
                                o.PaymentStyle == 1);
                    if (middle != null)
                    {
                        var policeInsurance = db.PoliceInsurance.FirstOrDefault(o => o.Id == single.PoliceInsuranceId);
                        if (single.StopPayment.InsuranceMonth.HasValue)
                        {
                            int ir = policeInsurance == null ? 0 : policeInsurance.InsuranceReduce ?? 0;
                            int ia = policeInsurance.InsuranceAdd ?? 0;
                            DateTime endDate = single.StopPayment.InsuranceMonth.Value.AddMonths(-ir).AddMonths(-1); //.AddMonths(ir - ia);
                            middle.EndedDate = Convert.ToInt32(endDate.ToString("yyyyMM"));
                            middle.UpdateTime = DateTime.Now;
                        }
                    }
                }
                db.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;
        }
        #endregion


        #region 通过员工姓名和身份证号获取员工ID

        /// <summary>
        /// 通过员工姓名和身份证号获取员工ID
        /// </summary>
        /// <param name="db"></param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="cardId">身份证号</param>
        /// <returns>返回员工ID，如果没有查到返回0</returns>
        public int GetEmployeeIdByNameAndCardId(SysEntities db, string employeeName, string cardId)
        {
            int employeeId = 0;
            if (!string.IsNullOrEmpty(employeeName) && !string.IsNullOrEmpty(cardId))
            {
                var model = db.Employee.FirstOrDefault(o => o.Name == employeeName && o.CertificateNumber == cardId);
                if (model != null)
                {
                    employeeId = model.Id;
                }
            }
            return employeeId;
        }
        #endregion

        #region 通过企业ID和员工ID获取企业员工关系表ID

        /// <summary>
        /// 通过企业ID和员工ID获取企业员工关系表ID
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyId">企业ID</param>
        /// <param name="employeeId">员工ID</param>
        /// <returns>返回企业员工关系表ID，如果没有查到返回0</returns>
        public int GetCompanyEmployeeRelationId(SysEntities db, int companyId, int employeeId)
        {
            int relationId = 0;
            if (companyId > 0 && employeeId > 0)
            {
                var model =
                    db.CompanyEmployeeRelation.FirstOrDefault(
                        o => o.CompanyId == companyId && o.EmployeeId == employeeId);
                if (model != null)
                {
                    relationId = model.Id;
                }
            }
            return relationId;
        }
        #endregion

        #region 通过险种和企业员工关系表ID获取增员表ID

        /// <summary>
        /// 通过险种和企业员工关系表ID获取增员表ID
        /// </summary>
        /// <param name="db"></param>
        /// <param name="kind">险种</param>
        /// <param name="companyEmployeeRelationId">企业员工关系表ID</param>
        /// <returns>返回状态是报增成功的报增记录ID，如果没查到返回0</returns>
        public int GetEmployeeAddIdByKindIdAndRelationId(SysEntities db, EmployeeAdd_InsuranceKindId kind, int companyEmployeeRelationId)
        {
            int employeeAddId = 0;
            int kindId = Convert.ToInt32(kind);
            string strState = EmployeeAdd_State.申报成功.ToString();
            var model =
                db.EmployeeAdd.Where(
                    o =>
                        o.InsuranceKindId == kindId && o.CompanyEmployeeRelationId == companyEmployeeRelationId &&
                        o.State == strState).OrderByDescending(o => o.Id).FirstOrDefault();
            if (model != null)
            {
                employeeAddId = model.Id;
            }
            return employeeAddId;
        }
        #endregion

        #region 通过增员表ID和手续名称获取手续ID
        /// <summary>
        /// 通过增员表ID和手续名称获取手续ID
        /// </summary>
        /// <param name="employeeAddId">增员表ID</param>
        /// <param name="operationName">手续名称</param>
        /// <returns>返回手续ID，如果没查到返回0</returns>
        public int GetPoliceOperationIdByEmployeeAddIdAndOperationName(SysEntities db, int employeeAddId, string operationName)
        {
            int operationId = 0;

            var model =
                db.EmployeeAdd.First(o => o.Id == employeeAddId)
                    .PoliceInsurance.InsuranceKind.PoliceOperation.FirstOrDefault(o => o.Name == operationName);
            if (model != null)
            {
                operationId = model.Id;
            }
            return operationId;
        }
        #endregion


        #region 社保报减查询
        /// <summary>
        /// 社保报减查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeStopView> GetEmployeeStopList(SysEntities db, int page, int rows, string search, List<ORG_User> userList, ref int count)
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
                    var empStop = ent.EmployeeStopPayment.Where(a => true);


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
                    //由于报减表主表为报增表,需要以报增表进行过滤
                    empAdd = from a in empAdd
                             join b in empAddLast on a.Id equals b.Id
                             select a;

                    //由于报减存在中断，所以需要排重
                    //

                    var empStopLast = from a in empAdd
                                      join b in empStop on a.Id equals b.EmployeeAddId
                                      group b by b.EmployeeAddId into g
                                      select new
                                      {
                                          EmployeeAddId = g.Key,
                                          Id = g.Max(a => a.Id)
                                      };
                    //由于报减表主表为报增表,需要以报增表进行过滤
                    empStop = from a in empStop
                              join b in empStopLast on a.Id equals b.Id
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
                            empStop = empStop.Where(a => states.Contains(a.State));
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
                            empStop = empStop.Where(a => a.YearMonth == str);
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
                            empAdd = (from a in empAdd
                                      join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                      join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                      join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                      select a).Distinct();
                        }

                        // 若选择了供应商筛选条件，则过滤供应商
                        if (queryDic.ContainsKey("SuppliersId") && !string.IsNullOrWhiteSpace(queryDic["SuppliersId"]))
                        {
                            int suppliersID = Convert.ToInt32(queryDic["SuppliersId"]);
                            empAdd = empAdd.Where(a => a.SuppliersId == suppliersID);
                        }
                        // 若为供应商提取页面，则过滤供应商负责的客服
                        if (queryDic.ContainsKey("UserID_Supplier") && !string.IsNullOrWhiteSpace(queryDic["UserID_Supplier"]))
                        {
                            int userId_supplier = Convert.ToInt32(queryDic["UserID_Supplier"]);
                            empAdd = from a in empAdd
                                     join b in ent.Supplier.Where(x => x.CustomerServiceId == userId_supplier) on a.SuppliersId equals b.Id
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
                                join es in empStop on b.Id equals es.EmployeeAddId
                                join g2 in ent.UserCityCompany.Where(a => userIdList2.Contains(a.UserID_YG)) on new { CityId = e.Id, r.CompanyId } equals new { g2.CityId, g2.CompanyId } into gg
                                from g in gg.DefaultIfEmpty()           //数据有无代表缴纳地是否属于此用户
                                join h2 in ent.CRM_CompanyToBranch.Where(a => userIdList2.Contains(a.UserID_ZR)) on c.ID equals h2.CRM_Company_ID into hh
                                from h in hh.DefaultIfEmpty()          //数据有无代表客户是否属于此用户

                                join i in ent.PoliceInsurance on b.PoliceInsuranceId equals i.Id
                                join j in ent.InsuranceKind on i.InsuranceKindId equals j.Id
                                join k2 in ent.ORG_UserInsuranceKind.Where(a => userIdList.Contains(a.UserID)) on j.Id equals k2.InsuranceKindId into kk
                                from k in kk.DefaultIfEmpty()          //数据有无代表险种是否属于此用户   
                                where (g.ID != null || h.ID != null || k.ID != null)

                                select new EmployeeStopView()
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

                    List<EmployeeStopView> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                join g in empStop on b.Id equals g.EmployeeAddId
                                join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on g.PoliceOperationId equals e.Id
                                select new
                                {
                                    g.Id,
                                    b.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    g.YearMonth,
                                    d.Name,
                                    State = g.Remark == null ? g.State : g.State + ":" + g.Remark,
                                    PoliceOperationName = e.Name,
                                    g.InsuranceMonth,
                                    b.IsIndependentAccount,
                                    b.InsuranceCode,
                                    b.CreateTime
                                }).ToList();

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
                               select new EmployeeStopView()
                               {
                                   CompanyEmployeeRelationId = a.CompanyEmployeeRelationId,
                                   CompanyCode = a.CompanyCode,
                                   CompanyName = a.CompanyName,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   City = a.City,
                                   PoliceAccountNatureName = a.PoliceAccountNatureName,
                                   Station = a.Station,
                                   StopIds = (b1 == null ? "" : b1.Id.ToString() + ",")
                                                     + (b2 == null ? "" : b2.Id.ToString() + ",")
                                                     + (b3 == null ? "" : b3.Id.ToString() + ",")
                                                     + (b4 == null ? "" : b4.Id.ToString() + ",")
                                                     + (b5 == null ? "" : b5.Id.ToString() + ",")
                                                     + (b6 == null ? "" : b6.Id.ToString() + ",")
                                                     + (b7 == null ? "" : b7.Id.ToString() + ",")
                                                     + (b8 == null ? "" : b8.Id.ToString() + ",")
                                                     + (b9 == null ? "" : b9.Id.ToString() + ","),


                                   State_1 = b1 == null ? null : b1.State,
                                   PoliceInsuranceName_1 = b1 == null ? null : b1.Name,
                                   YearMonth_1 = b1 == null ? null : b1.YearMonth,
                                   PoliceOperationName_1 = b1 == null ? null : b1.PoliceOperationName,
                                   InsuranceMonth_1 = b1 == null ? null : b1.InsuranceMonth,

                                   IsIndependentAccount_1 = b1 == null ? null : b1.IsIndependentAccount,
                                   InsuranceCode_1 = b1 == null ? null : b1.InsuranceCode,
                                   CreateTime_1 = b1 == null ? null : b1.CreateTime,


                                   State_2 = b2 == null ? null : b2.State,
                                   PoliceInsuranceName_2 = b2 == null ? null : b2.Name,
                                   YearMonth_2 = b2 == null ? null : b2.YearMonth,
                                   PoliceOperationName_2 = b2 == null ? null : b2.PoliceOperationName,
                                   InsuranceMonth_2 = b2 == null ? null : b2.InsuranceMonth,

                                   IsIndependentAccount_2 = b2 == null ? null : b2.IsIndependentAccount,
                                   InsuranceCode_2 = b2 == null ? null : b2.InsuranceCode,
                                   CreateTime_2 = b2 == null ? null : b2.CreateTime,

                                   State_3 = b3 == null ? null : b3.State,
                                   PoliceInsuranceName_3 = b3 == null ? null : b3.Name,
                                   YearMonth_3 = b3 == null ? null : b3.YearMonth,
                                   PoliceOperationName_3 = b3 == null ? null : b3.PoliceOperationName,
                                   InsuranceMonth_3 = b3 == null ? null : b3.InsuranceMonth,

                                   IsIndependentAccount_3 = b3 == null ? null : b3.IsIndependentAccount,
                                   InsuranceCode_3 = b3 == null ? null : b3.InsuranceCode,
                                   CreateTime_3 = b3 == null ? null : b3.CreateTime,

                                   State_4 = b4 == null ? null : b4.State,
                                   PoliceInsuranceName_4 = b4 == null ? null : b4.Name,
                                   YearMonth_4 = b4 == null ? null : b4.YearMonth,
                                   PoliceOperationName_4 = b4 == null ? null : b4.PoliceOperationName,
                                   InsuranceMonth_4 = b4 == null ? null : b4.InsuranceMonth,

                                   IsIndependentAccount_4 = b4 == null ? null : b4.IsIndependentAccount,
                                   InsuranceCode_4 = b4 == null ? null : b4.InsuranceCode,
                                   CreateTime_4 = b4 == null ? null : b4.CreateTime,


                                   State_5 = b5 == null ? null : b5.State,
                                   PoliceInsuranceName_5 = b5 == null ? null : b5.Name,
                                   YearMonth_5 = b5 == null ? null : b5.YearMonth,
                                   PoliceOperationName_5 = b5 == null ? null : b5.PoliceOperationName,
                                   InsuranceMonth_5 = b5 == null ? null : b5.InsuranceMonth,


                                   IsIndependentAccount_5 = b5 == null ? null : b5.IsIndependentAccount,
                                   InsuranceCode_5 = b5 == null ? null : b5.InsuranceCode,
                                   CreateTime_5 = b5 == null ? null : b5.CreateTime,


                                   State_6 = b6 == null ? null : b6.State,
                                   PoliceInsuranceName_6 = b6 == null ? null : b6.Name,
                                   YearMonth_6 = b6 == null ? null : b6.YearMonth,
                                   PoliceOperationName_6 = b6 == null ? null : b6.PoliceOperationName,
                                   InsuranceMonth_6 = b6 == null ? null : b6.InsuranceMonth,

                                   IsIndependentAccount_6 = b6 == null ? null : b6.IsIndependentAccount,
                                   InsuranceCode_6 = b6 == null ? null : b6.InsuranceCode,
                                   CreateTime_6 = b6 == null ? null : b6.CreateTime,


                                   State_7 = b7 == null ? null : b7.State,
                                   PoliceInsuranceName_7 = b7 == null ? null : b7.Name,
                                   YearMonth_7 = b7 == null ? null : b7.YearMonth,
                                   PoliceOperationName_7 = b7 == null ? null : b7.PoliceOperationName,
                                   InsuranceMonth_7 = b7 == null ? null : b7.InsuranceMonth,


                                   IsIndependentAccount_7 = b7 == null ? null : b7.IsIndependentAccount,
                                   InsuranceCode_7 = b7 == null ? null : b7.InsuranceCode,
                                   CreateTime_7 = b7 == null ? null : b7.CreateTime,


                                   State_8 = b8 == null ? null : b8.State,
                                   PoliceInsuranceName_8 = b8 == null ? null : b8.Name,
                                   YearMonth_8 = b8 == null ? null : b8.YearMonth,
                                   PoliceOperationName_8 = b8 == null ? null : b8.PoliceOperationName,
                                   InsuranceMonth_8 = b8 == null ? null : b8.InsuranceMonth,



                                   IsIndependentAccount_8 = b8 == null ? null : b8.IsIndependentAccount,
                                   InsuranceCode_8 = b8 == null ? null : b8.InsuranceCode,
                                   CreateTime_8 = b8 == null ? null : b8.CreateTime,




                                   State_9 = b9 == null ? null : b9.State,
                                   PoliceInsuranceName_9 = b9 == null ? null : b9.Name,
                                   YearMonth_9 = b9 == null ? null : b9.YearMonth,
                                   PoliceOperationName_9 = b9 == null ? null : b9.PoliceOperationName,
                                   InsuranceMonth_9 = b9 == null ? null : b9.InsuranceMonth,



                                   IsIndependentAccount_9 = b9 == null ? null : b9.IsIndependentAccount,
                                   InsuranceCode_9 = b9 == null ? null : b9.InsuranceCode,
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

        #region 社保报减查询-导出Excel（无分页）
        /// <summary>
        /// 社保报减查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <returns></returns>
        public List<EmployeeStopView> GetEmployeeStopListForExcel(SysEntities db, string search, List<ORG_User> userList)
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
                    var empStop = ent.EmployeeStopPayment.Where(a => true);

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
                    //由于报减表主表为报增表,需要以报增表进行过滤
                    empAdd = from a in empAdd
                             join b in empAddLast on a.Id equals b.Id
                             select a;

                    //由于报减存在中断，所以需要排重
                    //

                    var empStopLast = from a in empAdd
                                      join b in empStop on a.Id equals b.EmployeeAddId
                                      group b by b.EmployeeAddId into g
                                      select new
                                      {
                                          EmployeeAddId = g.Key,
                                          Id = g.Max(a => a.Id)
                                      };
                    //由于报减表主表为报增表,需要以报增表进行过滤
                    empStop = from a in empStop
                              join b in empStopLast on a.Id equals b.Id
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
                            empStop = empStop.Where(a => states.Contains(a.State));
                        }
                         // 导出时取全部险种
                        //if (queryDic.ContainsKey("InsuranceKinds") && !string.IsNullOrWhiteSpace(queryDic["InsuranceKinds"]))
                        //{
                        //    string str = queryDic["InsuranceKinds"];
                        //    int?[] Ids = Array.ConvertAll<string, int?>(str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });

                        //    empAdd = empAdd.Where(a => Ids.Contains(a.InsuranceKindId));
                        //}
                        if (queryDic.ContainsKey("YearMonth") && !string.IsNullOrWhiteSpace(queryDic["YearMonth"]))
                        {
                            int str = Convert.ToInt32(queryDic["YearMonth"]);
                            empStop = empStop.Where(a => a.YearMonth == str);
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
                            empAdd = (from a in empAdd
                                      join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                      join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                      join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                      select a).Distinct();
                        }

                        // 若选择了供应商筛选条件，则过滤供应商
                        if (queryDic.ContainsKey("SuppliersId") && !string.IsNullOrWhiteSpace(queryDic["SuppliersId"]))
                        {
                            int suppliersID = Convert.ToInt32(queryDic["SuppliersId"]);
                            empAdd = empAdd.Where(a => a.SuppliersId == suppliersID);
                        }
                        // 若为供应商提取页面，则过滤供应商负责的客服
                        if (queryDic.ContainsKey("UserID_Supplier") && !string.IsNullOrWhiteSpace(queryDic["UserID_Supplier"]))
                        {
                            int userId_supplier = Convert.ToInt32(queryDic["UserID_Supplier"]);
                            empAdd = from a in empAdd
                                     join b in ent.Supplier.Where(x => x.CustomerServiceId == userId_supplier) on a.SuppliersId equals b.Id
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
                                join es in empStop on b.Id equals es.EmployeeAddId
                                join g2 in ent.UserCityCompany.Where(a => userIdList2.Contains(a.UserID_YG)) on new { CityId = e.Id, r.CompanyId } equals new { g2.CityId, g2.CompanyId } into gg
                                from g in gg.DefaultIfEmpty()           //数据有无代表缴纳地是否属于此用户
                                join h2 in ent.CRM_CompanyToBranch.Where(a => userIdList2.Contains(a.UserID_ZR)) on c.ID equals h2.CRM_Company_ID into hh
                                from h in hh.DefaultIfEmpty()          //数据有无代表客户是否属于此用户

                                join i in ent.PoliceInsurance on b.PoliceInsuranceId equals i.Id
                                join j in ent.InsuranceKind on i.InsuranceKindId equals j.Id
                                join k2 in ent.ORG_UserInsuranceKind.Where(a => userIdList.Contains(a.UserID)) on j.Id equals k2.InsuranceKindId into kk
                                from k in kk.DefaultIfEmpty()          //数据有无代表险种是否属于此用户   
                                where (g.ID != null || h.ID != null || k.ID != null)

                                select new EmployeeStopView()
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

                    #endregion

                    #region 在内存中进行数据拼接

                    List<EmployeeStopView> appList = list.ToList();
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from b in empAdd
                                join g in empStop on b.Id equals g.EmployeeAddId
                                join c in list on b.CompanyEmployeeRelationId equals c.CompanyEmployeeRelationId
                                join d in ent.PoliceInsurance on b.PoliceInsuranceId equals d.Id
                                join e in ent.PoliceOperation on g.PoliceOperationId equals e.Id
                                select new
                                {
                                    g.Id,
                                    b.CompanyEmployeeRelationId,
                                    b.InsuranceKindId,
                                    g.YearMonth,
                                    d.Name,
                                    State = g.Remark == null ? g.State : g.State + ":" + g.Remark,
                                    PoliceOperationName = e.Name,
                                    g.InsuranceMonth,
                                    b.IsIndependentAccount,
                                    b.InsuranceCode,
                                    b.CreateTime
                                }).ToList();

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

                               join coms in ent.CompanyEmployeeRelation on a.CompanyEmployeeRelationId equals coms.Id
                               join branch in ent.CRM_CompanyToBranch on coms.CompanyId equals branch.CRM_Company_ID
                               join user in ent.ORG_User on branch.UserID_ZR equals user.ID
                               join depart in ent.ORG_Department on user.ORG_Department_ID equals depart.ID
                               join gsmc in ent.ORG_Department on depart.BranchID equals gsmc.ID

                               select new EmployeeStopView()
                               {
                                   Operator_CompanyName = gsmc.DepartmentName,  // 分公司名称
                                   CompanyEmployeeRelationId = a.CompanyEmployeeRelationId,
                                   CompanyCode = a.CompanyCode,
                                   CompanyName = a.CompanyName,
                                   Name = a.Name,
                                   CertificateNumber = a.CertificateNumber,
                                   City = a.City,
                                   PoliceAccountNatureName = a.PoliceAccountNatureName,
                                   Station = a.Station,
                                   StopIds = (b1 == null ? "" : b1.Id.ToString() + ",")
                                                     + (b2 == null ? "" : b2.Id.ToString() + ",")
                                                     + (b3 == null ? "" : b3.Id.ToString() + ",")
                                                     + (b4 == null ? "" : b4.Id.ToString() + ",")
                                                     + (b5 == null ? "" : b5.Id.ToString() + ",")
                                                     + (b6 == null ? "" : b6.Id.ToString() + ",")
                                                     + (b7 == null ? "" : b7.Id.ToString() + ",")
                                                     + (b8 == null ? "" : b8.Id.ToString() + ",")
                                                     + (b9 == null ? "" : b9.Id.ToString() + ","),


                                   State_1 = b1 == null ? null : b1.State,
                                   PoliceInsuranceName_1 = b1 == null ? null : b1.Name,
                                   YearMonth_1 = b1 == null ? null : b1.YearMonth,
                                   PoliceOperationName_1 = b1 == null ? null : b1.PoliceOperationName,
                                   InsuranceMonth_1 = b1 == null ? null : b1.InsuranceMonth,

                                   IsIndependentAccount_1 = b1 == null ? null : b1.IsIndependentAccount,
                                   InsuranceCode_1 = b1 == null ? null : b1.InsuranceCode,
                                   CreateTime_1 = b1 == null ? null : b1.CreateTime,


                                   State_2 = b2 == null ? null : b2.State,
                                   PoliceInsuranceName_2 = b2 == null ? null : b2.Name,
                                   YearMonth_2 = b2 == null ? null : b2.YearMonth,
                                   PoliceOperationName_2 = b2 == null ? null : b2.PoliceOperationName,
                                   InsuranceMonth_2 = b2 == null ? null : b2.InsuranceMonth,

                                   IsIndependentAccount_2 = b2 == null ? null : b2.IsIndependentAccount,
                                   InsuranceCode_2 = b2 == null ? null : b2.InsuranceCode,
                                   CreateTime_2 = b2 == null ? null : b2.CreateTime,

                                   State_3 = b3 == null ? null : b3.State,
                                   PoliceInsuranceName_3 = b3 == null ? null : b3.Name,
                                   YearMonth_3 = b3 == null ? null : b3.YearMonth,
                                   PoliceOperationName_3 = b3 == null ? null : b3.PoliceOperationName,
                                   InsuranceMonth_3 = b3 == null ? null : b3.InsuranceMonth,

                                   IsIndependentAccount_3 = b3 == null ? null : b3.IsIndependentAccount,
                                   InsuranceCode_3 = b3 == null ? null : b3.InsuranceCode,
                                   CreateTime_3 = b3 == null ? null : b3.CreateTime,

                                   State_4 = b4 == null ? null : b4.State,
                                   PoliceInsuranceName_4 = b4 == null ? null : b4.Name,
                                   YearMonth_4 = b4 == null ? null : b4.YearMonth,
                                   PoliceOperationName_4 = b4 == null ? null : b4.PoliceOperationName,
                                   InsuranceMonth_4 = b4 == null ? null : b4.InsuranceMonth,

                                   IsIndependentAccount_4 = b4 == null ? null : b4.IsIndependentAccount,
                                   InsuranceCode_4 = b4 == null ? null : b4.InsuranceCode,
                                   CreateTime_4 = b4 == null ? null : b4.CreateTime,


                                   State_5 = b5 == null ? null : b5.State,
                                   PoliceInsuranceName_5 = b5 == null ? null : b5.Name,
                                   YearMonth_5 = b5 == null ? null : b5.YearMonth,
                                   PoliceOperationName_5 = b5 == null ? null : b5.PoliceOperationName,
                                   InsuranceMonth_5 = b5 == null ? null : b5.InsuranceMonth,


                                   IsIndependentAccount_5 = b5 == null ? null : b5.IsIndependentAccount,
                                   InsuranceCode_5 = b5 == null ? null : b5.InsuranceCode,
                                   CreateTime_5 = b5 == null ? null : b5.CreateTime,


                                   State_6 = b6 == null ? null : b6.State,
                                   PoliceInsuranceName_6 = b6 == null ? null : b6.Name,
                                   YearMonth_6 = b6 == null ? null : b6.YearMonth,
                                   PoliceOperationName_6 = b6 == null ? null : b6.PoliceOperationName,
                                   InsuranceMonth_6 = b6 == null ? null : b6.InsuranceMonth,

                                   IsIndependentAccount_6 = b6 == null ? null : b6.IsIndependentAccount,
                                   InsuranceCode_6 = b6 == null ? null : b6.InsuranceCode,
                                   CreateTime_6 = b6 == null ? null : b6.CreateTime,


                                   State_7 = b7 == null ? null : b7.State,
                                   PoliceInsuranceName_7 = b7 == null ? null : b7.Name,
                                   YearMonth_7 = b7 == null ? null : b7.YearMonth,
                                   PoliceOperationName_7 = b7 == null ? null : b7.PoliceOperationName,
                                   InsuranceMonth_7 = b7 == null ? null : b7.InsuranceMonth,


                                   IsIndependentAccount_7 = b7 == null ? null : b7.IsIndependentAccount,
                                   InsuranceCode_7 = b7 == null ? null : b7.InsuranceCode,
                                   CreateTime_7 = b7 == null ? null : b7.CreateTime,


                                   State_8 = b8 == null ? null : b8.State,
                                   PoliceInsuranceName_8 = b8 == null ? null : b8.Name,
                                   YearMonth_8 = b8 == null ? null : b8.YearMonth,
                                   PoliceOperationName_8 = b8 == null ? null : b8.PoliceOperationName,
                                   InsuranceMonth_8 = b8 == null ? null : b8.InsuranceMonth,



                                   IsIndependentAccount_8 = b8 == null ? null : b8.IsIndependentAccount,
                                   InsuranceCode_8 = b8 == null ? null : b8.InsuranceCode,
                                   CreateTime_8 = b8 == null ? null : b8.CreateTime,




                                   State_9 = b9 == null ? null : b9.State,
                                   PoliceInsuranceName_9 = b9 == null ? null : b9.Name,
                                   YearMonth_9 = b9 == null ? null : b9.YearMonth,
                                   PoliceOperationName_9 = b9 == null ? null : b9.PoliceOperationName,
                                   InsuranceMonth_9 = b9 == null ? null : b9.InsuranceMonth,



                                   IsIndependentAccount_9 = b9 == null ? null : b9.IsIndependentAccount,
                                   InsuranceCode_9 = b9 == null ? null : b9.InsuranceCode,
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

        #region 供应商客服提取报减数据
        /// <summary>
        /// 供应商客服提取报减数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>      
        public List<SupplierAddView> GetEmployeeStopExcelListBySupplier(SysEntities db, string search)
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

                    var empStop = ent.EmployeeStopPayment.Where(a => true);//员工报减

                    // 获取每个员工同一险种的最后一条报增信息
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
                    //由于报减表主表为报增表,需要以报增表进行过滤
                    empAdd = from a in empAdd
                             join b in empAddLast on a.Id equals b.Id
                             select a;

                    //由于报减存在中断，所以需要排重
                    var empStopLast = from a in empAdd
                                      join b in empStop on a.Id equals b.EmployeeAddId
                                      group b by b.EmployeeAddId into g
                                      select new
                                      {
                                          EmployeeAddId = g.Key,
                                          Id = g.Max(a => a.Id)
                                      };
                    //由于报减表主表为报增表,需要以报增表进行过滤
                    empStop = from a in empStop
                              join b in empStopLast on a.Id equals b.Id
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
                            empStop = empStop.Where(a => states.Contains(a.State));
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
                            empStop = empStop.Where(a => a.YearMonth == str);
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
                            empAdd = (from a in empAdd
                                      join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                      join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                      join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                      select a).Distinct();
                        }

                        // 若选择了供应商筛选条件，则过滤供应商
                        if (queryDic.ContainsKey("SuppliersId") && !string.IsNullOrWhiteSpace(queryDic["SuppliersId"]))
                        {
                            int suppliersID = Convert.ToInt32(queryDic["SuppliersId"]);
                            empAdd = empAdd.Where(a => a.SuppliersId == suppliersID);
                        }
                        // 若为供应商提取页面，则过滤供应商负责的客服
                        if (queryDic.ContainsKey("UserID_Supplier") && !string.IsNullOrWhiteSpace(queryDic["UserID_Supplier"]))
                        {
                            int userId_supplier = Convert.ToInt32(queryDic["UserID_Supplier"]);
                            empAdd = from a in empAdd
                                     join b in ent.Supplier.Where(x => x.CustomerServiceId == userId_supplier) on a.SuppliersId equals b.Id
                                     select a;
                        }

                    }
                    #endregion

                    #region 数据调取
                    int comMark = 1;
                    int empMark = 2;


                    var list = from a in emp
                               join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                               join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                               join s in empStop on b.Id equals s.EmployeeAddId
                               join c in com on r.CompanyId equals c.ID
                               join d in comb on c.ID equals d.CRM_Company_ID
                               join e in city on r.CityId equals e.Id
                               //join f in ent.PoliceAccountNature on r.PoliceAccountNatureId equals f.Id
                               join user in ent.ORG_User on d.UserID_ZR equals user.ID
                               join phone in empPhone on a.Id equals phone.EmployeeId into tmp
                               from userphone in tmp.DefaultIfEmpty()
                               join dep in ent.ORG_Department on user.ORG_Department_ID equals dep.ID
                               join branch in ent.ORG_Department on dep.BranchID equals branch.ID
                               //join pi in ent.PoliceInsurance on b.PoliceInsuranceId equals pi.Id
                               //join po in ent.PoliceOperation on b.PoliceOperationId equals po.Id
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
                                   OperationTime = s.InsuranceMonth,
                                   CustomerName = user.RName,
                                   BranchName = branch.DepartmentName,
                                   Telephone = userphone == null ? "" : userphone.MobilePhone ?? userphone.Telephone,
                                   YearMonth = s.YearMonth,
                                   //PoliceInsuranceId = b.PoliceInsuranceId,
                                   //PoliceInsuranceName = pi.Name,
                                   //Wage = b.Wage,
                                   EmployeeAddId = s.Id,
                                   Remark = s.Remark,
                                   InsuranceKindId = b.InsuranceKindId
                               };

                    #endregion

                    //查看同一员工在同一地市同一险种是否也存在报增数据
                    string[] addStatus = new string[] { Common.EmployeeAdd_State.待供应商客服提取.ToString(),
                                                Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                                                Common.EmployeeAdd_State.待员工客服确认.ToString(),
                                                Common.EmployeeAdd_State.待责任客服确认.ToString(),
                                                Common.EmployeeAdd_State.社保专员已提取.ToString(),
                                                Common.EmployeeAdd_State.已发送供应商.ToString(),
                                                Common.EmployeeAdd_State.员工客服已确认.ToString()
                    };

                    var tmpAdd = (from a in ent.EmployeeAdd
                                   join b in ent.CompanyEmployeeRelation on a.CompanyEmployeeRelationId equals b.Id
                                   where b.State == "在职" && addStatus.Contains(a.State)
                                   select new
                                   {
                                       a.InsuranceKindId,
                                       b.CityId,
                                       b.EmployeeId
                                   }).ToList();


                    #region 在内存中进行数据计算

                    var appList = list.ToList();

                    appList = (from a in appList
                               join s in tmpAdd on new { insurancekindid = a.InsuranceKindId, cityid = a.CityID, employeeid = a.EmployeeId }
                                              equals new { insurancekindid = s.InsuranceKindId, cityid = s.CityId, employeeid = s.EmployeeId } into tmp2
                               from add in tmp2.DefaultIfEmpty()
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
                                   State = add == null ? "" : "1",
                                   //PoliceInsuranceName = a.PoliceInsuranceName,
                                   OperationTime = a.OperationTime,
                                   //Wage = a.Wage,
                                   CustomerName = a.CustomerName,
                                   BranchName = a.BranchName,
                                   Telephone = a.Telephone,
                                   YearMonth = a.YearMonth,
                                   Remark = a.Remark,
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
                    var stop = ent.EmployeeStopPayment.Where(a => true);
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
                            stop = stop.Where(a => states.Contains(a.State));
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
                            stop = stop.Where(a => a.YearMonth == str);
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
                        if (queryDic.ContainsKey("UserID_SB") && !string.IsNullOrWhiteSpace(queryDic["UserID_SB"]))
                        {
                            int str = Convert.ToInt32(queryDic["UserID_SB"]);
                            empAdd = from a in empAdd
                                     join b in ent.PoliceInsurance on a.PoliceInsuranceId equals b.Id
                                     join c in ent.InsuranceKind on b.InsuranceKindId equals c.Id
                                     join d in ent.ORG_UserInsuranceKind.Where(a => a.UserID == str) on c.Id equals d.InsuranceKindId
                                     select a;
                        }

                        // 若选择了供应商筛选条件，则过滤供应商
                        if (queryDic.ContainsKey("SuppliersId") && !string.IsNullOrWhiteSpace(queryDic["SuppliersId"]))
                        {
                            int suppliersID = Convert.ToInt32(queryDic["SuppliersId"]);
                            empAdd = empAdd.Where(a => a.SuppliersId == suppliersID);
                        }
                        // 若为供应商提取页面，则过滤供应商负责的客服
                        if (queryDic.ContainsKey("UserID_Supplier") && !string.IsNullOrWhiteSpace(queryDic["UserID_Supplier"]))
                        {
                            int userId_supplier = Convert.ToInt32(queryDic["UserID_Supplier"]);
                            empAdd = from a in empAdd
                                     join b in ent.Supplier.Where(x => x.CustomerServiceId == userId_supplier) on a.SuppliersId equals b.Id
                                     select a;
                        }
                        
                    }
                    var list = (from a in emp
                                join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                join g in stop on b.Id equals g.EmployeeAddId
                                join c in com on r.CompanyId equals c.ID
                                //join d in ent.InsuranceKind on b.InsuranceKindId equals d.Id
                                join e in city on r.CityId equals e.Id
                                join f in ent.PoliceAccountNature on b.PoliceAccountNatureId equals f.Id
                                join supplier in ent.Supplier on b.SuppliersId equals supplier.Id into ea
                                from ss in ea.DefaultIfEmpty()
                                select new EmployeeApprove()
                                {
                                    CompanyEmployeeRelationId = b.CompanyEmployeeRelationId,
                                    CompanyId = r.CompanyId,
                                    CompanyName = c.CompanyName,
                                    Name = a.Name,
                                    CertificateNumber = a.CertificateNumber,
                                    City = e.Name,
                                    CityID=e.Id,
                                    PoliceAccountNature = f.Name,
                                    YearMonth = g.YearMonth,
                                    SupplierID = b.SuppliersId??0,
                                    SupplierName = ss.Name
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
                    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
                    var temp = (from a in empAdd
                                join b in list on a.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
                                join g in stop on a.Id equals g.EmployeeAddId
                                //join d in ent.InsuranceKind on a.InsuranceKindId equals d.Id
                                where g.YearMonth == b.YearMonth
                                select new
                                {
                                    a.CompanyEmployeeRelationId,
                                    a.InsuranceKindId,
                                    g.YearMonth,
                                    g.Id
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
                            appList[i].InsuranceKinds = kindsName.TrimEnd(',');
                            appList[i].AddIds = addIds.TrimEnd(',');
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
        public bool EmployeeStopPaymentApproved(int[] ApprovedId, string StateOld, string StateNew, string message, string UpdatePerson)
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
                    var updateEmpStop = ent.EmployeeStopPayment.Where(a => ApprovedId.Contains(a.Id) && a.State == StateOld);
                    if (updateEmpStop != null && updateEmpStop.Count() >= 1)
                    {
                        foreach (var item in updateEmpStop)
                        {
                            item.State = StateNew;
                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                item.Remark = message;
                            }
                            if (!string.IsNullOrWhiteSpace(UpdatePerson))
                            {
                                item.UpdatePerson = UpdatePerson;
                            }
                            item.UpdateTime = DateTime.Now;
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

        #region 政策手续 报减
        /// <summary> 
        /// 政策手续 王帅
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id">社保种类</param>
        /// <returns></returns>
        public IQueryable<idname__> getPoliceOperationid(SysEntities db, int id)
        {
            string InsuranceKindStatus = Status.启用.ToString();
            string Style = PoliceOperation_Style.报减.ToString();
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

        #region 社保专员报减反馈列表

        //public List<SingleStopPaymentView> GetStopPaymentInfoForFeedback(SysEntities db, string search, int page, int rows, ref int total)
        //{
        //    List<SingleStopPaymentView> view = new List<SingleStopPaymentView>();
        //    int companyId = 0;
        //    string employeeName = string.Empty;
        //    string cardIds = string.Empty;

        //    Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
        //    if (queryDic != null && queryDic.Count > 0)
        //    {
        //        foreach (var item in queryDic)
        //        {
        //            if (item.Key == "CardIDs")
        //            {//查询一对多关系的列名
        //                cardIds = item.Value;
        //                continue;
        //            }
        //            if (item.Key == "EmployeeName")
        //            {//查询一对多关系的列名
        //                employeeName = item.Value;
        //                continue;
        //            }
        //            if (item.Key == "CompanyID")
        //            {//查询一对多关系的列名
        //                companyId = Convert.ToInt32(item.Value);
        //                continue;
        //            }
        //        }
        //    }

        //    //获取责任客服所负责的企业
        //    IQueryable<CRM_CompanyToBranch> companyToBranch = db.CRM_CompanyToBranch.Where(o => true);
        //    if (zrUserId > 0)
        //    {
        //        companyToBranch = db.CRM_CompanyToBranch.Where(o => o.UserID_ZR == zrUserId);
        //    }
        //    //获取员工姓名查询员工信息
        //    IQueryable<Employee> employee = db.Employee.Where(o => true);
        //    if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        employee = employee.Where(o => o.Name.Contains(employeeName));
        //    }
        //    if (!string.IsNullOrEmpty(cardIds))
        //    {
        //        string[] CARD_ID_LIST = cardIds.Split(Convert.ToChar(10));
        //        List<string> CARDLIST = new List<string>();
        //        //var CARD_ID_LIST = CARD_ID.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //        for (int i = 0; i < CARD_ID_LIST.Length; i++)
        //        {
        //            CARDLIST.Add(CARD_ID_LIST[i]);
        //            CARDLIST.Add(CardCommon.CardIDTo15(CARD_ID_LIST[i]));
        //            CARDLIST.Add(CardCommon.CardIDTo18(CARD_ID_LIST[i]));
        //        }
        //        CARDLIST = CARDLIST.Distinct().ToList();
        //        employee = employee.Where(o => CARDLIST.Contains(o.CertificateNumber));
        //    }
        //    //获取申报成功状态的报增信息
        //    string strState = Common.EmployeeAdd_State.申报成功.ToString();
        //    List<string> stopingState = new List<string>()
        //    {
        //        Common.EmployeeStopPayment_State.待责任客服确认.ToString(),
        //        Common.EmployeeStopPayment_State.待员工客服经理分配.ToString(),
        //        Common.EmployeeStopPayment_State.待员工客服确认.ToString(),
        //        Common.EmployeeStopPayment_State.员工客服已确认.ToString(),
        //        Common.EmployeeStopPayment_State.社保专员已提取.ToString(),
        //        Common.EmployeeStopPayment_State.待供应商客服提取.ToString(),
        //        Common.EmployeeStopPayment_State.已发送供应商.ToString(),
        //    };

        //    var employeeAdd = from a in db.EmployeeAdd
        //                      join b in employee on a.CompanyEmployeeRelation.EmployeeId equals b.Id
        //                      where
        //                          a.State == strState &&
        //                          !stopingState.Contains(
        //                              a.EmployeeStopPayment.OrderByDescending(o => o.CreateTime).FirstOrDefault().State)
        //                      select a;

        //    if (companyId > 0)
        //    {
        //        employeeAdd = employeeAdd.Where(o => o.CompanyEmployeeRelation.CompanyId == companyId);
        //    }

        //    var query = (from ea in employeeAdd
        //                 join ctb in companyToBranch on ea.CompanyEmployeeRelation.CompanyId equals ctb.CRM_Company_ID
        //                 join e in employee on ea.CompanyEmployeeRelation.EmployeeId equals e.Id
        //                 join c in db.CRM_Company on ea.CompanyEmployeeRelation.CompanyId equals c.ID
        //                 join ik in db.InsuranceKind on ea.InsuranceKindId.Value equals ik.Id
        //                 select new SingleStopPaymentView()
        //                 {
        //                     CompanyEmployeeRelationId = ea.CompanyEmployeeRelationId,
        //                     CompanyID = c.ID,
        //                     CompanyName = c.CompanyName,
        //                     EmployeeID = e.Id,
        //                     CertificateNumber = e.CertificateNumber,
        //                     EmployeeName = e.Name,
        //                     EmployeeAddId = ea.Id,
        //                 });
        //    total = 0;
        //    if (query.Any())
        //    {
        //        total = query.Count();
        //        if (page > -1)
        //        {
        //            query = query.OrderBy(a => a.CompanyID).ThenBy(a => a.CompanyEmployeeRelationId).Skip((page - 1) * rows).Take(rows);
        //        }
        //    }
        //    view = query.ToList();
        //    //将险种进行查询补充,将页面需要展示人员的新增险种信息全部查询并放入系统内存中;
        //    var temp = (from a in employeeAdd
        //                join b in query on a.CompanyEmployeeRelationId equals b.CompanyEmployeeRelationId
        //                join d in db.InsuranceKind on a.PoliceInsurance.InsuranceKind.Id equals d.Id
        //                select new
        //                {
        //                    a.CompanyEmployeeRelationId,
        //                    d.Name
        //                }).Distinct().ToList();


        //    foreach (SingleStopPaymentView v in view)
        //    {
        //        var kinds = temp.Where(a => a.CompanyEmployeeRelationId == v.CompanyEmployeeRelationId).ToList();
        //        string kindsName = kinds.Aggregate("", (current, a) => current + (a.Name + ","));
        //        v.CanSotpInsuranceKindName = kindsName.TrimEnd(',');

        //    }

        //    return view;
        //} 

        #endregion

        #region 设置报减成功

        public bool SetStopPaymentSuccess(SysEntities db, List<int> stopIds)
        {
            bool result = false;
            try
            {
                var stopPayment = db.EmployeeStopPayment.Where(o => stopIds.Contains(o.Id));
                foreach (EmployeeStopPayment stop in stopPayment)
                {
                    stop.State = EmployeeStopPayment_State.申报成功.ToString();
                    stop.EmployeeAdd.State = EmployeeAdd_State.已报减.ToString();
                }
                db.SaveChanges();
                //修改企业员工关系表 的状态
                var lstRelationId = stopPayment.Select(o => o.EmployeeAdd).GroupBy(g => g.CompanyEmployeeRelationId).Select(x => x.Key);
                foreach (int relationId in lstRelationId)
                {
                    string strState = EmployeeAdd_State.已报减.ToString();
                   
                    if (!db.EmployeeAdd.Any(o => o.CompanyEmployeeRelationId == relationId && o.State != strState))
                    {
                        int id = relationId;
                        var companyEmployeeRelation = db.CompanyEmployeeRelation.Single(o => o.Id == id);
                        companyEmployeeRelation.State = "离职";
                    }
                }


                db.SaveChanges();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region 设置报减失败

        public bool SetStopPaymentFail(SysEntities db, List<int> stopIds, string remark)
        {
            bool result = false;
            try
            {
                foreach (int id in stopIds)
                {
                    var stop = db.EmployeeStopPayment.FirstOrDefault(x => x.Id == id);

                    if (stop != null)
                    {
                        stop.State = EmployeeStopPayment_State.申报失败.ToString();
                        stop.Remark = remark;

                        //更新中间表
                        var middle =
                        db.EmployeeMiddle.FirstOrDefault(
                            o =>
                                o.InsuranceKindId == stop.EmployeeAdd.InsuranceKindId &&
                                o.CompanyEmployeeRelationId == stop.EmployeeAdd.CompanyEmployeeRelationId && o.State == "启用" &&
                                o.PaymentStyle == 1);
                        if (middle != null)
                        {

                            middle.EndedDate = 999912;
                            middle.UpdateTime = DateTime.Now;

                        }
                    }
                }
                db.SaveChanges();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region 查询报减失败导入模板中的数据是否属于该社保客服负责的、待责任客服审核的 数据
        /// <summary>
        /// 查询报减失败导入模板中的数据是否属于该社保客服负责的、待责任客服审核的 数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <returns></returns>      
        public int CheckApprove(string certificateNumber, string name, int insuranceKindId, string cityName, int userID_SB, ref int stopId)
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
                    //增加员工（社保报增表）   状态(申报成功)、险种
                    string AddState = Common.EmployeeAdd_State.申报成功.ToString();
                    var empAdd = ent.EmployeeAdd.Where(a => a.State == AddState && a.InsuranceKindId == insuranceKindId);

                    //社保缴纳地 地名
                    var city = ent.City.Where(a => a.Name.Equals(cityName));

                    //员工停缴 状态：社保专员已提取
                    string state = Common.EmployeeStopPayment_State.社保专员已提取.ToString();
                    var stop = ent.EmployeeStopPayment.Where(a => a.State.Equals(state));//状态--社保专员已提取

                    //// 根据员工ID、缴纳地、员工在职状态确定企业员工关系
                    //// 根据企业员工关系确定、险种、报增状态确定增加员工（报增）
                    //var chkList1 = from a in emp
                    //               join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                    //               join c in city on r.CityId equals c.Id
                    //               join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                    //               select new
                    //               {
                    //                   Id = b.Id
                    //               };
                    //if (chkList1.Count() == 0)
                    //{
                    //    return "员工没有申报成功的报增信息！";
                    //}

                    int addId = 0;

                    // 根据员工ID、缴纳地、员工在职状态确定企业员工关系
                    // 根据企业员工关系确定、险种、报增状态确定增加员工（报增）
                    // 根据报增ID、报减状态确定员工停缴
                    var chkList1 = from a in emp
                                   join r in ent.CompanyEmployeeRelation on a.Id equals r.EmployeeId
                                   join c in city on r.CityId equals c.Id
                                   join b in empAdd on r.Id equals b.CompanyEmployeeRelationId
                                   join g in stop on b.Id equals g.EmployeeAddId
                                   select g;

                    cnt = chkList1.Count();

                    if (cnt == 0)
                    {
                        return 2;
                    }
                    else
                    {
                        var stopModel = chkList1.ToList().FirstOrDefault();
                        stopId = stopModel.Id;
                        addId = stopModel.EmployeeAddId ?? 0;
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

        #region 导入报减失败原因

        public bool SetStopFail(SysEntities db, List<int> stopIds, List<string> failMsgs, string userName)
        {
            using (db)
            {
                try
                {
                    for (int i = 0; i < stopIds.Count; i++)
                    {
                        int id = stopIds[i];
                        var stop = db.EmployeeStopPayment.FirstOrDefault(x => x.Id == id);
                        if (stop != null)
                        {
                            stop.State = EmployeeStopPayment_State.申报失败.ToString();
                            stop.Remark = failMsgs[i];
                            stop.UpdatePerson = userName;
                            stop.UpdateTime = DateTime.Now;

                            //更新中间表
                            var middle =
                            db.EmployeeMiddle.FirstOrDefault(
                                o =>
                                    o.InsuranceKindId == stop.EmployeeAdd.InsuranceKindId &&
                                    o.CompanyEmployeeRelationId == stop.EmployeeAdd.CompanyEmployeeRelationId && o.State == "启用" &&
                                    o.PaymentStyle == 1);
                            if (middle != null)
                            {

                                middle.EndedDate = 999912;
                                middle.UpdateTime = DateTime.Now;

                            }
                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }

}
