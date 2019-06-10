using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Comm
{
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

            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                return new DataTable();
            }

            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable("dt");
            for (int i = 0; i < entityProperties.Length; i++)
            {
                if (arrStr.Contains(entityProperties[i].Name))
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

            //将所有entity添加到DataTable中
            foreach (T entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }

                DataRow dr = dt.NewRow();
                foreach (PropertyInfo p in entityProperties)
                {
                    if (arrStr.Contains(p.Name))
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
        ///  全部转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            //检查实体集合不能为空
            if (items == null || items.Count < 1)
            {
                return new DataTable();
            }

            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
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

        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
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

    }
}
