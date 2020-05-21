using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static void AdicionarPaginacao(this HttpResponse response, int currentPage, 
        int ItemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, ItemsPerPage, totalPages, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Acess-Control-Expose-Headers", "Pagination");
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