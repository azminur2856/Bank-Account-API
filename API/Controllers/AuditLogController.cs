using API.Auth;
using BLL.Services;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Services.Description;

namespace API.Controllers
{
    [RoutePrefix("api/auditlogs")]
    public class AuditLogController : ApiController
    {
        [Logged]
        [Admin]
        [HttpGet]
        [Route("all")]
        public HttpResponseMessage GetAllLogs()
        {
            try
            {
                var logs = AuditLogService.Get();
                if (logs == null || !logs.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Message = "No audit logs foud."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, logs);
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

        [HttpGet]
        [Route("bytype/{type}")]
        public HttpResponseMessage GetLogsByType(string type)
        {
            try
            {
                var result = AuditLogService.GetByType(type);
                if (!result.isValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        Message = "Invalid audit log type provided.",
                        ValidTypes = result.valueTypes
                    });
                }
                if (result.logs == null || !result.logs.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Message = $"No audit logs found for type '{type}'."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, result.logs);
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

        [HttpGet]
        [Route("bydaterange/{startDate}/{endDate}")]
        public HttpResponseMessage GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        Message = "Start date must be earlier than or equal to end date."
                    });
                }
                var logs = AuditLogService.GetByDateRange(startDate, endDate);
                if (logs == null || !logs.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Message = $"No audit logs found between {startDate} and {endDate}."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, logs);
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

        public class EmailRequest
        {
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("byuseremail")]
        public HttpResponseMessage GetLogsByUserEmail(EmailRequest email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email.Email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        Message = "Email must be provided."
                    });
                }
                var logs = AuditLogService.GetByUserEmail(email.Email);
                if (logs == null || !logs.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Message = $"No audit logs found for user with email '{email.Email}'."
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, logs);
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
