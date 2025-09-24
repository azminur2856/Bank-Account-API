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
    public class Role : AuthorizationFilterAttribute
    {
        private readonly string[] allowedRoles;

        public Role(params string[] roles)
        {
            allowedRoles = roles;
        }

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

            var userRole = AuthService.GetUserRoleByToken(header.ToString());

            if(userRole == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Message = "Authorization token is invalid or expired.",
                    Suggestion = "Please provide a valid token in the 'Authorization' header."
                });
            }
            else if (!allowedRoles.Contains(userRole))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "You do not have permission to access this resource.",
                    Suggetion = "Your role is not authorized for this operation."
                });
            }
            base.OnAuthorization(actionContext);
        }
    }
}