using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;
using Models;
using System.Data;

namespace Langben.App.Controllers
{
    public class EmployeeAddSetFailByExcelController : BaseController
    {
        // GET: EmployeeStopPaymentFeedback
        public ActionResult Index()
        {
            return View();
        }

        #region 导入失败原因
        public ActionResult SetFailByExcel(HttpPostedFileBase files)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            result.Code = Common.ClientCode.Fail;
            result.Message = "导入失败";
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
                    return Json(new { code = result.Code, msg = result.Message });
                }
                else
                {
                    string filename = System.IO.Path.GetFileName(file.FileName);
                    int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                    string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                    int Maxsize = 4000 * 1024;//定义上传文件的最大空间大小为4M
                    //string FileType = ".xls";//定义上传文件的类型字符串
                    string[] FileType = new string[] { ".xls", ".xlsx" };

                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        result.Code = Common.ClientCode.Fail;
                        result.Message = "文件类型不对，只能导入Excel文件";
                        return Json(new { code = result.Code, msg = result.Message });
                    }
                    if (filesize >= Maxsize)
                    {
                        result.Code = Common.ClientCode.Fail;
                        result.Message = "上传文件超过4M，不能上传";
                        //return result;
                        return Json(new { code = 0, msg = result.Message });
                    }
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
                    return Json(new { code = result.Code, msg = result.Message });
                }

                if (table == null && table.Rows.Count <= 0)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "导入Excel中不存在有效信息，请核实！";
                    return Json(new { code = result.Code, msg = result.Message });
                }


                IBLL.IEmployeeAddBLL mbll = new BLL.EmployeeAddBLL();
                string errMsg = mbll.SetAddFailByExcel(table, LoginInfo.UserID, LoginInfo.UserName);

                if (errMsg == "")
                {
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "导入成功！";
                }
                else
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = errMsg;
                }

                return Json(new { code = result.Code, msg = result.Message });
            }
            catch (Exception ex)
            {
                result.Code = Common.ClientCode.Fail;
                result.Message = ex.Message + "导入失败,请认真检查excel";
                return Json(new { code = result.Code, msg = result.Message });
            }

            #endregion
        }
        #endregion
    }
}