using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 员工
    /// </summary>
    public partial class EmployeeRepository : BaseRepository<Employee>, IDisposable
    {
        /// <summary>
        /// 通过主键id，获取员工---查看详细
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>员工</returns>
        public List<EmployeeInfo> GetByInfo(int id)
        {
            using (SysEntities db = new SysEntities())
            {
                var aa = from employee in db.Employee
                         join bank in db.EmployeeBank on employee.Id equals bank.EmployeeId into empbank
                         from newbank in empbank.DefaultIfEmpty()

                         join contact in db.EmployeeContact on employee.Id equals contact.EmployeeId into empcontact
                         from newcontact in empcontact.DefaultIfEmpty()
                         where employee.Id == id
                         select new EmployeeInfo()
                         {
                             Empname = employee.Name,
                             CertificateNumber = employee.CertificateNumber,
                             CertificateType = employee.CertificateType,
                             Sex = employee.Sex,
                             AccountType = employee.AccountType,
                             Telephone = newcontact.Telephone,
                             MobilePhone = newcontact.MobilePhone,
                             CState = newcontact.State,
                             Email = newcontact.Email,
                             Address = newcontact.Address,
                             CRemark = newcontact.Remark,
                             Bank = newbank.Bank,
                             BranchBank = newbank.BranchBank,
                             Account = newbank.Account,
                             AccountName = newbank.AccountName,
                             BState = newbank.State
                         };

                return aa.ToList();
            }
        }

        /// <summary>
        /// 查询员工列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort"></param>
        /// <param name="search"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<EmployeeInfo> GetEmployeeList(SysEntities db, string search)
        {
            var query = from employee in db.Employee
                        join relation in db.CompanyEmployeeRelation
                        on employee.Id equals relation.EmployeeId into lg1
                        from relation in lg1.DefaultIfEmpty()
                        join branch in db.CRM_CompanyToBranch on relation.CompanyId equals branch.CRM_Company_ID into lg2
                        from branch in lg2.DefaultIfEmpty()

                        select new EmployeeInfo()
                        {
                            empId = employee.Id,
                            Empname = employee.Name,
                            CertificateType = employee.CertificateType,
                            CertificateNumber = employee.CertificateNumber,
                            Sex = employee.Sex,
                            AccountType = employee.AccountType,
                            CreateTime = employee.CreateTime,
                            zrkf = branch.UserID_ZR,
                            ygkf = branch.UserID_YG
                        };
            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
                {
                    string str = queryDic["Name"];
                    query = query.Where(a => a.Empname.Contains(str));
                }

                if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
                {
                    string str = queryDic["CertificateNumber"];
                    string[] numList = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> CARDLIST = new List<string>();
                    for (int i = 0; i < numList.Length; i++)
                    {
                        numList[i] = numList[i].TrimEnd(',');
                        CARDLIST.Add(numList[i]);
                        CARDLIST.Add(CardCommon.CardIDTo15(numList[i]));
                        CARDLIST.Add(CardCommon.CardIDTo18(numList[i]));
                    }
                    CARDLIST = CARDLIST.Distinct().ToList();
                    query = query.Where(o => CARDLIST.Contains(o.CertificateNumber));
                }
            }
            return query.OrderByDescending(o => o.CreateTime);

            //获取员工姓名查询员工信息
            //IQueryable<Employee> employee = db.Employee.Where(o => true);

            //Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            //if (queryDic != null && queryDic.Count > 0)
            //{
            //    if (queryDic.ContainsKey("Name") && !string.IsNullOrWhiteSpace(queryDic["Name"]))
            //    {
            //        string str = queryDic["Name"];
            //        employee = employee.Where(a => a.Name.Contains(str));
            //    }

            //    if (queryDic.ContainsKey("CertificateNumber") && !string.IsNullOrWhiteSpace(queryDic["CertificateNumber"]))
            //    {
            //        string str = queryDic["CertificateNumber"];
            //        string[] numList = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //        List<string> CARDLIST = new List<string>();
            //        for (int i = 0; i < numList.Length; i++)
            //        {
            //            numList[i] = numList[i].TrimEnd(',');
            //            CARDLIST.Add(numList[i]);
            //            CARDLIST.Add(CardCommon.CardIDTo15(numList[i]));
            //            CARDLIST.Add(CardCommon.CardIDTo18(numList[i]));
            //        }
            //        CARDLIST = CARDLIST.Distinct().ToList();
            //        employee = employee.Where(o => CARDLIST.Contains(o.CertificateNumber));
            //    }
            //}

            //return employee;
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="contactModel"></param>
        /// <param name="bankModel"></param>
        /// <returns></returns>
        public int EmployeeAdd(Employee baseModel, EmployeeContact contactModel, EmployeeBank bankModel, CompanyEmployeeRelation relationModel)
        {
            using (SysEntities db = new SysEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //创建公司
                        db.Employee.Add(baseModel);
                        db.SaveChanges();

                        //添加联系方式
                        contactModel.EmployeeId = baseModel.Id;
                        db.EmployeeContact.Add(contactModel);

                        //添加银行信息
                        if (bankModel != null)
                        {
                            bankModel.EmployeeId = baseModel.Id;
                            db.EmployeeBank.Add(bankModel);
                        }

                        //企业员工关系
                        relationModel.EmployeeId = baseModel.Id;
                        db.CompanyEmployeeRelation.Add(relationModel);

                        db.SaveChanges();
                        tran.Commit();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        #region 批量添加员工信息


        public bool EmployeeListAdd(List<EmployeeAddExcle> empList, string createPerson)
        {
            using (SysEntities db = new SysEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {



                        if (empList != null)
                        {
                            foreach (EmployeeAddExcle employ in empList)
                            {
                                Employee employee = new Employee();//员工表
                                EmployeeContact employeeContact = new EmployeeContact();//联系人表
                                CompanyEmployeeRelation companyEmployeeRelation = new CompanyEmployeeRelation();//员工企业关系表
                                EmployeeBank employeebank = new EmployeeBank();//员工银行


                                //员工基本
                                employee.Name = employ.Empname;
                                employee.CertificateType = employ.CertificateType;
                                employee.CertificateNumber = employ.CertificateNumber;
                                employee.AccountType = employ.AccountType;
                                employee.Sex = Common.CardCommon.Getsex(employ.CertificateNumber) == 1 ? "男" : "女";
                                employee.Birthday = Common.CardCommon.GetShengRi(employ.CertificateNumber);
                                employee.CreateTime = DateTime.Now;
                                employee.CreatePerson = createPerson;
                                db.Employee.Add(employee);


                                //联系人
                                employeeContact.Telephone = employ.Telephone;
                                employeeContact.MobilePhone = employ.MobilePhone;
                                employeeContact.Address = employ.Address;
                                employeeContact.Email = employ.Email;
                                employeeContact.EmployeeId = employee.Id;
                                employeeContact.State = "启用";
                                employeeContact.CreateTime = DateTime.Now;
                                employeeContact.CreatePerson = createPerson;
                                db.EmployeeContact.Add(employeeContact);
                                db.SaveChanges();

                                // 银行信息
                                //if (!string.IsNullOrWhiteSpace(employeebank.AccountName))
                                {
                                    employeebank.EmployeeId = employee.Id;
                                    employeebank.AccountName = employ.AccountName;
                                    employeebank.Bank = employ.Bank;
                                    employeebank.BranchBank = employ.BranchBank;
                                    employeebank.Account = employ.Account;
                                    employeebank.IsDefault = employ.Account;
                                    employeebank.State = "启用";
                                    employeebank.CreateTime = DateTime.Now;
                                    employeebank.CreatePerson = createPerson;
                                    db.EmployeeBank.Add(employeebank);
                                    db.SaveChanges();
                                }


                                //员工关系表
                                companyEmployeeRelation.CityId = employ.City;
                                companyEmployeeRelation.CompanyId = employ.CompanyId;
                                companyEmployeeRelation.EmployeeId = employee.Id;
                                companyEmployeeRelation.State = "在职";
                                companyEmployeeRelation.Station = employ.Station;
                                companyEmployeeRelation.PoliceAccountNatureId = employ.PoliceAccountNature;
                                companyEmployeeRelation.CreateTime = DateTime.Now;
                                companyEmployeeRelation.CreatePerson = createPerson;
                                db.CompanyEmployeeRelation.Add(companyEmployeeRelation);
                                db.SaveChanges();
                            }
                        }


                        tran.Commit();
                        return true;
                    }
                    //catch
                    //{
                    //    tran.Rollback();

                    //    return false;
                    //}

                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        tran.Rollback();
                        foreach (var errors in ex.EntityValidationErrors)
                        {
                            foreach (var item in errors.ValidationErrors)
                            {
                                Console.WriteLine(item.ErrorMessage + item.PropertyName);
                            }
                        }
                        return false;
                    }

                }
            }
        }
        #endregion

        /// <summary>
        /// 验证身份证号唯一
        /// </summary>
        /// <param name="CertificateNumber">身份证号</param>
        /// <returns></returns>
        public int CheckCertificateNumber(string CertificateNumber)
        {
            using (SysEntities db = new SysEntities())
            {
                var list = db.Employee.Where(e => e.CertificateNumber.Equals(CertificateNumber));

                return list.Count();
            }
        }
    }
}

