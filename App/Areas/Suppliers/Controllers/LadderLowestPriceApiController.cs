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

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class LadderLowestPriceApiController : BaseApiController
    {
        IBLL.ILadderLowestPriceBLL m_BLL = new BLL.LadderLowestPriceBLL();
        ValidationErrors validationErrors = new ValidationErrors();

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            List<LadderLowestPrice> queryData = m_BLL.GetByParam("", "ASC", "ID", "SupplierIdDDL_Int&" + getParam.search);
            var data = new Common.ClientResult.DataResult
            {
                total = queryData.Count,
                rows = queryData.Select(s => new
                {
                    ID = s.Id,
                    SupplierID = s.SupplierId,
                    LowestPrice = s.LowestPrice,
                    ProductID = s.ProductId,
                    AddPrice = s.AddPrice,
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
            LadderLowestPrice item = m_BLL.GetById(id);
            var rstItem = new
            {
                ID = item.Id,
                SupplierID = item.SupplierId,
                LowestPrice = item.LowestPrice,
                ProductID = item.ProductId,
                AddPrice = item.AddPrice
            };
            return rstItem;
        }

        /// <summary>
        /// 检验报价信息
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public int CheckLowestPrice(int id)
        {
            return m_BLL.CheckLowestPrice(id);
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
                string returnValue = string.Empty;
                if (m_BLL.StopPrice(id))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，最低报价信息的Id为" + id, "最低报价信息_停用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，最低报价信息的Id为" + id + "," + returnValue, "最低报价信息_停用"
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
                LadderLowestPrice item = m_BLL.GetById(id);

                //List<LadderLowestPrice> list = m_BLL.GetByParam("", "ASC", "ID", "SupplierIdDDL_Int&" + item.SupplierId + "^StatusDDL_String&" + Common.Status.启用.ToString());
                int ckh = m_BLL.CheckLowestPrice(Convert.ToInt32(item.SupplierId));

                if (ckh != 0)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "供应商只能有一条启用中的最低报价信息!";
                    return result;
                }

                item.Status = Common.Status.启用.ToString();//启用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，最低报价信息的Id为" + id, "最低报价信息_启用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息的Id为" + id + "," + returnValue, "客户_企业阶梯报价信息_启用"
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
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]LadderLowestPrice entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                LadderLowestPrice model = m_BLL.GetById(entity.Id);
                model.ProductId = entity.ProductId;
                model.LowestPrice = entity.LowestPrice;
                model.AddPrice = entity.AddPrice;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，最低报价信息的Id为" + entity.Id, "最低报价"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，最低报价信息的Id为" + entity.Id + "," + returnValue, "最低报价"
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

        public Common.ClientResult.Result PostPrice([FromBody]SupplierInfo entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                //entity.CRM_Company_ID = entity.CRM_Company_Audit_ID;

                //最低报价
                LadderLowestPrice LowestPrice = new LadderLowestPrice();
                string price = entity.Price;
                if (!string.IsNullOrEmpty(price))
                {
                    LowestPrice = GetLowestPrice(price);
                    LowestPrice.Id = Common.Result.GetNewId();
                }
                //阶梯报价
                List<LadderPrice> listLadderPrice = new List<LadderPrice>();
                string ladderPrice = entity.LadderPrice;
                if (!string.IsNullOrEmpty(ladderPrice))
                {
                    listLadderPrice = GetLadderPriceList(ladderPrice, LowestPrice.Id);
                }
                string returnValue = string.Empty;
                if (m_BLL.CreatePrice(LowestPrice, listLadderPrice))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，报价信息的Id为" + LowestPrice.Id, "报价信息"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，报价信息，" + returnValue, "报价信息"
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


        // 获取最低报价
        public LadderLowestPrice GetLowestPrice(string price)
        {
            LadderLowestPrice model = new LadderLowestPrice();
            if (!string.IsNullOrEmpty(price))
            {
                string[] arrGroup = price.Split('^');
                string[] arrItem;

                if (arrGroup.Length > 0)
                {

                    arrItem = arrGroup[0].Split('&');
                    model.ProductId = arrItem[0];
                    if (!string.IsNullOrEmpty(arrItem[1]))
                        model.LowestPrice = Convert.ToDecimal(arrItem[1]);
                    if (!string.IsNullOrEmpty(arrItem[2]))
                        model.AddPrice = Convert.ToDecimal(arrItem[2]);
                    if (!string.IsNullOrEmpty(arrItem[3]))
                        model.SupplierId = Convert.ToInt32(arrItem[3]);

                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = Common.Status.启用.ToString();
                }
            }
            return model;
        }
        // 获取阶梯报价
        public List<LadderPrice> GetLadderPriceList(string ladderPrice,string lowestPriceId)
        {
            List<LadderPrice> list = new List<LadderPrice>();
            if (!string.IsNullOrEmpty(ladderPrice))
            {
                string[] arrGroup = ladderPrice.Split('^');
                string[] arrItem;
                for (int i = 0; i < arrGroup.Length; i++)
                {
                    LadderPrice model = new LadderPrice();
                    arrItem = arrGroup[i].Split('&');

                    if (!string.IsNullOrEmpty(arrItem[0]))
                        model.SinglePrice = Convert.ToDecimal(arrItem[0]);
                    if (!string.IsNullOrEmpty(arrItem[1]))
                        model.BeginLadder = Convert.ToInt32(arrItem[1]);
                    if (!string.IsNullOrEmpty(arrItem[2]))
                        model.EndLadder = Convert.ToInt32(arrItem[2]);
                    model.LadderLowestPriceId = lowestPriceId;
                    model.Id = Common.Result.GetNewId();
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = Common.Status.启用.ToString();
                    list.Add(model);
                }
            }
            return list;
        }
    }
}