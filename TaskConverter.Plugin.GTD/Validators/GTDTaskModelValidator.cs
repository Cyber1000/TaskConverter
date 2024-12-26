using FluentValidation;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Validators;

public class GTDTaskModelValidator : GTDBaseModelValidator<GTDTaskModel>
{
    public GTDTaskModelValidator()
    {
        RuleFor(x => x.StartDate).Null().WithMessage("'Start Date' not implemented.");
        RuleFor(x => x.StartTimeSet).Equal(false).WithMessage("'Start Time Set' not implemented.");
        RuleFor(x => x.DueDateModifier).Must(x => x == DueDateModifier.DueBy || x == DueDateModifier.OptionallyOn).WithMessage(v => $"'Due date' {v.DueDateModifier} not implemented.");
        RuleFor(x => x.Duration).Equal(0).WithMessage("'Duration' not implemented.");
        RuleFor(x => x.Goal).Equal(0).WithMessage("'Goal' not implemented.");
        RuleFor(x => x.TrashBin).Empty().WithMessage("'TrashBin' not implemented.");
        RuleFor(x => x.Importance).Equal(0).WithMessage("'Importance' not implemented.");
        RuleFor(x => x.MetaInformation).Empty().WithMessage("'MetaInformation' not implemented.");
        RuleFor(x => x.Hide).Must(x => x == Hide.DontHide || x == Hide.SixMonthsBeforeDue).WithMessage(v => $"'Hide' not implemented with value {v.Hide}.");
    }
}
