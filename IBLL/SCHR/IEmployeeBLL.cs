using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.IBLL
{
    public partial interface IEmployeeBLL
    {
        List<EmployeeInfo> GetEmployeeList(int page, int rows, string search, ref int count);

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="contactModel"></param>
        /// <param name="bankModel"></param>
        /// <returns></returns>
        bool EmployeeAdd(ref Common.ValidationErrors validationErrors, Employee baseModel, EmployeeContact contactModel, EmployeeBank bankModel, CompanyEmployeeRelation relationModel);

         /// <summary>
        /// 验证身份证号唯一
        /// </summary>
        /// <param name="CertificateNumber">身份证号</param>
        /// <returns></returns>
        int CheckCertificateNumber(string CertificateNumber);
    }
}
