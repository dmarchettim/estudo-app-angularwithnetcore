using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LoginUserActivity))] //para utilizar o Filter que criamos
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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await repository.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            //var users = await repository.GetUsers();
            var users = await repository.GetUsers(userParams);

            var usersMapped = mapper.Map<IEnumerable<UserForListDTO>>(users);

            //como estamos na Controller, temos acesso ao objeto Response e, portanto, vamos usar o extended method que criamos
            Response.AdicionarPaginacao(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersMapped);
        }

        [HttpGet]
        [Route("{id}", Name = "GetUser")]
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