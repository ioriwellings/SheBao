using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    public partial interface ICOST_PayRecordBLL
    {
        List<CostPayDuibi> GetData(int? id, int page, int rows, int yearMonthSrart, int yearMonthEnd, int[] companyId, int costTableType, ref int total);

        List<CostPayDuibiDetails> GetDetails(int id, int yearMonthSrart, int yearMonthEnd, int costType);
    }
}
