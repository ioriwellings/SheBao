using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 枚举类相关的方法
    /// </summary>
    public class EnumsCommon
    {
        /// <summary>
        /// 根据社保种类名称获取对应的枚举值
        /// </summary>
        public static int GetInsuranceKindValue(string name)
        {
            foreach (EmployeeAdd_InsuranceKindId dbt in Enum.GetValues(typeof(EmployeeAdd_InsuranceKindId)))
            {
                if (dbt.ToString() == name)
                    return Convert.ToInt32(dbt);
            }

            return -1; // 没有符合的
        }

        /// <summary>
        /// 根据枚举类型获List数据源
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<EnumsListModel> GetEnumList(Type enumType)
        {
            /*
             * DropDownList/RadioButtonList调用示例         
             * DropDownList1.DataSource = EnumDict.GetEnumList(typeof(EnumDict.AccountType));
             * DropDownList1.DataTextField = "text";
             * DropDownList1.DataValueField = "value";
             * DropDownList1.DataBind();         
             */

            List<EnumsListModel> list = new List<EnumsListModel>();
            foreach (object obj in Enum.GetValues(enumType))
            {
                EnumsListModel item = new EnumsListModel();
                item.Name = obj.ToString();
                item.Code = (int)obj;
                list.Add(item);
            }
            return list;
        }

        public class EnumsListModel 
        {
            public string Name { get; set; }
            public int Code { get; set; }
        }
    }
}
