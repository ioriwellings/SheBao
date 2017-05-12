using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    #region 列表展示 添加 用
    public partial class CompanyInsurance
    {
        public Nullable<int> CRM_Company_ID { get; set; }

        public string CityId { get; set; }

        [Display(Name = "缴纳地")]
        public string CityName { get; set; }

        [Display(Name = "工伤政策ID")]
        public string PoliceID1 { get; set; }

        [Display(Name = "公积金政策ID")]
        public string PoliceID2 { get; set; }

        [Display(Name = "工伤政策")]
        public string Police1 { get; set; }

        [Display(Name = "公积金政策")]
        public string Police2 { get; set; }

        [Display(Name = "社保账户")]
        public string Account1 { get; set; }

        [Display(Name = "公积金账户")]
        public string Account2 { get; set; }

        [Display(Name = "状态")]
        public string State { get; set; }

        [Display(Name = "企业社保政策ID集合")]
        public string PoliceIds { get; set; }

        [Display(Name = "企业社保账户ID集合")]
        public string InsuranceIds { get; set; }

    }

    public partial class CRM_Company_Insurance_View
    {
        public int ID { get; set; }
        public Nullable<int> CRM_Company_ID { get; set; }
        public Nullable<int> InsuranceKindId { get; set; }
        public string InsuranceKindName { get; set; }
        public string Account { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdatePerson { get; set; }
    }

    public partial class CRM_Company_PoliceInsurance_View
    {
        public int ID { get; set; }
        public Nullable<int> CRM_Company_ID { get; set; }
        public Nullable<int> InsuranceKind { get; set; }
        public string InsuranceKindName { get; set; }
        public Nullable<int> PoliceInsuranceId { get; set; }
        public string PoliceInsuranceName { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdatePerson { get; set; }
    }
    #endregion

    #region 编辑取值用
    public partial class CompanyInsurance_EditView
    {
        public int CRM_Company_ID { get; set; }
        public string CityId { get; set; }
        public List<CRM_Company_Insurance_View> ListCI { get; set; }
        public List<CRM_Company_PoliceInsurance_View> ListCPI { get; set; }
    }
    #endregion
}
