using System;


namespace Framework.Library.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HistoryAttribute : System.Attribute
    {
        public HistoryAttribute(string Name)
        {
            this.Name = Name;
        }
        public string Name;// { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class HistoryColumnAttribute : System.Attribute
    {
        
    }
}
