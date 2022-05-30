using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RssServiceApi.DTOs;

namespace RssServiceApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
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
        public ActionResult<UserDetailsDto> RegisterUser()
        {
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
