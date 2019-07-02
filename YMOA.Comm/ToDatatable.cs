using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Comm
{
    /// <summary>
    /// 说明: List 转 Datatable
    /// </summary>
    public class ToDatatable
    {
        public ToDatatable() { }

        /// <summary>
        ///  选中属性转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="arrStr"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys, string[] arrStr)
        {
            if (entitys == null || entitys.Count < 1)
            {
                return new DataTable();
            }
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();
            DataTable dt = new DataTable("dt");
            for (int i = 0; i < entityProperties.Length; i++)
            {
                if (IsExist(entityProperties[i].Name, arrStr))
                { 
                    Type t = entityProperties[i].PropertyType;
                    //类型存在Nullable<Type>时，需要进行以下处理，否则异常
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        t = t.GetGenericArguments()[0];
                    }
                    dt.Columns.Add(entityProperties[i].Name, t);
                }
            }
            foreach (T entity in entitys)
            {
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo p in entityProperties)
                {
                    if (IsExist(p.Name,arrStr))
                    {
                        object obj = p.GetValue(entity);
                        if (obj == null) continue;
                        if (p.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                            continue;
                        dr[p.Name] = obj;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        
        /// <summary>
        ///  全转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }
            if (items == null || items.Count < 1)
            {
                return tb;
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }
        
        /// <summary>
        ///  返回基础类型(如果类型为空)，否则返回该类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        /// <summary>
        ///  判断属性是否可为空
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        ///  判断属性是否存在
        /// </summary>
        /// <param name="str"></param>
        /// <param name="arrStr"></param>
        /// <returns></returns>
        private static bool IsExist(string str, string[] arrStr)
        {
            return arrStr.Contains(str);
        }

    }
}
