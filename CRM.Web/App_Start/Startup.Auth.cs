using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CRM.Web
{
	public partial class Startup
	{
		private static bool IsAjaxRequest(IOwinRequest request)
		{
			IReadableStringCollection query = request.Query;
			if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
			{
				return true;
			}
			IHeaderDictionary headers = request.Headers;
			return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
		}

		// For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Configure the sign in cookie
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				CookieName = "Custom.Cookie",
				//CookieDomain = "prapazar.com",
				CookieSameSite = Microsoft.Owin.SameSiteMode.None, /*Chrome needs this to be set. PaymentCallBacks won't work in chrome if we do not set this*/
				CookieSecure = CookieSecureOption.Always,
				LoginPath = new PathString("/Login/Index"),
				Provider = new CookieAuthenticationProvider
				{
					OnApplyRedirect = ctx =>
					{
						if (!IsAjaxRequest(ctx.Request))
						{
							ctx.Response.Redirect(ctx.RedirectUri);
						}
					},
					OnValidateIdentity = async ctx =>
					{
						/*Check only every 10 minutes*/
						var validateInterval = TimeSpan.FromMinutes(10);

						var currentUtc = DateTimeOffset.UtcNow;
						if (ctx.Options?.SystemClock != null)
						{
							currentUtc = ctx.Options.SystemClock.UtcNow;
						}
						var issuedUtc = ctx.Properties.IssuedUtc;

						// Only validate if enough time has elapsed
						var validate = (issuedUtc == null);
						if (issuedUtc != null)
						{
							var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
							validate = timeElapsed > validateInterval;
						}

						if (validate)
						{
							var claim = ctx.Identity.FindFirst("AspNet.Identity.SecurityStamp");
							var userId = ctx.Identity.GetUserId();
							var currentCustomerIdClaim = ctx.Identity.FindFirst("AspNet.Identity.UserCurrentCustomer");
							if (claim != null && !String.IsNullOrWhiteSpace(userId))
							{
								var reject = true;

								long? currentCustomerId = null;
								if (currentCustomerIdClaim != null)
								{
									currentCustomerId = Convert.ToInt64(currentCustomerIdClaim.Value);
								}



								/* Regenerate fresh claims and resign in, will reset IssuedUtc on claim*/
								reject = false;
								var identity = new ClaimsIdentity(new[] 
								{                        
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
								if (identity != null)
								{
									// Fix for regression where this value is not updated
									// Setting it to null so that it is refreshed by the cookie middleware
									ctx.Properties.IssuedUtc = null;
									ctx.Properties.ExpiresUtc = null;
									ctx.OwinContext.Authentication.SignIn(ctx.Properties, identity);
								}


								if (reject)
								{
									ctx.RejectIdentity();
									ctx.OwinContext.Authentication.SignOut(ctx.Options.AuthenticationType);
								}
							}
						}

					}
				}
			});
		}
	}
}