using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetAllCategory
    {
        public record GetAllCategoryQuery() : IRequest<Result<IEnumerable<GetAllCategoryResponse>>>;

        public class GetAllCategoryHandler : IRequestHandler<GetAllCategoryQuery, Result<IEnumerable<GetAllCategoryResponse>>>
        {
            private readonly ICategoryRepository _categoryRepository;
            public GetAllCategoryHandler(ICategoryRepository categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }
            public async Task<Result<IEnumerable<GetAllCategoryResponse>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
            {
                var response = await _categoryRepository.GetAllCategoriesAsync();
                return Result<IEnumerable<GetAllCategoryResponse>>.Success("Success!", response.Adapt<IEnumerable<GetAllCategoryResponse>>());
            }
        }

        public record GetAllCategoryResponse(Guid Id, string Name);
    }
}