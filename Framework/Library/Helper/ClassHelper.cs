using Framework.CustomDataType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library.Helper
{
    public static class ClassHelper
    {
        public static T1 Parse<T1,T2>(this T2 DataObject) 
        {
            JObject TmpObj = JObject.FromObject(DataObject);
            return TmpObj.ToObject<T1>();            
        }
        public static T1 Parse<T1, T2>(this T2 DataObject,string Fields)
        {            
            JsonSerializer jsonSerializer = new JsonSerializer
            {
                ContractResolver = new OnlyPropertiesResolver(Fields)
            };

            JObject TmpObj = JObject.FromObject(DataObject, jsonSerializer);
            return TmpObj.ToObject<T1>();
        }
       
    }

    public class OnlyPropertiesResolver : DefaultContractResolver
    {
        string _prop;
        public OnlyPropertiesResolver(string prop)
        {
            _prop = prop;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var allProps = base.CreateProperties(type, memberSerialization);           
            return allProps.Where(p => _prop.Split(',').Contains(p.PropertyName)).ToList();
        }
    }
}
