using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateCategory
    {
        public record CreateCategoryCommand(string Name) : IRequest<Result<CreateCategoryResponse>>;

        public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
        {
            private readonly ICategoryRepository _categoryRepository;
            private readonly IUnitOfWork _unitOfWork;
            public CreateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
            {
                _categoryRepository = categoryRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                var categoryExist = await _categoryRepository.GetCategoryAsync(request.Name);
                if (categoryExist is null) throw new Exception("Category already exists");

                var category = new Category
                {
                    Name = request.Name
                };

                await _categoryRepository.AddCategoryAsync(category);
                await _unitOfWork.SaveAsync();

                return Result<CreateCategoryResponse>.Success("Success!", category.Adapt<CreateCategoryResponse>());
            }
        }

        public record CreateCategoryResponse(Guid Id, string Name);
    }
}