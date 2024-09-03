using FluentValidation;
using Messenger.Business.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Messenger.Business.Validators;

public class ImageValidator : AbstractValidator<IFormFile>
{
    private readonly List<string> _supportedFormats;

    public ImageValidator(IOptions<ImageFormatsSettings> imageServiceSettings)
    {
        _supportedFormats = imageServiceSettings.Value.SupportedImageFormats;

        RuleFor(image => image)
            .NotNull()
            .WithMessage("Image must not be null.")
            .Must(image => image.Length > 0)
            .WithMessage("Image must not be empty.");

        RuleFor(image => Path.GetExtension(image.FileName).ToLowerInvariant())
            .Must(extension => _supportedFormats.Contains(extension))
            .WithMessage(image => $"Unsupported image format. Supported formats are: {string.Join(", ", _supportedFormats)}.");
    }
}
