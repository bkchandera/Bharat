using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using System.Collections.Generic; 
using Framework.Models;
using System;
using Framework.Library.Helper;
using System.Reflection;
using Framework.Extension;
using Framework.DataAccess.Dapper;

namespace Framework.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase,IBaseController
    {

       
        protected string Token,FileName= "\\Areas\\";


        /*
        * Token : used in list service.
        *         value : Remove 'Controller' from controller name eg. StateController,Token=state
        *                  if it is differ then controller name then set it from specific controller 
        * FileName : used in list service.
        *          value : will be "\\Areas\\General\\Config\\List.json"
        *          if it is differ then set it from specific controller
        */
        protected void SetParam(string Flag="List",bool refresh=false)
        {
            if (refresh)
            {
                FileName = "\\Areas\\";
            }
            Token = this.GetType().Name.Replace("Controller", "").ToLower();
            object area = null;
            if (RouteData.Values.TryGetValue("area", out area))
            {
                FileName += area.ToString() + $"\\Config\\{Flag}.json";
            }
        }
        //protected FluentValidation.Results.ValidationResult IsValid(Type ValidatorType,object ValidateObject)
        //{                       
        //    var validator = Activator.CreateInstance(ValidatorType);           
        //    MethodInfo method = ValidatorType.GetMethod("Validate",new Type[] { ValidateObject.GetType()});
        //    var retrun_value=method.Invoke(validator,new object[] { ValidateObject });
        //    return (FluentValidation.Results.ValidationResult)retrun_value;

        //}

        //protected IActionResult save(Dictionary<Type,BaseModel> ObjectList) 
        //{
        //    foreach(KeyValuePair<Type,BaseModel> e in ObjectList)            
        //    {
        //        var t=IsValid(e.Key, e.Value);
        //        if (!t.IsValid)
        //        {
        //            t.AddToModelState(ModelState, null);
        //            return BadRequest(ModelState);
        //        }


        //        Type typeArgument = e.Value.GetType();
        //        Type genericClass = typeof(BaseRepository<>);                
        //        Type constructedClass = genericClass.MakeGenericType(typeArgument);
        //        object created = Activator.CreateInstance(constructedClass);
        //        MethodInfo method = constructedClass.GetMethod("Add", new Type[] { typeArgument });
        //        var retrun_value = method.Invoke(created, new object[] { e.Value });


        //    }

        //    return Ok("done");
        //}

        //protected IActionResult UpdateModel(Dictionary<Type, BaseModel> ObjectList)
        //{
        //    foreach (KeyValuePair<Type, BaseModel> e in ObjectList)
        //    {
        //        var t = IsValid(e.Key, e.Value);
        //        if (!t.IsValid)
        //        {
        //            t.AddToModelState(ModelState, null);
        //            return BadRequest(ModelState);
        //        }


        //        Type typeArgument = e.Value.GetType();
        //        Type genericClass = typeof(BaseRepository<>);
        //        Type constructedClass = genericClass.MakeGenericType(typeArgument);
        //        object created = Activator.CreateInstance(constructedClass);
        //        MethodInfo method = constructedClass.GetMethod("Update", new Type[] { typeArgument });
        //        var retrun_value = method.Invoke(created, new object[] { e.Value });


        //    }

        //    return Ok("done");
        //}

        //protected JsonResult listData(Type t)
        //{

        //    object modelObject= Activator.CreateInstance(t);
        //    return new JsonResult(modelObject);
        //}

        //[HttpGet]
        //[Route("general-list")]
        //public JsonResult GeneralList(string m)
        //{
        //    return new JsonResult(m);
        //}
    }
}
