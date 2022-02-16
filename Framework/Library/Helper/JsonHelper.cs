using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Framework.Library.Helper
{
    public static class JsonHelper
    {
        public static String ToJson(object convertObject)
        {
            return JsonConvert.SerializeObject(convertObject);
        }
    }
}
