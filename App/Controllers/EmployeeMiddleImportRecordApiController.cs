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

namespace Langben.App.Controllers
{
    /// <summary>
    /// 导入其他社保费记录
    /// </summary>
    public class EmployeeMiddleImportRecordApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string createUser = string.Empty;
            string beginTime = string.Empty;
            string endTime = string.Empty;
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                createUser = search[0];
                beginTime = search[1];
                endTime = search[2];
            }

            List<EmployeeMiddleImportRecord> queryData = m_BLL.GetDataByParam(getParam.id, getParam.page, getParam.rows, createUser, beginTime, endTime, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id
					,URL = s.URL
					,ImportCount = s.ImportCount
					,ImportPayment = s.ImportPayment
					,CreateTime = s.CreateTime
					,CreateUserID = s.CreateUserID
					,CreateUserName = s.CreateUserName
					

                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public EmployeeMiddleImportRecord Get(int id)
        {
            EmployeeMiddleImportRecord item = m_BLL.GetById(id);
            return item;
        }
 
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]EmployeeMiddleImportRecord entity)
        {           

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                //string currentPerson = GetCurrentPerson();
                //entity.CreateTime = DateTime.Now;
                //entity.CreatePerson = currentPerson;
              
                   
                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed  + "，导入其他社保费记录的信息的Id为" + entity.Id,"导入其他社保费记录"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，导入其他社保费记录的信息，" + returnValue,"导入其他社保费记录"
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

        IBLL.IEmployeeMiddleImportRecordBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeMiddleImportRecordApiController()
            : this(new EmployeeMiddleImportRecordBLL()) { }

        public EmployeeMiddleImportRecordApiController(EmployeeMiddleImportRecordBLL bll)
        {
            m_BLL = bll;
        }
        
    }
}


