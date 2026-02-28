using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetMessages
    {
        public record GetMessagesQuery(Guid SenderId) : IRequest<Result<ICollection<GetMessagesResponse>>>;

        public class GetMessagesHandler(IMessageRepository messageRepository) : IRequestHandler<GetMessagesQuery, Result<ICollection<GetMessagesResponse>>>
        {
            public async Task<Result<ICollection<GetMessagesResponse>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
            {
                var messages = await messageRepository.GetAllAsync(request.SenderId);
                return Result<ICollection<GetMessagesResponse>>.Success("Success!", messages.Adapt<ICollection<GetMessagesResponse>>());
            }
        }

        public record GetMessagesResponse(Guid Id, Guid ConversationId, Guid SenderId, string? Content, bool IsRead, DateTime SentAt, DateTime? ReadAt, string? AttachmentUrl);
    }
}