using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
     public partial interface ICRM_CompanyLinkManBLL
    {
         //设置默认联系人
         bool SetDefault(ref Common.ValidationErrors validationErrors, int id,int companyID);
    
    }
}

