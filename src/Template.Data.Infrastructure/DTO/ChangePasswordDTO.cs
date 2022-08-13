using System.ComponentModel.DataAnnotations;

namespace Template.Data.Infrastructure.DTO;

public class ChangePasswordDTO
{
    [Required, DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required, StringLength(100, MinimumLength = 4), DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; }
}