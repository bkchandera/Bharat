using Dapper;
using Dapper.Contrib.Extensions;
using Framework.CustomDataType;
using Framework.Library.Helper;
using Framework.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Framework.DataAccess.Dapper
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        //Microsoft.Extensions.Configuration.ConfigurationExtensions.get

        protected readonly string connectionstring = @"Server=182.74.63.142,14033;Database=NDS_NEW;User Id=sa;Password=everest@123;Trusted_Connection=True;Integrated Security=False";
       // protected readonly string connectionstring = @"Server=182.74.63.142,14033;Database=NDS_TESTING;User Id=sa;Password=everest@123;Trusted_Connection=True;Integrated Security=False";


        private readonly IDbConnection _db;
        public BaseRepository()
        {

            _db = new SqlConnection(connectionstring);
        }
        public dynamic Add(T item)
        {

            /*   var columns = GetColumns();
               var stringOfColumns = string.Join(", ", columns);
               var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
               var query = $"insert into {typeof(T).Name} ({stringOfColumns}) values ({stringOfParameters})";

               this._db.Execute(query, item); */
            return this._db.Insert(item);

        }
        public dynamic Add(IBaseModel SaveModel)
        {

            var columns = GetColumns(SaveModel.GetType());
            var stringOfColumns = string.Join(", ", columns);
            var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
            var query = $"insert into {SaveModel.GetType().GetTableName()} ({stringOfColumns}) values ({stringOfParameters});SELECT SCOPE_IDENTITY()";

           return this._db.Query<int>(query, SaveModel).SingleOrDefault();
            
            // return this._db.Insert(SaveModel);

        }
        //public dynamic Add(BaseModel SaveModel)
        //{

        //    /*   var columns = GetColumns();
        //       var stringOfColumns = string.Join(", ", columns);
        //       var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
        //       var query = $"insert into {typeof(T).Name} ({stringOfColumns}) values ({stringOfParameters})";

        //       this._db.Execute(query, item); */
        //    return this._db.Insert(SaveModel);

        //}

        public T Find(string fields = "*", Func<T, bool> predicate = null)
        {

            string query = $"select {fields} from {TableName()} ";
            return this._db.Query<T>(query).Where(predicate).FirstOrDefault();
            //  throw new NotImplementedException();
        }


        public int Count(List<ConditionParameter> ConditionList = null)
        {
            var builder = new SqlBuilder();

            var selector = builder.AddTemplate($"SELECT count(*) FROM {TableName()} /**where**/");
            var parameter = new DynamicParameters();
            if (ConditionList != null)
            {
                foreach (var entry in ConditionList)
                {
                    builder.Where($"{entry.PropertyName}{entry.Operator}@{entry.PropertyName}");
                    parameter.Add(entry.PropertyName, entry.PropertyValue);
                }

            }
            return this._db.Query<int>(selector.RawSql, parameter).First();
        }

        public IEnumerable<T> FindAll()
        {

            return this._db.GetAll<T>();
        }


        public IEnumerable<T> FindAll(string TableName, string Fields, List<JoinParameter> join = null, List<ConditionParameter> ConditionList = null)
        {
            var builder = new SqlBuilder();

            var selector = builder.AddTemplate($"SELECT distinct {Fields} FROM {TableName} /**innerjoin**/ /**leftjoin**/ /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            if (join != null)
            {
                foreach (JoinParameter entry in join)
                {
                    if (entry.type == "left")
                    {
                        builder.LeftJoin($"{entry.table} ON {entry.condition}");
                    }
                    else
                    {
                        builder.InnerJoin($"{entry.table} ON {entry.condition}");
                    }
                }
            }
            if (ConditionList != null)
            {
                foreach (ConditionParameter entry in ConditionList)
                {
                    string where = "", where_value = "";

                    if (entry.PropertyName != "")
                    {
                        where = entry.PropertyName;
                        where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                    }
                    builder.Where($"{where}{entry.Operator}@{where_value}");
                    parameter.Add(where_value, entry.PropertyValue);
                }
            }
            //var query = $"SELECT {Fields} FROM {TableName}";
            return this._db.Query<T>(selector.RawSql, parameter);

        }

        public IEnumerable<T> FindAll(QueryParam Query)
        {
            DynamicParameters parameter = new DynamicParameters();
            if (Query.Sp.NotEmpty())
            {
                foreach (ConditionParameter Condition in Query.Where.NotEmpty())
                {
                    parameter.Add($"@{Condition.PropertyName}", Condition.PropertyValue);
                }
                return this._db.Query<T>(sql: Query.Sp, param: parameter, commandType: CommandType.StoredProcedure);
            }
            else
            {
                SqlBuilder builder = new SqlBuilder();
                SqlBuilder.Template selector;
                string FinalQuery;
                if (Query.DirectQuery.NotEmpty())
                {
                    selector = builder.AddTemplate($"{Query.DirectQuery} /**innerjoin**/ /**leftjoin**/ /**where**/ /**orderby**/");
                }
                else
                {
                    selector = builder.AddTemplate($"SELECT {Query.Distinct} {Query.Fields} FROM {Query.Table} /**innerjoin**/ /**leftjoin**/ /**where**/ /**orderby**/");
                }
                foreach (JoinParameter entry in Query.Join.NotEmpty())
                {
                    if (entry.type == "left")
                    {
                        builder.LeftJoin($"{entry.table} ON {entry.condition}");
                    }
                    else
                    {
                        builder.InnerJoin($"{entry.table} ON {entry.condition}");
                    }
                }
                foreach (ConditionParameter entry in Query.Where.NotEmpty())
                {
                    string where = "", where_value = "";

                    where = entry.PropertyName;
                    where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);

                    if (entry.Operator.ToLower() == "between")
                    {
                        builder.Where($"{where} {entry.Operator} @{where_value}1 And @{where_value}2");
                        parameter.Add(where_value + "1", entry.BetweenParam.Param1);
                        parameter.Add(where_value + "2", entry.BetweenParam.Param2);
                    }
                    else
                    {
                        builder.Where($"{where} {entry.Operator} @{where_value}");
                        parameter.Add(where_value, entry.PropertyValue);
                    }
                }
                FinalQuery = selector.RawSql;
                if (Query.OrderBy.NotEmpty())
                {
                    builder.OrderBy(Query.OrderBy);
                    FinalQuery = selector.RawSql;
                    if (Query.Offset != 0 && Query.Limit != 0)
                    {
                        FinalQuery += " OFFSET @OFFSET ROWS FETCH NEXT @LIMIT ROWS ONLY";
                        parameter.Add("@OFFSET", Query.Offset);
                        parameter.Add("@LIMIT", Query.Limit);
                    }

                }
                return this._db.Query<T>(FinalQuery, parameter);

            }


        }

        public List<T> FindAll(List<ConditionParameter> Condition)
        {
            DynamicParameters parameter = new DynamicParameters();

            SqlBuilder builder = new SqlBuilder();
            SqlBuilder.Template selector;
            selector = builder.AddTemplate($"SELECT * FROM {typeof(T).GetTableName()} /**innerjoin**/ /**leftjoin**/ /**where**/ /**orderby**/");

            foreach (ConditionParameter entry in Condition.NotEmpty())
            {
                string where = "", where_value = "";
                where = entry.PropertyName;
                where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);

                if (entry.Operator.ToLower() == "between")
                {
                    builder.Where($"{where} {entry.Operator} @{where_value}1 And @{where_value}2");
                    parameter.Add(where_value + "1", entry.BetweenParam.Param1);
                    parameter.Add(where_value + "2", entry.BetweenParam.Param2);
                }
                else
                {
                    builder.Where($"{where} {entry.Operator} @{where_value}");
                    parameter.Add(where_value, entry.PropertyValue);
                }
            }
            return this._db.Query<T>(selector.RawSql, parameter).ToList();
        }

        public IEnumerable<T> FindAll(DirectQuery QueryData)
        {
            var builder = new SqlBuilder();

            var selector = builder.AddTemplate($"{QueryData.Query} /**innerjoin**/ /**leftjoin**/ /**where**/ /**orderby**/");
            DynamicParameters parameter = new DynamicParameters();
            if (QueryData.Join != null)
            {
                foreach (JoinParameter entry in QueryData.Join)
                {
                    if (entry.type == "left")
                    {
                        builder.LeftJoin($"{entry.table} ON {entry.condition}");
                    }
                    else
                    {
                        builder.InnerJoin($"{entry.table} ON {entry.condition}");
                    }
                }
            }
            if (QueryData.where != null)
            {
                foreach (ConditionParameter entry in QueryData.where)
                {
                    string where = "", where_value = "";

                    if (entry.PropertyName != "")
                    {
                        where = entry.PropertyName;
                        where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                    }
                    if (entry.Operator.ToLower() == "between")
                    {
                        builder.Where($"{where} {entry.Operator} @{where_value}1 And @{where_value}2");
                        parameter.Add(where_value + "1", entry.BetweenParam.Param1);
                        parameter.Add(where_value + "2", entry.BetweenParam.Param2);
                    }
                    else
                    {
                        builder.Where($"{where} {entry.Operator} @{where_value}");
                        parameter.Add(where_value, entry.PropertyValue);
                    }


                }
            }
            if (QueryData.orderby != null)
            {
                builder.OrderBy(QueryData.orderby);
            }
            //var query = $"SELECT {Fields} FROM {TableName}";
            return this._db.Query<T>(selector.RawSql, parameter);

        }

        public T Find(QueryParam Query)
        {
            if (typeof(T) != typeof(object) && !Query.Table.NotEmpty())
            {
                Query.Table = typeof(T).GetTableName();
            }
            DynamicParameters parameter = new DynamicParameters();
            if (Query.Sp.NotEmpty())
            {
                foreach (ConditionParameter Condition in Query.Where.NotEmpty())
                {
                    parameter.Add($"@{Condition.PropertyName}", Condition.PropertyValue);
                }
                return this._db.Query<T>(sql: Query.Sp, param: parameter, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            else
            {
                SqlBuilder builder = new SqlBuilder();
                SqlBuilder.Template selector;

                if (Query.DirectQuery.NotEmpty())
                {
                    selector = builder.AddTemplate($"{Query.DirectQuery} /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**orderby**/");
                }
                else
                {
                    selector = builder.AddTemplate($"SELECT {Query.Fields} FROM {Query.Table} /**innerjoin**/ /**leftjoin**/ /**where**/ /**orderby**/");
                }
                foreach (JoinParameter entry in Query.Join.NotEmpty())
                {
                    if (entry.type == "left")
                    {
                        builder.LeftJoin($"{entry.table} ON {entry.condition}");
                    }
                    else
                    {
                        builder.InnerJoin($"{entry.table} ON {entry.condition}");
                    }
                }
                foreach (ConditionParameter entry in Query.Where.NotEmpty())
                {
                    if (entry.PropertyValue == "?")
                    {
                        throw new Exception("Param Value not Bind");
                    }
                    string where = "", where_value = "";

                    where = entry.PropertyName;
                    where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);

                    if (entry.Operator.ToLower() == "between")
                    {
                        builder.Where($"{where} {entry.Operator} @{where_value}1 And @{where_value}2");
                        parameter.Add(where_value + "1", entry.BetweenParam.Param1);
                        parameter.Add(where_value + "2", entry.BetweenParam.Param2);
                    }
                    else
                    {
                        builder.Where($"{where} {entry.Operator} @{where_value}");
                        parameter.Add(where_value, entry.PropertyValue);
                    }
                }
                if (Query.GroupBy.NotEmpty())
                {
                    builder.GroupBy(Query.GroupBy);
                }
                return this._db.Query<T>(selector.RawSql, parameter).FirstOrDefault();

            }

        }

        public T Find(string TableName, string Fields, List<JoinParameter> join = null, List<ConditionParameter> ConditionList = null)
        {
            var builder = new SqlBuilder();

            var selector = builder.AddTemplate($"SELECT {Fields} FROM {TableName} /**innerjoin**/ /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            if (join != null)
            {
                foreach (JoinParameter entry in join)
                {
                    builder.InnerJoin($"{entry.table} ON {entry.condition}");
                }
            }
            if (ConditionList != null)
            {
                string where = "", where_value = "";
                foreach (ConditionParameter entry in ConditionList)
                {
                    string whereField = entry.PropertyName;
                    if (entry.PropertyName != "")
                    {
                        where = entry.PropertyName;
                        where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                    }
                    builder.Where($"{where}{entry.Operator}@{where_value}");
                    parameter.Add(where_value, entry.PropertyValue);
                }
            }
            //var query = $"SELECT {Fields} FROM {TableName}";
            return this._db.Query<T>(selector.RawSql, parameter).FirstOrDefault();
        }

        public T FindByID(int id)
        {
            return this._db.Get<T>(id);
        }

        public T FindByID(string id)
        {
            return this._db.Get<T>(id);
        }


        public void Remove(T item)
        {
            throw new NotImplementedException();
        }

        //public void Update(T item)
        //{
        //    /* var columns = GetColumns();
        //     var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
        //     var query = $"update {typeof(T).Name} set {stringOfColumns} where Id = @Id";
        //     this._db.Execute(query, item);
        //     */
        //    this._db.Update(item);

        //}

        public bool Update(T item)
        {
            /* var columns = GetColumns();
             var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
             var query = $"update {typeof(T).Name} set {stringOfColumns} where Id = @Id";
             this._db.Execute(query, item);
             */
            return this._db.Update(item);

        }

        public void Insert_Sp(T item)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<T> ListSp()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ListSp(string SpName, int Offset, int Limit)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@offset", Offset);
            parameters.Add("@limit", Limit);
            return this._db.Query<T>(sql: SpName, param: parameters, commandType: CommandType.StoredProcedure);
        }


        private IEnumerable<string> GetColumns()
        {
            return typeof(T)
                    .GetProperties()
                    .Where(e => e.Name != "Id" && !e.PropertyType.GetTypeInfo().IsGenericType && e.PropertyType.Equals(typeof(KeyAttribute)))
                    .Select(e => e.Name);
        }
        private IEnumerable<string> GetColumns(Type ModelType)
        {
            //return ModelType
            //        .GetProperties()
            //        .Where(e => e.PropertyType.Equals(typeof(KeyAttribute)))
            //        .Select(e => e.Name);
            var pinfo = ModelType.GetProperties();
            return pinfo.Where(e => !e.IsDefined(typeof(KeyAttribute)) && !e.IsDefined(typeof(ComputedAttribute))).Select(e => e.Name).ToArray();
            
        }

        public string GetPrimaryKey(Type Model)
        {
            return Model.GetProperties()
                        .Where(e => e.IsDefined(typeof(ExplicitKeyAttribute)))
                       .Select(e => e.Name).FirstOrDefault<string>();
        }
        public string GetPrimaryKey()
        {
            return typeof(T).GetProperties()
                        .Where(e => e.IsDefined(typeof(ExplicitKeyAttribute)))
                       .Select(e => e.Name).FirstOrDefault<string>();
        }
        private string TableName(Type Table)
        {
            return Table.GetCustomAttribute<TableAttribute>().Name;
        }

        private string TableName()
        {
            return typeof(T).GetCustomAttribute<TableAttribute>().Name;
        }

        public int MaxKey(ConditionParameter ConditionList = null, List<JoinParameter> join = null)
        {
            var builder = new SqlBuilder();
            string pk = GetPrimaryKey(), table = TableName();

            var selector = builder.AddTemplate($"SELECT max(SUBSTRING({table}.{pk},{ConditionList.PropertyValue.Length + 1},LEN({table}.{pk}))) FROM {table} /**innerjoin**/ /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            if (join != null)
            {
                foreach (JoinParameter entry in join)
                {
                    builder.InnerJoin($"{entry.table} ON {entry.condition}");
                }
            }
            if (ConditionList != null)
            {
                builder.Where($"{ConditionList.PropertyName}{ConditionList.Operator}@{ConditionList.PropertyName}");
                parameter.Add(ConditionList.PropertyName, ConditionList.PropertyValue);
            }


            //var query = $"SELECT {Fields} FROM {TableName}";
            return Convert.ToInt32(this._db.Query<string>(selector.RawSql, parameter).FirstOrDefault());
        }

        public bool Delete(QueryParam QueryData)
        {
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate($"Delete FROM {QueryData.Table} /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            if (QueryData.Where != null)
            {
                foreach (ConditionParameter entry in QueryData.Where)
                {
                    string where = "", where_value = "";
                    where = entry.PropertyName;
                    where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                    builder.Where($"{where}{entry.Operator}@{where_value}");
                    parameter.Add(where_value, entry.PropertyValue);
                }
            }
            int affectedrows = this._db.Execute(selector.RawSql, parameter);
            return affectedrows >= 0;
        }

        public bool Update(QueryParam QueryData)
        {
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate($"Update {QueryData.Table} set {QueryData.Fields} /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            if (QueryData.Where != null)
            {
                foreach (ConditionParameter entry in QueryData.Where)
                {
                    string where = "", where_value = "";
                    where = entry.PropertyName;
                    where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                    builder.Where($"{where} {entry.Operator} @{where_value}");
                    parameter.Add(where_value, entry.PropertyValue);
                }
            }
            int affectedrows = this._db.Execute(selector.RawSql, parameter);
            return affectedrows >= 0;
        }

        public bool SaveUpdate(List<QueryParam> QueryDataList,List<ModelParameter> SaveDataList)
        {
            foreach(QueryParam QueryData in QueryDataList)
            {
                var builder = new SqlBuilder();
                var selector = builder.AddTemplate($"Update {QueryData.Table} set {QueryData.Fields} /**where**/");
                DynamicParameters parameter = new DynamicParameters();
                if (QueryData.Where != null)
                {
                    foreach (ConditionParameter entry in QueryData.Where)
                    {
                        string where = "", where_value = "";
                        where = entry.PropertyName;
                        where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                        builder.Where($"{where} {entry.Operator} @{where_value}");
                        parameter.Add(where_value, entry.PropertyValue);
                    }
                }
                this._db.Execute(selector.RawSql, parameter);
            }
            foreach(ModelParameter SaveData in SaveDataList)
            {
               // this.Add(SaveData.SaveModel);
            }

            return true;
        }

        public IEnumerable<T> UnionQuery(List<QueryParam> QueryList)
        {
            string UnionSql = "";
            DynamicParameters parameter = new DynamicParameters();
            foreach (QueryParam Query in QueryList)
            {
                var builder = new SqlBuilder();

                var selector = builder.AddTemplate($"SELECT distinct {Query.Fields} FROM {Query.Table} /**innerjoin**/ /**where**/ /**orderby**/");

                if (Query.Join != null)
                {
                    foreach (JoinParameter entry in Query.Join)
                    {
                        builder.InnerJoin($"{entry.table} ON {entry.condition}");
                    }
                }
                if (Query.Where != null)
                {
                    foreach (ConditionParameter entry in Query.Where)
                    {
                        string where = "", where_value = "";
                        where = entry.PropertyName;
                        where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                        builder.Where($"{where}{entry.Operator}@{where_value}");
                        parameter.Add(where_value, entry.PropertyValue);
                    }
                }
                if (Query.OrderBy != null)
                {
                    builder.OrderBy(Query.OrderBy);
                }
                UnionSql += selector.RawSql + " Union All ";
            }
            UnionSql = UnionSql.Substring(0, UnionSql.Length - "Union All ".Length);
            return this._db.Query<T>(UnionSql, parameter);

        }




        public int MaxKey()
        {
            string query = $"select max({GetPrimaryKey()}) from {TableName()} ";
            return Convert.ToInt32(this._db.Query<string>(query).FirstOrDefault());
        }
    }
}
