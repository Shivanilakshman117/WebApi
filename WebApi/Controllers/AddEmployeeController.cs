using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    public class AddEmployeeController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("api/AddEmployee/newemployee")]
        public IHttpActionResult AddEmployee(Employee newEmp)
        {
            if (DBOperations.AddEmployee(newEmp, out string msg))
            {
                if (DBOperations.AddUser(newEmp))
                {
                    if (newEmp.IsManager == "1")
                    {
                        var resultOfAddManagers = StoredProcsCall.AddManagers(newEmp.EmployeeId);
                        if(!resultOfAddManagers)
                        {
                            return Ok("Unable to add employee to managers table");
                        }
                    }
                  
                    var resultOfLeaveAllocation = StoredProcsCall.AllocateLeave(newEmp.EmployeeId, newEmp.DOJ.Month);
                    if(!resultOfLeaveAllocation)
                    {
                        return Ok("Unable to set up leave allocation");

                    }
                    if (!DBOperations.AddReportingAuthorities(newEmp.EmployeeId, newEmp.managerName))
                    {
                        return Ok("Unable to set Reporting Authority!");
                    }
                    CompleteRegistration(newEmp);
                    return Ok("Associate has been added successfully!");
                }

                else
                {
                    return Ok("Failed to add user");
                }
            }
            else
            {
                return Ok(msg);
            }
                
        }
        public void  CompleteRegistration(Employee newEmp)
        {
            using (PsiogEntities PE = new PsiogEntities())
            {
                string VerificationCode = Guid.NewGuid().ToString();
                //var link = HttpContext.Current.Request.Url.AbsoluteUri + "/VerifyAccount?id=" + newEmp.EmployeeId;
                string encodedId = Hasher.EncodeId(newEmp.EmployeeId);
                var link = "http://localhost:4200/verify-employee/" + encodedId;
                var mail = ComposeMail.CompleteRegistration(newEmp.EmployeeId,"techxems@gmail.com", VerificationCode, out SmtpClient messenger);
                var account = PE.Users.Where(a => a.EmployeeId == newEmp.EmployeeId).FirstOrDefault();
               
              
                mail.Body += link + ">Click here to complete registration</a>";
                messenger.Send(mail);
                messenger.Dispose();
            }
        }

        [HttpPost]
        [Route("api/AddEmployee/newemployee/VerifyAccount/{id=id}")]
        public IHttpActionResult VerifyAccount(string id, VerifyUser verifyEmp)
        {

            string message = DBOperations.VerifyUserAccount(id, verifyEmp);
            return Ok(message); 
           
        }
    }
   
}
