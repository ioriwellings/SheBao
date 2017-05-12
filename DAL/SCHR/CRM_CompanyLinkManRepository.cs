using System;
using System.Linq;

namespace Langben.DAL
{
    public partial class CRM_CompanyLinkManRepository
    {
        /// <summary>
        /// 修改默认联系人
        /// </summary>
        /// <param name="id">联系人表id</param>
        /// <returns></returns>
        public int SetDefault(int id,int companyID)
        {
            using (SysEntities db = new SysEntities())
            {
                var data = db.CRM_CompanyLinkMan.Where(e => (e.CRM_Company_ID == companyID && e.IsDefault == "Y"));
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        item.IsDefault = "N";
                    }
                }
                db.CRM_CompanyLinkMan.Where(e => e.ID == id).FirstOrDefault().IsDefault = "Y";
                return db.SaveChanges();
            }
            
          
        }

   
    }
}
