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
        public List<UserDto> GetUsers()
        {
            return new List<UserDto>();
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

        [Route("/login")]
        [HttpPost]
        public ActionResult<UserTokenDto> AuthenticateUser([FromBody] LoginCredentials loginCredentials)
        {
            UserServices userServices = new UserServices(_dbCtx, _configuration);
            string? jwtToken = userServices.AuthenticateUser(loginCredentials);

            return Ok(new UserTokenDto() { Id = 1, Token = "sometopsecrettoken"});
        }
    }
}
