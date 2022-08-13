namespace Template.AuthenticationAPI.Interfaces;

public interface ISecurityService
{
    string GetSha256Hash(string input);
    Guid CreateCryptographicallySecureGuid();
}