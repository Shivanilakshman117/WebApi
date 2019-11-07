using Newtonsoft.Json;
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
    public class LeaveController : ApiController
    {
        [HttpPost]
        [Route("api/Leave/ApplyLeave")]
        public IHttpActionResult ApplyLeave(LeaveApplication leave)
        {
           
            string message = DBOperations.ApplyLeave(leave);
            return Ok(message);
        }

       
    }
}
