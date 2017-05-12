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
using Langben.App.Models;
using System.Web;
using System.Data;
using Langben.DAL.Model;
using System.Text.RegularExpressions;

namespace Langben.App.Controllers
{
    /// <summary>
    /// 员工费用中间表
    /// </summary>
    public class EmployeeMiddleController : BaseController
    {
        //string menuId = "1041";   // 菜单“费用中间表管理”
        string AddButton = "1041-1";//添加按钮权限码
        string EditButton = "1041-2";//启用/停用按钮权限码

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            #region 按钮权限
            ViewBag.AddButton = this.MenuOpAuthority(AddButton);
            ViewBag.EditButton = this.MenuOpAuthority(EditButton);
            #endregion
            return View();
        }

        /// <summary>
        /// 导入其他费用
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeMiddleByExcel()
        {
            return View();
        }
         /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexSef()
        {

            return View();
        }
 
        /// <summary>
        /// 首次创建
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Create(string companyName, string employeeName, string cardId, string companyEmployeeRelationId, string cityId)
        {
            ViewBag.CompanyName = companyName;
            ViewBag.EmployeeName = employeeName;
            ViewBag.CardId = cardId;
            ViewBag.CompanyEmployeeRelationId = companyEmployeeRelationId;
            ViewBag.CityId = cityId;

            return View();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Edit(int Id)
        {
            BLL.EmployeeMiddleBLL mybll = new EmployeeMiddleBLL();
            EmployeeMiddleShow Model = mybll.GetDataByID(Id);
            ViewBag.EmployeeMiddleShow = Model;
            return View();
        }

        #region  社保模板导入其他社保费

        /// <summary>
        /// 社保模板报增导入
        /// </summary>
        /// <returns></returns>
        public ActionResult InputEmployeeMiddleByExcel(HttpPostedFileBase files)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            #region 导入Excel

            try
            {

                HttpPostedFileBase file = files;
                string FileName;
                string savePath = string.Empty;
                if (file == null || file.ContentLength <= 0)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "文件不能为空";
                    //return result;
                    return Json(new { code = 0, msg = result.Message });
                }
                else
                {
                    string filename = System.IO.Path.GetFileName(file.FileName);
                    int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                    string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                    int Maxsize = 4000 * 1024;//定义上传文件的最大空间大小为4M
                    string FileType = ".xls";//定义上传文件的类型字符串

                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        result.Code = Common.ClientCode.Fail;
                        result.Message = "文件类型不对，只能导入xls格式的文件";
                        //return result;
                        return Json(new { code = 0, msg = result.Message });
                    }
                    //if (filesize >= Maxsize)
                    //{
                    //    result.Code = Common.ClientCode.Fail;
                    //    result.Message = "上传文件超过4M，不能上传";
                    //    //return result;
                    //    return Json(new { code = 0, msg = result.Message });
                    //}
                    string path = AppDomain.CurrentDomain.BaseDirectory + "excel/";
                    savePath = System.IO.Path.Combine(path, FileName);
                    file.SaveAs(savePath);
                }
                //string filename = System.IO.Path.GetFileName(file.FileName);
                string message = string.Empty;
                DataTable table = Util_XLS.NpoiReadExcle2(savePath, "Sheet1", out message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = message;
                    return Json(new { code = 0, msg = result.Message });
                }

