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
    
    public partial class ORG_Department
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentType { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string NodePath { get; set; }
        public Nullable<int> LeftValue { get; set; }
        public Nullable<int> RightValue { get; set; }
        public int Sort { get; set; }
        public string XYBZ { get; set; }
    }
}