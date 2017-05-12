using Common;
using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.BLL
{
    public partial class EmployeeBLL : IBLL.IEmployeeBLL, IDisposable
    {
        /// <summary>
        /// 查询员工列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeInfo> GetEmployeeList(int page, int rows, string search, ref int total)
        {
            IQueryable<EmployeeInfo> queryData = repository.GetEmployeeList(db, search);
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryData = queryData.Take(rows);
                }
                else
                {
                    queryData = queryData.Skip((page - 1) * rows).Take(rows);
                }

            }
            return queryData.ToList();
        }
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="contactModel"></param>
        /// <param name="bankModel"></param>
        /// <returns></returns>
        public bool EmployeeAdd(ref ValidationErrors validationErrors, Employee baseModel, EmployeeContact contactModel, EmployeeBank bankModel, CompanyEmployeeRelation relationModel)
        {
            try
            {
                int result = repository.EmployeeAdd(baseModel, contactModel, bankModel,relationModel);
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }


        #region yyh 批量添加员工

        public bool EmployeeListAdd(List<EmployeeAddExcle> employeeList, string CreatePerson)
        {
            return repository.EmployeeListAdd(employeeList, CreatePerson);
        }
        #endregion

        /// <summary>
        /// 验证身份证号唯一
        /// </summary>
        /// <param name="CertificateNumber">身份证号</param>
        /// <returns></returns>
        public int CheckCertificateNumber(string CertificateNumber)
        {
            int count = repository.CheckCertificateNumber(CertificateNumber);
            return count;
        }
    }
}
