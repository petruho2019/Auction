using System.ComponentModel.DataAnnotations;

namespace Data.Dtos;

public sealed record LoginRequestDto(
    [property: Required(ErrorMessage = "Укажите адрес электронной почты")]
    [property: EmailAddress(ErrorMessage = "Укажите адрес электронной почты в правильном формате")]
    [property: StringLength(320, ErrorMessage = "Адрес электронной почты не должен превышать 320 символов")]
    string Email,
    [property: Required(ErrorMessage = "Укажите пароль")]
    string Password);
