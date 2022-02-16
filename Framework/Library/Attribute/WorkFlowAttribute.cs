using System;

namespace Framework.Library.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class WorkFlowAttribute : System.Attribute
    {
        public WorkFlowAttribute(int code)
        {
            this.Code = code;
        }
        public int Code;
    }
}
