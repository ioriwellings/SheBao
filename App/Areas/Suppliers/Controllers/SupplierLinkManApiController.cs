using Common;
using Langben.BLL;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierLinkManApiController : BaseApiController
    {
        #region Get
        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public dynamic Get(string id)
        {
            SupplierLinkMan model = m_BLL.GetById(id);
            var item = new
            {
                Id = model.Id,
                SupplierId = model.SupplierId,
                Name = model.Name,
                Position = model.Position,
                Address = model.Address,
                Mobile = model.Mobile,
                Telephone = model.Telephone,
                Email = model.Email,
                Remark = model.Remark,
                CreateTime = model.CreateTime,
                CreateUserID = model.CreateUserID,
                CreateUserName = model.CreateUserName,
                Status=model.Status,
                IsDefault = model.IsDefault
            };
            return item;
        }
        #endregion

        #region Post
        /// <summary>
        /// 得到列表数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<SupplierLinkMan> queryData = m_BLL.GetByParam("", getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id,
                    SupplierId = s.SupplierId,
                    Name = s.Name,
                    Position = s.Position,
                    Address = s.Address,
                    Mobile = s.Mobile,
                    Telephone = s.Telephone,
                    Email = s.Email,
                    Remark = s.Remark,
                    IsDefault = s.IsDefault == "N" ? "否" : "是",
                    CreateTime = s.CreateTime,
                    CreateUserID = s.CreateUserID,
                    CreateUserName = s.CreateUserName,
                    Status = s.Status
                })
            };
            return data;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]SupplierLinkMan entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.Id = Common.Result.GetNewId();
                entity.IsDefault="N";
                entity.CreateTime = DateTime.Now;
                entity.CreateUserID = LoginInfo.UserID.ToString();
                entity.CreateUserName = LoginInfo.RealName;
                entity.Status = Common.Status.启用.ToString();

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，供应商联系人信息的Id为" + entity.Id, "添加供应商联系人信息"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，供应商联系人信息，" + returnValue, "添加供应商联系人信息"
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
        /// 修改联系人信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Put([FromBody]SupplierLinkMan entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                //SupplierLinkMan model = m_BLL.GetById(entity.Id);
                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，供应商联系人信息的Id为" + entity.Id, "供应商联系人信息"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业联系人信息_待审核的信息，" + returnValue, "客户_企业联系人信息_待审核"
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
        /// 停用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Stop(string id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                SupplierLinkMan item = m_BLL.GetById(id);
                item.Status = Common.Status.停用.ToString();//停用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商的Id为" + id, "供应商_停用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，供应商的Id为" + id + "," + returnValue, "供应商_停用"
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

                SupplierLinkMan item = m_BLL.GetById(id);

                item.Status = Common.Status.启用.ToString();//启用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商联系人的Id为" + id, "供应商联系人_启用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，供应商联系人的Id为" + id + "," + returnValue, "供应商联系人_启用"
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
        /// 设置默认联系人
        /// </summary>
        /// <param name="id">联系人id</param>
        /// <returns></returns>
        public Common.ClientResult.Result SetDefault(string id,int supplierID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            string returnValue = string.Empty;
            if (m_BLL.SetDefault(ref validationErrors, id, supplierID))
            {
                LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商联系人的Id为" + id, "供应商联系人_设置默认联系人"
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
                LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业联系人的Id为" + id + "," + returnValue, "客户_企业联系人_设置默认联系人"
                    );//写入日志   
                result.Code = Common.ClientCode.Fail;
                result.Message = Suggestion.UpdateFail + returnValue;
                return result; //提示更新失败
            }

        }
        #endregion 

        #region Private
        #endregion

          IBLL.ISupplierLinkManBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public SupplierLinkManApiController()
            : this(new SupplierLinkManBLL()) { }

        public SupplierLinkManApiController(SupplierLinkManBLL bll)
        {
            m_BLL = bll;
        }
    }
}
