using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetCategoryById
    {
        public record GetCategoryByIdQuery(Guid Id)
            : IRequest<Result<GetCategoryByIdResponse>>;

        public class GetCategoryByIdValidator
            : AbstractValidator<GetCategoryByIdQuery>
        {
            public GetCategoryByIdValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Category ID is required");
            }
        }

        public class GetCategoryByIdHandler(
            ICategoryRepository categoryRepository)
            : IRequestHandler<GetCategoryByIdQuery,
                Result<GetCategoryByIdResponse>>
        {
            public async Task<Result<GetCategoryByIdResponse>> Handle(
                GetCategoryByIdQuery request,
                CancellationToken cancellationToken)
            {
                var category = await categoryRepository.GetByIdAsync(request.Id);
                if (category is null)
                    return Result<GetCategoryByIdResponse>.Failure(
                        "Category not found");

                return Result<GetCategoryByIdResponse>.Success(
                    "Success", category.Adapt<GetCategoryByIdResponse>());
            }
        }

        public record GetCategoryByIdResponse(Guid Id, string Name);
    }
}