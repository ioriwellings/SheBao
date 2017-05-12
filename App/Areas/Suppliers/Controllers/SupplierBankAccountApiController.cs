using Common;
using Langben.BLL;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierBankAccountApiController : BaseApiController
    {

        #region Get
        
        /// <summary>
        /// 获取银行帐户列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<SupplierBankAccount> queryData = m_BLL.GetByParam("", getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    ID = s.Id,
                    Supplier_ID = s.SupplierId,
                    Bank = s.Bank,
                    Account = s.Account,
                    CreateTime = s.CreateTime,
                    CreateUserID = s.CreateUserID,
                    CreateUserName = s.CreateUserName,
                    Status = s.Status
                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string Get(string id)
        {
            SupplierBankAccount item = m_BLL.GetById(id);
            SupplierBankAccount info = new SupplierBankAccount();
            info.Id = item.Id;
            info.Account = item.Account;
            info.Bank = item.Bank;
            info.SupplierId = item.SupplierId;
            return Newtonsoft.Json.JsonConvert.SerializeObject(info);
        }

        #endregion

        #region Post
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns> 
        public Common.ClientResult.Result Post([FromBody]SupplierBankAccount entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.Id = Common.Result.GetNewId();
                entity.CreateUserID = LoginInfo.UserID;
                entity.CreateTime = DateTime.Now;
                entity.CreateUserName = LoginInfo.RealName;
                entity.Status = "启用";

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，供应商银行账户信息的Id为" + entity.Id, "供应商银行账户"
                        );//写入日志 
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，供应商银行账户的信息，" + returnValue, "供应商银行账户"
                       );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Put([FromBody]SupplierBankAccount entity)
        {
            IBLL.ISupplierBankAccountBLL b_BLL = new SupplierBankAccountBLL();
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                SupplierBankAccount model = m_BLL.GetById(entity.Id);

                entity.CreateTime = model.CreateTime;
                entity.CreateUserID = model.CreateUserID;
                entity.CreateUserName = model.CreateUserName;
                entity.Status = model.Status;
                

                string returnValue = string.Empty;
                if (b_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商银行信息的Id为" + entity.Id, "供应商银行"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，供应商银行信息的Id为" + entity.Id + "," + returnValue, "供应商银行"
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

        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Stop(string id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   
                //数据校验
                SupplierBankAccount item = m_BLL.GetById(id);

                item.Status = "停用";//停用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商银行信息的Id为" + id, "供应商银行信息_停用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，供应商银行信息的Id为" + id + "," + returnValue, "供应商银行信息_停用"
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

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Start(string id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验
                SupplierBankAccount item = m_BLL.GetById(id);

                item.Status = "启用";//启用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商银行信息的Id为" + id, "供应商银行信息_启用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，供应商银行信息的Id为" + id + "," + returnValue, "供应商银行信息_启用"
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
        #endregion

        #region 内置
       
        #endregion


        IBLL.ISupplierBankAccountBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public SupplierBankAccountApiController()
            : this(new SupplierBankAccountBLL()) { }

        public SupplierBankAccountApiController(SupplierBankAccountBLL bll)
        {
            m_BLL = bll;
        }
    }
}