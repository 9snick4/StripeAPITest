using FluentValidation;

namespace StripeAPITest.Shared.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public long Amount { get; set; }
        public string? Description { get; set; }
        public string? ISOCurrency { get; set; }
        public TransactionTypeId TransactionTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentTransactionId { get; set; }
        public Guid CreatedUserId { get; set; }
        public string? PayerCoordinates { get; set; }
        public string? PayeeCoordinates { get; set; }
        public string? ChargeCoordinates { get; set; }

    }

    public class TransactionValidator : AbstractValidator<Transaction>
    {

    }
}