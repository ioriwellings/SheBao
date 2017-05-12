using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;
using System.Linq.Expressions;

namespace Langben.BLL
{
    /// <summary>
    /// 员工费用中间表 
    /// </summary>
    public partial class EmployeeMiddleImportRecordBLL : IBLL.IEmployeeMiddleImportRecordBLL, IDisposable
    {
        public List<EmployeeMiddleImportRecord> GetDataByParam(int? id, int page, int rows, string createUser,string beginTime,string endTime, ref int total)
        {
            List<EmployeeMiddleImportRecord> queryList = new List<EmployeeMiddleImportRecord>();
            var queryData = repository.GetDataByParam(db,createUser,beginTime,endTime);
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }

            }
            return queryList;
        }
    }
}

