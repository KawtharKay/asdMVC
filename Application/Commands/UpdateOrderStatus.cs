using Application.Common.Dtos;
using Application.Repositories;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class UpdateOrderStatus
    {
        public record UpdateOrderStatusCommand(
            Guid OrderId,
            OrderStatus Status) : IRequest<Result<string>>;

        public class UpdateOrderStatusValidator
            : AbstractValidator<UpdateOrderStatusCommand>
        {
            public UpdateOrderStatusValidator()
            {
                RuleFor(x => x.OrderId)
                    .NotEmpty().WithMessage("Order ID is required");

                RuleFor(x => x.Status)
                    .IsInEnum().WithMessage("Invalid order status");
            }
        }

        public class UpdateOrderStatusHandler(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateOrderStatusCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateOrderStatusCommand request,
                CancellationToken cancellationToken)
            {
                var order = await orderRepository.GetByIdAsync(request.OrderId);
                if (order is null)
                    return Result<string>.Failure("Order not found");

                if (order.Status == OrderStatus.Cancelled)
                    return Result<string>.Failure("Cannot update a cancelled order");

                order.Status = request.Status;
                order.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    $"Order status updated to {request.Status}", "updated");
            }
        }
    }
}