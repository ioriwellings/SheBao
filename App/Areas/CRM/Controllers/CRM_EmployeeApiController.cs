using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using Langben.App.Models;
using Langben.DAL.Model;
using System.Web;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 员工
    /// </summary>
    public class CRM_EmployeeApiController : BaseApiController
    {
        ValidationErrors validationErrors = new ValidationErrors();

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            int total = 0;
            List<EmployeeInfo> queryData = m_BLL.GetEmployeeList(getParam.page, getParam.rows, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.empId,
                    Name = s.Empname,
                    CertificateType = s.CertificateType,
                    CertificateNumber = s.CertificateNumber,
                    Sex = s.Sex,
                    AccountType = s.AccountType,
                    CreateTime = s.CreateTime,
                    ZRKF=s.zrkf,
                    YGKF=s.ygkf
                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string Get(int id)
        {
            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            Employee item = m_BLL.GetById(id);

            //EmployeeRepository aa = new EmployeeRepository();
            //List<EmployeeInfo> lis = aa.GetByInfo(id);

            EmployeeInfo info = new EmployeeInfo();
            info.empId = item.Id;
            info.Empname = item.Name;
            info.CertificateNumber = item.CertificateNumber;
            info.CertificateType = item.CertificateType;
            info.Sex = item.Sex;
            info.AccountType = item.AccountType;

            info.bankList = (from a in item.EmployeeBank
                             select new EmployeeBanks
                             {
                                 Bank = a.Bank,
                                 BranchBank = a.BranchBank,
                                 Account = a.Account,
                                 AccountName = a.AccountName,
                                 BState = a.State
                             }).ToList();

            info.contactList = (from a in item.EmployeeContact
                                select new EmployeeContacts
                                {
                                    Telephone = a.Telephone,
                                    MobilePhone = a.MobilePhone,
                                    CState = a.State,
                                    Email = a.Email,
                                    Address = a.Address,
                                    Remark = a.Remark
                                }).ToList();

            return Newtonsoft.Json.JsonConvert.SerializeObject(info);

        }

        /// <summary>
        /// 返回基本信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmployeeInfo Gets(int id)
        {
            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            Employee item = m_BLL.GetById(id);

            EmployeeInfo info = new EmployeeInfo();
            info.empId = item.Id;
            info.Empname = item.Name;
            info.CertificateNumber = item.CertificateNumber;
            info.CertificateType = item.CertificateType;
            info.Sex = item.Sex;
            info.AccountType = item.AccountType;
            return info;
        }

        #region Post
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result BaseEdit([FromBody]Employee entity)
        {
            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            #region 验证
            if (entity.CertificateType == null)
            {
                result.Code = Common.ClientCode.FindNull;
                result.Message = "请选择证件类型";
                return result; //提示输入的数据的格式不对         
            }
            if (entity.Sex == null)
            {
                result.Code = Common.ClientCode.FindNull;
                result.Message = "请选择性别";
                return result; //提示输入的数据的格式不对         
            }
            if (entity.AccountType == null)
            {
                result.Code = Common.ClientCode.FindNull;
                result.Message = "请选择户口类型";
                return result; //提示输入的数据的格式不对         
            }
            if (entity.CertificateType == "居民身份证")
            {
                string number = entity.CertificateNumber;

                if (Common.CardCommon.CheckCardID18(number) == false)
                {
                    result.Code = Common.ClientCode.FindNull;
                    result.Message = "证件号不正确请重新输入";
                    return result; //提示输入的数据的格式不对 
                }
            }

            #endregion
            //数据校验
            if (entity != null && ModelState.IsValid)
            {   
                Employee item = m_BLL.GetById(entity.Id);
                item.Name = entity.Name;
                item.CertificateNumber = entity.CertificateNumber;
                item.CertificateType = entity.CertificateType;
                item.Sex = entity.Sex;
                item.AccountType = entity.AccountType;

                item.UpdateTime = DateTime.Now;
                item.UpdatePerson = LoginInfo.RealName;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，员工信息的Id为" + entity.Id, "员工"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示更新成功 
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，员工信息的Id为" + entity.Id + "," + returnValue, "员工"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }


        // DELETE api/<controller>/5
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        //public Common.ClientResult.Result Delete(string query)
        //{
        //    IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
        //    Common.ClientResult.Result result = new Common.ClientResult.Result();

        //    string returnValue = string.Empty;
        //    int[] deleteId = Array.ConvertAll<string, int>(query.Split(','), delegate(string s) { return int.Parse(s); });
        //    if (deleteId != null && deleteId.Length > 0)
        //    {
        //        if (m_BLL.DeleteCollection(ref validationErrors, deleteId))
        //        {
        //            LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + string.Join(",", deleteId), "消息"
        //                );//删除成功，写入日志
        //            result.Code = Common.ClientCode.Succeed;
        //            result.Message = Suggestion.DeleteSucceed;
        //        }
        //        else
        //        {
        //            if (validationErrors != null && validationErrors.Count > 0)
        //            {
        //                validationErrors.All(a =>
        //                {
        //                    returnValue += a.ErrorMessage;
        //                    return true;
        //                });
        //            }
        //            LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", deleteId) + "," + returnValue, "消息"
        //                );//删除失败，写入日志
        //            result.Code = Common.ClientCode.Fail;
        //            result.Message = Suggestion.DeleteFail + returnValue;
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        //public Common.ClientResult.Result Post([FromBody]Employee entity)
        //{
        //    IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
        //    Common.ClientResult.Result result = new Common.ClientResult.Result();
        //    if (entity != null && ModelState.IsValid)
        //    {
        //        //string currentPerson = GetCurrentPerson();
        //        //entity.CreateTime = DateTime.Now;
        //        //entity.CreatePerson = currentPerson;


        //        string returnValue = string.Empty;
        //        if (m_BLL.Create(ref validationErrors, entity))
        //        {
        //            LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工的信息的Id为" + entity.Id, "员工"
        //                );//写入日志 
        //            result.Code = Common.ClientCode.Succeed;
        //            result.Message = Suggestion.InsertSucceed;
        //            return result; //提示创建成功
        //        }
        //        else
        //        {
        //            if (validationErrors != null && validationErrors.Count > 0)
        //            {
        //                validationErrors.All(a =>
        //                {
        //                    returnValue += a.ErrorMessage;
        //                    return true;
        //                });
        //            }
        //            LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工的信息，" + returnValue, "员工"
        //                );//写入日志                      
        //            result.Code = Common.ClientCode.Fail;
        //            result.Message = Suggestion.InsertFail + returnValue;
        //            return result; //提示插入失败
        //        }
        //    }

        //    result.Code = Common.ClientCode.FindNull;
        //    result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
        //    return result;
        //}

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Create([FromBody]EmployeeInfo entity)
        {
            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            #region 验证
            //if (entity.CertificateType == null)
            //{
            //    result.Code = Common.ClientCode.FindNull;
            //    result.Message = "请选择证件类型";
            //    return result; //提示输入的数据的格式不对         
            //}
            //if (entity.Sex == null)
            //{
            //    result.Code = Common.ClientCode.FindNull;
            //    result.Message = "请选择性别";
            //    return result; //提示输入的数据的格式不对         
            //}
            //if (entity.AccountType == null)
            //{
            //    result.Code = Common.ClientCode.FindNull;
            //    result.Message = "请选择户口类型";
            //    return result; //提示输入的数据的格式不对         
            //}
            //if (entity.CertificateType == "居民身份证")
            //{
            //    string number = entity.BasicInfo.CertificateNumber;

            //    if (Common.CardCommon.CheckCardID18(number) == false)
            //    {
            //        result.Code = Common.ClientCode.FindNull;
            //        result.Message = "证件号不正确请重新输入";
            //        return result; //提示输入的数据的格式不对 
            //    }
            //}
            //if (entity.CompanyId==0)
            //{
            //    result.Code = Common.ClientCode.FindNull;
            //    result.Message = "请选择所属公司";
            //    return result; //提示输入的数据的格式不对 
            //}
            //if (string.IsNullOrEmpty(entity.Citys))
            //{
            //    result.Code = Common.ClientCode.FindNull;
            //    result.Message = "请选择社保缴纳地";
            //    return result; //提示输入的数据的格式不对 
            //}
            //if (entity.PoliceAccountNatureId == 0)
            //{
            //    result.Code = Common.ClientCode.FindNull;
            //    result.Message = "请选择户口性质";
            //    return result; //提示输入的数据的格式不对 

            //}
            #endregion
            //if (entity != null && ModelState.IsValid)
            //{
                Employee baseModel = entity.BasicInfo;//基本信息    
                baseModel.AccountType = entity.AccountType;
                baseModel.CertificateType = entity.CertificateType;
                baseModel.Sex = entity.Sex;
                baseModel.CreateTime = DateTime.Now;
                baseModel.CreatePerson = LoginInfo.RealName;

                EmployeeContact contact = entity.empContacts;
                contact.CreatePerson = LoginInfo.RealName;
                contact.CreateTime = DateTime.Now;

                EmployeeBank bank = entity.empBank;
                if (bank.AccountName == null && bank.Bank == null && bank.BranchBank == null && bank.Account == null)
                {
                    bank = null;
                }
                else
                {
                    bank.CreatePerson = LoginInfo.RealName;
                    bank.CreateTime = DateTime.Now;
                }
                
                CompanyEmployeeRelation relation = new CompanyEmployeeRelation();
                relation.CityId = entity.Citys;
                relation.State = "在职";
                relation.CompanyId = entity.CompanyId;
                relation.CreateTime=DateTime.Now;
                relation.CreatePerson=LoginInfo.RealName;
                relation.Station=entity.Station;
                relation.PoliceAccountNatureId=entity.PoliceAccountNatureId;


                string returnValue = string.Empty;
                if (m_BLL.EmployeeAdd(ref validationErrors, baseModel, contact, bank, relation))
                {
                    //LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业信息_待审核的信息的Id为" + entity.ID, "客户_企业信息_待审核"
                    //);//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed;
                    return result; //提示创建成功
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    //LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业信息_待审核的信息，" + returnValue, "客户_企业信息_待审核"
                    //    );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    return result; //提示插入失败
                }
            //}
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }
        #endregion

        #region 内置
        //验证用户名唯一
        public Common.ClientResult.Result CheckCertificateNumber(string CertificateNumber,string types)
        {
            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            int count = m_BLL.CheckCertificateNumber(CertificateNumber);
            if (count == 0)
            {
                if (types == "1")
                {
                    if (Common.CardCommon.CheckCardID18(CertificateNumber) == false)
                    {
                        result.Code = Common.ClientCode.Fail;
                        result.Message = "证件号不正确请重新输入";
                    }
                    else
                    {
                        result.Code = Common.ClientCode.FindNull;
                        result.Message = "该用户可以使用！";
                    }
                }
                else
                {
                    result.Code = Common.ClientCode.FindNull;
                    result.Message = "该用户可以使用！";
                }
            }
            else
            {
                result.Code = Common.ClientCode.Succeed;
                result.Message = "该用户已存在！";
            }
            return result;
        }
        #endregion
    }
}


