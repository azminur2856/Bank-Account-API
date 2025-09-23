using BLL;
using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("create")]
        public async Task<HttpResponseMessage> Create(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Validation failed.",
                    Errors = errors
                });
            }

            try
            {
                var result = await UserService.Create(user);
                
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "User created successfully. Please check your email to verify your account.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "User creation failed.");
            }
            catch(InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An error occurred while creating the user.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("verifyaccount/{token}")]
        public HttpResponseMessage VerifyEmail(string token)
        {
            try
            {
                var result = VerificationService.VerifyEmail(token);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Email verified successfully. You can now log in.");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid or expired verification link.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An error occurred while verifying the email.",
                    Error = ex.Message
                });
            }
        }
    }
}
