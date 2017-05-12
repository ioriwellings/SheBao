using Common;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class LadderPriceApiController : BaseApiController
    {
        IBLL.ILadderPriceBLL m_BLL = new BLL.LadderPriceBLL();
        ValidationErrors validationErrors = new ValidationErrors();

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int supplierID = Convert.ToInt32(getParam.search);
            List<LadderPrice> queryData = m_BLL.GetBySupplierId(supplierID);

            var data = new Common.ClientResult.DataResult
            {
                total = queryData.Count,
                rows = queryData.Select(s => new
                {
                    ID = s.Id,
                    SupplierID = supplierID,
                    LadderLowestPriceId = s.LadderLowestPriceId,
                    SinglePrice = s.SinglePrice,
                    BeginLadder = s.BeginLadder,
                    EndLadder = s.EndLadder,
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
        public dynamic Get(string id)
        {
            LadderPrice item = m_BLL.GetById(id);
            var rstItem = new
            {
                Id = item.Id,
                LadderLowestPriceId = item.LadderLowestPriceId,
                BeginLadder = item.BeginLadder,
                EndLadder = item.EndLadder,
                SinglePrice = item.SinglePrice
            };
            return rstItem;
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

                LadderPrice item = m_BLL.GetById(id);

                item.Status = Common.Status.停用.ToString();//停用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，阶梯报价信息的Id为" + id, "阶梯报价信息_停用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，阶梯报价信息的Id为" + id + "," + returnValue, "阶梯报价信息_停用"
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

                LadderPrice item = m_BLL.GetById(id);
                IBLL.ILadderLowestPriceBLL bll = new BLL.LadderLowestPriceBLL();
                LadderLowestPrice chk = bll.GetById(item.LadderLowestPriceId);

                if (chk.Status == Common.Status.停用.ToString())
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "该阶梯价格的最低价格处于停用状态！";
                    return result;
                }

                if (!m_BLL.CheckRange(item.LadderLowestPriceId, Convert.ToInt32(item.BeginLadder), Convert.ToInt32(item.EndLadder), item.Id))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，阶梯报价信息的Id为" + id, "阶梯报价信息_启用"
                      );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }

                item.Status = Common.Status.启用.ToString();//启用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，阶梯报价信息的Id为" + id, "阶梯报价信息_启用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，阶梯报价信息的Id为" + id + "," + returnValue, "阶梯报价信息_启用"
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
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]LadderPrice entity, int id)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            //IBLL.ILadderPriceBLL bll = new LadderPriceBLL();
            if (entity != null && ModelState.IsValid)
            {
                int currentPerson = LoginInfo.UserID;
                string currentPersonName = LoginInfo.RealName;
                int intBranchID = LoginInfo.BranchID;
                entity.CreateTime = DateTime.Now;
                entity.CreateUserID = currentPerson;
                entity.CreateUserName = currentPersonName;
                entity.BranchID = intBranchID;

                entity.Id = Common.Result.GetNewId();
                entity.Status = Common.Status.启用.ToString();

                IBLL.ILadderLowestPriceBLL l_BLL = new BLL.LadderLowestPriceBLL();
                LadderLowestPrice lowest = l_BLL.GetByParam("", "ASC", "ID", "SupplierIdDDL_Int&" + id + "^StatusDDL_String&" + entity.Status).FirstOrDefault();
                if (!string.IsNullOrEmpty(lowest.Id))
                {
                    entity.LadderLowestPriceId = lowest.Id;
                }
                else
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，创建阶梯报价信息", "阶梯报价信息"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }
                if (!m_BLL.CheckRange(entity.LadderLowestPriceId, Convert.ToInt32(entity.BeginLadder), Convert.ToInt32(entity.EndLadder), ""))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，创建阶梯报价信息", "阶梯报价信息"
                      );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，创建阶梯报价信息的Id", "阶梯报价信息"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业阶梯报价_待审核的信息，" + returnValue, "客户_企业阶梯报价_待审核"
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
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]LadderPrice entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            if (entity != null && ModelState.IsValid)
            {
                //数据校验
                if (!m_BLL.CheckRange(entity.LadderLowestPriceId, Convert.ToInt32(entity.BeginLadder), Convert.ToInt32(entity.EndLadder), entity.Id))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，阶梯报价信息的Id为" + entity.Id, "阶梯报价"
                      );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }

                LadderPrice model = m_BLL.GetById(entity.Id);
                model.SinglePrice = entity.SinglePrice;
                model.BeginLadder = entity.BeginLadder;
                model.EndLadder = entity.EndLadder;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，阶梯报价信息的Id为" + entity.Id, "阶梯报价"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，阶梯报价信息的Id为" + entity.Id + "," + returnValue, "阶梯报价"
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
    }
}