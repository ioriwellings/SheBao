using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
using Langben.DAL.Model;
using System.Data.Entity.Validation;
namespace Langben.DAL
{
    /// <summary>
    /// 客户_社保信息
    /// </summary>
    public partial class CRM_Company_InsuranceRepository : BaseRepository<CRM_Company_Insurance>, IDisposable
    {
        #region 根据企业和缴纳地获取企业社保信息
        /// <summary>
        /// 根据企业和缴纳地获取企业社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <returns></returns>
        public CompanyInsurance_EditView GetByCompanyCity(SysEntities db, int companyID, string cityID)
        {
            var Company_Insurance = db.CRM_Company_Insurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID).ToList()
                .Select(c => new CRM_Company_Insurance_View() { InsuranceKindId = c.InsuranceKind, Account = c.Account, ID = c.ID }).ToList();
            var Company_PoliceInsurance = db.CRM_Company_PoliceInsurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID).ToList()
                .Select(c => new CRM_Company_PoliceInsurance_View() { InsuranceKind = c.InsuranceKind, PoliceInsuranceId = c.PoliceInsurance, ID = c.ID }).ToList();
            CompanyInsurance_EditView model = new CompanyInsurance_EditView();
            model.CRM_Company_ID = companyID;
            model.CityId = cityID;

            model.ListCI = Company_Insurance;
            model.ListCPI = Company_PoliceInsurance;

