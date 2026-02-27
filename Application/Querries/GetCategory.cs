using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetCategory
    {
        public record GetCategoryQuery(Guid Id) : IRequest<Result<GetCategoryResponse>>;

        public class GetCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryQuery, Result<GetCategoryResponse>>
        {
            public async Task<Result<GetCategoryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
            {
                var category = await categoryRepository.GetCategoryByIdAsync(request.Id);
                if (category is null) throw new Exception("Category not found");

                return Result<GetCategoryResponse>.Success("Success!", category.Adapt<GetCategoryResponse>());
            }
        }

        public record GetCategoryResponse(Guid Id, string Name);
    }
}