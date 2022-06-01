using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
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

        [HttpPost]
        public ActionResult<UserDetailsDto> RegisterUser([FromBody]RegistrationCredentials credentials)
        {
            if (!Request.Headers.ContainsKey("AppKey"))
            {
                return BadRequest();
            }

            if (Request.Headers["AppKey"] != _configuration["AppKey"])
            {
                return BadRequest();
            }

            UserServices userServices = new UserServices(_dbCtx);

            try
            {
                userServices.RegisterUser(credentials);
            }
            catch (DbUpdateException e)
            {
                return BadRequest();
            }
            

            UserDetailsDto user = userServices.GetUserDetailsByEmail(credentials.Email);

            return user;
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
