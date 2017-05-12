using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Convert<T> where T : new()
    {
        #region 实体集转换成DataTable
        /// <summary>
        /// 实体集转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    object val = propertyInfo.GetValue(model, null);
                    if (val != null)
                        dataRow[propertyInfo.Name] = val.ToString();
                    else
                        dataRow[propertyInfo.Name] = "";
                }

                //foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                //{
                //    Type type = propertyInfo.PropertyType;
                //    //如果是Nullable类型
                //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                //    {
                //        Type tt = type.GetGenericArguments()[0];
                //        dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                //    }
                //    else
                //        dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                //}


                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable CreateData(T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                Type type = propertyInfo.PropertyType;

                //如果是Nullable类型
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type tt = type.GetGenericArguments()[0];
                    //dataTable.Columns.Add(new DataColumn(propertyInfo.Name, tt));
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name));
                }
                else
                    //dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name));
            }

            return dataTable;
        }
        #endregion
    }
}
