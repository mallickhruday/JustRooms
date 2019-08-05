using System.Threading;
using System.Threading.Tasks;
using Accounts.Ports.Commands;
using Accounts.Ports.Queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Accounts.Adapters.Controllers
{
    public class AccountControllerAsync : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public AccountControllerAsync(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(AddNewAccountCommand addNewAccountCommand, CancellationToken ct)
        {
            await _commandProcessor.SendAsync(addNewAccountCommand, false, ct);
            var account = await _queryProcessor.ExecuteAsync(new GetAccountById(addNewAccountCommand.Id), ct); 
            return Ok(account);
        }
    }
}