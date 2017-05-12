using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;
using Models;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class Cost_CostTable_SupplerController : BaseController
    {
        //string menuId = "1043";   // 菜单“责任客服审核费用”
        string LockButton = "1043-1";//锁定费用表按钮权限码
        string DeleteButton = "1043-2";//作废按钮权限码
        string ExportButton = "1043-3";//导出按钮权限码
        string RemarkButton = "1043-4";//备注按钮权限码
        string DetailButton = "1043-5";//查看详情按钮权限码
        IBLL.ICOST_PayRecordStatusBLL m_BLL = new BLL.COST_PayRecordStatusBLL();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public ActionResult Remark()
        {
            return View();
        }
        public ActionResult List()
        {
            #region 按钮权限
            ViewBag.LockButton = this.MenuOpAuthority(LockButton);
            ViewBag.DeleteButton = this.MenuOpAuthority(DeleteButton);
            ViewBag.ExportButton = this.MenuOpAuthority(ExportButton);
            ViewBag.RemarkButton = this.MenuOpAuthority(RemarkButton);
            ViewBag.DetailButton = this.MenuOpAuthority(DetailButton);
            #endregion
            return View();
        }
        [HttpPost]
        public ActionResult ImportExcel(string yearM, string suppliersid, string CreateFrom)
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


                    string SheetName = Util_XLS.ConvertToSQLSheetName(yearMonth.ToString());
                    // string strSQL = string.Format("select * from {0}", SheetName);
                    DataTable dt = Util_XLS.NpoiReadExcle(savePath, SheetName, true, out outMsg);
                    string Msg = m_BLL.ImportExcelForGYS(dt, Convert.ToInt32(CreateFrom), yearMonth, suppliersId, LoginInfo.UserID, LoginInfo.BranchID, LoginInfo.UserName);
                    if (Msg != "")
                    {
                        return Json(new { Code = 0, Message = Msg });
                    }
                    else
                    {
                        return Json(new { Code = 1, Message = "导入成功" });
                    }


                }
                catch (Exception ex)
                {
                    return Json(new { Code = 1, Message = "导入失败" + ex.ToString() });
                }
            }

        }
        //对比详情页
        public ActionResult Detail()
        {
            ViewBag.Id = "1069";
            return View();
        }
    }
}
