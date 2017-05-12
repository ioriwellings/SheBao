using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.IBLL
{
    public partial interface ISupplierLinkManBLL
    {
        bool SetDefault(ref Common.ValidationErrors validationErrors, string id,int supplierID);
    }
}
