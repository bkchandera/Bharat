using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.CustomDataType
{
  
        public struct ReturnValue
        {
            public string id { get; set; }
            public string value { get; set; }
        }

        public struct ListData
        {
            public string[] header { get; set; }
            public dynamic data_list { get; set; }
        }

    public struct ReturnPK
    {
        public string code { get; set; }
    }


}
