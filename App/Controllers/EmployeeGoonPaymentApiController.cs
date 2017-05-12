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
using System.Data.Entity.Validation;
using System.Data.Linq.SqlClient;
using System.Transactions;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
namespace Langben.App.Controllers
{
    /// <summary>
    /// 员工补缴
    /// </summary>
    public class EmployeeGoonPaymentApiController : BaseApiController
    {
        #region 生成代码
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<EmployeeGoonPayment> queryData = m_BLL.GetByParam(getParam.id, getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id
                    ,
                    InsuranceMonth = s.InsuranceMonth
                    ,
                    EmployeeAddId = s.EmployeeAddIdOld
                    ,
                    EndTime = s.EndTime
                    ,
                    StartTime = s.StartTime
                    ,
                    Remark = s.Remark
                    ,
                    State = s.State
                    ,
                    CreateTime = s.CreateTime
                    ,
                    CreatePerson = s.CreatePerson
                    ,
                    UpdateTime = s.UpdateTime
                    ,
                    UpdatePerson = s.UpdatePerson


                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public EmployeeGoonPayment Get(int id)
        {
            EmployeeGoonPayment item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]EmployeeGoonPayment entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工补缴的信息的Id为" + entity.Id, "员工补缴"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工补缴的信息，" + returnValue, "员工补缴"
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

        // PUT api/<controller>/5
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]EmployeeGoonPayment entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {   //数据校验

                //string currentPerson = GetCurrentPerson();
                //entity.UpdateTime = DateTime.Now;
                //entity.UpdatePerson = currentPerson;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，员工补缴信息的Id为" + entity.Id, "员工补缴"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，员工补缴信息的Id为" + entity.Id + "," + returnValue, "员工补缴"
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
        public Common.ClientResult.Result Delete(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int[] deleteId = Array.ConvertAll<string, int>(query.Split(','), delegate(string s) { return int.Parse(s); });
            if (deleteId != null && deleteId.Length > 0)
            {
                if (m_BLL.DeleteCollection(ref validationErrors, deleteId))
                {
                    LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + string.Join(",", deleteId), "消息"
                        );//删除成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.DeleteSucceed;
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
                    LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", deleteId) + "," + returnValue, "消息"
                        );//删除失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.DeleteFail + returnValue;
                }
            }
            return result;
        }

        IBLL.IEmployeeGoonPaymentBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeGoonPaymentApiController()
            : this(new EmployeeGoonPaymentBLL()) { }

        public EmployeeGoonPaymentApiController(EmployeeGoonPaymentBLL bll)
        {
            m_BLL = bll;
        }
        #endregion
        SysEntities SysEntitiesO2O = new SysEntities();
        #region 责任客服补缴列表 敬

