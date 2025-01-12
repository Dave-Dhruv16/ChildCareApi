using ChildCareApi.Models;
using FluentValidation;

public class TutorValidator : AbstractValidator<Tutor>
{
    public TutorValidator()
    {
        RuleFor(tutor => tutor.UserId)
            .GreaterThan(0).WithMessage("UserId must be a valid positive number.");

        RuleFor(tutor => tutor.Qualifications)
            .NotEmpty().WithMessage("Qualifications are required.")
            .MaximumLength(255).WithMessage("Qualifications cannot exceed 255 characters.");

        RuleFor(tutor => tutor.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");

        RuleFor(tutor => tutor.ExperienceYears)
            .GreaterThanOrEqualTo(0).WithMessage("ExperienceYears must be zero or a positive number.");
    }
}
