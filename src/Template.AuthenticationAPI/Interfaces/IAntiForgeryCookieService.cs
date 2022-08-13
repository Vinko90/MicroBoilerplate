using System.Security.Claims;

namespace Template.AuthenticationAPI.Interfaces;

public interface IAntiForgeryCookieService
{
    void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims);
    void DeleteAntiForgeryCookies();
}