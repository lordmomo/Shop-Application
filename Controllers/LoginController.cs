using DemoWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using DemoWebApplication.Utils;
using DemoWebApplication.Service.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using DemoWebApplication.Constants;

using Serilog;
using DemoWebApplication.Service.ServiceImplementation;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using AutoMapper;

namespace DemoWebApplication.Controllers;

[AllowAnonymous]
public class LoginController : Controller
{
    public ApplicationDbContext db;
    private readonly SignInManager<Person> signInManager;
    private readonly UserManager<Person> _userManager;
    public IUserInterface userIntefaceImpl;
    public IConfiguration configuration;
    public AuthServiceImpl authServiceImpl;
    public IMapper mapper;
    public LoginController(ApplicationDbContext db, SignInManager<Person> signInManager,
        UserManager<Person> userManager, IConfiguration configuration, IUserInterface userIntefaceImpl,
        AuthServiceImpl authServiceImpl, IMapper mapper)
    {
        this.db = db;
        this.signInManager = signInManager;
        this._userManager = userManager;
        this.configuration = configuration;
        this.authServiceImpl = authServiceImpl;
        this.userIntefaceImpl = userIntefaceImpl;
        this.mapper = mapper;
    }


    [Route(Urls.loginApi)]
    [HttpPost]
    public async Task<IActionResult> Index([FromBody]Login loginDetails)
    {
       
            var result = await authServiceImpl.CheckUserCredentials(loginDetails);
            if (!result)
            {
                return StatusCode(400, new { message = Message.InvalidCredentialsMessage });

            }
            if (result == true)
            {
                PersonDto? user = userIntefaceImpl.GetUserByUsername(loginDetails.Username);

                if (user == null)
                {
                    return StatusCode(404, new { message = Message.UserNotFound });
                }
                
                var jwtToken = await authServiceImpl.GenerateTokenString(mapper.Map<Person>(user));
               
                Log.Debug($"Jwt token: {jwtToken}");

                var userRole = await userIntefaceImpl.getRoleofUser(user.Id);

                return Json(new { username = user.Username, token = jwtToken ,role = userRole});

            }
            return StatusCode(500, new { message = Message.InternalServerError });
    }

}
