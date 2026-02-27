using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Application.Querries
{
    public class GetAllTransactionWithWalletId 
    {
        public record GetAllTransactionWithWalletIdQuery(int Page, int PageSize);
        public record GetAllTransactionWithWalletIdResponse(decimal Amount, TransactionType Type, Domain.Enums.TransactionStatus Status);
    }
}
