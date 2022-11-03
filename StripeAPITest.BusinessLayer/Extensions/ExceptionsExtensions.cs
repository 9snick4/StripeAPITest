using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.BusinessLayer.Extensions
{

    [Serializable]
    public class NoParentTransactionException : Exception
    {
        public NoParentTransactionException() : base("The requested operation requires a parent transaction, and none was provided.") { }


    }
}
