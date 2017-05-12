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
using Langben.DAL.SCHR;
using System.IO;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
using Langben.IBLL;
using System.Web;
//using System.Data.OleDb;
using System.Data;
using Langben.DAL.Model;
using NPOI.HSSF.UserModel;
using System.Text.RegularExpressions;

namespace Langben.App.Controllers
{
    /// <summary> 
    /// 增加员工
    /// </summary>
    public class EmployeeAddController : BaseController
    {
        SysEntities SysEntitiesO2O = new SysEntities();

        #region 责任客服单人报增列表
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {

            return View();
        }
        #endregion
        #region 责任客服审核列表页面
        /// <summary>
        /// 责任客服审核列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult ApproveList()
        {
            return View();
        }
        #endregion
        #region 客服经理审核列表页面
        /// <summary>
        /// 客服经理审核列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult AllotList()
        {
            return View();
        }
        #endregion
        #region 社保专员提取报增信息列表
        /// <summary>
        /// 社保专员提取报增信息列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult SupplierList()
        {
            return View();
        }
        #endregion
        #region 社保报增查询列表页面
        /// <summary>
        /// 社保报增查询列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeAddViewList()
        {
            //根据菜单查询权限配置读取相应范围的数据
            //1016-0 全部查询
            //1016-1 责任客服查询
            //1016-2 员工客服查询
            //1016-3 社保专员查询
            ViewBag.User_ALL = this.MenuOpAuthority("1016-0");
            ViewBag.User_ZR = this.MenuOpAuthority("1016-1");
            ViewBag.User_YG = this.MenuOpAuthority("1016-2");
            ViewBag.User_SB = this.MenuOpAuthority("1016-3");
            return View();
        }
        #endregion

        #region 社保模板报增页面
        /// <summary>
        /// 社保模板报增
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeAddByExcel()
        {
            return View();
        }
        #endregion

        #region 社保专员反馈 申报成功页面
        /// <summary>
        /// 社保专员反馈 申报成功页面
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmpFeedBackSuccess(string ids, string Cityid, string CompanyEmployeeRelationId, string alltype, string countnn)
        {
            ViewBag.ids = ids;
            ViewBag.alltype = HttpUtility.HtmlDecode(alltype);
            ViewBag.Cityid = Cityid;
            ViewBag.CompanyEmployeeRelationId = CompanyEmployeeRelationId;
            ViewBag.flag = countnn;
            return View();
        }
        #endregion

        #region 社保专员/供应商专员报增反馈页面
        /// <summary>
        /// 社保专员/供应商专员报增反馈
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult FeedbackList()
        {

            return View();
        }
        #endregion



        #region 回退页面
        /// <summary>
        /// 回退页面
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Fallback(string ids)
        {
            ViewBag.ids = ids.TrimEnd(',').ToString();
            return View();


        }
        #endregion
        #region 模板更新社保编号
        /// <summary>
        /// 模板更新社保编号
        /// </summary>        
        /// <returns></returns>
        [SupportFilter]
        public ActionResult InsCodeUpdatebyExcel()
        {

            return View();


        }
        #endregion

        #region 报增失败页面
        /// <summary>
        /// 失败页面
        /// </summary>
        /// <param name="ids">失败人员id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult FeedbackIndex(string ids, string Cityid, string CompanyEmployeeRelationId, string alltype, string countnn)
        {
            ViewBag.ids = ids.TrimEnd(',').ToString();
            ViewBag.Cityid = Cityid;
            ViewBag.CompanyEmployeeRelationId = CompanyEmployeeRelationId;

            ViewBag.alltype = HttpUtility.HtmlDecode(alltype);
            ViewBag.flag = countnn;


            return View();

        }
        #endregion

        #region 社保专员报增信息 回退页面
        /// 社保专员报增信息 回退页面
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeFallback(string ids, string Cityid, string alltype, string countnn)
        {
            ViewBag.ids = ids.TrimEnd(',').ToString();
            ViewBag.Cityid = Cityid;
            //ViewBag.CompanyEmployeeRelationId = CompanyEmployeeRelationId;

            ViewBag.alltype = HttpUtility.HtmlDecode(alltype);
            ViewBag.flag = countnn;
            return View();

        }
        #endregion


        #region 社保申报
        /// <summary>
        /// 首次创建
        /// </summary>
        /// <returns></returns>
        [SupportFilter]

