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
    
    public partial class City
    {
        public City()
        {
            this.InsuranceKind = new HashSet<InsuranceKind>();
            this.ORG_UserCity = new HashSet<ORG_UserCity>();
            this.PoliceAccountNature = new HashSet<PoliceAccountNature>();
            this.PoliceAccountNature1 = new HashSet<PoliceAccountNature>();
        }
    
        public string Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdatePerson { get; set; }
    
        public virtual ICollection<InsuranceKind> InsuranceKind { get; set; }
        public virtual ICollection<ORG_UserCity> ORG_UserCity { get; set; }
        public virtual ICollection<PoliceAccountNature> PoliceAccountNature { get; set; }
        public virtual ICollection<PoliceAccountNature> PoliceAccountNature1 { get; set; }
    }
}
