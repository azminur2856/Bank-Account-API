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
        [HttpPost]
        [Route("create")]
        public HttpResponseMessage Create(AccountDTO account)
        {
            if (account != null)
            {
                var data = AccountService.Create(account);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid account data");
        }
    }
}
