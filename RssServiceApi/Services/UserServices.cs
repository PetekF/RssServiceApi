using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
using RssServiceApi.RequestModels;
using RssServiceApi.Exceptions;
using EntityFramework.Exceptions.Common;
using System.Net.Mail;

namespace RssServiceApi.Services
{
    public class UserServices
    {
        private RssDbContext _dbCtx;
        private IConfiguration? _config;

        public UserServices(RssDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        public UserServices(RssDbContext dbCtx, IConfiguration config)
        {
            _dbCtx = dbCtx;
            _config = config;
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
                EmailVerificationKey = GenerateEmailConfirmationKey()
            });

            try
            {
                _dbCtx.SaveChanges();
            }
            catch(UniqueConstraintException e) when(e.InnerException.Message.Contains(nameof(User.Email)))
            {
                throw new UserAlreadyExistsException(credentials.Email);
            }

            if (_config.GetValue<bool>("Email:EmailVerificationEnabled") == true)
            {
                User user = _dbCtx.Users.First(u => u.Email == credentials.Email);
                SendVerificationEmail(user);
            }
        }

        public UserDetailsDto GetUserDetailsByEmail(string email)
        {
            IQueryable<UserDetailsDto> q = UserDetailsSelect();

            UserDetailsDto user = q.Where(u => u.Email == email).First();

            return user;
        }

        public UserDetailsDto GetUserDetailsById(int id)
        {
            IQueryable<UserDetailsDto> q = UserDetailsSelect();

            UserDetailsDto user = q.Where(u => u.Id == id).First();

            return user;
        }

        /// <exception cref="EmailVerificationException"></exception>
        public void ValidateEmail (string key, User user)
        {

        }

        // PRIVATE METHODS =========================================================
        private string GenerateEmailConfirmationKey()
        {
            return "token";
        }


        private void SendVerificationEmail(User user)
        {
           if (user.EmailVerified == true)
           {
                return;
           }

           // Rest of code...
        }

        private IQueryable<UserDetailsDto> UserDetailsSelect()
        {
            return _dbCtx.Users.Select(u => new UserDetailsDto()
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
            });
        }
    }
}
