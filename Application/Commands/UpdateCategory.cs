using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class UpdateCategory
    {
        public record UpdateCategoryCommand(Guid Id, string Name) : IRequest<Result<UpdateCategoryResponse>>;

        public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
        {
            private readonly ICategoryRepository _categoryRepository;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
            {
                _categoryRepository = categoryRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<Result<UpdateCategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
                if (category is null) throw new Exception("Category not found");

                var categoryExists = await _categoryRepository.GetCategoryAsync(request.Name);
                if (categoryExists is null) throw new Exception("Category name exist");

                category.Name = request.Name;

                _categoryRepository.Update(category);
                await _unitOfWork.SaveAsync();

                return Result<UpdateCategoryResponse>.Success("Success!", category.Adapt<UpdateCategoryResponse>());
            }
        }

        public record UpdateCategoryResponse(Guid Id, string Name);
    }
}