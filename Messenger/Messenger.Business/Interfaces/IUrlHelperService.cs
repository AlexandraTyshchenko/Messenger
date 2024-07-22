namespace Messenger.Business.Interfaces;

public interface IUrlHelperService
{
    string GenerateEmailConfirmationLink(string email, string token);
}
