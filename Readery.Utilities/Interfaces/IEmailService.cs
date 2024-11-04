using Readery.Utilities.EmailModels;


namespace Readery.Utilities.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmail(EmailMessage emailMessage);
    }
}
