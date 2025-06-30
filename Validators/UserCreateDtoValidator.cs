using FluentValidation;
using UserManagementAPI.Dtos;
using UserManagementAPI.Models;

namespace UserManagementAPI.Validators
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot be longer than 100 characters.")
                .MinimumLength(6).WithMessage("Full name cannot be shorter than 6 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
