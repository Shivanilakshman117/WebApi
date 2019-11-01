using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    public class AddEmployeeController : ApiController
    {
        [HttpPost]
        [Route("api/AddEmployee/newemployee")]
        public IHttpActionResult AddEmployee(Employee newEmp)
        {
           /* Employee newEmp = new Employee
            {
                EmployeeId = "P118",
                Name = "test",
                Gender = "Female",
                Mobile = 9988774455,
                Email = "test@psiog.com",
                DOB = (DateTime.Now).Date,
                DOJ = (DateTime.Now).Date,
                Address = "abc",
                BloodType = "A+",
                IsManager = "1"
            };*/

            if (DBOperations.AddEmployee(newEmp))
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
                return Ok("Failed to add employee");
            }

        }
        public void  CompleteRegistration(Employee newEmp)
        {
            PsiogEntities PE = new PsiogEntities();
            
            string VerificationCode = Guid.NewGuid().ToString();
            var link = HttpContext.Current.Request.Url.AbsoluteUri + "/VerifyAccount";
            var mail = ComposeMail.CompleteRegistration("techxems@gmail.com", VerificationCode, out SmtpClient messenger);
            var account = PE.Users.Where(a => a.EmployeeId == newEmp.EmployeeId).FirstOrDefault();
            account.VerificationCode = VerificationCode;
            mail.Body += link + ">Click here to complete registration</a>";
            messenger.Send(mail);
            messenger.Dispose();
        }

        [HttpPost]
        [Route("api / AddEmployee / newemployee / VerifyAccount")]
        public IHttpActionResult VerifyAccount(User newEmp)
        {
            return Ok("HI");
        }
    }
   
}
