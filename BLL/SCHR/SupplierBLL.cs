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
    /// 供应商信息 
    /// </summary>
    public partial class SupplierBLL :  IBLL.ISupplierBLL, IDisposable
    {
        
        /// <summary>
        /// 根据缴纳地，获取所有供应商数据
        /// </summary>
        /// <param name="cityID">缴纳地行政区划</param>
        /// <returns>供应商信息</returns>
        public List<Supplier> GetSupplierByCity(string cityID)
        {
            return repository.GetSupplierByCity(db, cityID).ToList();           
        }

        /// <summary>
        /// 根据供应商名称查询供应商列表
        /// </summary>
        /// <param name="companyName">供应商名称</param>
        /// <returns></returns>
        public List<SupplierView> GetSupplierList(int? id, int page, int rows, string companyName, int? CustomerServiceId, int branchID, ref int total)
        {
            var queryData = repository.GetSupplierList(db, companyName, CustomerServiceId, branchID);
            List<SupplierView> queryList = new List<SupplierView>();
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
        /// 创建新供应商
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="baseModel"></param>
        /// <param name="listLink"></param>
        /// <param name="listBank"></param>
        /// <param name="listBill"></param>
        /// <param name="listPrice"></param>
        /// <param name="listLadderPrice"></param>
        /// <returns></returns>
        public bool CreateNewSupplier(ref ValidationErrors validationErrors, Supplier baseModel, List<SupplierLinkMan> listLink, List<SupplierBankAccount> listBank, List<SupplierBill> listBill, List<LadderLowestPrice> listPrice, List<LadderPrice> listLadderPrice,List<SupplierNatureCity> listCity)
        {
            try
            {
                int result = repository.CreateNewSupplier(baseModel, listLink, listBank, listBill, listPrice, listLadderPrice, listCity);
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }
        /// <summary>
        /// 验证供应商名称唯一性
        /// </summary>
        /// <param name="supplierID"></param>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        public int CheckSupplierName(string supplierID, string supplierName)
        {
            int count = repository.CheckSupplierName(supplierID, supplierName);
            return count;
        }

        public bool EditSupplier(ref ValidationErrors validationErrors,SupplierView model)
        {
            try
            {
                int result = repository.EditSupplier(model);
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }

        /// <summary>
        /// 批量删除供应商（将状态设置为‘停用’）
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <returns></returns>
        public bool DeleteSupplier(ref ValidationErrors validationErrors, int?[] ids)
        {
            return repository.DeleteSupplier(ids);
        }

        /// <summary>
        /// 批量更新供应商客服
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="customerServiceId">供应商客服ID</param>
        /// <returns></returns>
        public bool UpdateCustomerService(ref ValidationErrors validationErrors, int?[] ids, int customerServiceId)
        {
            return repository.UpdateCustomerService(ids,customerServiceId);
        }

         /// <summary>
        /// 根据供应商客服ID获取数据
        /// </summary>
        /// <param name="customerServiceId">供应商客服Id</param>
        /// <returns></returns>
        public int GetSupplierByCustomerServiceId(int customerServiceId)
        {
            List<Supplier> list = repository.GetSupplierByCustomerServiceId(db, customerServiceId).ToList();
            int count = 0;
            if (list != null)
            {
                count = list.Count;
            }
            return count;
        }
    }
}

