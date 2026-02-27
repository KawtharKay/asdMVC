using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetAllConversations
    {
        public record GetAllConversationsQuery() : IRequest<Result<IEnumerable<GetAllConversationsResponse>>>;

        public class GetAllConversationsHandler : IRequestHandler<GetAllConversationsQuery, Result<IEnumerable<GetAllConversationsResponse>>>
        {
            private readonly IConversationRepository _conversationRepository;
            public GetAllConversationsHandler(IConversationRepository conversationRepository)
            {
                _conversationRepository = conversationRepository;
            }
            public async Task<Result<IEnumerable<GetAllConversationsResponse>>> Handle(GetAllConversationsQuery request, CancellationToken cancellationToken)
            {
                var response = await _conversationRepository.GetAllConversationsAsync();
                return Result<IEnumerable<GetAllConversationsResponse>>.Success("Success!", response.Adapt<IEnumerable<GetAllConversationsResponse>>());
            }
        }

        public record GetAllConversationsResponse(Guid Id, string Title, DateTime LastMessageAt);
    }
}