using Newtonsoft.Json;
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
    public class LeaveController : ApiController
    {
     
        [HttpPost]
        [Route("api/Leave/ApplyLeave")]
        public IHttpActionResult ApplyLeave(LeaveApplication leave)
        {
            var identity = (ClaimsIdentity)User.Identity;
            Requestor r = new Requestor(identity);
            string message = DBOperations.ApplyLeave(leave,r.employeeId);
            return Ok(message);
        }


       


    }
}
