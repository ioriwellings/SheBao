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
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Transactions;
using System.Web.Script.Serialization;


namespace Langben.App.Controllers
{
    /// <summary>
    /// 费用_费用表
    /// </summary>
    public class COST_CostTableApiController : BaseApiController
    {
        string menuId_CostTable = "1043"; // 责任客服费用审核菜单编号

        #region 费用自检（责任客服费用审核）相关
        /// <summary>
        /// 异步加载数据(费用自检列表，既责任客服费用审核列表)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam, string search)
        {
            int total = 0;
            // 设置搜索默认值
            string yearMonth = DateTime.Now.ToString("yyyy-MM").Replace("-", "");
            int costTableType = 0;
            int status = 0;
            int companyId = 0;

            #region 已删除代码（未加权限时，取得公司是责任客服为当前用户的公司）
            //using (SysEntities db = new SysEntities())
            //{
            //    var query = (from cc in db.CRM_Company
            //                 join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
            //                 where cctb.UserID_ZR == LoginInfo.UserID
            //                 select cc.ID).ToList();
            //    int i = 0;
            //    int count = query.Count();
            //    companyId = new int[count];
            //    foreach (var item in query)
            //    {
            //        companyId[i] = item;
            //        i++;
            //    }
            //}
            #endregion

            // 各搜索项赋值
            if (!string.IsNullOrEmpty(search))
            {
                string[] searchList = search.Split('^');
                yearMonth = searchList[0].Split('$')[1];
                costTableType = searchList[1].Split('$')[1] != "" ? Convert.ToInt32(searchList[1].Split('$')[1]) : 0;
                status = searchList[2].Split('$')[1] != "" ? Convert.ToInt32(searchList[2].Split('$')[1]) : 0;
                if (searchList[3].Split('$')[1] != "" && searchList[3].Split('$')[1] != "0" && searchList[3].Split('$')[1] != "null")
                {
                    companyId = Convert.ToInt32(searchList[3].Split('$')[1]);
                }
            }

            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuId_CostTable);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuId_CostTable);
            }
            #endregion

            List<CostFeeModel> queryData = m_BLL.GetCostFeeList(getParam.id, getParam.page, getParam.rows, yearMonth, companyId, costTableType, status,
                departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    ID = s.ID
                    ,
                    CostTableType = ((Common.COST_Table_CostTableType)s.CostTableType).ToString()  // 费用表类型
                    ,
                    ChargeCost = s.ChargeCost
                    ,
                    ServiceCost = s.ServiceCost
                    ,
                    Remark = s.Remark
                    ,
                    Status = s.Status  // 费用表状态（实际值，数字）
                    ,
                    StatusName = ((Common.COST_Table_Status)s.Status).ToString()   // 费用表状态(文字)
                    ,
                    CRM_Company_ID = s.CRM_Company_ID
                    ,
                    CreateTime = s.CreateTime
                    ,
                    CreateUserID = s.CreateUserID
                    ,
                    CreateUserName = s.CreateUserName
                    ,
                    BranchID = s.BranchID
                    ,
                    YearMonth = s.YearMonth
                    ,
                    SerialNumber = s.SerialNumber   // 批次号
                    ,
                    CompanyName = s.CompanyName     // 企业名称

                })
            };
            return data;
        }


        /// <summary>
        /// 锁定费用表
        /// </summary>
        /// <param name="query">锁定的费用表记录ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result PostLock(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int[] lockedId = Array.ConvertAll<string, int>(query.Split(','), delegate(string s) { return int.Parse(s); });
            if (lockedId != null && lockedId.Length > 0)
            {
                if (m_BLL.UpdateCostTableStatusCollection(ref validationErrors, lockedId, (int)Common.COST_Table_Status.待核销))
                {
                    LogClassModels.WriteServiceLog("锁定成功" + "，信息的Id为" + string.Join(",", lockedId), "消息"
                        );//锁定成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "锁定成功";
                }
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
                LogClassModels.WriteServiceLog("锁定失败" + "，信息的Id为" + string.Join(",", lockedId) + "," + returnValue, "消息"
                    );//锁定失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "锁定失败" + returnValue;
            }
            return result;
        }

        /// <summary>
        /// 异步加载费用详情数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostCostDetailData(int id)
        {
            List<Cost_CostTableDetails> queryData = m_BLL.GetCostFeeDetailList(id);
            var data = new Common.ClientResult.DataResult
            {
                rows = queryData.Select(s => new
                {
                    No = s.No
                    ,
                    EmployName = s.EmployName
                    ,
                    CertificateNumber = s.CertificateNumber
                    ,
                    Supplier = string.IsNullOrEmpty(s.SupplierName) == true ? "河北搜才人力资源股份有限公司" : s.SupplierName// 供应商
                    ,

                    Operator_CompanyName=s.Operator_CompanyName,
                    PaymentStyle = ((Common.EmployeeMiddle_PaymentStyle)s.PaymentStyle).ToString()
                    ,
                    CityName = s.CityName   // 缴纳地
                    ,
                    // 养老
                    YanglaoPaymentInterval = s.YanglaoPaymentInterval
                    ,
                    YanglaoPaymentMonth = s.YanglaoPaymentMonth
                    ,
                    YanglaoCompanyRadix = s.YanglaoCompanyRadix
                    ,
                    YanglaoCompanyCost = s.YanglaoCompanyCost
                    ,
                    YanglaoPersonCost = s.YanglaoPersonCost
                    ,
                    YanglaoSum = s.YanglaoCompanyCost + s.YanglaoPersonCost

                    ,
                    // 失业
                    ShiyePaymentInterval = s.ShiyePaymentInterval
                    ,
                    ShiyePaymentMonth = s.ShiyePaymentMonth
                    ,
                    ShiyeCompanyRadix = s.ShiyeCompanyRadix
                    ,
                    ShiyeCompanyCost = s.ShiyeCompanyCost
                    ,
                    ShiyePersonCost = s.ShiyePersonCost
                    ,
                    ShiyeSum = s.ShiyeCompanyCost + s.ShiyePersonCost

                    ,
                    // 工伤
                    GongshangPaymentInterval = s.GongshangPaymentInterval
                    ,
                    GongshangCompanyRadix = s.GongshangCompanyRadix
                    ,
                    GongshangCompanyCost = s.GongshangCompanyCost

                    ,
                    // 医疗
                    YiliaoPaymentInterval = s.YiliaoPaymentInterval
                    ,
                    YiliaoPaymentMonth = s.YiliaoPaymentMonth
                    ,
                    YiliaoCompanyRadix = s.YiliaoCompanyRadix
                    ,
                    YiliaoCompanyCost = s.YiliaoCompanyCost
                    ,
                    YiliaoPersonCost = s.YiliaoPersonCost
                    ,
                    YiliaoSum = s.YiliaoCompanyCost + s.YiliaoPersonCost

                    ,
                    // 大病
                    DaeCompanyCost = s.DaeCompanyCost
                    ,
                    DaePersonCost = s.DaePersonCost

                    ,
                    // 生育
                    ShengyuCompanyCost = s.ShengyuCompanyCost

                    ,
                    // 公积金
                    GongjijinPaymentInterval = s.GongjijinPaymentInterval
                    ,
                    GongjijinPaymentMonth = s.GongjijinPaymentMonth
                    ,
                    GongjijinCompanyRadix = s.GongjijinCompanyRadix
                    ,
                    GongjijinCompanyCost = s.GongjijinCompanyCost
                    ,
                    GongjijinPersonCost = s.GongjijinPersonCost
                    ,
                    GongjijinSum = s.GongjijinCompanyCost + s.GongjijinPersonCost

                    ,
                    // 补充公积金
                    GongjijinBCCompanyCost = s.GongjijinBCCompanyCost
                    ,
                    GongjijinBCPersonCost = s.GongjijinBCPersonCost

                    ,
                    OtherInsuranceCost = s.OtherInsuranceCost // 其他社保费用
                    ,
                    OtherCost = s.OtherCost // 其他费用
                    ,
                    // 单位保险小计（各保险费用相加，养老、医疗、生育、工伤、失业、大病、公积金、补充公积金）
                    CompanyInsuranceSum = s.YanglaoCompanyCost + s.YiliaoCompanyCost + s.ShengyuCompanyCost + s.GongshangCompanyCost + s.ShiyeCompanyCost + s.GongjijinCompanyCost + s.DaeCompanyCost + s.GongjijinBCCompanyCost
                    ,
                    // 个人保险小计（各保险费用相加，养老、医疗、失业、大病、公积金、补充公积金）（生育和工伤只有单位才交）
                    PersonInsuranceSum = s.YanglaoPersonCost + s.ShiyePersonCost + s.YiliaoPersonCost + s.DaePersonCost + s.GongjijinPersonCost + s.GongjijinBCPersonCost
                    ,
                    // 保险合计（单位保险小计+个人保险小计）
                    InsuranceSum = (s.YanglaoCompanyCost + s.YiliaoCompanyCost + s.ShengyuCompanyCost + s.GongshangCompanyCost + s.ShiyeCompanyCost + s.GongjijinCompanyCost + s.DaeCompanyCost + s.GongjijinBCCompanyCost)
                        + (s.YanglaoPersonCost + s.ShiyePersonCost + s.YiliaoPersonCost + s.DaePersonCost + s.GongjijinPersonCost + s.GongjijinBCPersonCost)
                        + s.OtherInsuranceCost  // 其他社保费用计入到保险合计中
                    ,
                    ProductionCost = s.ProductionCost // 工本费
                    ,
                    ServiceCost = s.ServiceCost // 服务费
                    ,
                    // 合计列（其他费用+保险合计+工本费+服务费）
                    Sum = s.OtherCost + (s.YanglaoCompanyCost + s.YiliaoCompanyCost + s.ShengyuCompanyCost + s.GongshangCompanyCost + s.ShiyeCompanyCost + s.GongjijinCompanyCost + s.DaeCompanyCost + s.GongjijinBCCompanyCost)
                        + (s.YanglaoPersonCost + s.ShiyePersonCost + s.YiliaoPersonCost + s.DaePersonCost + s.GongjijinPersonCost + s.GongjijinBCPersonCost) + s.OtherInsuranceCost + s.ProductionCost + s.ServiceCost
                })
            };
            return data;
        }

        /// <summary>
        /// 导出费用明细信息
        /// </summary>
        /// <param name="id">费用表ID</param>
        /// <returns></returns>
        public string PostExportToExcel(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            using (MemoryStream ms = new MemoryStream())
            {
                CostFeeModel costItem = m_BLL.GetCostFeeModelById(id);
                List<Cost_CostTableDetails> queryData = m_BLL.GetCostFeeDetailList(id);
                FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Template/Excel/费用详情导出模板.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                hssfworkbook.SetSheetName(0, "费用明细");

                ISheet sheet1 = hssfworkbook.GetSheetAt(0);
                InsertRowsdaili2(sheet1, 7, queryData.Count() - 1);

                sheet1.GetRow(2).GetCell(0).SetCellValue(costItem.YearMonth.ToString().Insert(4, "年") + "月代理单位费用明细");
                sheet1.GetRow(3).GetCell(3).SetCellValue(costItem.CompanyName);
                sheet1.GetRow(3).GetCell(11).SetCellValue("社保期间：" + costItem.YearMonth.ToString());
                sheet1.GetRow(3).GetCell(25).SetCellValue("批次号：" + costItem.SerialNumber);
                sheet1.GetRow(3).GetCell(40).SetCellValue("客户编号：" + costItem.CompanyCode);
                int j = 0;

                //HSSFCellStyle styleRed = CellStyle(hssfworkbook);

                for (int i = 0; i < queryData.Count(); i++)
                {
                    IRow row = sheet1.GetRow(7 + i);
                    row.GetCell(1).SetCellValue(queryData[i].EmployName);

                    // 原有row.GetCell(0).SetCellValue(i + 1);               
                    #region  人员序号
                    if (i - 1 >= 0)
                    {
                        if (queryData[i].Employee_ID != queryData[i - 1].Employee_ID)
                        {
                            j++;
                            row.GetCell(0).SetCellValue(j);
                        }
                    }
                    else
                    {
                        j++;
                        row.GetCell(0).SetCellValue(j);
                    }
                    #endregion

                    row.GetCell(2).SetCellValue(queryData[i].CertificateNumber);  // 身份证号
                    row.GetCell(3).SetCellValue("河北搜才人力资源股份有限公司");  // 供应商
                    string style = ((Common.EmployeeMiddle_PaymentStyle)queryData[i].PaymentStyle).ToString();
                    row.GetCell(4).SetCellValue(style);  // 缴费类型
                    row.GetCell(5).SetCellValue(queryData[i].CityName);  // 缴纳地

                    #region 社保信息
                    // 养老
                    row.GetCell(6).SetCellValue(queryData[i].YanglaoPaymentInterval);
                    row.GetCell(7).SetCellValue(queryData[i].YanglaoPaymentMonth == null ? 0 : (int)queryData[i].YanglaoPaymentMonth);
                    row.GetCell(8).SetCellValue(queryData[i].YanglaoCompanyRadix == null ? 0 : (double)queryData[i].YanglaoCompanyRadix);
                    row.GetCell(9).SetCellValue((double)queryData[i].YanglaoCompanyCost);
                    row.GetCell(10).SetCellValue((double)queryData[i].YanglaoPersonCost);

                    // 失业
                    row.GetCell(12).SetCellValue(queryData[i].ShiyePaymentInterval);
                    row.GetCell(13).SetCellValue(queryData[i].ShiyePaymentMonth == null ? 0 : (int)queryData[i].ShiyePaymentMonth);
                    row.GetCell(14).SetCellValue(queryData[i].ShiyeCompanyRadix == null ? 0 : (double)queryData[i].ShiyeCompanyRadix);
                    row.GetCell(15).SetCellValue((double)queryData[i].ShiyeCompanyCost);
                    row.GetCell(16).SetCellValue((double)queryData[i].ShiyePersonCost);

                    // 工伤
                    row.GetCell(18).SetCellValue(queryData[i].GongshangPaymentInterval);
                    row.GetCell(19).SetCellValue(queryData[i].GongshangCompanyRadix == null ? 0 : (double)queryData[i].GongshangCompanyRadix);
                    row.GetCell(20).SetCellValue((double)queryData[i].GongshangCompanyCost);

                    // 医疗
                    row.GetCell(21).SetCellValue(queryData[i].YiliaoPaymentInterval);
                    row.GetCell(22).SetCellValue(queryData[i].YiliaoPaymentMonth == null ? 0 : (int)queryData[i].YiliaoPaymentMonth);
                    row.GetCell(23).SetCellValue(queryData[i].YiliaoCompanyRadix == null ? 0 : (double)queryData[i].YiliaoCompanyRadix);
                    row.GetCell(24).SetCellValue((double)queryData[i].YiliaoCompanyCost);
                    row.GetCell(25).SetCellValue((double)queryData[i].YiliaoPersonCost);

                    // 大病
                    row.GetCell(27).SetCellValue((double)queryData[i].DaeCompanyCost);
                    row.GetCell(28).SetCellValue((double)queryData[i].DaePersonCost);

                    // 生育
                    row.GetCell(29).SetCellValue((double)queryData[i].ShengyuCompanyCost);

                    // 公积金
                    row.GetCell(30).SetCellValue(queryData[i].GongjijinPaymentInterval);
                    row.GetCell(31).SetCellValue(queryData[i].GongjijinPaymentMonth == null ? 0 : (int)queryData[i].GongjijinPaymentMonth);
                    row.GetCell(32).SetCellValue(queryData[i].GongjijinCompanyRadix == null ? 0 : (double)queryData[i].GongjijinCompanyRadix);
                    row.GetCell(33).SetCellValue((double)queryData[i].GongjijinCompanyCost);
                    row.GetCell(34).SetCellValue((double)queryData[i].GongjijinPersonCost);

                    // 补充公积金
                    row.GetCell(36).SetCellValue((double)queryData[i].GongjijinBCCompanyCost);
                    row.GetCell(37).SetCellValue((double)queryData[i].GongjijinBCPersonCost);

                    row.GetCell(38).SetCellValue((double)queryData[i].OtherInsuranceCost);  // 其他社保费用
                    row.GetCell(39).SetCellValue((double)queryData[i].OtherCost);  // 其他费用

                    row.GetCell(43).SetCellValue((double)queryData[i].ProductionCost);  // 工本费
                    row.GetCell(44).SetCellValue((double)queryData[i].ServiceCost);  // 服务费
                    #endregion
                }
                IRow LastRow = sheet1.CreateRow(9 + queryData.Count());
                LastRow.CreateCell(0);
                LastRow.GetCell(0).SetCellValue("");
                sheet1.ForceFormulaRecalculation = true;
                string fileName = costItem.CompanyName + "_" + costItem.YearMonth.ToString() + "_费用明细详情" + ".xls";
                string urlPath = "/DataExport/" + fileName; // 文件下载的URL地址，供给前台下载
                string filePath = System.Web.HttpContext.Current.Server.MapPath("\\" + urlPath); // 文件路径

                file = new FileStream(filePath, FileMode.Create);
                hssfworkbook.Write(file);
                file.Close();

                return urlPath;  // 导出成功
            }
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// 责任客服作废费用表(将状态改为“责任客服作废”)
        /// </summary>   
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result DeleteCostTable(int id,int Status)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            if (m_BLL.CancelCostTable(ref validationErrors, id, Status))
            {
                LogClassModels.WriteServiceLog("作废成功，信息的Id为" + id, "消息"
                    );//作废成功，写入日志
                result.Code = Common.ClientCode.Succeed;
                result.Message = "作废成功";
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
                LogClassModels.WriteServiceLog("作废失败，信息的Id为" + id + "," + returnValue, "消息"
                    );//作废失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "作废失败" + returnValue;
            }
            return result;
        }
        
        /// <summary>
        /// 编辑备注信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  


        public Common.ClientResult.Result EditRemark(string ID, [FromBody]GroupUserModel data)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            int nid = 0;
            int.TryParse(ID, out nid);
            try
            {
                using (SysEntities db = new SysEntities())
                {
                        COST_CostTable model = db.COST_CostTable.SingleOrDefault(s => s.ID == nid);
                         model.Remark = data.Remark;
                         db.SaveChanges();
                        LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，费用_费用表信息的Id为" + ID, "费用_费用表");//写入日志                   
                        result.Code = Common.ClientCode.Succeed;
                        result.Message = Suggestion.UpdateSucceed;
                        return result; //提示更新成功 
                  
                }
            }
            catch
            {
                LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，费用_费用表信息的Id为" + ID + ",", "费用_费用表");//写入日志   
                result.Code = Common.ClientCode.Fail;
                result.Message = Suggestion.UpdateFail;
                return result; //提示更新失败
            }

        }

        #endregion

        #region 财务确认费用表相关api
        /// <summary>
        /// 异步加载数据(财务确认费用表列表数据)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostFinanceAduitData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            // 设置搜索默认值
            string yearMonth = DateTime.Now.ToString("yyyy-MM").Replace("-", "");
            int status = 0;
            List<CostFeeModel> queryData = m_BLL.GetCostFeeFinanceAduitList(getParam.id, getParam.page, getParam.rows, yearMonth, status, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    ID = s.ID
                    ,
                    No = s.YearMonth.ToString() + s.SerialNumber.ToString()   // 批次号=时间+流水号
                    ,
                    ChargeCost = s.ChargeCost
                    ,
                    ServiceCost = s.ServiceCost
                    ,
                    Remark = s.Remark
                    ,
                    Status = s.Status  // 费用表状态（实际值，数字）
                    ,
                    StatusName = ((Common.COST_Table_Status)s.Status).ToString()   // 费用表状态(文字)
                    ,
                    CRM_Company_ID = s.CRM_Company_ID
                    ,
                    CreateTime = s.CreateTime
                    ,
                    BranchID = s.BranchID
                    ,
                    YearMonth = s.YearMonth
                    ,
                    SerialNumber = s.SerialNumber
                    ,
                    CompanyName = s.CompanyName     // 企业名称
                    ,
                    UserName_ZR = s.UserName_ZR     // 责任客服名称

                })
            };
            return data;
        }

        #endregion


        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public COST_CostTable GetById(int id)
        {
            COST_CostTable item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]COST_CostTable entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，费用_费用表的信息的Id为" + entity.ID, "费用_费用表"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，费用_费用表的信息，" + returnValue, "费用_费用表"
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
        public Common.ClientResult.Result Put([FromBody]COST_CostTable entity)
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，费用_费用表信息的Id为" + entity.ID, "费用_费用表"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，费用_费用表信息的Id为" + entity.ID + "," + returnValue, "费用_费用表"
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




        IBLL.ICOST_CostTableBLL m_BLL = new BLL.COST_CostTableBLL();

        ValidationErrors validationErrors = new ValidationErrors();

        public COST_CostTableApiController()
            : this(new COST_CostTableBLL()) { }

        public COST_CostTableApiController(COST_CostTableBLL bll)
        {
            m_BLL = bll;
        }

        /// <summary>
        /// 生成费用表
        /// </summary>
        /// <param name="CRM_Company_ID">企业编码</param>
        /// <param name="yearMonth">年月</param>
        /// <returns></returns>
        public Common.ClientResult.Result PostCreate(int CRM_Company_ID, string yearMonth)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            string msg = "";

            bool f = IsTrue(out msg, CRM_Company_ID);
            if (f == false)
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = msg;
                return result;
            }

            //创建人Id，创建人，所属分支机构
            int createUserId = LoginInfo.UserID;
            string createUserName = LoginInfo.UserName;
            int breach = LoginInfo.BranchID;


            int id = 0;
            try
            {
                var yearMonth_Int = Convert.ToInt32(yearMonth);

                //判断这个客户这个月，是否有正常的费用表，如果有，则不允许生成
                using (SysEntities db = new SysEntities())
                {
                    int[] status = { 3, 5, 7 };
                    var comtablelist = db.COST_CostTable.Where(x => x.CRM_Company_ID == CRM_Company_ID && !status.Contains(x.Status) && x.YearMonth == yearMonth_Int).FirstOrDefault();
                    if (comtablelist != null)
                    {
                        result.Code = Common.ClientCode.Fail;
                        string s = ((Common.COST_Table_Status)comtablelist.Status).ToString();
                        result.Message = "该企业已经生成了费用表,状态【" + s + "】!";
                        return result;
                    }


                }



                //1费用表主表添加数据
                COST_CostTable cct = new COST_CostTable();
                cct.CreateFrom = (int)Common.CostTable_CreateFrom.本地费用;
                cct.YearMonth = yearMonth_Int;
                cct.CRM_Company_ID = CRM_Company_ID;
                cct.CostTableType = getPRD_Product(CRM_Company_ID);// "从企业那边找";
                if (cct.CostTableType == -1)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "该企业暂时没有可用的报价，请先维护报价！！！！";
                    return result;
                }
                cct.SerialNumber = m_BLL.GetSerialNumber(yearMonth_Int);
                cct.ChargeCost = 0;
                cct.ServiceCost = 0;
                cct.Status = (int)Common.COST_Table_Status.待责任客服确认;//待责任客服确认
                cct.CreateTime = DateTime.Now;
                cct.CreateUserID = createUserId;
                cct.CreateUserName = createUserName;
                cct.BranchID = breach;

                id = cct.ID;

                //2保存到数据库
                if (m_BLL.Save(ref validationErrors, cct, CRM_Company_ID, yearMonth_Int, createUserId, createUserName, breach))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，费用_费用表信息的Id为" + cct.ID, "费用_费用表"
                           );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed + cct.ID;
                }
                else
                {
                    string returnValue = "";
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }

                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，费用_费用表信息的Id为" + cct.ID, "费用_费用表"
                                              );//写入日志                   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + "：" + returnValue;
                }
            }
            catch (Exception ex)
            {
                LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", id) + "," + ex.ToString(), "消息"
                       );//删除失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = Suggestion.DeleteFail + ex.ToString();
            }

            return result;
        }
        ///// <summary>
        ///// 获取费用表流水号（每月从1开始）
        ///// </summary>
        ///// <param name="yearMonth"></param>
        ///// <returns></returns>
        //public string GetSerialNumber(int yearMonth)
        //{
        //    using (SysEntities db = new SysEntities())
        //    {
        //        var query = db.COST_CostTable.Where(cct => cct.YearMonth == yearMonth);
        //        if (query.Count() > 0)
        //        {
        //            var serinlNumber = query.OrderByDescending(o => o.SerialNumber).FirstOrDefault().SerialNumber;
        //            return yearMonth.ToString() + (Convert.ToInt32(serinlNumber.Substring(6)) + 1).ToString("00000");
        //        }
        //        else return yearMonth.ToString() + "00001";
        //    }
        //}

        /// <summary>
        /// 获取公司列表（根据页面设定权限获取）
        /// </summary>
        /// <param name="menuID">菜单对应编号</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCompany(string menuID = "")
        {
            // 获取公司列表（添加权限配置后的）
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

            var query = m_BLL.GetCompanyList(departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);

            var data = new Common.ClientResult.DataResult
            {
                total = query.Count(),
                rows = query.Select(s => new
                {
                    ID = s.ID,
                    Name = s.CompanyName
                })
            };
            return data;
        }

        #region 私有方法

        /// <summary>
        /// 费用明细导出excel模版设定（excel中格式）
        /// </summary>
        /// <param name="targetSheet"></param>
        /// <param name="fromRowIndex"></param>
        /// <param name="rowCount"></param>
        static void InsertRowsdaili2(ISheet targetSheet, int fromRowIndex, int rowCount)
        {
            if (rowCount != 0)
            {
                targetSheet.ShiftRows(fromRowIndex + 1, targetSheet.LastRowNum, rowCount, true, false);
                IRow rowSource = targetSheet.GetRow(fromRowIndex);
                ICellStyle rowstyle = rowSource.RowStyle;

                for (int rowIndex = fromRowIndex; rowIndex <= fromRowIndex + rowCount; rowIndex++)
                {
                    IRow rowInsert = targetSheet.CreateRow(rowIndex);
                    rowInsert.RowStyle = rowstyle;
                    rowInsert.Height = rowSource.Height;
                    for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
                    {
                        ICell cellSource = rowSource.GetCell(colIndex);
                        ICell cellInsert = rowInsert.CreateCell(colIndex);
                        if (cellSource != null)
                        {
                            cellInsert.CellStyle = cellSource.CellStyle;
                        }
                    }
                    targetSheet.GetRow(rowIndex).GetCell(11).CellFormula = string.Format("J{0}+K{0}", rowIndex + 1);//养老保险小计
                    targetSheet.GetRow(rowIndex).GetCell(17).CellFormula = string.Format("P{0}+Q{0}", rowIndex + 1);//失业保险小计
                    targetSheet.GetRow(rowIndex).GetCell(26).CellFormula = string.Format("Y{0}+Z{0}", rowIndex + 1);//医疗保险小计
                    targetSheet.GetRow(rowIndex).GetCell(35).CellFormula = string.Format("AH{0}+AI{0}", rowIndex + 1);//住房公积金小计
                    targetSheet.GetRow(rowIndex).GetCell(40).CellFormula = string.Format("J{0}+P{0}+U{0}+Y{0}+AB{0}+AD{0}+AH{0}+AK{0}", rowIndex + 1);//单位保险小计
                    targetSheet.GetRow(rowIndex).GetCell(41).CellFormula = string.Format("K{0}+Q{0}+Z{0}+AC{0}+AI{0}+AL{0}", rowIndex + 1);//个人保险小计
                    targetSheet.GetRow(rowIndex).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}+AM{0}", rowIndex + 1);//保险合计  (将其他社保费用计入保险合计)
                    targetSheet.GetRow(rowIndex).GetCell(45).CellFormula = string.Format("AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 1);//费用合计

                    //合计行
                    if (rowIndex == fromRowIndex + rowCount)
                    {
                        targetSheet.GetRow(rowIndex + 1).GetCell(9).CellFormula = string.Format("SUM(J{0}:J{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(10).CellFormula = string.Format("SUM(K{0}:K{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(15).CellFormula = string.Format("SUM(P{0}:P{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(16).CellFormula = string.Format("SUM(Q{0}:Q{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(20).CellFormula = string.Format("SUM(U{0}:U{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(24).CellFormula = string.Format("SUM(Y{0}:Y{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(25).CellFormula = string.Format("SUM(Z{0}:Z{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(27).CellFormula = string.Format("SUM(AB{0}:AB{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(28).CellFormula = string.Format("SUM(AC{0}:AC{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(29).CellFormula = string.Format("SUM(AD{0}:AD{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(33).CellFormula = string.Format("SUM(AH{0}:AH{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(34).CellFormula = string.Format("SUM(AI{0}:AI{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(36).CellFormula = string.Format("SUM(AK{0}:AK{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(37).CellFormula = string.Format("SUM(AL{0}:AL{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(38).CellFormula = string.Format("SUM(AM{0}:AM{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(39).CellFormula = string.Format("SUM(AN{0}:AN{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(43).CellFormula = string.Format("SUM(AR{0}:AR{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                        targetSheet.GetRow(rowIndex + 1).GetCell(44).CellFormula = string.Format("SUM(AS{0}:AS{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                        targetSheet.GetRow(rowIndex + 1).GetCell(11).CellFormula = string.Format("J{0}+K{0}", rowIndex + 2);//养老保险小计
                        targetSheet.GetRow(rowIndex + 1).GetCell(17).CellFormula = string.Format("P{0}+Q{0}", rowIndex + 2);//失业保险小计
                        targetSheet.GetRow(rowIndex + 1).GetCell(26).CellFormula = string.Format("Y{0}+Z{0}", rowIndex + 2);//医疗保险小计
                        targetSheet.GetRow(rowIndex + 1).GetCell(35).CellFormula = string.Format("AH{0}+AI{0}", rowIndex + 2);//住房公积金小计
                        targetSheet.GetRow(rowIndex + 1).GetCell(40).CellFormula = string.Format("J{0}+P{0}+U{0}+Y{0}+AB{0}+AD{0}+AH{0}+AK{0}", rowIndex + 2);//单位保险小计
                        targetSheet.GetRow(rowIndex + 1).GetCell(41).CellFormula = string.Format("K{0}+Q{0}+Z{0}+AC{0}+AI{0}+AL{0}", rowIndex + 2);//个人保险小计
                        targetSheet.GetRow(rowIndex + 1).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}+AM{0}", rowIndex + 2);//保险合计 (将其他社保费用计入保险合计)
                        targetSheet.GetRow(rowIndex + 1).GetCell(45).CellFormula = string.Format("AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 2);//费用合计
                    }
                }
            }
            else
            {
                int rowIndex = 7; fromRowIndex = 7;
                targetSheet.GetRow(rowIndex).GetCell(11).CellFormula = string.Format("J{0}+K{0}", rowIndex + 1);//养老保险小计
                targetSheet.GetRow(rowIndex).GetCell(17).CellFormula = string.Format("P{0}+Q{0}", rowIndex + 1);//失业保险小计
                targetSheet.GetRow(rowIndex).GetCell(26).CellFormula = string.Format("Y{0}+Z{0}", rowIndex + 1);//医疗保险小计
                targetSheet.GetRow(rowIndex).GetCell(35).CellFormula = string.Format("AH{0}+AI{0}", rowIndex + 1);//住房公积金小计
                targetSheet.GetRow(rowIndex).GetCell(40).CellFormula = string.Format("J{0}+P{0}+U{0}+Y{0}+AB{0}+AD{0}+AH{0}+AK{0}", rowIndex + 1);//单位保险小计
                targetSheet.GetRow(rowIndex).GetCell(41).CellFormula = string.Format("K{0}+Q{0}+Z{0}+AC{0}+AI{0}+AL{0}", rowIndex + 1);//个人保险小计
                targetSheet.GetRow(rowIndex).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}+AM{0}", rowIndex + 1);//保险合计 (将其他社保费用计入保险合计)
                targetSheet.GetRow(rowIndex).GetCell(45).CellFormula = string.Format("AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 1);//费用合计

                targetSheet.GetRow(rowIndex + 1).GetCell(9).CellFormula = string.Format("SUM(J{0}:J{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(10).CellFormula = string.Format("SUM(K{0}:K{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(15).CellFormula = string.Format("SUM(P{0}:P{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(16).CellFormula = string.Format("SUM(Q{0}:Q{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(20).CellFormula = string.Format("SUM(U{0}:U{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(24).CellFormula = string.Format("SUM(Y{0}:Y{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(25).CellFormula = string.Format("SUM(Z{0}:Z{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(27).CellFormula = string.Format("SUM(AB{0}:AB{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(28).CellFormula = string.Format("SUM(AC{0}:AC{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(29).CellFormula = string.Format("SUM(AD{0}:AD{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(33).CellFormula = string.Format("SUM(AH{0}:AH{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(34).CellFormula = string.Format("SUM(AI{0}:AI{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(36).CellFormula = string.Format("SUM(AK{0}:AK{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(37).CellFormula = string.Format("SUM(AL{0}:AL{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(38).CellFormula = string.Format("SUM(AM{0}:AM{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(39).CellFormula = string.Format("SUM(AN{0}:AN{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(43).CellFormula = string.Format("SUM(AR{0}:AR{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                targetSheet.GetRow(rowIndex + 1).GetCell(44).CellFormula = string.Format("SUM(AS{0}:AS{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                targetSheet.GetRow(rowIndex + 1).GetCell(11).CellFormula = string.Format("J{0}+K{0}", rowIndex + 2);//养老保险小计
                targetSheet.GetRow(rowIndex + 1).GetCell(17).CellFormula = string.Format("P{0}+Q{0}", rowIndex + 2);//失业保险小计
                targetSheet.GetRow(rowIndex + 1).GetCell(26).CellFormula = string.Format("Y{0}+Z{0}", rowIndex + 2);//医疗保险小计
                targetSheet.GetRow(rowIndex + 1).GetCell(35).CellFormula = string.Format("AH{0}+AI{0}", rowIndex + 2);//住房公积金小计
                targetSheet.GetRow(rowIndex + 1).GetCell(40).CellFormula = string.Format("J{0}+P{0}+U{0}+Y{0}+AB{0}+AD{0}+AH{0}+AK{0}", rowIndex + 2);//单位保险小计
                targetSheet.GetRow(rowIndex + 1).GetCell(41).CellFormula = string.Format("K{0}+Q{0}+Z{0}+AC{0}+AI{0}+AL{0}", rowIndex + 2);//个人保险小计
                targetSheet.GetRow(rowIndex + 1).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}+AM{0}", rowIndex + 2);//保险合计 (将其他社保费用计入保险合计)
                targetSheet.GetRow(rowIndex + 1).GetCell(45).CellFormula = string.Format("AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 2);//费用合计
            }
        }

        private int getPRD_Product(int cid)
        {
            using (SysEntities db = new SysEntities())
            {
                var query = db.CRM_CompanyPrice.Where(o => o.CRM_Company_ID == cid && o.Status == (int)Common.Status.启用);
                if (query.Count() > 0)
                {
                    return (int)query.FirstOrDefault().PRD_Product_ID;
                }
                else return -1;
            }
        }




        public bool IsTrue(out string meg, int CRM_Company_ID)
        {
            using (SysEntities db = new SysEntities())
            {

                var count = db.CompanyEmployeeRelation.Where(ce => ce.State == "在职" && ce.CompanyId == CRM_Company_ID).Count();
                if (count > 0)
                {
                    var cRM_CompanyLadderPrice = db.CRM_CompanyLadderPrice.Where(cclp => cclp.CRM_Company_ID == CRM_Company_ID && (new[] { (int)Common.Status.启用, (int)Common.Status.修改中 }).Contains(cclp.Status) && cclp.BeginLadder <= count && cclp.EndLadder >= count);
                    if (cRM_CompanyLadderPrice.Count() <= 0)
                    {
                        meg = "没有找到对应的单人报价，请先维护报价信息";
                        return false;
                    }
                    else
                    {
                        meg = "";
                        return true;
                    }


                }
                else
                {
                    meg = "该公司没有在职人员无法费用测算";
                    return false;
                }

            }
        }


        #endregion

        #region 根据费用表的ID获得费用表的信息
       
        public Common.ClientResult.DataResult GetCost_Cost_Company(int Cost_TableID)
        {

            var query = m_BLL.GetCost_Cost_Company(Cost_TableID);


            var data = new Common.ClientResult.DataResult
            {
                total = query.Count(),
                rows = query
            };
            return data;
        }
        #endregion
        public class GroupUserModel
        {
            public string Remark { get; set; }
          
        }
    }
}


