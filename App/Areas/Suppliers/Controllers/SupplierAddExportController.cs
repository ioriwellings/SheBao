using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierAddExportController : BaseController
    {
        public ActionResult Index()
        {
            IBLL.ISupplierBLL BLL = new BLL.SupplierBLL();

            var SupplierList = BLL.GetByParam("", "ASC", "Id", "CustomerServiceIdDDL_Int&" + LoginInfo.UserID);
            SelectList selSuppliers = new SelectList(SupplierList, "Id", "Name", null);
            ViewBag.listSupplier = selSuppliers;

            return View();
        }

    }
}