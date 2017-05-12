using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;
using Models;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentFeedbackController : BaseController
    {
        // GET: EmployeeStopPaymentFeedback
        public ActionResult Index()
        {
            return View();
        }

        #region 社保专员修改
        /// <summary>
        /// 社保专员修改
        /// </summary>
        /// <param name="id">员工企业关系表id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();

        }
 #endregion
        public ActionResult SetSuccess(string stopIds, string stopName, string empName)
        {
            ViewBag.EmpName = empName;
            string[] arrIDs = stopIds.Split(',');
            string[] arrName = stopName.Split(',');
            string html = string.Empty;
            for (int i = 0; i < arrIDs.Count(); i++)
            {
                html += string.Format("<input type='checkbox' id='cbox_{0}'/>{1}", arrIDs[i], arrName[i]);
            }
            ViewBag.strHtml = html;
            return View();
        }

        public ActionResult SetFail(string stopIds, string stopName, string empName)
        {
            ViewBag.EmpName = empName;
            string[] arrIDs = stopIds.Split(',');
            string[] arrName = stopName.Split(',');
            string html = string.Empty;
            for (int i = 0; i < arrIDs.Count(); i++)
            {
                html += string.Format("<input type='checkbox' id='cbox_{0}'/>{1}", arrIDs[i], arrName[i]);
            }
            ViewBag.strHtml = html;
            return View();
        }
    }
}