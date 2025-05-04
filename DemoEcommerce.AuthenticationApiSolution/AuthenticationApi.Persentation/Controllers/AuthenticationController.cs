using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using eCommerce.SharedLibarary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Persentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUser user) : ControllerBase
    {
        [HttpPost("register")]
        
        public async Task<ActionResult<Response>> Register(AppUserDto appUser)
        {

            if (appUser == null)
            {
                return BadRequest("Please enter a valid input");
            }
            if(!ModelState.IsValid)
            {
                return BadRequest("Please enter a valid input");

            }

            var result= await user.Register(appUser);

            return result.Flag ? Ok(result) : BadRequest(result);
        }


        [HttpPost("login")]

        public async Task<ActionResult<Response>> Login(LoginDto login)
        {

            if (login == null || !ModelState.IsValid)
            {
                return BadRequest("Please enter a valid input");
            }
            

            var result = await user.Login(login);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
            public async Task<ActionResult<GetUserDto>> GetUser(int id)
            {

                if(id<0)
                {
                    return BadRequest("Please enter a valid input");

                }

                var returnedUser= await user.GetUser(id);
                return returnedUser.Id > 0 ? Ok(returnedUser) : NotFound();
            }


    }
}
