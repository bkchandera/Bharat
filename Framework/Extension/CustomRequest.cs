using Framework.CustomDataType;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//for production testing
namespace Framework.Extension
{
    public static class CustomRequest
    {
        public static T ParseRequest<T>(this object DataObject)
        {
            JObject json = (JObject)DataObject;
            //if (json.Count == 1)
            //{
            //    json["data"]["user_code"] = json["user_code"];
                
            //}
            

            return json.GetValue("data").ToObject<T>();
        }

        public static List<T> ParseRequestList<T>(this object DataObject)
        {
            JObject json = (JObject)DataObject;
            return json.GetValue("data").ToObject<List<T>>();
        }

        public static List<T> ParseRequest<T>(this object DataObject, string Token = "data")
        {
            JObject json = (JObject)DataObject;
            JObject json1 = json.GetValue("data").ToObject<JObject>();
            return json1.GetValue(Token).ToObject<List<T>>();
        }

        public static JObject ParseRequest(this object DataObject)
        {
            JObject json = (JObject)DataObject;
            return json.GetValue("data").ToObject<JObject>();
        }


        public static GenericType<T1, T2> ParseRequest<T1, T2>(this object DataObject)
        {
            JObject json = (JObject)DataObject;
            if (json.GetValue("data") == null)
            {
                return new GenericType<T1, T2> { Param1 = json.ToObject<T1>(), Param2 = json.ToObject<T2>() };
            }
            else
            {
                return new GenericType<T1, T2> { Param1 = json.GetValue("data").ToObject<T1>(), Param2 = json.GetValue("data").ToObject<T2>() };
            }
        }



        public static GenericType<T1, T2, T3> ParseRequest<T1, T2, T3>(this object DataObject)
        {
            JObject json = (JObject)DataObject;
            return new GenericType<T1, T2, T3> { Param1 = json.GetValue("data").ToObject<T1>(), Param2 = json.GetValue("data").ToObject<T2>(), Param3 = json.GetValue("data").ToObject<T3>() };


        }

        //public static T1 ParseArrayRequest<T1,T2>(this object DataObject)
        //{
        //    JObject json = (JObject)DataObject;            
        //    json.GetValue("data").ToObject<T2>();
        //}
    }
}







