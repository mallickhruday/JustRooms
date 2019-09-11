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
    /// <summary>
    /// Manages guests accounts
    /// </summary>
    public class AccountApiController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        /// <summary>
        /// Manages guest accounts
        /// </summary>
        /// <param name="commandProcessor">The Brighter Command Processor used to decouple to a Clean Architecture</param>
        /// <param name="queryProcessor">The Darker Command Processor used to decouple to a Clean Architecture</param>
        public AccountApiController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        /// <summary>
        /// Marks a guest account as deleted
        /// </summary>
        /// <param name="id">The id of the guest account</param>
        /// <param name="ct">Cancellation token to halt an ongoing request</param>
        /// <returns></returns>
        [HttpDelete("/accounts/{id}", Name = "Delete_Account")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteExistingAccountCommand =new DeleteExistingAccountCommand(Guid.Parse(id));
            await _commandProcessor.SendAsync(deleteExistingAccountCommand, false, ct);
            return Ok();
        }
        
        /// <summary>
        /// Gets the details of a guest account
        /// </summary>
        /// <param name="id">The id of the guest account</param>
        /// <param name="ct">Cancellation token to halt an ongoing request</param>
        /// <returns></returns>
        [HttpGet("/accounts/{id}", Name = "Get_Account")]
        public async Task<IActionResult> Get(string id, CancellationToken ct)
        {
            var accountById = new GetAccountById(Guid.Parse(id));
            var account = await _queryProcessor.ExecuteAsync(accountById, ct); 
            return Ok(AccountDTO.FromQueryResult(account));
        }
        
        /// <summary>
        /// Create a new guest account
        /// </summary>
        /// <param name="accountDto">The guest's details</param>
        /// <param name="ct">Cancellation token to halt an ongoing request</param>
        /// <returns></returns>
        [HttpPost("/accounts", Name = "Add_Account")]
        public async Task<IActionResult> Post([FromBody]AccountDTO accountDto, CancellationToken ct)
        {
            var addNewAccountCommand = new AddNewAccountCommand(
                new Name{FirstName = accountDto.Name.FirstName, LastName = accountDto.Name.LastName},
                accountDto.Addresses.Select(addr =>
                        new Address(
                            addr.FistLineOfAddress,
                            Enum.Parse<AddressType>(addr.AddressType),
                            addr.State,
                            addr.ZipCode))
                    .ToList(),
                new ContactDetails{Email = accountDto.ContactDetails.Email, TelephoneNumber = accountDto.ContactDetails.TelephoneNumber},
                new CardDetails{CardNumber = accountDto.CardDetails.CardNumber, CardSecurityCode = accountDto.CardDetails.CardSecurityCode}
                );
                
            await _commandProcessor.SendAsync(addNewAccountCommand, false, ct);
            var account = await _queryProcessor.ExecuteAsync(new GetAccountById(addNewAccountCommand.Id), ct); 
            return Ok(AccountDTO.FromQueryResult(account));
        }

        /// <summary>
        /// Update an existing guest account
        /// </summary>
        /// <param name="id">The id of the guest account to update</param>
        /// <param name="accountDto">The guest's new details</param>
        /// <param name="ct">Cancellation token to halt an ongoing request</param>
        /// <returns></returns>
        [HttpPut("/accounts/{id}", Name = "Update_Account")]
        public async Task<IActionResult> Put(string id, [FromBody]AccountDTO accountDto, CancellationToken ct)
        {
            var updateExistingAccountCommand = new UpdateExistingAccountCommand(
                Guid.Parse(id),
                new Name {FirstName = accountDto.Name.FirstName, LastName = accountDto.Name.LastName},
                accountDto.Addresses.Select(addr =>
                        new Address(
                            addr.FistLineOfAddress,
                            Enum.Parse<AddressType>(addr.AddressType),
                            addr.State,
                            addr.ZipCode))
                    .ToList(),
                new ContactDetails
                {
                    Email = accountDto.ContactDetails.Email, TelephoneNumber = accountDto.ContactDetails.TelephoneNumber
                },
                new CardDetails
                {
                    CardNumber = accountDto.CardDetails.CardNumber,
                    CardSecurityCode = accountDto.CardDetails.CardSecurityCode
                });
            
            await _commandProcessor.SendAsync(updateExistingAccountCommand, false, ct);
            var account = await _queryProcessor.ExecuteAsync(new GetAccountById(updateExistingAccountCommand.AccountId), ct); 
            return Ok(AccountDTO.FromQueryResult(account));
        }
    }
}