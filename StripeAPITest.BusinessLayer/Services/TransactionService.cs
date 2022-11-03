using Microsoft.Extensions.Logging;
using Stripe;
using StripeAPITest.DataAccessLayer;
using StripeAPITest.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StripeAPITest.BusinessLayer.Extensions;

namespace StripeAPITest.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly StripeContext context;
        private readonly ILogger<TransactionService> _logger;
        private readonly ChargeService chargeService;
        private readonly RefundService refundService;
        private readonly Balance balance;

        public TransactionService(StripeContext context, ILogger<TransactionService> logger)
        {
            this.context = context;
            _logger = logger;
            chargeService = new ChargeService();
            var balanceService = new BalanceService();
            refundService = new RefundService();
            balance = balanceService.Get();
        }

        public async Task<Charge> AuthoriseAsync(Transaction tran)
        {
           
            var options = new ChargeCreateOptions
            {
                Amount = tran.Amount,
                Currency = tran.ISOCurrency,
                Source = "tok_mastercard",
                Description = tran.Description,
            };

            var ret = await chargeService.CreateAsync(options);
            var entity = await InsertTransaction(tran);
            tran.Id = entity.Id;
            return ret;
        }

        public async Task<Charge> CaptureAsync(Transaction tran)
        {
            if (!tran.ParentTransactionId.HasValue)
            {
                throw new NoParentTransactionException();
            }
            var ret = chargeService.Capture(tran.ChargeCoordinates);
            var entity = await InsertTransaction(tran);
            tran.Id = entity.Id;
            return ret;
        }

        public async Task<Refund> VoidAsync(Transaction tran)
        {
            if (!tran.ParentTransactionId.HasValue)
            {
                throw new NoParentTransactionException();
            }
            var options = new RefundCreateOptions
            {
                Charge =  tran.ChargeCoordinates,
            };           
            var ret = refundService.Create(options);
            var entity = await InsertTransaction(tran);
            tran.Id = entity.Id;
            return ret;
        }

        private async Task<DataAccessLayer.Entities.Transaction> InsertTransaction(Transaction tran)
        {
            var entity = new DataAccessLayer.Entities.Transaction
            {
                Amount = tran.Amount,
                CreatedDate = DateTime.Now,
                CreatedUserId = tran.CreatedUserId,
                TransactionTypeId = tran.TransactionTypeId,
                PayeeCoordinates = tran.PayeeCoordinates,
                ParentTransactionId = tran.ParentTransactionId,
                PayerCoordinates = tran.PayerCoordinates,
                ChargeCoordinates = tran.ChargeCoordinates,
                Description = tran.Description
            };
            await context.Transactions.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
