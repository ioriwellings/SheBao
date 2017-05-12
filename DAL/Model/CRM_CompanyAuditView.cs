using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Langben.DAL.Model
{
    public class CRM_CompanyAuditView
    {
        public int AuditType { get; set; }
        public string AuditTypeName { get; set; }
        public int ID { get; set; }
        public int? MainTableID { get; set; }
        public int? CompanyID { get; set; }
        public string CityID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> UserID_ZR { get; set; }
        public string UserID_ZR_Name { get; set; }
        public Nullable<int> UserID_XS { get; set; }
        public string UserID_XS_Name { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<int> OperateStatus { get; set; }
        public int BranchID { get; set; }

    }
}