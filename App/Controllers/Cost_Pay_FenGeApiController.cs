using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using Langben.App.Models;
using Langben.DAL.Model;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
namespace Langben.App.Controllers
{
    public class Cost_Pay_FenGeApiController : BaseApiController
    {
        IBLL.ICOST_PayRecordStatusBLL m_BLL = new BLL.COST_PayRecordStatusBLL();
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam, string search)
        {

            // 设置搜索默认值
            string yearMonth = DateTime.Now.ToString("yyyy-MM").Replace("-", "");
            int Kinds = 0;
            int? companyId = 0;



            // 各搜索项赋值
            if (!string.IsNullOrEmpty(search))
            {
                string[] searchList = search.Split('^');
                yearMonth = searchList[0].Split('$')[1];
                Kinds = searchList[1].Split('$')[1] != "" ? Convert.ToInt32(searchList[1].Split('$')[1]) : 0;

                if (searchList[2].Split('$')[1] != "" && searchList[2].Split('$')[1] != "0" && searchList[2].Split('$')[1] != "null")
                {
                    companyId = Convert.ToInt32(searchList[2].Split('$')[1]);
                }
            }


            int total = 0;
            List<CostPayFenGe> queryData = m_BLL.GetCostPayFenGeList(Kinds, Convert.ToInt32(yearMonth), companyId, true, getParam.rows, getParam.page, out total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    C_NAME = s.C_NAME
                    ,
                    QIJIAN = s.QIJIAN
                    ,
                    CHARGE_P = s.CHARGE_P
                    ,
                    CHARGE_C = s.CHARGE_C
                    ,
                    工本费 = s.工本费
                    ,
                    H_Sum = s.H_Sum

                })
            };
            return data;
        }



        /// <summary>
        /// 导出费用明细信息
        /// </summary>
        /// <param name="id">费用表ID</param>
        /// <returns></returns>
        public string PostExportToExcel([FromBody]GetDataParam getParam, string search)
        { // 设置搜索默认值
            string yearMonth = DateTime.Now.ToString("yyyy-MM").Replace("-", "");
            int Kinds = 0;
            int? companyId = 0;



            // 各搜索项赋值
            if (!string.IsNullOrEmpty(search))
            {
                string[] searchList = search.Split('^');
                yearMonth = searchList[0].Split('$')[1];
                Kinds = searchList[1].Split('$')[1] != "" ? Convert.ToInt32(searchList[1].Split('$')[1]) : 0;

                if (searchList[2].Split('$')[1] != "" && searchList[2].Split('$')[1] != "0" && searchList[2].Split('$')[1] != "null")
                {
                    companyId = Convert.ToInt32(searchList[2].Split('$')[1]);
                }
            }


            int total = 0;
            List<CostPayFenGe> queryData = m_BLL.GetCostPayFenGeList(Kinds, Convert.ToInt32(yearMonth), companyId, false, 0, 0, out total);
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            using (MemoryStream ms = new MemoryStream())
            {

                FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Template/Excel/社保分割单.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);

                ISheet sheet1 = hssfworkbook.GetSheetAt(0);
                hssfworkbook.SetSheetName(0, yearMonth);

                string kname = Enum.GetName(typeof(Common.CostPay_InsuranceKind), Kinds).ToString();
                sheet1.GetRow(1).GetCell(0).SetCellValue("" + yearMonth + "（" + kname + "）缴费明细表");

                int j = 0;

                //HSSFCellStyle styleRed = CellStyle(hssfworkbook);
                InsertRows(sheet1, 4, queryData.Count() - 1, hssfworkbook);
                for (int i = 0; i < queryData.Count(); i++)
                {
                    IRow row = sheet1.GetRow(4 + i);
                    row.GetCell(0).SetCellValue(i + 1);
                    row.GetCell(1).SetCellValue(queryData[i].CID.ToString());



                    row.GetCell(2).SetCellValue(queryData[i].CW_CID.ToString());  // 身份证号
                    row.GetCell(3).SetCellValue(queryData[i].C_NAME.ToString());  // 供应商
                    row.GetCell(4).SetCellValue(queryData[i].QIJIAN.ToString());  // 缴费类型
                    row.GetCell(5).SetCellValue(queryData[i].CHARGE_C.ToString());  // 缴纳地


                    row.GetCell(6).SetCellValue(queryData[i].CHARGE_P.ToString());
                    row.GetCell(7).SetCellValue(queryData[i].工本费.ToString());
                    row.GetCell(8).SetCellValue(queryData[i].H_Sum.ToString());


                }

                sheet1.ForceFormulaRecalculation = true;
                string fileName = yearMonth + "_" + kname + "分割单" + ".xls";
                string urlPath = "DataExport/" + fileName; // 文件下载的URL地址，供给前台下载
                string filePath = System.Web.HttpContext.Current.Server.MapPath("\\" + urlPath); // 文件路径

                file = new FileStream(filePath, FileMode.Create);
                hssfworkbook.Write(file);
                file.Close();

                return urlPath;  // 导出成功
            }
        }
        void InsertRows(ISheet targetSheet, int fromRowIndex, int rowCount, HSSFWorkbook hssfworkbook)
        {
            if (rowCount != 0)
            {
                targetSheet.ShiftRows(fromRowIndex + 1, targetSheet.LastRowNum, rowCount, true, false);
                IRow xxx = targetSheet.GetRow(fromRowIndex);
                for (int rowIndex = fromRowIndex; rowIndex <= fromRowIndex + rowCount; rowIndex++)
                {
                    IRow rowInsert = targetSheet.CreateRow(rowIndex);

                    rowInsert.Height = xxx.Height;
                    for (int colIndex = 0; colIndex < xxx.LastCellNum; colIndex++)
                    {
                        ICell cellInsert = rowInsert.CreateCell(colIndex);
                        cellInsert.CellStyle = xxx.GetCell(colIndex).CellStyle;
                    }

                    ////合计行
                    //if (rowIndex == fromRowIndex + rowCount)
                    //{
                    //    targetSheet.GetRow(rowIndex).GetCell(4).CellFormula = string.Format("SUM(E{0}:E{1})", fromRowIndex , fromRowIndex + rowCount);
                    //    targetSheet.GetRow(rowIndex).GetCell(5).CellFormula = string.Format("SUM(F{0}:F{1})", fromRowIndex , fromRowIndex + rowCount);
                    //    targetSheet.GetRow(rowIndex).GetCell(6).CellFormula = string.Format("E{0}+F{0}", rowIndex + 1);//费用合计
                    //}
                }
            }
            else
            {
                //int rowIndex = 3; fromRowIndex = 3;
                //targetSheet.GetRow(rowIndex + 1).GetCell(4).CellFormula = string.Format("SUM(E{0}:E{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                //targetSheet.GetRow(rowIndex + 1).GetCell(5).CellFormula = string.Format("SUM(F{0}:F{1})", fromRowIndex + 1, fromRowIndex + rowCount + 1);
                //targetSheet.GetRow(rowIndex + 1).GetCell(6).CellFormula = string.Format("E{0}+F{0}", rowIndex + 2);//费用合计
            }
        }
    }
}
