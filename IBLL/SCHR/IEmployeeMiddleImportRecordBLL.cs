
using Langben.DAL;
using System.Collections.Generic;
namespace Langben.IBLL
{
    public partial interface IEmployeeMiddleImportRecordBLL
    {
         List<EmployeeMiddleImportRecord> GetDataByParam(int? id, int page, int rows, string createUser,string beginTime,string endTime , ref int total);
    }
}
