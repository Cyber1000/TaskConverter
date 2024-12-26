using FluentValidation;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Validators;

public class GTDDataModelValidator : AbstractValidator<GTDDataModel>
{
    public GTDDataModelValidator()
    {
        RuleFor(x => x.Preferences).NotEmpty();
        RuleFor(x => x.TaskNote).Null().WithMessage("'TaskNote' not implemented.");
        RuleForEach(x => x.GetAllEntries)
            .SetInheritanceValidator(v =>
            {
                v.Add(new GTDBaseModelValidator<GTDFolderModel>());
                v.Add(new GTDBaseModelValidator<GTDTagModel>());
                v.Add(new GTDTaskModelValidator());
                v.Add(new GTDBaseModelValidator<GTDContextModel>());
                v.Add(new GTDBaseModelValidator<GTDNotebookModel>());
            });
    }
}
