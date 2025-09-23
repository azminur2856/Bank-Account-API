using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace API.Auth
{
    public class Admin : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var header = actionContext.Request.Headers.Authorization;
            if (header == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Message = "Authorization token is missing from the 'Authorization' header.",
                    Suggestion = "Please provide the token in the 'Authorization' header."
                });
            }
            else if (!AuthService.IsAdmin(header.ToString())) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "You do not have permission to access this resource.",
                    Suggetion = "Please contact the administrator if you believe this is an error."
                });
            }
            base.OnAuthorization(actionContext);
        }
    }
}