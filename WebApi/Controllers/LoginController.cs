using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    public class LoginController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/login/forall")]
        public IHttpActionResult Get()
        {
            return Ok("Restricted Access");
        }

        [Authorize]
        [HttpGet]
        [Route("api/login/authenticate")]
        public IHttpActionResult GetAuthentication()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return Ok("Authentication Successful :" + identity.Name);

        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/login/authorize")]
        public IHttpActionResult GetAuthorization()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return Ok("Authorized Access:" + identity.Name + "Role:" + string.Join(",", roles.ToList()));

        }

        [HttpPost]
        [Route("api/login/forgotpassword")]
        public IHttpActionResult GetResetLink(ForgotPassword resetter)
        {
            using (PsiogEntities PE = new PsiogEntities())
            {
                string VerificationCode = Guid.NewGuid().ToString();
                var link = HttpContext.Current.Request.Url.AbsoluteUri +"/reset/" + resetter.employeeId;

                var result = PE.Users.Where(u => u.EmployeeId == resetter.employeeId).FirstOrDefault();

                if (result != null)
                {
                    if (string.Compare(result.Answer, resetter.answer) == 0)
                    {
                        var employee = PE.Employees.Where(u => u.EmployeeId == resetter.employeeId).FirstOrDefault();

                        var mail = ComposeMail.SendResetLink(employee.Email, out SmtpClient messenger);

                        mail.Body += link + ">Click here to reset password</a>";
                        messenger.Send(mail);
                        messenger.Dispose();
                        return Ok("Reset link has been sent to your registered email ID");
                    }

                    else
                    {
                        return Ok("Incorrect answer for security question");
                    }


                }

                else
                {
                    return Ok("Employee Id does not exist!");
                }

            }
        }


        [HttpPost]
        [Route("api/login/forgotpassword/reset/{id=id}")]
        public IHttpActionResult ResetPassword(string id, UpdatePassword pass)
        {
            string message = DBOperations.ResetPassword(pass.password, id);
            return Ok("Updated password successfully!");
        }
    }
}
