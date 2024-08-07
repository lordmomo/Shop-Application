using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceImplementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;


namespace DemoWebApplication.Middleware;

public class JwtAuthMiddleware 
{
    private readonly RequestDelegate _next;
    private readonly IAuthorizationService _authorizationService;

    public JwtAuthMiddleware(RequestDelegate next,IAuthorizationService authorizationService)
    {
        _next = next;
        _authorizationService = authorizationService;
    }

    public async Task Invoke(HttpContext context, AuthServiceImpl authServiceImpl)
    {
        Console.WriteLine(context.User.Identity.Name);
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                var principal = authServiceImpl.ValidateToken(token);

                //if(principal != null && principal.Identity is ClaimsIdentity claimsIdentity)
                //{
                //    var expirationClaim = claimsIdentity.FindFirst(ClaimTypes.Expiration);
                //    if (expirationClaim != null && DateTime.TryParse(expirationClaim.Value, out var expiration))
                //    {
                //        if (expiration <= DateTime.UtcNow)
                //        {
                //            //context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //            authServiceImpl.GenerateNewToken(context.User.Identity.Name, context);

                //            Console.WriteLine("token expired");
                //            return;
                //        }
                //    }
                //}
                if (principal != null)
                {
                    var usernameClaims = principal?.FindFirst("username")?.Value;
                    if (usernameClaims != context.Request.Cookies["usernameCookie"])
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                    //var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                    var authorizationResult = await _authorizationService.AuthorizeAsync(principal, null, new RoleRequirement());
                    if (authorizationResult.Succeeded)
                    {
                        // User is authorized, proceed with the request
                        await _next(context);
                        //return;
                    }
                }
                
            }
            //else if (string.IsNullOrWhiteSpace(context.User.Identity.Name) && context.Request.Path.Value == "/register")
            //{
            //    await _next(context);
            //}
            else if (string.IsNullOrWhiteSpace(context.User.Identity.Name) && context.Request.Path.Value != "/login")
            {
                if(context.Request.Path.Value == "/register")
                {
                    await _next(context);
                }
                context.Response.StatusCode = (int)HttpStatusCode.Redirect;
                context.Response.Redirect("/login");
            }
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 
            return;
        }

        await _next(context);
    }
}
