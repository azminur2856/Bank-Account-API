using API.Auth;
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
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login(LoginDTO log)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Login request validation failed. Please check the input fields.",
                    Errors = errors
                });
            }
            try
            {
                var result = AuthService.Authenticate(log);

                if(result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Message = "Login successful.",
                        Token = result
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Message = "Login failed.",
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Message = ex.Message
                });
            }
            catch(UnauthorizedAccessException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An error occurred during login.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpGet]
        [Route("logout")]
        public HttpResponseMessage Logout()
        {
            try
            {
                var header = Request.Headers.Authorization;

                bool result = AuthService.Logout(header.ToString());
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Message = "Logout successful."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Logout failed. Invalid token."
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An error occurred during logout.",
                    Error = ex.Message
                });
            }
        }
    }
}
