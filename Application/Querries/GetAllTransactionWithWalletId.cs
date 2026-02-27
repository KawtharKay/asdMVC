using Application.Common;
using Application.Common.Pagenation;
using Application.Repositories;
using Domain.Enums;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Application.Querries
{
    public class GetAllTransactionWithWalletId 
    {
        public record GetAllTransactionWithWalletIdQuery(int Page, int PageSize) : IRequest<Result<PagenatedList<GetAllTransactionWithWalletIdResponse>>>;

        public class GetAllTransactionWithWalletIdHandler(ITransactionRepository transactionRepository,
            ICurrentUser currentUser, IUserRepository user, ICustomerRepository customer, IWalletRepository wallet) :
            IRequestHandler<GetAllTransactionWithWalletIdQuery, Result<PagenatedList<GetAllTransactionWithWalletIdResponse>>>
        {
            public async Task<Result<PagenatedList<GetAllTransactionWithWalletIdResponse>>> Handle(GetAllTransactionWithWalletIdQuery request, CancellationToken cancellationToken)
            {
                var userId = currentUser.GetCurrentUser();
                var getUser = await user.GetAsync(userId);

                if (getUser is null)
                    throw new Exception("User not found");

                var getCustomer = await customer.GetCustomerAsync(getUser.Email);

                if (getCustomer is null)
                    throw new Exception("User not found");

                var getWallet = await wallet.GetByCustomerIdAsync(getCustomer.Id);
                if (getWallet is null) throw new Exception("Wallet not found");

                var page = new PageRequest
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                };

                var transact = await transactionRepository.GetTransactionsByWalletIdAsync(page, getWallet.Id);
                var type = transact.Items;

                return Result<PagenatedList<GetAllTransactionWithWalletIdResponse>>.Success("Reteived", transact.Adapt<PagenatedList<GetAllTransactionWithWalletIdResponse>>());
            }
        }

        public record GetAllTransactionWithWalletIdResponse(decimal Amount, TransactionType Type, Domain.Enums.TransactionStatus Status);
    }
}
