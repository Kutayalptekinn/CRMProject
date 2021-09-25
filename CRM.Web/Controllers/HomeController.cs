using CRM.Web.Controllers.Abstract;
using CRM.Web.Filters;
using Microsoft.Build.BuildEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using Thinktecture.IdentityModel.Authorization.Mvc;


namespace CRM.Web.Controllers
{
    public class HomeController : BaseController
    {


        public ActionResult Index()
        {
            return View();
        }
       // [CustomAuthorizeFilter(UserAuthCodes ="C")]
      


        
       
    }
}