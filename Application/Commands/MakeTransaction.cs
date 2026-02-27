
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands
{
    public class MakeTransaction
    {
        public record MakeTransactionCommand(decimal Amount, string PayStackREference, string Description, TransactionType Type) : IRequest<MakeTransactionResponse>;

        public class MakeTransactionHandler : IRequestHandler<MakeTransactionCommand, MakeTransactionResponse>
        {
            private readonly ITransactionRepository _transactionRepository;
            private readonly ICurrentUser _currentUser;
            private readonly IWalletRepository _walletRepository;
            private readonly IUserRepository _userRepository;
            private readonly ICustomerRepository _customerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public MakeTransactionHandler(ITransactionRepository transactionRepository,
                ICurrentUser currentUser, IWalletRepository walletRepository, 
                IUserRepository userRepository, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
            {
                _transactionRepository = transactionRepository;
                _currentUser = currentUser;
                _userRepository = userRepository;   
                _customerRepository = customerRepository;
                _walletRepository = walletRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<MakeTransactionResponse> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
            {
                var userId = _currentUser.GetCurrentUser();
                var getUser = await _userRepository.GetAsync(userId);

                if (getUser is null)
                    throw new Exception("User not found");

                var getCustomer = await _customerRepository.GetCustomerAsync(getUser.Email);

                if (getCustomer is null)
                    throw new Exception("User not found");

                var getWallet = await _walletRepository.GetByCustomerIdAsync(getCustomer.Id);


                var transaction = new Tranzaction
                {
                     Amount = request.Amount,
                     Description = request.Description,
                     PaystackReference = request.PayStackREference,
                     CreatedBy = getUser.Email,
                     Type = request.Type,
                     WalletId = getWallet.Id,
                     
                };

                await _transactionRepository.AddTransactionsAsync(transaction);
                await _unitOfWork.SaveAsync();

                return new MakeTransactionResponse(transaction.Id, transaction.Status);
            }
        }
        public record MakeTransactionResponse(Guid TransactionId, TranzactionStatus Transaction);
    }
}
