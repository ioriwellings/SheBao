using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.BLL
{
    public partial class SupplierLinkManBLL : IBLL.ISupplierLinkManBLL, IDisposable
    {
        /// <summary>
        /// 设置默认联系人
        /// </summary>
        /// <param name="id">联系人id</param>
        /// <returns></returns>
        public bool SetDefault(ref ValidationErrors validationErrors, string id,int supplierID)
        {
            try
            {
                repository.SetDefault(id,supplierID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
    }
}
