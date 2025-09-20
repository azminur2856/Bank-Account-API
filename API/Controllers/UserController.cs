using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("create")]
        public HttpResponseMessage Create(UserDTO user)
        {
            if (user == null) {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Please provide user details for registration.");
            }
            try
            {
                var res = UserService.Create(user);
                if (res) return Request.CreateResponse(HttpStatusCode.Created, "A OTP code send to your email. Please use it for user activation. Thank you." );
                return Request.CreateResponse(HttpStatusCode.GatewayTimeout, "Please try again!");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Error = ex.Message
                });
            }
        }
    }
}
