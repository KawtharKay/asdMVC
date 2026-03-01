using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class DeleteProduct
    {
        public record DeleteProductCommand(Guid Id) : IRequest<Result<string>>;

        public class DeleteProductValidator
            : AbstractValidator<DeleteProductCommand>
        {
            public DeleteProductValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Product ID is required");
            }
        }

        public class DeleteProductHandler(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<DeleteProductCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                DeleteProductCommand request,
                CancellationToken cancellationToken)
            {
                var product = await productRepository.GetByIdAsync(request.Id);
                if (product is null)
                    return Result<string>.Failure("Product not found");

                product.IsDeleted = true;
                product.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Product deleted successfully!", "deleted");
            }
        }
    }
}