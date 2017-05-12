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

namespace Langben.App.Controllers
{
    /// <summary>
    /// 费用支出，更新对比数据
    /// </summary>
    public class COST_PayRecord_ContrastedApiController : BaseApiController
    {
        ValidationErrors validationErrors = new ValidationErrors();
        IBLL.ICOST_PayRecordStatusBLL m_BLL = new BLL.COST_PayRecordStatusBLL();
        
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam) 
        {
            // 设置搜索默认值
            string yearMonth = DateTime.Now.ToString("yyyy-MM").Replace("-", "");
            int costType = 0;  // 默认值为空
            string cityId = ""; // 缴纳地

            // 各搜索项赋值
            if (getParam != null)
            {
                if(!string.IsNullOrEmpty(getParam.search))
                {
                    string[] search = getParam.search.Split('^');
                    yearMonth = search[0].Split('&')[1];
                    costType = search[1].Split('&')[1] != "" ? Convert.ToInt32(search[1].Split('&')[1]) : 0;
                    cityId = search[2].Split('&')[1] != "" ? search[2].Split('&')[1] : "";
                }
            }


            List<COST_PayRecordSummary> queryData = m_BLL.GetPayRecordContrastedList(yearMonth, costType, cityId);
            var data = new Common.ClientResult.DataResult
            {
                rows = queryData.Select(s => new
                {
                    YearMonth = s.YearMonth
                    ,CostType = s.CostType
                    ,CostTypeName = ((Common.EmployeeAdd_InsuranceKindId)s.CostType).ToString()   // 险种
                    ,CityId = s.CityId  // 缴纳地
                    ,CityName = s.CityName  // 缴纳地名称
                    ,
                    CompanyCost = s.CompanyCost
                    ,
                    PersonCost = s.PersonCost  
                    ,
                    Sum = s.CompanyCost + s.PersonCost
                    ,
                    Count = s.Count  // 人数
                    ,Status = s.Status  // 状态（0：可对比；1：不可对比）
                })
            };
            return data;
        }

        /// <summary>
        /// 加入对比
        /// </summary>
        /// <param name="id">支出费用表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result ContrastedPayRecord(int yearMonth, int costType, string cityId)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            string returnValue = string.Empty;

            if (m_BLL.ContrastedInsurance(ref validationErrors, yearMonth, costType, cityId, LoginInfo.UserName))
            {
                LogClassModels.WriteServiceLog("加入对比成功" + "，信息的险种为" + ((Common.EmployeeAdd_InsuranceKindId)costType).ToString(), ",缴纳地Id为" + cityId + "消息"
                    );//加入对比成功，写入日志
                result.Code = Common.ClientCode.Succeed;
                result.Message = "加入对比成功";
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
                LogClassModels.WriteServiceLog("加入对比失败" + "，信息的险种为" + ((Common.EmployeeAdd_InsuranceKindId)costType).ToString(), ",缴纳地Id为" + cityId + "消息"
                    );//加入对比成功，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "加入对比失败，" + returnValue;
            }
            return result;
        }

        /// <summary>
        /// 获取缴纳地列表(社保客服自己负责的缴纳地)
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCity()
        {
            var query = m_BLL.GetSBKFCityListByUser(LoginInfo.UserID);
            var data = new Common.ClientResult.DataResult
            {
                rows = query.Select(s => new
                {
                    ID = s.Id,
                    Name = s.Name
                })
            };
            return data;
        }

        /// <summary>
        /// 获取险种(社保客服自己负责的险种)
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCostType(string cityId)
        {
            var query = m_BLL.GetSBKFCostTypeByCity(LoginInfo.UserID, cityId);

            var data = new Common.ClientResult.DataResult
            {
                rows = query.Select(s => new
                {
                    Code = s.Code,
                    Name = s.Name
                })
            };
            return data;
        }
    }
}
