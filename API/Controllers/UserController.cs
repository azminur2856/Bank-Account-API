using API.Auth;
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
        public HttpResponseMessage VerifyAccount(string token)
        {
            try
            {
                var result = VerificationService.VerifyAccount(token);
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
        // TODO: sendemailverificationtoken
        //[HttpGet]
        //[Route("verifyemail/{token}")]
        //public HttpResponseMessage VerifyEmail(string token)
        //{
        //    try
        //    {
        //        var result = VerificationService.VerifyEmail(token);
        //        if (result)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, "Email verified successfully. You can now log in.");
        //        }
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid or expired verification link.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new
        //        {
        //            Message = "An error occurred while verifying the email.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        [Logged]
        [HttpPost]
        [Route("changepassword")]
        public HttpResponseMessage ChangePassword(PasswordChangeDTO pc)
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
                var header = Request.Headers.Authorization;

                var result = UserService.ChangePassword(header.ToString(), pc);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Password changed successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Old password ");
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
                    Message = "An error occurred while changing the password.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpPost]
        [Route("sendphoneverificationotp")]
        public async Task<HttpResponseMessage> SendPhoneVerificationOtp()
        {
            try
            {
                var header = Request.Headers.Authorization;
                var result = await VerificationService.SendPhoneVerificationOtp(header.ToString());

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Phone verification OTP sent successfully. Please check your phone.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to send phone verification OTP.");
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
                    Message = "An error occurred while changing the password.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpPost]
        [Route("verifyphone")]
        public HttpResponseMessage VerifyPhone(OtpDTO o)
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
                var header = Request.Headers.Authorization;
                var result = VerificationService.VerifyPhoneNumber(header.ToString(), o.Otp);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Phone number verified successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid or expired OTP.");
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
                    Message = "An error occurred while verifying the phone number.",
                    Error = ex.Message
                });
            }
        }
    }
}
