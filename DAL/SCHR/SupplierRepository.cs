using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
using Langben.DAL.Model;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Data.Entity;
namespace Langben.DAL
{
    /// <summary>
    /// 供应商信息
    /// </summary>
    public partial class SupplierRepository : BaseRepository<Supplier>, IDisposable
    {
        /// <summary>
        /// 根据缴纳地，获取所有供应商数据
        /// </summary>
        /// <param name="cityID">缴纳地行政区划</param>
        /// <returns></returns>
        public IQueryable<Supplier> GetSupplierByCity(SysEntities db, string cityID)
        {
            return from m in db.Supplier
                   join f in db.SupplierNatureCity on m.Id equals f.SupplierId
                   where f.NatureCityId == cityID
                   select m;
        }

        /// <summary>
        /// 根据供应商名称查询供应商列表
        /// </summary>
        /// <param name="companyName">供应商名称</param>
        /// <returns></returns>
        public IQueryable<SupplierView> GetSupplierList(SysEntities db, string companyName, int? CustomerServiceId, int branchID)
        {
            var query = from a in db.Supplier
                        join b in db.ORG_User on a.CustomerServiceId equals b.ID into tmpkf
                        from kf in tmpkf.DefaultIfEmpty()
                        where a.Name.Contains(companyName) && 
                        (CustomerServiceId == null || a.CustomerServiceId == CustomerServiceId)
                        && a.Status=="启用" 
                        select new SupplierView()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Name = a.Name,
                            Status = a.Status,
                            CreateTime = a.CreateTime,
                            CustomerServiceId = a.CustomerServiceId,
                            CustomerServiceName = kf.RName
                        };
            return query.OrderBy(e => e.Id);
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
        /// <returns>1:成功 2:失败</returns>
        public int CreateNewSupplier(Supplier baseModel, List<SupplierLinkMan> listLink, List<SupplierBankAccount> listBank, List<SupplierBill> listBill, List<LadderLowestPrice> listPrice, List<LadderPrice> listLadderPrice,List<SupplierNatureCity> listCity)
        {
            using (SysEntities db = new SysEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //创建供应商
                        db.Supplier.Add(baseModel);
                        db.SaveChanges();
                        //添加联系人
                        if (listLink != null && listLink.Count > 0)
                        {
                            listLink = GetList(listLink, baseModel.Id);
                            db.SupplierLinkMan.AddRange(listLink);
                        }
                        //添加银行
                        if (listBank != null && listBank.Count > 0)
                        {
                            listBank = GetList(listBank, baseModel.Id);
                            db.SupplierBankAccount.AddRange(listBank);
                        }
                        //添加开票信息
                        if (listBill != null && listBill.Count > 0)
                        {
                            listBill = GetList(listBill, baseModel.Id);
                            db.SupplierBill.AddRange(listBill);
                        }
                        //添加报价信息
                        if (listPrice != null && listPrice.Count > 0)
                        {
                            listPrice = GetList(listPrice, baseModel.Id);
                            db.LadderLowestPrice.AddRange(listPrice);
                        }
                        //添加阶梯报价信息
                        if (listLadderPrice != null && listLadderPrice.Count > 0)
                        {
                            listLadderPrice = GetLadderList(listLadderPrice, listPrice[0].Id);
                            db.LadderPrice.AddRange(listLadderPrice);
                        }
                        //添加缴纳地
                        if (listCity != null && listCity.Count > 0)
                        {
                            listCity = GetList(listCity, baseModel.Id);
                            db.SupplierNatureCity.AddRange(listCity);
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 验证供应商名是否唯一
        /// </summary>
        /// <param name="supplierID"></param>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        public int CheckSupplierName(string supplierID, string supplierName)
        {
            using (SysEntities db = new SysEntities())
            {
                var list = db.Supplier.Where(e => e.Name.Equals(supplierName));
                if (!string.IsNullOrEmpty(supplierID))
                {
                    int id = int.Parse(supplierID);
                    list = list.Where(e => e.Id != id);
                }
                return list.Count();
            }
        }
        /// <summary>
        /// 修改供应商基本信息
        /// </summary>
        /// <param name="viewModel">供应商</param>
        /// <returns></returns>
        public int EditSupplier(SupplierView viewModel)
        {
            using (SysEntities db = new SysEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //基本信息
                        var entity = db.Supplier.Where(e => e.Id == viewModel.Id).FirstOrDefault();
                        entity.Code = viewModel.Code;
                        entity.Name = viewModel.Name;
                        entity.OfficeAddress = viewModel.OfficeAddress;
                        entity.OrganizationCode = viewModel.OrganizationCode;
                        entity.RegisterAddress = viewModel.RegisterAddress;
                       
                        //社保缴纳地
                        var oldCityList = db.SupplierNatureCity.Where(e => e.SupplierId == viewModel.Id).ToList();
                        string[] arrOldCity = new string[oldCityList.Count()];//旧城市ID数组
                        for (int i = 0; i < oldCityList.Count(); i++)
                        {
                            arrOldCity[i] = oldCityList[i].NatureCityId;
                        }

                        List<SupplierNatureCity> newCityList = GetCityList(viewModel.Id, viewModel.NatureCityId, arrOldCity);
                        if (newCityList.Count>0)
                        {
                            db.SupplierNatureCity.RemoveRange(oldCityList);
                            db.SupplierNatureCity.AddRange(newCityList);      
                        }

                        db.SaveChanges();
                        tran.Commit();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 批量删除供应商（将状态设置为‘停用’）
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <returns></returns>
        public bool DeleteSupplier(int?[] ids)
        {
            using (SysEntities db = new SysEntities())
            {
                try
                {
                    var query = from supplier in db.Supplier
                                where ids.Contains(supplier.Id)
                                select supplier;
                    var f = query.ToList();
                    foreach (var item in f)
                    {
                        item.Status = "停用";
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }


        }

        /// <summary>
        /// 批量更新供应商客服
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="customerServiceId">供应商客服ID</param>
        /// <returns></returns>
        public bool UpdateCustomerService(int?[] ids, int customerServiceId)
        {
            using (SysEntities db = new SysEntities())
            {
                try
                {
                    var query = from supplier in db.Supplier
                                where ids.Contains(supplier.Id)
                                select supplier;
                    var f = query.ToList();
                    foreach (var item in f)
                    {
                        item.CustomerServiceId = customerServiceId;
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }


        }

        /// <summary>
        /// 根据供应商客服ID获取数据
        /// </summary>
        /// <param name="customerServiceId">供应商客服Id</param>
        /// <returns></returns>
        public IQueryable<Supplier> GetSupplierByCustomerServiceId(SysEntities db, int customerServiceId)
        {
            return from m in db.Supplier
                   where m.CustomerServiceId == customerServiceId
                   select m;
        }

        #region 内置
        //得到联系人信息
        private List<LadderPrice> GetLadderList(List<LadderPrice> listLink, string lowestPriceID)
        {
            for (int i = 0; i < listLink.Count(); i++)
            {
                listLink[i].Id = Common.Result.GetNewId();
                listLink[i].LadderLowestPriceId = lowestPriceID;
            }
            return listLink;
        }
        //得到联系人/银行信息.....公用方法
        private List<T> GetList<T>(List<T> list, int supplierID)
        {
            List<T> dynList = new List<T>();
            for (int i = 0; i < list.Count(); i++)
            {
                dynamic model = list[i];
                model.SupplierId = supplierID;
                model.Id = Common.Result.GetNewId();
                dynList.Add(model);
            }
            return dynList;
        }
        //得到Supplier Model
        private Supplier GetModel(SupplierView viewModel)
        {
            Supplier model = new Supplier();
            if (viewModel != null)
            {
                string[] arrField = new string[] { "Code", "Name", "OrganizationCode", "RegisterAddress", "OfficeAddress", "CustomerServiceId", "Status", "CreateTime", "CreateUserID", "CreateUserName" };//排除的字段
                Type t1 = typeof(SupplierView);
                PropertyInfo[] propertys1 = t1.GetProperties();
                Type t2 = typeof(Supplier);
                PropertyInfo[] propertys2 = t2.GetProperties();

                foreach (PropertyInfo pi in propertys2)
                {
                    string name = pi.Name;
                    if (arrField.Contains(name))
                    {
                        object value = t1.GetProperty(name).GetValue(viewModel, null);
                        t2.GetProperty(name).SetValue(model, value, null);
                    }
                }
            }
            return model;
        }
        //得到缴纳地list
        private List<SupplierNatureCity> GetCityList(int supplierID, string cityIDList, string[] arrOldCity)
        {
            List<SupplierNatureCity> list = new List<SupplierNatureCity>();


            string[] arrCity = cityIDList.Split(',');

            for (int i = 0; i < arrCity.Length; i++)
            {
                SupplierNatureCity model = new SupplierNatureCity();
                if (arrCity.Length != arrOldCity.Length)//长度不相等，直接赋新list
                {
                    model.Id = Common.Result.GetNewId();
                    model.NatureCityId = arrCity[i];
                    model.SupplierId = supplierID;
                    list.Add(model);
                }
                else  //长度相等，判断缴纳地是否完全一致
                {
                    if (!arrOldCity.Contains(arrCity[i]))
                    {
                        model.Id = Common.Result.GetNewId();
                        model.NatureCityId = arrCity[i];
                        model.SupplierId = supplierID;
                        list.Add(model);
                    }
                }
            }

            return list;
        }
        #endregion
    }
}

