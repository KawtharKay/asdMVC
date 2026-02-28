using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateMessage
    {
        public record CreateMessageCommand(Guid ConversationId, Guid SenderId, string? Content, string? AttachmentUrl) : IRequest<Result<CreateMessageResponse>>;

        public class CreateMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateMessageCommand, Result<CreateMessageResponse>>
        {
            public async Task<Result<CreateMessageResponse>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
            {
                var message = new Message
                {
                    ConversationId = request.ConversationId,
                    SenderId = request.SenderId,
                    Content = request.Content,
                    AttachmentUrl = request.AttachmentUrl,
                    IsRead = false,
                    SentAt = DateTime.UtcNow
                };

                await messageRepository.AddAsync(message);
                await unitOfWork.SaveAsync();

                return Result<CreateMessageResponse>.Success("Success!", message.Adapt<CreateMessageResponse>());
            }
        }

        public record CreateMessageResponse(Guid Id);
    }
}