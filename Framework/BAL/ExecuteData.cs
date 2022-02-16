using FluentValidation;
using Framework.DataAccess.Dapper;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.BAL
{
    //public interface IExecuteData<out T>
    //{

    //}
    //public class ExecuteData //<T> where T : class
    //{
    //  //  public IRepository<T> Repository { get; set; }
    //    public IValidator ValidateModel { get; set; }
    //    public IBaseModel<T> Model { get; set; }

    //        }

    public class ExecuteData
    {
        public IValidator validateModel { get; set; }
        public IBaseModel item { get; set; }
    }
}
