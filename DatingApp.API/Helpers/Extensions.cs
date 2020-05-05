using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AdicionarErroAplicacao(this HttpResponse response, string mensagem)
        {
            response.Headers.Add("Application-Error", mensagem);
            response.Headers.Add("Acess-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Acess-Control-Allow-Origin", "*");
        }

        public static int CalcularIdade(this DateTime dataEmQuestao)
        {
            var idade = DateTime.Today.Year - dataEmQuestao.Year;

            if(dataEmQuestao.AddYears(idade) > DateTime.Today)
            idade--;

            return idade;
        }
    }
}