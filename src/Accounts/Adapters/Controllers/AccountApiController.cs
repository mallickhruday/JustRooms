using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.DTOs;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Accounts.Adapters.Controllers
{
    public class AccountApiController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public AccountApiController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [HttpDelete("/accounts/{id}", Name = "Delete_Account")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteExistingAccountCommand =new DeleteExistingAccountCommand(Guid.Parse(id));
            await _commandProcessor.SendAsync(deleteExistingAccountCommand, false, ct);
            return Ok();
        }

        [HttpGet("/accounts/{id}", Name = "Get_Account")]
        public async Task<IActionResult> Get(string id, CancellationToken ct)
        {
            var accountById = new GetAccountById(Guid.Parse(id));
            var account = await _queryProcessor.ExecuteAsync(accountById, ct); 
            return Ok(AccountDTO.FromQueryResult(account));
        }
        
        [HttpPost("/accounts", Name = "Add_Account")]
        public async Task<IActionResult> Post([FromBody]AccountDTO accountDto, CancellationToken ct)
        {
            var addNewAccountCommand = new AddNewAccountCommand(
                new Name(accountDto.Name.FirstName, accountDto.Name.LastName),
                accountDto.Addresses.Select(addr =>
                        new Address(
                            addr.FistLineOfAddress,
                            Enum.Parse<AddressType>(addr.AddressType),
                            addr.State,
                            addr.ZipCode))
                    .ToList(),
                new ContactDetails(accountDto.ContactDetails.Email, accountDto.ContactDetails.TelephoneNumber),
                new CardDetails(accountDto.CardDetails.CardNumber, accountDto.CardDetails.CardSecurityCode)
            );
                
            await _commandProcessor.SendAsync(addNewAccountCommand, false, ct);
            var account = await _queryProcessor.ExecuteAsync(new GetAccountById(addNewAccountCommand.Id), ct); 
            return Ok(AccountDTO.FromQueryResult(account));
        }

        [HttpPut("/accounts/{id}", Name = "Update_Account")]
        public async Task<IActionResult> Put(string id, [FromBody]AccountDTO accountDto, CancellationToken ct)
        {
            var updateExistingAccountCommand = new UpdateExistingAccountCommand(
                Guid.Parse(id),
                new Name(accountDto.Name.FirstName, accountDto.Name.LastName),
                accountDto.Addresses.Select(addr =>
                        new Address(
                            addr.FistLineOfAddress, 
                            Enum.Parse<AddressType>(addr.AddressType), 
                            addr.State, 
                            addr.ZipCode))
                    .ToList(),
                new ContactDetails(accountDto.ContactDetails.Email, accountDto.ContactDetails.TelephoneNumber),
                new CardDetails(accountDto.CardDetails.CardNumber, accountDto.CardDetails.CardSecurityCode)
                );
            
            await _commandProcessor.SendAsync(updateExistingAccountCommand, false, ct);
            var account = await _queryProcessor.ExecuteAsync(new GetAccountById(updateExistingAccountCommand.AccountId), ct); 
            return Ok(AccountDTO.FromQueryResult(account));
        }
    }
}