using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Library.Helper
{
    public static class CollectionHelper
    {
        public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> me, Dictionary<TKey, TValue> merge)
        {
            foreach (var item in merge)
            {
                me[item.Key] = item.Value;
            }
        }

        public static List<T> Append<T>(this List<T> MainList,List<T> NewList)
        {
            if (MainList == null)
                MainList=NewList;
            else
            {
                if(NewList!=null)
                    MainList.AddRange(NewList);                
            }
            return MainList;

        }
    }
}
