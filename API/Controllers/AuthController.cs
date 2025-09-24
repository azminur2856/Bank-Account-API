using API.Auth;
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

                if (result != null)
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
            catch (UnauthorizedAccessException ex)
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

        [HttpPost]
        [Route("requestpasswordreset")]
        public async Task<HttpResponseMessage> RequestPasswordReset(PasswordResetRequestDTO prrd)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Password reset request validation failed. Please check the input fields.",
                    Errors = errors
                });
            }

            try
            {
                bool result = await VerificationService.SendPasswordResetOTP(prrd);

                var type = string.IsNullOrWhiteSpace(prrd.Type) ? "Email" : prrd.Type.Trim();
                type = type.ToLowerInvariant();

                if (result && type == "email")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Message = "Password reset OTP has been sent to your email."
                    });
                }
                else if (result && type == "sms")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Message = "Password reset OTP has been sent to your phone."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Failed to send OTP. Please try again."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Message = ex.Message
                });
            }
            catch (ArgumentException ex)
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
                    Message = "An error occurred while requesting password reset OTP.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("resetpassword")]
        public HttpResponseMessage ResetPassword(PasswordResetDTO prd)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Password reset validation failed. Please check the input fields.",
                    Errors = errors
                });
            }

            try
            {
                bool result = VerificationService.ResetPassword(prd);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Message = "Password has been reset successfully."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = "Failed to reset password. Please try again."
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An error occurred while resetting the password.",
                    Error = ex.Message
                });
            }
        }
    }
}
