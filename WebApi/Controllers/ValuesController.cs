using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
        [HttpGet]
        [Route("api/Values/GetEmployees/{name}")]
        public HttpResponseMessage GetEmployees(string name)
        {
            PsiogEntities PE = new PsiogEntities();

            var employeesList = (from emp in PE.Employees
                                 where emp.Name.Contains(name)
                                 select emp.Name).ToList();

            HttpResponseMessage response;

            response = Request.CreateResponse(HttpStatusCode.OK, employeesList);
            return response;
        }

    }
}
