using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Langben.DAL.Model
{
    public class CRM_CompanyView
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Dict_HY_Code { get; set; }
        public string TaxRegistryNumber { get; set; }
        public string OrganizationCode { get; set; }
        public string RegisterAddress { get; set; }
        public string OfficeAddress { get; set; }
        public int Source { get; set; }
        public Nullable<int> OperateStatus { get; set; }
        public Nullable<int> ParentID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; }

        public int ID { get; set; }
        public int CRM_Company_ID { get; set; }
        public Nullable<int> UserID_YG { get; set; }
        public string UserID_YG_Name { get; set; }
        public Nullable<int> UserID_XS { get; set; }
        public string UserID_XS_Name { get; set; }
        public Nullable<int> UserID_ZR { get; set; }
        public string UserID_ZR_Name { get; set; }
        public int Status { get; set; }
        public int BranchID { get; set; }
    }

    public class CRM_CompanySheBaoInfo
    {
        public string CompanyName { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public int InsuranceKindID { get; set; }//社保种类
        public string InsuranceKindName { get; set; }//社保种类
        public string PoliceInsurance { get; set; }//社保政策
        public string State { get; set; }
        public string CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public string UpdateTime { get; set; }
        public string UpdatePerson { get; set; }
    }
}