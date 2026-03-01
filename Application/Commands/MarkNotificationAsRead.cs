using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class MarkNotificationAsRead
    {
        public record MarkNotificationAsReadCommand(
            Guid NotificationId,
            Guid UserId,
            bool MarkAll = false) : IRequest<Result<string>>;

        public class MarkNotificationAsReadValidator
            : AbstractValidator<MarkNotificationAsReadCommand>
        {
            public MarkNotificationAsReadValidator()
            {
                RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID is required");
            }
        }

        public class MarkNotificationAsReadHandler(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<MarkNotificationAsReadCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                MarkNotificationAsReadCommand request,
                CancellationToken cancellationToken)
            {
                if (request.MarkAll)
                {
                    var all = await notificationRepository
                        .GetByUserIdAsync(request.UserId, unreadOnly: true);

                    foreach (var n in all)
                    {
                        n.IsRead = true;
                        n.DateModified = DateTime.UtcNow;
                    }
                }
                else
                {
                    var notification = await notificationRepository
                        .GetByIdAsync(request.NotificationId);

                    if (notification is null)
                        return Result<string>.Failure("Notification not found");

                    if (notification.UserId != request.UserId)
                        return Result<string>.Failure("Unauthorized");

                    notification.IsRead = true;
                    notification.DateModified = DateTime.UtcNow;
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Marked as read!", "updated");
            }
        }
    }
}