            return model;
        }
        #endregion

        #region 获取待审核的新建社保信息
        /// <summary>
        /// 获取待审核的新建社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <returns></returns>
        public CompanyInsurance GetAddData(SysEntities db, int companyID, string cityID)
        {
            int opType = (int)Common.OperateType.添加;
            int state = (int)Common.AuditStatus.待处理;
            var Company_Insurance = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == opType && c.OperateStatus == state);
            var Company_PoliceInsurance = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == opType && c.OperateStatus == state);


            CompanyInsurance model = GetInsurance_Audit(db, Company_Insurance, Company_PoliceInsurance, companyID, cityID);
            model.CityName = db.City.Where(c => c.Id == cityID).ToList().FirstOrDefault().Name;
            return model;
        }
        #endregion

        #region 获取待审核的修改社保信息
        /// <summary>
        /// 获取待审核的修改社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <returns></returns>
        public CompanyInsurance GetEditData(SysEntities db, int companyID, string cityID)
        {
            int opType = (int)Common.OperateType.修改;
            int state = (int)Common.AuditStatus.待处理;
            var Company_Insurance = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == opType && c.OperateStatus == state);
            var Company_PoliceInsurance = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == opType && c.OperateStatus == state);

            CompanyInsurance model = GetInsurance_Audit(db, Company_Insurance, Company_PoliceInsurance, companyID, cityID);
            model.CityName = db.City.Where(c => c.Id == cityID).ToList().FirstOrDefault().Name;
            return model;
        }
        #endregion

        #region 获取企业社保信息
        /// <summary>
        /// 根据企业ID获取企业社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地（可为空，为空时表示查询该企业所有缴纳的的社保信息）</param>
        /// <returns></returns>
        public List<CompanyInsurance> GetCRM_Company_Insurance(SysEntities db, int companyID, string cityID)
        {
            var Company_Insurance = db.CRM_Company_Insurance.Where(c => c.CRM_Company_ID == companyID && (cityID == "" || c.City == cityID));
            var Company_PoliceInsurance = db.CRM_Company_PoliceInsurance.Where(c => c.CRM_Company_ID == companyID && (cityID == "" || c.City == cityID));


            var Company_City = (from a in Company_Insurance
                                join b in db.City on a.City equals b.Id
                                select new CompanyInsurance()
                                            {
                                                CityId = a.City,
                                                CityName = b.Name,
                                                CRM_Company_ID = companyID,
                                                State = a.State
                                            }).Union(
                                            from a in Company_PoliceInsurance
                                            join b in db.City on a.City equals b.Id
                                            select new CompanyInsurance()
                                                        {
                                                            CityId = a.City,
                                                            CityName = b.Name,
                                                            CRM_Company_ID = companyID,
                                                            State = a.State
                                                        }
                                            )
                                            .Distinct().ToList();


            for (int i = 0; i < Company_City.Count(); i++)
            {
                string CityID = Company_City[i].CityId;
                string State = Company_City[i].State;
                //查询各社保种类的账户信息，公积金信息单独提出
                var Insurance = (from ci in Company_Insurance
                                 join b in db.InsuranceKind on ci.InsuranceKind equals b.InsuranceKindId
                                 where ci.City == CityID && ci.State == State && b.City == CityID
                                 select new CRM_Company_Insurance_View()
                                 {
                                     CRM_Company_ID = ci.CRM_Company_ID,
                                     CityId = ci.City,
                                     InsuranceKindId = ci.InsuranceKind,
                                     InsuranceKindName = b.Name,
                                     Account = ci.Account
                                 }
                        ).OrderBy(c => c.InsuranceKindId).ToList();

                string Account1 = "";
                string Account2 = "";
                foreach (var CI in Insurance)
                {
                    if (CI.InsuranceKindId == (int)Common.EmployeeAdd_InsuranceKindId.公积金)
                    {
                        Account2 = CI.Account;
                    }
                    else
                    {
                        Account1 += CI.InsuranceKindName + ":" + CI.Account + ";";
                    }
                }
                Company_City[i].Account1 = Account1;
                Company_City[i].Account2 = Account2;

                //查询各社保政策 目前只查工伤和公积金
                var PoliceInsurance = (from cp in Company_PoliceInsurance
                                       join b in db.InsuranceKind on cp.InsuranceKind equals b.InsuranceKindId
                                       join c in db.PoliceInsurance on cp.PoliceInsurance equals c.Id
                                       where cp.City == CityID && cp.State == State && b.City == CityID
                                       select new CRM_Company_PoliceInsurance_View()
                                       {
                                           CRM_Company_ID = cp.CRM_Company_ID,
                                           CityId = cp.City,
                                           InsuranceKind = cp.InsuranceKind,
                                           InsuranceKindName = b.Name,
                                           PoliceInsuranceId = cp.PoliceInsurance,
                                           PoliceInsuranceName = c.Name
                                       }).OrderBy(c => c.PoliceInsuranceId).ToList();

                foreach (var CP in PoliceInsurance)
                {
                    if (CP.InsuranceKind == (int)Common.EmployeeAdd_InsuranceKindId.公积金)
                    {
                        Company_City[i].PoliceID2 += CP.PoliceInsuranceId + ";";
                        Company_City[i].Police2 += CP.PoliceInsuranceName + ";";
                    }
                    else if (CP.InsuranceKind == (int)Common.EmployeeAdd_InsuranceKindId.工伤)
                    {
                        Company_City[i].PoliceID1 += CP.PoliceInsuranceId + ";";
                        Company_City[i].Police1 += CP.PoliceInsuranceName + ";";
                    }
                }

            }

            return Company_City;
        }
        #endregion

        #region 新建企业社保信息 数据插入到待审核表
        /// <summary>
        /// 创建企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model">社保信息实体</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public bool CreateCRM_Company_Insurance(SysEntities db, CompanyInsurance model, int userID, string userName, int branchID)
        {
            try
            {
                DateTime dtnow = DateTime.Now;
                List<CRM_Company_Insurance_Audit> ListI = new List<CRM_Company_Insurance_Audit>();
                CRM_Company_Insurance_Audit ModelI = new CRM_Company_Insurance_Audit();

                List<CRM_Company_PoliceInsurance_Audit> ListPI = new List<CRM_Company_PoliceInsurance_Audit>();
                CRM_Company_PoliceInsurance_Audit ModelPI = new CRM_Company_PoliceInsurance_Audit();

                string[] Accounts = model.Account1 == null ? null : model.Account1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] Polices1 = model.PoliceID1 == null ? null : model.PoliceID1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] Polices2 = model.PoliceID2 == null ? null : model.PoliceID2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                //公积金账户
                if (!string.IsNullOrEmpty(model.Account2))
                {
                    ModelI = new CRM_Company_Insurance_Audit();
                    ModelI.Account = model.Account2;
                    ModelI.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.公积金;
                    ModelI.CRM_Company_ID = model.CRM_Company_ID;
                    ModelI.City = model.CityId;

                    ModelI.State = Common.Status.启用.ToString();
                    ModelI.CreateTime = dtnow;
                    ModelI.CreateUserID = userID;
                    ModelI.CreatePerson = userName;
                    ModelI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                    ModelI.OperateNode = 2;//质控
                    ModelI.BranchID = branchID;
                    ModelI.OperateType = (int)Common.OperateType.添加;

                    ListI.Add(ModelI);
                }

                //社保账户
                if (!string.IsNullOrEmpty(model.Account1))
                {
                    foreach (string acc in Accounts)
                    {
                        string[] account = acc.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (account.Length == 2 && !string.IsNullOrEmpty(account[1]))
                        {

                            Common.EmployeeAdd_InsuranceKindId getType = (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), account[0]);

                            ModelI = new CRM_Company_Insurance_Audit();
                            ModelI.Account = account[1];
                            ModelI.InsuranceKind = (int)getType;

                            ModelI.CRM_Company_ID = model.CRM_Company_ID;
                            ModelI.City = model.CityId;

                            ModelI.State = Common.Status.启用.ToString();
                            ModelI.CreateTime = dtnow;
                            ModelI.CreateUserID = userID;
                            ModelI.CreatePerson = userName;
                            ModelI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                            ModelI.OperateNode = 2;//质控
                            ModelI.BranchID = branchID;
                            ModelI.OperateType = (int)Common.OperateType.添加;

                            ListI.Add(ModelI);
                        }
                    }
                }

                //公积金政策
                if (!string.IsNullOrEmpty(model.PoliceID2))
                {
                    foreach (string po in Polices2)
                    {
                        ModelPI = new CRM_Company_PoliceInsurance_Audit();

                        ModelPI.PoliceInsurance = Convert.ToInt32(po);
                        ModelPI.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.公积金;

                        ModelPI.CRM_Company_ID = model.CRM_Company_ID;
                        ModelPI.City = model.CityId;

                        ModelPI.State = Common.Status.启用.ToString();
                        ModelPI.CreateTime = dtnow;
                        ModelPI.CreateUserID = userID;
                        ModelPI.CreatePerson = userName;
                        ModelPI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                        ModelPI.OperateNode = 2;//质控
                        ModelPI.BranchID = branchID;
                        ModelPI.OperateType = (int)Common.OperateType.添加;

                        ListPI.Add(ModelPI);
                    }
                }

                //工伤政策
                if (!string.IsNullOrEmpty(model.PoliceID1))
                {
                    foreach (string po in Polices1)
                    {
                        ModelPI = new CRM_Company_PoliceInsurance_Audit();

                        ModelPI.PoliceInsurance = Convert.ToInt32(po);
                        ModelPI.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.工伤;

                        ModelPI.CRM_Company_ID = model.CRM_Company_ID;
                        ModelPI.City = model.CityId;

                        ModelPI.State = Common.Status.启用.ToString();
                        ModelPI.CreateTime = dtnow;
                        ModelPI.CreateUserID = userID;
                        ModelPI.CreatePerson = userName;
                        ModelPI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                        ModelPI.OperateNode = 2;//质控
                        ModelPI.BranchID = branchID;
                        ModelPI.OperateType = (int)Common.OperateType.添加;

                        ListPI.Add(ModelPI);
                    }
                }

                db.CRM_Company_Insurance_Audit.AddRange(ListI);
                db.CRM_Company_PoliceInsurance_Audit.AddRange(ListPI);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 检查企业和社保缴纳地组合唯一性
        /// <summary>
        /// 检查当前企业在当前缴纳地是否存在社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public int CheckCompanyCity(SysEntities db, int companyID, string cityID)
        {

            var cc = db.CRM_Company_Insurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID).ToList();
            var cc1 = db.CRM_Company_PoliceInsurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID).ToList();

            if (cc.Count > 0 || cc1.Count > 0)
            {
                return 1;//当前企业在当前缴纳地有社保信息
            }
            int state = (int)Common.AuditStatus.待处理;
            var cca = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateStatus == state).ToList();
            var cca1 = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateStatus == state).ToList();

            if (cca.Count > 0 || cca1.Count > 0)
            {
                return 2;//当前企业在当前缴纳地有正在审核的社保信息
            }

            return 0;//当前企业在当前缴纳地没有社保信息
        }
        #endregion

        #region 修改企业社保信息 数据插入到待审核表
        /// <summary>
        /// 修改企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model">社保信息实体</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public bool UpdateCRM_Company_Insurance(SysEntities db, CompanyInsurance model, int userID, string userName, int branchID)
        {
            try
            {
                DateTime dtnow = DateTime.Now;
                List<CRM_Company_Insurance_Audit> ListI = new List<CRM_Company_Insurance_Audit>();
                CRM_Company_Insurance_Audit ModelI = new CRM_Company_Insurance_Audit();

                List<CRM_Company_PoliceInsurance_Audit> ListPI = new List<CRM_Company_PoliceInsurance_Audit>();
                CRM_Company_PoliceInsurance_Audit ModelPI = new CRM_Company_PoliceInsurance_Audit();

                string[] Accounts = model.Account1 == null ? null : model.Account1.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] Polices1 = model.PoliceID1 == null ? null : model.PoliceID1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] Polices2 = model.PoliceID2 == null ? null : model.PoliceID2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


                List<CRM_Company_Insurance> ListI_Old = db.CRM_Company_Insurance.Where(c => c.City == model.CityId && c.CRM_Company_ID == model.CRM_Company_ID).OrderBy(c => c.InsuranceKind).ToList();
                List<CRM_Company_PoliceInsurance> ListPI_Old = db.CRM_Company_PoliceInsurance.Where(c => c.City == model.CityId && c.CRM_Company_ID == model.CRM_Company_ID).OrderBy(c => c.InsuranceKind).ToList();


                //公积金账户
                if (!string.IsNullOrEmpty(model.Account2))
                {
                    ModelI = new CRM_Company_Insurance_Audit();
                    ModelI.Account = model.Account2;
                    ModelI.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.公积金;
                    ModelI.CRM_Company_ID = model.CRM_Company_ID;
                    ModelI.City = model.CityId;

                    CRM_Company_Insurance oldGGJ = ListI_Old.Where(c => c.InsuranceKind == ModelI.InsuranceKind).ToList().FirstOrDefault();
                    if (oldGGJ != null)
                    {
                        ModelI.CRM_Company_Insurance_ID = oldGGJ.ID;
                    }

                    ModelI.State = Common.Status.启用.ToString();
                    ModelI.CreateTime = dtnow;
                    ModelI.CreateUserID = userID;
                    ModelI.CreatePerson = userName;
                    ModelI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                    ModelI.OperateNode = 2;//质控
                    ModelI.BranchID = branchID;
                    ModelI.OperateType = (int)Common.OperateType.修改;

                    ListI.Add(ModelI);
                }

                //社保账户
                if (!string.IsNullOrEmpty(model.Account1))
                {
                    foreach (string acc in Accounts)
                    {
                        string[] account = acc.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (account.Length == 2 && !string.IsNullOrEmpty(account[1]))
                        {

                            Common.EmployeeAdd_InsuranceKindId getType = (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), account[0]);

                            ModelI = new CRM_Company_Insurance_Audit();
                            ModelI.Account = account[1];
                            ModelI.InsuranceKind = (int)getType;

                            ModelI.CRM_Company_ID = model.CRM_Company_ID;
                            ModelI.City = model.CityId;

                            CRM_Company_Insurance oldInsurance = ListI_Old.Where(c => c.InsuranceKind == ModelI.InsuranceKind).ToList().FirstOrDefault();
                            if (oldInsurance != null)
                            {
                                ModelI.CRM_Company_Insurance_ID = oldInsurance.ID;
                            }
                            //ModelI.CRM_Company_Insurance_ID = ListI_Old.Where(c => c.InsuranceKind == ModelI.InsuranceKind).ToList().FirstOrDefault().ID;

                            ModelI.State = Common.Status.启用.ToString();
                            ModelI.CreateTime = dtnow;
                            ModelI.CreateUserID = userID;
                            ModelI.CreatePerson = userName;
                            ModelI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                            ModelI.OperateNode = 2;//质控
                            ModelI.BranchID = branchID;
                            ModelI.OperateType = (int)Common.OperateType.修改;

                            ListI.Add(ModelI);
                        }
                    }
                }
                //公积金政策
                if (!string.IsNullOrEmpty(model.PoliceID2))
                {
                    foreach (string po in Polices2)
                    {
                        ModelPI = new CRM_Company_PoliceInsurance_Audit();


                        ModelPI.PoliceInsurance = Convert.ToInt32(po);
                        ModelPI.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.公积金;

                        ModelPI.CRM_Company_ID = model.CRM_Company_ID;
                        ModelPI.City = model.CityId;

                        CRM_Company_PoliceInsurance oldPiGGJ = ListPI_Old.Where(c => c.InsuranceKind == ModelPI.InsuranceKind).ToList().FirstOrDefault();
                        if (oldPiGGJ != null)
                        {
                            ModelPI.CRM_Company_PoliceInsurance_ID = oldPiGGJ.ID;
                        }
                        //ModelPI.CRM_Company_PoliceInsurance_ID = ListPI_Old.Where(c => c.InsuranceKind == ModelPI.InsuranceKind).ToList().FirstOrDefault().ID;

                        ModelPI.State = Common.Status.启用.ToString();
                        ModelPI.CreateTime = dtnow;
                        ModelPI.CreateUserID = userID;
                        ModelPI.CreatePerson = userName;
                        ModelPI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                        ModelPI.OperateNode = 2;//质控
                        ModelPI.BranchID = branchID;
                        ModelPI.OperateType = (int)Common.OperateType.修改;

                        ListPI.Add(ModelPI);
                    }
                }


                //工伤政策
                if (!string.IsNullOrEmpty(model.PoliceID1))
                {
                    foreach (string po in Polices1)
                    {
                        ModelPI = new CRM_Company_PoliceInsurance_Audit();

                        ModelPI.PoliceInsurance = Convert.ToInt32(po);
                        ModelPI.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.工伤;

                        ModelPI.CRM_Company_ID = model.CRM_Company_ID;
                        ModelPI.City = model.CityId;

                        CRM_Company_PoliceInsurance oldPiGS = ListPI_Old.Where(c => c.InsuranceKind == ModelPI.InsuranceKind).ToList().FirstOrDefault();
                        if (oldPiGS != null)
                        {
                            ModelPI.CRM_Company_PoliceInsurance_ID = oldPiGS.ID;
                        }

                        ModelPI.State = Common.Status.启用.ToString();
                        ModelPI.CreateTime = dtnow;
                        ModelPI.CreateUserID = userID;
                        ModelPI.CreatePerson = userName;
                        ModelPI.OperateStatus = (int)Common.AuditStatus.待处理;//待处理
                        ModelPI.OperateNode = 2;//质控
                        ModelPI.BranchID = branchID;
                        ModelPI.OperateType = (int)Common.OperateType.修改;

                        ListPI.Add(ModelPI);
                    }
                }

                db.CRM_Company_Insurance_Audit.AddRange(ListI);
                db.CRM_Company_PoliceInsurance_Audit.AddRange(ListPI);

                foreach (var Iold in ListI_Old)
                {
                    Iold.State = Common.Status.修改中.ToString();
                }

                foreach (var PIold in ListPI_Old)
                {
                    PIold.State = Common.Status.修改中.ToString();
                }

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 企业社保信息停用启用
        /// <summary>
        /// 停用企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <param name="state">启用状态</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        public bool ChangeInsuranceState(SysEntities db, int companyID, string cityID, string state, string userName)
        {
            try
            {
                DateTime dtnow = DateTime.Now;
                var updI = db.CRM_Company_Insurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID);
                var updPI = db.CRM_Company_PoliceInsurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID);

                foreach (var modelI in updI)
                {
                    modelI.State = state;
                    modelI.UpdatePerson = userName;
                    modelI.UpdateTime = dtnow;
                }
                foreach (var modelPI in updPI)
                {
                    modelPI.State = state;
                    modelPI.UpdatePerson = userName;
                    modelPI.UpdateTime = dtnow;
                }
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 审核通过添加信息
        /// <summary>
        /// 审核通过新建数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool PassAdd(SysEntities db, int companyID, string cityID)
        {
            try
            {
                int operateType = (int)Common.OperateType.添加;
                int operateState = (int)Common.AuditStatus.待处理;
                //DateTime dtnow = DateTime.Now;
                var updI = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);
                var updPI = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);
                List<CRM_Company_Insurance> ListI = new List<CRM_Company_Insurance>();
                List<CRM_Company_PoliceInsurance> ListPI = new List<CRM_Company_PoliceInsurance>();
                foreach (var modelI in updI)
                {
                    //设置成功
                    modelI.OperateStatus = (int)Common.AuditStatus.成功;
                    //数据复制到正式表
                    CRM_Company_Insurance entityI = new CRM_Company_Insurance();
                    entityI.Account = modelI.Account;
                    entityI.City = modelI.City;
                    entityI.CreatePerson = modelI.CreatePerson;
                    entityI.CreateTime = modelI.CreateTime;
                    entityI.CRM_Company_ID = companyID;
                    entityI.InsuranceKind = modelI.InsuranceKind;
                    entityI.State = modelI.State;
                    ListI.Add(entityI);
                }
                foreach (var modelPI in updPI)
                {
                    //设置成功
                    modelPI.OperateStatus = (int)Common.AuditStatus.成功;
                    //数据复制到正式表
                    CRM_Company_PoliceInsurance entityPI = new CRM_Company_PoliceInsurance();
                    entityPI.City = modelPI.City;
                    entityPI.CreatePerson = modelPI.CreatePerson;
                    entityPI.CreateTime = modelPI.CreateTime;
                    entityPI.CRM_Company_ID = companyID;
                    entityPI.InsuranceKind = modelPI.InsuranceKind;
                    entityPI.PoliceInsurance = modelPI.PoliceInsurance;
                    entityPI.State = modelPI.State;
                    ListPI.Add(entityPI);
                }
                db.CRM_Company_Insurance.AddRange(ListI);
                db.CRM_Company_PoliceInsurance.AddRange(ListPI);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 审核退回添加信息
        /// <summary>
        /// 审核退回新建数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool ReturnAdd(SysEntities db, int companyID, string cityID)
        {
            try
            {
                int operateType = (int)Common.OperateType.添加;
                int operateState = (int)Common.AuditStatus.待处理;
                //DateTime dtnow = DateTime.Now;
                var updI = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);
                var updPI = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);

                foreach (var modelI in updI)
                {
                    //设置失败
                    modelI.OperateStatus = (int)Common.AuditStatus.失败;
                }
                foreach (var modelPI in updPI)
                {
                    //设置失败
                    modelPI.OperateStatus = (int)Common.AuditStatus.失败;
                }

                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 审核通过修改信息
        /// <summary>
        /// 审核通过修改信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool PassEdit(SysEntities db, int companyID, string cityID)
        {
            try
            {
                int operateType = (int)Common.OperateType.修改;
                int operateState = (int)Common.AuditStatus.待处理;
                string state = Common.Status.修改中.ToString();
                //DateTime dtnow = DateTime.Now;
                var updI = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);
                var updPI = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);

                var delI = db.CRM_Company_Insurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.State == state);
                var delPI = db.CRM_Company_PoliceInsurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.State == state);

                //删除正式表元数据
                db.CRM_Company_Insurance.RemoveRange(delI);
                db.CRM_Company_PoliceInsurance.RemoveRange(delPI);

                List<CRM_Company_Insurance> ListI = new List<CRM_Company_Insurance>();
                List<CRM_Company_PoliceInsurance> ListPI = new List<CRM_Company_PoliceInsurance>();

                foreach (var modelI in updI)
                {
                    //设置成功
                    modelI.OperateStatus = (int)Common.AuditStatus.成功;

                    //数据复制到正式表
                    CRM_Company_Insurance entityI = new CRM_Company_Insurance();
                    entityI.Account = modelI.Account;
                    entityI.City = modelI.City;
                    entityI.CreatePerson = modelI.CreatePerson;
                    entityI.CreateTime = modelI.CreateTime;
                    entityI.CRM_Company_ID = companyID;
                    entityI.InsuranceKind = modelI.InsuranceKind;
                    entityI.State = modelI.State;
                    ListI.Add(entityI);
                }
                foreach (var modelPI in updPI)
                {
                    //设置成功
                    modelPI.OperateStatus = (int)Common.AuditStatus.成功;
                    //数据复制到正式表
                    CRM_Company_PoliceInsurance entityPI = new CRM_Company_PoliceInsurance();
                    entityPI.City = modelPI.City;
                    entityPI.CreatePerson = modelPI.CreatePerson;
                    entityPI.CreateTime = modelPI.CreateTime;
                    entityPI.CRM_Company_ID = companyID;
                    entityPI.InsuranceKind = modelPI.InsuranceKind;
                    entityPI.PoliceInsurance = modelPI.PoliceInsurance;
                    entityPI.State = modelPI.State;
                    ListPI.Add(entityPI);
                }
                db.CRM_Company_Insurance.AddRange(ListI);
                db.CRM_Company_PoliceInsurance.AddRange(ListPI);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 审核退回修改加信息
        /// <summary>
        /// 审核退回修改信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool ReturnEdit(SysEntities db, int companyID, string cityID)
        {
            try
            {
                int operateType = (int)Common.OperateType.修改;
                int operateState = (int)Common.AuditStatus.待处理;
                string state = Common.Status.修改中.ToString();
                //DateTime dtnow = DateTime.Now;
                var AuditI = db.CRM_Company_Insurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);
                var AuditPI = db.CRM_Company_PoliceInsurance_Audit.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.OperateType == operateType && c.OperateStatus == operateState && c.OperateNode == 2);

                var updI = db.CRM_Company_Insurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.State == state);
                var updPI = db.CRM_Company_PoliceInsurance.Where(c => c.CRM_Company_ID == companyID && c.City == cityID && c.State == state);

                foreach (var modelI in AuditI)
                {
                    //设置失败
                    modelI.OperateStatus = (int)Common.AuditStatus.失败;
                }
                foreach (var modelPI in AuditPI)
                {
                    //设置失败
                    modelPI.OperateStatus = (int)Common.AuditStatus.失败;
                }
                foreach (var modelI in updI)
                {
                    //设置启用
                    modelI.State = Common.Status.启用.ToString();
                }
                foreach (var modelPI in updPI)
                {
                    //设置启用
                    modelPI.State = Common.Status.启用.ToString();
                }

                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 内置

        #region 拼接企业社保信息待审核数据
        /// <summary>
        /// 拼接企业社保信息待审核数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Company_Insurance"></param>
        /// <param name="Company_PoliceInsurance"></param>
        /// <param name="companyID"></param>
        /// <param name="cityID"></param>
        /// <returns></returns>
        private CompanyInsurance GetInsurance_Audit(SysEntities db, IQueryable<CRM_Company_Insurance_Audit> Company_Insurance, IQueryable<CRM_Company_PoliceInsurance_Audit> Company_PoliceInsurance, int companyID, string cityID)
        {
            CompanyInsurance Company_City = new CompanyInsurance();

            string CityID = cityID;
            //查询各社保种类的账户信息，公积金信息单独提出
            var Insurance = (from ci in Company_Insurance
                             join b in db.InsuranceKind on ci.InsuranceKind equals b.InsuranceKindId
                             where ci.City == CityID && b.City == CityID
                             select new CRM_Company_Insurance_View()
                             {
                                 CRM_Company_ID = ci.CRM_Company_ID,
                                 CityId = ci.City,
                                 InsuranceKindId = ci.InsuranceKind,
                                 InsuranceKindName = b.Name,
                                 Account = ci.Account
                             }
                    ).OrderBy(c => c.InsuranceKindId).ToList();

            string Account1 = "";
            string Account2 = "";
            foreach (var CI in Insurance)
            {
                if (CI.InsuranceKindId == (int)Common.EmployeeAdd_InsuranceKindId.公积金)
                {
                    Account2 = CI.Account;
                }
                else
                {
                    Account1 += CI.InsuranceKindName + ":" + CI.Account + ";";
                }
            }
            Company_City.Account1 = Account1;
            Company_City.Account2 = Account2;

            //查询各社保政策 目前只查工伤和公积金
            var PoliceInsurance = (from cp in Company_PoliceInsurance
                                   join b in db.InsuranceKind on cp.InsuranceKind equals b.InsuranceKindId
                                   join c in db.PoliceInsurance on cp.PoliceInsurance equals c.Id
                                   where cp.City == CityID && b.City == CityID
                                   select new CRM_Company_PoliceInsurance_View()
                                   {
                                       CRM_Company_ID = cp.CRM_Company_ID,
                                       CityId = cp.City,
                                       InsuranceKind = cp.InsuranceKind,
                                       InsuranceKindName = b.Name,
                                       PoliceInsuranceId = cp.PoliceInsurance,
                                       PoliceInsuranceName = c.Name
                                   }).OrderBy(c => c.PoliceInsuranceId).ToList();

            foreach (var CP in PoliceInsurance)
            {
                if (CP.InsuranceKind == (int)Common.EmployeeAdd_InsuranceKindId.公积金)
                {
                    Company_City.PoliceID2 += CP.PoliceInsuranceId + ";";
                    Company_City.Police2 += CP.PoliceInsuranceName + ";";
                }
                else if (CP.InsuranceKind == (int)Common.EmployeeAdd_InsuranceKindId.工伤)
                {
                    Company_City.PoliceID1 += CP.PoliceInsuranceId + ";";
                    Company_City.Police1 += CP.PoliceInsuranceName + ";";
                }
            }
            return Company_City;

        }
        #endregion

        #endregion

    }
}

