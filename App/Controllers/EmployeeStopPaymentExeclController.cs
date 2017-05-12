using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Common;
using Langben.BLL;
using Langben.DAL;
using Langben.IBLL;
using Models;
using NPOI.HPSF;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentExeclController : BaseController
    {
        // GET: EmployeeStopPaymentExecl
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InputEmployeeStopPaymentByExcel(HttpPostedFileBase files)
        {
            //int companyID = Convert.ToInt32(Request.QueryString["CompanyID"]);
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            try
            {
                #region 文件上传

                HttpPostedFileBase file = files;
                string uploadFilePath = AppDomain.CurrentDomain.BaseDirectory + "DataExport/";
                string returnMsg;
                string savePath = Util_XLS.UpLoadXls(file, uploadFilePath, out returnMsg);

                #endregion

                #region 读取Execl数据，验证，插入数据库

                if (string.IsNullOrEmpty(returnMsg))
                {
                    //DataSet myDataSet = Util_XLS.SelectFromXLS(savePath, "select * from [Sheet1$]", out returnMsg);
                    DataTable table = Util_XLS.NpoiReadExcle(savePath, "Sheet1", true, out returnMsg);
                    if (string.IsNullOrEmpty(returnMsg))
                    {
                        //DataTable table = myDataSet.Tables["SelectResult"].DefaultView.ToTable();
                        string message = string.Empty;
                        if (ImportEmployeeAdd(table, ref message))
                        {

                            result.Code = Common.ClientCode.Succeed;
                            result.Message = "导入成功";
                        }
                        else
                        {
                            result.Code = Common.ClientCode.Fail;
                            result.Message = message;

                        }
                    }
                    else
                    {
                        result.Code = Common.ClientCode.Fail;
                        result.Message = returnMsg;
                    }
                }
                else
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = returnMsg;
                }

                #endregion
            }
            catch (Exception ex)
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = ex.Message + "导入失败,请认真检查excel";

            }
            return Json(result);
        }

        #region 导入验证
        private string verification(DataTable table, out string resultMsg)
        {
            resultMsg = string.Empty;

            return "";
        }
        #endregion

        #region  数据库导入

        /// <summary>
        /// 数据库导入
        /// </summary>
        /// <param name="table">DataTable数据</param>
        /// <param name="cId">页面所选企业编号</param>
        /// <param name="message"></param>
        private bool ImportEmployeeAdd(DataTable table, ref string message)
        {
            bool result = false;

            IEmployeeBLL eadd_BLL = new EmployeeBLL();
            IEmployeeStopPaymentBLL stopBll = new EmployeeStopPaymentBLL();
            List<EmployeeStopPaymentSingle> lstStopPayments = new List<EmployeeStopPaymentSingle>();

            StringBuilder sbError = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    string rowId = row["NO."].ToString();
                    string cName = row["COMPANY_NAME"].ToString();
                    string eName = row["EMPLOYEE_NAME"].ToString();
                    string cardId = row["CARD_ID"].ToString();
                    string yanglaoType = row["YANGLAO_TYPE"].ToString();
                    string shiyeType = row["SHIYE_TYPE"].ToString();
                    string gongshangType = row["GONGSHANG_TYPE"].ToString();
                    string yiliaoType = row["YILIAO_TYPE"].ToString();
                    string shengyuType = row["SHENGYU_TYPE"].ToString();
                    string gongjijinType = row["GONGJIJIN_TYPE"].ToString();
                    string daeType = row["DAE_TYPE"].ToString();
                    string buchonggongjijinType = row["BUCHONG_GONGJIJIN_TYPE"].ToString();
                    StringBuilder errMsg = new StringBuilder();

                    #region 验证

                    int companyId = stopBll.GetCompanyIdByNameAndUserZRId(cName, this.LoginInfo.UserID);
                    if (companyId == 0)
                    {
                        errMsg.Append(string.Format("序号[{0}]：企业名称[{1}]系统中不存在.", rowId, cName));
                    }
                    else if (companyId == -1)
                    {
                        errMsg.Append(string.Format("序号[{0}]：企业名称[{1}]不是你负责的企业.", rowId, cName));
                    }

                    //string companyName = stopBll.GetCompanyNameById(companyId);
                    //if (cName != companyName)
                    //{
                    //    errMsg.Append(string.Format("序号[{0}]：企业名称[{1}]和选择的企业不一致.", rowId, cName));

                    //}

                    int employeeId = stopBll.GetEmployeeIdByNameAndCardId(eName, cardId);
                    if (employeeId == 0)
                    {
                        errMsg.AppendLine(string.Format("序号[{0}]：没找到姓名[{1}]身份证号[{2}]的员工.", rowId, eName, cardId));
                    }

                    int relationId = stopBll.GetCompanyEmployeeRelationId(companyId, employeeId);
                    if (relationId == 0)
                    {
                        errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]不在所选企业内.", rowId, eName));
                    }

                    int yanglaoAddId = 0, yanglaoOperationId = 0;
                    if (!string.IsNullOrEmpty(yanglaoType))
                    {
                        yanglaoAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.养老, relationId);
                        if (yanglaoAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的养老保险.", rowId, eName));
                        }
                        else
                        {
                            yanglaoOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(yanglaoAddId, yanglaoType);
                            if (yanglaoOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：养老报减方式[{1}]不存在.", rowId, yanglaoType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(yanglaoAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的养老记录.", rowId));
                            }
                        }

                    }

                    int shiyeAddId = 0, shiyeOperationId = 0;
                    if (!string.IsNullOrEmpty(shiyeType))
                    {
                        shiyeAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.失业, relationId);
                        if (shiyeAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的失业保险.", rowId, eName));
                        }
                        else
                        {
                            shiyeOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(shiyeAddId, shiyeType);
                            if (shiyeOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：失业报减方式[{1}]不存在.", rowId, shiyeType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(shiyeAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的失业记录.", rowId));
                            }
                        }

                    }

                    int gongshangAddId = 0, gongshangOperationId = 0;
                    if (!string.IsNullOrEmpty(gongshangType))
                    {
                        gongshangAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.工伤, relationId);
                        if (gongshangAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的工伤保险.", rowId, eName));
                        }
                        else
                        {
                            gongshangOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(gongshangAddId, gongshangType);
                            if (gongshangOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：工伤报减方式[{1}]不存在.", rowId, gongshangType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(gongshangAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的工伤记录.", rowId));
                            }
                        }

                    }

                    int yiliaoAddId = 0, yiliaoOperationId = 0;
                    if (!string.IsNullOrEmpty(yiliaoType))
                    {
                        yiliaoAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.医疗, relationId);
                        if (yiliaoAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的医疗保险.", rowId, eName));
                        }
                        else
                        {
                            yiliaoOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(yiliaoAddId, yiliaoType);
                            if (yiliaoOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：医疗报减方式[{1}]不存在.", rowId, yiliaoType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(yiliaoAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的医疗记录.", rowId));
                            }
                        }

                    }

                    int shengyuAddId = 0, shengyuOperationId = 0;
                    if (!string.IsNullOrEmpty(shengyuType))
                    {
                        shengyuAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.生育, relationId);
                        if (shengyuAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的生育保险.", rowId, eName));
                        }
                        else
                        {
                            shengyuOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(shengyuAddId, shengyuType);
                            if (shengyuOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：生育报减方式[{1}]不存在.", rowId, shengyuType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(shengyuAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的生育记录.", rowId));
                            }
                        }

                    }

                    int gongjijinAddId = 0, gongjijinOperationId = 0;
                    if (!string.IsNullOrEmpty(gongjijinType))
                    {
                        gongjijinAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.公积金, relationId);
                        if (gongjijinAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的公积金.", rowId, eName));
                        }
                        else
                        {
                            gongjijinOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(gongjijinAddId, gongjijinType);
                            if (gongjijinOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：公积金报减方式[{1}]不存在.", rowId, gongjijinType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(gongjijinAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的公积金记录.", rowId));
                            }
                        }

                    }

                    int daeAddId = 0, daeOperationId = 0;
                    if (!string.IsNullOrEmpty(daeType))
                    {
                        daeAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.大病, relationId);
                        if (daeAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的大病保险.", rowId, eName));
                        }
                        else
                        {
                            daeOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(daeAddId, daeType);
                            if (daeOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：大病报减方式[{1}]不存在.", rowId, daeType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(daeAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的大病记录.", rowId));
                            }
                        }

                    }

                    int buchonggongjijinAddId = 0, buchonggongjijinOperationId = 0;
                    if (!string.IsNullOrEmpty(buchonggongjijinType))
                    {
                        buchonggongjijinAddId = stopBll.GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId.补充公积金, relationId);
                        if (buchonggongjijinAddId == 0)
                        {
                            errMsg.AppendLine(string.Format("序号[{0}]：员工[{1}]没有申报成功的补充公积金.", rowId, eName));
                        }
                        else
                        {
                            buchonggongjijinOperationId = stopBll.GetPoliceOperationIdByEmployeeAddIdAndOperationName(buchonggongjijinAddId, buchonggongjijinType);
                            if (buchonggongjijinOperationId == 0)
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：补充公积金报减方式[{1}]不存在.", rowId, buchonggongjijinType));
                            }
                            if (stopBll.IsHaveStopingByEmployeeAddId(buchonggongjijinAddId))
                            {
                                errMsg.AppendLine(string.Format("序号[{0}]：已存在正在报减的补充公积金记录.", rowId));
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(errMsg.ToString()))
                    {
                        sbError.AppendLine(errMsg.ToString());
                        continue;
                    }

                    #endregion

                    DateTime yearMonth = DateTime.Now;

                    #region 养老
                    if (!string.IsNullOrEmpty(yanglaoType) && yanglaoAddId > 0 && yanglaoOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(yanglaoAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = yanglaoAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = yanglaoOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.养老),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 失业
                    if (!string.IsNullOrEmpty(shiyeType) && shiyeAddId > 0 && shiyeOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(shiyeAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = shiyeAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = shiyeOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.失业),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 工伤
                    if (!string.IsNullOrEmpty(gongshangType) && gongshangAddId > 0 && gongshangOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(gongshangAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = gongshangAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = gongshangOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.工伤),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 医疗
                    if (!string.IsNullOrEmpty(yiliaoType) && yiliaoAddId > 0 && yiliaoOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(yiliaoAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = yiliaoAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = yiliaoOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.医疗),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 生育
                    if (!string.IsNullOrEmpty(shengyuType) && shengyuAddId > 0 && shengyuOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(shengyuAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = shengyuAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = shengyuOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.生育),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 公积金
                    if (!string.IsNullOrEmpty(gongjijinType) && gongjijinAddId > 0 && gongjijinOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(gongjijinAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = gongjijinAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = gongjijinOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.公积金),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 大病
                    if (!string.IsNullOrEmpty(daeType) && daeAddId > 0 && daeOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(daeAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = daeAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = daeOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.大病),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                    #region 补充公积金
                    if (!string.IsNullOrEmpty(buchonggongjijinType) && buchonggongjijinAddId > 0 && buchonggongjijinOperationId > 0)
                    {
                        var policeInsurance = stopBll.GetPoliceInsuranceByEmployeeAddId(buchonggongjijinAddId);
                        DateTime insuranceMonth = yearMonth.AddMonths(policeInsurance.InsuranceReduce ?? 0);

                        EmployeeStopPayment stopPayment = new EmployeeStopPayment()
                        {
                            EmployeeAddId = buchonggongjijinAddId,
                            InsuranceMonth = insuranceMonth,
                            PoliceOperationId = buchonggongjijinOperationId,
                            State = EmployeeStopPayment_State.待员工客服确认.ToString(),
                            CreateTime = yearMonth,
                            CreatePerson = LoginInfo.UserName,
                            UpdateTime = yearMonth,
                            UpdatePerson = LoginInfo.UserName,
                            YearMonth = Convert.ToInt32(yearMonth.ToString("yyyyMM")),
                        };
                        EmployeeStopPaymentSingle stopPaymentSingle = new EmployeeStopPaymentSingle()
                        {
                            InsuranceKindId = Convert.ToInt32(EmployeeAdd_InsuranceKindId.补充公积金),
                            CompanyEmployeeRelationId = relationId,
                            PoliceInsuranceId = policeInsurance.Id,
                            StopPayment = stopPayment,
                        };
                        lstStopPayments.Add(stopPaymentSingle);
                    }
                    #endregion
                }
                message = sbError.ToString().Replace("\r\n", "<br />");
                if (lstStopPayments.Any() && string.IsNullOrEmpty(message))
                {
                    result = stopBll.InsertStopPayment(lstStopPayments);
                }
            }



            return result;
        }

        #endregion
    }
}