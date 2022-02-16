using Dapper.Contrib.Extensions;
using Framework.CustomDataType;
using Framework.Library.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework.Library.Helper
{
    public static class DbHelper
    {
        public static ModelInfo GetProperty(this object obj, string property_name)
        {
            return new ModelInfo(obj)
            {
                Property = obj.GetType().GetProperty(property_name)
            };

        }
        public static bool HasProperty(this object obj, string property_name)
        {
            return obj.GetType().GetProperty(property_name) != null;
        }
        public static dynamic GetPropertyValue(this object obj, string property_name)
        {
            //return obj.GetType().GetField(property_name).GetValue(obj);
            return obj.GetType().GetProperty(property_name).GetValue(obj, null);

        }
        public static dynamic GetPropValue(this object obj, string property_name)
        {
            return obj.GetType().GetField(property_name).GetValue(obj);
            // return obj.GetType().GetProperty(property_name).GetValue(obj, null);

        }

        public static dynamic GetPropertyValue<T>(this T obj, string property_name)
        {
            if (property_name.Trim() == "")
                return "";
            return obj.GetType().GetProperty(property_name).GetValue(obj, null);

        }

        public static ModelInfo GetPrimaryKey(this object obj)
        {
            return new ModelInfo(obj)
            {
                Property = obj.GetType().GetProperties()
                        .Where(e => e.IsDefined(typeof(ExplicitKeyAttribute))).FirstOrDefault(),

            };

        }
        public static string GetPrimaryKey(this Type Obj)
        {
            return Obj.GetProperties()
                        .Where(e => e.IsDefined(typeof(ExplicitKeyAttribute)))
                       .Select(e => e.Name).FirstOrDefault<string>();
        }

        public static string GetTableName(this Type Obj)
        {
            return Obj.GetCustomAttribute<TableAttribute>().Name;
        }

        public static string GetHistoryTableName(this Type Obj)
        {
            HistoryAttribute attribute = Obj.GetCustomAttribute<HistoryAttribute>();
            if (attribute == null)
                return "";
            return attribute.Name;
        }
        public static IEnumerable<string> GetColumns(this Type ModelType)
        {
            var pinfo = ModelType.GetProperties();
            return pinfo.Where(e => !e.IsDefined(typeof(KeyAttribute)) && !e.IsDefined(typeof(ComputedAttribute)) && !e.IsDefined(typeof(HistoryColumnAttribute))).Select(e => e.Name).ToArray();

        }
        public static IEnumerable<string> GetColumnsForHistory(this Type ModelType)
        {
            var pinfo = ModelType.GetProperties();
            return pinfo.Where(e => !e.IsDefined(typeof(ComputedAttribute)) && !e.IsDefined(typeof(HistoryColumnAttribute))).Select(e => e.Name).ToArray();

        }
        public static IEnumerable<string> GetHistoryColumns(this Type ModelType)
        {
            var pinfo = ModelType.GetProperties();
            return pinfo.Where(e => !e.IsDefined(typeof(ComputedAttribute))).Select(e => e.Name).ToArray();
        }
        public static IEnumerable<string> GetColumnsWithKey(this Type ModelType)
        {
            var pinfo = ModelType.GetProperties();
            return pinfo.Where(e => !e.IsDefined(typeof(ComputedAttribute))).Select(e => e.Name).ToArray();

        }
        public static IEnumerable<string> GetAllColumns(this Type ModelType)
        {
            var pinfo = ModelType.GetProperties();
            return pinfo.Select(e => e.Name).ToArray();

        }
        public static PropertyInfo[] GetColumnsInfo(this Type ModelType)
        {
            var pinfo = ModelType.GetProperties();
            return pinfo.Where(e => !e.IsDefined(typeof(ComputedAttribute)) && !e.IsDefined(typeof(HistoryColumnAttribute))).ToArray<PropertyInfo>();

        }
        public static string UniqueKey()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString().ToUpper();
        }

    }
}
