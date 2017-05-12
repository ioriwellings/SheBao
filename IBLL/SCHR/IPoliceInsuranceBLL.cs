using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using System.Data;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    public partial interface IPoliceInsuranceBLL
    {

        //[OperationContract]
        string POSTPoliceCascadeRelationship(string Cityid);
    }
}
