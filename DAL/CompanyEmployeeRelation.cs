//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Langben.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompanyEmployeeRelation
    {
        public CompanyEmployeeRelation()
        {
            this.EmployeeInsuranceAttachment = new HashSet<EmployeeInsuranceAttachment>();
            this.EmployeeAdd = new HashSet<EmployeeAdd>();
        }
    
        public int Id { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string Remark { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdatePerson { get; set; }
        public string CityId { get; set; }
        public string Station { get; set; }
        public Nullable<int> PoliceAccountNatureId { get; set; }
    
        public virtual ICollection<EmployeeInsuranceAttachment> EmployeeInsuranceAttachment { get; set; }
        public virtual ICollection<EmployeeAdd> EmployeeAdd { get; set; }
    }
}