        public ActionResult Create(int id)
        {
            IBLL.IEmployeeBLL e_BLL = new BLL.EmployeeBLL();
            IBLL.ICityBLL city_BLL = new BLL.CityBLL();
            IBLL.ICRM_CompanyBLL crm_Company_BLL = new BLL.CRM_CompanyBLL();
            IBLL.IPoliceAccountNatureBLL policeAccountNature_BLL = new BLL.PoliceAccountNatureBLL();
            var Employeelist = e_BLL.GetById(id);
            ViewBag.Employee = Employeelist;
            List<City> Citylist = city_BLL.GetAll();
            ViewBag.Citylist = Citylist.ToList();
            List<CRM_Company> CRM_Companylist = crm_Company_BLL.GetAll();
            ViewBag.CRM_Companylist = CRM_Companylist.ToList();
            List<PoliceAccountNature> PoliceAccountNaturelist = policeAccountNature_BLL.GetAll();
            ViewBag.PoliceAccountNaturelist = PoliceAccountNaturelist.ToList();
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
        #endregion



        #region 获取所在地客服人员
        /// <summary>
        /// 获取所在地客服人员
        /// </summary>
        /// <param name="企业所在地code">企业所在地所有客服人员</param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult Kfry(string CityCode)
        {
            try
            {
                // var json="[{'UserID':1,'LoginName':'xujie','RealName':'徐杰','CityCode':'100001','CityName':'石家庄'}]";
                var json = "[{\"UserID\":1,\"LoginName\":\"xujie\",\"RealName\":\"张三\",\"CityCode\":\"100001\",\"CityName\":\"石家庄\"},{\"UserID\":2,\"LoginName\":\"xujie\",\"RealName\":\"李四\",\"CityCode\":\"100001\",\"CityName\":\"石家庄\"},{\"UserID\":3,\"LoginName\":\"xujie\",\"RealName\":\"王五\",\"CityCode\":\"100001\",\"CityName\":\"石家庄\"}]";
                return Content(json);

            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
        #endregion

        #region 获取服务人数
        /// <summary>
        /// 获取服务人数
        /// </summary>
        /// <param name="UserID_YG">客服人员id</param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult Sercount(string UserID_YG)
        {
            string result = "";
            try
            {

                int uid = 0;
                if (int.TryParse(UserID_YG, out uid))
                {
                    result = GetSercount(uid).ToString();
                }


            }
            catch (Exception e)
            {
                result = "无服务信息";

            }
            result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Content(result);
        }
        #endregion

        #region 获取当前客服服务人数
        /// <summary>
        /// 获取当前客服服务人数
        /// </summary>
        /// <param name="UserID_YG">客服人员id</param>
        /// <returns></returns>


        public int GetSercount(int UserID_YG)
        {
            string CityId = LoginInfo.CompanyCityCode;
            int count = 0;
            using (var ent = new SysEntities())
            {
                try
                {
                    List<string> StateList = new List<string>();
                    StateList.Add(EmployeeAdd_State.待责任客服确认.ToString());
                    StateList.Add(EmployeeAdd_State.待员工客服确认.ToString());
                    StateList.Add(EmployeeAdd_State.员工客服已确认.ToString());
                    StateList.Add(EmployeeAdd_State.社保专员已提取.ToString());
                    StateList.Add(EmployeeAdd_State.申报成功.ToString());
                    var CrmComp = ent.UserCityCompany.Where(a => true);
                    var ComEmpRel = ent.CompanyEmployeeRelation.Where(a => true);
                    var Emp = ent.EmployeeAdd.Where(a => true);
                    CrmComp = CrmComp.Where(a => a.UserID_YG == UserID_YG);
                    ComEmpRel = ComEmpRel.Where(a => true);
                    Emp = Emp.Where(a => StateList.Contains(a.State));
                    var list = (from a in CrmComp
                                join b in ComEmpRel on a.CompanyId equals b.CompanyId
                                join c in Emp on b.Id equals c.CompanyEmployeeRelationId
                                select b).Distinct();
                    if (list != null && list.Count() >= 1)
                    {
                        count = list.Count();
                    }




                }
                catch (Exception e)
                {
                    count = -1;
                }

            }
            return count;
        }
        #endregion

        #region 责任客服修改列表
        /// <summary>
        /// 责任客服修改列表 jing 
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CustomerModifyList()
        {
            return View();
        }
        #endregion

        #region 责任客服修改页面
        /// <summary>
        /// 责任客服修改列表 jing
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CustomerModify(int id)
        {
            IBLL.IEmployeeBLL e_BLL = new BLL.EmployeeBLL();
            IBLL.ICityBLL city_BLL = new BLL.CityBLL();
            IBLL.ICRM_CompanyBLL crm_Company_BLL = new BLL.CRM_CompanyBLL();
            IBLL.IPoliceAccountNatureBLL policeAccountNature_BLL = new BLL.PoliceAccountNatureBLL();
            IBLL.ICompanyEmployeeRelationBLL CompanyEmployeeRelation_BLL = new BLL.CompanyEmployeeRelationBLL();
            var CompanyEmployeeRelationlist = CompanyEmployeeRelation_BLL.GetById(id);

            if (CompanyEmployeeRelationlist != null)
            {
                var Employeelist = e_BLL.GetById((int)CompanyEmployeeRelationlist.EmployeeId);
                ViewBag.Employee = Employeelist;
                ViewBag.CityId = CompanyEmployeeRelationlist.CityId;
                ViewBag.CompanyId = CompanyEmployeeRelationlist.CompanyId;
                ViewBag.PoliceAccountNatureId = CompanyEmployeeRelationlist.PoliceAccountNatureId;
                ViewBag.Station = CompanyEmployeeRelationlist.Station;
                ViewBag.CompanyEmployeeRelationid = id;
                object YearMonth = Request["YearMonth"];
                ViewBag.YearMonth = YearMonth;
            }
            List<City> Citylist = city_BLL.GetAll();
            ViewBag.Citylist = Citylist.ToList();
            List<CRM_Company> CRM_Companylist = crm_Company_BLL.GetAll();
            ViewBag.CRM_Companylist = CRM_Companylist.ToList();
            List<PoliceAccountNature> PoliceAccountNaturelist = policeAccountNature_BLL.GetAll();
            ViewBag.PoliceAccountNaturelist = PoliceAccountNaturelist.ToList();
            return View();

        }
        #endregion

        #region 责任客服及员工客服详情页面
        /// <summary>
        /// 责任客服详情页面 jing
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult CustomerDetailed(int id)
        {
            IBLL.IEmployeeBLL e_BLL = new BLL.EmployeeBLL();
            IBLL.ICityBLL city_BLL = new BLL.CityBLL();
            IBLL.ICRM_CompanyBLL crm_Company_BLL = new BLL.CRM_CompanyBLL();
            IBLL.IPoliceAccountNatureBLL policeAccountNature_BLL = new BLL.PoliceAccountNatureBLL();
            IBLL.ICompanyEmployeeRelationBLL CompanyEmployeeRelation_BLL = new BLL.CompanyEmployeeRelationBLL();
            var CompanyEmployeeRelationlist = CompanyEmployeeRelation_BLL.GetById(id);

            if (CompanyEmployeeRelationlist != null)
            {
                var Employeelist = e_BLL.GetById((int)CompanyEmployeeRelationlist.EmployeeId);
                ViewBag.Employee = Employeelist;
                ViewBag.CityId = CompanyEmployeeRelationlist.CityId;
                ViewBag.CompanyId = CompanyEmployeeRelationlist.CompanyId;
                ViewBag.PoliceAccountNatureId = CompanyEmployeeRelationlist.PoliceAccountNatureId;
                ViewBag.Station = CompanyEmployeeRelationlist.Station;
                ViewBag.CompanyEmployeeRelationid = id;
                object Parameter = Request["Parameter"];
                object YearMonth = Request["YearMonth"];
                object SupplierId = Request["SupplierId"];
                ViewBag.YearMonth = YearMonth;
                ViewBag.Parameter = Parameter;
                ViewBag.SupplierId = SupplierId;
            }
            List<City> Citylist = city_BLL.GetAll();
            ViewBag.Citylist = Citylist.ToList();
            List<CRM_Company> CRM_Companylist = crm_Company_BLL.GetAll();
            ViewBag.CRM_Companylist = CRM_Companylist.ToList();
            List<PoliceAccountNature> PoliceAccountNaturelist = policeAccountNature_BLL.GetAll();
            ViewBag.PoliceAccountNaturelist = PoliceAccountNaturelist.ToList();
            return View();

        }
        #endregion

        #region 社保专员修改
        /// <summary>
        /// 社保专员修改
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult FeedbackListModify(int id)
        {
            IBLL.IEmployeeBLL e_BLL = new BLL.EmployeeBLL();
            IBLL.ICityBLL city_BLL = new BLL.CityBLL();
            IBLL.ICRM_CompanyBLL crm_Company_BLL = new BLL.CRM_CompanyBLL();
            IBLL.IPoliceAccountNatureBLL policeAccountNature_BLL = new BLL.PoliceAccountNatureBLL();
            IBLL.ICompanyEmployeeRelationBLL CompanyEmployeeRelation_BLL = new BLL.CompanyEmployeeRelationBLL();
            var CompanyEmployeeRelationlist = CompanyEmployeeRelation_BLL.GetById(id);

            if (CompanyEmployeeRelationlist != null)
            {
                var Employeelist = e_BLL.GetById((int)CompanyEmployeeRelationlist.EmployeeId);
                ViewBag.Employee = Employeelist;
                ViewBag.CityId = CompanyEmployeeRelationlist.CityId;
                ViewBag.CompanyId = CompanyEmployeeRelationlist.CompanyId;
                ViewBag.CompanyEmployeeRelationid = id;
                ViewBag.Station = CompanyEmployeeRelationlist.Station;
                ViewBag.PoliceAccountNatureId = CompanyEmployeeRelationlist.PoliceAccountNatureId;
                object Parameter = Request["Parameter"];
                ViewBag.Parameter = Parameter;
                object YearMonth = Request["YearMonth"];
                ViewBag.YearMonth = YearMonth;
            }

            List<City> Citylist = city_BLL.GetAll();
            ViewBag.Citylist = Citylist.ToList();
            List<CRM_Company> CRM_Companylist = crm_Company_BLL.GetAll();
            ViewBag.CRM_Companylist = CRM_Companylist.ToList();
            List<PoliceAccountNature> PoliceAccountNaturelist = policeAccountNature_BLL.GetAll();
            ViewBag.PoliceAccountNaturelist = PoliceAccountNaturelist.ToList();
            return View();

        }
        #endregion

        #region 员工客服修改列表
        /// <summary>
        /// 员工客服修改列表 jing 
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeModifyList()
        {
            return View();
        }
        #endregion

        #region 员工客服修改页面
        /// <summary>
        /// 员工客服修改页面 jing
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeModify(int id)
        {
            IBLL.IEmployeeBLL e_BLL = new BLL.EmployeeBLL();
            IBLL.ICityBLL city_BLL = new BLL.CityBLL();
            IBLL.ICRM_CompanyBLL crm_Company_BLL = new BLL.CRM_CompanyBLL();
            IBLL.IPoliceAccountNatureBLL policeAccountNature_BLL = new BLL.PoliceAccountNatureBLL();
            IBLL.ICompanyEmployeeRelationBLL CompanyEmployeeRelation_BLL = new BLL.CompanyEmployeeRelationBLL();
            var CompanyEmployeeRelationlist = CompanyEmployeeRelation_BLL.GetById(id);

            if (CompanyEmployeeRelationlist != null)
            {
                var Employeelist = e_BLL.GetById((int)CompanyEmployeeRelationlist.EmployeeId);
                ViewBag.Employee = Employeelist;
                ViewBag.CityId = CompanyEmployeeRelationlist.CityId;
                ViewBag.CompanyId = CompanyEmployeeRelationlist.CompanyId;
                ViewBag.CompanyEmployeeRelationid = id;
                ViewBag.Station = CompanyEmployeeRelationlist.Station;
                ViewBag.PoliceAccountNatureId = CompanyEmployeeRelationlist.PoliceAccountNatureId;
                object YearMonth = Request["YearMonth"];
                ViewBag.YearMonth = YearMonth;
            }
            List<City> Citylist = city_BLL.GetAll();
            ViewBag.Citylist = Citylist.ToList();
            List<CRM_Company> CRM_Companylist = crm_Company_BLL.GetAll();
            ViewBag.CRM_Companylist = CRM_Companylist.ToList();
            List<PoliceAccountNature> PoliceAccountNaturelist = policeAccountNature_BLL.GetAll();
            ViewBag.PoliceAccountNaturelist = PoliceAccountNaturelist.ToList();
            return View();

        }
        #endregion

        #region 员工客服终止页面
        /// <summary>
        /// 员工客服终止页面
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmployeeStop(string ids, string Cityid, string alltype, string countnn, string type)
        {
            ViewBag.ids = ids.TrimEnd(',').ToString();//报增表id
            ViewBag.Cityid = Cityid;//城市
            //ViewBag.CompanyEmployeeRelationId = CompanyEmployeeRelationId;

            ViewBag.alltype = HttpUtility.HtmlDecode(alltype);//险种
            ViewBag.flag = countnn;
            ViewBag.type = type;//1:终止 2：挂起
            return View();

        }
        #endregion

        #region  社保模板报增导入

        /// <summary>
        /// 社保模板报增导入
        /// </summary>
        /// <returns></returns>
        //[System.Web.Mvc.HttpPost]
        //public Common.ClientResult.Result EmployeeAddByExcel(HttpPostedFileBase files)

        public ActionResult InputEmployeeAddByExcel(HttpPostedFileBase files)
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
                    string[] FileType = new string[] { ".xls", ".xlsx" };//定义上传文件的类型字符串

                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        result.Code = Common.ClientCode.Fail;
                        result.Message = "文件类型不对，只能导入Excel文件";
                        //return result;
                        return Json(new { code = 0, msg = result.Message });
                    }
                    string path = AppDomain.CurrentDomain.BaseDirectory + "excel/";
                    savePath = System.IO.Path.Combine(path, FileName);
                    file.SaveAs(savePath);
                }
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
                if (ImportEmployeeAdd(table, ref message, out SuccessNum))
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




        #region 更新社保编号导入
        public ActionResult InputInsCodeUpdatebyExcel(HttpPostedFileBase files)
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
                string UpdateMessage = ImportEmployeeAddUpdate(table);
                if (UpdateMessage == "1")
                {

                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "更新成功!";
                    return Json(new { code = 1, msg = result.Message });
                }
                else
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = UpdateMessage;
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
        #endregion



        IBLL.IEmployeeAddBLL e_BLL = new BLL.EmployeeAddBLL();
        IBLL.ICRM_CompanyBLL crm_BLL = new BLL.CRM_CompanyBLL();
        IBLL.ICityBLL city_BLL = new BLL.CityBLL();
        IBLL.IPoliceAccountNatureBLL pan_BLL = new BLL.PoliceAccountNatureBLL();
        IBLL.IPoliceOperationBLL po_BLL = new BLL.PoliceOperationBLL();
        IBLL.IPoliceInsuranceBLL pi_BLL = new BLL.PoliceInsuranceBLL();
        IBLL.ICRM_CompanyToBranchBLL crmR_BLL = new BLL.CRM_CompanyToBranchBLL();

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
                    EmployeeAddApiController api = new EmployeeAddApiController();
                    //校验并转换信息
                    List<returnData> employeeList = CheckImportEmployeeAdd(table, ref message);

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        return false;
                    }
                    foreach (returnData postinfos in employeeList)
                    {
                        int CompanyId = postinfos.CompanyId;

                        string re = api.Addcommit1(postinfos, CompanyId, 2);
                        //string re = "dsd";
                        if (re != "")
                        {
                            string DataForm = "证件号:{0},姓名:{1},的异常信息为:{2}\r\n";
                            message = string.Format(DataForm, postinfos.IDNumber, postinfos.Name, re);
                            return false;
                        }
                        else
                        {
                            SuccessNum += 1;
                        }
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


        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        private string ImportEmployeeAddUpdate(DataTable table)
        {
            string resultn = string.Empty;
            try
            {
                StringBuilder sbError = new StringBuilder();
                // SuccessNum = 0;
                if (table.Rows.Count > 0)
                {
                    #region Excel数据提取
                    EmployeeAddApiController api = new EmployeeAddApiController();
                    resultn = api.AddcommitUpdate(table);
                    //result = true;
                }


            }
            catch (Exception e)
            {
                throw e;
                resultn = e.Message;

            }

            return resultn;
        }

        private List<returnData> CheckImportEmployeeAdd(DataTable table, ref string message)
        {
            IBLL.IPoliceInsuranceBLL Polic_BLL = new BLL.PoliceInsuranceBLL();
            EmployeeAddApiController api = new EmployeeAddApiController();
            List<EmployeeAddExcelModel1> employeeList = new List<EmployeeAddExcelModel1>();
            #region 数据转换
            foreach (DataRow dr in table.Rows)
            {

                EmployeeAddExcelModel1 employee = new EmployeeAddExcelModel1();
                employee.CompanyName = dr["单位"].ToString().Trim();
                employee.City = dr["社保缴纳地"].ToString().Trim();
                employee.SupplierName = dr["供应商"].ToString().Trim();
                employee.Name = dr["姓名"].ToString().Trim();
                employee.CertificateNumber = dr["证件号码"].ToString().Trim();
                employee.CertificateType = dr["证件类型"].ToString().Trim();
                employee.Station = dr["岗位"].ToString().Trim();
                employee.PoliceAccountNatureName = dr["户口性质"].ToString().Trim();
                employee.StartTime = dr["社保起缴时间"].ToString().Trim();

                employee.Wage = dr["社保工资"].ToString().Trim();
                employee.StartTime_5 = dr["公积金起缴时间"].ToString().Trim();
                employee.Wage_5 = dr["公积金基数"].ToString().Trim();
                List<DAL.Insurance> list = new List<DAL.Insurance>();
                var city = SysEntitiesO2O.City.FirstOrDefault(p => p.Name == employee.City);
                if (city != null)
                {

                    string POSTPoliceCascadeRelationshipName = Polic_BLL.POSTPoliceCascadeRelationship(city.Id);

                    string[] pics = POSTPoliceCascadeRelationshipName.Split(',');

                    foreach (var ad in city.InsuranceKind)
                    {
                        DAL.Insurance ina = new DAL.Insurance();

                        if (ad.Name != "公积金" && ad.Name != "补充公积金" && !string.IsNullOrWhiteSpace(employee.Wage))
                        {

                            if (table.Columns.Contains("" + ad.Name + "政策名称") && table.Columns.Contains("" + ad.Name + "报增方式"))
                            {

                                //ina.StartTime = employee.StartTime;

                                if (pics.Contains(ad.Name))
                                {
                                    ina.InsuranceKind = ad.Name;
                                    ina.StartTime = employee.StartTime;
                                    ina.PoliceInsurancename = dr["" + ad.Name + "政策名称"].ToString().Trim();
                                    ina.PoliceOperationname = dr["" + ad.Name + "报增方式"].ToString().Trim();
                                    ina.SupplierRemark = dr["" + ad.Name + "备注"].ToString().Trim();
                                    list.Add(ina);
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(dr["" + ad.Name + "政策名称"].ToString().Trim()) && !string.IsNullOrWhiteSpace(dr["" + ad.Name + "报增方式"].ToString().Trim()))
                                    {
                                        ina.InsuranceKind = ad.Name;
                                        ina.StartTime = employee.StartTime;
                                        ina.PoliceInsurancename = dr["" + ad.Name + "政策名称"].ToString().Trim();
                                        ina.PoliceOperationname = dr["" + ad.Name + "报增方式"].ToString().Trim();
                                        ina.SupplierRemark = dr["" + ad.Name + "备注"].ToString().Trim();
                                        list.Add(ina);
                                    }
                                }
                            }
                        }
                        if ((ad.Name == "公积金" || ad.Name == "补充公积金") && !string.IsNullOrWhiteSpace(employee.Wage_5))
                        {
                            if (table.Columns.Contains("" + ad.Name + "政策名称") && table.Columns.Contains("" + ad.Name + "报增方式"))
                            {


                                ina.InsuranceKind = ad.Name;

                                ina.StartTime = employee.StartTime;
                                ina.PoliceInsurancename = dr["" + ad.Name + "政策名称"].ToString().Trim();
                                ina.PoliceOperationname = dr["" + ad.Name + "报增方式"].ToString().Trim();
                                ina.SupplierRemark = dr["" + ad.Name + "备注"].ToString().Trim();
                                list.Add(ina);
                            }
                        }

                    }
                    employee.Insurance = list;
                }
                employee.AccountType = dr["户口类型"].ToString().Trim();
                employee.AccountAddress = dr["户口所在地"].ToString().Trim();
                employee.MobilePhone = dr["联系电话"].ToString().Trim();
                employee.Address = dr["联系地址"].ToString().Trim();
                employeeList.Add(employee);
            }
            #endregion

            string search = string.Empty;
            StringBuilder errMessageAll = new StringBuilder();
            List<returnData> PostInfoList = new List<returnData>();
            string DataForm = "证件号:{0},姓名:{1},的异常信息为:{2}\r\n";
            Regex r = new Regex(@"(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0-9]|170)\d{8}$");
            Regex r2 = new Regex(@"^[A-Za-z\u4e00-\u9fa5]*$");
            foreach (EmployeeAddExcelModel1 item in employeeList)
            {
                decimal shebao_wage = 0;
                StringBuilder errMessage = new StringBuilder();
                returnData postinfos = new returnData();
                #region 基础信息赋值
                //系统中不存在报增公司,抛出异常
                if (string.IsNullOrEmpty(item.CertificateNumber))
                {
                    errMessage.Append("证件号码为空;\r\n");
                }
                //else if (!CardCommon.CheckCardID18(item.CertificateNumber))
                //{
                //    errMessage.Append("身份证号不合法：" + postinfos.IDNumber + ";\r\n");
                //}
                if (string.IsNullOrEmpty(item.Name))
                {
                    errMessage.Append("姓名为空;\r\n");
                }
                else if (!r2.IsMatch(item.Name))
                {
                    errMessage.Append("姓名只能输入汉字和字母;\r\n");
                }
                if (string.IsNullOrEmpty(item.CompanyName))
                {
                    errMessage.Append("单位为空;\r\n");
                }
                else
                {
                    postinfos.CompanyId = getCompanyIdByExcel(item, ref errMessage);
                }
                if (string.IsNullOrEmpty(item.City))
                {
                    errMessage.Append("社保缴纳地为空;\r\n");
                }
                else
                {
                    postinfos.City = getCityIdByExcel(item, ref errMessage);
                    var city = SysEntitiesO2O.City.FirstOrDefault(p => p.Name == item.City);
                    if (city == null || city.InsuranceKind == null || city.InsuranceKind.Count == 0)
                    {
                        errMessage.Append("社保缴纳地:" + item.City + "不存在社保险种信息,请联系相关人员维护险种政策信息;\r\n");
                    }
                }

                if (!string.IsNullOrWhiteSpace(item.SupplierName))
                {
                    postinfos.SuppliersId = getSupplierIdByExcel(item, ref errMessage);
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
                if (!string.IsNullOrEmpty(item.Wage))
                {

                    //社保工资
                    if (decimal.TryParse(item.Wage, out shebao_wage))
                    {

                        if (shebao_wage <= 0)
                        {
                            errMessage.Append("社保工资不正确;\r\n");
                        }
                        if (!Business.IsNumber(item.Wage, 32, 2))
                        {
                            errMessage.Append("社保工资请保留两位小数;\r\n");
                        }
                    }
                    else
                    {
                        errMessage.Append("社保工资不正确;\r\n");
                    }
                }
                if (string.IsNullOrEmpty(item.PoliceAccountNatureName))
                {
                    errMessage.Append("户口性质为空;\r\n");
                }
                else
                {
                    postinfos.PoliceAccountNature = getPoliceAccountNatureIdByExcel(item, ref errMessage);
                }
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dt = new DateTime();
                    if (!DateTime.TryParse(item.StartTime, out dt))
                    {
                        errMessage.Append("社保起缴时间格式不正确;\r\n");
                    }
                }
                if (string.IsNullOrEmpty(item.StartTime) && string.IsNullOrEmpty(item.StartTime_5))
                {
                    errMessage.Append("社保起缴时间和公积金起缴时间不能同时为空;\r\n");
                }
                if (string.IsNullOrEmpty(item.MobilePhone))
                {
                    errMessage.Append("联系人电话为空;\r\n");
                }
                #endregion
                List<DAL.Model.Insurance> list = new List<DAL.Model.Insurance>();
                #region 公积金判断赋值
                if (string.IsNullOrEmpty(item.StartTime_5))
                {
                    //没有报增公积金
                }
                else
                {
                    DateTime dt = new DateTime();
                    if (!DateTime.TryParse(item.StartTime_5, out dt))
                    {
                        errMessage.Append("公积金起缴时间格式不正确;\r\n");
                    }
                    int i = 0;

                    foreach (var ad in item.Insurance)
                    {

                        if (ad.InsuranceKind == "公积金" || ad.InsuranceKind == "补充公积金")
                        {
                            i++;
                            DAL.Model.Insurance ina = new DAL.Model.Insurance();

                            ina.InsuranceKind = ad.InsuranceKind;
                            ina.PoliceOperationname = ad.PoliceOperationname;
                            ina.PoliceInsurancename = ad.PoliceInsurancename;
                            ina.StartTime = item.StartTime_5;
                            ina.SupplierRemark = ad.SupplierRemark;


                            if (string.IsNullOrEmpty(item.Wage_5))
                            {
                                errMessage.Append("" + ad.InsuranceKind + "基数为空;\r\n");
                            }
                            else
                            {
                                decimal wage = 0;
                                //公积金工资
                                if (decimal.TryParse(item.Wage_5, out wage))
                                {
                                    //int result = item.Wage_5.Length - item.Wage_5.IndexOf('.') - 1;
                                    if (wage <= 0)
                                    {
                                        errMessage.Append("" + ad.InsuranceKind + "基数不正确;\r\n");
                                    }
                                    else if (!Business.IsNumber(item.Wage_5, 32, 2))
                                    {
                                        errMessage.Append("" + ad.InsuranceKind + "工资请保留两位小数;\r\n");
                                    }
                                    else
                                    {
                                        ina.Wage = wage;
                                    }
                                }
                                else
                                {
                                    errMessage.Append("" + ad.InsuranceKind + "基数不正确;\r\n");
                                }
                            }

                            if (string.IsNullOrEmpty(ad.PoliceInsurancename))
                            {
                                errMessage.Append("" + ad.InsuranceKind + "政策名称为空;\r\n");
                                ina.PoliceInsurance = -1;
                            }
                            else
                            {
                                ina.PoliceInsurance = getPension_InsuranceByExcel(item, ad.PoliceInsurancename, ref errMessage);
                            }
                            if (string.IsNullOrEmpty(ad.PoliceOperationname))
                            {
                                errMessage.Append("" + ad.InsuranceKind + "报增方式为空;\r\n");
                                ina.PoliceOperation = -1;
                            }
                            else
                            {
                                ina.PoliceOperation = getPoliceOperationByExcel(item, ad.PoliceOperationname, ref errMessage);

                            }
                            list.Add(ina);
                        }
                    }
                    if (i == 0)
                    {
                        errMessage.Append("公积金起缴时间已填写,请填写公积金或补充公积金的政策和报增方式;\r\n");
                    }
                    // postinfos.Insurance=list;
                }
                #endregion
                #region 社保判断赋值


                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    int i = 0;
                    foreach (var Insurancetype in item.Insurance)
                    {
                        if (Insurancetype.InsuranceKind != "公积金" && Insurancetype.InsuranceKind != "补充公积金")
                        {
                            i++;

                            DAL.Model.Insurance Insurancetypeina = new DAL.Model.Insurance();
                            if (string.IsNullOrEmpty(Insurancetype.PoliceInsurancename))
                            {
                                errMessage.Append("" + Insurancetype.InsuranceKind + "政策名称为空;\r\n");
                                Insurancetypeina.PoliceInsurance = -1;
                            }
                            else
                            {
                                Insurancetypeina.PoliceInsurance = getPension_InsuranceByExcel(item, Insurancetype.PoliceInsurancename, ref errMessage);

                            }
                            if (string.IsNullOrEmpty(Insurancetype.PoliceOperationname))
                            {
                                errMessage.Append("" + Insurancetype.InsuranceKind + "报增方式为空;\r\n");
                                Insurancetypeina.PoliceOperation = -1;
                            }
                            else
                            {
                                Insurancetypeina.PoliceOperation = getPoliceOperationByExcel(item, Insurancetype.PoliceOperationname, ref errMessage);
                            }
                            Insurancetypeina.InsuranceKind = Insurancetype.InsuranceKind;
                            Insurancetypeina.PoliceOperationname = Insurancetype.PoliceOperationname;
                            Insurancetypeina.PoliceInsurancename = Insurancetype.PoliceInsurancename;
                            Insurancetypeina.StartTime = Insurancetype.StartTime;
                            Insurancetypeina.Wage = shebao_wage;
                            Insurancetypeina.SupplierRemark = Insurancetype.SupplierRemark;
                            list.Add(Insurancetypeina);
                        }
                    }
                    if (i == 0)
                    {
                        errMessage.Append("社保起缴时间已填写,请填写至少一种社保政策和报增方式;\r\n");
                    }
                }
                postinfos.Insurance = list;
                #endregion
                /*
                 * 当某报增险种的关键数据齐全时,验证是否存在有且仅有一条具有关联关系的数据
                 * 若存在则险种报增验证成功,若无或者多条,则验证失败
                 */
                #region 险种关键数据关联性校验


                foreach (var Insurancetype in postinfos.Insurance)
                {


                    if (!string.IsNullOrWhiteSpace(postinfos.City) && postinfos.PoliceAccountNature != -1 && Insurancetype.PoliceOperation != -1 && Insurancetype.PoliceInsurance != -1)
                    {
                        List<EmployeeAddCheckModel> checkModel = e_BLL.EmployeeAddCkeck(postinfos.City, postinfos.PoliceAccountNature, Insurancetype.PoliceOperation, Insurancetype.PoliceInsurance, Insurancetype.InsuranceKind);
                        if (!string.IsNullOrWhiteSpace(item.City))
                        {
                            if (checkModel == null || checkModel.Count == 0)
                            {
                                //一定可以查出内容
                                //errMessage.Append(string.Format("养老社保缴纳地 {0} 不能申报 {1} 的户口性质", item.City, item.PoliceAccountNatureName));
                            }
                            else if (checkModel.Count >= 2)
                            {
                                errMessage.Append(string.Format("系统数据中" + Insurancetype.InsuranceKind + "申报的社保缴纳地、户口性质、社保政策、社保手续关联存在重复,请核实;\r\n"));
                            }
                            else if (checkModel[0].PoliceAccountNatureId == null)
                            {
                                errMessage.Append(string.Format("" + Insurancetype.InsuranceKind + "社保缴纳地 {0} 不能申报 {1} 的户口性质;\r\n", item.City, item.PoliceAccountNatureName));
                            }
                            else if (checkModel[0].PoliceInsuranceId == null)
                            {
                                errMessage.Append(string.Format("" + Insurancetype.InsuranceKind + "社保缴纳地: {0},户口性质: {1},此情况不能申报社保政策:{2};\r\n", item.City, item.PoliceAccountNatureName, Insurancetype.PoliceInsurancename));
                            }
                            else if (checkModel[0].PoliceOperationId == null)
                            {
                                errMessage.Append(string.Format("" + Insurancetype.InsuranceKind + "社保缴纳地: {0},户口性质: {1},社保政策:{2},此情况下社保手续不能为{3};\r\n", item.City, item.PoliceAccountNatureName, Insurancetype.PoliceInsurancename, Insurancetype.PoliceOperationname));
                            }
                        }
                    }

                }
                #endregion


                postinfos.Name = item.Name;
                postinfos.IDNumber = item.CertificateNumber;
                postinfos.IDType = item.CertificateType;
                postinfos.Station = item.Station;   //岗位字段没有
                postinfos.Telephone = item.MobilePhone;
                postinfos.ResidentLocation = item.Address;
                postinfos.ResidentType = item.AccountType;

                if (string.IsNullOrWhiteSpace(errMessage.ToString()))
                {

                    var employeelist = SysEntitiesO2O.Employee.FirstOrDefault(p => p.CertificateNumber == postinfos.IDNumber);
                    string error = "";
                    if (employeelist == null)
                    {
                        error = api.Platform_verification1(postinfos);
                    }
                    else
                    {
                        error = api.verification1(postinfos);
                    }

                    string err = error.Replace("<br />", "\r\n");
                    errMessage.Append(err);
                    //}
                    PostInfoList.Add(postinfos);
                    if (!string.IsNullOrWhiteSpace(errMessage.ToString()))
                    {

                        errMessageAll.Append(string.Format(DataForm, item.CertificateNumber, item.Name, errMessage.ToString()));
                    }
                }
                else
                {
                    errMessageAll.Append(string.Format(DataForm, item.CertificateNumber, item.Name, errMessage.ToString()));
                }
            }
            message = errMessageAll.ToString();
            return PostInfoList;
        }

