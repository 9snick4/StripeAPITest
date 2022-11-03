using StripeAPITest.BusinessLayer.Services;
using StripeAPITest.BusinessLayer.Extensions;
using StripeAPITest.Shared.Models;

namespace StripeAPITest.UnitTests
{
    public class TransactionTests
    {
        private readonly ITransactionService _service;
        [Fact]
        public async void ThrowError_CaptureVoidWithoutParent()
        {
            var parent = new Transaction
            {
                ISOCurrency = "usd",
                Amount = 1000,
                Description = "I'm getting the charge coordinates",
                PayeeCoordinates = "iban",
                PayerCoordinates = "credit card",
                TransactionTypeId = TransactionTypeId.Authorize
            };
            var chargeParent = await _service.AuthoriseAsync(parent);    


            var t = new Transaction
            {
                ISOCurrency = "usd",
                Amount = 1000,
                Description = "I don't have a parent transaction id"
            };
            t.ChargeCoordinates = chargeParent.Id;

            await Assert.ThrowsAsync<NoParentTransactionException>(()=> _service.CaptureAsync(t));
            await Assert.ThrowsAsync<NoParentTransactionException>(()=> _service.VoidAsync(t));            
        }
        [Fact]
        public void ThrowError_CaptureVoidWithoutChargeCoordinates()
        {
            var t = new Transaction
            {
                ISOCurrency = "usd",
                Amount = 1000,
                Description = "I don't have the stripe charge coordinates",
                ParentTransactionId = 1
            };
            Assert.NotNull(t);
            Assert.Null(t.ParentTransactionId);
            Assert.ThrowsAsync<Stripe.StripeException>(() => _service.CaptureAsync(t));
            Assert.ThrowsAsync<Stripe.StripeException>(() => _service.VoidAsync(t));
        }



    }
}