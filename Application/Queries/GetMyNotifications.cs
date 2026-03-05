using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetMyNotifications
    {
        public record GetMyNotificationsQuery(
            Guid UserId,
            bool UnreadOnly = false)
            : IRequest<Result<List<GetMyNotificationsResponse>>>;

        public class GetMyNotificationsValidator
            : AbstractValidator<GetMyNotificationsQuery>
        {
            public GetMyNotificationsValidator()
            {
                RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID is required");
            }
        }

        public class GetMyNotificationsHandler(
            INotificationRepository notificationRepository)
            : IRequestHandler<GetMyNotificationsQuery,
                Result<List<GetMyNotificationsResponse>>>
        {
            public async Task<Result<List<GetMyNotificationsResponse>>> Handle(
                GetMyNotificationsQuery request,
                CancellationToken cancellationToken)
            {
                var notifications = await notificationRepository
                    .GetByUserIdAsync(request.UserId, request.UnreadOnly);

                var response = notifications.Select(n =>
                    new GetMyNotificationsResponse(
                        n.Id, n.Title, n.Message,
                        n.Type.ToString(), n.IsRead,
                        n.Ref, n.DateCreated))
                    .ToList();

                return Result<List<GetMyNotificationsResponse>>
                    .Success("Success", response);
            }
        }

        public record GetMyNotificationsResponse(
            Guid Id, string Title, string Message,
            string Type, bool IsRead,
            string Ref, DateTime DateCreated);
    }
}