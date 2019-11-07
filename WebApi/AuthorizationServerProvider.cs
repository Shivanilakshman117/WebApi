using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider

    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            string message = DBOperations.ValidateLogin(context.UserName, context.Password, out User user);


            using (PsiogEntities PE = new PsiogEntities())
            {
                if (user != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

                    var result = from logger in PE.Users
                                 join employee in PE.Employees on logger.EmployeeId equals employee.EmployeeId
                                 select new
                                 {
                                     username = employee.Name,
                                     employeeid = employee.EmployeeId
                                 };

                    foreach (var l in result)
                    {
                        if (l.employeeid == user.EmployeeId)
                        {
                            identity.AddClaim(new Claim("username", l.username));
                            identity.AddClaim(new Claim(ClaimTypes.Name, l.username));
                            context.Validated(identity);

                        }
                        else
                        {
                            context.SetError(message);
                        }

                    }




                }
                else
                {
                    context.SetError(message);
                }
            }




        }
    }
}