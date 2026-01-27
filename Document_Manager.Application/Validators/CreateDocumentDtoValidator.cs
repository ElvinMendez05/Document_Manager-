using Document_Manager.Application.DTOs;
using FluentValidation;

namespace Document_Manager.Application.Validators
{
    public class CreateDocumentDtoValidator : AbstractValidator<CreateDocumentDto>
    {
        public CreateDocumentDtoValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.FilePath)
                .NotEmpty();
        }
    }
}
