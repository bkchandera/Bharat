using Framework.Library.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.DataAccess.Utility
{
    public class IgnoreExtraPropertiesResolver : DefaultContractResolver
    {
        //bool IgnoreBase = false;
        //public IgnoreParentPropertiesResolver(bool ignoreBase)
        //{
        //    IgnoreBase = ignoreBase;
        //}
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var allProps = base.CreateProperties(type, memberSerialization);
            //Choose the properties you want to serialize/deserialize
            var props = type.GetColumnsInfo();
            return allProps.Where(p => props.Any(a => a.Name == p.PropertyName)).ToList();
        }
    }
}
