using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetMyConversations
    {
        public record GetMyConversationsQuery(Guid UserId)
            : IRequest<Result<List<GetMyConversationsResponse>>>;

        public class GetMyConversationsValidator
            : AbstractValidator<GetMyConversationsQuery>
        {
            public GetMyConversationsValidator()
            {
                RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID is required");
            }
        }

        public class GetMyConversationsHandler(
            IConversationRepository conversationRepository)
            : IRequestHandler<GetMyConversationsQuery,
                Result<List<GetMyConversationsResponse>>>
        {
            public async Task<Result<List<GetMyConversationsResponse>>> Handle(
                GetMyConversationsQuery request,
                CancellationToken cancellationToken)
            {
                var conversations = await conversationRepository
                    .GetByUserIdAsync(request.UserId);

                var response = conversations.Select(c =>
                    new GetMyConversationsResponse(
                        c.Id, c.Title,
                        c.IsHandedOffToHuman,
                        c.LastMessageAt,
                        c.Messages.Count,
                        c.Messages
                            .OrderByDescending(m => m.SentAt)
                            .FirstOrDefault()?.Content ?? ""))
                    .ToList();

                return Result<List<GetMyConversationsResponse>>
                    .Success("Success", response);
            }
        }

        public record GetMyConversationsResponse(
            Guid ConversationId,
            string Title,
            bool IsHandedOffToHuman,
            DateTime LastMessageAt,
            int TotalMessages,
            string LastMessage);
    }
}