using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using Langben.BLL;
//using Langben.IBLL;
//using Langben.DAL;

namespace Models
{
    /// <summary>
    ///  此处实现数据字典的功能
    /// </summary>
    public class SysFieldModels
    {

        /// <summary>
        /// 获取字段，首选默认
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSysField(string table, string colum, string parentMyTexts)
        {
            //if (string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(colum) || string.IsNullOrWhiteSpace(parentMyTexts))
            {
                List<SelectList> sl = new List<SelectList>();
                return new SelectList(sl);
            }
            //ISysFieldHander baseDDL = new SysFieldHander();
            //return new SelectList(baseDDL.GetSysField(table, colum, parentMyTexts), "MyTexts", "MyTexts");

        }
        /// <summary>
        /// 获取字段，首选默认，MyTexts做为value值
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSysField(string table, string colum)
        {
            //if (string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(colum))
            {
                List<SelectList> sl = new List<SelectList>();
                return new SelectList(sl);
            }
            //ISysFieldHander baseDDL = new SysFieldHander();
            //return new SelectList(baseDDL.GetSysField(table, colum), "MyTexts", "MyTexts");

        }
        /// <summary>
        /// 获取字段，首选默认，Id做为value值
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSysFieldById(string table, string colum)
        {
            //if (string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(colum))
            {
                List<SelectList> sl = new List<SelectList>();
                return new SelectList(sl);
            }
            //ISysFieldHander baseDDL = new SysFieldHander();
            //return new SelectList(baseDDL.GetSysField(table, colum), "Id", "MyTexts");

        }
        /// <summary>
        /// 根据主键id，获取数据字典的展示字段
        /// </summary>
        /// <param name="id">父亲节点的主键</param>
        /// <returns></returns>
        public static string GetMyTextsById(string id)
        {
            //if (string.IsNullOrWhiteSpace(id))
            {
                return string.Empty;
            }
            //ISysFieldHander baseDDL = new SysFieldHander();
            //return baseDDL.GetMyTextsById(id);

        }

        /// <summary>
        /// 根据枚举类型获取实际值绑定列表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static SelectList GetSelectList(Type enumType)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach(var item in Enum.GetValues(enumType))
            {
                SelectListItem temp = new SelectListItem();
                temp.Text = item.ToString();
                temp.Value = Convert.ToInt32( item).ToString();
                list.Add(temp);

            }
            
            return new SelectList(list,"Value","Text"); ;
        }

        /// <summary>
        /// 根据枚举类型获取显示值绑定列表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static SelectList GetEnumList(Type enumType)
        {
            return new SelectList(Enum.GetValues(enumType)); ;
        }


        /// <summary>
        /// 根据传入参数获取实际值绑定列表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static SelectList GetSelectList<T>(List<T> list,string Value,string Text)
        {
            return new SelectList(list, Value, Text);
        }
    }
}
