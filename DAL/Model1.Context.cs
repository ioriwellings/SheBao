﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SysEntities : DbContext
    {
        public SysEntities()
            : base("name=SysEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UserCityCompany> UserCityCompany { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<CompanyEmployeeRelation> CompanyEmployeeRelation { get; set; }
        public virtual DbSet<COST_CostTable> COST_CostTable { get; set; }
        public virtual DbSet<COST_CostTableInsurance> COST_CostTableInsurance { get; set; }
        public virtual DbSet<COST_CostTableOther> COST_CostTableOther { get; set; }
        public virtual DbSet<COST_CostTableService> COST_CostTableService { get; set; }
        public virtual DbSet<COST_ExpenditureInsurance> COST_ExpenditureInsurance { get; set; }
        public virtual DbSet<COST_PayGongJiJin> COST_PayGongJiJin { get; set; }
        public virtual DbSet<COST_PayGongJiJinBC> COST_PayGongJiJinBC { get; set; }
        public virtual DbSet<COST_PayGongShang> COST_PayGongShang { get; set; }
        public virtual DbSet<COST_PayRecord> COST_PayRecord { get; set; }
        public virtual DbSet<COST_PayRecordStatus> COST_PayRecordStatus { get; set; }
        public virtual DbSet<COST_PayShengYu> COST_PayShengYu { get; set; }
        public virtual DbSet<COST_PayShiYe> COST_PayShiYe { get; set; }
        public virtual DbSet<COST_PayTemporary> COST_PayTemporary { get; set; }
        public virtual DbSet<COST_PayYangLao> COST_PayYangLao { get; set; }
        public virtual DbSet<COST_PayYiLiao> COST_PayYiLiao { get; set; }
        public virtual DbSet<COST_PayYiLiaoDaE> COST_PayYiLiaoDaE { get; set; }
        public virtual DbSet<CRM_Company> CRM_Company { get; set; }
        public virtual DbSet<CRM_Company_Audit> CRM_Company_Audit { get; set; }
        public virtual DbSet<CRM_Company_Insurance> CRM_Company_Insurance { get; set; }
        public virtual DbSet<CRM_Company_Insurance_Audit> CRM_Company_Insurance_Audit { get; set; }
        public virtual DbSet<CRM_Company_PoliceInsurance> CRM_Company_PoliceInsurance { get; set; }
        public virtual DbSet<CRM_Company_PoliceInsurance_Audit> CRM_Company_PoliceInsurance_Audit { get; set; }
        public virtual DbSet<CRM_CompanyBankAccount> CRM_CompanyBankAccount { get; set; }
        public virtual DbSet<CRM_CompanyBankAccount_Audit> CRM_CompanyBankAccount_Audit { get; set; }
        public virtual DbSet<CRM_CompanyContract> CRM_CompanyContract { get; set; }
        public virtual DbSet<CRM_CompanyContract_Audit> CRM_CompanyContract_Audit { get; set; }
        public virtual DbSet<CRM_CompanyFinance_Bill> CRM_CompanyFinance_Bill { get; set; }
        public virtual DbSet<CRM_CompanyFinance_Bill_Audit> CRM_CompanyFinance_Bill_Audit { get; set; }
        public virtual DbSet<CRM_CompanyFinance_Payment> CRM_CompanyFinance_Payment { get; set; }
        public virtual DbSet<CRM_CompanyFinance_Payment_Audit> CRM_CompanyFinance_Payment_Audit { get; set; }
        public virtual DbSet<CRM_CompanyLadderPrice> CRM_CompanyLadderPrice { get; set; }
        public virtual DbSet<CRM_CompanyLadderPrice_Audit> CRM_CompanyLadderPrice_Audit { get; set; }
        public virtual DbSet<CRM_CompanyLinkMan> CRM_CompanyLinkMan { get; set; }
        public virtual DbSet<CRM_CompanyLinkMan_Audit> CRM_CompanyLinkMan_Audit { get; set; }
        public virtual DbSet<CRM_CompanyPrice> CRM_CompanyPrice { get; set; }
        public virtual DbSet<CRM_CompanyPrice_Audit> CRM_CompanyPrice_Audit { get; set; }
        public virtual DbSet<CRM_CompanyToBranch> CRM_CompanyToBranch { get; set; }
        public virtual DbSet<CRM_ZD_HY> CRM_ZD_HY { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAdd> EmployeeAdd { get; set; }
        public virtual DbSet<EmployeeAddHistory> EmployeeAddHistory { get; set; }
        public virtual DbSet<EmployeeBank> EmployeeBank { get; set; }
        public virtual DbSet<EmployeeContact> EmployeeContact { get; set; }
        public virtual DbSet<EmployeeGoonPayment> EmployeeGoonPayment { get; set; }
        public virtual DbSet<EmployeeGoonPayment2> EmployeeGoonPayment2 { get; set; }
        public virtual DbSet<EmployeeGoonPayment3adsa> EmployeeGoonPayment3adsa { get; set; }
        public virtual DbSet<EmployeeGoonPaymentHistory> EmployeeGoonPaymentHistory { get; set; }
        public virtual DbSet<EmployeeInsuranceAttachment> EmployeeInsuranceAttachment { get; set; }
        public virtual DbSet<EmployeeMiddle> EmployeeMiddle { get; set; }
        public virtual DbSet<EmployeeMiddleImportRecord> EmployeeMiddleImportRecord { get; set; }
        public virtual DbSet<EmployeeStopPayment> EmployeeStopPayment { get; set; }
        public virtual DbSet<EmployeeStopPaymentHistory> EmployeeStopPaymentHistory { get; set; }
        public virtual DbSet<InsuranceKind> InsuranceKind { get; set; }
        public virtual DbSet<LadderLowestPrice> LadderLowestPrice { get; set; }
        public virtual DbSet<LadderPrice> LadderPrice { get; set; }
        public virtual DbSet<ORG_Group> ORG_Group { get; set; }
        public virtual DbSet<ORG_GroupUser> ORG_GroupUser { get; set; }
        public virtual DbSet<ORG_Menu> ORG_Menu { get; set; }
        public virtual DbSet<ORG_MenuOp> ORG_MenuOp { get; set; }
        public virtual DbSet<ORG_Role> ORG_Role { get; set; }
        public virtual DbSet<ORG_RoleDepartmenScopetAuthority> ORG_RoleDepartmenScopetAuthority { get; set; }
        public virtual DbSet<ORG_RoleDepartmentAuthority> ORG_RoleDepartmentAuthority { get; set; }
        public virtual DbSet<ORG_RoleMenuOpAuthority> ORG_RoleMenuOpAuthority { get; set; }
        public virtual DbSet<ORG_UserCity> ORG_UserCity { get; set; }
        public virtual DbSet<ORG_UserDepartmenScopetAuthority> ORG_UserDepartmenScopetAuthority { get; set; }
        public virtual DbSet<ORG_UserDepartmentAuthority> ORG_UserDepartmentAuthority { get; set; }
        public virtual DbSet<ORG_UserInsuranceKind> ORG_UserInsuranceKind { get; set; }
        public virtual DbSet<ORG_UserMenuOpAuthority> ORG_UserMenuOpAuthority { get; set; }
        public virtual DbSet<ORG_UserRole> ORG_UserRole { get; set; }
        public virtual DbSet<PoliceAccountNature> PoliceAccountNature { get; set; }
        public virtual DbSet<PoliceCascadeRelationship> PoliceCascadeRelationship { get; set; }
        public virtual DbSet<PoliceInsurance> PoliceInsurance { get; set; }
        public virtual DbSet<PoliceInsurance_Four_Five> PoliceInsurance_Four_Five { get; set; }
        public virtual DbSet<PoliceMasterRelationship> PoliceMasterRelationship { get; set; }
        public virtual DbSet<PoliceOperation> PoliceOperation { get; set; }
        public virtual DbSet<PoliceOperationPoliceInsurancePoliceAccountNature> PoliceOperationPoliceInsurancePoliceAccountNature { get; set; }
        public virtual DbSet<PoliceOperationPoliceInsurancePoliceAccountNature2> PoliceOperationPoliceInsurancePoliceAccountNature2 { get; set; }
        public virtual DbSet<PoliceRejectRelationship> PoliceRejectRelationship { get; set; }
        public virtual DbSet<PRD_Product> PRD_Product { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<SupplierBankAccount> SupplierBankAccount { get; set; }
        public virtual DbSet<SupplierBill> SupplierBill { get; set; }
        public virtual DbSet<SupplierLinkMan> SupplierLinkMan { get; set; }
        public virtual DbSet<SupplierNatureCity> SupplierNatureCity { get; set; }
        public virtual DbSet<Allot> Allot { get; set; }
        public virtual DbSet<ORG_Department> ORG_Department { get; set; }
        public virtual DbSet<ORG_User> ORG_User { get; set; }
    
        public virtual ObjectResult<Nullable<int>> FillPayTable(Nullable<int> yearMonth, string batchGuid, Nullable<int> suppliersId, Nullable<int> userId, string userName, Nullable<int> branchId)
        {
            var yearMonthParameter = yearMonth.HasValue ?
                new ObjectParameter("yearMonth", yearMonth) :
                new ObjectParameter("yearMonth", typeof(int));
    
            var batchGuidParameter = batchGuid != null ?
                new ObjectParameter("batchGuid", batchGuid) :
                new ObjectParameter("batchGuid", typeof(string));
    
            var suppliersIdParameter = suppliersId.HasValue ?
                new ObjectParameter("suppliersId", suppliersId) :
                new ObjectParameter("suppliersId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("userName", userName) :
                new ObjectParameter("userName", typeof(string));
    
            var branchIdParameter = branchId.HasValue ?
                new ObjectParameter("branchId", branchId) :
                new ObjectParameter("branchId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("FillPayTable", yearMonthParameter, batchGuidParameter, suppliersIdParameter, userIdParameter, userNameParameter, branchIdParameter);
        }
    
        public virtual int Pro_GetUserAuthority(Nullable<int> userID)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("userID", userID) :
                new ObjectParameter("userID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Pro_GetUserAuthority", userIDParameter);
        }
    
        public virtual int SYS_AddORG_Department(string code, string departmentName, Nullable<int> departmentType, Nullable<int> branchID, Nullable<int> parentID, string nodePath, string desc)
        {
            var codeParameter = code != null ?
                new ObjectParameter("Code", code) :
                new ObjectParameter("Code", typeof(string));
    
            var departmentNameParameter = departmentName != null ?
                new ObjectParameter("DepartmentName", departmentName) :
                new ObjectParameter("DepartmentName", typeof(string));
    
            var departmentTypeParameter = departmentType.HasValue ?
                new ObjectParameter("DepartmentType", departmentType) :
                new ObjectParameter("DepartmentType", typeof(int));
    
            var branchIDParameter = branchID.HasValue ?
                new ObjectParameter("BranchID", branchID) :
                new ObjectParameter("BranchID", typeof(int));
    
            var parentIDParameter = parentID.HasValue ?
                new ObjectParameter("ParentID", parentID) :
                new ObjectParameter("ParentID", typeof(int));
    
            var nodePathParameter = nodePath != null ?
                new ObjectParameter("NodePath", nodePath) :
                new ObjectParameter("NodePath", typeof(string));
    
            var descParameter = desc != null ?
                new ObjectParameter("Desc", desc) :
                new ObjectParameter("Desc", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SYS_AddORG_Department", codeParameter, departmentNameParameter, departmentTypeParameter, branchIDParameter, parentIDParameter, nodePathParameter, descParameter);
        }
    
        public virtual int ORG_GroupUser_AddSubNode(Nullable<int> iD, string oRG_Group_Code, Nullable<int> oRG_User_ID)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var oRG_Group_CodeParameter = oRG_Group_Code != null ?
                new ObjectParameter("ORG_Group_Code", oRG_Group_Code) :
                new ObjectParameter("ORG_Group_Code", typeof(string));
    
            var oRG_User_IDParameter = oRG_User_ID.HasValue ?
                new ObjectParameter("ORG_User_ID", oRG_User_ID) :
                new ObjectParameter("ORG_User_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ORG_GroupUser_AddSubNode", iDParameter, oRG_Group_CodeParameter, oRG_User_IDParameter);
        }
    
        public virtual int ORG_GroupUser_DelNode(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ORG_GroupUser_DelNode", iDParameter);
        }
    }
}
