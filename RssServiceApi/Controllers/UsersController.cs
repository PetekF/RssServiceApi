using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
using RssServiceApi.Exceptions;
using RssServiceApi.RequestModels;
using RssServiceApi.Services;

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
            return Ok();
        }

        [HttpGet("{id:int}")]
        public IActionResult GetUser(int id)
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegisterUser([FromBody]RegistrationCredentials credentials)
        {
            if (!Request.Headers.ContainsKey("AppKey"))
            {
                return BadRequest();
            }

            if (Request.Headers["AppKey"] != _configuration["AppKey"])
            {
                return BadRequest();
            }

            try
            {
                _userServices.RegisterUser(credentials);
            }
            catch (UserAlreadyExistsException e)
            {
                return Conflict();
            }
            

            UserDetailsDto user = _userServices.GetUserDetailsByEmail(credentials.Email);
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
        public ActionResult<UserDetailsDto> EditUser(int id)
        {
            return Ok(new UserDetailsDto());
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
