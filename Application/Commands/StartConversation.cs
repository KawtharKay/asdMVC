using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class StartConversation
    {
        public record StartConversationCommand(
            Guid UserId,
            string Title = "Support Chat")
            : IRequest<Result<StartConversationResponse>>;

        public class StartConversationValidator
            : AbstractValidator<StartConversationCommand>
        {
            public StartConversationValidator()
            {
                RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID is required");

                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Title is required")
                    .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
            }
        }

        public class StartConversationHandler(
            IUserRepository userRepository,
            IConversationRepository conversationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<StartConversationCommand,
                Result<StartConversationResponse>>
        {
            public async Task<Result<StartConversationResponse>> Handle(
                StartConversationCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetAsync(request.UserId);
                if (user is null)
                    return Result<StartConversationResponse>.Failure("User not found");

                var conversation = new Conversation
                {
                    Title = request.Title,
                    IsHandedOffToHuman = false,
                    LastMessageAt = DateTime.UtcNow,
                    CreatedBy = user.Email
                };

                conversation.UserConversations.Add(new UserConversation
                {
                    UserId = request.UserId,
                    IsAdmin = false
                });

                await conversationRepository.AddAsync(conversation);
                await unitOfWork.SaveAsync();

                return Result<StartConversationResponse>.Success(
                    "Conversation started!",
                    new StartConversationResponse(conversation.Id));
            }
        }

        public record StartConversationResponse(Guid ConversationId);
    }
}