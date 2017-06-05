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
using System.Data;
using Langben.DAL.Model;
using System.Transactions;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Web;
using System.Data.OleDb;
using Langben.DAL.SCHR;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Data.Linq.SqlClient;
using Langben.IBLL;
using System.Collections;
namespace Langben.App.Controllers
{
    /// <summary>
    /// 增加员工
    /// </summary>
    public class EmployeeAddApiController : BaseApiController
    {
        JsonResult jr = new JsonResult();
        SysEntities SysEntitiesO2O = new SysEntities();
        IBLL.IEmployeeAddBLL eadd_BLL = new BLL.EmployeeAddBLL();
        IORG_UserBLL userBLL = new ORG_UserBLL();

        #region 增加员工列表 敬
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {

            IBLL.IEmployeeAddBLL e_BLL = new BLL.EmployeeAddBLL();

            int total = 0;
            List<Employee> queryData = e_BLL.GetEmployeeList(LoginInfo.UserID, getParam.page, getParam.rows, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id
                    ,
                    Name = s.Name
                    ,
                    CertificateNumber = s.CertificateNumber
                })
            };
            return data;
        }
        #endregion

        #region 缴纳地初始绑定 敬
        /// <summary>
        /// 缴纳地初始绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult getCitylist()
        {

            //var q = from c in SysEntitiesO2O.City
            //        select new idname__ { Cityid = c.Id, Name = c.Name };
            List<idname__> list = eadd_BLL.getCitylist();
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 企业初始绑定 敬
        /// <summary>
        /// 企业初始绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult getCompanyList()
        {
            List<idname__> list = eadd_BLL.getCompanyList();

            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 缴纳地户口性质 敬
        public ActionResult getPoliceAccountNatureList(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {

                //var q = from c in w.PoliceAccountNature

                //        select new idname__ { ID = c.Id, Name = c.Name };

                List<idname__> list = eadd_BLL.getPoliceAccountNatureid(ID);

                //string sql = "SELECT a.ID,a.Name  FROM [O2OSYS].[dbo].[PoliceAccountNature] AS a LEFT JOIN [PoliceAccountNatureCity] AS b  on A.Id = B.PoliceAccountNatureId WHERE b.CityId=" + ID + "";
                //List<idname__> list = SysEntitiesO2O.Database.SqlQuery<idname__>(sql).ToList();
                jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
                return jr;
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 社保种类 敬
        /// <summary>
        /// 社保种类
        /// </summary>
        /// <param name="ID">缴纳地id</param>
        /// <returns></returns>
        public ActionResult getInsuranceKindList(string ID)
        {
            List<idname__> list = eadd_BLL.getInsuranceKindid(ID);
            //var q = from c in SysEntitiesO2O.InsuranceKind
            //        where c.City == ID
            //        select new idname__ { ID = c.Id, Name = c.Name };
            //List<idname__> list = q.ToList();

            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 政策手续 敬
        /// <summary>
        /// 政策手续
        /// </summary>
        /// <param name="ID">社保种类id</param>
        /// <returns></returns>
        public ActionResult getPoliceOperationList(int ID)
        {
            List<idname__> list = eadd_BLL.getPoliceOperationid(ID);
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;

        }
        #endregion

        #region 社保政策 敬
        /// <summary>
        /// 社保政策
        /// </summary>
        /// <param name="ID">政策手续id</param>
        /// <param name="PoliceAccountNatureid">户口性质id</param>
        /// <returns></returns>
        public ActionResult getPoliceInsuranceList(int ID, int PoliceAccountNatureid, int kindid)
        {
            string PoliceInsuranceStatus = Status.启用.ToString();
            var w = SysEntitiesO2O.PoliceOperationPoliceInsurancePoliceAccountNature.Where(p => p.PoliceOperationId == ID && p.PoliceAccountNatureId == PoliceAccountNatureid);
            if (w.Count() > 0)
            {
                List<PoliceInsurance> PoliceInsuranceList = new List<PoliceInsurance>();
                foreach (var q in w)
                {
                    PoliceInsuranceList.Add(q.PoliceInsurance);
                }
                var r = from c in PoliceInsuranceList

                        where c.State == PoliceInsuranceStatus && c.InsuranceKindId == kindid
                        select new idname__ { ID = c.Id, Name = c.Name };

                List<idname__> list = r.ToList();
                // join b in SysEntitiesO2O.InsuranceKind on c.InsuranceKindId equals b.P_ID
                jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
                return jr;
            }
            return null;

        }
        #endregion

        #region 社保政策新 信
        /// <summary>
        /// 社保政策
        /// </summary>
        /// <param name="ID">政策手续id</param>
        /// <param name="PoliceAccountNatureid">户口性质id</param>
        /// <returns></returns>
        public ActionResult getNewPoliceInsuranceList(int ID, int PoliceAccountNatureid, int kindid, int companyID)
        {
            string PoliceInsuranceStatus = Status.启用.ToString();
            string CompanyPoliceInsuranceState = Status.停用.ToString();
            var w = SysEntitiesO2O.PoliceOperationPoliceInsurancePoliceAccountNature.Where(p => p.PoliceOperationId == ID && p.PoliceAccountNatureId == PoliceAccountNatureid);
            if (w.Count() > 0)
            {
                List<PoliceInsurance> PoliceInsuranceList = new List<PoliceInsurance>();
                List<CRM_Company_PoliceInsurance> CompanyPoliceInsurance = new List<CRM_Company_PoliceInsurance>();
                CompanyPoliceInsurance = SysEntitiesO2O.CRM_Company_PoliceInsurance.Where(p => p.CRM_Company_ID == companyID && p.State != CompanyPoliceInsuranceState).ToList();
                foreach (var q in w)
                {
                    PoliceInsuranceList.Add(q.PoliceInsurance);
                }
                List<InsuranceKind> kind = SysEntitiesO2O.InsuranceKind.Where(p => p.Id == kindid).ToList();
                List<idname__> list = new List<idname__>();
                if (kind[0].InsuranceKindId == (int)Common.EmployeeAdd_InsuranceKindId.公积金 || kind[0].InsuranceKindId == (int)Common.EmployeeAdd_InsuranceKindId.工伤)
                {
                    var r = from a in PoliceInsuranceList
                            join b in CompanyPoliceInsurance on a.Id equals b.PoliceInsurance
                            where a.State == PoliceInsuranceStatus && a.InsuranceKindId == kindid
                            select new idname__ { ID = a.Id, Name = a.Name };
                    list = r.ToList();
                }
                else
                {
                    var r = from c in PoliceInsuranceList

                            where c.State == PoliceInsuranceStatus && c.InsuranceKindId == kindid
                            select new idname__ { ID = c.Id, Name = c.Name };

                    list = r.ToList();
                }


                //List<idname__> list = r.ToList();
                // join b in SysEntitiesO2O.InsuranceKind on c.InsuranceKindId equals b.P_ID
                jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
                return jr;
            }
            return null;

        }
        #endregion

        #region 责任客服修改员工信息列表 敬

        /// <summary>
        /// 待员工客服确认列表查询
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostCustomerModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeAdd_State.待员工客服确认.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeAdd_State.待员工客服确认.ToString() + "^";
            }
            search += "UserID_ZR&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 员工客服修改员工信息列表 敬

        /// <summary>
        /// 待员工客服确认列表查询
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostEmployeeModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeAdd_State.待员工客服确认.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeAdd_State.待员工客服确认.ToString() + "^";
            }
            search += "UserID_YG&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 员工客服审核通过 敬

        // Approved api/<controller>/5
        /// <summary>
        /// 员工客服审核通过
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result EmployeeApproved(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] ApprovedId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (ApprovedId != null && ApprovedId.Length > 0)
            {
                if (m_BLL.EmployeeAddApproved(ref validationErrors, ApprovedId, Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString()))
                {
                    LogClassModels.WriteServiceLog("审核通过成功" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
                        );//审核通过成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "审核成功";
                    return result;
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
                    LogClassModels.WriteServiceLog("审核通过失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                        );//审核通过失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "审核失败" + returnValue;
                    return result;
                }
            }
            return result;
        }
        #endregion

        #region 员工客服终止 敬
        /// <summary>
        /// 员工客服终止
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Common.ClientResult.Result POSTEmployeeStop(string ids, string message, string alltype)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            try
            {
                string enable = Status.启用.ToString();
                alltype = HttpUtility.HtmlDecode(alltype);
                string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // intArrayall = Array.ConvertAll<string, int>(strArrayall, s => int.Parse(s));
                List<int?> InsuranceKindTypes = new List<int?>();
                foreach (var a in strArrayall)
                {
                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                    InsuranceKindTypes.Add(InsuranceKindId);
                }
                string[] all_STATUS = new string[]{
                        Common.EmployeeAdd_State.待责任客服确认.ToString(),   Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                        Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString(),  Common.EmployeeAdd_State.社保专员已提取.ToString(),
                         Common.EmployeeAdd_State.申报成功.ToString(),
                        };
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                using (var ent = new SysEntities())
                {
                    var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));
                    if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                    {
                        foreach (var item in updateEmpAdd)
                        {
                            var EmployeeAddList = SysEntitiesO2O.EmployeeAdd.Where(a => a.CompanyEmployeeRelationId == item.CompanyEmployeeRelationId && all_STATUS.Contains(a.State));
                            if (EmployeeAddList.Count() <= 1)
                            {
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(a => a.Id == item.CompanyEmployeeRelationId && a.State == "在职");
                                if (CompanyEmployeeRelation != null)
                                {
                                    CompanyEmployeeRelation.State = "离职";
                                }
                            }
                            item.State = EmployeeAdd_State.终止.ToString();
                            item.Remark = message;
                            item.UpdatePerson = LoginInfo.UserName;
                            item.UpdateTime = DateTime.Now;

                            var updateEmpAddmiddle = ent.EmployeeMiddle.Where(a => a.InsuranceKindId == item.InsuranceKindId && a.CompanyEmployeeRelationId == item.CompanyEmployeeRelationId && a.State == enable && (a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴));
                            if (updateEmpAddmiddle != null && updateEmpAddmiddle.Count() >= 1)
                            {
                                foreach (EmployeeMiddle itemn in updateEmpAddmiddle)
                                {
                                    itemn.State = Status.停用.ToString();
                                    itemn.UpdateTime = DateTime.Now;
                                    item.UpdatePerson = LoginInfo.UserName;
                                }
                            }
                            ent.SaveChanges();
                        }

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

        #region 报增缴纳保险政策联动判断 王帅
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
        public Common.ClientResult.Result POSTEmployeeAddLinkage(string Cityid, [FromBody]PostInfo postinfos, string yanglao_InsuranceKind1)
        {
            string guan = "";
            var s = (from a in SysEntitiesO2O.InsuranceKind where a.City == Cityid select a).ToList();
            List<string> aa = new List<string>();
            foreach (var a in s)
            {
                var PoliceCascadeRelationshiplist = SysEntitiesO2O.PoliceCascadeRelationship.FirstOrDefault(o => o.InsuranceKindId == a.Id);
                if (PoliceCascadeRelationshiplist != null)
                {
                    aa.Add(PoliceCascadeRelationshiplist.Tag);
                }
            }
            var e = (from a in SysEntitiesO2O.PoliceCascadeRelationship where aa.Contains(a.Tag) select a).ToList();

            List<PoliceCascadeRelationship> distinctPeople = e.GroupBy(p => p.InsuranceKindId).Select(g => g.First()).ToList();
            if (distinctPeople.Count() > 0)
            {
                foreach (var u in distinctPeople)
                {
                    guan = guan + "," + u.InsuranceKindId;
                }
                guan = guan.TrimEnd(',').TrimStart(',');
                string[] guanlian = guan.Split(',');
                List<string> zuihou = new List<string>();
                //当前保险种类
                string strzl = yanglao_InsuranceKind1;
                string tishi = ""; int aaa = 0;
                strzl = strzl.TrimStart(',').TrimEnd(',');
                int.TryParse(strzl, out aaa);
                var e1 = (from a in SysEntitiesO2O.PoliceCascadeRelationship where a.InsuranceKindId == aaa select a).ToList();
                string guan1 = string.Empty;
                foreach (var u in e1)
                {
                    guan1 = guan1 + "," + u.Tag;
                }

                foreach (var sd in guanlian)
                {
                    int sad = Convert.ToInt32(sd);
                    if (!strzl.Contains(sd))
                    {
                        guan1 = guan1.TrimEnd(',').TrimStart(',');
                        string[] guanlian1 = guan1.Split(',');
                        var InsuranceKindnew = SysEntitiesO2O.PoliceCascadeRelationship.FirstOrDefault(o => o.InsuranceKindId == sad && guanlian1.Contains(o.Tag));
                        if (InsuranceKindnew != null)
                        {
                            var InsuranceKindnew1 = SysEntitiesO2O.PoliceCascadeRelationship.Where(o => o.InsuranceKindId == sad);
                            foreach (var u in InsuranceKindnew1)
                            {
                                guan1 = guan1 + "," + u.Tag;
                            }
                        }
                    }
                }
                guan1 = guan1.TrimEnd(',').TrimStart(',');
                string[] guanlian11 = guan1.Split(',');

                var InsuranceKindnew11 = SysEntitiesO2O.PoliceCascadeRelationship.Where(o => o.InsuranceKindId != aaa && guanlian11.Contains(o.Tag)).ToList();
                var CompanyEmployeeRelationlist = (from ce in InsuranceKindnew11
                                                   join es in SysEntitiesO2O.InsuranceKind on ce.InsuranceKindId equals es.Id

                                                   select new
                                                   {
                                                       Name = es.Name
                                                   }).Distinct().ToList();

                if (CompanyEmployeeRelationlist != null)
                {
                    foreach (var at in CompanyEmployeeRelationlist)
                    {
                        tishi = tishi + "," + at.Name;
                    }

                }
                if (tishi != "")
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Fail;
                    result.Message = tishi.TrimEnd(',').TrimStart(',');
                    return result;
                }
                else
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "";
                    return result;
                }
            }
            else
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Succeed;
                result.Message = "";
                return result;
            }




        }
        #endregion

        #region 获取社保种类 王帅
        /// <summary>
        /// 获取社保种类
        /// </summary>
        /// <param name="Cityid">城市id</param>
        /// <returns></returns>
        public IHttpActionResult InsuranceKindCity(string Cityid)
        {

            var s = (from a in SysEntitiesO2O.InsuranceKind where a.City == Cityid select a).ToList();
            var empApp = new List<InsuranceKind>();
            if (s != null && s.Count() >= 1)
            {
                foreach (object item in s)
                {
                    Type t = item.GetType();
                    InsuranceKind temp = new InsuranceKind();
                    temp.Id = (int)t.GetProperty("Id").GetValue(item, null);
                    temp.Name = (string)t.GetProperty("Name").GetValue(item, null);

                    empApp.Add(temp);
                }
            }
            var jsonData = new
            {
                total = empApp.Count,// 总行数
                rows = empApp
            };
            //string result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
            return Json(jsonData);
            //  return Json(s);

        }
        #endregion
        #region 社保专员反馈通过 王帅
        /// <summary>
        /// 获取社保种类
        /// </summary>
        /// <param name="Cityid">城市id</param>
        /// <returns></returns>
        public IHttpActionResult Passall(string Cityid)
        {

            var s = (from a in SysEntitiesO2O.InsuranceKind where a.City == Cityid select a).ToList();
            var empApp = new List<InsuranceKind>();
            if (s != null && s.Count() >= 1)
            {
                foreach (object item in s)
                {
                    Type t = item.GetType();
                    InsuranceKind temp = new InsuranceKind();
                    temp.Id = (int)t.GetProperty("Id").GetValue(item, null);
                    temp.Name = (string)t.GetProperty("Name").GetValue(item, null);

                    empApp.Add(temp);
                }
            }
            var jsonData = new
            {
                total = empApp.Count,// 总行数
                rows = empApp
            };
            //string result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
            return Json(jsonData);
            //  return Json(s);

        }
        #endregion

        #region 生成编码


        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public EmployeeAdd Get(int id)
        {
            EmployeeAdd item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]EmployeeAdd entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，增加员工的信息的Id为" + entity.Id, "增加员工"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，增加员工的信息，" + returnValue, "增加员工"
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
        public Common.ClientResult.Result Put([FromBody]EmployeeAdd entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，增加员工信息的Id为" + entity.Id, "增加员工"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，增加员工信息的Id为" + entity.Id + "," + returnValue, "增加员工"
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

        IBLL.IEmployeeAddBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeAddApiController()
            : this(new EmployeeAddBLL()) { }

        public EmployeeAddApiController(EmployeeAddBLL bll)
        {
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            m_BLL = bll;
        }
        #endregion

        #region 责任客服审核列表查询 赫

        /// <summary>
        /// 责任客服审核列表查询
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostApproveList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeAdd_State.待责任客服确认.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeAdd_State.待责任客服确认.ToString() + "^";
            }
            search += "UserID_ZR&" + LoginInfo.UserID + "^";

            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 责任客服审核通过

        // Approved api/<controller>/5
        /// <summary>
        /// 责任客服审核通过
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Approved(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] ApprovedId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (ApprovedId != null && ApprovedId.Length > 0)
            {
                if (m_BLL.EmployeeAddApproved(ref validationErrors, ApprovedId, Common.EmployeeAdd_State.待责任客服确认.ToString(), Common.EmployeeAdd_State.待员工客服确认.ToString()))
                {
                    LogClassModels.WriteServiceLog("审核通过成功" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
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
                    LogClassModels.WriteServiceLog("审核通过失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                        );//审核通过失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "审核失败" + returnValue;
                }
            }
            return result;
        }
        #endregion

        #region 查询待客服经理分配列表
        /// <summary>
        /// 查询待客服经理分配列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public Common.ClientResult.DataResult PostAllotList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            string state = Common.EmployeeAdd_State.待责任客服确认.ToString();
            state += ("," + Common.EmployeeAdd_State.待员工客服确认.ToString());
            state += ("," + Common.EmployeeAdd_State.员工客服已确认.ToString());
            state += ("," + Common.EmployeeAdd_State.社保专员已提取.ToString());
            state += ("," + Common.EmployeeAdd_State.申报成功.ToString());
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + state + "^";
            }
            else
            {
                search += "State&" + state + "^";
            }
            search += "ServerState&" + Common.EmployeeAdd_State.申报成功.ToString() + "^";
            search += "UserID&" + LoginInfo.UserID + "^";
            List<EmployeeAllot> queryData = m_BLL.GetAllotList(getParam.page, getParam.rows, search, ref total);

            EmployeeAddController addCon = new EmployeeAddController();
            ContentResult result = (ContentResult)addCon.Kfry("石家庄");

            List<User> users = new List<User>();
            users = JSONStringToList<User>(result.Content);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = from a in queryData
                       join b in users on a.UserID_YG equals b.UserID into bb
                       from b2 in bb.DefaultIfEmpty()
                       join c in users on a.UserID_ZR equals c.UserID into cc
                       from c2 in cc.DefaultIfEmpty()
                       select new EmployeeAllot()
                       {
                           AllotState = a.AllotState,
                           City = a.City,
                           CompanyId = a.CompanyId,
                           CompanyName = a.CompanyName,
                           EmployeeAddSum = a.EmployeeAddSum,
                           EmployeeServerSum = a.EmployeeServerSum,
                           UserID_YG = a.UserID_YG,
                           UserID_ZR = a.UserID_ZR,
                           RealName_YG = b2 == null ? null : b2.RealName,
                           RealName_ZR = c2 == null ? null : c2.RealName,
                       }
            };
            return data;
        }

        public List<T> JSONStringToList<T>(string JsonStr)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<T> objs = Serializer.Deserialize<List<T>>(JsonStr);
            return objs;
        }
        #endregion

        #region 社保专员提取报增信息列表

        /// <summary>
        /// 社保专员提取报增信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostSupplierList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;

            if (!string.IsNullOrWhiteSpace(search))
            {
                Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());

                //if (queryDic.ContainsKey("State") && !string.IsNullOrWhiteSpace(queryDic["State"]))
                //{
                //    string str = queryDic["State"];
                //    string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //    empAdd = empAdd.Where(a => states.Contains(a.State));
                //}


                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (states.Contains(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else
                        {
                            string state = Common.EmployeeAdd_State.社保专员已提取.ToString();
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
            search += "UserID_SB&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            //List<EmployeeApprove> queryData = m_BLL.GetCommissionerListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 社保报增查询列表

        /// <summary>
        /// 社保报增查询列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostEmployeeAddViewList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = new List<EmployeeAddView>()
            };
            //当没有险种查询条件时,拒绝进行查询
            if (string.IsNullOrWhiteSpace(search) || !search.Contains("InsuranceKinds"))
            {

                return data;
            }
            List<ORG_User> userList = getSubordinatesData(Common.ORG_Group_Code.SBKF.ToString(), "1016");      //	报增信息查询

            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.YGKF.ToString(), "1016"));
            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.ZRKF.ToString(), "1016"));

            List<EmployeeAddView> queryData = m_BLL.GetEmployeeAddList(getParam.page, getParam.rows, search, userList, ref total);
            data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion



        #region 社保报增查询导出

        /// <summary>
        /// 社保报增查询导出
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult PostEmployeeAddViewListForExcel(string search)
        {
            int total = 0;

            search = HttpUtility.HtmlDecode(search);

            //var data = new Common.ClientResult.DataResult
            //{
            //    total = total,
            //    rows = new List<EmployeeAddView>()
            //};
            //当没有险种查询条件时,拒绝进行查询
            if (string.IsNullOrWhiteSpace(search) || !search.Contains("InsuranceKinds"))
            {

                var data = new Common.ClientResult.UrlResult
                {
                    Code = ClientCode.FindNull,
                    Message = "没有符合条件的数据",

                };
                return data;
            }
            List<ORG_User> userList = getSubordinatesData(Common.ORG_Group_Code.SBKF.ToString(), "1016");      //	报增信息查询
            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.YGKF.ToString(), "1016"));
            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.ZRKF.ToString(), "1016"));
            List<EmployeeAddView> queryData = m_BLL.GetEmployeeAddListForExcel(search, userList, ref total);

            #region 报增信息导出
            FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Images/报增查询导出.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook workbook = new HSSFWorkbook(file);



            try
            {
                string excelName = Guid.NewGuid().ToString() + "报增查询导出";
                using (MemoryStream ms = new MemoryStream())
                {

                    // var CompanyName = SysEntitiesO2O.ORG_Department.FirstOrDefault(o=>o.ID==LoginInfo.BranchID);


                    workbook.SetSheetName(0, "报增查询导出");
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rowNum = 1;
                    //IRow currentRow = sheet.CreateRow(rowNum);
                    // int colNum = 0;

                    for (int i = 0; i < queryData.Count; i++)
                    {
                        int colnum = 0;

                        IRow currentRow = sheet.CreateRow(rowNum);


                        ICell cell = currentRow.CreateCell(colnum);
                        cell = currentRow.CreateCell(colnum);
                        cell.SetCellValue(queryData[i].Operator_CompanyName);
                        colnum++;
                        ICell cell1 = currentRow.CreateCell(colnum);
                        cell1 = currentRow.CreateCell(colnum);
                        cell1.SetCellValue(queryData[i].CompanyName);
                        colnum++;
                        ICell cell2 = currentRow.CreateCell(colnum);
                        cell2 = currentRow.CreateCell(colnum);
                        cell2.SetCellValue(queryData[i].Name);
                        colnum++;
                        ICell cell3 = currentRow.CreateCell(colnum);
                        cell3 = currentRow.CreateCell(colnum);
                        cell3.SetCellValue(queryData[i].CertificateNumber);
                        colnum++;
                        ICell cell4 = currentRow.CreateCell(colnum);
                        cell4 = currentRow.CreateCell(colnum);
                        cell4.SetCellValue(queryData[i].City);
                        colnum++;
                        ICell cell5 = currentRow.CreateCell(colnum);
                        cell5 = currentRow.CreateCell(colnum);
                        cell5.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colnum++;

                        //养老
                        ICell cell_yl = currentRow.CreateCell(colnum);
                        cell_yl = currentRow.CreateCell(colnum);
                        cell_yl.SetCellValue(queryData[i].Wage_1 == null ? "" : queryData[i].Wage_1.ToString());
                        colnum++;
                        ICell cell_yl1 = currentRow.CreateCell(colnum);
                        cell_yl1 = currentRow.CreateCell(colnum);
                        cell_yl1.SetCellValue(queryData[i].PoliceOperationName_1);
                        colnum++;
                        ICell cell_yl2 = currentRow.CreateCell(colnum);
                        cell_yl2 = currentRow.CreateCell(colnum);
                        cell_yl2.SetCellValue(queryData[i].StartTime_1 == null ? "" : queryData[i].StartTime_1.ToString());
                        colnum++;
                        ICell cell_yl3 = currentRow.CreateCell(colnum);
                        cell_yl3 = currentRow.CreateCell(colnum);
                        cell_yl3.SetCellValue(queryData[i].CompanyNumber_1 == null ? "" : queryData[i].CompanyNumber_1.ToString());
                        colnum++;
                        ICell cell_yl4 = currentRow.CreateCell(colnum);
                        cell_yl4 = currentRow.CreateCell(colnum);
                        cell_yl4.SetCellValue(queryData[i].CompanyPercent_1 == null ? "" : queryData[i].CompanyPercent_1.ToString());
                        colnum++;
                        ICell cell_yl5 = currentRow.CreateCell(colnum);
                        cell_yl5 = currentRow.CreateCell(colnum);
                        cell_yl5.SetCellValue(queryData[i].EmployeeNumber_1 == null ? "" : queryData[i].EmployeeNumber_1.ToString());
                        colnum++;
                        ICell cell_yl7 = currentRow.CreateCell(colnum);
                        cell_yl7 = currentRow.CreateCell(colnum);
                        cell_yl7.SetCellValue(queryData[i].EmployeePercent_1 == null ? "" : queryData[i].EmployeePercent_1.ToString());
                        colnum++;
                        ICell cell_yl8 = currentRow.CreateCell(colnum);
                        cell_yl8 = currentRow.CreateCell(colnum);
                        cell_yl8.SetCellValue(queryData[i].State_1);
                        colnum++;
                        ICell cell_yl9 = currentRow.CreateCell(colnum);
                        cell_yl9 = currentRow.CreateCell(colnum);
                        cell_yl9.SetCellValue(queryData[i].PoliceInsuranceName_1);
                        colnum++;
                        ICell cell_yl10 = currentRow.CreateCell(colnum);
                        cell_yl10 = currentRow.CreateCell(colnum);
                        cell_yl10.SetCellValue(queryData[i].YearMonth_1 == null ? "" : queryData[i].YearMonth_1.ToString());
                        colnum++;

                        //医疗
                        ICell cell_yiliao = currentRow.CreateCell(colnum);
                        cell_yiliao = currentRow.CreateCell(colnum);
                        cell_yiliao.SetCellValue(queryData[i].Wage_2 == null ? "" : queryData[i].Wage_2.ToString());
                        colnum++;
                        ICell cell_yiliao1 = currentRow.CreateCell(colnum);
                        cell_yiliao1 = currentRow.CreateCell(colnum);
                        cell_yiliao1.SetCellValue(queryData[i].PoliceOperationName_2);
                        colnum++;
                        ICell cell_yiliao2 = currentRow.CreateCell(colnum);
                        cell_yiliao2 = currentRow.CreateCell(colnum);
                        cell_yiliao2.SetCellValue(queryData[i].StartTime_2 == null ? "" : queryData[i].StartTime_2.ToString());
                        colnum++;
                        ICell cell_yiliao3 = currentRow.CreateCell(colnum);
                        cell_yiliao3 = currentRow.CreateCell(colnum);
                        cell_yiliao3.SetCellValue(queryData[i].CompanyNumber_2 == null ? "" : queryData[i].CompanyNumber_2.ToString());
                        colnum++;
                        ICell cell_yiliao4 = currentRow.CreateCell(colnum);
                        cell_yiliao4 = currentRow.CreateCell(colnum);
                        cell_yiliao4.SetCellValue(queryData[i].CompanyPercent_2 == null ? "" : queryData[i].CompanyPercent_2.ToString());
                        colnum++;
                        ICell cell_yiliao5 = currentRow.CreateCell(colnum);
                        cell_yiliao5 = currentRow.CreateCell(colnum);
                        cell_yiliao5.SetCellValue(queryData[i].EmployeeNumber_2 == null ? "" : queryData[i].EmployeeNumber_2.ToString());
                        colnum++;
                        ICell cell_yiliao7 = currentRow.CreateCell(colnum);
                        cell_yiliao7 = currentRow.CreateCell(colnum);
                        cell_yiliao7.SetCellValue(queryData[i].EmployeePercent_2 == null ? "" : queryData[i].EmployeePercent_2.ToString());
                        colnum++;
                        ICell cell_yiliao8 = currentRow.CreateCell(colnum);
                        cell_yiliao8 = currentRow.CreateCell(colnum);
                        cell_yiliao8.SetCellValue(queryData[i].State_2);
                        colnum++;
                        ICell cell_yiliao9 = currentRow.CreateCell(colnum);
                        cell_yiliao9 = currentRow.CreateCell(colnum);
                        cell_yiliao9.SetCellValue(queryData[i].PoliceInsuranceName_2);
                        colnum++;
                        ICell cell_yiliao10 = currentRow.CreateCell(colnum);
                        cell_yiliao10 = currentRow.CreateCell(colnum);
                        cell_yiliao10.SetCellValue(queryData[i].YearMonth_2 == null ? "" : queryData[i].YearMonth_2.ToString());
                        colnum++;

                        //工伤
                        ICell cell_gongshang = currentRow.CreateCell(colnum);
                        cell_gongshang = currentRow.CreateCell(colnum);
                        cell_gongshang.SetCellValue(queryData[i].Wage_3 == null ? "" : queryData[i].Wage_3.ToString());
                        colnum++;
                        ICell cell_gongshang1 = currentRow.CreateCell(colnum);
                        cell_gongshang1 = currentRow.CreateCell(colnum);
                        cell_gongshang1.SetCellValue(queryData[i].PoliceOperationName_3);
                        colnum++;
                        ICell cell_gongshang2 = currentRow.CreateCell(colnum);
                        cell_gongshang2 = currentRow.CreateCell(colnum);
                        cell_gongshang2.SetCellValue(queryData[i].StartTime_3 == null ? "" : queryData[i].StartTime_3.ToString());
                        colnum++;
                        ICell cell_gongshang3 = currentRow.CreateCell(colnum);
                        cell_gongshang3 = currentRow.CreateCell(colnum);
                        cell_gongshang3.SetCellValue(queryData[i].CompanyNumber_3 == null ? "" : queryData[i].CompanyNumber_3.ToString());
                        colnum++;
                        ICell cell_gongshang4 = currentRow.CreateCell(colnum);
                        cell_gongshang4 = currentRow.CreateCell(colnum);
                        cell_gongshang4.SetCellValue(queryData[i].CompanyPercent_3 == null ? "" : queryData[i].CompanyPercent_3.ToString());
                        colnum++;
                        ICell cell_gongshang5 = currentRow.CreateCell(colnum);
                        cell_gongshang5 = currentRow.CreateCell(colnum);
                        cell_gongshang5.SetCellValue(queryData[i].EmployeeNumber_3 == null ? "" : queryData[i].EmployeeNumber_3.ToString());
                        colnum++;
                        ICell cell_gongshang7 = currentRow.CreateCell(colnum);
                        cell_gongshang7 = currentRow.CreateCell(colnum);
                        cell_gongshang7.SetCellValue(queryData[i].EmployeePercent_3 == null ? "" : queryData[i].EmployeePercent_3.ToString());
                        colnum++;
                        ICell cell_gongshang8 = currentRow.CreateCell(colnum);
                        cell_gongshang8 = currentRow.CreateCell(colnum);
                        cell_gongshang8.SetCellValue(queryData[i].State_3);
                        colnum++;
                        ICell cell_gongshang9 = currentRow.CreateCell(colnum);
                        cell_gongshang9 = currentRow.CreateCell(colnum);
                        cell_gongshang9.SetCellValue(queryData[i].PoliceInsuranceName_3);
                        colnum++;
                        ICell cell_gongshang10 = currentRow.CreateCell(colnum);
                        cell_gongshang10 = currentRow.CreateCell(colnum);
                        cell_gongshang10.SetCellValue(queryData[i].YearMonth_3 == null ? "" : queryData[i].YearMonth_3.ToString());
                        colnum++;

                        //失业
                        ICell cell_shiye = currentRow.CreateCell(colnum);
                        cell_shiye = currentRow.CreateCell(colnum);
                        cell_shiye.SetCellValue(queryData[i].Wage_4 == null ? "" : queryData[i].Wage_4.ToString());
                        colnum++;
                        ICell cell_shiye1 = currentRow.CreateCell(colnum);
                        cell_shiye1 = currentRow.CreateCell(colnum);
                        cell_shiye1.SetCellValue(queryData[i].PoliceOperationName_4);
                        colnum++;
                        ICell cell_shiye2 = currentRow.CreateCell(colnum);
                        cell_shiye2 = currentRow.CreateCell(colnum);
                        cell_shiye2.SetCellValue(queryData[i].StartTime_4 == null ? "" : queryData[i].StartTime_4.ToString());
                        colnum++;
                        ICell cell_shiye3 = currentRow.CreateCell(colnum);
                        cell_shiye3 = currentRow.CreateCell(colnum);
                        cell_shiye3.SetCellValue(queryData[i].CompanyNumber_4 == null ? "" : queryData[i].CompanyNumber_4.ToString());
                        colnum++;
                        ICell cell_shiye4 = currentRow.CreateCell(colnum);
                        cell_shiye4 = currentRow.CreateCell(colnum);
                        cell_shiye4.SetCellValue(queryData[i].CompanyPercent_4 == null ? "" : queryData[i].CompanyPercent_4.ToString());
                        colnum++;
                        ICell cell_shiye5 = currentRow.CreateCell(colnum);
                        cell_shiye5 = currentRow.CreateCell(colnum);
                        cell_shiye5.SetCellValue(queryData[i].EmployeeNumber_4 == null ? "" : queryData[i].EmployeeNumber_4.ToString());
                        colnum++;
                        ICell cell_shiye7 = currentRow.CreateCell(colnum);
                        cell_shiye7 = currentRow.CreateCell(colnum);
                        cell_shiye7.SetCellValue(queryData[i].EmployeePercent_4 == null ? "" : queryData[i].EmployeePercent_4.ToString());
                        colnum++;
                        ICell cell_shiye8 = currentRow.CreateCell(colnum);
                        cell_shiye8 = currentRow.CreateCell(colnum);
                        cell_shiye8.SetCellValue(queryData[i].State_4);
                        colnum++;
                        ICell cell_shiye9 = currentRow.CreateCell(colnum);
                        cell_shiye9 = currentRow.CreateCell(colnum);
                        cell_shiye9.SetCellValue(queryData[i].PoliceInsuranceName_4);
                        colnum++;
                        ICell cell_shiye10 = currentRow.CreateCell(colnum);
                        cell_shiye10 = currentRow.CreateCell(colnum);
                        cell_shiye10.SetCellValue(queryData[i].YearMonth_4 == null ? "" : queryData[i].YearMonth_4.ToString());
                        colnum++;

                        //公积金
                        ICell cell_gongjijin = currentRow.CreateCell(colnum);
                        cell_gongjijin = currentRow.CreateCell(colnum);
                        cell_gongjijin.SetCellValue(queryData[i].Wage_5 == null ? "" : queryData[i].Wage_5.ToString());
                        colnum++;
                        ICell cell_gongjijin1 = currentRow.CreateCell(colnum);
                        cell_gongjijin1 = currentRow.CreateCell(colnum);
                        cell_gongjijin1.SetCellValue(queryData[i].PoliceOperationName_5);
                        colnum++;
                        ICell cell_gongjijin2 = currentRow.CreateCell(colnum);
                        cell_gongjijin2 = currentRow.CreateCell(colnum);
                        cell_gongjijin2.SetCellValue(queryData[i].StartTime_5 == null ? "" : queryData[i].StartTime_5.ToString());
                        colnum++;
                        ICell cell_gongjijin3 = currentRow.CreateCell(colnum);
                        cell_gongjijin3 = currentRow.CreateCell(colnum);
                        cell_gongjijin3.SetCellValue(queryData[i].CompanyNumber_5 == null ? "" : queryData[i].CompanyNumber_5.ToString());
                        colnum++;
                        ICell cell_gongjijin4 = currentRow.CreateCell(colnum);
                        cell_gongjijin4 = currentRow.CreateCell(colnum);
                        cell_gongjijin4.SetCellValue(queryData[i].CompanyPercent_5 == null ? "" : queryData[i].CompanyPercent_5.ToString());
                        colnum++;
                        ICell cell_gongjijin5 = currentRow.CreateCell(colnum);
                        cell_gongjijin5 = currentRow.CreateCell(colnum);
                        cell_gongjijin5.SetCellValue(queryData[i].EmployeeNumber_5 == null ? "" : queryData[i].EmployeeNumber_5.ToString());
                        colnum++;
                        ICell cell_gongjijin7 = currentRow.CreateCell(colnum);
                        cell_gongjijin7 = currentRow.CreateCell(colnum);
                        cell_gongjijin7.SetCellValue(queryData[i].EmployeePercent_5 == null ? "" : queryData[i].EmployeePercent_5.ToString());
                        colnum++;
                        ICell cell_gongjijin8 = currentRow.CreateCell(colnum);
                        cell_gongjijin8 = currentRow.CreateCell(colnum);
                        cell_gongjijin8.SetCellValue(queryData[i].State_5);
                        colnum++;
                        ICell cell_gongjijin9 = currentRow.CreateCell(colnum);
                        cell_gongjijin9 = currentRow.CreateCell(colnum);
                        cell_gongjijin9.SetCellValue(queryData[i].PoliceInsuranceName_5);
                        colnum++;
                        ICell cell_gongjijin10 = currentRow.CreateCell(colnum);
                        cell_gongjijin10 = currentRow.CreateCell(colnum);
                        cell_gongjijin10.SetCellValue(queryData[i].YearMonth_5 == null ? "" : queryData[i].YearMonth_5.ToString());
                        colnum++;

                        //生育
                        ICell cell_shengyu = currentRow.CreateCell(colnum);
                        cell_shengyu = currentRow.CreateCell(colnum);
                        cell_shengyu.SetCellValue(queryData[i].Wage_6 == null ? "" : queryData[i].Wage_6.ToString());
                        colnum++;
                        ICell cell_shengyu1 = currentRow.CreateCell(colnum);
                        cell_shengyu1 = currentRow.CreateCell(colnum);
                        cell_shengyu1.SetCellValue(queryData[i].PoliceOperationName_6);
                        colnum++;
                        ICell cell_shengyu2 = currentRow.CreateCell(colnum);
                        cell_shengyu2 = currentRow.CreateCell(colnum);
                        cell_shengyu2.SetCellValue(queryData[i].StartTime_6 == null ? "" : queryData[i].StartTime_6.ToString());
                        colnum++;
                        ICell cell_shengyu3 = currentRow.CreateCell(colnum);
                        cell_shengyu3 = currentRow.CreateCell(colnum);
                        cell_shengyu3.SetCellValue(queryData[i].CompanyNumber_6 == null ? "" : queryData[i].CompanyNumber_6.ToString());
                        colnum++;
                        ICell cell_shengyu4 = currentRow.CreateCell(colnum);
                        cell_shengyu4 = currentRow.CreateCell(colnum);
                        cell_shengyu4.SetCellValue(queryData[i].CompanyPercent_6 == null ? "" : queryData[i].CompanyPercent_6.ToString());
                        colnum++;
                        ICell cell_shengyu5 = currentRow.CreateCell(colnum);
                        cell_shengyu5 = currentRow.CreateCell(colnum);
                        cell_shengyu5.SetCellValue(queryData[i].EmployeeNumber_6 == null ? "" : queryData[i].EmployeeNumber_6.ToString());
                        colnum++;
                        ICell cell_shengyu7 = currentRow.CreateCell(colnum);
                        cell_shengyu7 = currentRow.CreateCell(colnum);
                        cell_shengyu7.SetCellValue(queryData[i].EmployeePercent_6 == null ? "" : queryData[i].EmployeePercent_6.ToString());
                        colnum++;
                        ICell cell_shengyu8 = currentRow.CreateCell(colnum);
                        cell_shengyu8 = currentRow.CreateCell(colnum);
                        cell_shengyu8.SetCellValue(queryData[i].State_6);
                        colnum++;
                        ICell cell_shengyu9 = currentRow.CreateCell(colnum);
                        cell_shengyu9 = currentRow.CreateCell(colnum);
                        cell_shengyu9.SetCellValue(queryData[i].PoliceInsuranceName_6);
                        colnum++;
                        ICell cell_shengyu10 = currentRow.CreateCell(colnum);
                        cell_shengyu10 = currentRow.CreateCell(colnum);
                        cell_shengyu10.SetCellValue(queryData[i].YearMonth_6 == null ? "" : queryData[i].YearMonth_6.ToString());
                        colnum++;

                        //补充公积金工资
                        ICell cell_bcgongjijin = currentRow.CreateCell(colnum);
                        cell_bcgongjijin = currentRow.CreateCell(colnum);
                        cell_bcgongjijin.SetCellValue(queryData[i].Wage_7 == null ? "" : queryData[i].Wage_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin1 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin1 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin1.SetCellValue(queryData[i].PoliceOperationName_7);
                        colnum++;
                        ICell cell_bcgongjijin2 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin2 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin2.SetCellValue(queryData[i].StartTime_7 == null ? "" : queryData[i].StartTime_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin3 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin3 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin3.SetCellValue(queryData[i].CompanyNumber_7 == null ? "" : queryData[i].CompanyNumber_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin4 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin4 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin4.SetCellValue(queryData[i].CompanyPercent_7 == null ? "" : queryData[i].CompanyPercent_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin5 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin5 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin5.SetCellValue(queryData[i].EmployeeNumber_7 == null ? "" : queryData[i].EmployeeNumber_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin7 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin7 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin7.SetCellValue(queryData[i].EmployeePercent_7 == null ? "" : queryData[i].EmployeePercent_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin8 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin8 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin8.SetCellValue(queryData[i].State_7);
                        colnum++;
                        ICell cell_bcgongjijin9 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin9 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin9.SetCellValue(queryData[i].PoliceInsuranceName_7);
                        colnum++;
                        ICell cell_bcgongjijin10 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin10 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin10.SetCellValue(queryData[i].YearMonth_7 == null ? "" : queryData[i].YearMonth_7.ToString());
                        colnum++;

                        //大病工资
                        ICell cell_dbgongzi = currentRow.CreateCell(colnum);
                        cell_dbgongzi = currentRow.CreateCell(colnum);
                        cell_dbgongzi.SetCellValue(queryData[i].Wage_8 == null ? "" : queryData[i].Wage_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi1 = currentRow.CreateCell(colnum);
                        cell_dbgongzi1 = currentRow.CreateCell(colnum);
                        cell_dbgongzi1.SetCellValue(queryData[i].PoliceOperationName_8);
                        colnum++;
                        ICell cell_dbgongzi2 = currentRow.CreateCell(colnum);
                        cell_dbgongzi2 = currentRow.CreateCell(colnum);
                        cell_dbgongzi2.SetCellValue(queryData[i].StartTime_8 == null ? "" : queryData[i].StartTime_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi3 = currentRow.CreateCell(colnum);
                        cell_dbgongzi3 = currentRow.CreateCell(colnum);
                        cell_dbgongzi3.SetCellValue(queryData[i].CompanyNumber_8 == null ? "" : queryData[i].CompanyNumber_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi4 = currentRow.CreateCell(colnum);
                        cell_dbgongzi4 = currentRow.CreateCell(colnum);
                        cell_dbgongzi4.SetCellValue(queryData[i].CompanyPercent_8 == null ? "" : queryData[i].CompanyPercent_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi5 = currentRow.CreateCell(colnum);
                        cell_dbgongzi5 = currentRow.CreateCell(colnum);
                        cell_dbgongzi5.SetCellValue(queryData[i].EmployeeNumber_8 == null ? "" : queryData[i].EmployeeNumber_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi7 = currentRow.CreateCell(colnum);
                        cell_dbgongzi7 = currentRow.CreateCell(colnum);
                        cell_dbgongzi7.SetCellValue(queryData[i].EmployeePercent_8 == null ? "" : queryData[i].EmployeePercent_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi8 = currentRow.CreateCell(colnum);
                        cell_dbgongzi8 = currentRow.CreateCell(colnum);
                        cell_dbgongzi8.SetCellValue(queryData[i].State_8);
                        colnum++;
                        ICell cell_dbgongzi9 = currentRow.CreateCell(colnum);
                        cell_dbgongzi9 = currentRow.CreateCell(colnum);
                        cell_dbgongzi9.SetCellValue(queryData[i].PoliceInsuranceName_8);
                        colnum++;
                        ICell cell_dbgongzi10 = currentRow.CreateCell(colnum);
                        cell_dbgongzi10 = currentRow.CreateCell(colnum);
                        cell_dbgongzi10.SetCellValue(queryData[i].YearMonth_8 == null ? "" : queryData[i].YearMonth_8.ToString());
                        colnum++;

                        //大病
                        ICell cell_dabing = currentRow.CreateCell(colnum);
                        cell_dabing = currentRow.CreateCell(colnum);
                        cell_dabing.SetCellValue(queryData[i].Wage_9 == null ? "" : queryData[i].Wage_9.ToString());
                        colnum++;
                        ICell cell_dabing1 = currentRow.CreateCell(colnum);
                        cell_dabing1 = currentRow.CreateCell(colnum);
                        cell_dabing1.SetCellValue(queryData[i].PoliceOperationName_9);
                        colnum++;
                        ICell cell_dabing2 = currentRow.CreateCell(colnum);
                        cell_dabing2 = currentRow.CreateCell(colnum);
                        cell_dabing2.SetCellValue(queryData[i].StartTime_9 == null ? "" : queryData[i].StartTime_9.ToString());
                        colnum++;
                        ICell cell_dabing3 = currentRow.CreateCell(colnum);
                        cell_dabing3 = currentRow.CreateCell(colnum);
                        cell_dabing3.SetCellValue(queryData[i].CompanyNumber_9 == null ? "" : queryData[i].CompanyNumber_9.ToString());
                        colnum++;
                        ICell cell_dabing4 = currentRow.CreateCell(colnum);
                        cell_dabing4 = currentRow.CreateCell(colnum);
                        cell_dabing4.SetCellValue(queryData[i].CompanyPercent_9 == null ? "" : queryData[i].CompanyPercent_9.ToString());
                        colnum++;
                        ICell cell_dabing5 = currentRow.CreateCell(colnum);
                        cell_dabing5 = currentRow.CreateCell(colnum);
                        cell_dabing5.SetCellValue(queryData[i].EmployeeNumber_9 == null ? "" : queryData[i].EmployeeNumber_9.ToString());
                        colnum++;
                        ICell cell_dabing7 = currentRow.CreateCell(colnum);
                        cell_dabing7 = currentRow.CreateCell(colnum);
                        cell_dabing7.SetCellValue(queryData[i].EmployeePercent_9 == null ? "" : queryData[i].EmployeePercent_9.ToString());
                        colnum++;
                        ICell cell_dabing8 = currentRow.CreateCell(colnum);
                        cell_dabing8 = currentRow.CreateCell(colnum);
                        cell_dabing8.SetCellValue(queryData[i].State_9);
                        colnum++;
                        ICell cell_dabing9 = currentRow.CreateCell(colnum);
                        cell_dabing9 = currentRow.CreateCell(colnum);
                        cell_dabing9.SetCellValue(queryData[i].PoliceInsuranceName_9);
                        colnum++;
                        ICell cell_dabing10 = currentRow.CreateCell(colnum);
                        cell_dabing10 = currentRow.CreateCell(colnum);
                        cell_dabing10.SetCellValue(queryData[i].YearMonth_9 == null ? "" : queryData[i].YearMonth_9.ToString());
                        colnum++;

                        rowNum++;
                    }

                    sheet.ForceFormulaRecalculation = true;
                    string fileName = excelName + ".xls";
                    string urlPath = "DataExport/" + fileName; // 文件下载的URL地址，供给前台下载
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("\\" + urlPath); // 文件路径

                    file = new FileStream(filePath, FileMode.Create);
                    workbook.Write(file);
                    file.Close();

                    if (queryData.Count == 0)
                    {
                        var data = new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.FindNull,
                            Message = "没有符合条件的数据",
                            URL = urlPath
                        };
                        return data;
                    }
                    string Message = "已成功提取报增信息";

                    return new Common.ClientResult.UrlResult
                    {
                        Code = ClientCode.Succeed,
                        Message = Message,
                        URL = urlPath
                    };
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



            #endregion









        }
        #endregion

        #region 待社保专员报增反馈

        /// <summary>
        /// 待社保专员报增反馈
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult FeedbackModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeAdd_State.社保专员已提取.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeAdd_State.社保专员已提取.ToString() + "^";
            }
            search += "UserID_SB&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 详细信息（保险）
        /// <summary>
        /// 责任客服审核
        /// </summary>
        /// <param name="id">企业员工关系id</param>
        /// <param name="YearMonth">当前月</param>
        /// <returns></returns>
        public IHttpActionResult Zrkfsh_Detail(string id, string YearMonth)
        {


            int companempID = int.Parse(id);
            using (var ent = new SysEntities())
            {

                int yeamonth = string.IsNullOrEmpty(YearMonth) == true ? -1 : int.Parse(YearMonth);
                var empadd = ent.EmployeeAdd.Where(a => true);
                var Comp = ent.CompanyEmployeeRelation.Where(a => true);
                var emp = ent.Employee.Where(a => true);
                var crm_com = ent.CRM_Company.Where(a => true);
                var insuran = ent.InsuranceKind.Where(a => true);
                var police = ent.PoliceOperation.Where(a => true);
                var policerance = ent.PoliceInsurance.Where(a => true);
                var policenature = ent.PoliceAccountNature.Where(a => true);
                var supp = ent.Supplier.Where(a => true);
                empadd = empadd.Where(a => a.CompanyEmployeeRelationId == companempID);//获取企业员工id
                empadd = empadd.Where(a => a.YearMonth == yeamonth);//比对当前月
                var leftjoin = from a in empadd
                               join b in Comp on a.CompanyEmployeeRelationId equals b.Id
                               join c in emp on b.EmployeeId equals c.Id
                               join d in crm_com on b.CompanyId equals d.ID
                               join e in insuran on a.InsuranceKindId equals e.Id
                               join f in police on a.PoliceOperationId equals f.Id
                               join g in policerance on a.PoliceInsuranceId equals g.Id
                               join h in policenature on a.PoliceAccountNatureId equals h.Id

                               select new EmployeeAppdetail
                               {
                                   bxzl = e.Name,
                                   zclx = g.Name,
                                   sbjs = (decimal)(a.Wage > g.CompanyHighestNumber ? g.CompanyHighestNumber : (a.Wage < g.CompanyLowestNumber ? g.CompanyLowestNumber : a.Wage)),
                                   qjsj = (DateTime)a.StartTime,
                                   bzlx = f.Name,
                                   YearMonth = (int)a.YearMonth
                               };
                var list = leftjoin.ToList<EmployeeAppdetail>();
                var empApp = new List<EmployeeAppdetail>();
                if (list != null && list.Count() >= 1)
                {
                    foreach (object item in list)
                    {
                        Type t = item.GetType();
                        EmployeeAppdetail temp = new EmployeeAppdetail();
                        temp.bxzl = (string)t.GetProperty("bxzl").GetValue(item, null);
                        temp.zclx = (string)t.GetProperty("zclx").GetValue(item, null);
                        temp.sbjs = (decimal)t.GetProperty("sbjs").GetValue(item, null);
                        temp.qjsj = (DateTime)t.GetProperty("qjsj").GetValue(item, null);
                        temp.bzlx = (string)t.GetProperty("bzlx").GetValue(item, null);
                        temp.YearMonth = (int)t.GetProperty("YearMonth").GetValue(item, null);
                        empApp.Add(temp);
                    }
                }
                var jsonData = new
                {
                    total = empApp.Count,// 总行数
                    rows = empApp
                };
                //string result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
                return Json(jsonData);
            }


        }
        #endregion

        #region 回退动作
        /// <summary>
        /// 责任客服审核人员回退
        /// </summary>
        /// <param name="ids">回退人员的id集合</param>
        /// <returns></returns>




        public IHttpActionResult Zrkfsh_Detail11(string id, string YearMonth)
        {


            int companempID = int.Parse(id);
            using (var ent = new SysEntities())
            {

                int yeamonth = string.IsNullOrEmpty(YearMonth) == true ? -1 : int.Parse(YearMonth);
                var empadd = ent.EmployeeAdd.Where(a => true);
                var Comp = ent.CompanyEmployeeRelation.Where(a => true);
                var emp = ent.Employee.Where(a => true);
                var crm_com = ent.CRM_Company.Where(a => true);
                var insuran = ent.InsuranceKind.Where(a => true);
                var police = ent.PoliceOperation.Where(a => true);
                var policerance = ent.PoliceInsurance.Where(a => true);
                var policenature = ent.PoliceAccountNature.Where(a => true);
                var supp = ent.Supplier.Where(a => true);
                empadd = empadd.Where(a => a.CompanyEmployeeRelationId == companempID);//获取企业员工id
                empadd = empadd.Where(a => a.YearMonth == yeamonth);//比对当前月
                var leftjoin = from a in empadd
                               join b in Comp on a.CompanyEmployeeRelationId equals b.Id
                               join c in emp on b.EmployeeId equals c.Id
                               join d in crm_com on b.CompanyId equals d.ID
                               join e in insuran on a.InsuranceKindId equals e.Id
                               join f in police on a.PoliceOperationId equals f.Id
                               join g in policerance on a.PoliceInsuranceId equals g.Id
                               join h in policenature on a.PoliceAccountNatureId equals h.Id

                               select new EmployeeAppdetail
                               {
                                   bxzl = e.Name,
                                   zclx = g.Name,
                                   sbjs = (decimal)(a.Wage > g.CompanyHighestNumber ? g.CompanyHighestNumber : (a.Wage < g.CompanyLowestNumber ? g.CompanyLowestNumber : a.Wage)),
                                   qjsj = (DateTime)a.StartTime,
                                   bzlx = f.Name,
                                   YearMonth = (int)a.YearMonth
                               };
                var list = leftjoin.ToList<EmployeeAppdetail>();
                var empApp = new List<EmployeeAppdetail>();
                if (list != null && list.Count() >= 1)
                {
                    foreach (object item in list)
                    {
                        Type t = item.GetType();
                        EmployeeAppdetail temp = new EmployeeAppdetail();
                        temp.bxzl = (string)t.GetProperty("bxzl").GetValue(item, null);
                        temp.zclx = (string)t.GetProperty("zclx").GetValue(item, null);
                        temp.sbjs = (decimal)t.GetProperty("sbjs").GetValue(item, null);
                        temp.qjsj = (DateTime)t.GetProperty("qjsj").GetValue(item, null);
                        temp.bzlx = (string)t.GetProperty("bzlx").GetValue(item, null);
                        temp.YearMonth = (int)t.GetProperty("YearMonth").GetValue(item, null);
                        empApp.Add(temp);
                    }
                }
                var jsonData = new
                {
                    total = empApp.Count,// 总行数
                    rows = empApp
                };
                //string result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
                return Json(jsonData);
            }


        }

        public string Zrkf_FallBack(string ids, string message)
        {
            try
            {

                message = HttpUtility.HtmlDecode(message);

                var results = 0;//返回的结果
                // int companempID = int.Parse(ids);
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                using (var ent = new SysEntities())
                {

                    var empadd = ent.EmployeeAdd.Where(a => true);




                    var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id));
                    if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                    {
                        foreach (var item in updateEmpAdd)
                        {
                            item.State = EmployeeAdd_State.责任客服未通过.ToString();
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
                return "error";
            }
        }

        #endregion

        #region 员工客服经理分配
        /// <summary>
        /// 员工客服经理分配
        /// </summary>
        /// <param name="id">公司id</param>
        /// <param name="usernid">客服人员id</param>
        /// <returns></returns>


        public string FenpeiAction(string id, string usernid, string Citycode)
        {
            try
            {

                string result = "";
                var results = 0;//返回的结果
                int compID = int.Parse(id); //公司id

                using (var ent = new SysEntities())
                {

                    if (string.IsNullOrEmpty(usernid))
                    {
                        //result = Newtonsoft.Json.JsonConvert.SerializeObject("请选择客服人员！");
                        return "请选择客服人员";
                    }

                    var empadd = ent.UserCityCompany.Where(a => true);
                    empadd = empadd.Where(a => a.CompanyId == compID && a.CityId == Citycode);



                    var query1 = (from q in empadd
                                  select q).SingleOrDefault();
                    if (query1 != null)
                    {
                        query1.UserID_YG = int.Parse(usernid);
                        results = ent.SaveChanges();
                    }
                    else
                    {

                        UserCityCompany aa = new UserCityCompany()
                        {
                            ID = Common.Result.GetNewId(),
                            CompanyId = compID,
                            UserID_YG = int.Parse(usernid),
                            CityId = Citycode,
                            Status = (int)Status.启用

                        };
                        ent.UserCityCompany.Add(aa);
                        results = ent.SaveChanges();
                    }
                    result = "操作成功！";

                    return result;
                }
            }
            catch (Exception e)
            {
                return "操作失败！";
            }
        }
        #endregion

        #region 社保专员回退动作
        /// <summary>
        /// 社保专员报增信息
        /// </summary>
        /// <param name="ids">回退人员的id集合</param>
        /// <returns></returns>
        public string EmployeeFallbackAction(string ids, string message, string alltype)
        {
            try
            {

                message = HttpUtility.HtmlDecode(message);

                alltype = HttpUtility.HtmlDecode(alltype);
                string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // intArrayall = Array.ConvertAll<string, int>(strArrayall, s => int.Parse(s));
                List<int?> InsuranceKindTypes = new List<int?>();
                foreach (var a in strArrayall)
                {
                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                    InsuranceKindTypes.Add(InsuranceKindId);
                }



                var results = 0;//返回的结果
                // int companempID = int.Parse(ids);
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                using (var ent = new SysEntities())
                {

                    var empadd = ent.EmployeeAdd.Where(a => true);

                    var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));
                    if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                    {
                        foreach (var item in updateEmpAdd)
                        {
                            if (item.State == EmployeeAdd_State.社保专员已提取.ToString())
                            {
                                item.State = EmployeeAdd_State.员工客服已确认.ToString();
                            }
                            else
                            {
                                item.State = EmployeeAdd_State.待员工客服确认.ToString();
                            }
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

        #region 员工客服挂起 敬
        /// <summary>
        /// 员工客服挂起
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Common.ClientResult.Result POSTEmployeeHang(string ids, string message, string alltype)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            try
            {
                var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                string enable = Status.启用.ToString();
                alltype = HttpUtility.HtmlDecode(alltype);
                string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // intArrayall = Array.ConvertAll<string, int>(strArrayall, s => int.Parse(s));
                List<int?> InsuranceKindTypes = new List<int?>();
                foreach (var a in strArrayall)
                {
                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                    InsuranceKindTypes.Add(InsuranceKindId);
                }
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                using (var ent = new SysEntities())
                {
                    var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));
                    if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                    {
                        foreach (var item in updateEmpAdd)
                        {

                            item.InsuranceMonth = Convert.ToDateTime(item.InsuranceMonth).AddMonths(1);
                            string YearMonth = item.YearMonth.ToString();
                            string Year = YearMonth.Substring(0, 4);
                            string Month = YearMonth.Substring(4, 2);
                            string YearMonth1 = Year + "-" + Month + "-01";
                            item.YearMonth = Convert.ToInt32(Convert.ToDateTime(YearMonth1).AddMonths(1).ToString("yyyyMM"));//报增自然月
                            item.Remark = message;
                            item.UpdatePerson = LoginInfo.UserName;
                            item.UpdateTime = DateTime.Now;
                            decimal GZ = (decimal)item.Wage;
                            int ZC_ID = (int)item.PoliceInsuranceId;

                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                            var PoliceInsurancelist = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                            DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
                            Int32 Months = Business.CHA_Months((DateTime)item.StartTime, _NowDate.AddMonths(1).AddMonths((int)PoliceInsurancelist.InsuranceAdd));
                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                            employeeMiddle_BJ.InsuranceKindId = item.InsuranceKindId;
                            employeeMiddle_BJ.CompanyEmployeeRelationId = item.CompanyEmployeeRelationId; ;
                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;//.ToString("yyyyMM")
                            employeeMiddle_BJ.PaymentBetween = Convert.ToDateTime(item.StartTime).ToString("yyyyMM") + "-" + (_NowDate.AddMonths((int)PoliceInsurancelist.InsuranceAdd).ToString("yyyyMM"));
                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_ID);

                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_ID);

                            employeeMiddle_BJ.PaymentMonth = Months;
                            employeeMiddle_BJ.UseBetween = 0;
                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.AddMonths(1).ToString("yyyyMM"));
                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.AddMonths(1).ToString("yyyyMM"));
                            employeeMiddle_BJ.State = Status.启用.ToString();//正常
                            employeeMiddle_BJ.CityId = item.CompanyEmployeeRelation.CityId;
                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);



                            //var updateEmpAddmiddle = ent.EmployeeMiddle.Where(a => a.InsuranceKindId == item.InsuranceKindId && a.CompanyEmployeeRelationId == item.CompanyEmployeeRelationId && a.State == enable && (a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴));
                            //if (updateEmpAddmiddle != null && updateEmpAddmiddle.Count() >= 1)
                            //{
                            //    foreach (EmployeeMiddle itemn in updateEmpAddmiddle)
                            //    {
                            //        itemn.State = Status.停用.ToString();
                            //        itemn.UpdateTime = DateTime.Now;
                            //        item.UpdatePerson = LoginInfo.UserName;
                            //    }
                            //}
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
        /// <summary>
        /// 动态
        /// </summary>
        /// <param name="Reportedincreasedata"></param>
        /// <param name="CompanyId"></param>
        /// <param name="Direction"></param>
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
        public Common.ClientResult.Result POSTEmployeeAddCREATELimit1(string Cityid, string InsuranceKind)
        {
            string guan = "";
            string[] AddKind = InsuranceKind.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var s = (from a in SysEntitiesO2O.InsuranceKind where a.City == Cityid select a).ToList();
            List<string> aa = new List<string>();
            foreach (var a in s)
            {
                var PoliceCascadeRelationshiplist = SysEntitiesO2O.PoliceCascadeRelationship.FirstOrDefault(o => o.InsuranceKindId == a.Id);
                if (PoliceCascadeRelationshiplist != null)
                {
                    aa.Add(PoliceCascadeRelationshiplist.Tag);
                }
            }
            var e = (from a in SysEntitiesO2O.PoliceCascadeRelationship where aa.Contains(a.Tag) select a).ToList();

            List<PoliceCascadeRelationship> distinctPeople = e.GroupBy(p => p.InsuranceKindId).Select(g => g.First()).ToList();

            foreach (var u in distinctPeople)
            {
                guan = u.InsuranceKindId + "," + guan;
            }
            String[] guanlian = guan.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            // string[] guanlian = tempstrlist.Split(',');
            List<string> zuihou = new List<string>();

            //判断联动
            string tishi = "";
            if (!string.IsNullOrEmpty(InsuranceKind))
            {
                foreach (var kind in AddKind)
                {
                    if (tishi != "")//提示消息不为空，说明已经发现了违反联动正常的的现象，不必再重复判断
                    {
                        break;
                    }
                    if (guanlian.Contains(kind))//要报增的险种属于联动险种
                    {
                        foreach (var sd in guanlian)
                        {
                            if (!AddKind.Contains(sd))//要报增的险种不包含联动险种中的任何一个
                            {
                                //提示需要联动的险种必须同时报增
                                foreach (var sd1 in guanlian)
                                {
                                    int sad = Convert.ToInt32(sd1);
                                    var InsuranceKindnew = SysEntitiesO2O.InsuranceKind.FirstOrDefault(o => o.Id == sad);
                                    if (InsuranceKindnew != null)
                                    {
                                        tishi = tishi + "," + InsuranceKindnew.Name;
                                    }
                                }
                                break;//只要要报增的险种不包含联动险种中的任何一个，就可以跳出循环，不必再重复判断
                            }
                        }
                    }
                }

                //foreach (var sd in guanlian)
                //{
                //    if (!InsuranceKind.Contains(sd))
                //    {
                //        int sad = Convert.ToInt32(sd);
                //        var InsuranceKindnew = SysEntitiesO2O.InsuranceKind.FirstOrDefault(o => o.Id == sad);
                //        if (InsuranceKindnew != null)
                //        {
                //            tishi = tishi + "," + InsuranceKindnew.Name;
                //        }
                //    }
                //}
                if (tishi != "")
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Fail;
                    result.Message = tishi;
                    return result;
                }
                else
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "修改成功";
                    return result;
                }
            }
            else
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.FindNull;
                result.Message = "无保险";
                return result;
            }



        }
        #endregion
        #region 社保报增 敬
        [System.Web.Http.HttpPost]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postinfos">初始值</param>
        /// <param name="CompanyId">公司id</param>
        /// <param name="Direction">方向（1：平台，2：系统）</param>
        /// <returns></returns>
        public Common.ClientResult.Result POSTEmployeeAddCREATE1(string Reportedincreasedata, int CompanyId, int Direction = 1)
        {
            returnData postinfos = new returnData();
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            postinfos = (returnData)jsonSerializer.Deserialize(Reportedincreasedata, typeof(returnData));


            var employeelist = SysEntitiesO2O.Employee.FirstOrDefault(p => p.CertificateNumber == postinfos.IDNumber);
            //#region 平台新员工
            //if (employeelist == null)
            //{
            string aa = Addcommit1(postinfos, CompanyId, Direction);

            // string aa = "sdsd";
            try
            {
                if (aa == "")
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "报增成功";
                    return result;
                }
                else
                {
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Fail;
                    result.Message = aa;
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
        public string verification1(returnData postinfos)
        {
            StringBuilder Error = new StringBuilder();
            //不可以报增的情况
            string[] ZENG_STATUS = new string[]
                {
                     Common.EmployeeAdd_State.待责任客服确认.ToString(),  Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                     Common.EmployeeAdd_State.待员工客服确认.ToString(),Common.EmployeeAdd_State.员工客服已确认.ToString(),   Common.EmployeeAdd_State.社保专员已提取.ToString(),
                     Common.EmployeeAdd_State.已发送供应商.ToString(),Common.EmployeeAdd_State.待供应商客服提取.ToString()
                };
            string[] JIAN_STATUS = new string[]
                {
                     Common.EmployeeStopPayment_State.申报失败.ToString(),  Common.EmployeeStopPayment_State.终止.ToString(), Common.EmployeeStopPayment_State.责任客服未通过.ToString()                       
                };

            string EmployeeAdd_State = Common.EmployeeAdd_State.申报成功.ToString();
            var employeelist = SysEntitiesO2O.Employee.FirstOrDefault(p => p.CertificateNumber == postinfos.IDNumber);
            if (employeelist != null)
            {
                if (employeelist.Name != postinfos.Name)
                {
                    Error.Append("身份证号与姓名不匹配：" + postinfos.IDNumber + "<br />");
                }


                foreach (var Insurance in postinfos.Insurance)
                {
                    DateTime QJ_Time = DateTime.MinValue;

                    DateTime.TryParse(Insurance.StartTime, out QJ_Time);

                    List<string> cardList = new List<string>();

                    DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天


                    if (QJ_Time != DateTime.MinValue)
                    {

                        int state = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);
                        //var companyEmployeeRelationlist = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.EmployeeId == employeelist.Id && p.CityId == postinfos.City && p.State == "在职");
                        var companyEmployeeRelationlist = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.EmployeeId == employeelist.Id && p.CompanyId == postinfos.CompanyId && p.State == "在职");
                        if (companyEmployeeRelationlist != null)
                        {

                            //if (companyEmployeeRelationlist.PoliceAccountNatureId != postinfos.PoliceAccountNature)
                            //{
                            //    Error.Append("请确认已报增险种的户口性质与此次报增户口性质一致<br />");
                            //}



                            if (SysEntitiesO2O.EmployeeAdd.FirstOrDefault(f => f.CompanyEmployeeRelationId == companyEmployeeRelationlist.Id && ZENG_STATUS.Contains(f.State) && f.InsuranceKindId == state) != null)
                            {
                                Error.Append("已存在正在报增的" + Insurance.InsuranceKind + "保险<br />");
                            }

                            var ReportedSuccess = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(f => f.CompanyEmployeeRelationId == companyEmployeeRelationlist.Id && f.State == EmployeeAdd_State && f.InsuranceKindId == state);
                            if (ReportedSuccess != null)
                            {
                                if (ReportedSuccess.EmployeeStopPayment.FirstOrDefault() == null || ReportedSuccess.EmployeeStopPayment.FirstOrDefault(f => JIAN_STATUS.Contains(f.State)) == null)
                                {
                                    Error.Append("已存在报增成功的" + Insurance.InsuranceKind + "保险<br />");
                                }
                            }
                        }
                        if (Insurance.PoliceInsurance != null)
                        {
                            int ZC_ID = (int)Insurance.PoliceInsurance;
                            var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                            if (Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth || Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) < 0)
                            {
                                Error.Append("" + Insurance.InsuranceKind + "起缴时间超出政策允许补缴数<br />");
                            }

                            if (PoliceInsurance.MaxPayMonth == 0) //不允许补缴
                            {
                                if (QJ_Time.ToString("yyyyMM") != _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).ToString("yyyyMM"))
                                {
                                    Error.Append("此社保机构" + Insurance.InsuranceKind + "不允许补缴，请修改起缴时间或社保政策后再申报！<br />");

                                }
                            }
                            else if (PoliceInsurance.MaxPayMonth == -1)
                            {
                            }
                            else
                            {
                                if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Time.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                                {
                                    Error.Append("此社保机构" + Insurance.InsuranceKind + "只允许补缴 " + PoliceInsurance.MaxPayMonth +
                                                      " 个月，请修改起缴时间或社保政策后再申报！<br />");

                                }
                            }

                        }


                    }
                }
            }
            return Error.ToString();

        }
        #endregion
        #region 平台验证 敬
        public string Platform_verification1(returnData postinfos)
        {
            StringBuilder Error = new StringBuilder();
            //不可以报增的情况
            string[] ZENG_STATUS = new string[]
                {

                     Common.EmployeeAdd_State.待责任客服确认.ToString(),   Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                     Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString(),  Common.EmployeeAdd_State.社保专员已提取.ToString(),
                     Common.EmployeeAdd_State.已发送供应商.ToString(),Common.EmployeeAdd_State.待供应商客服提取.ToString()

                   
                };
            string[] JIAN_STATUS = new string[]
                {
                     Common.EmployeeStopPayment_State.申报失败.ToString(),  Common.EmployeeStopPayment_State.终止.ToString(), Common.EmployeeStopPayment_State.责任客服未通过.ToString()                       
                };
            string EmployeeAdd_State = Common.EmployeeAdd_State.申报成功.ToString();

            List<string> cardList = new List<string>();
            #region 为空判断
            if (postinfos.Name == "")
            {
                Error.Append("员工姓名未填写<br />");
            }
            if (postinfos.IDType == "")
            {
                Error.Append("证件类型未填写<br />");
            }
            if (postinfos.Telephone == "")
            {
                Error.Append("员工电话未填写<br />");
            }
            if (postinfos.City == "")
            {
                Error.Append("城市编码未填写<br />");
            }
            if (postinfos.IDNumber == "")
            {
                Error.Append("身份证号未填写<br />");
            }
            #endregion
            else
            {
                if (!CardCommon.CheckCardID18(postinfos.IDNumber))
                {
                    Error.Append("身份证号不合法：" + postinfos.IDNumber + "<br />");
                }
                var employeelist = SysEntitiesO2O.Employee.FirstOrDefault(p => p.CertificateNumber == postinfos.IDNumber);
                if (employeelist != null)
                {
                    if (employeelist.Name != postinfos.Name)
                    {
                        Error.Append("身份证号与姓名不匹配：" + postinfos.IDNumber + "<br />");
                    }

                    DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
                    //var companyEmployeeRelationlist = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.EmployeeId == employeelist.Id && p.CityId == postinfos.City && p.State == "在职");
                    var companyEmployeeRelationlist = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.EmployeeId == employeelist.Id && p.CompanyId == postinfos.CompanyId && p.State == "在职");
                    if (companyEmployeeRelationlist != null)
                    {

                        foreach (var Insurance in postinfos.Insurance)
                        {
                            DateTime QJ_Time = DateTime.MinValue;

                            DateTime.TryParse(Insurance.StartTime, out QJ_Time);


                            if (QJ_Time != DateTime.MinValue)
                            {


                                int state = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);


                                if (companyEmployeeRelationlist != null)
                                {
                                    if (companyEmployeeRelationlist.PoliceAccountNatureId != postinfos.PoliceAccountNature)
                                    {
                                        Error.Append("请确认已报增险种的户口性质与此次报增户口性质一致<br />");
                                    }
                                    if (SysEntitiesO2O.EmployeeAdd.FirstOrDefault(f => f.CompanyEmployeeRelationId == companyEmployeeRelationlist.Id && ZENG_STATUS.Contains(f.State) && f.InsuranceKindId == state) != null)
                                    {
                                        Error.Append("已存在正在报增的" + Insurance.InsuranceKind + "保险<br />");
                                    }

                                    var ReportedSuccess = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(f => f.CompanyEmployeeRelationId == companyEmployeeRelationlist.Id && f.State == EmployeeAdd_State && f.InsuranceKindId == state);
                                    if (ReportedSuccess != null)
                                    {
                                        if (ReportedSuccess.EmployeeStopPayment == null || ReportedSuccess.EmployeeStopPayment.FirstOrDefault(f => JIAN_STATUS.Contains(f.State)) == null)
                                        {
                                            Error.Append("已存在报增成功的" + Insurance.InsuranceKind + "保险<br />");
                                        }
                                    }
                                }
                                if (Insurance.PoliceInsurance != null)
                                {

                                    int ZC_ID = (int)Insurance.PoliceInsurance;
                                    var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                                    if (Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth || Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) < 0)
                                    {
                                        Error.Append("" + Insurance.InsuranceKind + "起缴时间超出政策允许补缴数<br />");
                                    }



                                    if (PoliceInsurance.MaxPayMonth == 0) //不允许补缴
                                    {
                                        if (QJ_Time.ToString("yyyyMM") != _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).ToString("yyyyMM"))
                                        {
                                            Error.Append("此社保机构" + Insurance.InsuranceKind + "不允许补缴，请修改起缴时间或社保政策后再申报！<br />");

                                        }
                                    }
                                    else if (PoliceInsurance.MaxPayMonth == -1)
                                    {
                                    }
                                    else
                                    {

                                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Time.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                                        {
                                            Error.Append("此社保机构" + Insurance.InsuranceKind + "只允许补缴 " + PoliceInsurance.MaxPayMonth +
                                                              " 个月，请修改起缴时间或社保政策后再申报！<br />");

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var Insurance in postinfos.Insurance)
                    {
                        DateTime QJ_Time = DateTime.MinValue;
                        DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
                        DateTime.TryParse(Insurance.StartTime, out QJ_Time);


                        if (QJ_Time != DateTime.MinValue)
                        {
                            #region 不能报增情况
                            if (QJ_Time != DateTime.MinValue)
                            {
                                if (Insurance.Wage == null)
                                {
                                    Error.Append("" + Insurance.InsuranceKind + "工资未填写<br />");
                                }

                                if (Insurance.PoliceInsurance == null)
                                {
                                    Error.Append("" + Insurance.InsuranceKind + "社保政策标示未填写<br />");
                                }
                                if (Insurance.PoliceOperation == null)
                                {
                                    Error.Append("" + Insurance.InsuranceKind + "政策手续未填写<br />");
                                }
                                int state = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);


                                if (Insurance.PoliceInsurance != null)
                                {

                                    int ZC_ID = (int)Insurance.PoliceInsurance;
                                    var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                                    if (Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth || Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) < 0)
                                    {
                                        Error.Append("" + Insurance.InsuranceKind + "起缴时间超出政策允许补缴数<br />");
                                    }



                                    if (PoliceInsurance.MaxPayMonth == 0) //不允许补缴
                                    {
                                        if (QJ_Time.ToString("yyyyMM") != _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).ToString("yyyyMM"))
                                        {
                                            Error.Append("此社保机构" + Insurance.InsuranceKind + "不允许补缴，请修改起缴时间或社保政策后再申报！<br />");

                                        }
                                    }
                                    else if (PoliceInsurance.MaxPayMonth == -1)
                                    {
                                    }
                                    else
                                    {

                                        //if (QJ_Yanglao.AddMonths((int)PoliceInsurance.MaxPayMonth) < _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd))
                                        if (SqlMethods.DateDiffMonth(_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd), QJ_Time.AddMonths((int)PoliceInsurance.MaxPayMonth)) < 0)
                                        {
                                            Error.Append("此社保机构" + Insurance.InsuranceKind + "只允许补缴 " + PoliceInsurance.MaxPayMonth +
                                                              " 个月，请修改起缴时间或社保政策后再申报！<br />");

                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }

            }

            return Error.ToString();

        }
        #endregion
        #region 提交 敬
        /// <summary>
        /// 社保报增提交
        /// </summary>
        /// <param name="postinfos">初始值</param>
        /// <param name="CompanyId">公司id</param>
        /// <param name="Direction">方向（1：平台，2：系统）</param>
        /// <returns></returns>
        public string Addcommit1([FromBody]returnData postinfos, int CompanyId, int Direction)
        {
            var employeelist = SysEntitiesO2O.Employee.FirstOrDefault(p => p.CertificateNumber == postinfos.IDNumber);
            int companyEmployeeRelationID = 0;
            StringBuilder sbError = new StringBuilder();
            if (CompanyId > 0)
            {
                string error = "";

                if (employeelist == null)
                {
                    postinfos.CompanyId = CompanyId;
                    error = Platform_verification1(postinfos);
                }
                else
                {
                    postinfos.CompanyId = CompanyId;
                    error = verification1(postinfos);
                    postinfos.EmployeeId = employeelist.Id;
                }
                if (error != "")
                {
                    sbError.Append("申报失败以下信息错误：<br />");
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
                            var Employeebll = new BLL.EmployeeAddBLL();
                            var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                            Employee employee = new Employee();//员工表
                            EmployeeContact employeeContact = new EmployeeContact();//联系人表
                            if (employeelist == null)
                            {
                                //员工
                                employee.Name = postinfos.Name;
                                employee.CertificateType = postinfos.IDType;
                                employee.CertificateNumber = postinfos.IDNumber;
                                employee.AccountType = postinfos.ResidentType;
                                employee.Sex = Common.CardCommon.Getsex(postinfos.IDNumber) == 1 ? "男" : "女";
                                employee.Birthday = Common.CardCommon.GetShengRi(postinfos.IDNumber);
                                employee.CreateTime = DateTime.Now;
                                employee.CreatePerson = LoginInfo.UserName;
                                SysEntitiesO2O.Employee.Add(employee);
                                SysEntitiesO2O.SaveChanges();
                                //联系人
                                employeeContact.MobilePhone = postinfos.Telephone;
                                employeeContact.Address = postinfos.ResidentLocation;
                                employeeContact.CreateTime = DateTime.Now;
                                employeeContact.CreatePerson = LoginInfo.UserName;
                                employeeContact.EmployeeId = employee.Id;
                                SysEntitiesO2O.EmployeeContact.Add(employeeContact);
                                SysEntitiesO2O.SaveChanges();
                                postinfos.EmployeeId = employee.Id;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(postinfos.Telephone))
                                {

                                    postinfos.EmployeeId = employeelist.Id;
                                    //联系信息
                                    var Contact = SysEntitiesO2O.EmployeeContact.FirstOrDefault(o => o.EmployeeId == employeelist.Id);
                                    if (Contact != null)
                                    {
                                        if (!string.IsNullOrEmpty(postinfos.Telephone))
                                        {
                                            Contact.MobilePhone = postinfos.Telephone;
                                            // SysEntitiesO2O.SaveChanges();
                                        }
                                        if (!string.IsNullOrEmpty(postinfos.ResidentLocation))
                                        {
                                            Contact.Address = postinfos.ResidentLocation;
                                        }
                                    }
                                    else
                                    {
                                        employeeContact.MobilePhone = postinfos.Telephone;
                                        employeeContact.Address = postinfos.ResidentLocation;
                                        employeeContact.CreateTime = DateTime.Now;
                                        employeeContact.CreatePerson = LoginInfo.UserName;
                                        employeeContact.EmployeeId = employeelist.Id;
                                        SysEntitiesO2O.EmployeeContact.Add(employeeContact);

                                    }
                                }
                                if (!string.IsNullOrEmpty(postinfos.ResidentType))
                                {
                                    employeelist.AccountType = postinfos.ResidentType;
                                }

                                if (!string.IsNullOrEmpty(postinfos.IDType))
                                {
                                    employeelist.CertificateType = postinfos.IDType;
                                }
                                // SysEntitiesO2O.SaveChanges();
                            }
                            CompanyEmployeeRelation companyEmployeeRelation = new CompanyEmployeeRelation();//员工企业关系表
                            IBLL.ICompanyEmployeeRelationBLL CEmployeeRelation_BLL = new BLL.CompanyEmployeeRelationBLL();

                            int EmployeeId = Convert.ToInt32(postinfos.EmployeeId);
                            var CompanyEmployeeRelationList = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(o => o.CompanyId == CompanyId && o.EmployeeId == EmployeeId && o.State == "在职");
                            if (CompanyEmployeeRelationList == null)
                            {
                                //员工关系表
                                companyEmployeeRelation.CityId = postinfos.City;
                                companyEmployeeRelation.CompanyId = CompanyId;
                                companyEmployeeRelation.EmployeeId = EmployeeId;
                                companyEmployeeRelation.State = "在职";
                                //companyEmployeeRelation.Station = postinfos.Station;
                                companyEmployeeRelation.PoliceAccountNatureId = postinfos.PoliceAccountNature;
                                companyEmployeeRelation.CreateTime = DateTime.Now;
                                companyEmployeeRelation.Station = postinfos.Station;
                                companyEmployeeRelation.CreatePerson = LoginInfo.UserName;
                                SysEntitiesO2O.CompanyEmployeeRelation.Add(companyEmployeeRelation);
                                SysEntitiesO2O.SaveChanges();
                                companyEmployeeRelationID = companyEmployeeRelation.Id;
                            }
                            else
                            {
                                companyEmployeeRelationID = CompanyEmployeeRelationList.Id;
                                if (!string.IsNullOrEmpty(postinfos.Station))
                                {
                                    CompanyEmployeeRelationList.Station = postinfos.Station;
                                }
                            }

                            DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天
                            foreach (var Insurance in postinfos.Insurance)
                            {
                                DateTime QJ_Time = DateTime.MinValue;
                                DateTime.TryParse(Insurance.StartTime, out QJ_Time);
                                if (QJ_Time != DateTime.MinValue)
                                {
                                    int state = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);
                                    decimal GZ = (decimal)Insurance.Wage;
                                    int ZC_ID = (int)Insurance.PoliceInsurance;

                                    var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                    var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                    var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                    var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                    EmployeeAdd employeeAdd = new EmployeeAdd();
                                    employeeAdd.Wage = GZ;//工资
                                    var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                                    employeeAdd.InsuranceKindId = state;//社保种类
                                    employeeAdd.CompanyEmployeeRelationId = companyEmployeeRelationID;//员工关系
                                    employeeAdd.PoliceInsuranceId = ZC_ID;//社保政策
                                    employeeAdd.InsuranceCode = Insurance.InsuranceNumber;//社保编号                                   
                                    employeeAdd.SupplierRemark = Insurance.SupplierRemark;//供应商备注
                                    //employeeAdd.PoliceOperationId = postinfos.Pension_PoliceOperation;//政策手续
                                    employeeAdd.PoliceOperationId = Insurance.PoliceOperation;//政策手续
                                    employeeAdd.InsuranceMonth = DateTime.Now.AddMonths((int)PoliceInsurance.InsuranceAdd);//社保月
                                    employeeAdd.StartTime = QJ_Time;//起缴时间
                                    employeeAdd.CreateTime = DateTime.Now;
                                    employeeAdd.CreatePerson = LoginInfo.UserName;
                                    // employeeAdd.CreatePerson = UID;
                                    employeeAdd.PoliceAccountNatureId = postinfos.PoliceAccountNature;//户口性质
                                    employeeAdd.IsIndependentAccount = "是";//是否单立户
                                    employeeAdd.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
                                    employeeAdd.SupplierRemark = Insurance.SupplierRemark;//供应商备注
                                    if (Direction == 1)
                                    {
                                        employeeAdd.State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                                    }
                                    else
                                    {
                                        if (postinfos.SuppliersId != null)
                                        {
                                            employeeAdd.SuppliersId = postinfos.SuppliersId; 
                                            employeeAdd.State = Common.EmployeeAdd_State.待供应商客服提取.ToString();
                                        }
                                        else
                                        {
                                            employeeAdd.State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                                        }
                                    }

                                    Employeebll.CreateEmployee(SysEntitiesO2O, employeeAdd);
                                    #region 正常



                                    EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                                    employeeMiddle.InsuranceKindId = state;
                                    employeeMiddle.CompanyEmployeeRelationId = companyEmployeeRelationID;
                                    employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                                    employeeMiddle.CompanyBasePayment = JISHU_C;
                                    employeeMiddle.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_C, PERCENT_C, 1, ZC_ID);


                                    //Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                                    employeeMiddle.EmployeeBasePayment = JISHU_P;
                                    employeeMiddle.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_P, PERCENT_P, 1, ZC_ID);

                                    //Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                                    employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                                    employeeMiddle.UseBetween = 0;
                                    employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                    employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                                    employeeMiddle.State = Status.启用.ToString();//正常
                                    employeeMiddle.CityId = postinfos.City;
                                    employeeMiddle.CreateTime = DateTime.Now;
                                    employeeMiddle.CreatePerson = LoginInfo.UserName;
                                    EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle);
                                    #endregion
                                    #region 补缴
                                    if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                                    {

                                        Int32 Months = Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                                        var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                        var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                        var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                        var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                        EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                        employeeMiddle_BJ.InsuranceKindId = state;
                                        employeeMiddle_BJ.CompanyEmployeeRelationId = companyEmployeeRelationID;
                                        employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                                        employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                        employeeMiddle_BJ.PaymentBetween = QJ_Time.ToString("yyyyMM") + "-" + (_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).AddMonths(-1).ToString("yyyyMM"));
                                        employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_ID);
                                        // Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                        employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                        employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_ID);
                                        //Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                        employeeMiddle_BJ.PaymentMonth = Months;
                                        employeeMiddle_BJ.UseBetween = 0;
                                        employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                        employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                        employeeMiddle_BJ.State = Status.启用.ToString();//正常
                                        employeeMiddle_BJ.CityId = postinfos.City;
                                        employeeMiddle_BJ.CreateTime = DateTime.Now;
                                        employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                        EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);

                                    }
                                    #endregion

                                }

                            }
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
                    //catch (Exception er)
                    //{
                    //    //var movieToUpdate = SysEntitiesO2O.CompanyEmployeeRelation.First(m => m.Id == companyEmployeeRelationID);
                    //    //SysEntitiesO2O.CompanyEmployeeRelation.Remove(movieToUpdate);
                    //    //SysEntitiesO2O.SaveChanges();
                    //    return "异常";
                    //}

                }
                #endregion
            }
            else
            {
                return "无此企业";
            }
        }

        #endregion
        #region 更新报增表 社保编号 王帅
        /// <summary>
        /// 更新报增表
        /// </summary>
        /// <param name="dt">导入的数据</param>
        /// <returns></returns>
        public string AddcommitUpdate(DataTable dt)
        {

            string result = string.Empty;
            int successcount = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        string[] InKind = dt.Rows[i]["险种"].ToString().Split(',');
                        int InKindcount = InKind.Length;
                        if (InKindcount > 1)
                        {
                            for (int j = 0; j < InKindcount; j++)
                            {

                                var employeelist = SysEntitiesO2O.EmployeeAdd.Where(o => true);// a 报增表
                                var relation = SysEntitiesO2O.CompanyEmployeeRelation.Where(o => true);// b 企业员工关系                                
                                var emp = SysEntitiesO2O.Employee.Where(o => true);//c 员工表
                                string CerNumber = dt.Rows[i]["身份证号"].ToString();
                                emp = emp.Where(o => o.CertificateNumber == CerNumber);//身份证号作为条件;

                                var InKindType = SysEntitiesO2O.InsuranceKind.Where(o => true);//d  险种
                                string InKindName = InKind[j].ToString();
                                InKindType = InKindType.Where(o => o.Name == InKindName);//险种条件
                                var CityInf = SysEntitiesO2O.City.Where(o => true);// e 城市
                                string Cityname = dt.Rows[i]["社保缴纳地"].ToString();//社保缴纳地
                                CityInf = CityInf.Where(o => o.Name == Cityname);


                                var query1 = (from a in employeelist
                                              join b in relation on a.CompanyEmployeeRelationId equals b.Id
                                              join c in emp on b.EmployeeId equals c.Id
                                              join d in InKindType on a.InsuranceKindId equals d.Id
                                              join e in CityInf on b.CityId equals e.Id
                                              select a).SingleOrDefault();
                                if (query1 != null)
                                {
                                    query1.InsuranceCode = dt.Rows[i]["社保编号"].ToString();
                                    SysEntitiesO2O.SaveChanges();
                                    successcount++;
                                }
                                else
                                {
                                    result += "系统不存在客户" + dt.Rows[i]["姓名"].ToString() + InKindName + "保险信息" + ",";
                                }

                            }

                        }
                        else
                        {

                            var employeelist = SysEntitiesO2O.EmployeeAdd.Where(o => true);// a 报增表
                            var relation = SysEntitiesO2O.CompanyEmployeeRelation.Where(o => true);// b 企业员工关系   


                            var emp = SysEntitiesO2O.Employee.Where(o => true);//c 员工表
                            string CerNumber = dt.Rows[i]["身份证号"].ToString();
                            emp = emp.Where(o => o.CertificateNumber == CerNumber);//身份证号作为条件;


                            var InKindType = SysEntitiesO2O.InsuranceKind.Where(o => true);//d  险种
                            string InKindName = dt.Rows[i]["险种"].ToString(); ;
                            InKindType = InKindType.Where(o => o.Name == InKindName);//险种条件


                            var CityInf = SysEntitiesO2O.City.Where(o => true);// e 城市
                            string Cityname = dt.Rows[i]["社保缴纳地"].ToString();//社保缴纳地
                            CityInf = CityInf.Where(o => o.Name == Cityname);


                            var query1 = (from a in employeelist
                                          join b in relation on a.CompanyEmployeeRelationId equals b.Id
                                          join c in emp on b.EmployeeId equals c.Id
                                          join d in InKindType on a.InsuranceKindId equals d.Id
                                          join e in CityInf on b.CityId equals e.Id
                                          select a).SingleOrDefault();
                            if (query1 != null)
                            {

                                query1.InsuranceCode = dt.Rows[i]["社保编号"].ToString();
                                SysEntitiesO2O.SaveChanges();
                                successcount++;

                            }
                            else
                            {
                                result += "系统不存在客户" + dt.Rows[i]["姓名"].ToString() + InKindName + "保险信息" + ",";
                            }
                        }
                    }
                    scope.Complete();
                    // Common.ClientResult.Result result = new Common.ClientResult.Result();
                    //result.Code = ClientCode.Succeed;
                    // result = "修改成功";
                    result = "更新" + successcount + "条" + result;

                }
            }
            catch (Exception e)
            {
                throw e;
                result = e.Message;
            }


            return result.TrimEnd(',');

        }

        #endregion
        #region 员工客服修改页面初始化 敬
        /// <summary>
        /// 员工客服修改页面初始化
        /// </summary>
        /// <param name="CompanyEmployeeRelationId">员工关系id</param>
        /// <param name="type">保险类别</param>
        /// <returns></returns>
        public IHttpActionResult getEmployeeAddList1(int CompanyEmployeeRelationId, string typeKind, string YearMonth, int? Parameter)
        {
            if (!string.IsNullOrEmpty(typeKind))
            {
                typeKind = HttpUtility.HtmlDecode(typeKind);
                int type = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), typeKind);
                int YearMonth1 = Convert.ToInt32(YearMonth);
                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                else if (Parameter == 3)
                {
                    State = Common.EmployeeAdd_State.待责任客服确认.ToString();
                }
                else if (Parameter != null)
                {
                    State = ((Common.EmployeeAdd_State)Parameter).ToString();
                }
                // var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State);
                var data = (SysEntitiesO2O.EmployeeAdd.Select(c => new
                {
                    id = c.Id,
                    CompanyEmployeeRelationId = c.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    Wage = c.Wage,
                    StartTime = c.StartTime,
                    PoliceOperationId = c.PoliceOperation.Id,
                    PoliceOperationName = c.PoliceOperation.Name,
                    PoliceInsuranceId = c.PoliceInsurance.Id,
                    PoliceInsuranceName = c.PoliceInsurance.Name,
                    InsuranceCode = c.InsuranceCode,
                    YearMonth = c.YearMonth,
                    SupplierRemark=c.SupplierRemark
                    // Name = s.Name

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == type && c.State == State && c.YearMonth == YearMonth1).FirstOrDefault());

                return Json(data);

            }
            else
            {
                return null;
            }

        }
        #endregion
        #region 责任客服修改员工信息 敬
        public Common.ClientResult.Result POSTEmployeeAddModify1(string Reportedincreasedata)
        {
            Reportedincreasedata = HttpUtility.HtmlDecode(Reportedincreasedata);
            returnData postinfos = new returnData();
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            postinfos = (returnData)jsonSerializer.Deserialize(Reportedincreasedata, typeof(returnData));
            StringBuilder sbError = new StringBuilder();
            string error = "";
            error = verificationBacknumber1(postinfos);
            int[] EmployeeMiddle_STATUS = new int[]
                {
                    (int)Common.EmployeeMiddle_PaymentStyle.正常,   (int)Common.EmployeeMiddle_PaymentStyle.补缴,       
                };
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
                string Enable = Status.启用.ToString();
                string Disable = Status.停用.ToString();
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                        DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
                        foreach (var Insurance in postinfos.Insurance)
                        {
                            DateTime QJ_Time = DateTime.MinValue;
                            DateTime.TryParse(Insurance.StartTime, out QJ_Time);
                            #region 修改
                            if (QJ_Time != DateTime.MinValue)
                            {
                                var employeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == Insurance.Id);
                                if (employeeAdd != null)
                                {
                                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);


                                    if (employeeAdd.Wage != Insurance.Wage || DateTime.Compare((DateTime)employeeAdd.StartTime, QJ_Time) != 0 || employeeAdd.SupplierRemark != Insurance.SupplierRemark)
                                    {
                                        employeeAdd.SupplierRemark = Insurance.SupplierRemark;
                                        employeeAdd.StartTime = QJ_Time;
                                        employeeAdd.Wage = Insurance.Wage;
                                        employeeAdd.UpdateTime = DateTime.Now;
                                        var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == employeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && (o.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || o.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴));
                                        if (EmployeeMiddle.Count() > 0)
                                        {
                                            foreach (var order in EmployeeMiddle)
                                            {
                                                order.State = Disable; order.UpdateTime = DateTime.Now;


                                            }
                                        }
                                        decimal GZ = (decimal)Insurance.Wage;
                                        int ZC_ID = (int)employeeAdd.PoliceInsuranceId;
                                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                                        var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == employeeAdd.CompanyEmployeeRelationId);
                                        #region 正常
                                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                                        employeeMiddle.InsuranceKindId = InsuranceKindId;
                                        employeeMiddle.CompanyEmployeeRelationId = employeeAdd.CompanyEmployeeRelationId;
                                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                                        employeeMiddle.CompanyBasePayment = JISHU_C;
                                        employeeMiddle.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_C, PERCENT_C, 1, ZC_ID);
                                        //Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                                        employeeMiddle.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_P, PERCENT_P, 1, ZC_ID);
                                        //Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                                        employeeMiddle.UseBetween = 0;
                                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                                        employeeMiddle.State = Enable;//正常
                                        employeeMiddle.CityId = CompanyEmployeeRelation.CityId;
                                        employeeMiddle.CreateTime = DateTime.Now;
                                        employeeMiddle.CreatePerson = LoginInfo.UserName;
                                        EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle);
                                        #endregion
                                        #region 补缴
                                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                                        {
                                            Int32 Months = Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                            employeeMiddle_BJ.InsuranceKindId = InsuranceKindId;
                                            employeeMiddle_BJ.CompanyEmployeeRelationId = employeeAdd.CompanyEmployeeRelationId;
                                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                            employeeMiddle_BJ.PaymentBetween = QJ_Time.ToString("yyyyMM") + "-" + (_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).AddMonths(-1).ToString("yyyyMM"));
                                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_ID);
                                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_ID);
                                            employeeMiddle_BJ.PaymentMonth = Months;
                                            employeeMiddle_BJ.UseBetween = 0;
                                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                            employeeMiddle_BJ.State = Enable;//正常
                                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                                            employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                        }
                                        #endregion
                                    }


                                }
                            }
                            #endregion

                        }
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
        #region 起缴时间补缴月份超出政策允许补缴数 敬
        private string verificationBacknumber1(returnData postinfos)
        {
            StringBuilder Error = new StringBuilder();

            foreach (var Insurance in postinfos.Insurance)
            {
                DateTime QJ_Time = DateTime.MinValue;
                DateTime.TryParse(Insurance.StartTime, out QJ_Time);
                List<string> cardList = new List<string>();
                DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//当前月的第一天

                if (QJ_Time != DateTime.MinValue)
                {
                    var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == Insurance.Id);

                    var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == EmployeeAdd.PoliceInsuranceId);
                    if (Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > PoliceInsurance.MaxPayMonth)
                    {
                        Error.Append("" + Insurance.InsuranceKind + "起缴时间补缴月份超出政策允许补缴数<br />");
                    }

                }

            }
            return Error.ToString();

        }
        #endregion
        #region 员工客服修改员工信息 敬
        public Common.ClientResult.Result POSTEmployeeAddEmployeeModify1(string Reportedincreasedata, int CompanyEmployeeRelationid)
        {
            returnData postinfos = new returnData();
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            postinfos = (returnData)jsonSerializer.Deserialize(Reportedincreasedata, typeof(returnData));
            string Enable = Status.启用.ToString();
            string Disable = Status.停用.ToString();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var CompanyEmployeeRelationlist = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == CompanyEmployeeRelationid);
                    if (CompanyEmployeeRelationlist.PoliceAccountNatureId != postinfos.PoliceAccountNature)
                    {
                        CompanyEmployeeRelationlist.PoliceAccountNatureId = postinfos.PoliceAccountNature;
                    }
                    var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                    DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
                    foreach (var Insurance in postinfos.Insurance)
                    {
                        #region 修改
                        var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == Insurance.Id);
                        if (EmployeeAdd != null)
                        {
                            int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);
                            DateTime QJ_Yanglao = (DateTime)EmployeeAdd.StartTime;
                            if (EmployeeAdd.PoliceOperationId != Insurance.PoliceOperation || EmployeeAdd.PoliceInsuranceId != Insurance.PoliceInsurance)
                            {
                                EmployeeAdd.PoliceOperationId = Insurance.PoliceOperation;
                                EmployeeAdd.PoliceInsuranceId = Insurance.PoliceInsurance;
                                EmployeeAdd.UpdateTime = DateTime.Now;
                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && (o.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || o.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴));
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {
                                        order.State = Disable; order.UpdateTime = DateTime.Now;
                                    }
                                }
                                decimal GZ = (decimal)EmployeeAdd.Wage;
                                int ZC_ID = (int)Insurance.PoliceInsurance;
                                var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                                var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                                #region 正常
                                EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                                employeeMiddle.InsuranceKindId = InsuranceKindId;
                                employeeMiddle.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                                employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                                employeeMiddle.CompanyBasePayment = JISHU_C;
                                employeeMiddle.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_C, PERCENT_C, 1, ZC_ID);
                                //Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                                employeeMiddle.EmployeeBasePayment = JISHU_P;
                                employeeMiddle.UseBetween = 0;
                                employeeMiddle.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_P, PERCENT_P, 1, ZC_ID);
                                //Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                                employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                                employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                                employeeMiddle.State = Enable;//正常
                                employeeMiddle.CityId = CompanyEmployeeRelation.CityId;
                                employeeMiddle.CreateTime = DateTime.Now;
                                employeeMiddle.CreatePerson = LoginInfo.UserName;
                                EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle);
                                #endregion
                                #region 补缴
                                if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Yanglao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                                {
                                    Int32 Months = Business.CHA_Months(QJ_Yanglao, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                                    var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                    var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                    var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                    var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                    EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                    employeeMiddle_BJ.InsuranceKindId = InsuranceKindId;
                                    employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                                    employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                                    employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                    employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_ID);
                                    employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                    employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_ID);
                                    employeeMiddle_BJ.PaymentMonth = Months;
                                    employeeMiddle_BJ.UseBetween = 0;
                                    employeeMiddle_BJ.PaymentBetween = QJ_Yanglao.ToString("yyyyMM") + "-" + (_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).AddMonths(-1).ToString("yyyyMM"));
                                    employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                    employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                    employeeMiddle_BJ.State = Enable;//正常
                                    employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                    employeeMiddle_BJ.CreateTime = DateTime.Now;
                                    employeeMiddle_BJ.CreatePerson = LoginInfo.UserName;
                                    EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                }
                                #endregion
                            }


                        }
                        #endregion
                    }
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
                result.Message = "修改失败";
                return result;
            }

        }
        #endregion
        #region 社保专员导出报增信息列表

        /// <summary>
        /// 社保专员导出报增信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult SupplierExport1([FromBody]GetDataParam getParam)
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
                search += "UserID_SB&" + LoginInfo.UserID + "^";
                string excelName = "供应商导出" + DateTime.Now.ToString();
                using (MemoryStream ms = new MemoryStream())
                {

                    #region 生成Excel


                    workbook.SetSheetName(0, "供应商导出");
                    // List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
                    List<EmployeeAddView> queryData = m_BLL.GetEmployeeAddExcelList1(1, int.MaxValue, search, ref total);

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

                    List<string> CertificateNumberlist = new List<string>();
                    List<string> CityList = new List<string>();
                    List<int> YearMonthList = new List<int>();


                    List<EmployeeAddView> sdsd = new List<EmployeeAddView>();
                    #region 标头
                    foreach (var InsuranceKindname in Enum.GetNames(typeof(Common.EmployeeAdd_InsuranceKindId)))
                    {
                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）政策手续");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）社保政策");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）单位比例");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）个人比例");
                        colNum++;
                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）工资");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）起缴时间");
                        colNum++;
                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）户口性质");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）报增自然月");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）社保月");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）是否单立户");
                        colNum++;
                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）社保编号");
                        colNum++;

                        cell = currentRow.CreateCell(colNum);
                        cell.SetCellValue("（" + (Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), InsuranceKindname) + "）创建时间");
                        colNum++;

                    }
                    #endregion
                    #region 数据输出
                    foreach (var query in queryData)
                    {
                        // var ii1 = queryData.Where(a => a.CertificateNumber == query.CertificateNumber && a.City == query.City && a.YearMonth == query.YearMonth);

                        //if (!CertificateNumberlist.Contains(query.CertificateNumber) && !CityList.Contains(query.City))

                        EmployeeAddView ddd = new EmployeeAddView();

                        ddd.CertificateNumber = query.CertificateNumber;
                        ddd.City = query.City;
                        if (sdsd.Where(o => o.CertificateNumber == ddd.CertificateNumber && o.City == ddd.City).Count() <= 0)
                        {
                            sdsd.Add(ddd);


                            CityList.Add(query.City);
                            YearMonthList.Add(query.YearMonth);
                            rowNum++;
                            int colNum1 = 0;
                            IRow currentRow1 = sheet.CreateRow(rowNum);
                            ICell cell1 = currentRow1.CreateCell(colNum1);

                            cell = currentRow1.CreateCell(colNum1);
                            cell.SetCellValue(query.CompanyCode);
                            colNum1++;

                            cell = currentRow1.CreateCell(colNum1);
                            cell.SetCellValue(query.CompanyName);
                            colNum1++;

                            cell = currentRow1.CreateCell(colNum1);
                            cell.SetCellValue(query.Name);
                            colNum1++;

                            cell = currentRow1.CreateCell(colNum1);
                            cell.SetCellValue(query.CertificateNumber);
                            colNum1++;

                            cell = currentRow1.CreateCell(colNum1);
                            cell.SetCellValue(query.Station);
                            colNum1++;

                            cell = currentRow1.CreateCell(colNum1);
                            cell.SetCellValue(query.City);
                            colNum1++;
                            var ii = queryData.Where(a => a.CertificateNumber == query.CertificateNumber && a.City == query.City && a.YearMonth == query.YearMonth);
                            foreach (var aa in Enum.GetNames(typeof(Common.EmployeeAdd_InsuranceKindId)))
                            {
                                List<string> kindstype = new List<string>();
                                int state = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), aa);
                                foreach (var iie in ii)
                                {

                                    kindstype.Add(iie.InsuranceKinds);

                                }
                                if (kindstype.Contains(state.ToString()))
                                {
                                    foreach (var uu in ii)
                                    {

                                        if (state.ToString() == uu.InsuranceKinds)
                                        {
                                            //（养老）政策手续
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.PoliceOperationName_1);
                                            colNum1++;
                                            //（养老）政策
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.PoliceInsuranceName_1);
                                            colNum1++;
                                            //（养老）单位比例
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.CompanyPercent_1.ToString());
                                            colNum1++;
                                            //（养老）个人比例
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.EmployeePercent_1.ToString());
                                            colNum1++;
                                            //（养老）工资
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.Wage_1.ToString());
                                            colNum1++;
                                            //（养老）起缴时间
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(Convert.ToDateTime(uu.StartTime_1).ToString("yyyyMM"));
                                            colNum1++;
                                            //（养老）户口性质
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.PoliceAccountNatureName);
                                            colNum1++;
                                            //（养老）报增自然月
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.YearMonth_1.ToString());
                                            colNum1++;
                                            //（养老）社保月
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(Convert.ToDateTime(uu.InsuranceMonth_1).ToString("yyyyMM"));
                                            colNum1++;
                                            //（养老）是否单立户
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.IsIndependentAccount_1);
                                            colNum1++;
                                            //（养老）社保编号
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.InsuranceCode_1);
                                            colNum1++;
                                            //（养老）创建时间
                                            cell = currentRow1.CreateCell(colNum1);
                                            cell.SetCellValue(uu.CreateTime_1.ToString());
                                            colNum1++;
                                            ids = uu.AddIds + "," + ids;


                                        }
                                    }
                                }
                                else
                                {
                                    //（养老）政策手续
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）政策
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）单位比例
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）个人比例
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）工资
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）起缴时间
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）户口性质
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）报增自然月
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）社保月
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）是否单立户
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）社保编号
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    //（养老）创建时间
                                    cell = currentRow1.CreateCell(colNum1);
                                    cell.SetCellValue("");
                                    colNum1++;
                                    // ids = uu.AddIds + "," + ids;
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                    var results = 0;//返回的结果
                    int[] intArray;
                    string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                    using (var ent = new SysEntities())
                    {
                        string State = EmployeeAdd_State.员工客服已确认.ToString();
                        var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && a.State == State);
                        if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                        {
                            foreach (var item in updateEmpAdd)
                            {
                                item.State = EmployeeAdd_State.社保专员已提取.ToString();
                                item.UpdateTime = DateTime.Now;
                                item.UpdatePerson = LoginInfo.LoginName;

                            }
                            results = ent.SaveChanges();
                        }
                    }

                    string fileName = "社保专员导出_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
                    //string fileName = name + "供应商导出.xls";
                    string urlPath = "DataExport\\" + fileName; // 文件下载的URL地址，供给前台下载
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("\\") + urlPath; // 文件路径
                    file = new FileStream(filePath, FileMode.Create);
                    workbook.Write(file);
                    file.Close();
                    if (queryData.Count == 0)
                    {
                        var data = new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.FindNull,
                            Message = "没有符合条件的数据",
                            URL = urlPath
                        };
                        return data;
                    }
                    string Message = "已成功提取报增信息";

                    return new Common.ClientResult.UrlResult
                    {
                        Code = ClientCode.Succeed,
                        Message = Message,
                        URL = urlPath
                    };
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
        #region 社保专员修改员工信息
        public Common.ClientResult.Result POSTFeedbackModify(string Reportedincreasedata, int CompanyEmployeeRelationid)
        {
            returnData postinfos = new returnData();
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            postinfos = (returnData)jsonSerializer.Deserialize(Reportedincreasedata, typeof(returnData));
            StringBuilder sbError = new StringBuilder();
            string error = "";
            error = verificationBacknumber1(postinfos);
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
                string Enable = Status.启用.ToString();
                string Disable = Status.停用.ToString();
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var CompanyEmployeeRelationlist = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == CompanyEmployeeRelationid);
                        if (CompanyEmployeeRelationlist.PoliceAccountNatureId != postinfos.PoliceAccountNature)
                        {
                            CompanyEmployeeRelationlist.PoliceAccountNatureId = postinfos.PoliceAccountNature;
                        }
                        var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                        DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
                        foreach (var Insurance in postinfos.Insurance)
                        {
                            DateTime QJ_Time = DateTime.MinValue;
                            DateTime.TryParse(Insurance.StartTime, out QJ_Time);

                            #region 修改
                            if (QJ_Time != DateTime.MinValue)
                            {
                                var EmployeeAdd = SysEntitiesO2O.EmployeeAdd.FirstOrDefault(p => p.Id == Insurance.Id);
                                if (EmployeeAdd != null)
                                {
                                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), Insurance.InsuranceKind);
                                    if (EmployeeAdd.PoliceOperationId != Insurance.PoliceOperation || EmployeeAdd.PoliceInsuranceId != Insurance.PoliceInsurance || DateTime.Compare((DateTime)EmployeeAdd.StartTime, QJ_Time) != 0)
                                    {
                                        EmployeeAdd.StartTime = QJ_Time;
                                        EmployeeAdd.PoliceOperationId = Insurance.PoliceOperation;
                                        EmployeeAdd.PoliceInsuranceId = Insurance.PoliceInsurance;
                                        EmployeeAdd.UpdateTime = DateTime.Now;

                                        var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable && (o.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || o.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴));
                                        if (EmployeeMiddle.Count() > 0)
                                        {
                                            foreach (var order in EmployeeMiddle)
                                            {
                                                order.State = Disable; order.UpdateTime = DateTime.Now;


                                            }
                                        }
                                        decimal GZ = (decimal)EmployeeAdd.Wage;
                                        int ZC_ID = (int)Insurance.PoliceInsurance;
                                        var JISHU_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                        var JISHU_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                        var PERCENT_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                        var PERCENT_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                        var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(p => p.Id == ZC_ID);
                                        var CompanyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(p => p.Id == EmployeeAdd.CompanyEmployeeRelationId);
                                        #region 正常
                                        EmployeeMiddle employeeMiddle = new EmployeeMiddle();
                                        employeeMiddle.InsuranceKindId = InsuranceKindId;
                                        employeeMiddle.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                                        employeeMiddle.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                                        employeeMiddle.CompanyBasePayment = JISHU_C;
                                        employeeMiddle.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_C, PERCENT_C, 1, ZC_ID);
                                        //Business.Get_TwoXiaoshu(JISHU_C * PERCENT_C);
                                        employeeMiddle.EmployeeBasePayment = JISHU_P;
                                        employeeMiddle.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_P, PERCENT_P, 1, ZC_ID);
                                        //Business.Get_TwoXiaoshu(JISHU_P * PERCENT_P);
                                        employeeMiddle.PaymentMonth = 1; //正常生成一个月的费用
                                        employeeMiddle.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                        employeeMiddle.EndedDate = Convert.ToInt32(DateTime.MaxValue.ToString("yyyyMM"));
                                        employeeMiddle.State = Enable;//正常
                                        employeeMiddle.UseBetween = 0;
                                        employeeMiddle.CityId = CompanyEmployeeRelation.CityId;
                                        employeeMiddle.CreateTime = DateTime.Now;
                                        EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle);
                                        #endregion
                                        #region 补缴
                                        if (PoliceInsurance.MaxPayMonth != 0 && Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd)) > 0)
                                        {
                                            Int32 Months = Business.CHA_Months(QJ_Time, _NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd));
                                            var JISHU_BJ_C = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 1);
                                            var JISHU_BJ_P = EmployeeAddRepository.Get_Jishu(SysEntitiesO2O, ZC_ID, GZ, 2);
                                            var PERCENT_BJ_C = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 1);
                                            var PERCENT_BJ_P = EmployeeAddRepository.Get_BILI(SysEntitiesO2O, ZC_ID, GZ, 2);
                                            EmployeeMiddle employeeMiddle_BJ = new EmployeeMiddle();
                                            employeeMiddle_BJ.InsuranceKindId = InsuranceKindId;
                                            employeeMiddle_BJ.CompanyEmployeeRelationId = EmployeeAdd.CompanyEmployeeRelationId;
                                            employeeMiddle_BJ.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;
                                            employeeMiddle_BJ.CompanyBasePayment = JISHU_BJ_C;
                                            employeeMiddle_BJ.CompanyPayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_C, PERCENT_BJ_C, Months, ZC_ID);
                                            //Business.Get_TwoXiaoshu(JISHU_BJ_C * PERCENT_BJ_C) * Months; ;
                                            employeeMiddle_BJ.EmployeeBasePayment = JISHU_BJ_P;
                                            employeeMiddle_BJ.EmployeePayment = EmployeeAddRepository.Get_CompanyPayment(SysEntitiesO2O, JISHU_BJ_P, PERCENT_BJ_P, Months, ZC_ID);
                                            //Business.Get_TwoXiaoshu(JISHU_BJ_P * PERCENT_BJ_P) * Months; ;
                                            employeeMiddle_BJ.PaymentMonth = Months;
                                            employeeMiddle_BJ.PaymentBetween = QJ_Time.ToString("yyyyMM") + "-" + (_NowDate.AddMonths((int)PoliceInsurance.InsuranceAdd).AddMonths(-1).ToString("yyyyMM"));
                                            employeeMiddle_BJ.UseBetween = 0;
                                            employeeMiddle_BJ.StartDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                            employeeMiddle_BJ.EndedDate = Convert.ToInt32(_NowDate.ToString("yyyyMM"));
                                            employeeMiddle_BJ.State = Enable;//正常
                                            employeeMiddle_BJ.CityId = CompanyEmployeeRelation.CityId;
                                            employeeMiddle_BJ.CreateTime = DateTime.Now;
                                            EmployeeMiddle_BLL.CreateEmployee(SysEntitiesO2O, employeeMiddle_BJ);
                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                        }
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
                    result.Message = "修改失败";
                    return result;
                }

            }
        }
        #endregion
        #region 社保专员/供应商专员报增反馈 成功 动作
        /// <summary>
        /// 供应商专员报增反馈 成功
        /// </summary>
        /// <param name="query">成功人员的id集合</param>        
        /// <returns></returns>
        public string FeedbackIndexPass1(string query, string CompanyEmployeeRelationId, string alltype)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    alltype = HttpUtility.HtmlDecode(alltype);
                    string result = "";
                    string prompt = "";
                    int[] intArray;
                    string[] strArray = query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                    int?[] intArrayall1 = new int?[10];
                    List<int?> InsuranceKindTypes = new List<int?>();
                    string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var a in strArrayall)
                    {
                        int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                        InsuranceKindTypes.Add(InsuranceKindId);
                    }

                    int[] fuwuintArray;
                    string[] fuwuArray = CompanyEmployeeRelationId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    fuwuintArray = Array.ConvertAll<string, int>(fuwuArray, s => int.Parse(s));
                    using (var ent = new SysEntities())
                    {

                        var empadd = ent.EmployeeAdd.Where(a => true);
                        var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));
                        if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                        {
                            foreach (var item in updateEmpAdd)
                            {
                                string statesuccess = EmployeeAdd_State.申报成功.ToString();//申报成功 && o.State == statesuccess
                                var EmployeeAdd = ent.EmployeeAdd.FirstOrDefault(o => o.Id == item.Id);
                                var CompanyEmployeeRelation = ent.CompanyEmployeeRelation.FirstOrDefault(o => o.Id == EmployeeAdd.CompanyEmployeeRelationId);
                                var CompanyEmployeeRelationList = ent.CompanyEmployeeRelation.FirstOrDefault(o => o.EmployeeId == CompanyEmployeeRelation.EmployeeId && o.CompanyId == CompanyEmployeeRelation.CompanyId && o.Id == EmployeeAdd.CompanyEmployeeRelationId && o.State == "在职");

                                var EmployeeAdd_Seccess = ent.EmployeeAdd.FirstOrDefault(o => o.CompanyEmployeeRelationId == CompanyEmployeeRelationList.Id && o.State == statesuccess && o.InsuranceKindId == EmployeeAdd.InsuranceKindId);

                                if (EmployeeAdd_Seccess != null)
                                {
                                    var KindName = Enum.GetName(typeof(Common.EmployeeAdd_InsuranceKindId), EmployeeAdd.InsuranceKindId);
                                    var Employee = ent.Employee.FirstOrDefault(o => o.Id == CompanyEmployeeRelationList.EmployeeId);
                                    result = result + Employee.Name + "在此公司下以存在" + KindName + "的报增成功记录";
                                }
                                else
                                {
                                    item.State = EmployeeAdd_State.申报成功.ToString();
                                    item.UpdateTime = DateTime.Now;
                                    item.UpdatePerson = LoginInfo.LoginName;
                                    ent.SaveChanges();
                                    #region 添加服务费
                                    #region 得出服务费
                                    //int? CompanyEmployeeRelationid = item1.Id;
                                    //var aa = SysEntitiesO2O.CompanyEmployeeRelation.FirstOrDefault(ce => ce.Id == CompanyEmployeeRelationid);
                                    decimal payService_One = 0;
                                    decimal payService_OneBJ = 0;
                                    int count = 0;
                                    CRM_CompanyRepository api1 = new CRM_CompanyRepository();
                                    count = api1.getEmployee(SysEntitiesO2O, item.CompanyEmployeeRelation.CompanyId, item.YearMonth);
                                    //先找出企业交社保的人数
                                    //var companyEmployeeRelation = SysEntitiesO2O.CompanyEmployeeRelation.Where(ce => ce.State == "在职" && ce.CompanyId == aa.CompanyId);
                                    //if (companyEmployeeRelation.Count() > 0)
                                    //{
                                    //    count = companyEmployeeRelation.Count();
                                    //}

                                    int[] cclpStatus = new int[]{
                       (int)Common.Status.启用, (int)Common.Status.修改中 
                        };
                                    //找出单人服务费
                                    var cRM_CompanyLadderPrice = SysEntitiesO2O.CRM_CompanyLadderPrice.Where(cclp => cclp.CRM_Company_ID == item.CompanyEmployeeRelation.CompanyId && cclpStatus.Contains(cclp.Status) && cclp.BeginLadder <= count && cclp.EndLadder >= count);
                                    if (cRM_CompanyLadderPrice.Count() > 0)
                                    {
                                        payService_One = cRM_CompanyLadderPrice.OrderByDescending(o => o.EndLadder).FirstOrDefault().SinglePrice;

                                    }
                                    decimal payService_All = 0;
                                    bool zhengHu = false;
                                    //找出整户服务费
                                    var cRM_CompanyPrice = SysEntitiesO2O.CRM_CompanyPrice.Where(ccp => (new[] { (int)Common.Status.启用, (int)Common.Status.修改中 }).Contains(ccp.Status) && ccp.CRM_Company_ID == item.CompanyEmployeeRelation.CompanyId);
                                    if (cRM_CompanyPrice.Count() > 0)
                                    {
                                        payService_All = cRM_CompanyPrice.FirstOrDefault().LowestPrice.Value;
                                        payService_OneBJ = cRM_CompanyPrice.FirstOrDefault().AddPrice.Value;
                                    }
                                    //判断取单人服务费还是整户服务费
                                    if (payService_One * count > payService_All)
                                    {
                                        zhengHu = false;
                                        payService_All = 0;
                                    }
                                    else
                                    {
                                        zhengHu = true;
                                        payService_One = 0;
                                    }
                                    #endregion
                                    #region 得出需要修改的人和公司全部人
                                    string[] ZENG_STATUS = new string[]{
                        Common.EmployeeAdd_State.待责任客服确认.ToString(),   Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                        Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString(),  Common.EmployeeAdd_State.社保专员已提取.ToString() ,
                         Common.EmployeeAdd_State.申报成功.ToString() 
                        };
                                    //var CompanyEmployeeRelationlist = from a in SysEntitiesO2O.EmployeeMiddle.Where(em => em.StartDate <= item.YearMonth && em.EndedDate >= item.YearMonth && ZENG_STATUS.Contains(em.State))
                                    //                                  join b in SysEntitiesO2O.CompanyEmployeeRelation.Where(x => x.CompanyId == item.CompanyEmployeeRelation.CompanyId) on a.CompanyEmployeeRelationId equals b.Id
                                    //                                  where a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴
                                    //                                  group new { b } by new
                                    //                                  {
                                    //                                      Employee_ID = b.EmployeeId,
                                    //                                      CompanyId = (int)b.CompanyId,
                                    //                                      YearMonth = a.UseBetween,
                                    //                                  }
                                    //                                      into s

                                    //                                      select new
                                    //                                      {
                                    //                                          Employee_ID = s.Key.Employee_ID,
                                    //                                          YearMonth = s.Key.YearMonth,
                                    //                                          CompanyId = s.Key.Employee_ID,

                                    //  
                                    //  };

                                    int[] emploees = api1.getEmployeeIDs(SysEntitiesO2O, item.CompanyEmployeeRelation.CompanyId, item.YearMonth);
                                    var CompanyEmployeeRelationlist = (from ce in SysEntitiesO2O.CompanyEmployeeRelation.Where(ce => ce.State == "在职")
                                                                       join e in SysEntitiesO2O.EmployeeAdd on ce.Id equals e.CompanyEmployeeRelationId
                                                                       where ce.CompanyId == item.CompanyEmployeeRelation.CompanyId && ZENG_STATUS.Contains(e.State) && emploees.Contains(ce.EmployeeId ?? 0)
                                                                       select new
                                                                       {
                                                                           CompanyId = (int)ce.CompanyId,
                                                                           Employee_ID = ce.EmployeeId,
                                                                           YearMonth = e.YearMonth
                                                                       }).Distinct().ToList();
                                    #endregion
                                    #endregion
                                    #region 修改费用服务费表中的正常服务费
                                    int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                                    int[] FEIYONG_STATUS = new int[] { (int)Common.COST_Table_Status.财务作废, (int)Common.COST_Table_Status.客户作废, (int)Common.COST_Table_Status.责任客服作废 };
                                    var COST_CostTable = ent.COST_CostTable.Where(a => true);
                                    List<int> dss = new List<int>();

                                    foreach (var item3 in CompanyEmployeeRelationlist)
                                    {
                                        dss.Add((int)item3.Employee_ID);
                                    }
                                    foreach (var item3 in CompanyEmployeeRelationlist)
                                    {
                                        var list1 = (from a in COST_CostTable
                                                     join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                     where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID == item.CompanyEmployeeRelation.EmployeeId
                                                     select b).FirstOrDefault();//费用表
                                        if (list1 != null)
                                        {
                                            if (zhengHu == true)//整户服务费
                                            {
                                                list1.ServiceCoset = payService_All;

                                                var list2 = from a in COST_CostTable
                                                            join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                            where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID != item.CompanyEmployeeRelation.EmployeeId && dss.Contains((int)b.Employee_ID)
                                                            select b;//
                                                if (list2.Count() > 0)
                                                {
                                                    foreach (var item4 in list2)
                                                    {
                                                        item4.ServiceCoset = 0;
                                                    }
                                                }
                                            }
                                            else//单人服务费
                                            {
                                                list1.ServiceCoset = payService_One;
                                                var list2 = from a in COST_CostTable
                                                            join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                            where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID != item.CompanyEmployeeRelation.EmployeeId && dss.Contains((int)b.Employee_ID)
                                                            select b;//
                                                if (list2.Count() > 0)
                                                {
                                                    foreach (var item4 in list2)
                                                    {


                                                        item4.ServiceCoset = payService_One;


                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region  修改补缴服务费
                                    //取中间表的补缴数据
                                    string Middlestate = Common.Status.启用.ToString();
                                    var CompanyEmployeeRelationlistMiddle =
                                        (from em in ent.EmployeeMiddle.Where(em => em.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴)
                                         join cer in ent.CompanyEmployeeRelation on em.CompanyEmployeeRelationId equals cer.Id
                                         join e in ent.Employee on cer.EmployeeId equals e.Id
                                         where cer.CompanyId == item.CompanyEmployeeRelation.CompanyId && em.State == Middlestate
                                         select new
                                         {
                                             StartDate = em.StartDate,
                                             Employee_ID = e.Id,
                                             CompanyId = cer.CompanyId,
                                             PaymentMonth = em.PaymentMonth,

                                         }).GroupBy(o => new { o.Employee_ID, o.CompanyId, o.PaymentMonth, o.StartDate }).Select(o => new
                                         {
                                             StartDate = o.Key.StartDate,
                                             ServiceCoset = (decimal)o.Max(m => m.PaymentMonth) * payService_OneBJ,
                                             Employee_ID = o.Key.Employee_ID,
                                             CompanyId = item.CompanyEmployeeRelation.CompanyId,
                                             PaymentMonth = o.Key.PaymentMonth,
                                         }).ToList();
                                    #region 修改费用服务费表中的补缴服务费
                                    int bujiaoPaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;


                                    foreach (var item3 in CompanyEmployeeRelationlistMiddle)
                                    {
                                        var listooo = (from a in COST_CostTable
                                                       join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                       where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == bujiaoPaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.StartDate && b.Employee_ID == item3.Employee_ID
                                                       select b).FirstOrDefault();//费用表
                                        if (listooo != null)
                                        {
                                            listooo.ServiceCoset = item3.ServiceCoset;

                                        }
                                    }
                                    #endregion
                                    #endregion
                                    ent.SaveChanges();
                                }
                            }
                            scope.Complete();
                            prompt = "操作成功";


                        }
                        return prompt;
                    }
                }

            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        #endregion
        #region 社保专员/供应商专员报增反馈 失败 动作
        /// <summary>
        /// 供应商专员报增反馈 失败
        /// </summary>
        /// <param name="ids">成功人员的id集合</param>        
        /// <returns></returns>
        public string FeedBackAction1(string ids, string message, string alltype, string CompanyEmployeeRelationId)
        {
            try
            {
                message = HttpUtility.HtmlDecode(message);
                string enable = Status.启用.ToString();
                alltype = HttpUtility.HtmlDecode(alltype);


                string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // intArrayall = Array.ConvertAll<string, int>(strArrayall, s => int.Parse(s));
                List<int?> InsuranceKindTypes = new List<int?>();

                foreach (var a in strArrayall)
                {
                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                    InsuranceKindTypes.Add(InsuranceKindId);
                }

                string result = "";//返回的结果
                // int companempID = int.Parse(ids);
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                using (TransactionScope scope = new TransactionScope())
                {
                    using (var ent = new SysEntities())
                    {
                        var empadd = ent.EmployeeAdd.Where(a => true);
                        var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));
                        if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                        {
                            foreach (var item in updateEmpAdd)
                            {
                                result = eadd_BLL.ChangeServicecharge(item, message, LoginInfo.LoginName);
                                //ent.SaveChanges();

                            }
                            scope.Complete();
                        }
                        if (result != "")
                        {
                            return result;
                        }
                        else
                        {

                            return result = "操作成功";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        #endregion
        #region 获取登录人所有下属
        /// <summary>
        /// 获取登录人所有下属
        /// </summary>
        /// <returns></returns>

        public Common.ClientResult.DataResult Subordinates()
        {
            string menuID = "1012";//员工客服经理分配员工客服
            List<ORG_User> queryData = getSubordinatesData(Common.ORG_Group_Code.YGKF.ToString(), menuID);      //	报增信息查询,menuID);

            var data = new Common.ClientResult.DataResult
            {
                rows = queryData
            };
            return data;
        }

        public List<ORG_User> getSubordinatesData(string code, string menuID)
        {
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion
            List<ORG_User> queryData = userBLL.GetGroupUsers(code, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            return queryData;
        }
        #endregion


        #region 根据缴纳地初始化户口性质 敬
        /// <summary>
        /// 根据缴纳地初始化户口性质
        /// </summary>
        /// <param name="ID">城市id</param>
        /// <param name="EmployeeId">个人id</param>
        /// <returns></returns>
        public ActionResult getCompanyEmployeeRelationList(string ID, int CompanyId, int EmployeeId)
        {

            var q = from c in SysEntitiesO2O.CompanyEmployeeRelation
                    where c.CityId == ID && c.CompanyId == CompanyId && c.EmployeeId == EmployeeId && c.State == "在职"
                    select new idname__ { ID = (int)c.PoliceAccountNatureId, Name = c.Station };
            List<idname__> list = q.ToList();

            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

    }



}


