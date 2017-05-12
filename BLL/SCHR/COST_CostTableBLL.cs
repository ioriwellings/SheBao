using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;

namespace Langben.BLL
{
    /// <summary>
    /// 费用_费用表 
    /// </summary>
    public partial class COST_CostTableBLL
    {
        /// <summary>
        /// 查询的数据，责任客服费用审核列表(责任客服权限)
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="costTableType">费用表类型</param>
        /// <param name="status">状态</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CostFeeModel> GetCostFeeList(int? id, int page, int rows, string dateTime, int companyId, int costTableType, int status,
            int departmentScope, string departments, int branchID, int departmentID, int userID, ref int total)
        {
            // 获取公司列表（进行权限配置之后的）
            List<int> companyList = new List<int>();
            if (companyId != 0)
            {
                companyList.Add(companyId);
            }
            else
            {
                // 根据权限获取企业列表
                List<CRM_Company> company_list = repository.GetCompanyList(departmentScope, departments, branchID, departmentID, userID);
                foreach (var item in company_list)
                {
                    companyList.Add(item.ID);
                }
            }

            IOrderedQueryable<CostFeeModel> queryData = repository.GetCostFeeList(db, dateTime, companyList, costTableType, status).OrderBy(x => x.Status).ThenBy(x => x.SerialNumber);
            List<CostFeeModel> queryList = new List<CostFeeModel>();
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            return queryList;
        }
        /// <summary>
        /// 供应商费用表
        /// </summary>
        /// <param name="page">当前页数</param>
        /// <param name="rows">每一个多少条</param>
        /// <param name="Search">查询参数</param>
        /// <param name="total">总条数</param>
        /// <returns></returns>
        public List<CostFeeModel> GetAllCostFeeList(int page, int rows, string Search, ref int total)
        {

            IQueryable<CostFeeModel> queryData = repository.GetAllCostFeeList(db, Search, 2, null).OrderBy(x => x.Status).ThenBy(x => x.SerialNumber);
            List<CostFeeModel> queryList = new List<CostFeeModel>();
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            return queryList;
        }
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="costTableType">费用表类型</param>
        /// <param name="status">状态</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CostFeeModel> GetCostFeeList(int? id, int page, int rows, string dateTime, List<int> companyId, int costTableType, int status, ref int total)
        {
            IOrderedQueryable<CostFeeModel> queryData = repository.GetCostFeeList(db, dateTime, companyId, costTableType, status);
            List<CostFeeModel> queryList = new List<CostFeeModel>();
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            return queryList;
        }

        /// <summary>
        /// 查询的数据（财务确认费用表列表）
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="status">状态</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CostFeeModel> GetCostFeeFinanceAduitList(int? id, int page, int rows, string dateTime, int status, ref int total)
        {
            IOrderedQueryable<CostFeeModel> queryData = repository.GetCostFeeFinanceAduitList(db, dateTime, status);
            List<CostFeeModel> queryList = new List<CostFeeModel>();
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            return queryList;
        }

        /// <summary>
        /// 获取费用明细表信息（包含五险一金、其他费用、服务费等）
        /// </summary>
        /// <param name="costId">费用表ID</param>
        /// <returns></returns>
        public List<Cost_CostTableDetails> GetCostFeeDetailList(int costId)
        {
            List<Cost_CostTableDetails> detailList = repository.GetCostFeeDetailList(db, costId);

            return detailList;
        }

        /// <summary>
        /// 根据主键获取一个费用_费用表
        /// </summary>
        /// <param name="id">费用_费用表的主键</param>
        /// <returns>一个费用_费用表</returns>
        public CostFeeModel GetCostFeeModelById(int id)
        {
            return repository.GetCostFeeModelById(db, id);
        }

