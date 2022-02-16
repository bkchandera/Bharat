using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.CustomDataType
{
    public class ImportJson
    {
        public string Column { get; set; }
        public string Service { get; set; }
        public string Operation { get; set; } = "Create";
        public bool IsAll { get; set; } = false;
        public List<ColumnMappingValue> ColumnMapping { get; set; }
        public List<ColumnMappingValue> CopyColumn { get; set; }
        public List<TransformationValue> Transformation { get; set; }
        public List<Derived> Derived { get; set; }
        public List<DefaultValue> Default { get; set; }

        //for validate import file data record limit.
        public int record_limit { get; set; } = 2000;
    }
    public class ColumnMappingValue
    {
        public string Header { get; set; }
        public string Field { get; set; }
        public string Datatype { get; set; } = "string";
    }
    public class TransformationValue
    {
        public string ColumnName { get; set; }
        public List<TransformationSubValue> Value { get; set; }
    }
    public class TransformationSubValue
    {
        public string ColumnValue { get; set; }
        public string ActualValue { get; set; }
    }
    public class Derived
    {
        public string Fields { get; set; }
        public string Depended { get; set; }
        public string Table { get; set; }
        public bool IsDefault { get; set; } = false;
        public List<ConditionParameter> Where { get; set; }
        public List<JoinParameter> Join { get; set; }
    }
    public class DefaultValue
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
    }
}