        #region 判断
        private int getPension_InsuranceByExcel(EmployeeAddExcelModel1 item, string p, ref StringBuilder Message)
        {
            string search = "NameDDL_String&" + p;
            List<PoliceInsurance> PoliceInsuranceList = pi_BLL.GetByParam(0, "ASC", "Id", search);
            if (PoliceInsuranceList == null || PoliceInsuranceList.Count <= 0)
            {
                Message.Append(string.Format("社保政策 {0} 在系统中不存在;\r\n", p));
            }
            else if (PoliceInsuranceList.Count == 1)
            {
                return PoliceInsuranceList[0].Id;
            }
            else
            {
                Message.Append(string.Format("社保政策 {0} 在系统中重复;\r\n", p));
            }
            return -1;
        }

        private int getPoliceOperationByExcel(EmployeeAddExcelModel1 item, string p, ref StringBuilder Message)
        {
            string search = "NameDDL_String&" + p;
            List<PoliceOperation> PoliceOperationList = po_BLL.GetByParam(0, "ASC", "Id", search);
            if (PoliceOperationList == null || PoliceOperationList.Count <= 0)
            {
                Message.Append(string.Format("政策手续 {0} 在系统中不存在;\r\n", p));
            }
            else if (PoliceOperationList.Count == 1)
            {
                //组建员工企业关系信息
                return PoliceOperationList[0].Id;
            }
            else
            {
                Message.Append(string.Format("政策手续 {0} 在系统中重复;\r\n", p));
            }
            return -1;
        }

