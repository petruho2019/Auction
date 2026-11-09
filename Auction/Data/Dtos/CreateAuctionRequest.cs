using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AuctionApi.Data.Dtos;

/// <summary>Данные для создания аукциона.</summary>
public sealed record CreateAuctionRequest : IValidatableObject
{
    /// <summary>Название товара, выставленного на аукцион.</summary>
    [Required(ErrorMessage = "Укажите название товара")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Название товара должно содержать от 1 до 200 символов")]
    public string? ProductName { get; init; }

    /// <summary>Дата и время начала аукциона.</summary>
    [Required(ErrorMessage = "Укажите дату начала аукциона")]
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>Дата и время окончания аукциона.</summary>
    [Required(ErrorMessage = "Укажите дату окончания аукциона")]
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>Необязательное изображение товара.</summary>
    [AllowedImageExtensions(ErrorMessage = "Допустимые форматы фото: .jpg, .jpeg, .png, .webp")]
    [MaximumFileSize(10 * 1024 * 1024, ErrorMessage = "Размер фото не должен превышать 10 МБ")]
    public IFormFile? Photo { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            yield return new ValidationResult(
                "Название товара обязательно.",
                [nameof(ProductName)]);
        }

        if (StartsAt is not null && EndsAt is not null && EndsAt <= StartsAt)
        {
            yield return new ValidationResult(
                "Дата окончания должна быть позже даты начала.",
                [nameof(EndsAt)]);
        }
    }
}

public sealed class AllowedImageExtensionsAttribute : ValidationAttribute
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    public override bool IsValid(object? value) =>
        value is null
        || value is IFormFile file
            && (file.Length == 0
                || !string.IsNullOrWhiteSpace(file.FileName)
                    && AllowedExtensions.Contains(Path.GetExtension(file.FileName)));
}

public sealed class MaximumFileSizeAttribute(long maximumSizeInBytes) : ValidationAttribute
{
    public override bool IsValid(object? value) =>
        value is null || value is IFormFile file && file.Length <= maximumSizeInBytes;
}
