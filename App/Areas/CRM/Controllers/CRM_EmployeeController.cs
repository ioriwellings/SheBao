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
using System.Data;
using System.Web;
using System.Text.RegularExpressions;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 员工
    /// </summary>
    public class CRM_EmployeeController : BaseController
    {

        IBLL.IEmployeeBLL emp_BLL = new BLL.EmployeeBLL();
        IBLL.ICRM_CompanyBLL crm_BLL = new BLL.CRM_CompanyBLL();
        IBLL.ICityBLL city_BLL = new BLL.CityBLL();
        IBLL.IPoliceAccountNatureBLL pan_BLL = new BLL.PoliceAccountNatureBLL();


        /// <summary>
        /// 人员列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Id = LoginInfo.UserID;
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
        /// 查看详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Details(int id)
        {
            ViewBag.Id = id;
            return View();

        }

        /// <summary>
        /// 首次创建
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Create(string id)
        {
            return View();
        }

        /// <summary>
        /// 首次编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns> 
        [SupportFilter]
        public ActionResult BaseInfo(int id)
        {
            //得到登陆者是员工客服还是责任客服,经理级别的不可以修改
            List<CRM_CompanyToBranch> branchlist = new List<CRM_CompanyToBranch>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();
            IBLL.ICRM_CompanyToBranchBLL branchbll = new BLL.CRM_CompanyToBranchBLL();
            
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            List<CRM_Company> CompanyList = new List<CRM_Company>();
            IBLL.ICRM_CompanyBLL bll = new BLL.CRM_CompanyBLL();

            CompanyList = bll.GetCompanyList();
            SelectList selUsers = new SelectList(CompanyList, "ID", "CompanyName", null);
            ViewBag.listCompany = selUsers;
            return View();
        }

        #region 批量添加员工
        /// <summary>
        /// 批量添加员工
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult ImportEmployee()
        {
            return View();
        }

        #region  批量添加员工


        [HttpPost]
        public ActionResult ImportEmployeeByExcel(HttpPostedFileBase files)
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
                    result.Message = "导入Excel中不存在有效员工信息请核实!";
                    return Json(new { code = 0, msg = result.Message });
                }
                int SuccessNum = 0;
                if (ImportEmployeeAdd(table, ref message, out SuccessNum))
                {

                    result.Code = Common.ClientCode.Succeed;
                    result.Message = string.Format("成功导入{0}人员工信息!", table.Rows.Count);
                    return Json(new { code = 1, msg = result.Message });
                }
                else
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = message;
                    return Json(new { code = 0, msg = result.Message });

                }

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




        /// <summary>
        /// 数据库导入
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        private bool ImportEmployeeAdd(DataTable table, ref string message, out int SuccessNum)
        {
            try
            {
                StringBuilder sbError = new StringBuilder();
                SuccessNum = 0;
                if (table.Rows.Count > 0)
                {
                    #region Excel数据提取

                    //校验并转换信息
                    List<EmployeeAddExcle> employeeList = CheckImportEmployeeAdd(table, ref message);

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        return false;
                    }

                    if (emp_BLL.EmployeeListAdd(employeeList, LoginInfo.LoginName))
                    {
                        SuccessNum = employeeList.Count();
                        return true;
                    }
                    else
                    {
                        message = "系统错误！";
                        return false;
                    }


                    #endregion
                }

            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        private List<EmployeeAddExcle> CheckImportEmployeeAdd(DataTable table, ref string message)
        {
            List<EmployeeAddExcle> employeeList = new List<EmployeeAddExcle>();
            #region 数据转换
            foreach (DataRow dr in table.Rows)
            {
                EmployeeAddExcle employee = new EmployeeAddExcle();
                employee.CompanyName = dr["单位"].ToString().Trim();
                employee.City = dr["社保缴纳地"].ToString().Trim();
                employee.Empname = dr["姓名"].ToString().Trim();
                employee.CertificateNumber = dr["证件号码"].ToString().Trim();
                employee.CertificateType = dr["证件类型"].ToString().Trim();
                employee.Station = dr["岗位"].ToString().Trim();
                employee.PoliceAccountNatureName = dr["户口性质"].ToString().Trim();




                employee.AccountType = dr["户口类型"].ToString().Trim();

                employee.MobilePhone = dr["移动电话"].ToString().Trim();
                employee.Telephone = dr["固定电话"].ToString().Trim();
                employee.Address = dr["联系地址"].ToString().Trim();
                employee.Email = dr["邮箱"].ToString().Trim();


                employee.AccountName = dr["开户名称"].ToString().Trim();
                employee.BranchBank = dr["支行名称"].ToString().Trim();
                employee.Bank = dr["开户银行"].ToString().Trim();
                employee.Account = dr["账号"].ToString().Trim();

                employeeList.Add(employee);
            }
            #endregion

            string search = string.Empty;
            StringBuilder errMessageAll = new StringBuilder();

            string DataForm = "证件号:{0},姓名:{1},的异常信息为:{2}\r\n";
            Regex r = new Regex(@"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$");
            foreach (EmployeeAddExcle item in employeeList)
            {
                StringBuilder errMessage = new StringBuilder();

                #region 基础信息赋值
                if (string.IsNullOrEmpty(item.CertificateNumber))
                {
                    errMessage.Append("证件号码为空;\r\n");
                }
                else if (!CardCommon.CheckCardID18(item.CertificateNumber))
                {
                    errMessage.Append("身份证号不合法：" + item.CertificateNumber + ";\r\n");
                }
                if (string.IsNullOrEmpty(item.Empname))
                {
                    errMessage.Append("姓名为空;\r\n");
                }
                if (string.IsNullOrEmpty(item.CompanyName))
                {
                    errMessage.Append("单位为空;\r\n");
                }
                else
                {
                    item.CompanyId = getCompanyIdByExcel(item, ref errMessage);
                }
                if (string.IsNullOrEmpty(item.City))
                {
                    errMessage.Append("社保缴纳地为空;\r\n");
                }
                else
                {
                    item.City = getCityIdByExcel(item, ref errMessage);
                }
                if (string.IsNullOrEmpty(item.CertificateType))
                {
                    errMessage.Append("证件类型为空;\r\n");
                }
                else
                {
                    CertificateType certificateType = new CertificateType();
                    if (!Enum.TryParse<CertificateType>(item.CertificateType, out certificateType))
                    {
                        errMessage.Append("证件类型不正确;\r\n");
                    }
                }
                if (string.IsNullOrEmpty(item.Station))
                {
                    errMessage.Append("岗位为空;\r\n");
                }
                if (string.IsNullOrEmpty(item.PoliceAccountNatureName))
                {
                    errMessage.Append("户口性质为空;\r\n");
                }
                else
                {
                    item.PoliceAccountNature = getPoliceAccountNatureIdByExcel(item, ref errMessage);
                }
                if (string.IsNullOrEmpty(item.MobilePhone) && string.IsNullOrEmpty(item.Telephone))
                {
                    errMessage.Append("固定电话或者移动电话至少填一项;\r\n");
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.MobilePhone))
                    {
                        if (!Regex.IsMatch(item.MobilePhone, @"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$"))
                        {
                            errMessage.Append("移动电话格式不正确;\r\n");
                        };
                    }

                    if (!string.IsNullOrEmpty(item.Telephone))
                    {
                        if (!Regex.IsMatch(item.Telephone, @"(0[0-9]{2,3})-([2-9][0-9]{6,7})$"))
                        {
                            errMessage.Append("固定电话格式不正确;\r\n");
                        };
                    }
                }

                if (!string.IsNullOrEmpty(item.Email))
                {
                    if (!Regex.IsMatch(item.Email, @"(\w)+(\.\w+)*@(\w)+((\.\w+)+)"))
                    {
                        errMessage.Append("邮箱格式不正确;\r\n");
                    };
                }

                if (!(string.IsNullOrEmpty(item.AccountName) && string.IsNullOrEmpty(item.Bank) && string.IsNullOrEmpty(item.BranchBank) && string.IsNullOrEmpty(item.Account)))
                {
                    if (string.IsNullOrEmpty(item.AccountName) || string.IsNullOrEmpty(item.Bank) || string.IsNullOrEmpty(item.BranchBank) || string.IsNullOrEmpty(item.Account))
                    {
                        errMessage.Append("请完善银行信息;\r\n");
                    }
                }





                #endregion
                if (!string.IsNullOrWhiteSpace(errMessage.ToString()))
                {
                    errMessageAll.Append(string.Format(DataForm, item.CertificateNumber, item.Empname, errMessage.ToString()));
                }
            }
            message = errMessageAll.ToString();
            return employeeList;
        }

        #region 判断
        private int getPoliceAccountNatureIdByExcel(EmployeeAddExcle item, ref StringBuilder Message)
        {
            string search = "NameDDL_String&" + item.PoliceAccountNatureName;
            List<PoliceAccountNature> PoliceAccountNatureList = pan_BLL.GetByParam(0, "ASC", "Id", search);
            if (PoliceAccountNatureList == null || PoliceAccountNatureList.Count <= 0)
            {
                Message.Append("户口性质在系统不存在;\r\n");
            }
            else if (PoliceAccountNatureList.Count == 1)
            {

                return PoliceAccountNatureList[0].Id;
            }
            else
            {
                Message.Append("户口性质在系统中重复;\r\n");
            }
            return -1;
        }

        private string getCityIdByExcel(EmployeeAddExcle item, ref StringBuilder Message)
        {
            string search = "NameDDL_String&" + item.City;
            List<City> CityList = city_BLL.GetByParam(string.Empty, "ASC", "Id", search);
            if (CityList == null || CityList.Count <= 0)
            {
                Message.Append("社保缴纳地在系统不存在;\r\n");

            }
            else if (CityList.Count == 1)
            {
                //组建员工社保地关系信息
                return CityList[0].Id;
            }
            else
            {
                Message.Append("社保缴纳地在系统重复;\r\n");
            }
            return null;
        }

        private int getCompanyIdByExcel(EmployeeAddExcle item, ref StringBuilder Message)
        {
            string search = "CompanyNameDDL_String&" + item.CompanyName;
            List<CRM_Company> empComList = crm_BLL.GetByParam(string.Empty, "ASC", "Id", search);
            if (empComList == null || empComList.Count <= 0)
            {
                //系统中不存在报增公司,抛出异常
                Message.Append("单位在系统不存在;\r\n");
            }
            else if (empComList.Count == 1)
            {
                //组建员工企业关系信息
                return empComList[0].ID;
            }
            else
            {
                Message.Append("单位在系统重复;\r\n");
            }
            return -1;
        }

        #endregion


        #endregion


        #endregion
    }
}


