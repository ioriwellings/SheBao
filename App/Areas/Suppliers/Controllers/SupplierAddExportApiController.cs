using Common;
using Langben.DAL;
using Langben.DAL.Model;
using Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
    public class SupplierAddExportApiController : BaseApiController
    {
        IBLL.IEmployeeAddBLL m_BLL = new BLL.EmployeeAddBLL();


        #region 供应商客服提取报增信息列表

        /// <summary>
        /// 供应商客服提取报增信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult SupplierExportList([FromBody]GetDataParam getParam)
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
                        string[] states = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (states.Contains(Common.CollectState.未提取.ToString()))
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
            List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            //  List<EmployeeApprove> queryData = m_BLL.GetCommissionerListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 供应商客服提取报增信息

        /// <summary>
        /// 供应商客服提取报增信息
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult SupplierExport([FromBody]GetDataParam getParam)
        {
            FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Template/Excel/供应商客服报增数据提取模板.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook workbook = new HSSFWorkbook(file);
            //ICellStyle cellStyle = workbook.CreateCellStyle();
            //cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            //cellStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle style = workbook.CreateCellStyle();
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            style.FillPattern = FillPattern.SolidForeground;

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
                            string state = Common.EmployeeAdd_State.待供应商客服提取.ToString();
                            search += "State&" + state + "^";
                        }
                        else if (str.Equals(Common.CollectState.已提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.已发送供应商.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
                search += "UserID_Supplier&" + LoginInfo.UserID + "^";
                //search += "BranchID&" + LoginInfo.BranchID + "^";
                //search += "BranchName&" + LoginInfo.BranchName + "^";


                List<SupplierAddView> queryData = m_BLL.GetEmployeeAddExcelListBySupplier(search).OrderBy(c => c.CertificateNumber).ThenBy(c => c.CityID).ThenBy(c => c.OperationTime).ToList();
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

                using (MemoryStream ms = new MemoryStream())
                {

                    #region 生成Excel


                    string ids = string.Empty;
                    int[] intArray = new int[queryData.Count];

                    //员工社保一览
                    ISheet sheet = workbook.GetSheetAt(0);
                    sheet.SetActiveCell(0, 0);
                    int rowNum = 1;

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
                                currentRow.GetCell(19).SetCellValue(strRemark);  // 备注
                                strRemark = "";
                            }

                            rowNum++;

                            currentRow = sheet.CopyRow(rowNum + 1, rowNum);

                            currentRow.GetCell(0).SetCellValue(rowNum - 1);  // 序号
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
                        }
                        int cellNum = 0;
                        switch (Convert.ToInt32(query.InsuranceKindId))
                        {
                            case (int)Common.EmployeeAdd_InsuranceKindId.公积金:
                                currentRow.GetCell(15).SetCellValue(Convert.ToDateTime(query.OperationTime).ToString("yyyyMM")); // 公积金缴费开始时间
                                currentRow.GetCell(17).SetCellValue((query.EmployeePercent * 100).ToString() + "%");  // 公积金比例（个人）
                                currentRow.GetCell(18).SetCellValue((query.CompanyPercent * 100).ToString() + "%");  // 公积金比例（单位）

                                cellNum = 16;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.养老:
                                cellNum = 9;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.医疗:
                                cellNum = 10;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.失业:
                                cellNum = 11;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.工伤:
                                cellNum = 12;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.生育:
                                cellNum = 13;
                                break;
                            case (int)Common.EmployeeAdd_InsuranceKindId.大病:
                                cellNum = 14;
                                break;
                            default:
                                break;
                        }
                        currentRow.GetCell(cellNum).SetCellValue(query.CompanyNumber.ToString());  // 基数
                        if (query.State == "1")
                        {
                            currentRow.GetCell(cellNum).CellStyle = style;
                        }

                        if (!string.IsNullOrEmpty(query.SupplierRemark))
                        {
                            strRemark += query.SupplierRemark + "  ";
                        }

                    }

                    if (!string.IsNullOrEmpty(strRemark))
                    {
                        currentRow.GetCell(19).SetCellValue(strRemark);  // 备注
                    }

                    #endregion

                    #endregion

                    bool rst = m_BLL.ChangeStatus(intArray, Common.EmployeeAdd_State.待供应商客服提取.ToString(), EmployeeAdd_State.已发送供应商.ToString(), LoginInfo.LoginName);

                    if (rst)
                    {
                        string fileName = "供应商客服导出_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
                        //string fileName = name + "供应商导出.xls";
                        string urlPath = "DataExport\\" + fileName; // 文件下载的URL地址，供给前台下载
                        string filePath = System.Web.HttpContext.Current.Server.MapPath("\\") + urlPath; // 文件路径
                        file = new FileStream(filePath, FileMode.Create);
                        workbook.Write(file);
                        file.Close();

                        string Message = "已成功提取报增信息";

                        return new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.Succeed,
                            Message = "已成功提取报增信息",
                            URL = urlPath
                        };
                    }
                    else
                    {
                        return new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.Succeed,
                            Message = "报增信息提取失败",
                            URL = ""
                        };
                    }
                }
            }
            //catch (Exception e)
            //{
            //    file.Close();
            //    return new Common.ClientResult.UrlResult
            //    {
            //        Code = ClientCode.Fail,
            //        Message = e.Message
            //    };
            //}
            catch (DbEntityValidationException ex)
            {
                string errmsg = "";
                foreach (var errors in ex.EntityValidationErrors)
                {
                    foreach (var item in errors.ValidationErrors)
                    {
                        errmsg += item.ErrorMessage + item.PropertyName;
                    }
                }
                return null;
            }

        }

        #endregion
    }
}
