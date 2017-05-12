using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using Langben.DAL;
using Langben.DAL.Model;
using Models;

namespace Langben.App.Controllers
{
    public class CostPayDuibiApiController : BaseApiController
    {
        ValidationErrors validationErrors = new ValidationErrors();
        IBLL.ICOST_PayRecordBLL m_BLL = new BLL.COST_PayRecordBLL();
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int yearMonthStart = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
            int yearMonthEnd = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
            int costTableType = 1;
            int[] companyId = null;

            // 获取责任客服为当前用户的企业ID
            using (SysEntities db = new SysEntities())
            {
                var query = (from cc in db.CRM_Company
                             join cctb in db.CRM_CompanyToBranch on cc.ID equals cctb.CRM_Company_ID
                             where cctb.UserID_ZR == LoginInfo.UserID
                             select cc.ID).ToList();
                int i = 0;
                int count = query.Count();
                companyId = new int[count];
                foreach (var item in query)
                {
                    companyId[i] = item;
                    i++;
                }
            }

            // 各搜索项赋值
            if (!string.IsNullOrEmpty(getParam.search))
            {//Company&0^costType&1^yearMonthStart&2015-05^yearMonthEnd&2015-06
                string[] search = getParam.search.Split('^');
                yearMonthStart = Convert.ToInt32(search[2].Split('&')[1].Replace("-", ""));
                yearMonthEnd = Convert.ToInt32(search[3].Split('&')[1].Replace("-", ""));
                costTableType = search[1].Split('&')[1] != "0" ? Convert.ToInt32(search[1].Split('&')[1]) : 0;
                if (search[0].Split('&')[1] != "" && search[0].Split('&')[1] != "0")
                {
                    companyId = new int[1];
                    companyId[0] = Convert.ToInt32(search[0].Split('&')[1]);
                }
            }
            int total = 0;
            List<CostPayDuibi> list = m_BLL.GetData(getParam.id, getParam.page, getParam.rows, yearMonthStart, yearMonthEnd, companyId, costTableType, ref total);
            string costtype = ((Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), costTableType.ToString())).ToString();

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = list.Select(s => new
                {
                    ID = s.CompanyId,
                    CompanyName = s.CompanyName,
                    costType = costTableType,
                    CostTableType = costtype,
                    CompanyCost = s.CompanyCost,
                    PersonCost = s.PersonCost,
                    CompanyPay = s.CompanyPay,
                    PersonPay = s.PersonPay,
                    CompanyCha = s.CompanyPay - s.CompanyCost,
                    PersonCha = s.PersonPay - s.PersonCost,
                    Cha = s.CompanyPay - s.CompanyCost + s.PersonPay - s.PersonCost,
                })
            };

            return data;
        }
        /// <summary>
        /// 异步加载费用详情数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostCostDetailData(int id, string yearMonthStart, string yearMonthEnd, int costType)
        {
            int ymstart = Convert.ToInt32(yearMonthStart.Replace("-", ""));
            int ymend = Convert.ToInt32(yearMonthEnd.Replace("-", ""));
            List<CostPayDuibiDetails> list = m_BLL.GetDetails(id, ymstart, ymend, costType);
            string costtype = ((Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), costType.ToString())).ToString();
            var data = new Common.ClientResult.DataResult
             {
                 rows = list.Select(s => new
                 {
                     ID = s.CompanyId,
                     CompanyName = s.CompanyName,
                     CostTableType = costtype,
                     CompanyCost = s.CompanyCost,
                     PersonCost = s.PersonCost,
                     CompanyPay = s.CompanyPay,
                     PersonPay = s.PersonPay,
                     YearMonth = s.YearMonth,
                     CompanyCha = s.CompanyPay - s.CompanyCost,
                     PersonCha = s.PersonPay - s.PersonCost,
                     Cha = s.CompanyPay - s.CompanyCost + s.PersonPay - s.PersonCost
                 })
             };
            return data;
        }
    }
}
