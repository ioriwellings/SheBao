using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    /// <summary>
    /// 供应商信息 接口
    /// </summary>
    public partial interface ISupplierBLL
    {
          /// <summary>
        /// 根据缴纳地，获取所有供应商数据
        /// </summary>
        /// <param name="cityID">缴纳地行政区划</param>
        /// <returns>供应商信息</returns>
        List<Supplier> GetSupplierByCity(string cityID);
    
                /// <summary>
        /// 根据供应商名称查询供应商列表
        /// </summary>
        /// <param name="companyName">供应商名称</param>
        /// <returns></returns>
        List<SupplierView> GetSupplierList(int? id, int page, int rows, string companyName, int? CustomerServiceId, int branchID, ref int total);
        //创建供应商
        bool CreateNewSupplier(ref Common.ValidationErrors validationErrors, Supplier baseModel, List<SupplierLinkMan> listLink, List<SupplierBankAccount> listBank, List<SupplierBill> listBill, List<LadderLowestPrice> listPrice, List<LadderPrice> listLadderPrice,List<SupplierNatureCity> listCity);
        //验证供应商名称唯一
        int CheckSupplierName(string supplierID, string supplierName);
        //修改基本信息
        bool EditSupplier(ref Common.ValidationErrors validationErrors, SupplierView model);

        /// <summary>
        /// 批量删除供应商（将状态设置为‘停用’）
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <returns></returns>
        bool DeleteSupplier(ref Common.ValidationErrors validationErrors, int?[] ids);

        /// <summary>
        /// 批量更新供应商客服
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="customerServiceId">供应商客服ID</param>
        /// <returns></returns>
        bool UpdateCustomerService(ref ValidationErrors validationErrors, int?[] ids, int customerServiceId);

                 /// <summary>
        /// 根据供应商客服ID获取数据
        /// </summary>
        /// <param name="customerServiceId">供应商客服Id</param>
        /// <returns></returns>
        int GetSupplierByCustomerServiceId(int customerServiceId);

    }
}

