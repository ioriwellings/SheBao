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
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Langben.DAL.Model;

namespace Langben.App.Controllers
{
    /// <summary>
    /// 员工调基
    /// </summary>
    public class EmployeeGoonPayment2ApiController : BaseApiController
    {
        SysEntities db = new SysEntities();
        JsonResult jr = new JsonResult();
        IBLL.IEmployeeAddBLL eadd_BLL = new BLL.EmployeeAddBLL();
        private string menuID = "";
        private string approveMenuID = "";
        private string supplierMenuID = "";
        /// <summary>
        /// 责任客服补缴列表(调基列表)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostEmployeeList([FromBody]GetDataParam getParam)
        {
            //var EmployeeAddBLL = new BLL.EmployeeAddBLL();
            IBLL.IEmployeeGoonPayment2BLL Bll = new BLL.EmployeeGoonPayment2BLL();

            int total = 0;
            string search = getParam.search;

            //if (string.IsNullOrWhiteSpace(search))
            //{
            //    search = "State&" + Common.EmployeeAdd_State.申报成功.ToString() + "^";
            //}
            //else
            //{
            //    search += "State&" + Common.EmployeeAdd_State.申报成功.ToString() + "^";
            //}

            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            List<EmployeeGoonPayment2View> queryData = Bll.GetEmployeeList(getParam.page, getParam.rows, search, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        /// <summary>
        /// 员工客服确认调基数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostApproveList([FromBody]GetDataParam getParam)
        {
            //var EmployeeAddBLL = new BLL.EmployeeAddBLL();
            IBLL.IEmployeeGoonPayment2BLL Bll = new BLL.EmployeeGoonPayment2BLL();

            int total = 0;
            string search = getParam.search;

            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(approveMenuID);
            string departments = "";

            if (departmentScope == 0)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(approveMenuID);
            }
            #endregion

            List<EmployeeGoonPayment2View> queryData = Bll.GetApproveList(getParam.page, getParam.rows, search, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        #region 员工客服审核通过

        /// <summary>
        /// 员工客服审核通过
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result EmployeePass(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] ApprovedId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (ApprovedId != null && ApprovedId.Length > 0)
            {
                if (m_BLL.EmployeeGoonPayment2Approved(ApprovedId, Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString(), LoginInfo.UserName))
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

        #region 员工客服审核终止

        /// <summary>
        /// 员工客服审核终止
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result EmployeeEnd(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] ApprovedId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (ApprovedId != null && ApprovedId.Length > 0)
            {
                if (m_BLL.EmployeeGoonPayment2End(ApprovedId, LoginInfo.UserName))
                {
                    LogClassModels.WriteServiceLog("审核终止成功" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
                        );//审核通过成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "终止成功";
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
                    LogClassModels.WriteServiceLog("审核终止失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                        );//审核通过失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "终止失败" + returnValue;
                    return result;
                }
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public EmployeeGoonPayment2View Get(int id)
        {
            EmployeeGoonPayment2View item = m_BLL.GetChangeWageEmployeeInfo(id);
            if (item==null)
            {
                Redirect("");
            }
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            string error = m_BLL.Verification(entity, yanglaoID, yiliaoID, gongshangID, shiyeID, shengyuID, gongjijinID);

            if (!string.IsNullOrEmpty(error))//验证可调基的险种不通过
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = error; 
            }
            else
            {
                string loginName = LoginInfo.UserName;//登录人
                int success = m_BLL.ChangeWage(entity, yanglaoID, yiliaoID, gongshangID, shiyeID, shengyuID, gongjijinID, loginName);
                if (success == 1)//操作成功
                {
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed;//提示调基成功 
                }
                else
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail; //提示调基失败
                }
            }
           
            return result;
        }

        // PUT api/<controller>/5
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]EmployeeGoonPayment2 entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，员工调基信息的Id为" + entity.Id, "员工调基"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，员工调基信息的Id为" + entity.Id + "," + returnValue, "员工调基"
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

        #region 缴纳地初始绑定
        /// <summary>
        /// 缴纳地初始绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult getCitylist()
        {
            List<idname__> list = eadd_BLL.getCitylist();
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 企业初始绑定
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

        #region 缴纳地户口性质
        public ActionResult getPoliceAccountNatureList(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                List<idname__> list = eadd_BLL.getPoliceAccountNatureid(ID);
                jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
                return jr;
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 员工客服确认详细页面
        /// <summary>
        /// 员工客服确认详细页面
        /// </summary>
        /// <param name="CompanyEmployeeRelationId">员工关系id</param>
        /// <param name="type">保险类别</param>
        /// <returns></returns>
        public IHttpActionResult getEmployeeAddList(int CompanyEmployeeRelationId, int type, int? Parameter)
        {
            if (type == 1)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.养老;

                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                var data = (db.EmployeeGoonPayment2.Select(c => new
                {
                    yanglao_id = c.Id,
                    CompanyEmployeeRelationId=c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    OldWage = c.OldWage,
                    NewWage=c.NewWage

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State).FirstOrDefault());

                return Json(data);
            }
            else if (type == 2)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.医疗;
                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                var data = (db.EmployeeGoonPayment2.Select(c => new
                {
                    yiliao_id = c.Id,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    OldWage = c.OldWage,
                    NewWage = c.NewWage

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State).FirstOrDefault());

                return Json(data);
            }
            else if (type == 3)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.工伤;
                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                var data = (db.EmployeeGoonPayment2.Select(c => new
                {
                    gongshang_id = c.Id,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    OldWage = c.OldWage,
                    NewWage = c.NewWage

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State).FirstOrDefault());

                return Json(data);
            }
            else if (type == 4)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.失业;
                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                var data = (db.EmployeeGoonPayment2.Select(c => new
                {
                    shiye_id = c.Id,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    OldWage = c.OldWage,
                    NewWage = c.NewWage

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State).FirstOrDefault());

                return Json(data);
            }
            else if (type == 5)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.公积金;
                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                var data = (db.EmployeeGoonPayment2.Select(c => new
                {
                    gongjijin_id = c.Id,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    OldWage = c.OldWage,
                    NewWage = c.NewWage

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State).FirstOrDefault());

                return Json(data);
            }
            else if (type == 6)
            {
                int InsuranceKindId = (int)Common.EmployeeAdd_InsuranceKindId.生育;
                string State = Common.EmployeeAdd_State.待员工客服确认.ToString();
                if (Parameter == 1)
                {
                    State = Common.EmployeeAdd_State.员工客服已确认.ToString();
                }
                else if (Parameter == 2)
                {
                    State = Common.EmployeeAdd_State.社保专员已提取.ToString();
                }
                var data = (db.EmployeeGoonPayment2.Select(c => new
                {
                    shengyu_id = c.Id,
                    CompanyEmployeeRelationId = c.EmployeeAdd.CompanyEmployeeRelationId,
                    InsuranceKindId = c.InsuranceKindId,
                    State = c.State,
                    OldWage = c.OldWage,
                    NewWage = c.NewWage

                }).Where(c => c.CompanyEmployeeRelationId == CompanyEmployeeRelationId && c.InsuranceKindId == InsuranceKindId && c.State == State).FirstOrDefault());

                return Json(data);
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 回退动作
        /// <summary>
        /// 社保专员报增信息
        /// </summary>
        /// <param name="ids">回退人员的id集合</param>
        /// <returns></returns>
        public Common.ClientResult.Result EmployeeFallback(string ids)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] ApprovedId = Array.ConvertAll<string, int?>(ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (ApprovedId != null && ApprovedId.Length > 0)
            {
                if (m_BLL.EmployeeGoonPayment2Approved(ApprovedId, Common.EmployeeGoonPayment2_STATUS.待员工客服确认.ToString(), LoginInfo.UserName))
                {
                    LogClassModels.WriteServiceLog("退回成功" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
                        );//审核通过成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "退回成功";
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
                    LogClassModels.WriteServiceLog("退回失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                        );//审核通过失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "退回失败" + returnValue;
                    return result;
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 社保专员提取信息列表
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
                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        if (str.Equals(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else
                        {
                            string state = Common.EmployeeGoonPayment2_STATUS.社保专员已提取.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                    else
                    {
                        string state = Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString();
                        search = "State&" + state + "^";
                    }
                }
                else
                {
                    string state = Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString();
                    search = "State&" + state + "^";
                }
            }
            List<EmployeeGoonPayment2View> queryData = m_BLL.GetCommissionerListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        #region 社保专员导出调基信息列表

        /// <summary>
        /// 社保专员导出调基信息列表
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

                int total = 0;

                Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        if (str.Equals(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else if (str.Equals(Common.CollectState.已提取.ToString()))
                        {
                            string state = Common.EmployeeGoonPayment2_STATUS.社保专员已提取.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
                else
                {
                    string state = Common.EmployeeGoonPayment2_STATUS.员工客服已确认.ToString();
                    search = "State&" + state + "^";
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    #region 生成Excel
                    workbook.SetSheetName(0, "社保专员导出调基");
                    List<EmployeeAddView> queryData = m_BLL.GetEmployeeAddExcelList(1, int.MaxValue, search, ref total);
                    string ids = string.Empty;

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

                    #region 养老
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
                    cell.SetCellValue("（养老）起缴时间");
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
                    #endregion

                    #region 医疗
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
                    cell.SetCellValue("（医疗）起缴时间");
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
                    #endregion

                    #region 工伤
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
                    cell.SetCellValue("（工伤）起缴时间");
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
                    #endregion

                    #region 失业
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
                    cell.SetCellValue("（失业）起缴时间");
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
                    #endregion

                    #region 公积金
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
                    cell.SetCellValue("（公积金）起缴时间");
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
                    #endregion

                    #region 生育
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
                    cell.SetCellValue("（生育）起缴时间");
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
                    #endregion

                    #region 大病
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
                    cell.SetCellValue("（大病）起缴时间");
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
                    #endregion

                    #region 补充公积金
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
                    cell.SetCellValue("（补充公积金）起缴时间");
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
                    #endregion

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
                        //（养老）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_1.ToString());
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
                        //（医疗）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_2.ToString());
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
                        //（工伤）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_3.ToString());
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
                        //（失业）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_4.ToString());
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
                        //（公积金）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_5.ToString());
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
                        //（生育）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_6.ToString());
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
                        //（大病）单位比例
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
                        //（大病）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_7.ToString());
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
                        //（补充公积金）起缴时间
                        cell = currentRow1.CreateCell(colNum1);
                        cell.SetCellValue(queryData[i].StartTime_8.ToString());
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
                    }
                    #endregion

                    var results = 0;//返回的结果
                    int[] intArray;
                    string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                    using (var ent = new SysEntities())
                    {
                        string State = EmployeeGoonPayment2_STATUS.员工客服已确认.ToString();
                        var updateEmp = ent.EmployeeGoonPayment2.Where(a => intArray.Contains(a.Id) && a.State == State);
                        if (updateEmp != null && updateEmp.Count() >= 1)
                        {
                            foreach (var item in updateEmp)
                            {
                                item.State = EmployeeGoonPayment2_STATUS.社保专员已提取.ToString();
                                item.UpdateTime = DateTime.Now;
                                item.UpdatePerson = LoginInfo.LoginName;
                            }
                            results = ent.SaveChanges();
                        }
                    }
                    //excelName
                    string fileName = "社保专员导出调基_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
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
                    string Message = "已成功提取调基信息";
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

        #region Private
    
        #endregion

        IBLL.IEmployeeGoonPayment2BLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeGoonPayment2ApiController()
            : this(new EmployeeGoonPayment2BLL()) { }

        public EmployeeGoonPayment2ApiController(EmployeeGoonPayment2BLL bll)
        {
            m_BLL = bll;
        }
        #region 社保报增查询列表
       
        /// <summary>
        /// 社保报增查询列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostEmployeeGoonPayment2ViewList([FromBody]GetDataParam getParam)
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
            List<EmployeeAddView> queryData = m_BLL.GetEmployeeGoonPayment2List(getParam.page, getParam.rows, search, ref total);
            data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion
    }
}


