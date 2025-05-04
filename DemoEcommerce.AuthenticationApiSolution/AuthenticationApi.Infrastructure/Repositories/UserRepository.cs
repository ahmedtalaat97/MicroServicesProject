using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Context;
using eCommerce.SharedLibarary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context,IConfiguration configuration) : IUser
    {

        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.Email == email);
            return user is not null ? user : null;
        }
        public async Task<GetUserDto> GetUser(int userId)
        {
           var user = await context.Users.FindAsync(userId);
            return user is not null ? new GetUserDto(user.Id, user.Name, user.TelephoneNumber, user.Address, user.Email,user.Role)! : null!;
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var getUser = await GetUserByEmail(loginDto.Email);
            if (getUser is null)
            {
                return new Response(false, "Invalid email or password");
            }
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
            if (!verifyPassword)

            {
                return new Response(false, "Invalid email or password");
            }
            string token = GenerateToken(getUser);
            return new Response(true, token);

        }

        private   string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value!);
            var securityKey=new SymmetricSecurityKey(key);
            var cerdentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name,user.Name),
                new (ClaimTypes.Email,user.Email),


            };

            if(string.IsNullOrEmpty(user.Role)|| !Equals("string",user.Role))
            {
                claims.Add(new(ClaimTypes.Role,user.Role!));
            }
            var token = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: cerdentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            var getUser = await GetUserByEmail(appUserDto.Email);
            if (getUser is not null)
            {
                return new Response(false, $" you cannot use this email {getUser.Email}");
            }

            var result = context.Users.Add(new AppUser()
            {
                Name=appUserDto.Name,
                Email=appUserDto.Email,
                Role=appUserDto.Role,
                Password=BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                TelephoneNumber=appUserDto.TelephoneNumber,
                Address=appUserDto.Address,

            });

            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "User registerd successfully") : new Response(false, "Invalid data provided");
        }
    }
}
