using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class CancelOrder
    {
        public record CancelOrderCommand(
            Guid OrderId,
            Guid CustomerId) : IRequest<Result<string>>;

        public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
        {
            public CancelOrderValidator()
            {
                RuleFor(x => x.OrderId)
                    .NotEmpty().WithMessage("Order ID is required");

                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class CancelOrderHandler(
            IOrderRepository orderRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CancelOrderCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(CancelOrderCommand request,CancellationToken cancellationToken)
            {
                var order = await orderRepository.GetByIdAsync(request.OrderId);
                if (order is null) return Result<string>.Failure("Order not found");

                if (order.CustomerId != request.CustomerId) return Result<string>.Failure("Unauthorized");

                if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered) 
                    return Result<string>.Failure("Cannot cancel a shipped or delivered order");

                if (order.Status == OrderStatus.Cancelled)
                    return Result<string>.Failure("Order already cancelled");

                order.Status = OrderStatus.Cancelled;
                order.DateModified = DateTime.UtcNow;

                foreach (var item in order.ProductOrders)
                {
                    item.Product.StockQuantity += item.Quantity;
                    item.Product.DateModified = DateTime.UtcNow;
                }

                var wallet = await walletRepository.GetByCustomerIdAsync(request.CustomerId);
                if (wallet is not null)
                {
                    var balanceBefore = wallet.Balance;
                    wallet.Balance += order.TotalAmount;

                    await walletTransactionRepository.AddAsync(new WalletTransaction
                    {
                        WalletId = wallet.Id,
                        Amount = order.TotalAmount,
                        Type = TransactionType.Credit,
                        Status = WalletTransactionStatus.Success,
                        Description = $"Refund for cancelled order {order.OrderNo}",
                        BalanceBefore = balanceBefore,
                        BalanceAfter = wallet.Balance,
                        CreatedBy = request.CustomerId.ToString()
                    });
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Order cancelled and wallet refunded!", "cancelled");
            }
        }
    }
}