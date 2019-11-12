using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpPost]
        [Route("api/Values/leavebalance")]
        public IHttpActionResult Leavebalance(LeaveApplication leave)
        {

            var balance = DBOperations.CheckBalance(leave);
            return Ok(balance);
        }
        [HttpPost]
        [Route("api/Values/GetSearchedEmployees")]
        public HttpResponseMessage GetSearchedEmployees(Searcher search)
        {
            List<string> employeesList;
            PsiogEntities PE = new PsiogEntities();
            string name = search.employeeName;
            employeesList = (from emp in PE.Employees
                             where emp.Name.Contains(search.employeeName)
                             select emp.Name).ToList();
            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, employeesList);
            return response;
        }


        [HttpPost]
        [Route("api/Values/GetAllEmployees")]
        public HttpResponseMessage GetEmployees()
        {
            List<string> employeesList;
            PsiogEntities PE = new PsiogEntities();
            employeesList = (from emp in PE.Employees
                             select emp.Name).ToList();
            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, employeesList);
            return response;
        }
        [HttpPost]
        [Route("api/Values/GetReportingAuthorities")]
        public HttpResponseMessage GetReportingAuthorities(LoggedUser loggedUser)
        {
            List<string> authoritiesList;
            PsiogEntities PE = new PsiogEntities();
            authoritiesList = (from emp in PE.Employees
                               join rep in PE.ReportingAuthorities
                               on emp.EmployeeId equals rep.ManagerId

                               select emp.Name).ToList();

            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, authoritiesList);
            return response;
        }

        [HttpPost]
        [Route("api/Values/GetHolidaysList")]
        public HttpResponseMessage GetHolidaysList()
        {
            
           
            PsiogEntities PE = new PsiogEntities();
    
            var holidaysList = (from hols in PE.Holidays
                                orderby hols.Date
                                select hols);
           
            //var json = new JavaScriptSerializer().Serialize(holidaysList);
            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, holidaysList);
            return response;
        }

        [HttpPost]
        [Route("api/Values/GetManagersList")]
        public HttpResponseMessage GetManagersList()
        {
            PsiogEntities PE = new PsiogEntities();
           
            var managersList = (from emp in PE.Employees
                               join managers in PE.Managers
                               on emp.EmployeeId equals managers.ManagerId
                               select emp.Name).ToList();
    
            HttpResponseMessage response;
            response = Request.CreateResponse(HttpStatusCode.OK, managersList);
            return response;
        }

    }
}