        /// <summary>
        /// 修改费用表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用_费用表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <returns></returns>  
        public bool UpdateCostTableStatus(ref ValidationErrors validationErrors, int id, int status)
        {
            try
            {
                return repository.UpdateCostTableStatus(id, status) == 1;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        /// <summary>
        /// 费用表作废
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用表主键</param>
        /// <param name="status">要更新成的状态</param>
        /// <returns></returns>
        public bool CancelCostTable(ref ValidationErrors validationErrors, int id, int status)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    repository.CancelCostTable(db, id, status);
                    db.SaveChanges();
                    transactionScope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    validationErrors.Add(ex.Message);
                    ExceptionsHander.WriteExceptions(ex);
                    return false;
                }
            }
        }
        /// <summary>
        /// 批量修改费用表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="ids">费用表主键</param>
        /// <param name="status">要更新成的状态</param>
        /// <returns></returns>
        public bool UpdateCostTableStatusCollection(ref ValidationErrors validationErrors, int[] ids, int status)
        {
            try
            {
                if (ids != null)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        repository.UpdateCostTableStatus(db, ids, status);
                        if (ids.Length == repository.Save(db))
                        {
                            transactionScope.Complete();
                            return true;
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }

        /// <summary>
        /// 获取中间表里的社保费用（含正常和对比）
        /// </summary>
        /// <param name="costTable">主表Id</param>  
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        //public List<COST_CostTableInsurance> Get_List_COST_CostTableInsurance(int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID)
        //{
        //    IQueryable<EmployeeMiddle> employeeMiddle_List = null;
        //    List<COST_CostTableInsurance> queryData = repository.Get_List_COST_CostTableInsurance(COST_CostTable_ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID, out employeeMiddle_List);

        //    return queryData.ToList();
        //}

        /// <summary>
        /// 获取正常服务费用
        /// </summary>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        //public List<COST_CostTableService> Get_List_COST_CostTableService(int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID)
        //{
        //    List<COST_CostTableService> queryData = repository.Get_List_COST_CostTableService(COST_CostTable_ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID);
        //    return queryData.ToList();
        //}

        /// <summary>
        /// 获取中间表里的其他费用和其他社保费用
        /// </summary>
        /// <param name="costTable">主表Id</param>
        /// <param name="CRM_Company_ID">企业Id</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="CreateUserID">创建人Id</param>
        /// <param name="CreateUserName">创建人姓名</param>
        /// <param name="BranchID">所属分支机构</param>
        /// <returns></returns>
        //public List<COST_CostTableOther> Get_List_COST_CostTableOther(int COST_CostTable_ID, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID)
        //{
        //    IQueryable<EmployeeMiddle> employeeMiddle_List = null;
        //    List<COST_CostTableOther> queryData = repository.Get_List_COST_CostTableOther(COST_CostTable_ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID, out employeeMiddle_List);
        //    return queryData.ToList();
        // }
        //生成费用
        public bool Save(ref ValidationErrors validationErrors, COST_CostTable cOST_CostTable, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    //1.主表数据
                    repository.Create(db, cOST_CostTable);
                    db.SaveChanges();

                
                        




                    //2.获取将要插入社保明细费用表中的数据
                    IQueryable<EmployeeMiddle> employeeMiddle_List_Z = null;
                    var list_COST_CostTableInsurance = repository.Get_List_COST_CostTableInsurance(db, cOST_CostTable.ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID, out employeeMiddle_List_Z);
                    //2.2改变中间表的使用记录
                    if (employeeMiddle_List_Z.Count() > 0)
                    {
                        var emlz = new EmployeeMiddleBLL();
                        foreach (var list_emlz in employeeMiddle_List_Z)
                        {
                            list_emlz.UseBetween = yearMonth;
                            list_emlz.BillId = cOST_CostTable.ID.ToString();
                            emlz.Edit(db, list_emlz);
                        }
                    }
                    //3.获取正常服务费用
                    var list_COST_CostTableService = repository.Get_List_COST_CostTableService(db, cOST_CostTable.ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID);
                    //4.1获取其他费用和其他社保费用
                    IQueryable<EmployeeMiddle> employeeMiddle_List_B = null;
                    var list_COST_CostTableOther = repository.Get_List_COST_CostTableOther(db, cOST_CostTable.ID, CRM_Company_ID, yearMonth, CreateUserID, CreateUserName, BranchID, out employeeMiddle_List_B);
                    //4.2改变中间表的使用记录
                    if (employeeMiddle_List_B.Count() > 0)
                    {
                        var emlb = new EmployeeMiddleBLL();
                        foreach (var list_emlb in employeeMiddle_List_B)
                        {
                            list_emlb.UseBetween = yearMonth;
                            list_emlb.BillId = cOST_CostTable.ID.ToString();
                            emlb.Edit(db, list_emlb);
                        }
                    }
                    //5.创建社保费用明细数据（正常、补缴、上月对比）
                    var ccti = new COST_CostTableInsuranceBLL();
                    foreach (var list_ccti in list_COST_CostTableInsurance)
                    {
                        ccti.Create(db, list_ccti);
                    }
                    //6.创建服务费用明细数据（正常、补缴、上月对比）
                    var ccts = new COST_CostTableServiceBLL();
                    foreach (var list_ccts in list_COST_CostTableService)
                    {
                        ccts.Create(db, list_ccts);
                    }
                    //7.创建其他费用和其他社保费用
                    var ccto = new COST_CostTableOtherBLL();
                    foreach (var list_ccto in list_COST_CostTableOther)
                    {
                        ccto.Create(db, list_ccto);
                    }
                    //8.计算总费用
                    cOST_CostTable.ChargeCost = (decimal)(list_COST_CostTableInsurance.Sum(o => o.CompanyCost) + list_COST_CostTableInsurance.Sum(o => o.PersonCost)
                        + list_COST_CostTableService.Sum(o => o.ChargeCost) + list_COST_CostTableOther.Sum(o => o.ChargeCost));
                    //9.最后保存
                    int count = 1 + list_COST_CostTableInsurance.Count() + list_COST_CostTableService.Count
                        + list_COST_CostTableOther.Count + employeeMiddle_List_Z.Count() + employeeMiddle_List_B.Count();//先计算总数共多少个
                    int dbCount = db.SaveChanges();
                    if (count == dbCount)
                    {
                        transactionScope.Complete();
                        return true;
                    }
                    else
                    {
                        Transaction.Current.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Delete(ref validationErrors, cOST_CostTable.ID);
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        /// <summary>
        ///  获取正常服务费用
        /// </summary>
        /// <param name="db"></param>
        /// <param name="count">人数</param>
        /// <param name="payService_One">单人服务费</param>
        /// <param name="COST_CostTable_ID">费用表</param>
        /// <param name="CRM_Company_ID">企业</param>
        /// <param name="CreateUserID">创建人id</param>
        /// <param name="CreateUserName">创建人</param>
        /// <param name="BranchID"></param>
        /// <returns></returns>
        public List<COST_CostTableService> Get_List_COST_CostTableService_Z(int count, decimal payService_One, int COST_CostTable_ID, int CRM_Company_ID, int CreateUserID, string CreateUserName, int BranchID, int[] emploees)
        {
            List<COST_CostTableService> queryData = repository.Get_List_COST_CostTableService_Z(db, count, payService_One, COST_CostTable_ID, CRM_Company_ID, CreateUserID, CreateUserName, BranchID, emploees);
            return queryData.ToList();
        }

        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断,责任客服权限）
        /// </summary>
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        public List<CRM_Company> GetCompanyList(int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            return repository.GetCompanyList(departmentScope, departments, branchID, departmentID, userID);
        }
        public List<Cost_Cost_Company> GetCost_Cost_Company(int Cost_TableID)
        {
            return repository.GetCost_Cost_Company(Cost_TableID);
        }

        /// <summary>
        /// 获取费用表流水号（每月从1开始）
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <returns></returns>
        public string GetSerialNumber(int yearMonth)
        {
            return repository.GetSerialNumber(yearMonth);
        }
    }
}

