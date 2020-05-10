using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repository;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await repository.GetUsers();

            var usersMapped = mapper.Map<IEnumerable<UserForListDTO>>(users);

            return Ok(usersMapped);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await repository.GetUser(id);

            var userMapped = mapper.Map<UserForDetailedList>(user);

            return Ok(userMapped);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO user)
        {
            //verificando se o ID informado é o mesmo do ID do corpo
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized("Usuário diferente do ID fornecido no token");

            var userFromRepo = await repository.GetUser(id);
            
            Console.WriteLine("antes do mapping");
            Console.WriteLine(userFromRepo.Username);

            mapper.Map(user, userFromRepo);

            Console.WriteLine("depois do mapping");
            Console.WriteLine(userFromRepo.Username);

            if(await repository.SaveAll())
            return NoContent();

            throw new System.Exception($"Falha ao atualizar o Usuário com ID {id}");
        }

    }
}