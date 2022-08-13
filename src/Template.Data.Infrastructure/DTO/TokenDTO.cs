using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Template.Data.Infrastructure.DTO;

public class TokenDTO
{
    [JsonPropertyName("refreshToken"), Required]
    public string RefreshToken { get; set; }
}