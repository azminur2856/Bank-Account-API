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
    [RoutePrefix("api/transaction")]
    public class TransactionController : ApiController
    {
        [Logged]
        [HttpPost]
        [Route("deposit")]
        [Role("Employee")]
        public HttpResponseMessage Deposit(DepositDTO depositDTO)
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
                var result = TransactionService.CreateDeposit(header.ToString(), depositDTO);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Deposit completed successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to complete deposit.");
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
        [HttpPost]
        [Route("withdrawal")]
        [Role("Employee", "Customer")]
        public HttpResponseMessage Withdrawal(WithdrawalDTO withdrawalDTO)
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
                var result = TransactionService.CreateWithdrawal(header.ToString(), withdrawalDTO);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Withdrawal completed successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to complete withdrawal.");
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
        [HttpPost]
        [Route("transfer")]
        [Role("Employee", "Customer")]
        public HttpResponseMessage Transfer(TransferDTO transferDTO)
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
                var result = TransactionService.CreateTransfer(header.ToString(), transferDTO);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Transaction completed successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to complete transaction.");
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
        [HttpPost]
        [Route("system/credit")]
        [Role("Admin")]
        public HttpResponseMessage SystemCredit(SystemCreditDebitDTO data)
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

            if (data.Amount <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new 
                { 
                    Message = "Amount must be a positive value."
                });
            }
            try
            {
                var header = Request.Headers.Authorization;
                var result = TransactionService.CreateSystemCredit(header.ToString(), data.Amount);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "System credit completed successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to complete system credit.");
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
        [HttpPost]
        [Route("statement")]
        public HttpResponseMessage GetAccountStatement(StatementRequestDTO statementRequest)
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
                var token = Request.Headers.Authorization;
                var statementData = StatementService.GetAccountStatementData(token.ToString(), statementRequest);

                if (statementData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to retrieve statement data.");
                }
                return Request.CreateResponse(HttpStatusCode.OK, statementData);

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
    }
}