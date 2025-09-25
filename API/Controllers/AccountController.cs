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
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        [Logged]
        [HttpPost]
        [Route("create")]
        [Role("Admin", "Employee", "Customer")]
        public HttpResponseMessage Create(AccountCreateDTO accountDto)
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
                var data = AccountService.Create(header.ToString(), accountDto);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Account creation failed.");
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new 
                { 
                    Message = ex.Message 
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
                    Message = "An unexpected error occurred.",
                    Error = ex.Message
                });
            }
        }

        [Logged]
        [HttpPost]
        [Route("activate/{accountNumber}")]
        [Role("Employee")]
        public HttpResponseMessage ActivateAccount(string accountNumber)
        {
            try
            {
                var header = Request.Headers.Authorization;
                var result = AccountService.ActivateAccount(header.ToString(), accountNumber);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Account activated successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed to activate account.");
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

        [Logged]
        [HttpGet]
        [Route("all")]
        [Role("Admin", "Employee")]
        public HttpResponseMessage GetAllAccounts()
        {
            try
            {
                var header = Request.Headers.Authorization;
                var accounts = AccountService.GetAccounts(header.ToString());
                if (accounts == null || !accounts.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Message = "No accounts found." });
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