        private int getPoliceAccountNatureIdByExcel(EmployeeAddExcelModel1 item, ref StringBuilder Message)
        {
            string search = "NameDDL_String&" + item.PoliceAccountNatureName;
            List<PoliceAccountNature> PoliceAccountNatureList = pan_BLL.GetByParam(0, "ASC", "Id", search);
            if (PoliceAccountNatureList == null || PoliceAccountNatureList.Count <= 0)
            {
                Message.Append("户口性质在系统不存在;\r\n");
            }
            else if (PoliceAccountNatureList.Count == 1)
            {
                //组建员工企业关系信息
                return PoliceAccountNatureList[0].Id;
            }
            else
            {
                Message.Append("户口性质在系统中重复;\r\n");
            }
            return -1;
        }

        private string getCityIdByExcel(EmployeeAddExcelModel1 item, ref StringBuilder Message)
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

        /// <summary>
        /// 根据名称获取供应商ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        private int? getSupplierIdByExcel(EmployeeAddExcelModel1 item, ref StringBuilder Message)
        {
            ISupplierBLL supplier_BLL = new BLL.SupplierBLL();
            string search = "NameDDL_String&" + item.SupplierName;
            List<Supplier> SupplierList = supplier_BLL.GetByParam(string.Empty, "ASC", "Id", search);
            if (SupplierList == null || SupplierList.Count <= 0)
            {
                Message.Append("供应商在系统不存在;\r\n");

            }
            else if (SupplierList.Count == 1)
            {
                //返回供应商ID
                return SupplierList[0].Id;
            }
            else
            {
                Message.Append("供应商在系统重复;\r\n");
            }
            return null;
        }

