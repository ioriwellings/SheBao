using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Langben.DAL.Model;
using Common;

namespace Langben.DAL
{
    /// <summary>
    /// 员工费用中间表
    /// </summary>
    public partial class EmployeeMiddleRepository
    {
        public IQueryable<EmployeeMiddleShow> GetData(SysEntities db, Expression<Func<EmployeeMiddleShow, bool>> where)
        {
            var query = from em in db.EmployeeMiddle
                        join cer in db.CompanyEmployeeRelation on em.CompanyEmployeeRelationId equals cer.Id
                        join e in db.Employee on cer.EmployeeId equals e.Id
                        join c in db.CRM_Company on cer.CompanyId equals c.ID
                        select new EmployeeMiddleShow
                        {
                            EmployeeId = e.Id,
                            EmployeeName = e.Name,
                            CardId = e.CertificateNumber,

                            CompanyId = c.ID,
                            CompanyName = c.CompanyName,

                            CompanyEmployeeRelationId = em.CompanyEmployeeRelationId,
                            CityId = em.CityId,
                            InsuranceKindId = em.InsuranceKindId,
                            Id = em.Id,
                            PaymentBetween = em.PaymentBetween,
                            PaymentStyle = em.PaymentStyle,
                            CompanyBasePayment = em.CompanyBasePayment,
                            CompanyPayment = em.CompanyPayment,
                            EmployeeBasePayment = em.EmployeeBasePayment,
                            EmployeePayment = em.EmployeePayment,
                            PaymentMonth = em.PaymentMonth,
                            StartDate = em.StartDate,
                            EndedDate = em.EndedDate,
                            UseBetween = em.UseBetween,
                            Remark = em.Remark,
                            State = em.State,
                            CreateTime = em.CreateTime,
                            CreatePerson = em.CreatePerson,
                            UpdateTime = em.UpdateTime,
                            UpdatePerson = em.UpdatePerson
                        };
            return query.Where(where).OrderBy(o => o.EmployeeId);
        }

        /// <summary>
        /// 获取费用中间表中数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="yearMonth">时间</param>
        /// <param name="companyId">公司编号</param>
        /// <param name="insuranceId">种类</param>
        /// <param name="certificate">身份证号</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public IQueryable<EmployeeMiddleShow> GetData(SysEntities db, int yearMonth, List<int> companyId, List<string> cityId, int insuranceId, string certificate, string name)
        {
            var query = from em in db.EmployeeMiddle
                        join cer in db.CompanyEmployeeRelation on em.CompanyEmployeeRelationId equals cer.Id
                        join e in db.Employee on cer.EmployeeId equals e.Id
                        join c in db.CRM_Company on cer.CompanyId equals c.ID
                        select new EmployeeMiddleShow
                        {
                            EmployeeId = e.Id,
                            EmployeeName = e.Name,
                            CardId = e.CertificateNumber,

                            CompanyId = c.ID,
                            CompanyName = c.CompanyName,

                            CompanyEmployeeRelationId = em.CompanyEmployeeRelationId,
                            CityId = em.CityId,
                            InsuranceKindId = em.InsuranceKindId,
                            Id = em.Id,
                            PaymentBetween = em.PaymentBetween,
                            PaymentStyle = em.PaymentStyle,
                            CompanyBasePayment = em.CompanyBasePayment,
                            CompanyPayment = em.CompanyPayment,
                            EmployeeBasePayment = em.EmployeeBasePayment,
                            EmployeePayment = em.EmployeePayment,
                            PaymentMonth = em.PaymentMonth,
                            StartDate = em.StartDate,
                            EndedDate = em.EndedDate,
                            UseBetween = em.UseBetween,
                            Remark = em.Remark,
                            State = em.State,
                            CreateTime = em.CreateTime,
                            CreatePerson = em.CreatePerson,
                            UpdateTime = em.UpdateTime,
                            UpdatePerson = em.UpdatePerson
                        };

            if (yearMonth != 0)
            {
                query = query.Where(u => u.EndedDate >= yearMonth && u.StartDate <= yearMonth);
            }
            if (companyId != null)
            {
                query = query.Where(u => companyId.Contains(u.CompanyId));
            }
            if (cityId != null)
            {
                query = query.Where(u => cityId.Contains(u.CityId));
            }
            if (insuranceId != 0)
            {
                query = query.Where(u => u.InsuranceKindId == insuranceId);
            }
            if (!string.IsNullOrEmpty(certificate))
            {
                // 身份证号验证
                List<string> cardList = new List<string>();

                string[] certificateList = certificate.Split(Convert.ToChar(10));
                for (int i = 0; i < certificateList.Length; i++)
                {
                    cardList.Add(certificateList[i]);
                    cardList.Add(CardCommon.CardIDTo15(certificateList[i]));
                    cardList.Add(CardCommon.CardIDTo18(certificateList[i]));
                }
                cardList = cardList.Distinct().ToList();
                query = query.Where(u => cardList.Contains(u.CardId));
            }
            if (!string.IsNullOrEmpty(name))
            {
                // 名称验证
                string[] nameList = name.Split(Convert.ToChar(10));
                query = query.Where(u => nameList.Contains(u.EmployeeName));
            }
            return query.OrderBy(o => o.EmployeeId);
        }

