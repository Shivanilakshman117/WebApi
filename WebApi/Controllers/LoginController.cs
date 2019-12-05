using System;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;

using System.Web.Http;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    public class LoginController : ApiController
    {
       
        [Authorize]
        [HttpGet]
        [Route("api/login/authenticate")]
        public HttpResponseMessage GetAuthentication() 
        {
            var identity = (ClaimsIdentity)User.Identity;
            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
            UserSession current = new UserSession
            {
                employeeId = sid,
                name = identity.Name
            };
            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, current);
            return response;

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
                string encodedId = Hasher.EncodeId(resetter.employeeId);
                var link = "http://localhost:4200/verify-employee/" + Hasher.EncodeId(resetter.employeeId);

                var result = PE.Users.Where(u => u.EmployeeId == resetter.employeeId).FirstOrDefault();

                if (result != null)
                {
                    if (string.Compare(result.Answer, resetter.answer) == 0)
                    {
                        var employee = PE.Employees.Where(u => u.EmployeeId == resetter.employeeId).FirstOrDefault();
                        result.VerificationCode = VerificationCode;
                        try
                        {
                            PE.SaveChanges();
                        }
                        catch(Exception E)
                        {
                            ExceptionLog.Logger(E);
                            return Ok("Unable to get reset link");
                        }
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
        public IHttpActionResult ResetPassword(string encodedId, VerifyUser verifyEmp)
        {
            string id = Hasher.DecodeId(encodedId);
            string message = DBOperations.VerifyUserAccount(id, verifyEmp);
            return Ok("Updated password successfully!");
        }
       
        
    }
}
