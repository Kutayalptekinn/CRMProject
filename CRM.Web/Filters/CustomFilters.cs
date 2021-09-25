using CRM.Service.Common;
using CRM.Web.Controllers.Abstract;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

namespace CRM.Web.Filters
{
    public class CustomAuthorizeFilter : AuthorizeAttribute
    {
        public string UserAuthCodes { get; set; }
        public bool MainUserOnly = false;
        public bool MainCompanyOnly = false;
        public bool PassCurrentSubscription = false;
        private bool _userFound;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            _userFound = false;
            var identity = (ClaimsPrincipal)httpContext.User;
            if (!identity.Identity.IsAuthenticated)
            {
                return false;
            }

            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            var currentCustomerIdClaim = identity.FindFirst("AspNet.Identity.UserCurrentCustomer");
            if (claim == null) return false;
            var commonService = DependencyResolver.Current.GetService<CommonService>();
            var userId = Convert.ToInt64(claim.Value);


            /*Can cause deadlock, be sure to use ConfigureAwait(false) in GetCurrentUserModel*/
            var currentUser = commonService.GetCurrentUserModelAsync(userId).ConfigureAwait(false).GetAwaiter().GetResult();


            if (currentUser == null)
            {
                return false;
            }
            _userFound = true;



            if (MainUserOnly)
            {
                /*Allowed only for mainCompany*/
                if (!currentUser.IsMainUser)
                {
                    return false;
                }
            }

            var arrayUserAuthCodes = string.IsNullOrWhiteSpace(UserAuthCodes) ? new List<string>().ToArray() : UserAuthCodes.Split(',').Select(p => p.Trim()).ToArray();
            return currentUser.AuthorizedForAction(arrayUserAuthCodes);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new Ajax401Response(); // return 401 - unauthorised
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Login",
                                action = "Index",
                                area = String.Empty
                            })
                    );
                }
                return;
            }
            if (_userFound)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new Ajax403Response();
                }
                else
                {
                    var controller = (BaseController)filterContext.Controller;


                    filterContext.Result = controller.RedirectToPreviousOr(() => new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Home",
                                action = "Index",
                                area = String.Empty
                            })
                    ));
                }

            }
            else
            {
                /*Did not found any user so to be sure(User can be deactivated) Signout from owin*/
                filterContext.HttpContext.GetOwinContext()
                    .Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new Ajax401Response();
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "User",
                                action = "Login",
                                area = String.Empty
                            })
                        );
                }
            }
        }
    }
}