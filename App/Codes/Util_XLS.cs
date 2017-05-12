#region License
/*
 * ========================================================================
 * Copyright(c) 2011    河北搜才人力资源有限公司, All Rights Reserved.
 * ========================================================================
 * 作者：[中文姓名]   时间：2011-4-11 16:35:23
 * 文件名：Util_XLS
 * 版本：V1.0.0
 * ======================================================================== 
 * 修改者：         时间：               
 * 修改说明：
 * ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Data.SqlClient;
using System.ComponentModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
/// <summary>
///Util_XLS 的摘要说明
/// </summary>
public class Util_XLS
{
    public Util_XLS()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="ServerFileName">xls文件路径</param>
    /// <param name="SelectSQL">查询SQL语句</param>
    /// <returns>DataSet</returns>
    public static DataSet SelectFromXLS(string ServerFileName, string SelectSQL, out string returnMsg)
    {
        /*
        * 链接字符串8.0此连接只能操作Excel2007之前(.xls)文件
        * 12.0支持连接可以操作.xls与.xlsx文件 (支持Excel2003 和 Excel2007 的连接字符串)
        * 备注： "HDR=yes;"是说Excel文件的第一行是列名而不是数据，"HDR=No;"正好与前面的相反。
        * "IMEX=1 "如果列中的数据类型不一致，使用"IMEX=1"可必免数据类型冲突。 
        */

        //string connStr = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = '" + ServerFileName + "';Extended Properties='Excel 8.0'";
        //string connStr = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = '" + ServerFileName + "';Extended Properties='Excel 8.0;HDR=NO;IMEX=1'";
        //string connStr = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = '" + ServerFileName + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
        string connStr = "Provider = Microsoft.Ace.OleDb.12.0; Data source='" + ServerFileName + "';Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此连接可以操作.xls与.xlsx文件
        returnMsg = "";
        OleDbConnection conn = new OleDbConnection(connStr);
        OleDbDataAdapter da = null;
        DataSet ds = new DataSet();
        try
        {
            conn.Open();
            da = new OleDbDataAdapter(SelectSQL, conn);
            da.Fill(ds, "SelectResult");
        }
        catch (Exception e)
        {
            conn.Close();
            returnMsg = e.Message;
        }
        finally
        {
            conn.Close();

        }
        return ds;

    }

    /// <summary>
    /// 获取工作表对应的SQL表名
    /// </summary>
    /// <param name="SheetName">工作表名</param>
    /// <returns>SQL表名</returns>
    public static string ConvertToSQLSheetName(string SheetName)
    {
        return "[" + SheetName + "$]";
    }

    /// <summary>
    /// 执行无返回查询
    /// </summary>
    /// <param name="ServerFileName">xls文件路径</param>
    /// <param name="QuerySQL">待执行的SQL语句</param>
    public static void ExcuteNonQuery(string ServerFileName, string QuerySQL)
    {
        string connStr = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = '" + ServerFileName + "';Extended Properties=Excel 8.0";

        OleDbConnection conn = new OleDbConnection(connStr);
        OleDbCommand cmd = new OleDbCommand(QuerySQL, conn);
        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (Exception AnyError)
        {
            conn.Close();
            throw AnyError;
        }
        finally
        {
            conn.Close();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ServerFileName"></param>
    /// <returns></returns>
    public static DataTable GetSheetName(string ServerFileName)
    {
        string connStr = "Provider = Microsoft.Ace.OleDb.12.0; Data source='" + ServerFileName + "';Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此连接可以操作.xls与.xlsx文件

        // string connStr = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = '" + ServerFileName + "';Extended Properties=Excel 8.0";
        OleDbConnection conn = new OleDbConnection(connStr);
        DataTable dt = new DataTable();
        try
        {
            conn.Open();
            dt = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
        }
        catch (Exception AnyError)
        {
            conn.Close();
            throw AnyError;
        }
        finally
        {
            conn.Close();
        }
        return dt;
        //string SpreadSheetName = "[" + dtExcel.Rows[0]["TABLE_NAME"].ToString() + "]";

    }

    //根据Excel物理路径获取Excel文件中所有表名

    public static String[] GetExcelSheetNames(string excelFile)
    {
        OleDbConnection objConn = null;
        System.Data.DataTable dt = null;

        try
        {

            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + excelFile + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'"; //此连接可以操作.xls与.xlsx文件
            objConn = new OleDbConnection(strConn);
            objConn.Open();
            dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt == null)
            {
                return null;
            }
            String[] excelSheets = new String[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                excelSheets[i] = row["TABLE_NAME"].ToString();
                i++;
            }

            return excelSheets;
        }
        catch
        {
            return null;
        }
        finally
        {
            if (objConn != null)
            {
                objConn.Close();
                objConn.Dispose();
            }
            if (dt != null)
            {
                dt.Dispose();
            }
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filename">待删除的文件名</param>
    public static void DeleteFile(string filename)
    {
        if (filename != string.Empty && System.IO.File.Exists(filename))
        {
            System.IO.File.Delete(filename);
        }
    }


    /// <summary>
    /// 上传Excel文件
    /// </summary>
    /// <param name="inputfile">上传的控件名</param>
    /// <returns></returns>
    public static string UpLoadXls(HttpPostedFileBase inputfile, string uploadfilepath, out string returnMsg)
    {
        string orifilename = string.Empty;
        string modifyfilename = string.Empty;
        string fileExt = "";//文件扩展名
        int fileSize = 0;//文件大小
        returnMsg = "";
        try
        {
            if (inputfile != null)
            {
                //得到文件的大小
                fileSize = inputfile.ContentLength;
                if (fileSize == 0)
                {
                    throw new Exception("导入的Excel文件大小为0，请检查是否正确！");
                }
                //得到扩展名
                fileExt = System.IO.Path.GetExtension(inputfile.FileName);
                if (fileExt.ToLower() != ".xls" && fileExt.ToLower() != ".xlsx")
                {
                    throw new Exception("你选择的文件格式不正确，只能导入EXCEL文件！");
                }
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(inputfile.FileName);//获取无扩展名的文件名
                //新文件名
                modifyfilename = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss_") + System.Guid.NewGuid().ToString() + fileExt;
                //判断是否有该目录
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(uploadfilepath);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                orifilename = uploadfilepath + modifyfilename;
                // 上传文件
                inputfile.SaveAs(orifilename);
            }
            else
            {
                returnMsg = "请选择要导入的Excel文件!";
            }
        }
        catch (Exception ex)
        {
            return returnMsg = ex.ToString();
        }
        return orifilename;
    }
    /// <summary>
    /// List批量插入数据库数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection">数据库连接</param>
    /// <param name="tableName">数据库表名</param>
    /// <param name="list">要存入的数据源 List</param>
    public static void BulkInsert<T>(string connection, string tableName, IList<T> list, string[] st)
    {
        using (var bulkCopy = new SqlBulkCopy(connection))
        {
            bulkCopy.BatchSize = list.Count;
            bulkCopy.DestinationTableName = tableName;

            var table = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof(T))
                //Dirty hack to make sure we only have system data types 
                //i.e. filter out the relationships/collections
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();

            foreach (var propertyInfo in props)
            {
                bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            }

            var values = new object[props.Length];
            foreach (var item in list)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }
            //因为此批量插入偶尔会出错重复数据，为了保证数据的单一性，在此把table里的数据进行去重后再执行批量插入操作

            DataView dv = new DataView(table);
            DataTable dt = dv.ToTable(true, st);

            bulkCopy.WriteToServer(dt);
        }
    }

    #region 下载模板
    /// <summary>
    /// 下载模板
    /// </summary>
    /// <param name="filename">模板名称</param>
    public static void Templatedownload(string filename)
    {
        string fileName = "" + filename + ".xlsx";//客户端保存的文件名
        string filePath = System.Web.HttpContext.Current.Server.MapPath("../MBXZ_Excel/" + filename + ".xlsx");//路径
        //以字符流的形式下载文件
        FileStream fs = new FileStream(filePath, FileMode.Open);
        byte[] bytes = new byte[(int)fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();
        System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
        //通知浏览器下载文件而不是打开
        System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
        System.Web.HttpContext.Current.Response.BinaryWrite(bytes);
        System.Web.HttpContext.Current.Response.Flush();
        System.Web.HttpContext.Current.Response.End();
    }
    #endregion

    /// <summary>
    /// Npoi读取Excel
    /// </summary>
    /// <param name="savePath">Excel地址</param>
    /// <param name="sheetName">表名</param>
    /// <param name="isFirstRowColumn">第一行是否为列名</param>
    /// <param name="outMsg">错误信息</param>
    /// <returns></returns>
    public static DataTable NpoiReadExcle(string savePath, string sheetName,bool isFirstRowColumn, out string outMsg)
    {
        outMsg = "";
        IWorkbook workbook = null;
        FileStream fs = null;
        ISheet sheet = null;

        DataTable data = new DataTable();
        int startRow = 0;
        try
        {
            fs = new FileStream(savePath, FileMode.Open, FileAccess.Read);
            if (savePath.EndsWith(".xlsx")) // 2007版本
                workbook = new XSSFWorkbook(fs);
            else if (savePath.EndsWith(".xls")) // 2003版本
                workbook = new HSSFWorkbook(fs);

            if (sheetName != null)
            {
                sheet = workbook.GetSheet(sheetName);
                if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                {
                    sheet = workbook.GetSheetAt(0);
                }
            }
            else
            {
                sheet = workbook.GetSheetAt(0);
            }
            if (sheet != null)
            {
                IRow firstRow = sheet.GetRow(0);
                int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                if (isFirstRowColumn)
                {
                    for (int i = firstRow.FirstCellNum; i <= cellCount; ++i)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }
                    startRow = sheet.FirstRowNum + 1;
                }
                else
                {
                    startRow = sheet.FirstRowNum;
                }

                //最后一列的标号
                int rowCount = sheet.LastRowNum;
                for (int i = startRow; i <= rowCount; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue; //没有数据的行默认是null　　

                    //当首列为空时,则认为此行以及此行之后为空

                    if (row.GetCell(row.FirstCellNum) != null)
                    {
                        if (string.IsNullOrWhiteSpace(row.GetCell(row.FirstCellNum).ToString()))
                        {
                            continue;
                        }
                    }


                    DataRow dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            dataRow[j] = row.GetCell(j).ToString();
                    }
                    data.Rows.Add(dataRow);
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            outMsg = ex.Message.ToString();
            return null;
        }
    }


    /// <summary>
    /// Npoi读取Excel 赫
    /// </summary>
    /// <param name="savePath"></param>
    /// <param name="SheetName"></param>
    /// <param name="outMsg"></param>
    /// <returns></returns>
    public static DataTable NpoiReadExcle2(string savePath, string sheetName, out string outMsg)
    {
        outMsg = "";
        DataTable table = new DataTable();
        outMsg = "";
        IWorkbook workbook = null;
        FileStream fs = null;
        ISheet sheet = null;

        DataTable data = new DataTable();
        int startRow = 0;
        try
        {
            fs = new FileStream(savePath, FileMode.Open, FileAccess.Read);
            if (savePath.EndsWith(".xlsx")) // 2007版本
                workbook = new XSSFWorkbook(fs);
            else if (savePath.EndsWith(".xls")) // 2003版本
                workbook = new HSSFWorkbook(fs);

            if (sheetName != null)
            {
                sheet = workbook.GetSheet(sheetName);
                if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                {
                    sheet = workbook.GetSheetAt(0);
                }
            }
            else
            {
                sheet = workbook.GetSheetAt(0);
            }
            //获取sheet的首行
            var headerRow = sheet.GetRow(0);
            //一行最后一个方格的编号 即总的列数
            int cellCount = headerRow.LastCellNum;
            //最后一列的标号  即总的行数
            int rowCount = sheet.LastRowNum;
            for (int j = 0; j < headerRow.LastCellNum; j++)
            {
                table.Columns.Add(headerRow.GetCell(j).ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum && sheet.GetRow(i) != null; i++)
            {
                var row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();


                //当首列为空时,则认为此行以及此行之后为空


                if (row.GetCell(row.FirstCellNum) == null || string.IsNullOrWhiteSpace(row.GetCell(row.FirstCellNum).ToString()))
                {
                    continue;
                }


                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        ICell cell = row.GetCell(j);
                        //读取Excel格式，根据格式读取数据类型
                        #region //Excel格式，根据格式读取数据类型


                        switch (cell.CellType)
                        {
                            case CellType.Blank: //空数据类型处理
                                dataRow[j] = "";
                                break;
                            case CellType.String: //字符串类型
                                dataRow[j] = cell.StringCellValue;
                                break;
                            case CellType.Numeric: //数字类型                                   
                                if (DateUtil.IsCellDateFormatted(cell))//数字类型中分为数字、日期类型
                                {
                                    dataRow[j] = cell.DateCellValue;
                                }
                                else
                                {
                                    dataRow[j] = cell.NumericCellValue;
                                }
                                break;
                            case CellType.Formula:
                                HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(workbook);
                                dataRow[j] = e.Evaluate(cell).StringValue;
                                break;
                            default:
                                dataRow[j] = "";
                                break;
                        }
                        #endregion
                    }
                }
                table.Rows.Add(dataRow);

            }
        }
        catch (Exception ex)
        {
            outMsg = ex.ToString();
        }
        return table;
    }

}