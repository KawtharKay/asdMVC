using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class SendMessage
    {
        public record SendMessageCommand(
            Guid ConversationId,
            Guid SenderId,
            string Content)
            : IRequest<Result<SendMessageResponse>>;

        public class SendMessageValidator
            : AbstractValidator<SendMessageCommand>
        {
            public SendMessageValidator()
            {
                RuleFor(x => x.ConversationId)
                    .NotEmpty().WithMessage("Conversation ID is required");

                RuleFor(x => x.SenderId)
                    .NotEmpty().WithMessage("Sender ID is required");

                RuleFor(x => x.Content)
                    .NotEmpty().WithMessage("Message cannot be empty")
                    .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters");
            }
        }

        public class SendMessageHandler(
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository,
            IGeminiService geminiService,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
        {
            public async Task<Result<SendMessageResponse>> Handle(
                SendMessageCommand request,
                CancellationToken cancellationToken)
            {
                var conversation = await conversationRepository
                    .GetByIdAsync(request.ConversationId);

                if (conversation is null)
                    return Result<SendMessageResponse>.Failure(
                        "Conversation not found");

                // Save customer message
                var message = new Message
                {
                    ConversationId = request.ConversationId,
                    SenderId = request.SenderId,
                    Content = request.Content,
                    SentAt = DateTime.UtcNow,
                    CreatedBy = request.SenderId.ToString()
                };

                await messageRepository.AddAsync(message);
                conversation.LastMessageAt = DateTime.UtcNow;
                await unitOfWork.SaveAsync();

                // Route to AI if not handed off
                if (!conversation.IsHandedOffToHuman)
                {
                    var history = conversation.Messages
                        .OrderBy(m => m.SentAt)
                        .Select(m => new ChatHistoryItem
                        {
                            Role = m.SenderId == request.SenderId ? "user" : "model",
                            Content = m.Content ?? string.Empty
                        }).ToList();

                    var aiResponse = await geminiService
                        .GetResponseAsync(request.Content, history);

                    await messageRepository.AddAsync(new Message
                    {
                        ConversationId = request.ConversationId,
                        SenderId = Guid.Empty,
                        Content = aiResponse,
                        SentAt = DateTime.UtcNow,
                        CreatedBy = "AI"
                    });

                    conversation.LastMessageAt = DateTime.UtcNow;
                    await unitOfWork.SaveAsync();

                    return Result<SendMessageResponse>.Success(
                        "Message sent",
                        new SendMessageResponse(aiResponse, true));
                }

                // Notify admin if handed off
                var adminParticipant = conversation.UserConversations
                    .FirstOrDefault(uc => uc.IsAdmin);

                if (adminParticipant is not null)
                {
                    await notificationRepository.AddAsync(new Notification
                    {
                        UserId = adminParticipant.UserId,
                        Title = "New Support Message",
                        Message = $"New message in: {conversation.Title}",
                        Type = Domain.Enums.NotificationType.Others,
                        Ref = $"/chat/{conversation.Id}",
                        CreatedBy = request.SenderId.ToString()
                    });

                    await unitOfWork.SaveAsync();
                }

                return Result<SendMessageResponse>.Success(
                    "Message sent", new SendMessageResponse(null, false));
            }
        }

        public record SendMessageResponse(
            string? AiResponse, bool IsAiResponse);
    }
}