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
    
    public partial class COST_ExpenditureInsurance
    {
        public int ID { get; set; }
        public string 姓名 { get; set; }
        public string IDCard { get; set; }
        public string 缴费社保月 { get; set; }
        public string 缴费基数 { get; set; }
        public string 单位承担 { get; set; }
        public string 个人承担 { get; set; }
        public string 社保月 { get; set; }
        public int Status { get; set; }
        public int CRM_Company_ID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public int BranchID { get; set; }
    }
}