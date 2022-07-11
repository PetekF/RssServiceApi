using Microsoft.EntityFrameworkCore;
using RssServiceApi.DTOs;
using RssServiceApi.Entities;
using RssServiceApi.RequestModels;
using RssServiceApi.Exceptions;
using EntityFramework.Exceptions.Common;
using System.Net.Mail;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            var passwordHash = HashPassword(credentials.Password);

            _dbCtx.Users.Add(new User()
            {
                Email = credentials.Email,
                HashedPassword = passwordHash.hashedPassword,
                HashSalt = passwordHash.saltBase64String,
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

        public List<UserDto> GetUsers()
        {
            return _dbCtx.Users.Select(u =>

                new UserDto()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Links = new List<LinkDto>()
                    {
                        new LinkDto()
                        {
                            Rel = "self",
                            Method = "get",
                            Href = $"/users/{u.Id}"
                        }
                    }
                }).ToList();
        }

        public string? AuthenticateUser(LoginCredentials loginCredentials)
        {
            
            User user = _dbCtx.Users.SingleOrDefault(u => u.Email == loginCredentials.Email);

            if (user == null || user.HashedPassword != HashPassword(loginCredentials.Password, user.HashSalt))
            {
                return null;
            }
            
            return CreateJwtToken(user);
        }

        /// <exception cref="EmailVerificationException"></exception>
        public void ValidateEmail (string key, User user)
        {

        }


        // PRIVATE METHODS =========================================================

        private string CreateJwtToken(User user)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, _config.GetValue<String>("Jwt:Issuer")),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private (string hashedPassword, string saltBase64String) HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(salt);
            }

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            string saltBase64String = Convert.ToBase64String(salt);

            return (hashedPassword, saltBase64String);
        }

        private string HashPassword(string password, string saltBase64String)
        {
            byte[] salt = Convert.FromBase64String(saltBase64String);

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }

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
