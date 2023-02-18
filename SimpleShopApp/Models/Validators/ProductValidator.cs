using FluentValidation;

namespace SimpleShopApp.Models.Validators
{
    public class ProductValidator : AbstractValidator<ProductModel>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Product name could not be empty");
            RuleFor(p => p.Price).NotEmpty().WithMessage("Price could not be empty")
                .GreaterThan(0).WithMessage("Product price must be positive number");
            RuleFor(p => p.Quantity).NotEmpty().WithMessage("Quantity could not be empty")
                .GreaterThan(0).WithMessage("Quantity must be positive number");
        }
    }
}
