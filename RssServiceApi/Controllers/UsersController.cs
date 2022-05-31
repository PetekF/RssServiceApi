using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
using RssServiceApi.RequestModels;

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

        [HttpPost]
        public ActionResult<UserDetailsDto> RegisterUser([FromBody]RegistrationCredentials credentials)
        {
            // Check header for AppKey and make sure it matches the one in appsettings
            // If request doesn't have that header return some error code
            if (!Request.Headers.ContainsKey("AppKey"))
            {
                return BadRequest();
            }

            if (Request.Headers["AppKey"] != _configuration["AppKey"])
            {
                return BadRequest();
            }

            // Save data from body to database
            _dbCtx.Users.Add(new User() { 
                Email = credentials.Email,
                Password = credentials.Password,
                FirstName = credentials.FirstName,
                LastName = credentials.LastName
            });

            _dbCtx.SaveChanges();

            return new UserDetailsDto();
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<UserDetailsDto> EditUser()
        {
            return Ok(new UserDetailsDto());
        }

        [Route("/login")]
        [HttpPost]
        public ActionResult<UserTokenDto> AuthenticateUser()
        {
            return Ok(new UserTokenDto() { Id = 1, Token = "sometopsecrettoken"});
        }
    }
}
