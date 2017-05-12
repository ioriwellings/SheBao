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
    public partial class SupplierLinkManRepository : BaseRepository<SupplierLinkMan>, IDisposable
    {
        /// <summary>
        /// 修改默认联系人
        /// </summary>
        /// <param name="id">联系人表id</param>
        /// <returns></returns>
        public int SetDefault(string id,int supplierID)
        {
            using (SysEntities db = new SysEntities())
            {
                var data = db.SupplierLinkMan.Where(e => (e.SupplierId == supplierID && e.IsDefault == "Y"));
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        item.IsDefault = "N";
                    }
                }
                db.SupplierLinkMan.Where(e => e.Id == id).FirstOrDefault().IsDefault = "Y";
                return db.SaveChanges();
            }
        }
    }
}

