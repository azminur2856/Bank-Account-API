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
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Please provide user details for registration.");
            }
            try
            {
                var res = await UserService.Create(user);
                if (res) return Request.CreateResponse(HttpStatusCode.Created, "A OTP code send to your email. Please use it for user activation. Thank you.");
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

        public class SmsRequest
        {
            public string Number { get; set; }
            public string Message { get; set; }
        }

        [HttpPost]
        [Route("sms")]
        public async Task<HttpResponseMessage> SendSMS(SmsRequest sms)
        {
            try
            {
                var res = await ServiceFactory.SmsService.SendSMSAsync(sms.Number, sms.Message);
                if (res) return Request.CreateResponse(HttpStatusCode.OK, "SMS sent successfully.");
                return Request.CreateResponse(HttpStatusCode.GatewayTimeout, "Failed to send SMS. Please try again!");
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
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        [HttpPost]
        [Route("email")]
        public HttpResponseMessage SendEmail(EmailRequest e)
        {
            try
            {
                ServiceFactory.EmailService.SendEmail(e.Email, e.Subject, e.Body);
                return Request.CreateResponse(HttpStatusCode.OK, "SMS sent successfully.");

            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Error = ex.Message
                });
            }
        }
    }
}
