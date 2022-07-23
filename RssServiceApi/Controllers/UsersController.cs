using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
using RssServiceApi.Exceptions;
using RssServiceApi.RequestModels;
using RssServiceApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RssServiceApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        protected IConfiguration _configuration;
        protected RssDbContext _dbCtx;
        private UserServices _userServices; 

        public UsersController(IConfiguration conf, RssDbContext dbCtx)
            :base()
        {
            _configuration = conf;
            _dbCtx = dbCtx;
            _userServices = new UserServices(_dbCtx, _configuration);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userServices.GetUsers();

            return Ok(users);
        }

        [HttpGet]
        [Route("me")]
        public ActionResult<UserDetailsDto> GetAuthenticatedUser()
        {
            int userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

            var user = _userServices.GetUserDetailsById(userId);

            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            return Ok(user);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetUser(int id)
        {
            var user = _userServices.GetUserDetailsById(id);

            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegisterUser([FromBody]RegistrationCredentials credentials)
        {
            if (!Request.Headers.ContainsKey("AppKey"))
            {
                return BadRequest("AppKey header is missing");
            }

            if (Request.Headers["AppKey"] != _configuration["AppKey"])
            {
                return BadRequest("AppKey header contains wrong value");
            }

            try
            {
                _userServices.RegisterUser(credentials);
            }
            catch (UserAlreadyExistsException e)
            {
                return Conflict("User already exists");
            }
            

            UserDetailsDto? user = _userServices.GetUserDetailsByEmail(credentials.Email);
            user.Links.Add(new LinkDto()
            {
                Rel = "login",
                Method = "post",
                Href = "/login",
            });

            return Created($"/users/{user.Id}", user);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult EditUser([FromRoute]int id, [FromBody]EditUser editUser)
        {
            try
            {
                _userServices.EditUser(id, editUser);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            

            var user = _userServices.GetUserDetailsById(id);

            return Ok(user);
        }

        [HttpPut("me")]
        public IActionResult EditAuthenticatedUser([FromBody]EditUser editUser)
        {
            int userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

            try
            {
                _userServices.EditUser(userId, editUser);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            

            var user = _userServices.GetUserDetailsById(userId);

            return Ok(user);
        }

        [AllowAnonymous]
        [Route("/login")]
        [HttpPost]
        public IActionResult AuthenticateUser([FromBody] LoginCredentials loginCredentials)
        {
            string? jwtToken = _userServices.AuthenticateUser(loginCredentials);

            if (_configuration.GetValue<bool>("Email:EmailVerificationEnabled") && 
                !_userServices.IsVerified(loginCredentials.Email))
            {
                return Unauthorized($"Email {loginCredentials.Email} is not verified.");
            }

            if (jwtToken == null)
            {
                return Unauthorized();
            }

            return Ok(new UserTokenDto()
            {
                TokenType = "JWT",
                Token = jwtToken,
            });
        }
    }
}
