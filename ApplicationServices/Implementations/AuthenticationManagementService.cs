using ApplicationServices.DTOs;
using Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationServices.Utilities;
using Data.Entities;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationServices.Implementations
{
    public class AuthenticationManagementService : BaseManagementService<UserAuthDto>
    {
        //PasswordHasher hasher = new PasswordHasher();
        private const string SECRET_KEY = "ZRTeH3zO6FcF7wYUW1bAHIFlSMbJOn7zpQtzoaQ60Mgo09xpRZcf4syOJKw5wBzlabboOpy6H-B1JnnvchEDh5lK2Kpyl8gDfxwR9w56MVMJJUNhtb7qLIx5lC2MuQxqEJqTjmuenMKoc21vtOqOjNnGStmY7u85-zXXADW4rdk";
        HMACSHA256 sha256Hasher = new(Encoding.UTF8.GetBytes(SECRET_KEY));
        private string authSecret = "0faJ2Ysd3f4fMfs2bDXQAygaKq2jv8rtHhrf6sFz5iImZPHUqnCzpGiaPZIBtMHRfqpj1rkL8rG7TnzqWTQORqjxWxhXTn3WPn0npfUiOjMJ6nHxS0PcUwJwq92svE9VTvRyK6RDNeDAU2QNvIisjXxtNN-9nhsSuFkQdBwfe30";

        public override List<UserAuthDto> Get()
        {
            // not used?
            throw new NotSupportedException();
            return new();
        }

        public override async Task<List<UserAuthDto>> GetAsync()
        {
            // not used?
            throw new NotSupportedException();
            return new();
        }

        public override UserAuthDto? GetById(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<UserAuthDto>(unitOfWork.UsersRepository.GetById(id));
        }

        public override async Task<UserAuthDto?> GetByIdAsync(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<UserAuthDto>(await unitOfWork.UsersRepository.GetByIdAsync(id));
        }

        public UserAuthDto? GetByUserName(string username)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<UserAuthDto>(unitOfWork.UsersRepository.Get(
                filter: u => u.UserName == username
            ).FirstOrDefault());
        }

        public async Task<UserAuthDto?> GetByUserNameAsync(string username)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<UserAuthDto>((await unitOfWork.UsersRepository.GetAsync(
                filter: u => u.UserName == username
            )).FirstOrDefault());
        }

        public override bool Save(UserAuthDto userAuthDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userAuthDto)!;
            user.Password = Encoding.UTF8.GetString(sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
            user.CreatedOn = DateTime.UtcNow;
            user.UpdatedOn = user.CreatedOn;

            try
            {
                unitOfWork.UsersRepository.Insert(user);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> SaveAsync(UserAuthDto userAuthDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userAuthDto)!;
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(user.Password));
            user.Password = Encoding.UTF8.GetString(await sha256Hasher.ComputeHashAsync(memoryStream));
            user.CreatedOn = DateTime.UtcNow;
            user.UpdatedOn = user.CreatedOn;

            try
            {
                await unitOfWork.UsersRepository.InsertAsync(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync(UserRegisterDto userRegisterDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userRegisterDto)!;
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(user.Password));
            user.Password = Encoding.UTF8.GetString(await sha256Hasher.ComputeHashAsync(memoryStream));
            user.CreatedOn = DateTime.UtcNow;
            user.UpdatedOn = user.CreatedOn;

            try
            {
                await unitOfWork.UsersRepository.InsertAsync(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string? Authenticate(UserAuthDto userAuthDto)
        {
            using var unitOfWork = new UnitOfWork();

            userAuthDto.Password = Encoding.UTF8.GetString(
                sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(userAuthDto.Password))
            );

            try
            {
                var dbUser = unitOfWork.UsersRepository.Get(
                    u => u.UserName == userAuthDto.UserName
                ).FirstOrDefault();

                if (dbUser is not null && dbUser.Password == userAuthDto.Password)
                {
                    return GenerateToken(dbUser);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> AuthenticateAsync(UserAuthDto userAuthDto)
        {
            using var unitOfWork = new UnitOfWork();

            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(userAuthDto.Password));

            userAuthDto.Password = Encoding.UTF8.GetString(
                await sha256Hasher.ComputeHashAsync(memoryStream)
            );

            try
            {
                var dbUser = (await unitOfWork.UsersRepository.GetAsync(
                    u => u.UserName == userAuthDto.UserName
                )).FirstOrDefault();

                if (dbUser is not null && dbUser.Password == userAuthDto.Password)
                {
                    return GenerateToken(dbUser);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserDto?> GetAuthenticatedUserAsync(string token)
        {
            if (token is null) return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(authSecret);

                var validationResult = await tokenHandler.ValidateTokenAsync(
                    token,
                    new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        ClockSkew = TimeSpan.Zero
                    }
                );

                if (validationResult.IsValid)
                {
                    var data = validationResult.Claims;
                    var dto = new UserDto()
                    {
                               Id =    (int)data[nameof(User.Id).ToLowerInvariant()],
                         UserName = (string)data[nameof(User.UserName).ToLowerInvariant()],
                        FirstName = (string)data[nameof(User.FirstName).ToLowerInvariant()],
                         LastName = (string)data[nameof(User.LastName).ToLowerInvariant()],
                          IsAdmin =   (bool)data[nameof(User.IsAdmin).ToLowerInvariant()]
                    };

                    return dto;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private string? GenerateToken(User user)
        {
            var descriptor = new SecurityTokenDescriptor();

            var currentTime = DateTime.UtcNow;
            var expirationTimeInMinutes = 0.5;
            descriptor.Expires = currentTime.AddMinutes(expirationTimeInMinutes);

            descriptor.Claims = new Dictionary<string, object>
            {
                { nameof(user.Id).ToLowerInvariant()       , user.Id },
                { nameof(user.UserName).ToLowerInvariant() , user.UserName },
                { nameof(user.FirstName).ToLowerInvariant(), user.FirstName },
                { nameof(user.LastName).ToLowerInvariant() , user.LastName },
                { nameof(user.IsAdmin).ToLowerInvariant()  , user.IsAdmin },
                { "_iat", currentTime },
                { "_eam", expirationTimeInMinutes },
            };

            var key = Encoding.UTF8.GetBytes(authSecret);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            descriptor.SigningCredentials = signingCredentials;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (token is null) return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(authSecret);

                var validationResult = await tokenHandler.ValidateTokenAsync(
                    token,
                    new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        ClockSkew = TimeSpan.Zero
                    }
                );

                return validationResult.IsValid;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> RefreshTokenAsync(string oldToken)
        {
            if (oldToken is null) return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(authSecret);

                var validationResult = await tokenHandler.ValidateTokenAsync(
                    oldToken,
                    new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        ClockSkew = TimeSpan.Zero
                    }
                );

                if (validationResult.IsValid)
                {
                    var data = validationResult.Claims;
                    var user = new User()
                    {
                               Id =    (int)data[nameof(User.Id).ToLowerInvariant()],
                         UserName = (string)data[nameof(User.UserName).ToLowerInvariant()],
                        FirstName = (string)data[nameof(User.FirstName).ToLowerInvariant()],
                         LastName = (string)data[nameof(User.LastName).ToLowerInvariant()],
                          IsAdmin =   (bool)data[nameof(User.IsAdmin).ToLowerInvariant()]
                    };

                    return GenerateToken(user);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public override bool Update(UserAuthDto userAuthDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userAuthDto)!;
            user.Password = Encoding.UTF8.GetString(sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
            user.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.UsersRepository.Update(user);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(UserAuthDto userAuthDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userAuthDto)!;
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(user.Password));
            user.Password = Encoding.UTF8.GetString(await sha256Hasher.ComputeHashAsync(memoryStream));
            user.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.UsersRepository.Update(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // TODO: Remove?
        public override bool Delete(int id)
        {
            throw new NotSupportedException();
            return false;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            throw new NotSupportedException();
            return false;
        }
    }
}
