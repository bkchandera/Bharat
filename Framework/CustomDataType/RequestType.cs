using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.CustomDataType
{
    public struct DropdownData
    {
        public string model { get; set; }
        public Dictionary<string, dynamic> fields { get; set; }
    }

    public struct GenericType<T1, T2>
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
    }

    public class GenericType<T1, T2, T3>
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }

        public T3 Param3 { get; set; }
    }

    public struct ListJson
    {
        public string model { get; set; }
        public string fields { get; set; }
        public List<JoinParameter> join { get; set; }
        public string where_data { get; set; }
        public List<ConditionParameter> where { get; set; }
    }
    public struct ListJsonDDL
    {
        public string model { get; set; }
        public string distinct { get; set; }
        public string fields { get; set; }
        public List<JoinParameter> join { get; set; }
        public string where_data { get; set; }
        public List<ConditionParameter> where { get; set; }
        public string orderby { get; set; }
        public string sp { get; set; }
    }

    public class SearchParam
    {
        public string model { get; set; }
        public string type { get; set; }
        public List<ConditionParameter> Param { get; set; }
        public int Offset { get; set; } = 0;
        // On Karan's Instrunction limit set to 0--02/10/2020 -- mpp_collection delete list -- not passed offset so only 20 records are displayed
        // public int Limit { get; set; } = 20;
        public int Limit { get; set; } = 0;
    }   

    public struct HirarchyParam
    {
        public string object_type { get; set; }
        public string object_value { get; set; }
        public bool is_geo { get; set; }
    }
}
