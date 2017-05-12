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
using System.Web.Http;
using Langben.App.Models;
using Langben.DAL.Model;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;


namespace Langben.App.Areas.Suppliers.Controllers
{
    public class Cost_CostTable_SupplerApiController : BaseApiController
    {
        IBLL.ICOST_CostTableBLL m_BLL = new BLL.COST_CostTableBLL();
        /// <summary>
        /// 异步加载数据(费用自检列表，既责任客服费用审核列表)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<CostFeeModel> queryData = m_BLL.GetAllCostFeeList(getParam.page, getParam.rows, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    ID = s.ID
                    ,
                    CostTableType = ((Common.COST_Table_CostTableType)s.CostTableType).ToString()  // 费用表类型
                    ,
                    ChargeCost = s.ChargeCost
                    ,
                    ServiceCost = s.ServiceCost
                    ,
                    Remark = s.Remark
                    ,
                    Status = s.Status  // 费用表状态（实际值，数字）
                    ,
                    StatusName = ((Common.COST_Table_Status)s.Status).ToString()   // 费用表状态(文字)
                    ,
                    CRM_Company_ID = s.CRM_Company_ID
                    ,
                    CreateTime = s.CreateTime
                    ,
                    CreateUserID = s.CreateUserID
                    ,
                    CreateUserName = s.CreateUserName
                    ,
                    BranchID = s.BranchID
                    ,
                    YearMonth = s.YearMonth
                    ,
                    SerialNumber = s.SerialNumber   // 批次号
                    ,
                    CompanyName = s.CompanyName,     // 企业名称
                    CreateFrom = ((Common.CostTable_CreateFrom)s.CreateFrom).ToString() ,
                   Suppler= s.Suppler
                })
            };
            return data;
        }

    }
}
