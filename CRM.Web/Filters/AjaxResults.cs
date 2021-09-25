using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Web.Filters
{
    public class Ajax401Response : ActionResult
    {
        // Called by the MVC framework to run the action result using the specified controller context
        public override void ExecuteResult(ControllerContext context)
        {
            /*For some reason, TrySkipIisCustomErrors is not honoured if you don't set response.Status.*/
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
            context.HttpContext.Response.Status = context.HttpContext.Response.Status;
            context.HttpContext.Response.StatusCode = 401; /*Dont send 401, if you have not overridden owins onapplyredirect for ajaxcalls*/
            context.HttpContext.Response.Write("Yetkisiz: Geçersiz kimlik bilgileri nedeniyle erişim reddedildi"); // HTTP response
            context.HttpContext.Response.End();
        }

    }

    public class Ajax403Response : ActionResult
    {
        // Called by the MVC framework to run the action result using the specified controller context
        public override void ExecuteResult(ControllerContext context)
        {
            /*For some reason, TrySkipIisCustomErrors is not honoured if you don't set response.Status.*/
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
            context.HttpContext.Response.Status = context.HttpContext.Response.Status;
            context.HttpContext.Response.StatusCode = 403;
            context.HttpContext.Response.Write("İşlem için yetkiniz yok"); // HTTP response
            context.HttpContext.Response.End();
        }
    }

    public class AjaxUnSufficientCreditResponse : ActionResult
    {
        // Called by the MVC framework to run the action result using the specified controller context
        public override void ExecuteResult(ControllerContext context)
        {
            /*For some reason, TrySkipIisCustomErrors is not honoured if you don't set response.Status.*/
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
            context.HttpContext.Response.Status = context.HttpContext.Response.Status;
            context.HttpContext.Response.StatusCode = 420;
            context.HttpContext.Response.Write("Yetersiz bakiye"); // HTTP response
            context.HttpContext.Response.End();
        }
    }
}