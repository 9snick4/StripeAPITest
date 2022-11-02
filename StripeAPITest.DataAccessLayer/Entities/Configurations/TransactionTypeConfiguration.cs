using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StripeAPITest.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeAPITest.DataAccessLayer.Entities.Configurations
{
    internal class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
    {
        public void Configure(EntityTypeBuilder<TransactionType> builder)
        {

            builder
                .Property(e => e.TransactionTypeId)
                .HasConversion<int>();

            builder
                .HasData(
                    Enum.GetValues(typeof(TransactionTypeId))
                        .Cast<TransactionTypeId>()
                        .Select(e => new TransactionType()
                        {
                            TransactionTypeId = e,
                            TransactionTypeName = e.ToString()
                        })
                );
        }
    }
}
