using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _repository { get; set; }
        private IConfiguration _config { get; set; }
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repo;
            _config = config;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDTO userForRegister)
        {
            //DTO = Data Transfer Object: criado para capturar o JSON no Request da API
            userForRegister.Username = userForRegister.Username.ToLower();


            if (await _repository.UserExists(userForRegister.Username))
            {
                Log.Information(JsonConvert.SerializeObject(userForRegister));
                Log.Information("Passou pela API!");
                return BadRequest("Usuário já existente");
            }

            var usuarioParaCriar = _mapper.Map<User>(userForRegister);

            var usuarioCriado = await _repository.Register(usuarioParaCriar, userForRegister.Password);

            var usuarioParaRetornar = _mapper.Map<UserForDetailedList>(usuarioCriado);

            //return StatusCode(StatusCodes.Status201Created); // retorna 201
            return CreatedAtRoute("GetUser", new {Controller ="Users", id = usuarioCriado.Id}, usuarioParaRetornar);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDTO userForLogin)
        {
            var user = await _repository.Login(userForLogin.Username, userForLogin.Password);

            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),//+";"+"testediego"+";"+user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("chavediego","valordiego")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userToReturn = _mapper.Map<UserForListDTO>(user);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user =  userToReturn
            });

        }
    }
}