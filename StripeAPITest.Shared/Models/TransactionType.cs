using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.Shared.Models
{
    public enum TransactionTypeId : int
    {
        Authorize = 1,
        Capture = 2,
        Void = 3,

    }
    public class TransactionType
    {
        public TransactionTypeId TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public List<Transaction>? Transactions { get; set; }
    }
}
