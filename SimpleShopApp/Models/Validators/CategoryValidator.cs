using FluentValidation;

namespace SimpleShopApp.Models.Validators

{
    public class CategoryValidator : AbstractValidator<CategoryModel>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Category name could not be empty");
        }
    }
}
