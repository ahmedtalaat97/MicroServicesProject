using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibarary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register(AppUserDto appUserDto);

        Task<Response> Login(LoginDto loginDto);
        Task<GetUserDto> GetUser(int Id); 
    }
}