        /// <summary>
        /// 更新费用中间表状态
        /// </summary>
        /// <param name="id">费用中间表主键</param>
        /// <param name="status">要更新成的状态值</param>
        /// <param name="person">当前操作人</param>
        /// <returns></returns>
        public int UpdateEmployeeMiddleState(int id, string state, string person)
        {
            using (SysEntities db = new SysEntities())
            {
                EmployeeMiddle updateItem = GetById(db, id);
                if (updateItem != null)
                {
                    updateItem.State = state;
                    updateItem.UpdatePerson = person;
                    updateItem.UpdateTime = DateTime.Now;
                }
                return Save(db);
            }
        }


        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断）
        /// </summary>
        /// 责任客服可查询：自己负责的企业
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

                bool isUser_ZRKF = IsUserGroup(groupUser_ZRKF, userID);
                bool isUser_SBKF = IsUserGroup(groupUser_SBKF, userID);

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

                if (isUser_ZRKF)
                {
                    // 获取责任客服负责的企业
                    query = (from cc in query
                             join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                             join f in people on cctb.UserID_ZR equals f.ID
                             select cc);
                }
                #endregion

                return query.ToList();
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
        /// 根据用户组权限获取缴纳地列表（需进行权限判断）
        /// </summary>
        /// 责任客服可查询：所有缴纳地
        /// 社保客服可查询：自己负责的缴纳地
        /// <param name="userID">登录人ID</param>
        public List<City> GetCityListByGroup(int userID)
        {
            using (SysEntities db = new SysEntities())
            {
                string groupUser_SBKF = "SBKF";  // 用户组中“社保客服”的编码
                string groupUser_ZRKF = "ZRKF";  // 用户组中“责任客服”的编码

                bool isUser_ZRKF = IsUserGroup(groupUser_ZRKF, userID);
                bool isUser_SBKF = IsUserGroup(groupUser_SBKF, userID);

                var query = db.City.Where(o => true);

                #region 权限代码
                if (isUser_SBKF)  // 责任客服可以看所有的缴纳地
                {
                    // 缴纳地不需要做任何过滤
                    return query.ToList();
                }
                if (isUser_SBKF)
                {
                    // 员工客服或社保客服可以查看负责的缴纳地
                    return GetCityListByUser(userID);
                }
                #endregion

                return query.ToList();
            }
        }

        /// <summary>
        /// 根据用户编号获取所负责的缴纳地
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<City> GetCityListByUser(int userID)
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
        /// 获取费用中间表中数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id">时间</param>
        /// <returns></returns>
        public EmployeeMiddleShow GetDataByID(SysEntities db, int id)
        {
            var query = from em in db.EmployeeMiddle
                        join cer in db.CompanyEmployeeRelation on em.CompanyEmployeeRelationId equals cer.Id
                        join e in db.Employee on cer.EmployeeId equals e.Id
                        join c in db.CRM_Company on cer.CompanyId equals c.ID
                        where em.Id == id
                        select new EmployeeMiddleShow
                        {
                            EmployeeId = e.Id,
                            EmployeeName = e.Name,
                            CardId = e.CertificateNumber,

                            CompanyId = c.ID,
                            CompanyName = c.CompanyName,

                            CompanyEmployeeRelationId = em.CompanyEmployeeRelationId,
                            CityId = em.CityId,
                            InsuranceKindId = em.InsuranceKindId,
                            Id = em.Id,
                            PaymentBetween = em.PaymentBetween,
                            PaymentStyle = em.PaymentStyle,
                            CompanyBasePayment = em.CompanyBasePayment,
                            CompanyPayment = em.CompanyPayment,
                            EmployeeBasePayment = em.EmployeeBasePayment,
                            EmployeePayment = em.EmployeePayment,
                            PaymentMonth = em.PaymentMonth,
                            StartDate = em.StartDate,
                            EndedDate = em.EndedDate,
                            UseBetween = em.UseBetween,
                            Remark = em.Remark,
                            State = em.State,
                            CreateTime = em.CreateTime,
                            CreatePerson = em.CreatePerson,
                            UpdateTime = em.UpdateTime,
                            UpdatePerson = em.UpdatePerson
                        };

            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 批量导入其他社保费
        /// </summary>
        /// <param name="employeeList"></param>
        /// <returns></returns>
        public int InsertList(List<EmployeeMiddle> employeeList)
        {
            using (SysEntities db = new SysEntities())
            {
                var q = db.EmployeeMiddle.AddRange(employeeList);
                Save(db);
                return q.Count();
            }
        }
    }
}
