using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

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

        [Authorize(Roles ="admin")]
        [HttpPost]
        [Route("api/login/authorize")]
        public IHttpActionResult GetAuthorization()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return Ok("Authorized Access:" + identity.Name + "Role:" + string.Join(",", roles.ToList()));

        }
    }
}
