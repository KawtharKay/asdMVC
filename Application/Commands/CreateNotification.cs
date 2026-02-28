using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateNotification
    {
        public record CreateNotificationCommand(string Title, string Ref, Guid UserId, NotificationType Type, string Message) : IRequest<Result<CreateNotificationResponse>>;

        public class CreateNotificationHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateNotificationCommand, Result<CreateNotificationResponse>>
        {
            public async Task<Result<CreateNotificationResponse>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
            {
                var notification = new Notification
                {
                    Title = request.Title,
                    Ref = request.Ref,
                    UserId = request.UserId,
                    Type = request.Type,
                    Message = request.Message,
                    IsRead = false
                };

                await notificationRepository.AddAsync(notification);
                await unitOfWork.SaveAsync();

                return Result<CreateNotificationResponse>.Success("Success!", notification.Adapt<CreateNotificationResponse>());
            }
        }

        public record CreateNotificationResponse(Guid Id);
    }
}