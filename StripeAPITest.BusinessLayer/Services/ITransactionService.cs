

using Stripe;
using StripeAPITest.Shared.Models;

namespace StripeAPITest.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<Charge> AuthoriseAsync(Transaction tran);
        Task<Charge> CaptureAsync(Transaction tran);
        Task<Refund> VoidAsync(Transaction tran);
    }
}
