using StripeAPITest.Shared.Models;

namespace StripeAPITest.DataAccessLayer.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionTypeId TransactionTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentTransactionId { get; set; }
        public Guid CreatedUserId { get; set; }
        public string PayerCoordinates { get; set; }
        public string PayeeCoordinates { get; set; }
        public string? ChargeCoordinates { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }
        public virtual Transaction? ParentTransaction { get; set; }
        public string Description { get; set; }
    }
}