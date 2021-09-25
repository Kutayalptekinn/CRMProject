using CRM.Data;
using CRM.Service.Admin;
using CRM.ViewModels.Admin;
using CRM.Web.Controllers.Abstract;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static CRM.ViewModels.Admin.LoginViewModel;

namespace CRM.Web.Controllers
{

    public class LoginController : BaseController
    {
        //+
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private readonly SettingService _settingService;
        public LoginController(SettingService settingService)
        {
            _settingService = settingService;

        }
        private void _signIn(long userId, bool rememberMe = false)
        {
            var identity = new ClaimsIdentity(
                   new[] {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()/*UserId*/)

                  },
                       DefaultAuthenticationTypes.ApplicationCookie,
                       ClaimTypes.NameIdentifier, ClaimTypes.Role);

            identity.AddClaim(
                        new Claim(
                                "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider"/*schema url*/
                                , "ASP.NET Identity"
                                , "http://www.w3.org/2001/XMLSchema#string"
                                )
                            );
            //++
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe /*RememberMe*/}, identity);
        }
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = "Giriş";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]

        public ActionResult Index(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {


                var loginResultwithrole = _settingService.LoginWithRole(model);
                var loginResult = _settingService.Login(model);
                if (loginResult && loginResultwithrole != null)
                {
                    //getvalueordefault
                    _signIn(loginResultwithrole.GetValueOrDefault(), true);
                    return RedirectToLocalOr(returnUrl, () => RedirectToAction("Index", "Home", new { Area = String.Empty }));
                }
            }


            // If we got this far, something failed, redisplay form

            return View(model);
        }
        
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }



    }

}
