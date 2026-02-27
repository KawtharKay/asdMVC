using Application.Common;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateConversation
    {
        public record CreateConversationCommand(string Title) : IRequest<Result<CreateConversationResponse>>;

        public class CreateConversationHandler : IRequestHandler<CreateConversationCommand, Result<CreateConversationResponse>>
        {
            private readonly IConversationRepository _conversationRepository;
            private readonly IUnitOfWork _unitOfWork;
            public CreateConversationHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
            {
                _conversationRepository = conversationRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<Result<CreateConversationResponse>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
            {
                var conversation = new Conversation
                {
                    Title = request.Title,
                    LastMessageAt = DateTime.UtcNow
                };

                await _conversationRepository.AddToDbAsync(conversation);
                await _unitOfWork.SaveAsync();

                return Result<CreateConversationResponse>.Success("Success!", conversation.Adapt<CreateConversationResponse>());
            }
        }

        public record CreateConversationResponse(Guid Id, string Title, DateTime LastMessageAt);
    }
}