                if (table == null && table.Rows.Count <= 0)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "导入Excel中不存在有效报增信息请核实!";
                    return Json(new { code = 0, msg = result.Message });
                }
                int SuccessNum = 0;
                if (ImportEmployeeMiddle(table, ref message, out SuccessNum, FileName))
                {

                    result.Code = Common.ClientCode.Succeed;
                    result.Message = string.Format("成功导入{0}人报增信息!", table.Rows.Count);
                    return Json(new { code = 1, msg = result.Message });
                }
                else
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = message;
                    return Json(new { code = 0, msg = result.Message });

                }
                //return result;
            }
            catch (Exception ex)
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = ex.Message + "导入失败,请认真检查excel";
                //return result;
                return Json(new { code = 0, msg = result.Message });
            }

            #endregion
        }
        IBLL.ICRM_CompanyBLL crm_BLL = new BLL.CRM_CompanyBLL();
        IBLL.ICityBLL city_BLL = new BLL.CityBLL();
        IBLL.IEmployeeBLL emp_BLL = new BLL.EmployeeBLL();
        IBLL.ICompanyEmployeeRelationBLL empR_BLL = new BLL.CompanyEmployeeRelationBLL();
        IBLL.IEmployeeMiddleBLL empM_BLL = new BLL.EmployeeMiddleBLL();
        IBLL.IEmployeeMiddleImportRecordBLL empMIR_BLL = new BLL.EmployeeMiddleImportRecordBLL();
        /// <summary>
        /// 数据库导入
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        private bool ImportEmployeeMiddle(DataTable table, ref string message, out int SuccessNum, string FileName)
        {
            try
            {
                StringBuilder sbError = new StringBuilder();
                SuccessNum = 0;
                if (table.Rows.Count > 0)
                {
                    #region Excel数据提取
                    //校验并转换信息
                    List<EmployeeMiddle> employeeList = CheckImportEmployeeMiddle(table, ref message);

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        return false;
                    }

                    EmployeeMiddleImportRecord emir = new EmployeeMiddleImportRecord();
                    emir.CreateTime = DateTime.Now;
                    emir.CreateUserID = LoginInfo.UserID;
                    emir.CreateUserName = LoginInfo.RealName;
                    emir.ImportCount = employeeList.Count();
                    emir.URL = "/excel/" + FileName;
                    
                    decimal money1 = employeeList.Sum(a => a.CompanyPayment)??0;
                    decimal money2 = employeeList.Sum(a => a.EmployeePayment) ?? 0;
                    emir.ImportPayment = money1 + money2;
                    int num = empM_BLL.InsertList(employeeList);
                    ValidationErrors validationErrors = new ValidationErrors();
                    empMIR_BLL.Create(ref validationErrors,emir);

                    
                    #endregion
                }

            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        private List<EmployeeMiddle> CheckImportEmployeeMiddle(DataTable table, ref string message)
        {
            List<EmployeeMiddleExcelModel> employeeList = new List<EmployeeMiddleExcelModel>();
            #region 数据转换
            foreach (DataRow dr in table.Rows)
            {

                EmployeeMiddleExcelModel employee = new EmployeeMiddleExcelModel();
                employee.CertificateNumber = dr["证件号码"].ToString().Trim();
                employee.Name = dr["姓名"].ToString().Trim();
                employee.City = dr["社保缴纳地"].ToString().Trim();
                employee.CompanyPayment = dr["企业承担金额"].ToString().Trim();
                employee.EmployeePayment = dr["个人承担金额"].ToString().Trim();
                employee.Remark = dr["备注"].ToString().Trim();
                employeeList.Add(employee);
            }
            #endregion

            string search = string.Empty;
            StringBuilder errMessageAll = new StringBuilder();
            List<EmployeeMiddle> PostInfoList = new List<EmployeeMiddle>();
            string DataForm = "证件号:{0},姓名:{1},的异常信息为:{2}\r\n";
            foreach (EmployeeMiddleExcelModel item in employeeList)
            {
                StringBuilder errMessage = new StringBuilder();
                EmployeeMiddle postinfos = new EmployeeMiddle();
                CompanyEmployeeRelation cer = new CompanyEmployeeRelation();
                #region 基础信息赋值

                if (string.IsNullOrEmpty(item.CertificateNumber))
                {
                    errMessage.Append("证件号码为空;\r\n");
                }
                else {
                    cer.EmployeeId = getEmployeeIdByExcel(item, ref errMessage);
                }

                if (string.IsNullOrEmpty(item.Name))
                {
                    errMessage.Append("姓名为空;\r\n");
                }
                if (string.IsNullOrEmpty(item.City))
                {
                    errMessage.Append("社保缴纳地为空;\r\n");
                }
                else
                {
                    cer.CityId = getCityIdByExcel(item, ref errMessage);
                    postinfos.CityId = cer.CityId;
                }
                if (cer.EmployeeId != null && !string.IsNullOrWhiteSpace(cer.CityId))
                {
                    postinfos.CompanyEmployeeRelationId = CompanyEmployeeRelationId(cer.EmployeeId, cer.CityId, ref errMessage);
                }

                if(string.IsNullOrEmpty(item.CompanyPayment))
                {
                    errMessage.Append("企业承担金额为空;\r\n");
                }
                else
                {
                    decimal wage = 0;
                    if (decimal.TryParse(item.CompanyPayment, out wage))
                    {

                        if (wage <= 0)
                        {
                            errMessage.Append("企业承担金额不正确;\r\n");
                        }
                        if (!Business.IsNumber(item.CompanyPayment, 32, 2))
                        {
                            errMessage.Append("企业承担金额请保留两位小数;\r\n");
                        }
                        postinfos.CompanyPayment = wage;
                    }
                    else
                    {
                        errMessage.Append("企业承担金额不正确;\r\n");
                    }
                }


                if (string.IsNullOrEmpty(item.EmployeePayment))
                {
                    errMessage.Append("个人承担金额为空;\r\n");
                }
                else
                {
                    decimal wage = 0;
                    if (decimal.TryParse(item.EmployeePayment, out wage))
                    {

                        if (wage <= 0)
                        {
                            errMessage.Append("个人承担金额不正确;\r\n");
                        }
                        if (!Business.IsNumber(item.EmployeePayment, 32, 2))
                        {
                            errMessage.Append("个人承担金额请保留两位小数;\r\n");
                        }
                        postinfos.EmployeePayment = wage;
                    }
                    else
                    {
                        errMessage.Append("个人承担金额不正确;\r\n");
                    }
                }

                postinfos.Remark = item.Remark;
                postinfos.PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                DateTime now = DateTime.Now;
                postinfos.PaymentBetween =now.ToString("yyyyMM") + "-" + now.ToString("yyyyMM");
                postinfos.UseBetween = 0;

                postinfos.StartDate = Convert.ToInt32(now.ToString("yyyyMM"));
                postinfos.EndedDate = Convert.ToInt32(now.ToString("yyyyMM"));

                postinfos.InsuranceKindId = (int)Common.EmployeeMiddle_InsuranceKind.其他社保费用;
                postinfos.State = Common.Status.启用.ToString();
                postinfos.CreatePerson = LoginInfo.RealName;
                postinfos.CreateTime = DateTime.Now;

                PostInfoList.Add(postinfos);
                if (!string.IsNullOrWhiteSpace(errMessage.ToString()))
                {

                    errMessageAll.Append(string.Format(DataForm, item.CertificateNumber, item.Name, errMessage.ToString()));
                }
                #endregion
              
       
            }
            message = errMessageAll.ToString();
            return PostInfoList;
        }

        

        #region 判断        
        private int? CompanyEmployeeRelationId(int? EmployeeId, string CityId, ref StringBuilder Message)
        {
            string search = "EmployeeIdDDL_Int&" + EmployeeId;
            search += "^CityIdDDL_String&" + CityId;
            List<CompanyEmployeeRelation> CompanyEmployeeRelationList = empR_BLL.GetByParam(0, "ASC", "Id", search);
            if (CompanyEmployeeRelationList == null || CompanyEmployeeRelationList.Count <= 0)
            {
                Message.Append("人员和缴纳地关系在系统中不存在;\r\n");

            }
            else if (CompanyEmployeeRelationList.Count == 1)
            {
                //组建员工企业关系信息
                return CompanyEmployeeRelationList[0].Id;
            }
            else
            {
                Message.Append("人员和缴纳地关系在系统中重复;\r\n");
            }
            return null;
        }
        private string getCityIdByExcel(EmployeeMiddleExcelModel item, ref StringBuilder Message)
        {
            string search = "NameDDL_String&" + item.City;
            List<City> CityList = city_BLL.GetByParam(string.Empty, "ASC", "Id", search);
            if (CityList == null || CityList.Count <= 0)
            {
                Message.Append("社保缴纳地在系统不存在;\r\n");

            }
            else if (CityList.Count == 1)
            {
                //组建员工企业关系信息
                return CityList[0].Id;
            }
            else
            {
                Message.Append("社保缴纳地在系统重复;\r\n");
            }
            return null;
        }
        private int? getEmployeeIdByExcel(EmployeeMiddleExcelModel item, ref StringBuilder Message)
        {
            string search = "CertificateNumberDDL_String&" + item.CertificateNumber;
            List<Employee> EmployeeList = emp_BLL.GetByParam(0, "ASC", "Id", search);
            if (EmployeeList == null || EmployeeList.Count <= 0)
            {
                Message.Append("人员在系统不存在;\r\n");

            }
            else if (EmployeeList.Count == 1)
            {
                //组建员工企业关系信息
                return EmployeeList[0].Id;
            }
            else
            {
                Message.Append("人员在系统重复;\r\n");
            }
            return null;
        }
        #endregion
        #endregion
     
    }
}


