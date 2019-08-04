using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GuestsAccounts.Adapters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        // GET api/guests
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET api/guests/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/guests
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/guests/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/guests/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}