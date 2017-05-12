using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Langben.DAL.Model;
using Langben.DAL;

namespace Langben.BLL
{
    public partial class COST_PayRecordBLL
    {
        public List<CostPayDuibi> GetData(int? id, int page, int rows, int yearMonthSrart, int yearMonthEnd, int[] companyId, int costTableType, ref int total)
        {
            try
            {
                List<CostPayDuibi> queryData = repository.GetCostPay(db, yearMonthSrart, yearMonthEnd, companyId, costTableType);

                total = queryData.Count();
                if (total > 0)
                {
                    if (page <= 1)
                    {
                        queryData = queryData.Take(rows).ToList();
                    }
                    else
                    {
                        queryData = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                    }

                }

                return queryData.ToList();
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public List<CostPayDuibiDetails> GetDetails(int id, int yearMonthSrart, int yearMonthEnd, int costType)
        {
            List<CostPayDuibiDetails> querydata = repository.GetCostPay(db, yearMonthSrart, yearMonthEnd, id, costType);
            return querydata;
        }
    }
}
