using Common;
using Langben.BLL;
using Langben.DAL;
using Langben.DAL.Model;
using Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierStopExportApiController : BaseApiController
    {
        IBLL.IEmployeeStopPaymentBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public SupplierStopExportApiController()
            : this(new EmployeeStopPaymentBLL()) { }

        public SupplierStopExportApiController(EmployeeStopPaymentBLL bll)
        {
            m_BLL = bll;
        }

        #region 供应商客服提取停缴信息列表

        /// <summary>
        /// 供应商客服提取停缴信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostExportList([FromBody]GetDataParam getParam)
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
                        if (str.Contains(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.待供应商客服提取.ToString();
                            search += "State&" + state + "^";
                        }
                        else
                        {
                            string state = Common.EmployeeAdd_State.已发送供应商.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
            }
            else
            {
                string state = Common.EmployeeAdd_State.待供应商客服提取.ToString();
                search = "State&" + state + "^";
            }
            search += "UserID_Supplier&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveList(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 供应商客服提取报减信息

        /// <summary>
        /// 供应商客服提取报减信息
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult SupplierExport([FromBody]GetDataParam getParam)
        {
            FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Template/Excel/供应商客服报减数据提取模版.xls"), FileMode.Open, FileAccess.Read);
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
                        if (str.Contains(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.待供应商客服提取.ToString();
                            search += "State&" + state + "^";
                        }
                        else
                        {
                            string state = Common.EmployeeAdd_State.已发送供应商.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
                search += "UserID_Supplier&" + LoginInfo.UserID + "^";

                List<SupplierAddView> queryData = m_BLL.GetEmployeeStopExcelListBySupplier(search).OrderBy(c => c.CertificateNumber).ThenBy(c => c.CityID).ThenBy(c => c.OperationTime).ToList();
                if (queryData.Count == 0)
                {
                    var data = new Common.ClientResult.UrlResult
                    {
                        Code = ClientCode.FindNull,
                        Message = "没有符合条件的数据",
                        URL = ""
                    };
                    return data;
                }


                //string excelName = "供应商导出" + DateTime.Now.ToString();
                using (MemoryStream ms = new MemoryStream())
                {
                    string ids = string.Empty;
                    int[] intArray = new int[queryData.Count];

                    //员工社保一览
                    ISheet sheet = workbook.GetSheetAt(0);
                    sheet.SetActiveCell(0, 0);
                    int rowNum = 0;

                    ICellStyle style = workbook.CreateCellStyle();
                    style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
                    style.FillPattern = FillPattern.SolidForeground;

                    string tmpCertificateNumber = "";
                    string tmpCity = "";
                    string tmpYearMonth = "";
                    string strRemark = "";

                    IRow currentRow = null;

                    #region 数据输出
                    int i = 0;
                    foreach (var query in queryData)
                    {
                        intArray[i] = query.EmployeeAddId ?? 0;
                        i++;
                        if (tmpCertificateNumber != query.CertificateNumber || tmpCity != query.City ||
                            (tmpYearMonth != Convert.ToDateTime(query.OperationTime).ToString("yyyyMM") && query.InsuranceKindId != (int)Common.EmployeeAdd_InsuranceKindId.公积金))
                        {
                            //保存临时数据
                            tmpCertificateNumber = query.CertificateNumber;
                            tmpCity = query.City;
                            tmpYearMonth = Convert.ToDateTime(query.OperationTime).ToString("yyyyMM");

                            if (currentRow != null)
                            {
                                currentRow.GetCell(10).SetCellValue(strRemark);  // 备注
                                strRemark = "";
                            }

                            rowNum++;

                            currentRow = sheet.CopyRow(rowNum + 1, rowNum);

                            currentRow.GetCell(0).SetCellValue(rowNum);  // 序号
                            currentRow.GetCell(1).SetCellValue(query.CustomerName);  // 客服姓名
                            currentRow.GetCell(2).SetCellValue(query.City);  // 缴费地
                            currentRow.GetCell(3).SetCellValue(query.BranchName);  // 分公司名称
                            currentRow.GetCell(4).SetCellValue(query.CompanyName);  // 客户单位名称
                            currentRow.GetCell(5).SetCellValue(query.EmployeeName);  // 员工姓名
                            currentRow.GetCell(6).SetCellValue(query.CertificateNumber);  // 身份证号码
                            currentRow.GetCell(7).SetCellValue(query.Telephone);  // 联系电话
                            if (query.InsuranceKindId != (int)Common.EmployeeAdd_InsuranceKindId.公积金)
                            {
                                currentRow.GetCell(8).SetCellValue(tmpYearMonth);  // 缴费开始时间
                            }
                            else
                            {
                                currentRow.GetCell(9).SetCellValue(tmpYearMonth);  // 缴费开始时间
                            }
                        }
                        int cellNum = 0;
                        switch (Convert.ToInt32(query.InsuranceKindId))
                        {
                            case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                                cellNum = 9;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                                cellNum = 8;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                                cellNum = 8;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                                cellNum = 8;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                                cellNum = 8;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                                cellNum = 8;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                                cellNum = 8;
                                break;
                            default:
                                break;
                        }
                        if (query.State == "1")
                        {
                            currentRow.GetCell(cellNum).CellStyle = style;
                        }

                        if (!string.IsNullOrEmpty(query.Remark))
                        {
                            strRemark += query.Remark + Convert.ToChar(10);
                        }

                        ids += query.EmployeeAddId + ",";
                    }
                    #endregion

                    var results = 0;//返回的结果
                    if (queryDic["CollectState"].Equals(Common.CollectState.未提取.ToString()))
                    {
                        int[] ApprovedId;
                        string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        ApprovedId = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                        string returnValue = string.Empty;
                        if (ApprovedId != null && ApprovedId.Length > 0)
                        {
                            if (m_BLL.EmployeeStopPaymentApproved(ref validationErrors, ApprovedId, Common.EmployeeStopPayment_State.待供应商客服提取.ToString(), Common.EmployeeStopPayment_State.已发送供应商.ToString(), null, LoginInfo.LoginName))
                            {
                                results = ApprovedId.Count();
                                LogClassModels.WriteServiceLog("供应商客服已提取" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
                                    );//回退成功，写入日志
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
                                LogClassModels.WriteServiceLog("供应商客服提取失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                                    );//回退失败，写入日志
                            }
                        }
                    }
                    string fileName = "供应商客服导出_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
                    string urlPath = "DataExport\\" + fileName; // 文件下载的URL地址，供给前台下载
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("\\") + urlPath; // 文件路径
                    file = new FileStream(filePath, FileMode.Create);
                    workbook.Write(file);
                    file.Close();

                    string Message = queryDic["CollectState"].Equals(Common.CollectState.未提取.ToString()) ? "已成功提取报减信息" : "导出成功";

                    if (queryData.Count == 0)
                    {
                        //ActionResult result = 
                        var data = new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.FindNull,
                            Message = "没有符合条件的数据",
                            URL = urlPath
                        };
                        return data;
                    }
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

    }
}
