using Application.Common;
using Application.Repositories;
using MediatR;

namespace Application.Commands
{
    public class CreateMessage
    {
        public record CreateMessageCommand() : IRequest<Result<CreateMessageResponse>>;
        public class CreateMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateMessageCommand, Result<CreateMessageResponse>>
        {
            public Task<Result<CreateMessageResponse>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public record CreateMessageResponse();
    }
}
