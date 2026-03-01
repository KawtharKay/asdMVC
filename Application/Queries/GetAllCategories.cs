using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetAllCategories
    {
        public record GetAllCategoriesQuery()
            : IRequest<Result<List<GetAllCategoriesResponse>>>;

        public class GetAllCategoriesHandler(
            ICategoryRepository categoryRepository)
            : IRequestHandler<GetAllCategoriesQuery,
                Result<List<GetAllCategoriesResponse>>>
        {
            public async Task<Result<List<GetAllCategoriesResponse>>> Handle(
                GetAllCategoriesQuery request,
                CancellationToken cancellationToken)
            {
                var categories = await categoryRepository.GetAllAsync();
                return Result<List<GetAllCategoriesResponse>>.Success(
                    "Success",
                    categories.Adapt<List<GetAllCategoriesResponse>>());
            }
        }

        public record GetAllCategoriesResponse(Guid Id, string Name);
    }
}