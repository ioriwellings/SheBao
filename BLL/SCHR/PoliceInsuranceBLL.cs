using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Langben.DAL;
using Common;
using System.Data;
using Langben.DAL.Model;

namespace Langben.BLL
{
    public partial class PoliceInsuranceBLL : IBLL.IPoliceInsuranceBLL, IDisposable
    {
        public string POSTPoliceCascadeRelationship(string Cityid)
        {
            return repository.POSTPoliceCascadeRelationship(db, Cityid);
        }
    }
}
