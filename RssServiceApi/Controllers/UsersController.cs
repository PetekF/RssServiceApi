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

        public UsersController(IConfiguration conf, RssDbContext dbCtx)
            :base()
        {
            _configuration = conf;
            _dbCtx = dbCtx;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var userServices = new UserServices(_dbCtx, _configuration);
            var users = userServices.GetUsers();

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

            UserServices userServices = new UserServices(_dbCtx, _configuration);

            try
            {
                userServices.RegisterUser(credentials);
            }
            catch (UserAlreadyExistsException e)
            {
                // Change this to return error DTO!
                ErrorDto errorDto = new ErrorDto()
                {
                    ErrorCode = 101,
                    ErrorName = "User exists",
                    Message = e.Message,
                };

                return Conflict(errorDto);
            }
            

            UserDetailsDto user = userServices.GetUserDetailsByEmail(credentials.Email);
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
            UserServices userServices = new UserServices(_dbCtx, _configuration);
            string? jwtToken = userServices.AuthenticateUser(loginCredentials);

            if (_configuration.GetValue<bool>("Email:EmailVerificationEnabled") && 
                !userServices.IsVerified(loginCredentials.Email))
            {
                ErrorDto errorDto = new ErrorDto()
                {
                    ErrorCode = 103,
                    ErrorName = "Not verified",
                    Message = $"Email address {loginCredentials.Email} is not verified"
                };

                return Unauthorized(errorDto);
            }

            if (jwtToken == null)
            {
                ErrorDto errorDto = new ErrorDto()
                {
                    ErrorCode = 102,
                    ErrorName = "Login refused",
                    Message = "Wrong email or password",
                };

                return Unauthorized(errorDto);
            }

            return Ok(new UserTokenDto()
            {
                TokenType = "JWT",
                Token = jwtToken,
            });
        }
    }
}
