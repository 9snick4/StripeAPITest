

using Microsoft.AspNetCore.Mvc;
using StripeAPITest.BusinessLayer.Services;
using StripeAPITest.Shared.Models;

namespace StripeAPITest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService service)
        {
            _service =service;
            _logger = logger;
        }

        [HttpPost("Authorize")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> AuthorizeAsync(Transaction transaction)
        {
            try
            {
                var ok = await _service.AuthoriseAsync(transaction);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Capture")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> CaptureAsync( Transaction transaction)
        {
            try
            {
                var ok = await _service.CaptureAsync(transaction);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Void")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> VoidAsync( Transaction transaction)
        {
            try
            {
                var ok = await _service.VoidAsync(transaction);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}