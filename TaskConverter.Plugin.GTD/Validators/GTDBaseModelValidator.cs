using FluentValidation;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Validators;

public class GTDBaseModelValidator<T> : AbstractValidator<T>
    where T : GTDBaseModel
{
    public GTDBaseModelValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Uuid).Empty().WithMessage("Only empty Uuids are accepted.");
    }
}
