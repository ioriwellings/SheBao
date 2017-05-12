using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Common;
using Langben.BLL;
using Langben.DAL;
using Models;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_CompanyContractAPIController : BaseApiController
    {
        IBLL.ICRM_CompanyContractBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyContractAPIController()
            : this(new CRM_CompanyContractBLL()) { }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyContract Get(int id)
        {
            CRM_CompanyContract item = m_BLL.GetById(id);
            return item;
        }

        public CRM_CompanyContractAPIController(CRM_CompanyContractBLL bll)
        {
            m_BLL = bll;
        }
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="order">排序字段</param>
        /// <param name="sort">升序asc（默认）还是降序desc</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
          
            int total = 0;
            List<CRM_CompanyContract> queryData = m_BLL.GetByParam(getParam.id.ToString(), getParam.order, getParam.sort, getParam.search);
            var data = new Common.ClientResult.DataResult
             {
                 total = queryData.Count,
                 rows = queryData.Select(s => new
                 {
                     ID = s.ID
                     ,
                     CRM_Company_ID = s.CRM_Company_ID
                     ,
                     BillDay = s.BillDay
                     ,
                     ReceivedDay = s.ReceivedDay
                     ,
                     FeesCycle = s.FeesCycle
                     ,
                     ChangeDay = s.ChangeDay
                     ,
                     DatumDay = s.DatumDay
                     ,
                     ServceEndDay = s.ServceEndDay
                     ,
                     SendBillDay = s.SendBillDay
                     ,
                     ServiceBeginDay = s.ServiceBeginDay
                     ,
                     Status = s.Status
                     ,
                     BranchID = s.BranchID
                     ,
                     CreateTime = s.CreateTime
                     ,
                     CreateUserID = s.CreateUserID
                     ,
                     CreateUserName = s.CreateUserName

                 })

             };
            return data;
        }

       
    }
}