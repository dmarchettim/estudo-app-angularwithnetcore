using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public DataContext Context { get; }

        public ValuesController(DataContext context)
        {
            this.Context = context;
        }


        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
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
