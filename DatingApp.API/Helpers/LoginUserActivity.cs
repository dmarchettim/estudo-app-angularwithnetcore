using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection; //para obter o serviço

namespace DatingApp.API.Helpers
{
    //como colocanos na controller UserController, toda vez q chamarmos qlqr metodo/endpoint la,
    //em paralelo chamará esse método aqui
    public class LoginUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //requisitando a pipeline async. Isso traz tudo do contexto
            var resultContext = await next();

            //obtendo o id via token do user (lembrando que o token é gerado via AuthController.Login)
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //obtendo o repositorio criado via Dependency Injection na Registry da Startup.cs
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();

            //atualizando e salvando informações do usuário
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}