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
                    return Request.CreateResponse(HttpStatusCode.OK, "Your account activated and email verified successfully. You can now log in.");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid or expired verification link.");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An error occurred while activating account.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpPost]
        [Route("sendemailverificationtoken")]
        public async Task<HttpResponseMessage> SendEmailVerificationToken()
        {
            try
            {
                var header = Request.Headers.Authorization;
                var result = await VerificationService.SendEmailVerificationToken(header.ToString());

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Email verification link sent successfully. Please check your email inbox.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to send email verification link.");
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
                    Message = "An error occurred while sending email verification link.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("verifyemail/{token}")]
        public HttpResponseMessage VerifyEmail(string token)
        {
            try
            {
                var result = VerificationService.VerifyEmail(token);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Email verified successfully.");
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
                    Message = "An error occurred while sending phone verification otp.",
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

        [Logged]
        [HttpPost]
        [Route("address/add")]
        public HttpResponseMessage AddAddress(AddressAddDTO addressAddDTO)
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
                var result = AddressService.AddAddress(header.ToString(), addressAddDTO);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Address added successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to add address.");
            }
            catch (KeyNotFoundException ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
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
                    Message = "An unexpected error occurred.", 
                    Error = ex.Message 
                });
            }
        }

        [Logged]
        [HttpPost]
        [Route("address/update/present")]
        public HttpResponseMessage UpdatePresentAddress(AddressDTO addressDTO)
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
                var result = AddressService.UpdatePresentAddress(header.ToString(), addressDTO);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Present address updated successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to update address.");
            }
            catch (KeyNotFoundException ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new 
                { 
                    Message = ex.Message 
                });
            }
            catch (InvalidOperationException ex)
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
                    Message = "An unexpected error occurred.", 
                    Error = ex.Message 
                });
            }
        }

        [Logged]
        [HttpGet]
        [Route("address/all")]
        public HttpResponseMessage GetAllAddresses()
        {
            try
            {
                var header = Request.Headers.Authorization;
                var addresses = AddressService.GetUserAddresses(header.ToString());
                if (addresses == null || !addresses.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new 
                    { 
                        Message = "No addresses found for this user." 
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, addresses);
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

        [Logged]
        [Role("Admin", "Employee")]
        [HttpPost]
        [Route("address/verify/{addressId}")]
        public HttpResponseMessage VerifyAddress(int addressId)
        {
            try
            {
                var header = Request.Headers.Authorization;
                var result = AddressService.VerifyAddress(header.ToString(), addressId);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Address verified successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed to verify address. Address might already be verified or an error occurred.");
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
                    Message = "An unexpected error occurred.", 
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("change-role")]
        [Role("Admin")]
        public HttpResponseMessage ChangeUserRole(RoleChangeDTO roleChange)
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
                var result = UserService.ChangeUserRole(header.ToString(), roleChange);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "User role changed and notification email sent successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to change user role.");
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
            catch (InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new 
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
                    Message = "An unexpected error occurred.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpPost]
        [Route("change-status")]
        [Role("Admin", "Employee")]
        public HttpResponseMessage ChangeUserStatus(UserStatusChangeDTO statusChangeDTO)
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
                var result = UserService.ChangeUserStatus(header.ToString(), statusChangeDTO);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "User status changed successfully and notification email sent.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to change user status.");
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
            catch (InvalidOperationException ex)
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
                    Message = "An unexpected error occurred.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpGet]
        [Route("account/all")]
        public HttpResponseMessage GetAllUserAccount()
        {
            try
            {
                var header = Request.Headers.Authorization;
                var accounts = AccountService.GetUserAccounts(header.ToString());
                if (accounts == null || !accounts.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Message = "No account found for this user."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, accounts);
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
