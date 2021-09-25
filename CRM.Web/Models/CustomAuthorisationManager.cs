using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CRM.Web
{
    public class CustomAuthorisationManager: ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            {
                string resource = context.Resource.First().Value;
                string action = context.Action.First().Value;

                if (action == "Show" && resource == "Code")
                {
                    bool livesInSweden = context.Principal.HasClaim(ClaimTypes.Country, "Sweden");
                    bool isAndras = context.Principal.HasClaim(ClaimTypes.GivenName, "Andras");
                    return isAndras && livesInSweden;
                }

                return false;
            }
        }
    }
}