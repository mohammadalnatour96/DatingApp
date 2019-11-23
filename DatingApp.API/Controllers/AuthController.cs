using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {
            var authUser = await _repo.Login(user.username.ToLower(), user.password);
            if (authUser == null)
                return Unauthorized();

            var claimes = new[]
            {
              new Claim(ClaimTypes.NameIdentifier,authUser.Id.ToString()),
              new Claim(ClaimTypes.Name,authUser.Username)
          };
          var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)); 

          var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

          var tokenDescriptor=new SecurityTokenDescriptor
          {
              Subject=new ClaimsIdentity(claimes),
              Expires=DateTime.Now.AddDays(1),
              SigningCredentials=creds
          };

          var tokenHandler=new JwtSecurityTokenHandler();
          var token=tokenHandler.CreateToken(tokenDescriptor);

          return Ok(new{
              token=tokenHandler.WriteToken(token)
          });
        }
    }
}