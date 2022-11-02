using StripeAPITest.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.DataAccessLayer.Entities
{

    public class TransactionType
    {
        public TransactionTypeId TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public List<Transaction>? Transactions { get; set; }
    }
}
