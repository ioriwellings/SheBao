using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Models;

namespace Langben.App.Controllers
{
    public class COST_PayCreateController : BaseController
    {
        //
        // GET: /COST_PayCreate/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel(string yearM, string suppliersid)
        {
            using (SysEntities db = new SysEntities())
            {
                try
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();

                    string outMsg = "", errorMsg = "";
                    //获取要导入的年月
                    int yearMonth = Convert.ToInt32(yearM);
                    int suppliersId = Convert.ToInt32(suppliersid);
                    //获取文件
                    HttpPostedFileBase file = Request.Files["files"];

                    string path = Server.MapPath("../Uploads/Excel/");
                    string savePath = Util_XLS.UpLoadXls(file, path, out outMsg);
                    if (outMsg != "")
                    {
                        return Json(new { Code = 0, Message = outMsg });
                    }
                    #region 弃用
                    //if (file == null || file.ContentLength <= 0)
                    //{
                    //    ViewBag.error = "文件不能为空";
                    //    return View();
                    //}
                    //else
                    //{
                    //    string filename = Path.GetFileName(file.FileName);
                    //    int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                    //    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                    //    string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                    //    int Maxsize = 4000 * 1024;//定义上传文件的最大空间大小为4M
                    //    string FileType = ".xls,.xlsx";//定义上传文件的类型字符串

                    //    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    //    if (!FileType.Contains(fileEx))
                    //    {
                    //        ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                    //        return View();
                    //    }
                    //    if (filesize >= Maxsize)
                    //    {
                    //        ViewBag.error = "上传文件超过4M，不能上传";
                    //        return View();
                    //    }
                    //    string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/Excel/";
                    //    savePath = Path.Combine(path, FileName);
                    //    file.SaveAs(savePath);
                    //}

                    // string strConn;
                    //// strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + savePath + ";" + "Extended Properties=Excel 8.0";
                    // strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
                    // OleDbConnection conn = new OleDbConnection(strConn);
                    // conn.Open();

                    // string SheetName = yearMonth + "$";
                    // string strSQL = string.Format("select * from [{0}]", SheetName);
                    // OleDbDataAdapter myCommand = new OleDbDataAdapter(strSQL, strConn);

                    // DataSet myDataSet = new DataSet();
                    // // myCommand.Fill(myDataSet, "SelectResult");
                    // try
                    // {
                    //     myCommand.Fill(myDataSet, "SelectResult");
                    // }
                    // catch (Exception ex)
                    // {
                    //     ViewBag.error = ex.Message + "导入失败,请认真检查excel";
                    //     return Json(ViewBag.error);
                    // }
                    // DataTable table = myDataSet.Tables["SelectResult"].DefaultView.ToTable();
                    #endregion

                    string SheetName = Util_XLS.ConvertToSQLSheetName(yearMonth.ToString());
                    // string strSQL = string.Format("select * from {0}", SheetName);
                    DataTable dt = Util_XLS.NpoiReadExcle(savePath, SheetName, true, out outMsg);
                    if (outMsg != "")
                    {
                        return Json(new { Code = 0, Message = outMsg });
                    }
                    //验证Excel信息
                    List<COST_PayTemporary> list = new List<COST_PayTemporary>();
                    //验证
                    string guid = Common.Result.GetNewId();
                    string str = verification(db, dt, yearMonth, guid, out list);
                    if (str != "")
                    {
                        return Json(new { Code = 0, Message = str });
                    }
                    else
                    {
                        //批量插入数据库
                        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["connStr"].ToString();
                        string[] st = { "ID","YearMonth", "CreateTime", "CreateUserID", "CreateUserName", "BranchID", "PersonName", "CardId", "PersonId", 
                                  "CompanyId", "CompanyName", "Suppliers", "CityId", "PaymentSocialMonthYL", 
                                  "RadixYL", "CompanyCosYL", "PersonCostYL", "PaymentSocialMonthSY", "RadixSY", "CompanyCostSY", "PersonCostSY", "PaymentSocialMonthGS",
                                  "RadixGS", "CompanyCostGS", "PaymentSocialMonthYiL", "RadixYil", "CompanyCostYil", "PersonCostYil", "CompanyCostYilMax",
                                  "PersonCostYilMax", "CompanyCostShengY", "PaymentSocialMonthGJJ", "RadixGJJ", "CompanyCostGJJ", "PersonCostGJJ",
                                  "PaymentSocialMonthBCGJJ", "RadixBCGJJ", "CompanyCostBCGJJ", "PersonCostBCGJJ", "PayOtherSocial", "PayOther","YanglaoPayFee","YiliaoPayFee","ShiyePayFee","GongshangPayFee","GongjijinPayFee","ShengYuPayFee","BatchGuid",
                                  "Remark","SuppliersId","CardIdSB"
                              };
                        Util_XLS.BulkInsert(connString, "COST_PayTemporary", list, st);


                        //把数据分到不同的表中
                        Database dbEntity = EnterpriseLibraryContainer.Current.GetInstance<Database>("connStr");
                        DbCommand cmd = dbEntity.GetStoredProcCommand("FillPayTable");
                        dbEntity.AddInParameter(cmd, "@yearMonth", DbType.Int32, yearMonth);
                        dbEntity.AddInParameter(cmd, "@batchGuid", DbType.String, guid);
                        dbEntity.AddInParameter(cmd, "@suppliersId", DbType.Int32, suppliersId);
                        dbEntity.AddInParameter(cmd, "@userId", DbType.Int32, LoginInfo.UserID);
                        dbEntity.AddInParameter(cmd, "@userName", DbType.String, LoginInfo.UserName);
                        dbEntity.AddInParameter(cmd, "@branchId", DbType.String, LoginInfo.BranchID);
                        dbEntity.AddParameter(cmd, "@return", DbType.String, ParameterDirection.ReturnValue, "return", DataRowVersion.Default, "");
                        cmd.CommandTimeout = 0;
                        dbEntity.ExecuteNonQuery(cmd);
                        string result = dbEntity.GetParameterValue(cmd, "@return").ToString();
                        if (result == "1")
                        {
                            return Json(new { Code = 1, Message = "导入成功" });
                        }
                        else
                        {
                            return Json(new { Code = 1, Message = "导入失败" });
                        }

                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Code = 1, Message = "导入失败" + ex.ToString() });
                }
            }

        }

        #region 导入验证
        private string verification(SysEntities db, DataTable dt, int yearMonth, string guid, out List<COST_PayTemporary> list)
        {
            List<COST_PayTemporary> cpt_List = new List<COST_PayTemporary>();

            StringBuilder ErrorList = new StringBuilder();

            try
            {
                string cityFirst = "";
                for (int i = 3; i < dt.Rows.Count; i++)
                {
                    StringBuilder Error = new StringBuilder();
                    DataRow dr = dt.Rows[i];
                    //先创建一个实体COST_PayTemporary
                    COST_PayTemporary cpt = new COST_PayTemporary();
                    cpt.BatchGuid = guid;

                    string hanghao = "(第" + (i - 2) + "行)";
                    var cardId = dr["身份证号"].ToString().Trim();
                    var cardIdSB = dr["社保局身份证号"].ToString().Trim();
                    var personName = dr["姓名"].ToString().Trim();
                    var suppliers = dr["供应商"].ToString().Trim();
                    var cityName = dr["保险缴纳地"].ToString().Trim();

                    // 将第一个缴纳地赋给cityFirst，缴纳地只能为同一地区，用于判断缴纳地是否一致
                    if (i == 3)
                    {
                        cityFirst = cityName;

                        //判断缴纳地 是否是改用户负责的缴纳地
                        int uid = LoginInfo.UserID;
                        var usercitylist = from a in db.ORG_UserCity.Where(x => x.UserID == uid)
                                           join b in db.City.Where(x => x.Name == cityFirst) on a.CityId equals b.Id into g
                                           select g;
                        if (usercitylist.Count() <= 0)
                        {
                            Error.Append("请导入您负责的保险缴纳地！\r\n");
                        }
                    }

                    var PaymentSocialMonthYL = dr["养老缴纳社保月"].ToString().Trim();
                    var RadixYL = dr["养老缴费基数"].ToString().Trim();
                    var CompanyCosYL = dr["养老企业"].ToString().Trim();
                    var PersonCostYL = dr["养老个人"].ToString().Trim();

                    var PaymentSocialMonthSY = dr["失业缴纳社保月"].ToString().Trim();
                    var RadixSY = dr["失业缴费基数"].ToString().Trim();
                    var CompanyCostSY = dr["失业企业"].ToString().Trim();
                    var PersonCostSY = dr["失业个人"].ToString().Trim();

                    var PaymentSocialMonthGS = dr["工伤缴纳社保月"].ToString().Trim();
                    var RadixGS = dr["工伤缴费基数"].ToString().Trim();
                    var CompanyCostGS = dr["工伤企业"].ToString().Trim();

                    var PaymentSocialMonthYiL = dr["医疗缴纳社保月"].ToString().Trim();
                    var RadixYil = dr["医疗缴费基数"].ToString().Trim();
                    var CompanyCostYil = dr["医疗企业"].ToString().Trim();
                    var PersonCostYil = dr["医疗个人"].ToString().Trim();
                    var CompanyCostYilMax = dr["大病企业"].ToString().Trim();
                    var PersonCostYilMax = dr["大病个人"].ToString().Trim();
                    var CompanyCostShengY = dr["生育企业"].ToString().Trim();

                    var PaymentSocialMonthGJJ = dr["公积金缴纳社保月"].ToString().Trim();
                    var RadixGJJ = dr["公积金缴费基数"].ToString().Trim();
                    var CompanyCostGJJ = dr["公积金企业"].ToString().Trim();
                    var PersonCostGJJ = dr["公积金个人"].ToString().Trim();

                    var PaymentSocialMonthBCGJJ = dr["补充公积金缴纳社保月"].ToString().Trim();
                    var RadixBCGJJ = dr["补充公积金缴费基数"].ToString().Trim();
                    var CompanyCostBCGJJ = dr["补充公积金企业"].ToString().Trim();
                    var PersonCostBCGJJ = dr["补充公积金个人"].ToString().Trim();

                    //var PayOtherSocial = dr["其他社保费"].ToString().Trim();
                    //var PayOther = dr["其他费用"].ToString().Trim();
                    var yanglaoPayFee = dr["养老工本费"].ToString().Trim();
                    var yiliaoPayFee = dr["医疗工本费"].ToString().Trim();
                    var gongshangPayFee = 0;
                    var shiyePayFee = 0;
                    var gongjijinPayFee = 0;
                    var shengyuPayFee = 0;


                    #region 判断身份证号是否合法
                    //判断身份证号是否合法
                    if (!Common.CardCommon.CheckCardID(cardId))
                    {
                        Error.Append("身份证号不合法！\r\n");
                    }
                    else
                    {
                        //判断身份证号是否存在
                        var trueEmployee = db.Employee.Where(o => o.CertificateNumber == cardId);
                        if (trueEmployee.Count() > 0)
                        {//找出对应的员工编号和员工姓名
                            var oneTrueEmployee = trueEmployee.FirstOrDefault();
                            cpt.PersonId = oneTrueEmployee.Id;
                            cpt.CardId = cardId;
                            cpt.PersonName = oneTrueEmployee.Name;
                        }
                        else
                        {
                            Error.Append("系统中不存在此身份证号！\r\n");
                        }
                    }
                    #endregion

                    #region 判断保险缴纳地
                    //判断保险缴纳地
                    if (string.IsNullOrEmpty(cityName))
                    {
                        Error.Append("保险缴纳地不能为空！\r\n");
                    }
                    else
                    {
                        if (cityName != cityFirst)
                        {
                            Error.Append("保险缴纳地只能为同一地区，请拆分后导入！\r\n");
                        }
                        //判断保险缴纳地Code
                        var city = db.City.Where(o => o.Name == cityName);
                        if (city.Any())
                        {
                            cpt.CityId = city.FirstOrDefault().Id;

                            //判断所在企业
                            var company = from a in db.CompanyEmployeeRelation.Where(o => o.CityId == cpt.CityId && o.EmployeeId == cpt.PersonId)
                                          join b in db.CRM_Company on a.CompanyId equals b.ID
                                          select new { a.CompanyId, b.CompanyName };
                            if (company.Any())
                            {
                                var firstCompany = company.FirstOrDefault();
                                cpt.CompanyId = firstCompany.CompanyId;
                                cpt.CompanyName = firstCompany.CompanyName;
                            }
                            else
                            {
                                Error.Append("此员工在此保险缴纳地下不存在社保！\r\n");
                            }
                        }
                        else
                        {
                            Error.Append("保险缴纳地不存在！\r\n");
                        }
                    }
                    #endregion

                    #region 判断养老基数，和金额
                    //如果单位承担或个人承担不为空则认为缴纳此险种
                    if (!string.IsNullOrEmpty(CompanyCosYL) || !string.IsNullOrEmpty(PersonCostYL))
                    {
                        cpt.PaymentSocialMonthYL = PaymentSocialMonthYL;
                        if (!string.IsNullOrEmpty(RadixYL))
                        {
                            if (Common.Business.Is_Decimal(RadixYL))
                            {
                                cpt.RadixYL = Convert.ToDecimal(RadixYL);
                            }
                            else
                            {
                                Error.Append("养老基数格式不正确，请修改后再导入！\r\n");
                            }
                        }
                        else { cpt.RadixYL = (decimal)0; }
                        if (Common.Business.Is_Decimal(CompanyCosYL))
                        {
                            cpt.CompanyCosYL = Convert.ToDecimal(CompanyCosYL);
                        }
                        else { Error.Append("养老单位承担金额格式不正确！\r\n"); }
                        if (Common.Business.Is_Decimal(PersonCostYL))
                        {
                            cpt.PersonCostYL = Convert.ToDecimal(PersonCostYL);
                        }
                        else { Error.Append("养老个人承担金额格式不正确！\r\n"); }
                    }
                    #endregion

                    #region 判断供应商
                    if (!string.IsNullOrEmpty(suppliers))
                    {
                        var Suppliers = db.Supplier.Where(o => o.Name == suppliers);
                        if (Suppliers.Any())
                        {
                            cpt.SuppliersId = Suppliers.FirstOrDefault().Id;
                            cpt.Suppliers = suppliers;
                        }
                        else { Error.Append("供应商填写不正确！"); }
                    }
                    else
                    { Error.Append("请正确填写供应商！"); }
                    #endregion

                    #region 判断医疗，生育，大病
                    // 判断医疗
                    if (!string.IsNullOrEmpty(CompanyCostYil) || !string.IsNullOrEmpty(PersonCostYil))
                    {
                        cpt.PaymentSocialMonthYiL = PaymentSocialMonthYiL;
                        //基数
                        if (!string.IsNullOrEmpty(RadixYil))
                        {
                            if (Common.Business.Is_Decimal(RadixYil))
                            {
                                cpt.RadixYil = Convert.ToDecimal(RadixYil);
                            }
                            else { Error.Append("医疗基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixYil = (decimal)0; }
                        //医疗单位
                        if (Common.Business.Is_Decimal(CompanyCostYil))
                        {
                            cpt.CompanyCostYil = Convert.ToDecimal(CompanyCostYil);
                        }
                        else { Error.Append("医疗单位承担金额格式不正确！\r\n"); }
                        //医疗个人
                        if (Common.Business.Is_Decimal(PersonCostYil))
                        {
                            cpt.PersonCostYil = Convert.ToDecimal(PersonCostYil);
                        }
                        else { Error.Append("医疗个人承担金额不正确！\r\n"); }
                    }
                    // 判断生育
                    if (!string.IsNullOrEmpty(CompanyCostShengY))
                    {
                        //生育单位
                        if (!string.IsNullOrEmpty(CompanyCostShengY))
                        {
                            if (Common.Business.Is_Decimal(CompanyCostShengY))
                            {
                                cpt.CompanyCostShengY = Convert.ToDecimal(CompanyCostShengY);
                            }
                            else { Error.Append("生育金额格式不正确！\r\n"); }
                        }
                        else { cpt.CompanyCostShengY = (decimal)0; }
                    }
                    // 判断大病
                    if (!string.IsNullOrEmpty(CompanyCostYilMax) || !string.IsNullOrEmpty(PersonCostYilMax))
                    {
                        //大病单位
                        if (!string.IsNullOrEmpty(CompanyCostYilMax))
                        {
                            if (Common.Business.Is_Decimal(CompanyCostYilMax))
                            {
                                cpt.CompanyCostYilMax = Convert.ToDecimal(CompanyCostYilMax);
                            }
                            else { Error.Append("大病单位金额格式不正确！\r\n"); }
                        }
                        else { cpt.CompanyCostYilMax = (decimal)0; }
                        //大病个人
                        if (!string.IsNullOrEmpty(PersonCostYilMax))
                        {
                            if (Common.Business.Is_Decimal(PersonCostYilMax))
                            {
                                cpt.PersonCostYilMax = Convert.ToDecimal(PersonCostYilMax);
                            }
                            else { Error.Append("大病个人金额格式不正确！\r\n"); }
                        }
                        else { cpt.PersonCostYilMax = (decimal)0; }
                    }
                    #endregion

                    #region 判断工伤基数，金额
                    if (!string.IsNullOrEmpty(CompanyCostGS))
                    {
                        cpt.PaymentSocialMonthGS = PaymentSocialMonthGS;
                        //判断基数
                        if (!string.IsNullOrEmpty(RadixGS))
                        {
                            if (Common.Business.Is_Decimal(RadixGS))
                            {
                                cpt.RadixGS = Convert.ToDecimal(RadixGS);
                            }
                            else { Error.Append("工伤基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixGS = (decimal)0; }
                        //判断金额
                        if (Common.Business.Is_Decimal(CompanyCostGS))
                        {
                            cpt.CompanyCostGS = Convert.ToDecimal(CompanyCostGS);
                        }
                        else { Error.Append("工伤单位金额格式不正确！\r\n"); }
                    }
                    #endregion

                    #region 判断失业基数、金额
                    if (!string.IsNullOrEmpty(CompanyCostSY) || !string.IsNullOrEmpty(PersonCostSY))
                    {
                        cpt.PaymentSocialMonthSY = PaymentSocialMonthSY;
                        //判断基数
                        if (!string.IsNullOrEmpty(RadixSY))
                        {
                            if (Common.Business.Is_Decimal(RadixSY))
                            {
                                cpt.RadixSY = Convert.ToDecimal(RadixSY);
                            }
                            else { Error.Append("失业基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixSY = (decimal)0; }
                        //判断单位金额
                        if (Common.Business.Is_Decimal(CompanyCostSY))
                        {
                            cpt.CompanyCostSY = Convert.ToDecimal(CompanyCostSY);
                        }
                        else { Error.Append("失业单位金额格式不正确！\r\n"); }
                        //判断个人金额
                        if (Common.Business.Is_Decimal(PersonCostSY))
                        {
                            cpt.PersonCostSY = Convert.ToDecimal(PersonCostSY);
                        }
                        else { Error.Append("失业个人金额格式不正确！\r\n"); }
                    }
                    #endregion

                    #region 判断公积金基数、金额，补充公积金基数，金额
                    //公积金基数
                    if (!string.IsNullOrEmpty(CompanyCostGJJ) || !string.IsNullOrEmpty(PersonCostGJJ))
                    {
                        cpt.PaymentSocialMonthGJJ = PaymentSocialMonthGJJ;

                        if (!string.IsNullOrEmpty(RadixGJJ))
                        {
                            if (Common.Business.Is_Decimal(RadixGJJ))
                            {
                                cpt.RadixGJJ = Convert.ToDecimal(RadixGJJ);
                            }
                            else { Error.Append("公积金基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixGJJ = (decimal)0; }

                        //公积金单位承担
                        if (Common.Business.Is_Decimal(CompanyCostGJJ))
                        {
                            cpt.CompanyCostGJJ = Convert.ToDecimal(CompanyCostGJJ);
                        }
                        else { Error.Append("公积金单位承担金客格式不正确！\r\n"); }
                        //公积金个人承担
                        if (Common.Business.Is_Decimal(PersonCostGJJ))
                        {
                            cpt.PersonCostGJJ = Convert.ToDecimal(PersonCostGJJ);
                        }
                        else { Error.Append("公积金个人承担金额格式不正确！\r\n"); }
                    }
                    //判断补充公积金
                    if (!string.IsNullOrEmpty(CompanyCostBCGJJ) || !string.IsNullOrEmpty(PersonCostBCGJJ))
                    {
                        //补充公积金基数
                        if (!string.IsNullOrEmpty(RadixBCGJJ))
                        {
                            if (Common.Business.Is_Decimal(RadixBCGJJ))
                            {
                                cpt.RadixBCGJJ = Convert.ToDecimal(RadixBCGJJ);
                            }
                            else { Error.Append("补充公积金基数格式不正确！\r\n"); }
                        }
                        else { cpt.RadixBCGJJ = (decimal)0; }
                        //补充公积金单位承担
                        if (Common.Business.Is_Decimal(CompanyCostBCGJJ))
                        {
                            cpt.CompanyCostBCGJJ = Convert.ToDecimal(CompanyCostBCGJJ);
                        }
                        else { Error.Append("补充公积金单位承担金额格式不正确！\r\n"); }

                        //补充公积金个人承担
                        if (Common.Business.Is_Decimal(PersonCostBCGJJ))
                        {
                            cpt.PersonCostBCGJJ = Convert.ToDecimal(PersonCostBCGJJ);
                        }
                        else { Error.Append("补充公积金个人承担金额格式不正确！\r\n"); }
                    }

                    #endregion

                    #region 判断其他社保费、其他费用、工本费
                    ////其他社保费
                    //if (!string.IsNullOrEmpty(PayOtherSocial))
                    //{
                    //    if (Common.Business.Is_Decimal(PayOtherSocial))
                    //    {
                    //        cpt.PayOtherSocial = Convert.ToDecimal(PayOtherSocial);
                    //    }
                    //    else { Error.Append("其他社保费格式不正确！\r\n"); }
                    //}
                    //else { cpt.PayOtherSocial = (decimal)0; }
                    ////其他费用
                    //if (!string.IsNullOrEmpty(PayOther))
                    //{
                    //    if (Common.Business.Is_Decimal(PayOther))
                    //    {
                    //        cpt.PayOther = Convert.ToDecimal(PayOther);
                    //    }
                    //    else { Error.Append("其他费用格式不正确！\r\n"); }
                    //}
                    //else { cpt.PayOther = (decimal)0; }

                    //养老工本费
                    if (!string.IsNullOrEmpty(yanglaoPayFee))
                    {
                        if (Common.Business.Is_Decimal(yanglaoPayFee))
                        {
                            cpt.YanglaoPayFee = Convert.ToDecimal(yanglaoPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.YanglaoPayFee = (decimal)0; }
                    //医疗工本费
                    if (!string.IsNullOrEmpty(yiliaoPayFee))
                    {
                        if (Common.Business.Is_Decimal(yiliaoPayFee))
                        {
                            cpt.YiliaoPayFee = Convert.ToDecimal(yiliaoPayFee);
                        }
                        else { Error.Append("工本费格式不正确！\r\n"); }
                    }
                    else { cpt.YiliaoPayFee = (decimal)0; }
                    ////工伤工本费
                    //if (!string.IsNullOrEmpty(gongshangPayFee))
                    //{
                    //    if (Common.Business.Is_Decimal(gongshangPayFee))
                    //    {
                    //        cpt.GongshangPayFee = Convert.ToDecimal(gongshangPayFee);
                    //    }
                    //    else { Error.Append("工本费格式不正确！\r\n"); }
                    //}
                    //else { cpt.GongshangPayFee = (decimal)0; }
                    ////公积金工本费
                    //if (!string.IsNullOrEmpty(gongjijinPayFee))
                    //{
                    //    if (Common.Business.Is_Decimal(gongjijinPayFee))
                    //    {
                    //        cpt.GongjijinPayFee = Convert.ToDecimal(gongjijinPayFee);
                    //    }
                    //    else { Error.Append("工本费格式不正确！\r\n"); }
                    //}
                    //else { cpt.GongjijinPayFee = (decimal)0; }
                    ////失业工本费
                    //if (!string.IsNullOrEmpty(shiyePayFee))
                    //{
                    //    if (Common.Business.Is_Decimal(shiyePayFee))
                    //    {
                    //        cpt.ShiyePayFee = Convert.ToDecimal(shiyePayFee);
                    //    }
                    //    else { Error.Append("工本费格式不正确！\r\n"); }
                    //}
                    //else { cpt.ShiyePayFee = (decimal)0; }
                    ////生育工本费
                    //if (!string.IsNullOrEmpty(shengyuPayFee))
                    //{
                    //    if (Common.Business.Is_Decimal(shengyuPayFee))
                    //    {
                    //        cpt.ShengyuPayFee = Convert.ToDecimal(shengyuPayFee);
                    //    }
                    //    else { Error.Append("工本费格式不正确！\r\n"); }
                    //}
                    //else { cpt.ShengyuPayFee = (decimal)0; }


                    //工本费
                    //if (!string.IsNullOrEmpty(PayFee))
                    //{
                    //    if (Common.Business.Is_Decimal(PayFee))
                    //    {
                    //        cpt.PayFee = Convert.ToDecimal(PayFee);
                    //    }
                    //    else { Error.Append("工本费格式不正确！\r\n"); }
                    //}
                    //else { cpt.PayFee = (decimal)0; }

                    #endregion

                    #region 判断有没有错误，没有的话加入创建信息
                    if (Error.ToString() != "")
                    {
                        Error.Insert(0, hanghao);
                        ErrorList.Append(Error);
                    }
                    else
                    {
                        cpt.YearMonth = yearMonth;
                        cpt.CreateTime = DateTime.Now;
                        cpt.CreateUserID = LoginInfo.UserID;
                        cpt.CreateUserName = LoginInfo.UserName;
                        cpt.BranchID = LoginInfo.BranchID;
                        cpt.CardIdSB = cardIdSB;
                        cpt_List.Add(cpt);
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                ErrorList.Append(ex.ToString());
            }
            list = cpt_List;
            return ErrorList.ToString();
        }
        #endregion

    }
}
