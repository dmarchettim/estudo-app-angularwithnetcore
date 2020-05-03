using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public DataContext Context { get; }
        public ILoggerFactory LoggerFactory { get; }
        private readonly ILogger<ValuesController> logger;

        public ValuesController(DataContext context, ILoggerFactory loggerFactory, ILogger<ValuesController> logger)
        {
            this.logger = logger;
            this.Context = context;
            LoggerFactory = loggerFactory;
        }


        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            //throw new Exception("Computer says no");
            //Console.WriteLine("teste");
            Log.Information("You can clap up to 50 times per post!");
            //var logger = LoggerFactory.CreateLogger("diego");
            logger.LogError("chamada incorreta!!");
            var result = await Context.Values.ToListAsync();
            return Ok(result);
        }

        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await Context.Values.FirstOrDefaultAsync(val => val.Id == id);

            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
