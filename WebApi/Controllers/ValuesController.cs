using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

using WebApi.Helpers;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("api/Values/leavebalance")]
        public HttpResponseMessage Leavebalance()
        {
            var identity = (ClaimsIdentity)User.Identity;
            Requestor r = new Requestor(identity);
            using (PsiogEntities PE = new PsiogEntities())
            {
                var balance = (from l in PE.LeaveTypes
                               join bal in PE.EmployeeLeaveAvaliabilities on
                               l.LeaveTypeId equals bal.LeaveTypeId
                               where bal.EmployeeId == r.employeeId
                               select new { l.Type, bal.AllocatedDays, bal.AvailedDays }).ToList();



                //var json = new JavaScriptSerializer().Serialize(holidaysList);
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, balance);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/GetSearchedEmployees")]
        public HttpResponseMessage GetSearchedEmployees(Searcher search)
        {
            List<string> employeesList;
            using (PsiogEntities PE = new PsiogEntities())
            {
                string name = search.employeeName;
                employeesList = (from emp in PE.Employees
                                 where emp.Name.Contains(search.employeeName)
                                 select emp.Name).ToList();
            }
            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, employeesList);
            return response;
        }

        [Authorize]
        [HttpPost]
        [Route("api/Values/GetAllEmployees")]
        public HttpResponseMessage GetEmployees()
        {

            using (PsiogEntities PE = new PsiogEntities())
            {
                var employeesList = (from emp in PE.Employees
                                     select new
                                     {
                                         emp.Name,
                                         emp.Mobile,
                                         emp.Email,
                                         emp.DOB,
                                         emp.DOJ,
                                         emp.Address,
                                         emp.managerName,
                                         emp.Department,
                                         emp.Designation,
                                         emp.Image

                                     }).ToList();
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, employeesList);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/GetCCToList")]
        public HttpResponseMessage GetCCToList()
        {

            using (PsiogEntities PE = new PsiogEntities())
            {
                var employeesList = (from emp in PE.Employees
                                     select new
                                     {
                                        
                                         emp.Name,
                                        

                                     }).ToList();
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, employeesList);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/GetReportingAuthorities")]
        public HttpResponseMessage GetReportingAuthorities()
        {
            List<string> authoritiesList;
            using (PsiogEntities PE = new PsiogEntities())
            {
                authoritiesList = (from emp in PE.Employees
                                   join rep in PE.ReportingAuthorities
                                   on emp.EmployeeId equals rep.ManagerId
                                   select emp.Name).ToList();
            }

            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, authoritiesList);
            return response;
        }


        [Authorize]
        [HttpPost]
        [Route("api/Values/GetHolidaysList")]
        public HttpResponseMessage GetHolidaysList()
        {


            using (PsiogEntities PE = new PsiogEntities())
            {
                var holidaysList = (from hols in PE.Holidays
                                    orderby hols.Date
                                    select hols).ToList();

                //var json = new JavaScriptSerializer().Serialize(holidaysList);
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, holidaysList);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/GetManagersList")]
        public HttpResponseMessage GetManagersList()
        {
            using (PsiogEntities PE = new PsiogEntities())
            {
                var managersList = (from emp in PE.Employees
                                    join managers in PE.Managers
                                    on emp.EmployeeId equals managers.ManagerId
                                    select emp.Name).ToList();
                if(managersList.Count==0)
                {
                    managersList = null;
                }
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, managersList);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/GetAttendance")]
        public HttpResponseMessage GetAttendance()
        {
            var identity = (ClaimsIdentity)User.Identity;
            Requestor r = new Requestor(identity);
            using (PsiogEntities PE = new PsiogEntities())
            {
                var attendance = (from emp in PE.AttendanceTransactions
                                  where emp.EmployeeId == r.employeeId
                                  select emp).ToList();
                if(attendance.Count==0)
                {
                    attendance = null;
                }
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, attendance);
                return response;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Values/ApproveLeave")]
        public HttpResponseMessage ApproveLeave()
        {
            var identity = (ClaimsIdentity)User.Identity;
            Requestor r = new Requestor(identity);
            using (PsiogEntities PE = new PsiogEntities())
            {
                var approval = (from la in PE.LeaveApplications
                                join emp in PE.Employees
                                on la.EmployeeId equals emp.EmployeeId
                                join type in PE.LeaveTypes
                                on la.LeaveId equals type.LeaveTypeId
                                where la.SendTo == r.name && la.Status == "Applied"
                                select new
                                {
                                    emp.Name,
                                    type.Type,
                                    la.FromDate,
                                    la.ToDate,
                                    la.FromSession,
                                    la.ToSession,
                                    la.Reason,
                                    la.LeaveApplicationId

                                }
                                ).ToList();
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, approval);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/changeStatus")]
        public HttpResponseMessage ChangeStatus(Approveleaves l)
        {
            using (PsiogEntities PE = new PsiogEntities())
            {
                var appl = PE.LeaveApplications.Where(u => u.LeaveApplicationId == l.id).FirstOrDefault();
                appl.Status = l.status;
                string msg = "";

                if (l.status == "Rejected")
                {
                    var rollback = (from avail in PE.EmployeeLeaveAvaliabilities
                                    join leaveapp in PE.LeaveApplications
                                    on avail.LeaveTypeId equals leaveapp.LeaveId
                                    where leaveapp.LeaveApplicationId == l.id
                                    select new
                                    {
                                        avail.AllocatedDays,
                                        leaveapp.FromDate,
                                        leaveapp.ToDate,
                                        leaveapp.FromSession,
                                        leaveapp.ToSession,
                                        avail.Id

                                    }).FirstOrDefault();
                    int rollbackdays = (rollback.ToDate - rollback.FromDate).Days;
                    decimal rollbacksession = 0M;
                    if (rollback.FromSession == rollback.ToSession)
                    {
                        rollbacksession = 0.5M;
                    }

                    else if (rollback.FromSession > rollback.ToSession)
                    {
                        rollbacksession = 0M;
                    }
                    else
                    {
                        rollbacksession = 1M;
                    }

                    decimal updatedRollback = rollbackdays + rollbacksession;
                    var updateLeaveAvailability = PE.EmployeeLeaveAvaliabilities.Where(e => e.Id == rollback.Id).FirstOrDefault();
                    updateLeaveAvailability.AvailedDays = updatedRollback;

                }
                else if (l.status == "Approved")
                {
                    appl.Status = "Approved";
                }
                try
                {
                    PE.SaveChanges();
                    msg = "Done!";
                }
                catch (Exception E)
                {
                    ExceptionLog.Logger(E);
                    msg = "Unable to process request";
                }
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, msg);
                return response;
            }



        }

        [HttpPost]
        [Route("api/Values/securityQuestion")]
        public HttpResponseMessage SecurityQuestion()
        {

            using (PsiogEntities PE = new PsiogEntities())
            {
                var questions = (from q in PE.SecurityQuestions select q.Question).ToList();

                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, questions);
                return response;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Values/TrackLeave")]
        public HttpResponseMessage TrackLeave()
        {
            var identity = (ClaimsIdentity)User.Identity;
            Requestor r = new Requestor(identity);
            using (PsiogEntities PE = new PsiogEntities())
            {
                var myLeaves = (from la in PE.LeaveApplications
                                join l in PE.LeaveTypes
                                on la.LeaveId equals l.LeaveTypeId
                                where la.EmployeeId == r.employeeId
                                select new
                                {
                                    l.Type,
                                    la.FromDate,
                                    la.ToDate,
                                    la.FromSession,
                                    la.ToSession,
                                    la.Reason,
                                    la.Status
                                }
                                ).ToList();
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, myLeaves);
                return response;
            }
        }
        [HttpGet]
        [Route("api/Values/getMySecurityQuestion/{id=id}")]
        public HttpResponseMessage getMySecurityQuestion(string id)
        {
            string question;
            using (PsiogEntities PE = new PsiogEntities())
            {
                question = (from q in PE.Users where q.EmployeeId == id select q.SecurityQuestion).FirstOrDefault();
                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK, question);
                return response;
            }
        }

    }
}
