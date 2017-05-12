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
    /// 社保政策
    /// </summary>
    public class PoliceInsuranceApiController : BaseApiController
    {
        JsonResult jr = new JsonResult();
        SysEntities SysEntitiesO2O = new SysEntities();
        IBLL.IEmployeeAddBLL eadd_BLL = new BLL.EmployeeAddBLL();
        IBLL.IPoliceInsuranceBLL Polic_BLL = new BLL.PoliceInsuranceBLL();
        #region 原始代码
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<PoliceInsurance> queryData = m_BLL.GetByParam(getParam.id, getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id
                    ,
                    Name = s.Name
                    ,
                    InsuranceKindId = s.InsuranceKindIdOld
                    ,
                    StartTime = s.StartTime
                    ,
                    EndTime = s.EndTime
                    ,
                    MaxPayMonth = s.MaxPayMonth
                    ,
                    InsuranceAdd = s.InsuranceAdd
                    ,
                    InsuranceReduce = s.InsuranceReduce
                    ,
                    CompanyPercent = s.CompanyPercent
                    ,
                    CompanyLowestNumber = s.CompanyLowestNumber
                    ,
                    EmployeeLowestNumber = s.EmployeeLowestNumber
                    ,
                    CompanyHighestNumber = s.CompanyHighestNumber
                    ,
                    EmployeeHighestNumber = s.EmployeeHighestNumber
                    ,
                    EmployeePercent = s.EmployeePercent
                    ,
                    IsDefault = s.IsDefault
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
        public PoliceInsurance Get(int id)
        {
            PoliceInsurance item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]PoliceInsurance entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，社保政策的信息的Id为" + entity.Id, "社保政策"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，社保政策的信息，" + returnValue, "社保政策"
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
        public Common.ClientResult.Result Put([FromBody]PoliceInsurance entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，社保政策信息的Id为" + entity.Id, "社保政策"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，社保政策信息的Id为" + entity.Id + "," + returnValue, "社保政策"
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

        IBLL.IPoliceInsuranceBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public PoliceInsuranceApiController()
            : this(new PoliceInsuranceBLL()) { }

        public PoliceInsuranceApiController(PoliceInsuranceBLL bll)
        {
            m_BLL = bll;
        }
        #endregion

        #region 报增缴纳保险政策联动判断 敬
        /// <summary>
        /// 报增缴纳保险政策联动判断
        /// </summary>
        /// <param name="Cityid">缴纳地</param>
        /// <param name="postinfos">初始数据</param>
        /// <param name="yanglao_InsuranceKind1">养老种类id</param>
        /// <param name="yiliao_InsuranceKind1">医疗种类id</param>
        /// <param name="gongshang_InsuranceKind1">工伤种类id</param>
        /// <param name="shiye_InsuranceKind1">失业种类id</param>
        /// <param name="shengyu_InsuranceKind1">生育种类id</param>
        /// <returns></returns>
        public Common.ClientResult.Result POSTPoliceCascadeRelationship(string Cityid)
        {
            string POSTPoliceCascadeRelationshipName = Polic_BLL.POSTPoliceCascadeRelationship(Cityid);
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            result.Code = ClientCode.Succeed;
            result.Message = POSTPoliceCascadeRelationshipName;
            return result;
        }
        #endregion

        #region 社保政策 敬
        /// <summary>
        /// 社保政策
        /// </summary>
        /// <param name="ID">政策手续id</param>
        /// <param name="PoliceAccountNatureid">户口性质id</param>
        /// <returns></returns>
        public ActionResult getPoliceInsuranceList(string City)
        {
            string PoliceInsuranceStatus = Status.启用.ToString();
            var w = SysEntitiesO2O.InsuranceKind.Where(o => o.City == City);
            if (w.Count() > 0)
            {
                List<PoliceInsurance> PoliceInsuranceList = new List<PoliceInsurance>();
                foreach (var q in w)
                {
                    foreach (var e in q.PoliceInsurance)
                    {
                        PoliceInsurance PoliceInsuranceModle = new PoliceInsurance();
                        PoliceInsuranceModle.Id = e.Id;
                        PoliceInsuranceModle.Name = e.Name;
                        PoliceInsuranceModle.StartTime = e.StartTime;
                        PoliceInsuranceModle.EndTime = e.EndTime;
                        PoliceInsuranceModle.MaxPayMonth = e.MaxPayMonth;
                        PoliceInsuranceModle.InsuranceAdd = e.InsuranceAdd;
                        PoliceInsuranceModle.InsuranceReduce = e.InsuranceReduce;
                        PoliceInsuranceModle.CompanyPercent = e.CompanyPercent;
                        PoliceInsuranceModle.CompanyLowestNumber = e.CompanyLowestNumber;
                        PoliceInsuranceModle.EmployeeLowestNumber = e.EmployeeLowestNumber;
                        PoliceInsuranceModle.CompanyHighestNumber = e.CompanyHighestNumber;
                        PoliceInsuranceModle.EmployeeHighestNumber = e.EmployeeHighestNumber;
                        PoliceInsuranceModle.EmployeePercent = e.EmployeePercent;
                        PoliceInsuranceModle.IsDefault = e.IsDefault;
                        PoliceInsuranceModle.State = e.State;


                        PoliceInsuranceModle.CreateTime = e.CreateTime;
                        PoliceInsuranceModle.CreatePerson = e.CreatePerson;
                        PoliceInsuranceModle.UpdateTime = e.UpdateTime;
                        PoliceInsuranceModle.UpdatePerson = e.UpdatePerson;

                        PoliceInsuranceList.Add(PoliceInsuranceModle);
                    }
                }
                var r = from c in PoliceInsuranceList

                        where c.State == PoliceInsuranceStatus
                        select new PoliceInsuranceModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            StartTime = c.StartTime == null ? "" : Convert.ToDateTime(c.StartTime).ToString("yyyy-MM-dd"),
                            EndTime = c.EndTime == null ? "" : Convert.ToDateTime(c.EndTime).ToString("yyyy-MM-dd"),
                            MaxPayMonth = c.MaxPayMonth,
                            InsuranceAdd = c.InsuranceAdd,
                            InsuranceReduce = c.InsuranceReduce,
                            CompanyPercent = c.CompanyPercent * 100,
                            CompanyLowestNumber = c.CompanyLowestNumber,
                            EmployeeLowestNumber = c.EmployeeLowestNumber,
                            CompanyHighestNumber = c.CompanyHighestNumber,
                            EmployeeHighestNumber = c.EmployeeHighestNumber,
                            EmployeePercent = c.EmployeePercent * 100,
                            IsDefault = c.IsDefault,
                            State = c.State,
                            CreateTime = c.CreateTime == null ? "" : Convert.ToDateTime(c.CreateTime).ToString("yyyy-MM-dd"),
                            CreatePerson = c.CreatePerson == null ? "" : c.CreatePerson,

                            UpdateTime = c.UpdateTime == null ? "" : Convert.ToDateTime(c.UpdateTime).ToString("yyyy-MM-dd"),
                            UpdatePerson = c.UpdatePerson == null ? "" : c.UpdatePerson,
                        };
                List<PoliceInsuranceModel> list = r.ToList();
                // join b in SysEntitiesO2O.InsuranceKind on c.InsuranceKindId equals b.P_ID
                jr.Data = new JsonMessageResult<List<PoliceInsuranceModel>>("0000", "成功！", list);
                return jr;
            }
            return null;

        }
        #endregion

        #region 种类显示 敬
        /// <summary>
        /// 种类显示
        /// </summary>
        /// <param name="ID">政策手续id</param>
        /// <param name="PoliceAccountNatureid">户口性质id</param>
        /// <returns></returns>
        public ActionResult getInsuranceKindList(int Id)
        {
            var PoliceInsurancelist = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == Id);
            string InsuranceKindStatus = Status.启用.ToString();

            var r = from c in SysEntitiesO2O.InsuranceKind
                    where c.State == InsuranceKindStatus && c.Id == PoliceInsurancelist.InsuranceKindId
                    select new idname__ { ID = c.Id, Name = c.Name };
            List<idname__> list = r.ToList();
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 四舍五入基数取值显示 敬
        /// <summary>
        /// 四舍五入基数取值显示
        /// </summary>
        /// <param name="Id">政策id</param>
        /// <param name="type">类型（1：企业 2：个人）</param>
        /// <returns></returns>
        public ActionResult getPoliceInsurance_Four_FiveBase(int Id, int type)
        {
            var PoliceInsurance_Four_FiveBaseList = SysEntitiesO2O.PoliceInsurance_Four_Five.FirstOrDefault(p => p.PoliceInsuranceID == Id);
            if (PoliceInsurance_Four_FiveBaseList != null)
            {
                List<idname__> list = new List<idname__>();
                idname__ idname = new idname__();
                if (type == 1)
                {
                    idname.Name = Enum.GetName(typeof(IS_Four_Five), (int)PoliceInsurance_Four_FiveBaseList.C_BaseIS) + ",并保留" + PoliceInsurance_Four_FiveBaseList.C_BaseDigit + "位小数";
                }
                else
                {
                    idname.Name = Enum.GetName(typeof(IS_Four_Five), (int)PoliceInsurance_Four_FiveBaseList.B_BaseIS) + ",并保留" + PoliceInsurance_Four_FiveBaseList.B_BaseDigit + "位小数";
                }
                list.Add(idname);
                jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
                return jr;
            }
            return null;
        }
        #endregion

        #region 四舍五入金额取值显示 敬
        /// <summary>
        /// 四舍五入金额取值显示
        /// </summary>
        /// <param name="Id">政策id</param>
        /// <param name="type">类型（1：企业 2：个人）</param>
        /// <returns></returns>
        public ActionResult getPoliceInsurance_Four_FiveBear(int Id, int type)
        {
            var PoliceInsurance_Four_FiveBearList = SysEntitiesO2O.PoliceInsurance_Four_Five.FirstOrDefault(p => p.PoliceInsuranceID == Id);
            if (PoliceInsurance_Four_FiveBearList != null)
            {
                List<idname__> list = new List<idname__>();
                idname__ idname = new idname__();
                if (type == 1)
                {
                    idname.Name = Enum.GetName(typeof(IS_Four_Five), (int)PoliceInsurance_Four_FiveBearList.C_BearIS) + ",并保留" + PoliceInsurance_Four_FiveBearList.C_BearDigit + "位小数," + Enum.GetName(typeof(IS_BearMonth), (int)PoliceInsurance_Four_FiveBearList.C_BearMonth);
                }
                else
                {
                    idname.Name = Enum.GetName(typeof(IS_Four_Five), (int)PoliceInsurance_Four_FiveBearList.B_BearIS) + ",并保留" + PoliceInsurance_Four_FiveBearList.B_BearDigit + "位小数," + Enum.GetName(typeof(IS_BearMonth), (int)PoliceInsurance_Four_FiveBearList.B_BearMonth);
                }
                list.Add(idname);
                jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
                return jr;
            }
            return null;
        }
        #endregion

        #region 资料显示 敬

        public ActionResult getAttachmentList(int KindId)
        {
            //var Attachmentlist = SysEntitiesO2O.Attachment.Where(p => p.InsuranceKindId == KindId);
            string InsuranceKindStatus = Status.启用.ToString();

            var r = from c in SysEntitiesO2O.Attachment
                    where c.State == InsuranceKindStatus && c.InsuranceKindId == KindId
                    select new idname__ { ID = c.Id, Name = c.Name };
            List<idname__> list = r.ToList();
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 户口性质 敬
        public ActionResult getPoliceAccountNatureList(int Id)
        {
            var PoliceInsurancelist = SysEntitiesO2O.PoliceOperationPoliceInsurancePoliceAccountNature.Where(p => p.PoliceInsuranceId == Id);
            string InsuranceKindStatus = Status.启用.ToString();
            List<PoliceAccountNature> PoliceAccountNatureList = new List<PoliceAccountNature>();
            foreach (var q in PoliceInsurancelist)
            {
                PoliceAccountNatureList.Add(q.PoliceAccountNature);
            }
            var r = (from c in PoliceAccountNatureList
                     where c.State == InsuranceKindStatus
                     select new  { Name = c.Name }).Distinct();
            List<idname__> aa = new List<idname__>();
           
            foreach (var a in r)
            {
                idname__ uu = new idname__();
                uu.Name=a.Name;
                aa.Add(uu);
            }
            List<idname__> list = aa.ToList();
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

    }
}


