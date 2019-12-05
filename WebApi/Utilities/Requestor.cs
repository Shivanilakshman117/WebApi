using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace WebApi.Utilities
{
    public class Requestor : ApiController

    {
        public string name { get; set; }
        public string employeeId { get; set; }
        public string role { get; set; }
        public Requestor(ClaimsIdentity identity)
        {
            
            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
            this.name = identity.Name;
            this.employeeId = sid;

        }
    }
}