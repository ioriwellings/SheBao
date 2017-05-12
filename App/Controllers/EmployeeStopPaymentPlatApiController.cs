using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using Langben.BLL;
using Langben.DAL;
using Models;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentPlatApiController : BaseApiController
    {
        IBLL.IEmployeeStopPaymentBLL m_BLL = new BLL.EmployeeStopPaymentBLL();

        ValidationErrors validationErrors = new ValidationErrors();

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;

            List<SingleStopPaymentView> queryData = m_BLL.GetListFromP(1, getParam.page, getParam.rows, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new SingleStopPaymentView()
                {
                    CompanyID = s.CompanyID,
                    CompanyName = s.CompanyName,
                    EmployeeID = s.EmployeeID,
                    EmployeeName = s.EmployeeName,
                    CertificateNumber = s.CertificateNumber,
                    CanSotpInsuranceKindName = s.CanSotpInsuranceKindName,
                    EmployeeAddId = s.EmployeeAddId,
                    CompanyEmployeeRelationId = s.CompanyEmployeeRelationId,
                    CityName = s.CityName,
                    YearMonth = s.YearMonth,
                    CanSotpInsuranceKindIDs = s.CanSotpInsuranceKindIDs

                })
            };
            return data;
        }


        /// <summary>
        /// 责任客服操作平台数据：通过
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
          [System.Web.Http.HttpGet]
        public Common.ClientResult.Result PassYes(string IDs)
        {
            bool f= m_BLL.PassStopPaymentRepository(IDs,1);
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            result.Code = Common.ClientCode.Succeed; ;
            result.Message = "操作成功";
            return result;
        
        }
        /// <summary>
        /// 任客服操作平台数据：退回
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
      [System.Web.Http.HttpGet]
        public Common.ClientResult.Result PassNo(string IDs)
        {
            bool f = m_BLL.PassStopPaymentRepository(IDs, 2);
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            result.Code = Common.ClientCode.Succeed; ;
            result.Message = "操作成功";
            return result;

        }
    }
}
