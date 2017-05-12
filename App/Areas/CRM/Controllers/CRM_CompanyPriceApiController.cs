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

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业报价
    /// </summary>
    public class CRM_CompanyPriceApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            return m_BLL.GetCompanyPirceList(Convert.ToInt32(getParam.search), LoginInfo.BranchID);
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyPrice Get(int id)
        {
            CRM_CompanyPrice item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string GetPrice(int id)
        {
            string item = m_BLL.GetCompanyPrice(id);
            return item;
        }

        public int GetActiveProduct(int id)
        {
            int ProductID = 0;
            List<CRM_CompanyPrice> list = m_BLL.GetActiveProduct(id);
            if (list.Count > 0)
            {
                ProductID = list.FirstOrDefault().PRD_Product_ID;
            }
            return ProductID;
        }

        /// <summary>
        /// 检验报价信息
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public int CheckPrice(int id)
        {
            return m_BLL.CheckPrice(id);
        }

        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Stop(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                CRM_CompanyPrice item = m_BLL.GetById(id);

                //item.Status = 0;//停用

                string returnValue = string.Empty;
                if (m_BLL.StopPrice(ref validationErrors, id))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业报价信息的Id为" + id, "客户_企业报价信息_停用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业报价信息的Id为" + id + "," + returnValue, "客户_企业报价信息_停用"
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
        public Common.ClientResult.Result Start(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                CRM_CompanyPrice item = m_BLL.GetById(id);

                int chk = m_BLL.CheckPrice(item.CRM_Company_ID);

                if (chk == 1)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "企业只能有一条报价信息!";
                    return result;
                }
                else if (chk == 2)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "企业已有一条报价信息正在审核中";
                    return result;
                }

                item.Status = 1;//启用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业报价信息的Id为" + id, "客户_企业报价信息_启用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业报价信息的Id为" + id + "," + returnValue, "客户_企业报价信息_启用"
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


        IBLL.ICRM_CompanyPriceBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyPriceApiController()
            : this(new CRM_CompanyPriceBLL()) { }

        public CRM_CompanyPriceApiController(CRM_CompanyPriceBLL bll)
        {
            m_BLL = bll;
        }

    }
}


