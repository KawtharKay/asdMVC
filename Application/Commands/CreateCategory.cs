using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateCategory
    {
        public record CreateCategoryCommand(
            string Name) : IRequest<Result<CreateCategoryResponse>>;

        public class CreateCategoryValidator
            : AbstractValidator<CreateCategoryCommand>
        {
            public CreateCategoryValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Category name is required")
                    .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");
            }
        }

        public class CreateCategoryHandler(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
        {
            public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request,CancellationToken cancellationToken)
            {
                var exists = await categoryRepository.IsExistAsync(request.Name);
                if (exists)
                    return Result<CreateCategoryResponse>.Failure("Category already exists");

                var category = new Category
                {
                    Name = request.Name,
                    CreatedBy = "Admin"
                };

                await categoryRepository.AddAsync(category);
                await unitOfWork.SaveAsync();

                return Result<CreateCategoryResponse>.Success("Category created successfully!",category.Adapt<CreateCategoryResponse>());
            }
        }

        public record CreateCategoryResponse(Guid Id, string Name);
    }
}