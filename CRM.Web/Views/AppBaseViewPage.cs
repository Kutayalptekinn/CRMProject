using CRM.ViewModels.Admin;
using CRM.Web.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Web
{
    public abstract class AppBaseViewPage<TModel> : WebViewPage<TModel>
    {
        protected CurrentUserModel CurrentUser
        {
            get
            {
                if (ViewContext.Controller is BaseController baseController)
                    return baseController.CurrentUser ?? new CurrentUserModel();
                return null;
            }
        }
    }


    public abstract class AppBaseViewPage:AppBaseViewPage<dynamic>
    {

    }
}