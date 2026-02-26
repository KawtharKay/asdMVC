using Application.Common;
using Application.Repositories;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetNotifications
    {
        public record GetNotificationsQuery(Guid UserId) : IRequest<Result<ICollection<GetNotificationsResponse>>>;

        public class GetNotificationsHandler(INotificationRepository notificationRepository) : IRequestHandler<GetNotificationsQuery, Result<ICollection<GetNotificationsResponse>>>
        {
            public async Task<Result<ICollection<GetNotificationsResponse>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
            {
                var notifications = await notificationRepository.GetAllAsync(request.UserId);
                return Result<ICollection<GetNotificationsResponse>>.Success("Success!", notifications.Adapt<ICollection<GetNotificationsResponse>>());
            }
        }

        public record GetNotificationsResponse(Guid Id, string Title, string Ref, Guid UserId, NotificationType Type, string Message, bool IsRead);
    }
}