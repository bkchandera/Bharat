using FluentValidation;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Framework.CustomDataType
{
    /*
     * Auther : Roshani Shah
     * Use:  Data Type is used to configure join parameter of list,view,dropdown service
    */
    public class JoinParameter
    {        
        public string type { get; set; } = "inner";
        public string table { get; set; }
        public string condition { get; set; }
    }

    /*
     * Auther : Roshani Shah
     * Use:  Data Type is used to configure condition parameter to make dynemic select query with sql builder
    */
    public class ConditionParameter    {
        public string direct_condition { get; set; } = "n";

        public string PropertyName { get; set; } = "";
        public string Operator { get; set; } = "=";
        //#$#  value will not bind at run time
        public dynamic PropertyValue { get; set; }
        public BetweenParam BetweenParam { get; set; }
    }

    public class BetweenParam
    {
        public dynamic Param1 { get; set; }
        public dynamic Param2 { get; set; }
    }

    //public class QueryParam
    //{
    //    public string Table { get; set; }
    //    public string Fields { get; set; } = " * ";
    //    public List<JoinParameter> Join { get; set; }
    //    public List<ConditionParameter> where { get; set; }
    //    public string orderby { get; set; }

    //}
    public class QueryParam
    {
        public string Sp { get; set; }
        public string DirectQuery { get; set; }
        public string Table { get; set; }
        public string Distinct { get; set; } = "";
        public string Fields { get; set; } = " * ";  
        public string GroupBy { get; set; }
        public string Having { get; set; }
        public string OrderBy { get; set; }        
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<JoinParameter> Join { get; set; }
        public List<ConditionParameter> Where { get; set; }
        public Dictionary<string, dynamic> DynemicParam { get; set; }
        public List<ChildData> child { get; set; }


    }
    public class ChildData
    {
        public string type { get; set; }
        public string fields { get; set; }
        public List<JoinParameter> Join { get; set; }
        public List<ConditionParameter> Where { get; set; }
        public string groupby { get; set; }
    }
    public class InsertQueryParam
    {       
        public string DirectQuery { get; set; }     
        public List<ConditionParameter> Where { get; set; }
    }

    public class DirectQuery
    {
        public string Query { get; set; }        
        public List<JoinParameter> Join { get; set; }
        public List<ConditionParameter> where { get; set; }
        public string orderby { get; set; }

    }

    /*
     * Auther : Roshani Shah
     * Use:  Data Type is used to get model's property and property's value when using reflection 
    */
    public class ModelInfo
    {
      
        private object _obj;

        public ModelInfo(object obj)
        {
            _obj = obj;           
        }      
        public PropertyInfo Property { get; set; }
        public string PropertyName { get { return Property.Name.ToString(); } }
        public dynamic PropertyValue { get { return Property.GetValue(_obj, null); } }  
    }

    /*
     * Auther : Roshani Shah
     * Use:  Data Type is used in create and update data.
    */
    public class ModelParameter
    {
      public IValidator ValidateModel { get; set; }
      public BaseModel SaveModel { get; set; }
    }


}
