using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class HandOffToHuman
    {
        public record HandOffToHumanCommand(
            Guid ConversationId,
            Guid AdminUserId)
            : IRequest<Result<string>>;

        public class HandOffToHumanValidator
            : AbstractValidator<HandOffToHumanCommand>
        {
            public HandOffToHumanValidator()
            {
                RuleFor(x => x.ConversationId)
                    .NotEmpty().WithMessage("Conversation ID is required");

                RuleFor(x => x.AdminUserId)
                    .NotEmpty().WithMessage("Admin user ID is required");
            }
        }

        public class HandOffToHumanHandler(
            IConversationRepository conversationRepository,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<HandOffToHumanCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                HandOffToHumanCommand request,
                CancellationToken cancellationToken)
            {
                var conversation = await conversationRepository
                    .GetByIdAsync(request.ConversationId);

                if (conversation is null)
                    return Result<string>.Failure("Conversation not found");

                if (conversation.IsHandedOffToHuman)
                    return Result<string>.Failure(
                        "Conversation already handed off to human");

                conversation.IsHandedOffToHuman = true;
                conversation.DateModified = DateTime.UtcNow;

                // Add admin as participant
                conversation.UserConversations.Add(new UserConversation
                {
                    UserId = request.AdminUserId,
                    ConversationId = request.ConversationId,
                    IsAdmin = true
                });

                // Notify customer
                var customerParticipant = conversation.UserConversations
                    .FirstOrDefault(uc => !uc.IsAdmin);

                if (customerParticipant is not null)
                {
                    await notificationRepository.AddAsync(new Notification
                    {
                        UserId = customerParticipant.UserId,
                        Title = "Support Agent Joined",
                        Message = "A support agent has joined your conversation",
                        Type = NotificationType.Others,
                        Ref = $"/chat/{conversation.Id}",
                        CreatedBy = request.AdminUserId.ToString()
                    });
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Conversation handed off to human agent!", "handedoff");
            }
        }
    }
}