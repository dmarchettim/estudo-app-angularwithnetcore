using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _repository { get; set; }
        private IConfiguration _config {get; set; }

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repository = repo;
            _config = config;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDTO userForRegister)
        {
            //DTO = Data Transfer Object: criado para capturar o JSON no Request da API
            userForRegister.Username = userForRegister.Username.ToLower();


            if(await _repository.UserExists(userForRegister.Username))
            return BadRequest("Usuário já existente");

            var usuarioParaCriar = new User()
            {
                Username = userForRegister.Username
            };

            var usuarioCriado = await _repository.Register(usuarioParaCriar, userForRegister.Password);

            return StatusCode(StatusCodes.Status201Created); // retorna 201
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDTO userForLogin)
        {
            var user = await _repository.Login(userForLogin.Username, userForLogin.Password);

            if(user == null) 
            return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                    token = tokenHandler.WriteToken(token)
            });

        }        
    }
}