using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
using RssServiceApi.RequestModels;

namespace RssServiceApi.Services
{
    public class UserServices
    {
        protected RssDbContext _dbCtx;

        public UserServices(RssDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        public void RegisterUser(RegistrationCredentials credentials)
        {
            _dbCtx.Users.Add(new User()
            {
                Email = credentials.Email,
                Password = credentials.Password,
                FirstName = credentials.FirstName,
                LastName = credentials.LastName,
                CreatedAt = DateTime.Now,
                EmailConfirmationKey = GenerateEmailConfirmationKey()
            });

            try
            {
                _dbCtx.SaveChanges();
            }
            catch(DbUpdateException e)
            {
                throw e;
            }
        }

        public UserDetailsDto GetUserDetailsByEmail(string email)
        {
            UserDetailsDto user = _dbCtx.Users.Select(u => new UserDetailsDto()
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName ?? "N.N", // Maybe put N.N as default value in Database for FirstName and LastName
                LastName = u.LastName ?? "N.N",
                Links = new List<LinkDto>()
                {
                    new LinkDto()
                    {
                        Rel = "self",
                        Method = "get",
                        Href = $"/users/{u.Id}"
                    }
                }
            }).Where(u => u.Email == email)
              .First();

            return user;
        }

        public UserDetailsDto GetUserDetails(int id)
        {
            return new UserDetailsDto() { };
        }

        public void ValidateEmail (string key, string email)
        {
            // confirm email of user that has this key
        }

        // PRIVATE METHODS
        private string GenerateEmailConfirmationKey()
        {
            return "token";
        }
    }
}
