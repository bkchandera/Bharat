using Framework.CustomDataType;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Framework.DataAccess.Dapper{

    public interface IRepository<T> where T : class
    {
        dynamic Add(T item);
        void Remove(T item);
        bool Update(T item);
        bool Update(QueryParam QueryData);
        T FindByID(int id);
        T FindByID(string id);       
        T Find(string fields, Func<T, bool> predicate);
        T Find(QueryParam Query);
        T Find(string TableName, string Fields, List<JoinParameter> join = null, List<ConditionParameter> ConditionList = null);
        IEnumerable<T> FindAll();        
        IEnumerable<T> FindAll(string TableName, string Fields, List<JoinParameter> join = null, List<ConditionParameter> ConditionList = null);
        IEnumerable<T> FindAll(QueryParam Qugery);
        List<T> FindAll(List<ConditionParameter> Condition);
        void Insert_Sp(T item);
        IEnumerable<T> ListSp();
        IEnumerable<T> ListSp(string SpName, int Offset, int Limit);
        string GetPrimaryKey(Type Model);
        int Count(List<ConditionParameter> paramList);
        int MaxKey(ConditionParameter ConditionList = null, List<JoinParameter> join = null);
    }
       
}
