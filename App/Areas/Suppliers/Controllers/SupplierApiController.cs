using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using System.Web.Script.Serialization;
using Langben.DAL.Model;
using System.Web.Mvc;
using System.Reflection;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections;
using System.Linq.Expressions;



namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierApiController : BaseApiController
    {
        #region Get
        /// <summary>
        /// 根据ID获取供应商基本信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string Get(int id)
        {
            Supplier model = m_BLL.GetById(id);
            SupplierView viewModel = new SupplierView();
            if (model != null)
            {
                Type t1 = typeof(Supplier);
                PropertyInfo[] propertys1 = t1.GetProperties();
                Type t2 = typeof(SupplierView);
                PropertyInfo[] propertys2 = t2.GetProperties();

                foreach (PropertyInfo pi in propertys2)
                {
                    string[] arrField = new string[] { "Id", "Code", "Name", "OrganizationCode", "RegisterAddress", "OfficeAddress", "CustomerServiceId", "Status", "CreateTime", "CreateUserID", "CreateUserName" };//排除的字段
                    string name = pi.Name;
                    if (arrField.Contains(name))
                    {
                        object value = t1.GetProperty(name).GetValue(model, null);
                        t2.GetProperty(name).SetValue(viewModel, value, null);
                    }
                }
                string cityIDList = string.Empty;
                foreach (var city in model.SupplierNatureCity)
                {
                    cityIDList += city.NatureCityId + ",";
                }
                viewModel.NatureCityId = cityIDList.Trim(',');
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(viewModel);
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            int intBranchID = LoginInfo.BranchID;
            string strCompanyName = "";
            string strUserID_KF = "";
            int? intUserID_KF = null;
            string menuID = "";

            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                strCompanyName = search[0];
                strUserID_KF = search[1];
            }
            if (!string.IsNullOrEmpty(strUserID_KF))
            {
                intUserID_KF = Convert.ToInt32(strUserID_KF);
            }

            List<SupplierView> queryData = m_BLL.GetSupplierList(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_KF, intBranchID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        #region  根据缴纳地获取供应商
        /// <summary>
        /// 根据缴纳地获取供应商
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetSuppliers(string id)
        {
            List<Supplier> list = m_BLL.GetSupplierByCity(id);

            var data = new Common.ClientResult.DataResult
            {
                total = list.Count,
                rows = list.Select(s => new
                {
                    ID = s.Id,
                    Name = s.Name
                })
            };

            return data;
        }

        #endregion
        #region  获取所有供应商
        /// <summary>
        /// 获取所有供应商
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetAllSuppliers()
        {
            List<Supplier> list = m_BLL.GetAll();

            var data = new Common.ClientResult.DataResult
            {
                total = list.Count,
                rows = list.Select(s => new
                {
                    ID = s.Id,
                    Name = s.Name
                })
            };

            return data;
        }

        #endregion


        #endregion

        #region Post
        #region 添加供应商相关方法
        //创建新供应商
        [System.Web.Http.HttpPost]

        public Common.ClientResult.Result PostNewSupplier([FromBody]SupplierInfo entity)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                //基本信息   
                Supplier baseModel = entity.BasicInfo;
                baseModel.CreateTime = DateTime.Now;
                baseModel.CreateUserID = LoginInfo.UserID;
                baseModel.CreateUserName = LoginInfo.RealName;
                baseModel.Status = Common.Status.启用.ToString();

                //联系人信息
                List<SupplierLinkMan> listLink = new List<SupplierLinkMan>();
                string linkMan = entity.LinkMan;
                if (!string.IsNullOrEmpty(linkMan) && linkMan != "[]")
                {
                    listLink = GetLinkList(linkMan);
                }
                //银行信息
                List<SupplierBankAccount> listBank = new List<SupplierBankAccount>();
                string bank = entity.Bank;
                if (!string.IsNullOrEmpty(bank) && bank != "[]")
                    listBank = GetBankList(bank);
                //财务信息开票
                List<SupplierBill> listBill = new List<SupplierBill>();
                string bill = entity.Bill;
                if (!string.IsNullOrEmpty(bill))
                    listBill = GetBillList(bill);

                //报价
                List<LadderLowestPrice> listPrice = new List<LadderLowestPrice>();
                string price = entity.Price;
                if (!string.IsNullOrEmpty(price) && price != "[]")
                    listPrice = GetPriceList(price);
                //阶梯报价
                List<LadderPrice> listLadderPrice = new List<LadderPrice>();
                string ladderPrice = entity.LadderPrice;
                if (!string.IsNullOrEmpty(ladderPrice) && ladderPrice != "[]")
                    listLadderPrice = GetLadderPriceList(ladderPrice);
                //缴纳地
                List<SupplierNatureCity> listCity = new List<SupplierNatureCity>();
                string city = entity.NatureCityId;
                if (!string.IsNullOrEmpty(entity.NatureCityId))
                {
                    listCity = GetCityList(city);
                }

                string returnValue = string.Empty;
                if (m_BLL.CreateNewSupplier(ref validationErrors, baseModel, listLink, listBank, listBill, listPrice, listLadderPrice, listCity))
                {
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
                    //LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业信息_待审核的信息，" + returnValue, "客户_企业信息_待审核"
                    //    );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    return result; //提示插入失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        #endregion

        /// <summary>
        /// 修改供应商基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Common.ClientResult.Result PutBasic([FromBody]SupplierView model)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (model != null && ModelState.IsValid)
            {

                string returnValue = string.Empty;
                if (m_BLL.EditSupplier(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，供应商管理_供应商的Id为" + model.Id, "修改供应商管理基本信息"
                        );//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，供应商管理，" + returnValue, "修改供应商管理基本信息"
                        );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public Common.ClientResult.Result Delete(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int?[] SupplierId = Array.ConvertAll<string, int?>(query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (SupplierId != null && SupplierId.Length > 0)
            {
                if (m_BLL.DeleteSupplier(ref validationErrors, SupplierId))
                {
                    LogClassModels.WriteServiceLog("删除成功" + "，供应商的Id为" + string.Join(",", SupplierId), "消息"
                        );//删除成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "删除成功";
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
                    LogClassModels.WriteServiceLog("审核通过失败" + "，供应商的Id为" + string.Join(",", SupplierId) + "," + returnValue, "消息"
                        );//删除失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "删除失败" + returnValue;
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 分配企业责任销售
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public Common.ClientResult.Result Services(string supplierId, int searchUser)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            int?[] SupplierId = Array.ConvertAll<string, int?>(supplierId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            string returnValue = string.Empty;
            try
            {
                if (SupplierId != null && SupplierId.Length > 0)
                {
                    if (m_BLL.UpdateCustomerService(ref validationErrors, SupplierId, searchUser))
                    {
                        LogClassModels.WriteServiceLog("设置成功" + "，供应商的Id为" + string.Join(",", SupplierId), "消息"
                          );//设置成功，写入日志
                        result.Code = Common.ClientCode.Succeed;
                        result.Message = "设置成功";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                if (validationErrors != null && validationErrors.Count > 0)
                {
                    validationErrors.All(a =>
                    {
                        returnValue += a.ErrorMessage;
                        return true;
                    });
                }
                LogClassModels.WriteServiceLog("设置失败" + "，供应商的Id为" + string.Join(",", SupplierId), "消息"
                          );//设置失败，写入日志                  
                result.Code = Common.ClientCode.Fail;
                result.Message = Suggestion.InsertFail + returnValue;
                return result; //提示插入失败
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        #region 获取当前客服服务人数
        /// <summary>
        /// 获取当前客服服务人数
        /// </summary>
        /// <param name="UserID_YG">客服人员id</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public string Getcount(string UserID_GYSKF)
        {
            int userid = 0;
            int.TryParse(UserID_GYSKF, out userid);

            int count = m_BLL.GetSupplierByCustomerServiceId(userid);
            return count.ToString();
        }
        #endregion

        #region 费用表对比
        //导出Excel
        public string PostExportToExcel(int supplierCostID, int serverCostID,string supplierName)
        {
            IBLL.ICOST_CostTableBLL c_BLL = new BLL.COST_CostTableBLL();
            List<Cost_CostTableDetails> supplierData = c_BLL.GetCostFeeDetailList(supplierCostID);//供应商
            List<Cost_CostTableDetails> serverData = c_BLL.GetCostFeeDetailList(serverCostID);//客服
            List<Cost_CostTableDetails> allData = supplierData.Union(serverData).ToList();//全部数据

            List<ErrorColumn> errorList = new List<ErrorColumn>();

            foreach (var item in allData.Where(e=>e.Flag !="1"))//循环供应商上传的费用
            {
                var otherModel = allData.Find(e => (e.Employee_ID == item.Employee_ID && e.PaymentStyle == item.PaymentStyle && e.CityName == item.CityName && e.CreateFrom != item.CreateFrom));//对应的数据
                
                if (otherModel != null)
                {
                    string empID = otherModel.Employee_ID.ToString();
                    otherModel.Flag = "1";//标识为已循环
                    Type t1 = typeof(Cost_CostTableDetails);
                    PropertyInfo[] propertys1 = t1.GetProperties();
                    //比较费用
                    foreach (PropertyInfo pi in propertys1)
                    {
                        string name = pi.Name;
                        if (name != "CreateFrom" && name != "COST_CostTable_ID" && name != "Flag")
                        {
                            object serverValue = t1.GetProperty(name).GetValue(otherModel, null) == null ? "" : t1.GetProperty(name).GetValue(otherModel, null);
                            object supplierValue = t1.GetProperty(name).GetValue(item, null) == null ? "" : t1.GetProperty(name).GetValue(item, null);
                            if (!serverValue.Equals(supplierValue))
                            {
                                ErrorColumn errorModel = new ErrorColumn();
                                errorModel.ID = empID;
                                errorModel.Column = name;
                                errorList.Add(errorModel);
                            }
                        }
                    }
                }
                else//只存在一方的数据来源
                {
                    Type t1 = typeof(Cost_CostTableDetails);
                    PropertyInfo[] propertys1 = t1.GetProperties();
                    foreach (PropertyInfo pi in propertys1)
                    {
                        string name = pi.Name;
                        ErrorColumn errorModel = new ErrorColumn();
                        errorModel.ID = item.Employee_ID.ToString();
                        errorModel.Column = name;
                        errorList.Add(errorModel);
                    }
                }
            }
            string urlPath = string.Empty;
            if (errorList.Count != 0) //费用对比有差异
            {
                urlPath = ExportToExcel(allData, supplierName, errorList);
            }
 
            return urlPath;
        }
        #endregion
        #endregion


        #region 内置

        //验证用户名唯一
        public Common.ClientResult.Result CheckSupplierName(string supplierID, string supplierName)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            int count = m_BLL.CheckSupplierName(supplierID, supplierName);
            if (count == 0)
            {
                result.Code = Common.ClientCode.FindNull;
                result.Message = "该供应商名可以使用！";
            }
            else
            {
                result.Code = Common.ClientCode.Succeed;
                result.Message = "该供应商名已存在！";
            }
            return result;
        }

        //获得联系人信息
        private List<SupplierLinkMan> GetLinkList(string linkMan)
        {
            List<SupplierLinkMan> list = new List<SupplierLinkMan>();
            if (!string.IsNullOrEmpty(linkMan))
            {
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierLinkMan>>(linkMan);
                for (int i = 0; i < jsonData.Count; i++)
                {
                    SupplierLinkMan model = new SupplierLinkMan();
                    model.Name = jsonData[i].Name;
                    model.Position = jsonData[i].Position;
                    model.Address = jsonData[i].Address;
                    model.Mobile = jsonData[i].Mobile;
                    model.Telephone = jsonData[i].Telephone;
                    model.Email = jsonData[i].Email;
                    model.IsDefault = jsonData[i].IsDefault;
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID.ToString();
                    model.CreateUserName = LoginInfo.RealName;
                    model.Status = Common.Status.启用.ToString();
                    list.Add(model);
                }
            }
            return list;
        }
        //获得银行信息
        private List<SupplierBankAccount> GetBankList(string bank)
        {
            List<SupplierBankAccount> list = new List<SupplierBankAccount>();
            if (!string.IsNullOrEmpty(bank))
            {
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierBankAccount>>(bank);
                for (int i = 0; i < jsonData.Count; i++)
                {
                    SupplierBankAccount model = new SupplierBankAccount();
                    model.Bank = jsonData[i].Bank;
                    model.Account = jsonData[i].Account;
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.Status = Common.Status.启用.ToString();
                    list.Add(model);
                }
            }
            return list;
        }
        //获得开票信息
        private List<SupplierBill> GetBillList(string bill)
        {
            List<SupplierBill> list = new List<SupplierBill>();
            if (!string.IsNullOrEmpty(bill))
            {
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierBill>>(bill);
                for (int i = 0; i < jsonData.Count; i++)
                {
                    SupplierBill model = new SupplierBill();
                    model.PayName = jsonData[i].PayName;
                    model.BillName = jsonData[i].BillName;
                    model.TaxRegistryNumber = jsonData[i].TaxRegistryNumber;
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.Status = Common.Status.启用.ToString();
                    list.Add(model);
                }
            }
            return list;
        }
        //获得报价信息
        private List<LadderLowestPrice> GetPriceList(string price)
        {
            List<LadderLowestPrice> list = new List<LadderLowestPrice>();
            if (!string.IsNullOrEmpty(price))
            {
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LadderLowestPrice>>(price);
                for (int i = 0; i < jsonData.Count; i++)
                {
                    LadderLowestPrice model = new LadderLowestPrice();
                    model.ProductId = jsonData[i].ProductId;
                    model.LowestPrice = jsonData[i].LowestPrice;
                    model.AddPrice = jsonData[i].AddPrice;
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.Status = Common.Status.启用.ToString();
                    list.Add(model);
                }
            }
            return list;
        }
        //获得阶梯报价信息
        private List<LadderPrice> GetLadderPriceList(string ladderPrice)
        {
            List<LadderPrice> list = new List<LadderPrice>();
            if (!string.IsNullOrEmpty(ladderPrice))
            {
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LadderPrice>>(ladderPrice);
                for (int i = 0; i < jsonData.Count; i++)
                {
                    LadderPrice model = new LadderPrice();
                    model.SinglePrice = jsonData[i].SinglePrice;
                    model.BeginLadder = jsonData[i].BeginLadder;
                    model.EndLadder = jsonData[i].EndLadder;
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.Status = Common.Status.启用.ToString();
                    list.Add(model);
                }
            }
            return list;
        }
        //获得缴纳地
        private List<SupplierNatureCity> GetCityList(string city)
        {
            List<SupplierNatureCity> list = new List<SupplierNatureCity>();
            if (!string.IsNullOrEmpty(city))
            {
                string[] arrCity = city.Split(',');
                for (int i = 0; i < arrCity.Count(); i++)
                {
                    SupplierNatureCity model = new SupplierNatureCity();
                    model.NatureCityId = arrCity[i];
                    list.Add(model);
                }
            }
            return list;
        }

        public List<ORG_User> getUserbyCityData(string menuID, string citycode)
        {
            List<ORG_User> UserList = new List<ORG_User>();
            IBLL.IORG_UserBLL bll = new BLL.ORG_UserBLL();

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

            UserList = bll.GetGroupUsers(Common.ORG_Group_Code.GYSKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);

            List<int> temArray = new List<int>();
            foreach (ORG_User item in UserList)
            {
                temArray.Add(item.ID);
            }
            int[] intArray = temArray.ToArray();

            SysEntities db = new SysEntities();

            var usrquery = db.ORG_User.Where(o => intArray.Contains(o.ID));
            var query = db.ORG_UserCity.Where(o => o.CityId == citycode && intArray.Contains(o.UserID));
            var query1 = (from a in usrquery join b in query on a.ID equals b.UserID select a).ToList<ORG_User>();
            return query1;
        }

        //导出费用对比Excel表
        private string ExportToExcel(List<Cost_CostTableDetails> queryData, string supplierName,List<ErrorColumn> errorList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                queryData = queryData.OrderBy(o => o.EmployName).ThenBy(o => o.CreateFrom).ToList();
                IBLL.ICOST_CostTableBLL c_BLL = new BLL.COST_CostTableBLL();
                FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Template/Excel/费用对比导出模板.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                hssfworkbook.SetSheetName(0, "费用明细");

                ISheet sheet1 = hssfworkbook.GetSheetAt(0);
                InsertRowsdaili2(sheet1, 7, queryData.Count() - 1);

                //上色
                ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderBottom = BorderStyle.Thin;//边框
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.Alignment = HorizontalAlignment.Center;//水平居中
                IFont cellFont = hssfworkbook.CreateFont();
                cellFont.Color = (short)FontColor.Red;
                cellStyle.SetFont(cellFont);


                for (int i = 0; i < queryData.Count(); i++)
                {
                    List<ErrorColumn> singleEmployee = new List<ErrorColumn>();
                    string employeeID = queryData[i].Employee_ID.ToString();
                    singleEmployee = errorList.Where(o => o.ID == employeeID).ToList();
                    IRow row = sheet1.GetRow(7 + i);
                    row.GetCell(0).SetCellValue(((Common.CostTable_CreateFrom)queryData[i].CreateFrom).ToString());
                    row.GetCell(1).SetCellValue(queryData[i].EmployName);
                    row.GetCell(2).SetCellValue(queryData[i].CertificateNumber);  // 身份证号
                    row.GetCell(3).SetCellValue(queryData[i].SupplierName);  // 供应商
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
                    // 循环给不一致数据上色标红
                    List<ExcelVSList> excelList = GetExcelVSList();
                    for (int k = 0; k < excelList.Count; k++)
                    {
                        if (singleEmployee.Select(o => o.Column).ToArray().Contains(excelList[k].Column))
                        {
                            row.GetCell(excelList[k].Cell).CellStyle = cellStyle;
                        }
                    }

                }
                IRow LastRow = sheet1.CreateRow(9 + queryData.Count());
                LastRow.CreateCell(0);
                LastRow.GetCell(0).SetCellValue("");
                sheet1.ForceFormulaRecalculation = true;
                string fileName = supplierName + "_" + "费用对比差异详情" + ".xls";
                string urlPath = "/DataExport/" + fileName; // 文件下载的URL地址，供给前台下载
                string filePath = System.Web.HttpContext.Current.Server.MapPath("\\" + urlPath); // 文件路径

                file = new FileStream(filePath, FileMode.Create);
                hssfworkbook.Write(file);
                file.Close();

                return urlPath;  // 导出成功
            }
        }
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
                    targetSheet.GetRow(rowIndex).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}", rowIndex + 1);//保险合计
                    targetSheet.GetRow(rowIndex).GetCell(45).CellFormula = string.Format("AM{0}+AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 1);//费用合计

                    //合计行
                    //if (rowIndex == fromRowIndex + rowCount)
                    //{
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(9).CellFormula = string.Format("SUM(J{0}:J{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(10).CellFormula = string.Format("SUM(K{0}:K{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(15).CellFormula = string.Format("SUM(P{0}:P{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(16).CellFormula = string.Format("SUM(Q{0}:Q{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(20).CellFormula = string.Format("SUM(U{0}:U{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(24).CellFormula = string.Format("SUM(Y{0}:Y{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(25).CellFormula = string.Format("SUM(Z{0}:Z{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(27).CellFormula = string.Format("SUM(AB{0}:AB{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(28).CellFormula = string.Format("SUM(AC{0}:AC{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(29).CellFormula = string.Format("SUM(AD{0}:AD{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(33).CellFormula = string.Format("SUM(AH{0}:AH{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(34).CellFormula = string.Format("SUM(AI{0}:AI{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(36).CellFormula = string.Format("SUM(AK{0}:AK{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(37).CellFormula = string.Format("SUM(AL{0}:AL{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(38).CellFormula = string.Format("SUM(AM{0}:AM{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(39).CellFormula = string.Format("SUM(AN{0}:AN{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(43).CellFormula = string.Format("SUM(AR{0}:AR{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(44).CellFormula = string.Format("SUM(AS{0}:AS{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);

                    //    targetSheet.GetRow(rowIndex + 1).GetCell(11).CellFormula = string.Format("J{0}+K{0}", rowIndex + 2);//养老保险小计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(17).CellFormula = string.Format("P{0}+Q{0}", rowIndex + 2);//失业保险小计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(26).CellFormula = string.Format("Y{0}+Z{0}", rowIndex + 2);//医疗保险小计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(35).CellFormula = string.Format("AH{0}+AI{0}", rowIndex + 2);//住房公积金小计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(40).CellFormula = string.Format("J{0}+P{0}+U{0}+Y{0}+AB{0}+AD{0}+AH{0}+AK{0}", rowIndex + 2);//单位保险小计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(41).CellFormula = string.Format("K{0}+Q{0}+Z{0}+AC{0}+AI{0}+AL{0}", rowIndex + 2);//个人保险小计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}", rowIndex + 2);//保险合计
                    //    targetSheet.GetRow(rowIndex + 1).GetCell(45).CellFormula = string.Format("AM{0}+AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 2);//费用合计
                    //}
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
                targetSheet.GetRow(rowIndex).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}", rowIndex + 1);//保险合计
                targetSheet.GetRow(rowIndex).GetCell(45).CellFormula = string.Format("AM{0}+AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 1);//费用合计

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
                targetSheet.GetRow(rowIndex + 1).GetCell(42).CellFormula = string.Format("AO{0}+AP{0}", rowIndex + 2);//保险合计
                targetSheet.GetRow(rowIndex + 1).GetCell(45).CellFormula = string.Format("AM{0}+AN{0}+AQ{0}+AR{0}+AS{0}", rowIndex + 2);//费用合计
            }
        }
        //得到属性名
        private string GetPropertyName<T>(Expression<Func<Cost_CostTableDetails,T>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }
        //Excel单元格对应的字段名
        private List<ExcelVSList> GetExcelVSList()
        {
            List<ExcelVSList> list = new List<ExcelVSList>(){
            new ExcelVSList{Cell=1,Column="EmployName"},
            new ExcelVSList{Cell=2,Column="CertificateNumber"},
            new ExcelVSList{Cell=3,Column="SupplierName"},
            new ExcelVSList{Cell=4,Column="PaymentStyle"},
            new ExcelVSList{Cell=5,Column="CityName"},

            new ExcelVSList{Cell=6,Column="YanglaoPaymentInterval"},
            new ExcelVSList{Cell=7,Column="YanglaoPaymentMonth"},
            new ExcelVSList{Cell=8,Column="YanglaoCompanyRadix"},
            new ExcelVSList{Cell=9,Column="YanglaoCompanyCost"},
            new ExcelVSList{Cell=10,Column="YanglaoPersonCost"},

            new ExcelVSList{Cell=12,Column="ShiyePaymentInterval"},
            new ExcelVSList{Cell=13,Column="ShiyePaymentMonth"},
            new ExcelVSList{Cell=14,Column="ShiyeCompanyRadix"},
            new ExcelVSList{Cell=15,Column="ShiyeCompanyCost"},
            new ExcelVSList{Cell=16,Column="ShiyePersonCost"},
   
            new ExcelVSList{Cell=18,Column="GongshangPaymentInterval"},
            new ExcelVSList{Cell=19,Column="GongshangCompanyRadix"},
            new ExcelVSList{Cell=20,Column="GongshangCompanyCost"},

            new ExcelVSList{Cell=21,Column="YiliaoPaymentInterval"},
            new ExcelVSList{Cell=22,Column="YiliaoPaymentMonth"},
            new ExcelVSList{Cell=23,Column="YiliaoCompanyRadix"},
            new ExcelVSList{Cell=24,Column="YiliaoCompanyCost"},
            new ExcelVSList{Cell=25,Column="YiliaoPersonCost"},

            new ExcelVSList{Cell=27,Column="DaeCompanyCost"},
            new ExcelVSList{Cell=28,Column="DaePersonCost"},

            new ExcelVSList{Cell=29,Column="ShengyuCompanyCost"},

            new ExcelVSList{Cell=30,Column="GongjijinPaymentInterval"},
            new ExcelVSList{Cell=31,Column="GongjijinPaymentMonth"},
            new ExcelVSList{Cell=32,Column="GongjijinCompanyRadix"},
            new ExcelVSList{Cell=33,Column="GongjijinCompanyCost"},
            new ExcelVSList{Cell=34,Column="GongjijinPersonCost"},

            new ExcelVSList{Cell=36,Column="GongjijinBCCompanyCost"},
            new ExcelVSList{Cell=37,Column="GongjijinBCPersonCost"},

            new ExcelVSList{Cell=38,Column="OtherInsuranceCost"},
            new ExcelVSList{Cell=39,Column="OtherCost"},

            new ExcelVSList{Cell=43,Column="ProductionCost"},
            new ExcelVSList{Cell=44,Column="ServiceCost"}
            };

            return list;
        }
        //自定义类 费用对比
        private class ErrorColumn
        {
            public string ID { get; set; }
            public string Column { get; set; }
        }
        //Excel的列号与对应的model列名
        private class ExcelVSList
        {
            public int Cell { get; set; }
            public string Column { get; set; }
        }
        #endregion

        IBLL.ISupplierBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public SupplierApiController()
            : this(new SupplierBLL()) { }

        public SupplierApiController(SupplierBLL bll)
        {
            m_BLL = bll;
        }
    }
}