        private int getCompanyIdByExcel(EmployeeAddExcelModel1 item, ref StringBuilder Message)
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
                //校验单位是否是属于导入的责任客服
                string searchR = "CRM_Company_IDDDL_Int&" + empComList[0].ID + "^UserID_ZRDDL_Int&" + LoginInfo.UserID;
                List<CRM_CompanyToBranch> cr = crmR_BLL.GetByParam(0, "ASC", "Id", searchR);
                if (cr == null || cr.Count <= 0)
                {
                    Message.Append(string.Format("你不能导入{0}的社保;\r\n", item.CompanyName));
                }
                else
                {
                    //组建员工企业关系信息
                    return empComList[0].ID;
                }
            }
            else
            {
                Message.Append("单位在系统重复;\r\n");
            }
            return -1;
        }

        #endregion

        #region 导入验证
        private string verification(DataTable dt)
        {

            return "";
        }
        #endregion
                    #endregion










        //#region 获取保险种类数据
        ///// <summary>
        ///// 获取保险种类数据
        ///// </summary>
        ///// <returns></returns>

        //public ActionResult GetEmpFeedBackSuccessList(string ids)
        //{

        //    string result = string.Empty;
        //    try
        //    {
        //        using (var ent = new SysEntities())
        //        {
        //            // List<string> ids = new List<string>();
        //            ids = ids.TrimEnd(',');
        //            if (ids.Length > 0)
        //            {
        //                List<string> list = new List<string>(ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
        //                List<int> tempNum = list.Select(x => int.Parse(x)).ToList();
        //                var Comp = ent.EmployeeAdd.Where(a => true);
        //                Comp = Comp.Where(a => tempNum.Contains(a.Id));
        //                var InsuranceKind = ent.InsuranceKind.Where(a => true);
        //                var shp = ent.PoliceCascadeRelationship.Where(o => true);
        //                var shp1 = ent.PoliceCascadeRelationship.Where(o => true);


        //                var leftjoin = from a in Comp
        //                               join type in InsuranceKind on a.InsuranceKindId equals type.Id
        //                               join b in shp on type.Id equals b.InsuranceKindId
        //                               select new InsuranceKindtype
        //                               {
        //                                   Tag = b.Tag

        //                               };

        //                var list1 = leftjoin.ToList<InsuranceKindtype>();

        //                List<string> tagstring = new List<string>();
        //                for (int i = 0; i < list1.Count(); i++)
        //                {
        //                    tagstring.Add(list1[i].Tag);
        //                }
        //                var shp11 = ent.PoliceCascadeRelationship.Where(o => true && tagstring.Contains(o.Tag));
        //                var Insurance = ent.InsuranceKind.Where(o => true);

        //                var leftjoin1 = from a in shp11
        //                                join b in Insurance on a.InsuranceKindId equals b.Id

        //                                select new InsuranceKindtype
        //                                {
        //                                    ID = b.Id,
        //                                    Name = b.Name

        //                                };

        //                var list11 = leftjoin1.ToList<InsuranceKindtype>();

        //                List<InsuranceKindtype> empApp = new List<InsuranceKindtype>();

        //                if (list11 != null && list11.Count() >= 1)
        //                {
        //                    foreach (object item in list11)
        //                    {
        //                        Type t = item.GetType();
        //                        InsuranceKindtype temp = new InsuranceKindtype();
        //                        temp.Name = (string)t.GetProperty("Name").GetValue(item, null);
        //                        temp.ID = (int)t.GetProperty("ID").GetValue(item, null);
        //                        empApp.Add(temp);
        //                    }
        //                }
        //                var jsonData = new
        //                {
        //                    total = empApp.Count,// 总行数
        //                    rows = empApp
        //                };

        //                //var listn = leftjoin.ToList<InsuranceKind>();
        //                result = Newtonsoft.Json.JsonConvert.SerializeObject(empApp);
        //            }
        //        }
        //        return Content(result);
        //    }
        //    catch
        //    {

        //        result = "读取失败！";
        //        return Content(result);
        //    }
        //}

        //#endregion
    }



}


        #endregion