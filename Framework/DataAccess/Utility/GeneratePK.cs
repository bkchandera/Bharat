using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Framework.DataAccess.Utility
{
    public static class GeneratePK
    {
        
        public static string getPK<T>(int length, ConditionParameter ConditionList, List<JoinParameter> join = null) where T : BaseModel
        {
            //DBRepository repository = new DBRepository();
            //string Table = typeof(T).GetTableName(), PK = typeof(T).GetPrimaryKey();
            //QueryParam Query = new QueryParam
            //{
            //    DirectQuery = $"SELECT isnull(max(cast(SUBSTRING({Table}.{PK},{ ConditionList.PropertyValue.Length + 1},LEN({ Table}.{ PK})) as int)),0) FROM { Table}",
            //    Where =new List<ConditionParameter>
            //    {
            //        ConditionList
            //    },
            //    Join=join
            //};
            string[] TmpStr = getPK<T>(length, ConditionList, true, join);
            return TmpStr[0] + ((Convert.ToInt32(TmpStr[1]) + 1).ToString().PadLeft(length, '0'));
        }

        public static string[] getPK<T>(int length, ConditionParameter ConditionList, bool flag, List<JoinParameter> join = null) where T : BaseModel
        {
            DBRepository repository = new DBRepository();
            string Table = typeof(T).GetTableName(), PK = typeof(T).GetPrimaryKey();
            QueryParam Query = new QueryParam
            {
                DirectQuery = $"SELECT isnull(max(cast(SUBSTRING({Table}.{PK},{ ConditionList.PropertyValue.Length + 1},LEN({ Table}.{ PK})) as int)),0) FROM { Table}",
                Where = new List<ConditionParameter>
                {
                    ConditionList
                },
                Join = join
            };
            string[] TmpStr = new string[3];
            TmpStr[0] = ConditionList.PropertyValue;
            TmpStr[1] = repository.Find<int>(Query).ToString();
            TmpStr[2] = length.ToString();
            return TmpStr;
            //            return ConditionList.PropertyValue + (repository.Find<int>(Query) + 1).ToString().PadLeft(length, '0');
        }


        public static string getPK<T>(int length) where T : BaseModel
        {
            return (getLastPK<T>() + 1).ToString().PadLeft(length, '0');
        }
        public static string getPK<T>() where T : BaseModel
        {
            return (getLastPK<T>() + 1).ToString();
        }
        public static dynamic getKey<T>(string field, List<ConditionParameter> ConditionList, List<JoinParameter> join = null) where T : BaseModel
        {
            DBRepository repository = new DBRepository();
            string Table = typeof(T).GetTableName();
            QueryParam Query = new QueryParam
            {
                DirectQuery = $"SELECT isnull(max(cast({Table}.{field} as int)),0) FROM { Table}",
                Where = ConditionList,
                Join = join
            };
            return (repository.Find<int>(Query) + 1).ToString();
        }
        public static string getKey(string field, string table, int length, string code, List<ConditionParameter> ConditionList = null, List<JoinParameter> join = null)
        {
            DBRepository repository = new DBRepository();
            QueryParam Query = new QueryParam
            {
                DirectQuery = $"SELECT isnull(max(cast(SUBSTRING({table}.{field},{ code.Length + 1},LEN({table}.{ field})) as int)),0) FROM {table}",
                Where = ConditionList,
                Join = join
            };
            return code + (repository.Find<int>(Query) + 1).ToString().PadLeft(length, '0');
        }
        public static string getFormatedPK<T>(int length,string prefix_value) where T : BaseModel
        {
            DBRepository NewRepo = new DBRepository();
            QueryParam Query = new QueryParam
            {
                DirectQuery = $"SELECT isnull(max(cast(SUBSTRING({typeof(T).GetPrimaryKey()},{ prefix_value.Length + 1},LEN({typeof(T).GetPrimaryKey()})) as bigint)),0) FROM {typeof(T).GetTableName()} where SUBSTRING({typeof(T).GetPrimaryKey()},0,{prefix_value.Length + 1})='{prefix_value}'",

            };
            int no=NewRepo.Find<int>(Query);
            return prefix_value + (no + 1).ToString().PadLeft(length, '0');
        }

        public static Int64 getFormatedPK<T>(string prefix_value) where T : BaseModel
        {           
            DBRepository NewRepo = new DBRepository();
            QueryParam Query = new QueryParam
            {
                DirectQuery = $"SELECT isnull(max(cast(SUBSTRING({typeof(T).GetPrimaryKey()},{ prefix_value.Length + 1},LEN({typeof(T).GetPrimaryKey()})) as bigint)),0) FROM {typeof(T).GetTableName()} where SUBSTRING({typeof(T).GetPrimaryKey()},0,{prefix_value.Length+1})='{prefix_value}'",

            };
            Int64 key =NewRepo.Find<Int64>(Query);
            return key;
        }
        //private static int getLastPK<T>(ConditionParameter ConditionList, List<JoinParameter> join=null) where T : BaseModel
        //{
        //    DBRepository repository = DBRepository();
        //    string Table = typeof(T).GetTableName(), PK = typeof(T).GetPrimaryKey();
        //    QueryParam Query = new QueryParam
        //    {
        //        DirectQuery = $"SELECT max(SUBSTRING({Table}.{PK},{ ConditionList.PropertyValue.Length + 1},LEN({ Table}.{ PK}))) FROM { Table}",

        //    };
        //    return repository.Find<T>(ConditionList, join);
        //}

        private static int getLastPK<T>() where T : BaseModel
        {
            DBRepository NewRepo = new DBRepository();
            QueryParam Query = new QueryParam
            {
                DirectQuery = $"select ISNULL(max(cast({typeof(T).GetPrimaryKey()} as int)),0) from { typeof(T).GetTableName()}",

            };
            return NewRepo.Find<int>(Query);
        }


    }
}
