using Backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Middlewares;

public class AuthenticateAdminTokenMiddleware : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                var (exists, valid) = JWT.IsValid(token);
                if (exists && valid)
                {
                    context.HttpContext.Items["userId"] = JWT.GetId(token);
                    await next();
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        } catch (Exception e)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}