using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using Langben.App.Models;

namespace Langben.App.Controllers
{
    public class BillStatusApiController : BaseApiController
    {
        //  八、	客户端反馈账单状态
        #region
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/billstatus/{id}")]
        public IHttpActionResult Index(int Id, string Status, string Remark)
        {
            if (!IsValidation())
            {
                return Json(new { code = 1, message = "验证未通过" });
            }
            try
            {
                using (SysEntities db = new SysEntities())
                {

                    var costtable = db.COST_CostTable.Where(a => a.ID == Id).FirstOrDefault();
                    //  状态（1：待责任客服验证 2：待客户确认 3：客户作废 4：待责任客服确认 5：责任客服作废 6：待核销 7：财务作废 8：已核销 9：已支付）
                    if (Status.Contains("退回"))
                    {
                        costtable.Status = 3; //客户作废
                    }
                    if (Status.Contains("确认"))
                    {
                        costtable.Status = 4;
                    }
                    costtable.Remark = Remark;
                    db.SaveChanges();


                }
            }
            catch
            {
                throw new Exception();
            }



            return Json(new { code = 0, });
        }



        #endregion

    }
}
