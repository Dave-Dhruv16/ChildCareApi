using ChildCareApi.Models;
using FluentValidation;

public class TutoringRequestValidator : AbstractValidator<TutoringRequest>
{
    public TutoringRequestValidator()
    {
        RuleFor(request => request.StudentId)
            .GreaterThan(0).WithMessage("StudentId must be a valid positive number.");

        RuleFor(request => request.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(100).WithMessage("Subject cannot exceed 100 characters.");

        RuleFor(request => request.Description)
            .MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");

        RuleFor(request => request.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => new[] { "Pending", "Accepted", "Rejected" }.Contains(status))
            .WithMessage("Status must be 'Pending', 'Accepted', or 'Rejected'.");

        RuleFor(request => request.TutorId)
            .GreaterThanOrEqualTo(0).WithMessage("TutorId must be zero or a positive number.");
    }
}
