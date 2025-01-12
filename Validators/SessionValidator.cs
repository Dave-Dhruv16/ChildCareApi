using ChildCareApi.Models;
using FluentValidation;

public class SessionValidator : AbstractValidator<Session>
{
    public SessionValidator()
    {
        RuleFor(session => session.RequestId)
            .GreaterThan(0).WithMessage("RequestId must be a valid positive number.");

        RuleFor(session => session.TutorId)
            .GreaterThan(0).WithMessage("TutorId must be a valid positive number.");

        RuleFor(session => session.StudentId)
            .GreaterThan(0).WithMessage("StudentId must be a valid positive number.");

        RuleFor(session => session.StartTime)
            .LessThan(session => session.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(session => session.LocationId)
            .GreaterThan(0).WithMessage("LocationId must be a valid positive number.");
    }
}