        /// <summary>
        /// 责任客服补缴列表(调基列表)
        /// </summary>
        /// <param name="getParam"></param>
        /// <param name="type">1：补缴,2：调基</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostEmployeeList([FromBody]GetDataParam getParam, int type)
        {
            var EmployeeAddBLL = new BLL.EmployeeAddBLL();

            int total = 0;
            string search = getParam.search;
            if (type == 1)
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    search = "State&" + Common.EmployeeAdd_State.申报成功.ToString() + "^State&" + Common.EmployeeAdd_State.员工客服已确认.ToString() + "^State&" + Common.EmployeeAdd_State.社保专员已提取.ToString() + "^State&" + Common.EmployeeAdd_State.待员工客服确认.ToString() + "^";
                }
                else
                {
                    search += "State&" + Common.EmployeeAdd_State.申报成功.ToString() + "^State&" + Common.EmployeeAdd_State.员工客服已确认.ToString() + "^State&" + Common.EmployeeAdd_State.社保专员已提取.ToString() + "^State&" + Common.EmployeeAdd_State.待员工客服确认.ToString() + "^";
                }
            }
            if (type == 2)
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    search = "State&" + Common.EmployeeAdd_State.申报成功.ToString() + "^";
                }
                else
                {
                    search += "State&" + Common.EmployeeAdd_State.申报成功.ToString() + "^";
                }
            }
            List<EmployeeApprove> queryData = EmployeeAddBLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 责任客服补缴添加页面初始化 敬
        /// <summary>
        /// 责任客服补缴添加页面初始化
        /// </summary>
        /// <param name="CompanyEmployeeRelationId">员工关系id</param>
        /// <param name="type">保险类别</param>
        /// <returns></returns>
        public IHttpActionResult getEmployeeAddList(int CompanyEmployeeRelationId, int type, int? Parameter)
        {
            string[] State = new string[]
                {
                     Common.EmployeeAdd_State.申报成功.ToString(),  Common.EmployeeAdd_State.员工客服已确认.ToString(),
                     Common.EmployeeAdd_State.社保专员已提取.ToString(),Common.EmployeeAdd_State.待员工客服确认.ToString()                 
                };


            if (type == 1)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.养老;
                // string State = Common.EmployeeAdd_State.申报成功.ToString();
                //if (Parameter == 1)
                //{
                //    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                //}
                //else if (Parameter == 2)
                //{
                //    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                //}   
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    yanglao_id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode
                    // Name = s.Name

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && State.Contains(c.State)).FirstOrDefault());
                return Json(data);
            }
            else if (type == 2)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.医疗;
                // string State = Common.EmployeeAdd_State.申报成功.ToString();
                //if (Parameter == 1)
                //{
                //    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                //}
                //else if (Parameter == 2)
                //{
                //    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                //}
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    yiliao_id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode
                    // Name = s.Name

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && State.Contains(c.State)).FirstOrDefault());
                return Json(data);
            }
            else if (type == 3)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.工伤;
                // string State = Common.EmployeeAdd_State.申报成功.ToString();
                //if (Parameter == 1)
                //{
                //    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                //}
                //else if (Parameter == 2)
                //{
                //    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                //}
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    gongshang_id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode
                    // Name = s.Name

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && State.Contains(c.State)).FirstOrDefault());
                return Json(data);
            }
            else if (type == 4)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.失业;
                //string State = Common.EmployeeAdd_State.申报成功.ToString();
                //if (Parameter == 1)
                //{
                //    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                //}
                //else if (Parameter == 2)
                //{
                //    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                //}
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    shiye_id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode
                    // Name = s.Name
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && State.Contains(c.State)).FirstOrDefault());
                return Json(data);
            }
            else if (type == 5)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.公积金;
                //string State = Common.EmployeeAdd_State.申报成功.ToString();
                //if (Parameter == 1)
                //{
                //    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                //}
                //else if (Parameter == 2)
                //{
                //    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                //}
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    gongjijin_id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode
                    // Name = s.Name
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && State.Contains(c.State)).FirstOrDefault());

                return Json(data);
            }
            else if (type == 6)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.生育;
                // string State = Common.EmployeeAdd_State.申报成功.ToString();
                //if (Parameter == 1)
                //{
                //    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                //}
                //else if (Parameter == 2)
                //{
                //    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                //}
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    shengyu_id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode
                    // Name = s.Name

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && State.Contains(c.State)).FirstOrDefault());
                return Json(data);
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 责任客服补缴添加 敬
        #region 补缴添加
        public Common.ClientResult.Result POSTEmployeeGoonPaymentCREATE([FromBody]EmployeeGoonPaymentModels postinfos)
        {
            string EmployeeGoonPayment = EmployeeGoonPaymentcommit(postinfos);
            try
            {
                if (EmployeeGoonPayment == "")
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "补缴成功";
                    return result;
                }
                else
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Fail;
                    result.Message = EmployeeGoonPayment;
                    return result;
                }
            }
            catch (Exception er)
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Fail;
                result.Message = er.Message;
                return result;
            }
        }
        #endregion
        #region 系统验证 敬
        public string verification(EmployeeGoonPaymentModels postinfos)
        {
            StringBuilder Error = new StringBuilder();
            //不可以补缴的情况
            string[] BUJIAO_STATUS = new string[]
                {
                     Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString(),  Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString(),
                     Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString(),Common.EmployeeAdd_State.社保专员已提取.ToString(),   Common.EmployeeAdd_State.申报成功.ToString()                  
                };
            DateTime QJ_Yanglao = DateTime.MinValue;
            DateTime QJ_Yiliao = DateTime.MinValue;
            DateTime QJ_Gongshang = DateTime.MinValue;
            DateTime QJ_Shiye = DateTime.MinValue;
            DateTime QJ_Gongjijin = DateTime.MinValue;
            DateTime QJ_Shengyu = DateTime.MinValue;
            DateTime.TryParse(postinfos.YANGLAO_StartTime, out QJ_Yanglao);
            DateTime.TryParse(postinfos.YILIAO_StartTime, out QJ_Yiliao);
            DateTime.TryParse(postinfos.GONGSHANG_StartTime, out QJ_Gongshang);
            DateTime.TryParse(postinfos.SHIYE_StartTime, out QJ_Shiye);
            DateTime.TryParse(postinfos.GONGJIJIN_StartTime, out QJ_Gongjijin);
            DateTime.TryParse(postinfos.SHENGYU_StartTime, out QJ_Shengyu);
            DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
            #region 养老不能补缴情况
            if (QJ_Yanglao != DateTime.MinValue)
            {
                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.YANGLAO_EmployeeAddID);
                int ZC_Yanglao_ID = (int)EmployeeAdd.PoliceInsuranceId;
                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Yanglao_ID);
                var End_DATE = Convert.ToDateTime(postinfos.YANGLAO_EndTime);
                #region 验证补缴月重复
                var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == postinfos.YANGLAO_EmployeeAddID && (BUJIAO_STATUS.Contains(x.State)));
                if (BUJIAO_YANZHENG.Count() > 0)
                {
                    foreach (var v in BUJIAO_YANZHENG)
                    {

                        var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Yanglao);
                        var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                        if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                        {
                            string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                            Error.Append("" + yanzheng + "<br />");
                        }
                    }
                }
                #endregion
                #region 不允许补缴
                if (PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                {

                    Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                }
                else
                {

                    //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                    if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                    {
                        Error.Append("此社保机构养老只允许补缴 " + PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                    }
                }
                if (Business.CHA_Months(QJ_Yanglao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                {
                    Error.Append("养老起缴时间超出政策允许补缴数<br />");
                }
                if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) == 0)
                {
                    Error.Append("养老补缴不能报社保月<br />");
                }
                #endregion

            }
            #endregion

            #region 医疗不能补缴情况
            if (QJ_Yiliao != DateTime.MinValue)
            {
                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.YILIAO_EmployeeAddID);
                int ZC_Yiliao_ID = (int)EmployeeAdd.PoliceInsuranceId;
                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Yiliao_ID);
                var End_DATE = Convert.ToDateTime(postinfos.YILIAO_EndTime);
                #region 验证补缴月重复
                var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == postinfos.YILIAO_EmployeeAddID && (BUJIAO_STATUS.Contains(x.State)));
                if (BUJIAO_YANZHENG.Count() > 0)
                {
                    foreach (var v in BUJIAO_YANZHENG)
                    {

                        var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Yiliao);
                        var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                        if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                        {
                            string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                            Error.Append("" + yanzheng + "<br />");
                        }
                    }
                }
                #endregion
                #region 不允许补缴
                if (PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                {

                    Error.Append("此社保机构 医疗不允许补缴，请修改起缴时间或社保政策后再申报！");

                }
                else
                {
                    //if (QJ_Yiliao.AddMonths((int)PoliceInsurance.MaxPayMonth) < End_DATE.AddMonths((int)PoliceInsurance.InsuranceAdd))
                    if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Yiliao.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                    {
                        Error.Append("此社保机构 医疗只允许补缴 " + PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                    }
                }
                if (Business.CHA_Months(QJ_Yiliao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                {
                    Error.Append("医疗起缴时间超出政策允许补缴数<br />");
                }
                if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) == 0)
                {
                    Error.Append("医疗补缴不能报社保月<br />");
                }
                #endregion

            }
            #endregion

            #region 工伤不能补缴情况
            if (QJ_Gongshang != DateTime.MinValue)
            {
                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.GONGSHANG_EmployeeAddID);
                int ZC_Gongshang_ID = (int)EmployeeAdd.PoliceInsuranceId;
                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Gongshang_ID);
                var End_DATE = Convert.ToDateTime(postinfos.GONGSHANG_EndTime);
                #region 验证补缴月重复
                var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == postinfos.GONGSHANG_EmployeeAddID && (BUJIAO_STATUS.Contains(x.State)));
                if (BUJIAO_YANZHENG.Count() > 0)
                {
                    foreach (var v in BUJIAO_YANZHENG)
                    {

                        var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Gongshang);
                        var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                        if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                        {
                            string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                            Error.Append("" + yanzheng + "<br />");
                        }
                    }
                }
                #endregion
                #region 不允许补缴
                if (PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                {

                    Error.Append("此社保机构工伤不允许补缴，请修改起缴时间或社保政策后再申报！");

                }
                else
                {
                    //if (QJ_Gongshang.AddMonths((int)PoliceInsurance.MaxPayMonth) < End_DATE.AddMonths((int)PoliceInsurance.InsuranceAdd))
                    if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Gongshang.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                    {
                        Error.Append("此社保机构工伤只允许补缴 " + PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                    }
                }
                if (Business.CHA_Months(QJ_Gongshang, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                {
                    Error.Append("工伤起缴时间超出政策允许补缴数<br />");
                }
                if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) == 0)
                {
                    Error.Append("工伤补缴不能报社保月<br />");
                }
                #endregion

            }
            #endregion

            #region 失业不能补缴情况
            if (QJ_Shiye != DateTime.MinValue)
            {
                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.SHIYE_EmployeeAddID);
                int ZC_Shiye_ID = (int)EmployeeAdd.PoliceInsuranceId;
                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Shiye_ID);
                var End_DATE = Convert.ToDateTime(postinfos.SHIYE_EndTime);
                #region 验证补缴月重复
                var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == postinfos.SHIYE_EmployeeAddID && (BUJIAO_STATUS.Contains(x.State)));
                if (BUJIAO_YANZHENG.Count() > 0)
                {
                    foreach (var v in BUJIAO_YANZHENG)
                    {

                        var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Shiye);
                        var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                        if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                        {
                            string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                            Error.Append("" + yanzheng + "<br />");
                        }
                    }
                }
                #endregion
                #region 不允许补缴
                if (PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                {

                    Error.Append("此社保机构失业不允许补缴，请修改起缴时间或社保政策后再申报！");

                }
                else
                {
                    //if (QJ_Shiye.AddMonths((int)PoliceInsurance.MaxPayMonth) < End_DATE.AddMonths((int)PoliceInsurance.InsuranceAdd))
                    if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Shiye.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                    {
                        Error.Append("此社保机构失业只允许补缴 " + PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                    }
                }
                if (Business.CHA_Months(QJ_Shiye, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                {
                    Error.Append("失业起缴时间超出政策允许补缴数<br />");
                }
                if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) == 0)
                {
                    Error.Append("失业补缴不能报社保月<br />");
                }
                #endregion

            }
            #endregion

            #region 公积金不能补缴情况
            if (QJ_Gongjijin != DateTime.MinValue)
            {
                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.GONGJIJIN_EmployeeAddID);
                int ZC_Gongjijin_ID = (int)EmployeeAdd.PoliceInsuranceId;
                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Gongjijin_ID);
                var End_DATE = Convert.ToDateTime(postinfos.GONGJIJIN_EndTime);
                #region 验证补缴月重复
                var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == postinfos.GONGJIJIN_EmployeeAddID && (BUJIAO_STATUS.Contains(x.State)));
                if (BUJIAO_YANZHENG.Count() > 0)
                {
                    foreach (var v in BUJIAO_YANZHENG)
                    {

                        var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Gongjijin);
                        var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                        if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                        {
                            string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                            Error.Append("" + yanzheng + "<br />");
                        }
                    }
                }
                #endregion
                #region 不允许补缴
                if (PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                {

                    Error.Append("此社保机构公积金不允许补缴，请修改起缴时间或社保政策后再申报！");

                }
                else
                {
                    //if (QJ_Gongjijin.AddMonths((int)PoliceInsurance.MaxPayMonth) < End_DATE.AddMonths((int)PoliceInsurance.InsuranceAdd))
                    if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Gongjijin.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                    {
                        Error.Append("此社保机构公积金只允许补缴 " + PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                    }
                }
                if (Business.CHA_Months(QJ_Gongjijin, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                {
                    Error.Append("公积金起缴时间超出政策允许补缴数<br />");
                }
                if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) == 0)
                {
                    Error.Append("公积金补缴不能报社保月<br />");
                }
                #endregion

            }
            #endregion

            #region 生育不能补缴情况
            if (QJ_Shengyu != DateTime.MinValue)
            {
                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.SHENGYU_EmployeeAddID);
                int ZC_Shengyu_ID = (int)EmployeeAdd.PoliceInsuranceId;
                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Shengyu_ID);
                var End_DATE = Convert.ToDateTime(postinfos.SHENGYU_EndTime);
                #region 验证补缴月重复
                var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == postinfos.SHENGYU_EmployeeAddID && (BUJIAO_STATUS.Contains(x.State)));
                if (BUJIAO_YANZHENG.Count() > 0)
                {
                    foreach (var v in BUJIAO_YANZHENG)
                    {

                        var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Shengyu);
                        var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                        if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                        {
                            string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                            Error.Append("" + yanzheng + "<br />");
                        }
                    }
                }
                #endregion
                #region 不允许补缴
                if (PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                {

                    Error.Append("此社保机构生育不允许补缴，请修改起缴时间或社保政策后再申报！");

                }
                else
                {
                    //if (QJ_Shengyu.AddMonths((int)PoliceInsurance.MaxPayMonth) < End_DATE.AddMonths((int)PoliceInsurance.InsuranceAdd))
                    if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Shengyu.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                    {
                        Error.Append("此社保机构生育只允许补缴 " + PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                    }
                }
                if (Business.CHA_Months(QJ_Shengyu, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                {
                    Error.Append("生育起缴时间超出政策允许补缴数<br />");
                }
                if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) == 0)
                {
                    Error.Append("生育补缴不能报社保月<br />");
                }
                #endregion

            }
            #endregion

            return Error.ToString();

        }
        #endregion
        #region 提交 敬
        /// <summary>
        /// 社保补缴提交
        /// </summary>
        /// <param name="postinfos">补缴初始值</param>
        /// <returns></returns>
        private string EmployeeGoonPaymentcommit([FromBody]EmployeeGoonPaymentModels postinfos)
        {

            StringBuilder sbError = new StringBuilder();

            string error = "";
            error = verification(postinfos);

            if (error != "")
            {
                sbError.Append("导入失败以下信息错误：<br />");
                sbError.Append(error);
                return sbError.ToString();
            }
            #region 提交
            else
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {

                        var EmployeeGoonPaymentbll = new BLL.EmployeeGoonPaymentBLL();
                        var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                        DateTime QJ_Yanglao = DateTime.MinValue;
                        DateTime QJ_Yiliao = DateTime.MinValue;
                        DateTime QJ_Gongshang = DateTime.MinValue;
                        DateTime QJ_Shiye = DateTime.MinValue;
                        DateTime QJ_Gongjijin = DateTime.MinValue;
                        DateTime QJ_Shengyu = DateTime.MinValue;
                        DateTime.TryParse(postinfos.YANGLAO_StartTime, out QJ_Yanglao);
                        DateTime.TryParse(postinfos.YILIAO_StartTime, out QJ_Yiliao);
                        DateTime.TryParse(postinfos.GONGSHANG_StartTime, out QJ_Gongshang);
                        DateTime.TryParse(postinfos.SHIYE_StartTime, out QJ_Shiye);
                        DateTime.TryParse(postinfos.GONGJIJIN_StartTime, out QJ_Gongjijin);
                        DateTime.TryParse(postinfos.SHENGYU_StartTime, out QJ_Shengyu);
                        DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
                        #region 养老
                        if (QJ_Yanglao != DateTime.MinValue)
                        {
                            decimal GZ_Yanglao = (decimal)postinfos.YANGLAO_Wage;
                            var End_DATE = Convert.ToDateTime(postinfos.YANGLAO_EndTime);
                            var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.YANGLAO_EmployeeAddID);
                            var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                            int ZC_Yanglao_ID = (int)EmployeeAdd.PoliceInsuranceId;
                            var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                            var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                            var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                            var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Yanglao_ID);
                            #region 补缴表
                            EmployeeGoonPayment EmployeeGoonPayment_YANGLAO = new EmployeeGoonPayment();
                            EmployeeGoonPayment_YANGLAO.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);
                            EmployeeGoonPayment_YANGLAO.EmployeeAddId = postinfos.YANGLAO_EmployeeAddID;
                            EmployeeGoonPayment_YANGLAO.StartTime = QJ_Yanglao;
                            EmployeeGoonPayment_YANGLAO.EndTime = End_DATE;
                            EmployeeGoonPayment_YANGLAO.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            EmployeeGoonPayment_YANGLAO.CreateTime = DateTime.Now;
                            EmployeeGoonPayment_YANGLAO.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                            EmployeeGoonPayment_YANGLAO.CreatePerson = LoginInfo.UserName;
                            EmployeeGoonPaymentbll.CreateEmployeeGoonPayment(SysEntitiesO2O, EmployeeGoonPayment_YANGLAO);
                            #endregion
                            #region 补缴中间表
                            Int32 Months = Business.CHA_Months(QJ_Yanglao, End_DATE) + 1;
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_Yanglao_ID); ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_Yanglao_ID); ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            employeeMiddle_BJ.PaymentBetween = QJ_Yanglao.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                            #endregion


                        }
                        #endregion

                        #region 医疗
                        if (QJ_Yiliao != DateTime.MinValue)
                        {
                            decimal GZ_Yiliao = (decimal)postinfos.YILIAO_Wage;
                            var End_DATE = Convert.ToDateTime(postinfos.YILIAO_EndTime);
                            var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.YILIAO_EmployeeAddID);
                            var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                            int ZC_Yiliao_ID = (int)EmployeeAdd.PoliceInsuranceId;
                            var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                            var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                            var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                            var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Yiliao_ID);
                            #region 补缴表
                            EmployeeGoonPayment EmployeeGoonPayment_YILIAO = new EmployeeGoonPayment();
                            EmployeeGoonPayment_YILIAO.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);
                            EmployeeGoonPayment_YILIAO.EmployeeAddId = postinfos.YILIAO_EmployeeAddID;
                            EmployeeGoonPayment_YILIAO.StartTime = QJ_Yiliao;
                            EmployeeGoonPayment_YILIAO.EndTime = End_DATE;
                            EmployeeGoonPayment_YILIAO.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            EmployeeGoonPayment_YILIAO.CreateTime = DateTime.Now;
                            EmployeeGoonPayment_YILIAO.CreatePerson = LoginInfo.UserName;
                            EmployeeGoonPayment_YILIAO.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                            EmployeeGoonPaymentbll.CreateEmployeeGoonPayment(SysEntitiesO2O, EmployeeGoonPayment_YILIAO);
                            #endregion
                            #region 补缴中间表


                            Int32 Months = Business.CHA_Months(QJ_Yiliao, End_DATE) + 1;
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_Yiliao_ID); ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_Yiliao_ID); ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            employeeMiddle_BJ.PaymentBetween = QJ_Yiliao.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);


                            #endregion


                        }
                        #endregion

                        #region 工伤
                        if (QJ_Gongshang != DateTime.MinValue)
                        {
                            decimal GZ_Gongshang = (decimal)postinfos.GONGSHANG_Wage;
                            var End_DATE = Convert.ToDateTime(postinfos.GONGSHANG_EndTime);
                            var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.GONGSHANG_EmployeeAddID);
                            var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                            int ZC_Gongshang_ID = (int)EmployeeAdd.PoliceInsuranceId;
                            var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                            var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                            var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                            var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Gongshang_ID);
                            #region 补缴表
                            EmployeeGoonPayment EmployeeGoonPayment_GONGSHANG = new EmployeeGoonPayment();
                            EmployeeGoonPayment_GONGSHANG.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);
                            EmployeeGoonPayment_GONGSHANG.EmployeeAddId = postinfos.GONGSHANG_EmployeeAddID;
                            EmployeeGoonPayment_GONGSHANG.StartTime = QJ_Gongshang;
                            EmployeeGoonPayment_GONGSHANG.EndTime = End_DATE;
                            EmployeeGoonPayment_GONGSHANG.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            EmployeeGoonPayment_GONGSHANG.CreateTime = DateTime.Now;
                            EmployeeGoonPayment_GONGSHANG.CreatePerson = LoginInfo.UserName;
                            EmployeeGoonPayment_GONGSHANG.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                            EmployeeGoonPaymentbll.CreateEmployeeGoonPayment(SysEntitiesO2O, EmployeeGoonPayment_GONGSHANG);
                            #endregion
                            #region 补缴中间表


                            Int32 Months = Business.CHA_Months(QJ_Shengyu, End_DATE) + 1;
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_Gongshang_ID); ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_Gongshang_ID); ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            employeeMiddle_BJ.PaymentBetween = QJ_Gongshang.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);


                            #endregion


                        }
                        #endregion

                        #region 失业
                        if (QJ_Shiye != DateTime.MinValue)
                        {
                            decimal GZ_Shiye = (decimal)postinfos.SHIYE_Wage;
                            var End_DATE = Convert.ToDateTime(postinfos.SHIYE_EndTime);
                            var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.SHIYE_EmployeeAddID);
                            var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                            int ZC_Shiye_ID = (int)EmployeeAdd.PoliceInsuranceId;
                            var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                            var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                            var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                            var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Shiye_ID);
                            #region 补缴表
                            EmployeeGoonPayment EmployeeGoonPayment_SHIYE = new EmployeeGoonPayment();
                            EmployeeGoonPayment_SHIYE.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);
                            EmployeeGoonPayment_SHIYE.EmployeeAddId = postinfos.SHIYE_EmployeeAddID;
                            EmployeeGoonPayment_SHIYE.StartTime = QJ_Shiye;
                            EmployeeGoonPayment_SHIYE.EndTime = End_DATE;
                            EmployeeGoonPayment_SHIYE.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            EmployeeGoonPayment_SHIYE.CreateTime = DateTime.Now;
                            EmployeeGoonPayment_SHIYE.CreatePerson = LoginInfo.UserName;
                            EmployeeGoonPayment_SHIYE.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                            EmployeeGoonPaymentbll.CreateEmployeeGoonPayment(SysEntitiesO2O, EmployeeGoonPayment_SHIYE);
                            #endregion
                            #region 补缴中间表


                            Int32 Months = Business.CHA_Months(QJ_Shengyu, End_DATE) + 1;
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_Shiye_ID); ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_Shiye_ID); ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            employeeMiddle_BJ.PaymentBetween = QJ_Shiye.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);


                            #endregion


                        }
                        #endregion

                        #region 公积金
                        if (QJ_Gongjijin != DateTime.MinValue)
                        {
                            decimal GZ_Gongjijin = (decimal)postinfos.GONGJIJIN_Wage;
                            var End_DATE = Convert.ToDateTime(postinfos.GONGJIJIN_EndTime);
                            var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.GONGJIJIN_EmployeeAddID);
                            var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                            int ZC_Gongjijin_ID = (int)EmployeeAdd.PoliceInsuranceId;
                            var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                            var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                            var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                            var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Gongjijin_ID);
                            #region 补缴表
                            EmployeeGoonPayment EmployeeGoonPayment_GONGJIJIN = new EmployeeGoonPayment();
                            EmployeeGoonPayment_GONGJIJIN.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);
                            EmployeeGoonPayment_GONGJIJIN.EmployeeAddId = postinfos.GONGJIJIN_EmployeeAddID;
                            EmployeeGoonPayment_GONGJIJIN.StartTime = QJ_Gongjijin;
                            EmployeeGoonPayment_GONGJIJIN.EndTime = End_DATE;
                            EmployeeGoonPayment_GONGJIJIN.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            EmployeeGoonPayment_GONGJIJIN.CreateTime = DateTime.Now;
                            EmployeeGoonPayment_GONGJIJIN.CreatePerson = LoginInfo.UserName;
                            EmployeeGoonPayment_GONGJIJIN.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                            EmployeeGoonPaymentbll.CreateEmployeeGoonPayment(SysEntitiesO2O, EmployeeGoonPayment_GONGJIJIN);
                            #endregion
                            #region 补缴中间表


                            Int32 Months = Business.CHA_Months(QJ_Shengyu, End_DATE) + 1;
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_Gongjijin_ID); ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_Gongjijin_ID); ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            employeeMiddle_BJ.PaymentBetween = QJ_Gongjijin.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);


                            #endregion


                        }
                        #endregion

                        #region 生育
                        if (QJ_Shengyu != DateTime.MinValue)
                        {
                            decimal GZ_Shengyu = (decimal)postinfos.SHENGYU_Wage;
                            var End_DATE = Convert.ToDateTime(postinfos.SHENGYU_EndTime);
                            var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == postinfos.SHENGYU_EmployeeAddID);
                            var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                            int ZC_Shengyu_ID = (int)EmployeeAdd.PoliceInsuranceId;
                            var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                            var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                            var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                            var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_Shengyu_ID);
                            #region 补缴表
                            EmployeeGoonPayment EmployeeGoonPayment_SHENGYU = new EmployeeGoonPayment();
                            EmployeeGoonPayment_SHENGYU.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);
                            EmployeeGoonPayment_SHENGYU.EmployeeAddId = postinfos.SHENGYU_EmployeeAddID;
                            EmployeeGoonPayment_SHENGYU.StartTime = QJ_Shengyu;
                            EmployeeGoonPayment_SHENGYU.EndTime = End_DATE;
                            EmployeeGoonPayment_SHENGYU.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            EmployeeGoonPayment_SHENGYU.CreateTime = DateTime.Now;
                            EmployeeGoonPayment_SHENGYU.CreatePerson = LoginInfo.UserName;
                            EmployeeGoonPayment_SHENGYU.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                            EmployeeGoonPaymentbll.CreateEmployeeGoonPayment(SysEntitiesO2O, EmployeeGoonPayment_SHENGYU);
                            #endregion
                            #region 补缴中间表


                            Int32 Months = Business.CHA_Months(QJ_Shengyu, End_DATE) + 1;
                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_Shengyu_ID); ;
                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_EmployeePayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_Shengyu_ID); ;
                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            employeeMiddle_BJ.PaymentBetween = QJ_Shengyu.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                            #endregion


                        }
                        #endregion
                        //9.最后保存
                        SysEntitiesO2O.SaveChanges();
                        scope.Complete();
                        return "";
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var errors in ex.EntityValidationErrors)
                    {
                        foreach (var item in errors.ValidationErrors)
                        {
                            sb.Append(item.ErrorMessage + ";\r\n");
                        }
                    }
                    return sb.ToString();
                }



            #endregion
            }

        }

        #endregion
        #endregion

        #region 责任客服审核平台补缴数据 韩

        #region 获取补缴列表
        /// <summary>
        /// 责任客服审核列表查询
        /// </summary>b
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostApproveList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString() + "^";
            }

            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 确认通过
        /// <summary>
        /// 责任客服审核通过
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Approved(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] approvedId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (approvedId != null && approvedId.Length > 0)
            {
                if (m_BLL.EmployeeGoonPaymentApproved(ref validationErrors, approvedId, Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString(), Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString()))
                {
                    LogClassModels.WriteServiceLog("审核通过成功" + "，信息的Id为" + string.Join(",", approvedId), "消息"
                        );//审核通过成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "审核成功";
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
                    LogClassModels.WriteServiceLog("审核通过失败" + "，信息的Id为" + string.Join(",", approvedId) + "," + returnValue, "消息"
                        );//审核通过失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "审核失败" + returnValue;
                }
            }
            return result;
        }

        #endregion
        #endregion

        #region 补缴详情页面公共方法 敬
        /// <summary>
        /// 补缴详情页面公共方法
        /// </summary>
        /// <param name="CompanyEmployeeRelationId">员工关系id</param>
        /// <param name="type">保险类别</param>
        /// <param name="Parameter">状态参数</param>
        /// <returns></returns>
        public IHttpActionResult getEmployeeGoonPaymentList(int CompanyEmployeeRelationId, int type, int? Parameter, int YearMonth)
        {
            if (type == 1)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.养老;
                string State = Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeGoonPayment.Select(c => new
                {
                    Id = c.Id,
                    yanglao_id = c.EmployeeAddId,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.EmployeeAdd.InsuranceKindId,
                    Wage = c.EmployeeAdd.Wage,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    State = c.State,
                    YearMonth = c.YearMonth
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State && c.YearMonth == YearMonth).FirstOrDefault());

                return Json(data);
            }
            else if (type == 2)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.医疗;
                string State = Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeGoonPayment.Select(c => new
                {
                    Id = c.Id,
                    yanglao_id = c.EmployeeAddId,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.EmployeeAdd.InsuranceKindId,
                    Wage = c.EmployeeAdd.Wage,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    State = c.State,
                    YearMonth = c.YearMonth
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State && c.YearMonth == YearMonth).FirstOrDefault());

                return Json(data);
            }
            else if (type == 3)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.工伤;
                string State = Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeGoonPayment.Select(c => new
                {
                    Id = c.Id,
                    yanglao_id = c.EmployeeAddId,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.EmployeeAdd.InsuranceKindId,
                    Wage = c.EmployeeAdd.Wage,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    State = c.State,
                    YearMonth = c.YearMonth
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State && c.YearMonth == YearMonth).FirstOrDefault());

                return Json(data);
            }
            else if (type == 4)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.失业;
                string State = Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeGoonPayment.Select(c => new
                {
                    Id = c.Id,
                    yanglao_id = c.EmployeeAddId,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.EmployeeAdd.InsuranceKindId,
                    Wage = c.EmployeeAdd.Wage,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    State = c.State,
                    YearMonth = c.YearMonth
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State && c.YearMonth == YearMonth).FirstOrDefault());

                return Json(data);
            }
            else if (type == 5)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.公积金;

                string State = Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeGoonPayment.Select(c => new
                {
                    Id = c.Id,
                    yanglao_id = c.EmployeeAddId,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.EmployeeAdd.InsuranceKindId,
                    Wage = c.EmployeeAdd.Wage,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    State = c.State,
                    YearMonth = c.YearMonth
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State && c.YearMonth == YearMonth).FirstOrDefault());

                return Json(data);
            }
            else if (type == 6)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.生育;
                string State = Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeGoonPayment.Select(c => new
                {
                    Id = c.Id,
                    yanglao_id = c.EmployeeAddId,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.EmployeeAdd.InsuranceKindId,
                    Wage = c.EmployeeAdd.Wage,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    State = c.State,
                    YearMonth = c.YearMonth
                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State && c.YearMonth == YearMonth).FirstOrDefault());

                return Json(data);
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 待责任客服确认退回
        /// <summary>
        /// 责任客服退回
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result GoBack(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] approvedId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (approvedId != null && approvedId.Length > 0)
            {
                if (m_BLL.EmployeeGoonPaymentApproved(ref validationErrors, approvedId, Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString(), Common.EmployeeGoonPayment_STATUS.责任客服未通过.ToString()))
                {
                    LogClassModels.WriteServiceLog("退回成功" + "，信息的Id为" + string.Join(",", approvedId), "消息"
                        );//退回成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "退回成功";
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
                    LogClassModels.WriteServiceLog("退回失败" + "，信息的Id为" + string.Join(",", approvedId) + "," + returnValue, "消息"
                        );//退回失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "退回失败" + returnValue;
                }
            }
            return result;
        }
        #endregion

        #region 员工客服终止 王帅
        /// <summary>
        /// 员工客服终止
        /// </summary>
        /// <param name="ids"></param>      
        /// <returns></returns>
        public Common.ClientResult.Result POSTEmployeeStop(string ids)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            try
            {
                string Nowstate = EmployeeMiddle_PaymentStyle.补中间段.ToString();

                //string enable = Status.启用.ToString();
                //string disable = Status.停用.ToString();
                int?[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int?>(strArray, s => int.Parse(s));
                using (var ent = new SysEntities())
                {
                    var updateEmpGoonPayment = ent.EmployeeGoonPayment.Where(a => intArray.Contains(a.Id));
                    if (updateEmpGoonPayment != null && updateEmpGoonPayment.Count() >= 1)
                    {
                        foreach (var item in updateEmpGoonPayment)
                        {
                            item.State = EmployeeGoonPayment_STATUS.终止.ToString();
                            item.UpdatePerson = LoginInfo.UserName;
                            item.UpdateTime = DateTime.Now;

                            DateTime starttimeread = (DateTime)item.StartTime;
                            DateTime endtimeread = (DateTime)item.EndTime;

                            string starttimestr = starttimeread.ToString("yyyy-MM-dd").Substring(0, 7).Replace("-", "");
                            string endtimestr = endtimeread.ToString("yyyy-MM-dd").Substring(0, 7).Replace("-", "");
                            string alltimestr = starttimestr + "-" + endtimestr;

                            var updateEmpAddmiddle = ent.EmployeeMiddle.Where(a => a.InsuranceKindId == item.EmployeeAdd.InsuranceKindId && a.CompanyEmployeeRelationId == item.EmployeeAdd.CompanyEmployeeRelationId && a.State == Nowstate && a.PaymentBetween == alltimestr);
                            if (updateEmpAddmiddle != null && updateEmpAddmiddle.Count() >= 1)
                            {
                                foreach (EmployeeMiddle itemn in updateEmpAddmiddle)
                                {
                                    itemn.State = EmployeeGoonPayment_STATUS.终止.ToString();
                                    itemn.UpdateTime = DateTime.Now;
                                    item.UpdatePerson = LoginInfo.UserName;
                                }
                            }
                        }
                        ent.SaveChanges();
                    }
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "终止成功";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = e.Message;
                return result;
            }
        }
        #endregion

        #region 员工客服通过 王帅
        /// <summary>
        /// 员工客服通过
        /// </summary>
        /// <param name="ids"></param>      
        /// <returns></returns>
        public Common.ClientResult.Result POSTEmployeepass(string ids)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            try
            {
                int?[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int?>(strArray, s => int.Parse(s));
                using (var ent = new SysEntities())
                {
                    var updateEmpGoonPayment = ent.EmployeeGoonPayment.Where(a => intArray.Contains(a.Id));
                    if (updateEmpGoonPayment != null && updateEmpGoonPayment.Count() >= 1)
                    {
                        foreach (var item in updateEmpGoonPayment)
                        {
                            item.State = EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                            item.UpdatePerson = LoginInfo.UserName;
                            item.UpdateTime = DateTime.Now;

                        }
                        ent.SaveChanges();
                    }
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "确认成功";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = e.Message;
                return result;
            }
        }
        #endregion

        #region 责任客服修改员工信息列表 敬

        /// <summary>
        /// 责任客服修改员工信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostCustomerModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString() + "^";
            }
            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 修改限制 敬
        private string verificationBacknumber(EmployeeGoonPaymentModels postinfos, int? YANGLAObujiao_id, int? YILIAObujiao_id, int? GONGSHANGbujiao_id, int? SHIYEbujiao_id, int? GONGJIJINbujiao_id, int? SHENGYUbujiao_id)
        {
            StringBuilder Error = new StringBuilder();
            //不可以补缴的情况
            string[] BUJIAO_STATUS = new string[]
                {
                     Common.EmployeeGoonPayment_STATUS.待责任客服确认.ToString(),  Common.EmployeeGoonPayment_STATUS.待员工客服确认.ToString(),
                     Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString(),Common.EmployeeAdd_State.社保专员已提取.ToString(),   Common.EmployeeAdd_State.申报成功.ToString()                  
                };
            DateTime QJ_Yanglao = DateTime.MinValue;
            DateTime QJ_Yiliao = DateTime.MinValue;
            DateTime QJ_Gongshang = DateTime.MinValue;
            DateTime QJ_Shiye = DateTime.MinValue;
            DateTime QJ_Gongjijin = DateTime.MinValue;
            DateTime QJ_Shengyu = DateTime.MinValue;
            DateTime.TryParse(postinfos.YANGLAO_StartTime, out QJ_Yanglao);
            DateTime.TryParse(postinfos.YILIAO_StartTime, out QJ_Yiliao);
            DateTime.TryParse(postinfos.GONGSHANG_StartTime, out QJ_Gongshang);
            DateTime.TryParse(postinfos.SHIYE_StartTime, out QJ_Shiye);
            DateTime.TryParse(postinfos.GONGJIJIN_StartTime, out QJ_Gongjijin);
            DateTime.TryParse(postinfos.SHENGYU_StartTime, out QJ_Shengyu);
            DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
            #region 养老不能补缴情况
            if (QJ_Yanglao != DateTime.MinValue)
            {
                var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == YANGLAObujiao_id);

                var End_DATE = Convert.ToDateTime(postinfos.YANGLAO_EndTime);

                if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Yanglao) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                {
                    #region 验证补缴月重复
                    var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == EmployeeGoonPayment.EmployeeAddId && (BUJIAO_STATUS.Contains(x.State)) && x.Id != YANGLAObujiao_id);
                    if (BUJIAO_YANZHENG.Count() > 0)
                    {
                        foreach (var v in BUJIAO_YANZHENG)
                        {

                            var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Yanglao);
                            var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                            if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                            {
                                string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                                Error.Append("" + yanzheng + "<br />");
                            }
                        }
                    }
                    #endregion
                    #region 不允许补缴
                    if (EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                    {

                        Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                    }
                    else
                    {

                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd), QJ_Yanglao.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)) < 0)
                        {
                            Error.Append("此社保机构养老只允许补缴 " + EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                        }
                    }
                    if (Business.CHA_Months(QJ_Yanglao, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) > EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("养老起缴时间超出政策允许补缴数<br />");
                    }
                    if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) == 0)
                    {
                        Error.Append("养老补缴不能报社保月<br />");
                    }
                    #endregion
                }

            }
            #endregion
            #region 医疗不能补缴情况
            if (QJ_Yiliao != DateTime.MinValue)
            {
                var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == YILIAObujiao_id);

                var End_DATE = Convert.ToDateTime(postinfos.YILIAO_EndTime);

                if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Yiliao) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                {
                    #region 验证补缴月重复
                    var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == EmployeeGoonPayment.EmployeeAddId && (BUJIAO_STATUS.Contains(x.State)) && x.Id != YILIAObujiao_id);
                    if (BUJIAO_YANZHENG.Count() > 0)
                    {
                        foreach (var v in BUJIAO_YANZHENG)
                        {

                            var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Yiliao);
                            var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                            if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                            {
                                string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                                Error.Append("" + yanzheng + "<br />");
                            }
                        }
                    }
                    #endregion
                    #region 不允许补缴
                    if (EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                    {

                        Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                    }
                    else
                    {

                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd), QJ_Yiliao.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)) < 0)
                        {
                            Error.Append("此社保机构医疗只允许补缴 " + EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                        }
                    }
                    if (Business.CHA_Months(QJ_Yiliao, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) > EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("医疗起缴时间超出政策允许补缴数<br />");
                    }
                    if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) == 0)
                    {
                        Error.Append("医疗补缴不能报社保月<br />");
                    }
                    #endregion
                }

            }
            #endregion
            #region 工伤不能补缴情况
            if (QJ_Gongshang != DateTime.MinValue)
            {
                var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == GONGSHANGbujiao_id);

                var End_DATE = Convert.ToDateTime(postinfos.GONGSHANG_EndTime);

                if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Gongshang) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                {
                    #region 验证补缴月重复
                    var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == EmployeeGoonPayment.EmployeeAddId && (BUJIAO_STATUS.Contains(x.State)) && x.Id != GONGSHANGbujiao_id);
                    if (BUJIAO_YANZHENG.Count() > 0)
                    {
                        foreach (var v in BUJIAO_YANZHENG)
                        {

                            var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Gongshang);
                            var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                            if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                            {
                                string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                                Error.Append("" + yanzheng + "<br />");
                            }
                        }
                    }
                    #endregion
                    #region 不允许补缴
                    if (EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                    {

                        Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                    }
                    else
                    {

                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd), QJ_Gongshang.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)) < 0)
                        {
                            Error.Append("此社保机构工伤只允许补缴 " + EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                        }
                    }
                    if (Business.CHA_Months(QJ_Gongshang, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) > EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("工伤起缴时间超出政策允许补缴数<br />");
                    }
                    if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) == 0)
                    {
                        Error.Append("工伤补缴不能报社保月<br />");
                    }
                    #endregion
                }

            }
            #endregion
            #region 失业不能补缴情况
            if (QJ_Shiye != DateTime.MinValue)
            {
                var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == SHIYEbujiao_id);

                var End_DATE = Convert.ToDateTime(postinfos.SHIYE_EndTime);

                if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Shiye) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                {
                    #region 验证补缴月重复
                    var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == EmployeeGoonPayment.EmployeeAddId && (BUJIAO_STATUS.Contains(x.State)) && x.Id != SHIYEbujiao_id);
                    if (BUJIAO_YANZHENG.Count() > 0)
                    {
                        foreach (var v in BUJIAO_YANZHENG)
                        {

                            var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Shiye);
                            var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                            if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                            {
                                string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                                Error.Append("" + yanzheng + "<br />");
                            }
                        }
                    }
                    #endregion
                    #region 不允许补缴
                    if (EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                    {

                        Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                    }
                    else
                    {

                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd), QJ_Shiye.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)) < 0)
                        {
                            Error.Append("此社保机构失业只允许补缴 " + EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                        }
                    }
                    if (Business.CHA_Months(QJ_Shiye, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) > EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("失业起缴时间超出政策允许补缴数<br />");
                    }
                    if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) == 0)
                    {
                        Error.Append("失业补缴不能报社保月<br />");
                    }
                    #endregion
                }

            }
            #endregion
            #region 公积金不能补缴情况
            if (QJ_Gongjijin != DateTime.MinValue)
            {
                var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == GONGJIJINbujiao_id);

                var End_DATE = Convert.ToDateTime(postinfos.GONGJIJIN_EndTime);

                if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Gongjijin) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                {
                    #region 验证补缴月重复
                    var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == EmployeeGoonPayment.EmployeeAddId && (BUJIAO_STATUS.Contains(x.State)) && x.Id != GONGJIJINbujiao_id);
                    if (BUJIAO_YANZHENG.Count() > 0)
                    {
                        foreach (var v in BUJIAO_YANZHENG)
                        {

                            var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Gongjijin);
                            var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                            if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                            {
                                string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                                Error.Append("" + yanzheng + "<br />");
                            }
                        }
                    }
                    #endregion
                    #region 不允许补缴
                    if (EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                    {

                        Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                    }
                    else
                    {

                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd), QJ_Gongjijin.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)) < 0)
                        {
                            Error.Append("此社保机构公积金只允许补缴 " + EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                        }
                    }
                    if (Business.CHA_Months(QJ_Gongjijin, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) > EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("公积金起缴时间超出政策允许补缴数<br />");
                    }
                    if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) == 0)
                    {
                        Error.Append("公积金补缴不能报社保月<br />");
                    }
                    #endregion
                }

            }
            #endregion
            #region 生育不能补缴情况
            if (QJ_Shengyu != DateTime.MinValue)
            {
                var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == SHENGYUbujiao_id);

                var End_DATE = Convert.ToDateTime(postinfos.SHENGYU_EndTime);

                if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Shengyu) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                {
                    #region 验证补缴月重复
                    var BUJIAO_YANZHENG = SysEntitiesO2O.EmployeeGoonPayment.Where(x => x.EmployeeAddId == EmployeeGoonPayment.EmployeeAddId && (BUJIAO_STATUS.Contains(x.State)) && x.Id != SHENGYUbujiao_id);
                    if (BUJIAO_YANZHENG.Count() > 0)
                    {
                        foreach (var v in BUJIAO_YANZHENG)
                        {

                            var a = Common.Business.bujiao_max((DateTime)v.StartTime, QJ_Shengyu);
                            var b = Common.Business.bujiao_min((DateTime)v.EndTime, End_DATE);
                            if (a < b || SqlMethods.DateDiffMonth(a, b) == 0)
                            {
                                string yanzheng = String.Format("{0}至{1}重复", a.ToString("yyyy-MM-dd"), b.ToString("yyyy-MM-dd"));
                                Error.Append("" + yanzheng + "<br />");
                            }
                        }
                    }
                    #endregion
                    #region 不允许补缴
                    if (EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth == 0)//不允许补缴
                    {

                        Error.Append("此社保机构养老不允许补缴，请修改起缴时间或社保政策后再申报！");

                    }
                    else
                    {

                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd), QJ_Shengyu.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)) < 0)
                        {
                            Error.Append("此社保机构生育只允许补缴 " + EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth + " 个月，请修改起缴时间或社保政策后再申报！");
                        }
                    }
                    if (Business.CHA_Months(QJ_Shengyu, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) > EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("生育起缴时间超出政策允许补缴数<br />");
                    }
                    if (Business.CHA_Months(End_DATE, _NowDate.AddMonths((int)EmployeeGoonPayment.EmployeeAdd.PoliceInsurance.InsuranceAdd)) == 0)
                    {
                        Error.Append("生育补缴不能报社保月<br />");
                    }
                    #endregion
                }

            }
            #endregion
            return Error.ToString();

        }
        #endregion

        #region 责任客服修改员工信息 敬
        public Common.ClientResult.Result POSTEmployeeAddModify([FromBody]EmployeeGoonPaymentModels postinfos, int? YANGLAObujiao_id, int? YILIAObujiao_id, int? GONGSHANGbujiao_id, int? SHIYEbujiao_id, int? GONGJIJINbujiao_id, int? SHENGYUbujiao_id)
        {

            StringBuilder sbError = new StringBuilder();
            string error = "";
            error = verificationBacknumber(postinfos, YANGLAObujiao_id, YILIAObujiao_id, GONGSHANGbujiao_id, SHIYEbujiao_id, GONGJIJINbujiao_id, SHENGYUbujiao_id);

            if (error != "")
            {
                sbError.Append("以下信息错误：<br />");
                sbError.Append(error);
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Fail;
                result.Message = sbError.ToString();
                return result;
            }

            else
            {
                int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                string Enable = Status.启用.ToString();
                string Disable = Status.停用.ToString();
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                        DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
                        DateTime QJ_Yanglao = DateTime.MinValue;
                        DateTime QJ_Yiliao = DateTime.MinValue;
                        DateTime QJ_Gongshang = DateTime.MinValue;
                        DateTime QJ_Shiye = DateTime.MinValue;
                        DateTime QJ_Gongjijin = DateTime.MinValue;
                        DateTime QJ_Shengyu = DateTime.MinValue;
                        DateTime.TryParse(postinfos.YANGLAO_StartTime, out QJ_Yanglao);
                        DateTime.TryParse(postinfos.YILIAO_StartTime, out QJ_Yiliao);
                        DateTime.TryParse(postinfos.GONGSHANG_StartTime, out QJ_Gongshang);
                        DateTime.TryParse(postinfos.SHIYE_StartTime, out QJ_Shiye);
                        DateTime.TryParse(postinfos.GONGJIJIN_StartTime, out QJ_Gongjijin);
                        DateTime.TryParse(postinfos.SHENGYU_StartTime, out QJ_Shengyu);
                        #region 养老修改
                        if (QJ_Yanglao != DateTime.MinValue)
                        {

                            var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == YANGLAObujiao_id);

                            var End_DATE = Convert.ToDateTime(postinfos.YANGLAO_EndTime);

                            if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Yanglao) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                            {
                                int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                                string EmployeeMiddle_PaymentBetween = Convert.ToDateTime(EmployeeGoonPayment.StartTime).ToString("yyyyMM") + "-" + Convert.ToDateTime(EmployeeGoonPayment.EndTime).ToString("yyyyMM");
                                #region 修改补缴表数据
                                EmployeeGoonPayment.StartTime = QJ_Yanglao;
                                EmployeeGoonPayment.EndTime = End_DATE;
                                EmployeeGoonPayment.UpdatePerson = LoginInfo.UserName;
                                EmployeeGoonPayment.UpdateTime = DateTime.Now;
                                #endregion
                                #region 作废中间表数据
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && o.PaymentStyle == PaymentStyle && o.PaymentBetween == EmployeeMiddle_PaymentBetween);
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable;
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.UserName;
                                    }
                                }
                                #endregion
                                #region 补缴中间表
                                decimal GZ_Yanglao = (decimal)EmployeeGoonPayment.EmployeeAdd.Wage;
                                int ZC_Yanglao_ID = (int)EmployeeGoonPayment.EmployeeAdd.PoliceInsuranceId;
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId);
                                Int32 Months = Business.CHA_Months(QJ_Yanglao, End_DATE) + 1;
                                var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                                var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                                var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 1);
                                var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yanglao_ID, GZ_Yanglao, 2);
                                EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                                employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                                employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                employeeMiddle_BJ.PaymentMonth = Months;
                                employeeMiddle_BJ.UseBetween = 0;
                                employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle_BJ.CreateTime = DateTime.Now;
                                employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                employeeMiddle_BJ.PaymentBetween = QJ_Yanglao.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                #endregion
                            }
                        }
                        #endregion

                        #region 医疗修改
                        if (QJ_Yiliao != DateTime.MinValue)
                        {

                            var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == YILIAObujiao_id);

                            var End_DATE = Convert.ToDateTime(postinfos.YILIAO_EndTime);

                            if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Yiliao) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                            {
                                int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                                string EmployeeMiddle_PaymentBetween = Convert.ToDateTime(EmployeeGoonPayment.StartTime).ToString("yyyyMM") + "-" + Convert.ToDateTime(EmployeeGoonPayment.EndTime).ToString("yyyyMM");
                                #region 修改补缴表数据
                                EmployeeGoonPayment.StartTime = QJ_Yiliao;
                                EmployeeGoonPayment.EndTime = End_DATE;
                                EmployeeGoonPayment.UpdatePerson = LoginInfo.UserName;
                                EmployeeGoonPayment.UpdateTime = DateTime.Now;
                                #endregion
                                #region 作废中间表数据
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && o.PaymentStyle == PaymentStyle && o.PaymentBetween == EmployeeMiddle_PaymentBetween);
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable;
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.UserName;
                                    }
                                }
                                #endregion
                                #region 补缴中间表
                                decimal GZ_Yiliao = (decimal)EmployeeGoonPayment.EmployeeAdd.Wage;
                                int ZC_Yiliao_ID = (int)EmployeeGoonPayment.EmployeeAdd.PoliceInsuranceId;
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId);
                                Int32 Months = Business.CHA_Months(QJ_Yiliao, End_DATE) + 1;
                                var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                                var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                                var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 1);
                                var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Yiliao_ID, GZ_Yiliao, 2);
                                EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                                employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                                employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                employeeMiddle_BJ.PaymentMonth = Months;
                                employeeMiddle_BJ.UseBetween = 0;
                                employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle_BJ.CreateTime = DateTime.Now;
                                employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                employeeMiddle_BJ.PaymentBetween = QJ_Yiliao.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                #endregion
                            }
                        }
                        #endregion

                        #region 工伤修改
                        if (QJ_Gongshang != DateTime.MinValue)
                        {

                            var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == GONGSHANGbujiao_id);

                            var End_DATE = Convert.ToDateTime(postinfos.GONGSHANG_EndTime);

                            if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Gongshang) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                            {
                                int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                                string EmployeeMiddle_PaymentBetween = Convert.ToDateTime(EmployeeGoonPayment.StartTime).ToString("yyyyMM") + "-" + Convert.ToDateTime(EmployeeGoonPayment.EndTime).ToString("yyyyMM");
                                #region 修改补缴表数据
                                EmployeeGoonPayment.StartTime = QJ_Gongshang;
                                EmployeeGoonPayment.EndTime = End_DATE;
                                EmployeeGoonPayment.UpdatePerson = LoginInfo.UserName;
                                EmployeeGoonPayment.UpdateTime = DateTime.Now;
                                #endregion
                                #region 作废中间表数据
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && o.PaymentStyle == PaymentStyle && o.PaymentBetween == EmployeeMiddle_PaymentBetween);
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable;
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.UserName;
                                    }
                                }
                                #endregion
                                #region 补缴中间表
                                decimal GZ_Gongshang = (decimal)EmployeeGoonPayment.EmployeeAdd.Wage;
                                int ZC_Gongshang_ID = (int)EmployeeGoonPayment.EmployeeAdd.PoliceInsuranceId;
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId);
                                Int32 Months = Business.CHA_Months(QJ_Gongshang, End_DATE) + 1;
                                var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                                var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                                var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 1);
                                var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongshang_ID, GZ_Gongshang, 2);
                                EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                                employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                                employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                employeeMiddle_BJ.PaymentMonth = Months;
                                employeeMiddle_BJ.UseBetween = 0;
                                employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle_BJ.CreateTime = DateTime.Now;
                                employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                employeeMiddle_BJ.PaymentBetween = QJ_Gongshang.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                #endregion
                            }
                        }
                        #endregion

                        #region 失业修改
                        if (QJ_Shiye != DateTime.MinValue)
                        {

                            var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == SHIYEbujiao_id);

                            var End_DATE = Convert.ToDateTime(postinfos.SHIYE_EndTime);

                            if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Shiye) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                            {
                                int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;
                                string EmployeeMiddle_PaymentBetween = Convert.ToDateTime(EmployeeGoonPayment.StartTime).ToString("yyyyMM") + "-" + Convert.ToDateTime(EmployeeGoonPayment.EndTime).ToString("yyyyMM");
                                #region 修改补缴表数据
                                EmployeeGoonPayment.StartTime = QJ_Shiye;
                                EmployeeGoonPayment.EndTime = End_DATE;
                                EmployeeGoonPayment.UpdatePerson = LoginInfo.UserName;
                                EmployeeGoonPayment.UpdateTime = DateTime.Now;
                                #endregion
                                #region 作废中间表数据
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && o.PaymentStyle == PaymentStyle && o.PaymentBetween == EmployeeMiddle_PaymentBetween);
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable;
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.UserName;
                                    }
                                }
                                #endregion
                                #region 补缴中间表
                                decimal GZ_Shiye = (decimal)EmployeeGoonPayment.EmployeeAdd.Wage;
                                int ZC_Shiye_ID = (int)EmployeeGoonPayment.EmployeeAdd.PoliceInsuranceId;
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId);
                                Int32 Months = Business.CHA_Months(QJ_Shiye, End_DATE) + 1;
                                var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                                var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                                var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 1);
                                var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shiye_ID, GZ_Shiye, 2);
                                EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;
                                employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                                employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                employeeMiddle_BJ.PaymentMonth = Months;
                                employeeMiddle_BJ.UseBetween = 0;
                                employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle_BJ.CreateTime = DateTime.Now;
                                employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                employeeMiddle_BJ.PaymentBetween = QJ_Shiye.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                #endregion
                            }
                        }
                        #endregion

                        #region 公积金修改
                        if (QJ_Gongjijin != DateTime.MinValue)
                        {

                            var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == GONGJIJINbujiao_id);

                            var End_DATE = Convert.ToDateTime(postinfos.GONGJIJIN_EndTime);

                            if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Gongjijin) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                            {
                                int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                                string EmployeeMiddle_PaymentBetween = Convert.ToDateTime(EmployeeGoonPayment.StartTime).ToString("yyyyMM") + "-" + Convert.ToDateTime(EmployeeGoonPayment.EndTime).ToString("yyyyMM");
                                #region 修改补缴表数据
                                EmployeeGoonPayment.StartTime = QJ_Gongjijin;
                                EmployeeGoonPayment.EndTime = End_DATE;
                                EmployeeGoonPayment.UpdatePerson = LoginInfo.UserName;
                                EmployeeGoonPayment.UpdateTime = DateTime.Now;
                                #endregion
                                #region 作废中间表数据
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && o.PaymentStyle == PaymentStyle && o.PaymentBetween == EmployeeMiddle_PaymentBetween);
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable;
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.UserName;
                                    }
                                }
                                #endregion
                                #region 补缴中间表
                                decimal GZ_Gongjijin = (decimal)EmployeeGoonPayment.EmployeeAdd.Wage;
                                int ZC_Gongjijin_ID = (int)EmployeeGoonPayment.EmployeeAdd.PoliceInsuranceId;
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId);
                                Int32 Months = Business.CHA_Months(QJ_Gongjijin, End_DATE) + 1;
                                var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                                var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                                var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 1);
                                var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Gongjijin_ID, GZ_Gongjijin, 2);
                                EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                                employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                                employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                employeeMiddle_BJ.PaymentMonth = Months;
                                employeeMiddle_BJ.UseBetween = 0;
                                employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle_BJ.CreateTime = DateTime.Now;
                                employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                employeeMiddle_BJ.PaymentBetween = QJ_Gongjijin.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                #endregion
                            }
                        }
                        #endregion

                        #region 生育修改
                        if (QJ_Shengyu != DateTime.MinValue)
                        {

                            var EmployeeGoonPayment = SysEntitiesO2O.EmployeeGoonPayment.FirstOrDefault(p => p.Id == SHENGYUbujiao_id);

                            var End_DATE = Convert.ToDateTime(postinfos.SHENGYU_EndTime);

                            if (SqlMethods.DateDiffMonth(EmployeeGoonPayment.StartTime, QJ_Shengyu) != 0 || SqlMethods.DateDiffMonth(EmployeeGoonPayment.EndTime, End_DATE) != 0)
                            {
                                int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                                string EmployeeMiddle_PaymentBetween = Convert.ToDateTime(EmployeeGoonPayment.StartTime).ToString("yyyyMM") + "-" + Convert.ToDateTime(EmployeeGoonPayment.EndTime).ToString("yyyyMM");
                                #region 修改补缴表数据
                                EmployeeGoonPayment.StartTime = QJ_Shengyu;
                                EmployeeGoonPayment.EndTime = End_DATE;
                                EmployeeGoonPayment.UpdatePerson = LoginInfo.UserName;
                                EmployeeGoonPayment.UpdateTime = DateTime.Now;
                                #endregion
                                #region 作废中间表数据
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && o.PaymentStyle == PaymentStyle && o.PaymentBetween == EmployeeMiddle_PaymentBetween);
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable;
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.UserName;
                                    }
                                }
                                #endregion
                                #region 补缴中间表
                                decimal GZ_Shengyu = (decimal)EmployeeGoonPayment.EmployeeAdd.Wage;
                                int ZC_Shengyu_ID = (int)EmployeeGoonPayment.EmployeeAdd.PoliceInsuranceId;
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId);
                                Int32 Months = Business.CHA_Months(QJ_Shengyu, End_DATE) + 1;
                                var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                                var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                                var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 1);
                                var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_Shengyu_ID, GZ_Shengyu, 2);
                                EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                employeeMiddle_BJ.InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                                employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeGoonPayment.EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补中间段;
                                employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                employeeMiddle_BJ.CompanyPayment = Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                employeeMiddle_BJ.EmployeePayment = Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                employeeMiddle_BJ.PaymentMonth = Months;
                                employeeMiddle_BJ.UseBetween = 0;
                                employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle_BJ.CreateTime = DateTime.Now;
                                employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                employeeMiddle_BJ.PaymentBetween = QJ_Shengyu.ToString("yyyyMM") + "-" + (End_DATE).ToString("yyyyMM");
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                #endregion
                            }
                        }
                        #endregion
                        //9.最后保存
                        SysEntitiesO2O.SaveChanges();
                        scope.Complete();
                        Common.ClientResult.Result result = new Common.ClientResult.Result();
                        result.Code = ClientCode.Succeed;
                        result.Message = "修改成功";
                        return result;
                    }
                }
                catch (Exception er)
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Fail;
                    result.Message = er.Message + "修改失败,";
                    return result;
                }
            }

        }
        #endregion

        #region 社保专员提取补缴信息列表

        /// <summary>
        /// 社保专员提取报增信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostExtractionList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;

            if (!string.IsNullOrWhiteSpace(search))
            {
                Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        if (str.Equals(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else
                        {
                            string state = Common.EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
            }
            else
            {
                string state = Common.EmployeeAdd_State.员工客服已确认.ToString();
                search = "State&" + state + "^";
            }

            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 社保专员提取补缴信息列表回退动作
        /// <summary>
        /// 社保专员报增信息
        /// </summary>
        /// <param name="ids">回退人员的id集合</param>
        /// <returns></returns>


        public string EmployeeFallbackAction(string ids, string message)
        {
            try
            {
                message = HttpUtility.HtmlDecode(message);
                var results = 0;//返回的结果
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                using (var ent = new SysEntities())
                {
                    var updateEmployeeGoonPayment = ent.EmployeeGoonPayment.Where(a => intArray.Contains(a.Id));
                    if (updateEmployeeGoonPayment != null && updateEmployeeGoonPayment.Count() >= 1)
                    {
                        foreach (var item in updateEmployeeGoonPayment)
                        {
                            item.State = EmployeeGoonPayment_STATUS.待员工客服确认.ToString();
                            item.Remark = message;
                        }
                        results = ent.SaveChanges();
                    }
                    string result = "操作成功！";
                    if (results == 0)
                    {
                        result = "操作失败！";
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
        #endregion

        #region 社保专员导出报增信息列表

        /// <summary>
        /// 社保专员导出报增信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult SupplierExport([FromBody]GetDataParam getParam)
        {



            FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Images/供应商导出.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook workbook = new HSSFWorkbook(file);
            try
            {
                string search = getParam.search;
                int getid = 0;
                int total = 0;


                Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        if (str.Equals(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else if (str.Equals(Common.CollectState.已提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.社保专员已提取.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
                string excelName = "供应商导出" + DateTime.Now.ToString();
                using (MemoryStream ms = new MemoryStream())
                {

                    #region 生成Excel


                    workbook.SetSheetName(0, "供应商导出");
                    List<EmployeeGoonPaymentView> queryData = m_BLL.GetEmployeeGoonPaymentExcelList(1, int.MaxValue, search, ref total);

                    string ids = string.Empty;


                    //  IWorkbook workbook = new HSSFWorkbook();
                    //员工社保一览
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rowNum = 0;
                    IRow currentRow = sheet.CreateRow(rowNum);

                    int colNum = 0;


                    ICell cell = currentRow.CreateCell(colNum);


                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("企业编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("企业名称");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("员工姓名");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("员工证件号");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工作岗位");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("缴纳地");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）补缴开始时间");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）补缴结束时间");
                    colNum++;


                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（养老）创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（医疗）创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（工伤）创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（失业）创建时间");
                    colNum++;







                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（公积金）创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（生育）创建时间");
                    colNum++;



                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（大病）创建时间");
                    colNum++;



                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）单位比例");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）个人比例");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）工资");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）补缴开始时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）补缴结束时间");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）户口性质");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）报增自然月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("（补充公积金）创建时间");
                    colNum++;



                    for (int i = 0; i < queryData.Count; i++)
                    {


                        rowNum++;
                        int colNum1 = 0;
                        IRow currentRow1 = sheet.CreateRow(rowNum);
                        ICell cell1 = currentRow1.CreateCell(colNum1);

                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyCode);
                        colNum1++;

                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyName);
                        colNum1++;

                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Name);
                        colNum1++;

                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CertificateNumber);
                        colNum1++;

                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Station);
                        colNum1++;

                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].City);
                        colNum1++;




                        //（养老）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_1);
                        colNum1++;
                        //（养老）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_1);
                        colNum1++;
                        //（养老）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_1.ToString());
                        colNum1++;
                        //（养老）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_1.ToString());
                        colNum1++;
                        //（养老）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_1.ToString());
                        colNum1++;
                        //（养老）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_1.ToString());
                        colNum1++;

                        //（养老）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_1.ToString());
                        colNum1++;
                        //（养老）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（养老）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_1.ToString());
                        colNum1++;
                        //（养老）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_1.ToString());
                        colNum1++;
                        //（养老）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_1);
                        colNum1++;
                        //（养老）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_1);
                        colNum1++;
                        //（养老）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_1.ToString());
                        colNum1++;



                        //（医疗）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_2);
                        colNum1++;
                        //（医疗）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_2);
                        colNum1++;
                        //（医疗）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_2.ToString());
                        colNum1++;
                        //（医疗）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_2.ToString());
                        colNum1++;
                        //（医疗）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_2.ToString());
                        colNum1++;
                        //（医疗）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_2.ToString());
                        colNum1++;
                        //（医疗）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_2.ToString());
                        colNum1++;
                        //（医疗）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（医疗）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_2.ToString());
                        colNum1++;
                        //（医疗）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_2.ToString());
                        colNum1++;
                        //（医疗）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_2);
                        colNum1++;
                        //（医疗）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_2);
                        colNum1++;
                        //（医疗）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_2.ToString());
                        colNum1++;



                        //（工伤）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_3);
                        colNum1++;
                        //（工伤）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_3);
                        colNum1++;
                        //（工伤）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_3.ToString());
                        colNum1++;
                        //（工伤）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_3.ToString());
                        colNum1++;
                        //（工伤）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_3.ToString());
                        colNum1++;
                        //（工伤）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_3.ToString());
                        colNum1++;
                        //（工伤）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_3.ToString());
                        colNum1++;
                        //（工伤）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（工伤）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_3.ToString());
                        colNum1++;
                        //（工伤）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_3.ToString());
                        colNum1++;
                        //（工伤）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_3);
                        colNum1++;
                        //（工伤）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_3);
                        colNum1++;
                        //（工伤）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_3.ToString());
                        colNum1++;




                        //（失业）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_4);
                        colNum1++;
                        //（失业）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_4);
                        colNum1++;
                        //（失业）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_4.ToString());
                        colNum1++;
                        //（失业）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_4.ToString());
                        colNum1++;
                        //（失业）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_4.ToString());
                        colNum1++;
                        //（失业）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_4.ToString());
                        colNum1++;
                        //（失业）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_4.ToString());
                        colNum1++;
                        //（失业）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（失业）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_4.ToString());
                        colNum1++;
                        //（失业）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_4.ToString());
                        colNum1++;
                        //（失业）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_4);
                        colNum1++;
                        //（失业）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_4);
                        colNum1++;
                        //（失业）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_4.ToString());
                        colNum1++;




                        //（公积金）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_5);
                        colNum1++;
                        //（公积金）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_5);
                        colNum1++;
                        //（公积金）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_5.ToString());
                        colNum1++;
                        //（公积金）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_5.ToString());
                        colNum1++;
                        //（公积金）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_5.ToString());
                        colNum1++;
                        //（公积金）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_5.ToString());
                        colNum1++;
                        //（公积金）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_5.ToString());
                        colNum1++;
                        //（公积金）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（公积金）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_5.ToString());
                        colNum1++;
                        //（公积金）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_5.ToString());
                        colNum1++;
                        //（公积金）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_5);
                        colNum1++;
                        //（公积金）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_5);
                        colNum1++;
                        //（公积金）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_5.ToString());
                        colNum1++;


                        //（生育）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_6);
                        colNum1++;
                        //（生育）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_6);
                        colNum1++;
                        //（生育）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_6.ToString());
                        colNum1++;
                        //（生育）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_6.ToString());
                        colNum1++;
                        //（生育）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_6.ToString());
                        colNum1++;
                        //（生育）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_6.ToString());
                        colNum1++;
                        //（生育）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_6.ToString());
                        colNum1++;
                        //（生育）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（生育）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_6.ToString());
                        colNum1++;
                        //（生育）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_6.ToString());
                        colNum1++;
                        //（生育）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_6);
                        colNum1++;
                        //（生育）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_6);
                        colNum1++;
                        //（生育）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_6.ToString());
                        colNum1++;

                        //（大病）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_7);
                        colNum1++;
                        //（大病）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_7);
                        colNum1++;
                        //（医大病）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_7.ToString());
                        colNum1++;
                        //（大病）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_7.ToString());
                        colNum1++;
                        //（大病）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_7.ToString());
                        colNum1++;
                        //（大病）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_7.ToString());
                        colNum1++;
                        //（大病）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_7.ToString());
                        colNum1++;
                        //（大病）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（大病）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_7.ToString());
                        colNum1++;
                        //（大病）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_7.ToString());
                        colNum1++;
                        //（大病）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_7);
                        colNum1++;
                        //（大病）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_7);
                        colNum1++;
                        //（大病）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_7.ToString());
                        colNum1++;
                        //（补充公积金）政策手续
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceOperationName_8);
                        colNum1++;
                        //（补充公积金）政策
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_8);
                        colNum1++;
                        //（补充公积金）单位比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CompanyPercent_8.ToString());
                        colNum1++;
                        //（补充公积金）个人比例
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EmployeePercent_8.ToString());
                        colNum1++;
                        //（补充公积金）工资
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].Wage_8.ToString());
                        colNum1++;
                        //（补充公积金）补缴开始时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_8.ToString());
                        colNum1++;
                        //（补充公积金）补缴结束时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].EndTime_8.ToString());
                        colNum1++;
                        //（补充公积金）户口性质
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colNum1++;
                        //（补充公积金）报增自然月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].YearMonth_8.ToString());
                        colNum1++;
                        //（补充公积金）社保月
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceMonth_8.ToString());
                        colNum1++;
                        //（补充公积金）是否单立户
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_8);
                        colNum1++;
                        //（补充公积金）社保编号
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].InsuranceCode_8);
                        colNum1++;
                        //（补充公积金）创建时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].CreateTime_8.ToString());
                        colNum1++;
                        ids += queryData[i].AddIds;
                    #endregion
                    }
                    var results = 0;//返回的结果
                    int[] intArray;
                    string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                    if (queryData.Count == 0)
                    {
                        var data = new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.FindNull,
                            Message = "没有符合条件的数据",
                        };
                        return data;
                    }
                    else
                    {
                        using (var ent = new SysEntities())
                        {
                            string State = EmployeeGoonPayment_STATUS.员工客服已确认.ToString();
                            var updateEmployeeGoonPayment = ent.EmployeeGoonPayment.Where(a => intArray.Contains(a.Id) && a.State == State);
                            if (updateEmployeeGoonPayment != null && updateEmployeeGoonPayment.Count() >= 1)
                            {
                                foreach (var item in updateEmployeeGoonPayment)
                                {
                                    item.State = EmployeeGoonPayment_STATUS.社保专员已提取.ToString();
                                    item.UpdateTime = DateTime.Now;
                                    item.UpdatePerson = LoginInfo.LoginName;

                                }
                                results = ent.SaveChanges();

                            }
                        }
                        if (results == 1)
                        {
                            string fileName = "社保专员导出_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
                            string urlPath = "DataExport\\" + fileName; // 文件下载的URL地址，供给前台下载
                            string filePath = System.Web.HttpContext.Current.Server.MapPath("\\") + urlPath; // 文件路径
                            file = new FileStream(filePath, FileMode.Create);
                            workbook.Write(file);
                            file.Close();

                            string Message = "已成功提取报增信息";
                            return new Common.ClientResult.UrlResult
                            {
                                Code = ClientCode.Succeed,
                                Message = Message,
                                URL = urlPath
                            };
                        }
                        else
                        {
                            var data = new Common.ClientResult.UrlResult
                            {
                                Code = ClientCode.FindNull,
                                Message = "提取失败",

                            };
                            return data;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                file.Close();
                return new Common.ClientResult.UrlResult
                {
                    Code = ClientCode.Fail,
                    Message = e.Message
                };
            }

        }

        #endregion

    }
}


