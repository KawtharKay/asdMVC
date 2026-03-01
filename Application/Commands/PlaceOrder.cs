using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class PlaceOrder
    {
        public record PlaceOrderCommand(
            Guid CustomerId,
            string DeliveryAddress) : IRequest<Result<PlaceOrderResponse>>;

        public class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
        {
            public PlaceOrderValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");

                RuleFor(x => x.DeliveryAddress)
                    .NotEmpty().WithMessage("Delivery address is required")
                    .MaximumLength(300).WithMessage("Address cannot exceed 300 characters");
            }
        }

        public class PlaceOrderHandler(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResponse>>
        {
            public async Task<Result<PlaceOrderResponse>> Handle(
                PlaceOrderCommand request,
                CancellationToken cancellationToken)
            {
                var cart = await cartRepository.GetByCustomerIdAsync(request.CustomerId);
                if (cart is null)
                    return Result<PlaceOrderResponse>.Failure("Cart not found");

                var cartItems = cart.CartItems.Where(ci => !ci.IsDeleted).ToList();
                if (!cartItems.Any())
                    return Result<PlaceOrderResponse>.Failure("Cart is empty");

                // Check stock
                foreach (var item in cartItems)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                        return Result<PlaceOrderResponse>.Failure(
                            $"{item.Product.Name} only has {item.Product.StockQuantity} items left");
                }

                var totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price);

                // Check wallet balance
                var wallet = await walletRepository.GetByCustomerIdAsync(request.CustomerId);
                if (wallet is null)
                    return Result<PlaceOrderResponse>.Failure("Wallet not found");

                if (wallet.Balance < totalAmount)
                    return Result<PlaceOrderResponse>.Failure(
                        $"Insufficient balance. Balance: ₦{wallet.Balance:N2}, Total: ₦{totalAmount:N2}");

                var order = new Order
                {
                    OrderNo = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
                    CustomerId = request.CustomerId,
                    TotalAmount = totalAmount,
                    DeliveryAddress = request.DeliveryAddress,
                    Status = OrderStatus.Pending,
                    CreatedBy = request.CustomerId.ToString()
                };

                foreach (var item in cartItems)
                {
                    order.ProductOrders.Add(new ProductOrder
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        CreatedBy = request.CustomerId.ToString()
                    });

                    item.Product.StockQuantity -= item.Quantity;
                    item.Product.DateModified = DateTime.UtcNow;
                }

                await orderRepository.AddAsync(order);

                // Debit wallet
                var balanceBefore = wallet.Balance;
                wallet.Balance -= totalAmount;

                await walletTransactionRepository.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = totalAmount,
                    Type = TransactionType.Debit,
                    Status = WalletTransactionStatus.Success,
                    Description = $"Payment for order {order.OrderNo}",
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    CreatedBy = request.CustomerId.ToString()
                });

                // Clear cart
                foreach (var item in cartItems)
                    item.IsDeleted = true;

                await unitOfWork.SaveAsync();

                return Result<PlaceOrderResponse>.Success(
                    "Order placed successfully!",
                    new PlaceOrderResponse(order.Id, order.OrderNo, totalAmount));
            }
        }

        public record PlaceOrderResponse(
            Guid OrderId, string OrderNo, decimal TotalAmount);
    }
}