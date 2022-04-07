using AutoMapper;
using diplomChat.Authorization;
using diplomChat.Entities;
using diplomChat.Helpers;
using diplomChat.Models.Users;
using diplomChat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace diplomChat.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
   private IUserService _userService;
   private IMapper _mapper;
   private readonly AppSettings _appSettings;

   public UsersController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
   {
      _userService = userService;
      _mapper = mapper;
      _appSettings = appSettings.Value;
   }

   [AllowAnonymous]
   [HttpPost("[action]")]
   public IActionResult Authenticate(AuthenticateRequest request)
   {
      var response = _userService.Authenticate(request);
      return Ok(response);
   }
   
   [AllowAnonymous]
   [HttpPost("register")]
   public IActionResult Register(RegisterRequest model)
   {
      _userService.Register(model);
      return Ok(new { message = "Registration successful" });
   }

   [Authorize(Role.Admin)]
   [HttpGet]
   public IActionResult GetAll()
   {
      var users = _userService.GetAll();
      return Ok(users);
   }

   [HttpGet("{id:int}")]
   public IActionResult GetById(int id)
   {
      var currentUser = (User) HttpContext.Items["User"]!;
      if (id != currentUser.Id && currentUser.Role != Role.Admin)
         return Unauthorized(new { message = "Unauthorized" });

      var user = _userService.GetById(id);
      return Ok(user);
   }
   
   [HttpPut("{id:int}")]
   public IActionResult Update(int id, UpdateRequest model)
   {
      _userService.Update(id, model);
      return Ok(new { message = "User updated successfully" });
   }
   
   [Authorize(Role.Admin)]
   [HttpDelete("{id:int}")]
   public IActionResult Delete(int id)
   {
      _userService.Delete(id);
      return Ok(new { message = "User deleted successfully" });
   }
}