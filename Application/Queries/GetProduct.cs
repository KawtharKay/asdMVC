using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetProductById
    {
        public record GetProductByIdQuery(Guid Id)
            : IRequest<Result<GetProductByIdResponse>>;

        public class GetProductByIdValidator
            : AbstractValidator<GetProductByIdQuery>
        {
            public GetProductByIdValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Product ID is required");
            }
        }

        public class GetProductByIdHandler(
            IProductRepository productRepository)
            : IRequestHandler<GetProductByIdQuery,
                Result<GetProductByIdResponse>>
        {
            public async Task<Result<GetProductByIdResponse>> Handle(
                GetProductByIdQuery request,
                CancellationToken cancellationToken)
            {
                var p = await productRepository.GetByIdAsync(request.Id);
                if (p is null)
                    return Result<GetProductByIdResponse>.Failure("Product not found");

                return Result<GetProductByIdResponse>.Success(
                    "Success",
                    new GetProductByIdResponse(
                        p.Id, p.Name, p.Sku,
                        p.Price, p.ImageUrl,
                        p.QrCodeImagePath,
                        p.StockQuantity,
                        p.IsInStock,
                        p.IsLowStock,
                        p.Category.Name,
                        p.LowStockThreshold));
            }
        }

        public record GetProductByIdResponse(
            Guid Id,
            string Name,
            string Sku,
            decimal Price,
            string ImageUrl,
            string? QrCodeImagePath,
            int StockQuantity,
            bool IsInStock,
            bool IsLowStock,
            string CategoryName,
            int LowStockThreshold);
    }
}