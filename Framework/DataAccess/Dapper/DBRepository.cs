using Dapper;
using Framework.CustomDataType;
using Framework.DataAccess.Utility;
using Framework.Library.Filter;
using Framework.Library.Helper;
using Framework.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace Framework.DataAccess.Dapper
{
    public class DBRepository : IDBRepository
    {

        private readonly IDbConnection _db;
        private string _connection_string;
        private Type _Type;
        public DBRepository()
        {
            try
            {
                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FrameworkPath("DataAccess\\Config\\Setting.json")));
                string Environment = json.SelectToken("Environment").ToString();
                if (Environment.IsEmpty())
                {
                    Environment = "Development";
                }
                _connection_string = json.SelectToken(Environment).Value<string>("ConnectionString");
                _db = new SqlConnection(_connection_string);
            }
            catch 
            {

            }

        }
        public string AddWithoutSentbox(BaseModel SaveModel)
        {
            _Type = SaveModel.GetType();
            SetDefaultParam(ref SaveModel);
            var query = $"insert into {_Type.GetTableName()} {this.ColumnField()};SELECT SCOPE_IDENTITY()";
            string pk = this._db.Query<string>(query, SaveModel, commandTimeout: 0).SingleOrDefault();
            return pk;
        }
        public dynamic Add(BaseModel SaveModel)
        {
            string pk = this.AddWithoutSentbox(SaveModel);
            if (pk == null)
            {
                ModelInfo model = SaveModel.GetPrimaryKey();
                pk = model.PropertyValue.ToString();
            }
            else
            {
                ModelInfo model = SaveModel.GetPrimaryKey();
                if (model.Property.GetType().ToString() == "System.String")
                {
                    model.Property.SetValue(SaveModel, pk, null);
                }
                else
                {
                    model.Property.SetValue(SaveModel, Convert.ToInt32(pk), null);
                }
            }

            SentBoxUtility SentBoxUtilityObj = new SentBoxUtility();
            SentBoxUtilityObj.SaveSentBox(SaveModel);

            CustomEditTransaction(SaveModel);
            return pk;
        }
        public string Add(List<BaseModel> SaveModelList)
        {
            using (var transaction = new TransactionScope())
            {
                int i = 0, parent_value = 0;
                string pk = "0", tmp_pk = "0";
                foreach (BaseModel SaveModel1 in SaveModelList)
                {
                    BaseModel SaveModel = SaveModel1;
                    if (SaveModel.parent_child != "none" && SaveModel.parent_child != "parent")
                    {
                        PropertyInfo propertyInfo = SaveModel.GetType().GetProperty(SaveModel.parent_child);
                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            propertyInfo.SetValue(SaveModel, parent_value.ToString(), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(SaveModel, parent_value, null);
                        }

                    }
                    tmp_pk = this.Add(SaveModel);
                    if (SaveModel.parent_child == "parent")
                    {
                        parent_value = Convert.ToInt32(tmp_pk);
                    }
                    if (i == 0)
                    {
                        pk = tmp_pk;
                    }
                    i++;
                }
                transaction.Complete();
                return pk;
            }
        }
        public void BulkAdd(List<IBaseModel> SaveModelList, string TableName, DataTable Dt, List<string> ColumnMapping)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (BaseModel SaveModel in SaveModelList.NotEmpty())
                {
                    _Type = SaveModel.GetType();
                    var query = $"insert into {_Type.GetTableName()} {this.ColumnField()}";
                    _db.Execute(query, SaveModel);
                }
                var bulkCopy = new SqlBulkCopy(_connection_string);
                foreach (string s in ColumnMapping)
                {
                    bulkCopy.ColumnMappings.Add(s, s);
                }
                bulkCopy.DestinationTableName = TableName;
                bulkCopy.BatchSize = 50000;
                bulkCopy.BulkCopyTimeout = 300; //seconds
                bulkCopy.WriteToServer(Dt);
                transaction.Complete();
            }
        }

        public void Add(List<QueryParam> InsertList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (QueryParam Query in InsertList)
                {
                    var builder = new SqlBuilder();
                    var selector = builder.AddTemplate($"{Query.DirectQuery}  /**where**/");
                    DynamicParameters parameter = new DynamicParameters();
                    foreach (ConditionParameter entry in Query.Where.NotEmpty())
                    {
                        string where = "", where_value = "";
                        where = entry.PropertyName;
                        where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                        if (entry.Operator.ToLower() == "is" && entry.PropertyValue == "null")
                        {
                            builder.Where($"{where} {entry.Operator} {entry.PropertyValue}");
                        }
                        else
                        {
                            builder.Where($"{where} {entry.Operator} @{where_value}");
                            parameter.Add(where_value, entry.PropertyValue);
                        }


                    }
                    foreach (KeyValuePair<string, dynamic> Entry in Query.DynemicParam.NotEmpty())
                    {
                        parameter.Add(Entry.Key, Entry.Value);
                    }
                    this._db.Execute(selector.RawSql, parameter);
                }
                transaction.Complete();
            }

        }

        public void Add(QueryParam Query)
        {
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate($"{Query.DirectQuery}  /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            foreach (ConditionParameter entry in Query.Where.NotEmpty())
            {
                parameter.Add(entry.PropertyName, entry.PropertyValue);
            }
            this._db.Execute(selector.RawSql, parameter);

        }

        public int AUDOperation(List<BaseModel> SaveModelList)
        {
            using (var transaction = new TransactionScope())
            {
                int parent_value = 0;
                string tmp_pk = "0";
                foreach (BaseModel SaveModel1 in SaveModelList)
                {
                    BaseModel SaveModel = SaveModel1;
                    if (SaveModel.model_operation == "insert")
                    {
                        if (SaveModel.parent_child != "none" && SaveModel.parent_child != "parent")
                        {
                            PropertyInfo propertyInfo = SaveModel.GetType().GetProperty(SaveModel.parent_child);
                            if (propertyInfo.PropertyType.FullName == "System.String")
                            {
                                propertyInfo.SetValue(SaveModel, parent_value.ToString(), null);
                            }
                            else
                            {
                                propertyInfo.SetValue(SaveModel, parent_value, null);
                            }

                        }
                        tmp_pk = this.Add(SaveModel);
                        if (SaveModel.parent_child == "parent")
                        {
                            parent_value = Convert.ToInt32(tmp_pk);
                        }
                    }
                    else if (SaveModel.model_operation == "update")
                    {
                        this.SingleUpdate(SaveModel);
                    }
                    else
                    {
                        this.Delete(SaveModel);
                    }

                    //set box logic

                }
                transaction.Complete();
                return 1;
            }            
        }

        public void Update(BaseModel SaveModel)
        {
            using (var transaction = new TransactionScope())
            {
                this.SingleUpdate(SaveModel);
                transaction.Complete();
            }
        }
        private void SingleUpdate(BaseModel SaveModel)
        {
            if(SaveModel.operation_type.ToLower()=="delete_special")
            {
                SaveModel.operation_type = "DELETE";
            }else if(SaveModel.operation_type.ToLower() == "insert_special")
                SaveModel.operation_type = "INSERT";
            else
                SaveModel.operation_type = "UPDATE";
            _Type = SaveModel.GetType();
            SetHistory(SaveModel);
            SetDefaultParam(ref SaveModel, 2);
            string pk = _Type.GetPrimaryKey();
            var query = $"update {_Type.GetTableName()} set {this.ColumnField("update")} where {pk} = @{pk}";
            _db.Execute(query, SaveModel);

            //set box logic
            SentBoxUtility SentBoxUtilityObj = new SentBoxUtility();
            SentBoxUtilityObj.SaveSentBox(SaveModel);
            // set box logic end

            CustomEditTransaction(SaveModel);

        }
        public int Update(List<BaseModel> SaveModelList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (BaseModel SaveModel1 in SaveModelList)
                {
                    BaseModel SaveModel = SaveModel1;
                    this.SingleUpdate(SaveModel);
                }
                transaction.Complete();
            }
            return 1;
        }
        public bool Update(QueryParam QueryData)
        {
            var builder = new SqlBuilder();
            var selector = builder.AddTemplate($"Update {QueryData.Table} set {QueryData.Fields} /**where**/");
            DynamicParameters parameter = new DynamicParameters();

            foreach (ConditionParameter entry in QueryData.Where.NotEmpty())
            {
                string where = "", where_value = "";
                where = entry.PropertyName;
                where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                if (entry.Operator.ToLower() == "is" && entry.PropertyValue == "null")
                {
                    builder.Where($"{where} {entry.Operator} {entry.PropertyValue}");
                }
                else
                {
                    builder.Where($"{where} {entry.Operator} @{where_value}");
                    parameter.Add(where_value, entry.PropertyValue);
                }
            }
            int affectedrows = this._db.Execute(selector.RawSql, parameter);
            return affectedrows >= 0;
        }
        public void Update(List<QueryParam> QueryDataList)
        {
            using (var transaction = new TransactionScope())
            {
                foreach (QueryParam QueryData in QueryDataList)
                {
                    this.Update(QueryData);
                }
                transaction.Complete();
            }

        }

        public bool Delete(QueryParam QueryData)
        {
            var builder = new SqlBuilder();
            string query = (QueryData.DirectQuery.Empty()) ? $"Delete FROM {QueryData.Table}" : QueryData.DirectQuery;
            var selector = builder.AddTemplate($"{query}  /**where**/");
            DynamicParameters parameter = new DynamicParameters();
            foreach (ConditionParameter entry in QueryData.Where.NotEmpty())
            {
                string where = "", where_value = "";
                where = entry.PropertyName;
                where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);
                builder.Where($"{where} {entry.Operator} @{where_value}");
                parameter.Add(where_value, entry.PropertyValue);
            }

            int affectedrows = this._db.Execute(selector.RawSql, parameter);
            return affectedrows >= 0;
        }
        public int Delete(List<BaseModel> SaveModelList)
        {
            using (var transaction = new TransactionScope())
            {
                int i = 0;
                foreach (BaseModel SaveModel1 in SaveModelList)
                {
                    BaseModel SaveModel = SaveModel1;
                    this.Delete(SaveModel);
                }
                transaction.Complete();
            }
            return 1;
        }
        public int Delete(BaseModel SaveModel)
        {
            SaveModel.operation_type = "DELETE";
            _Type = SaveModel.GetType();
            SetHistory(SaveModel);
            SetDefaultParam(ref SaveModel, 3);
            string pk = _Type.GetPrimaryKey();
            var query = $"delete from {_Type.GetTableName()} where {pk} = @{pk}";
            CustomEditTransaction(SaveModel);
            return _db.Execute(query, SaveModel);

        }

        public IEnumerable<T> FindAll<T>(QueryParam Query)
        {
            if (Query.Sp.NotEmpty())
            {
                return this.ExecuteSp<T>(Query);
            }
            else
            {
                return this.ExecuteQuery<T>(Query);
            }
        }
        public IEnumerable<object> FindAll(QueryParam Query)
        {
            //if (Query.Sp.NotEmpty())
            //{
            //    return this.ExecuteSp<Object>(Query);
            //}
            //else
            //{
            //    return this.ExecuteQuery<Object>(Query);
            //}
            return this.FindAll<object>(Query);
        }
        public List<T> FindAll<T>(List<ConditionParameter> Condition)
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
            return this._db.Query<T>(selector.RawSql, parameter, commandTimeout: 0).ToList();
        }
        public T Find<T>(QueryParam Query)
        {
            if (Query.Sp.NotEmpty())
            {
                return this.ExecuteSp<T>(Query).FirstOrDefault();
            }
            else
            {
                return this.ExecuteQuery<T>(Query).FirstOrDefault();
            }
        }
        public T FindByKey<T>(dynamic Id)
        {
            QueryParam query = new QueryParam
            {
                Fields = "*",
                Where = new List<ConditionParameter>
                {
                    new ConditionParameter{PropertyName=typeof(T).GetPrimaryKey(),PropertyValue=Id}
                }
            };
            return this.Find<T>(query);
        }
        public T FindByColumn<T>(List<ConditionParameter> Where) where T : BaseModel
        {
            QueryParam query = new QueryParam
            {
                Fields = "*",
                Where = Where
            };
            return this.Find<T>(query);
        }

        public int Count<T>(QueryParam Query) where T : BaseModel
        {
            _Type = typeof(T);
            Query.DirectQuery = $"SELECT count({_Type.GetTableName()}.{_Type.GetPrimaryKey()}) FROM {_Type.GetTableName()}";
            return this.ExecuteQuery<int>(Query).FirstOrDefault();
        }
        public IEnumerable<T> UnionQuery<T>(List<QueryParam> QueryList)
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
                        //builder.Where($"{where}{entry.Operator}@{where_value}");
                        //parameter.Add(where_value, entry.PropertyValue);

                        if (entry.PropertyValue != null && entry.PropertyValue.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                        {
                            entry.Operator = " IN ";
                            var ds = (JArray)entry.PropertyValue;
                            string[] tmp = ds.Select(e => (string)e).ToArray();
                            parameter.Add(where_value, tmp);
                            builder.Where($"{where} {entry.Operator} @{where_value}");
                        }
                        else
                        {
                            parameter.Add(where_value, entry.PropertyValue);
                            builder.Where($"{where} {entry.Operator} @{where_value}");
                        }
                    }
                }
                if (Query.OrderBy != null)
                {
                    builder.OrderBy(Query.OrderBy);
                }
                UnionSql += selector.RawSql + " Union ";
            }
            UnionSql = UnionSql.Substring(0, UnionSql.Length - "Union ".Length);
            return this._db.Query<T>(UnionSql, parameter, commandTimeout: 0);

        }
        private IEnumerable<T> ExecuteSp<T>(QueryParam Query)
        {
            DynamicParameters parameter = new DynamicParameters();
            foreach (ConditionParameter Condition in Query.Where.NotEmpty())
            {
                parameter.Add($"@{Condition.PropertyName}", Condition.PropertyValue);
            }
            return this._db.Query<T>(sql: Query.Sp, param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
        }

        public void ExecuteSp(QueryParam Query)
        {
            DynamicParameters parameter = new DynamicParameters();
            foreach (ConditionParameter Condition in Query.Where.NotEmpty())
            {
                parameter.Add($"@{Condition.PropertyName}", Condition.PropertyValue);
            }
            this._db.Query(sql: Query.Sp, param: parameter, commandType: CommandType.StoredProcedure, commandTimeout: 0);
        }
        private IEnumerable<T> ExecuteQuery<T>(QueryParam Query)
        {

            DynamicParameters parameter = new DynamicParameters();

            SqlBuilder builder = new SqlBuilder();
            SqlBuilder.Template selector;
            string FinalQuery = string.Empty;
            if (Query.DirectQuery.NotEmpty())
            {
                selector = builder.AddTemplate($"{Query.DirectQuery} /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**orderby**/");
            }
            else
            {
                if (typeof(T) != typeof(object) && !Query.Table.NotEmpty())
                {
                    Query.Table = typeof(T).GetTableName();
                }
                selector = builder.AddTemplate($"SELECT {Query.Distinct} {Query.Fields} FROM {Query.Table} /**innerjoin**/ /**leftjoin**/  /**where**/ /**groupby**/ /**orderby**/");
            }
            foreach (JoinParameter entry in Query.Join.NotEmpty())
            {
                if (entry.type.ToLower() == "left")
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
                if (entry.PropertyValue != null && entry.PropertyValue.GetType() == typeof(string) && entry.PropertyValue == "?")
                {
                    throw new Exception("Param Value not Bind");
                }
                string where = "", where_value = "";


                where = entry.PropertyName;
                where_value = entry.PropertyName.Substring(entry.PropertyName.IndexOf('.') + 1);

                //if (entry.PropertyValue != null && entry.PropertyValue.GetType() == typeof(System.DateTime))
                //{
                //    entry.PropertyValue = entry.PropertyValue.ToString("yyyy-MM-dd hh:mm",CultureInfo.InvariantCulture);
                //}
                if (entry.Operator.ToLower() == "between")
                {
                    if (entry.BetweenParam.Param1.GetType() == typeof(System.DateTime) && entry.BetweenParam.Param1.GetType() == typeof(System.DateTime))
                    {
                        entry.BetweenParam.Param1 = entry.BetweenParam.Param1.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        entry.BetweenParam.Param2 = entry.BetweenParam.Param2.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    }
                    builder.Where($"{where} {entry.Operator} @{where_value}1 And @{where_value}2");
                    parameter.Add(where_value + "1", entry.BetweenParam.Param1);
                    parameter.Add(where_value + "2", entry.BetweenParam.Param2);
                }
                else
                {
                    if (entry.Operator.ToLower() == "is" && entry.PropertyValue == "null")
                    {
                        builder.Where($"{where} {entry.Operator} {entry.PropertyValue}");
                    }
                    else
                    {
                        if (entry.direct_condition != "n")
                        {
                            builder.Where($"{entry.direct_condition}");
                            if (entry.PropertyValue.GetType() != typeof(System.String) || entry.PropertyValue != "#$#")
                                parameter.Add(where_value, entry.PropertyValue);
                        }
                        else if (entry.PropertyValue != null && entry.PropertyValue.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                        {
                            entry.Operator = " IN ";
                            var ds = (JArray)entry.PropertyValue;
                            string[] tmp = ds.Select(e => (string)e).ToArray();
                            parameter.Add(where_value, tmp);
                            builder.Where($"{where} {entry.Operator} @{where_value}");
                        }
                        else
                        {
                            parameter.Add(where_value, entry.PropertyValue);
                            builder.Where($"{where} {entry.Operator} @{where_value}");
                        }

                    }
                }
            }
            foreach (KeyValuePair<string, dynamic> Entry in Query.DynemicParam.NotEmpty())
            {
                parameter.Add(Entry.Key, Entry.Value);
            }
            if (Query.GroupBy.NotEmpty())
            {
                builder.GroupBy(Query.GroupBy);
                if (Query.Having.NotEmpty())
                {
                    builder.Having(Query.Having);
                }
            }
            FinalQuery = selector.RawSql;
            if (Query.OrderBy.NotEmpty())
            {
                builder.OrderBy(Query.OrderBy);
                FinalQuery = selector.RawSql;
                if (Query.Offset != -1 && Query.Limit != 0)
                {
                    FinalQuery += " OFFSET @OFFSET ROWS FETCH NEXT @LIMIT ROWS ONLY";
                    parameter.Add("@OFFSET", Query.Offset);
                    parameter.Add("@LIMIT", Query.Limit);
                }

            }
            //   Task.Delay(2000).Wait();
            //LoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddFile($"{FileHelper.ProjectPath()}//FileServer//mylog.txt");
            //Microsoft.Extensions.Logging.ILogger logger1 = loggerFactory.CreateLogger("test");
            //logger1.LogInformation(FinalQuery);
            //logger1.LogInformation(GetParameterLogString(parameter));


            return this._db.Query<T>(FinalQuery, parameter, commandTimeout: 0);

        }

        private void SetHistory(BaseModel SaveModel)
        {

            string history_name = _Type.GetHistoryTableName();
            if (history_name.Trim() != "")
            {
                SetDefaultParam(ref SaveModel, 3);
                ModelInfo pk = SaveModel.GetPrimaryKey();
                var columns = _Type.GetColumnsForHistory();
                var stringOfColumns = string.Join(", ", columns);
                QueryParam query = new QueryParam
                {
                    Fields = $"{stringOfColumns},'{SaveModel.history_created_by}' as history_created_by,'{SaveModel.history_created_at}' as history_created_at,'{SaveModel.operation_type}' as operation_type",
                    Table = _Type.GetTableName(),
                    Where = new List<ConditionParameter>
                    {
                        new ConditionParameter{PropertyName=pk.PropertyName,PropertyValue=pk.PropertyValue}
                    }
                };
                var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
                var queryInsert = $"insert into {_Type.GetHistoryTableName()} {this.ColumnField("history")};";
                object tmp = this.Find<object>(query);
                this._db.Execute(queryInsert, tmp);
            }

        }
        private string ColumnField(string type = "insert")
        {

            if (type == "insert")
            {
                var columns = _Type.GetColumns();
                string stringOfColumns = string.Join(", ", columns);
                string stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
                return $"({ stringOfColumns}) values({ stringOfParameters})";
            }
            else if (type == "update")
            {
                List<string> columns = _Type.GetColumns().ToList();
                columns.Remove("created_at");
                columns.Remove("created_by");
                columns.Remove("originating_type");
                return string.Join(", ", columns.Select(e => $"{e} = @{e}"));
            }
            else
            {
                var columns = _Type.GetHistoryColumns();
                string stringOfColumns = string.Join(", ", columns);
                string stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
                return $"({ stringOfColumns}) values({ stringOfParameters})";
            }
        }

        private void CustomEditTransaction(BaseModel SaveModel)
        {
            if (SaveModel.custom_edit != "0")
            {

                SetDefaultParam(ref SaveModel, 3);
                QueryParam Query = new QueryParam
                {
                    DirectQuery = $"insert into approval_transaction_history select *,'{SaveModel.history_created_by}','{SaveModel.history_created_at}','DELETE' from approval_transaction where approval_transaction_code=@approval_transaction_code",
                    Where = new List<ConditionParameter>()
                        {
                            new ConditionParameter{ PropertyName="approval_transaction_code",PropertyValue=SaveModel.custom_edit }
                        }
                };
                this.Add(Query);

                Query = new QueryParam
                {
                    Table = "approval_transaction",
                    Where = new List<ConditionParameter>()
                        {
                            new ConditionParameter{PropertyName="approval_transaction_code",PropertyValue=SaveModel.custom_edit}
                        }
                };
                this.Delete(Query);

            }
        }

        /*
         * 1: create
         * 2: update
         * 3: history
         *
        */
        private void SetDefaultParam(ref BaseModel SaveModel, int flag = 1)
        {
            switch (flag)
            {
                case 1:
                    if (SaveModel.created_by == null || SaveModel.created_by == "")
                        SaveModel.created_by = UserData.user_code;
                    if (SaveModel.created_at == null)
                        SaveModel.created_at = DateHelper.CurrentDate();
                    break;
                case 2:
                    SaveModel.updated_by = UserData.user_code;
                    SaveModel.updated_at = DateHelper.CurrentDate();
                    break;
                case 3:
                    SaveModel.history_created_by = UserData.user_code;
                    SaveModel.history_created_at = DateHelper.CurrentDate();
                    break;
                default: break;
            }
        }


        protected string GetParameterLogString(DynamicParameters Parameter)
        {
            string outputText;
            StringBuilder output = new StringBuilder();
            output.Append("; ");
            foreach (var paramName in Parameter.ParameterNames)
            {
                var value = Parameter.Get<dynamic>(paramName);
                output.Append(string.Format("{0} = '{1}'", paramName, value));
                output.Append(", ");
            }
            outputText = output.ToString();
            return outputText;
        }
    }
}
