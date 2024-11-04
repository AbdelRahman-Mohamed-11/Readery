using Microsoft.Extensions.Configuration;
using Readery.Utilities.EmailModels;
using Readery.Utilities.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Readery.Utilities.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    public async Task SendEmail(EmailMessage emailMessage)
    {
        var client = new SendGridClient(config["SendGrid:sendGridKey"]);

        var msg = new SendGridMessage();

        msg.SetFrom(
            new SendGrid.Helpers.Mail.EmailAddress("belalmohamed5350@gmail.com", "MyBooks Application"));

        msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(emailMessage.ToAddress, emailMessage.Name));

        msg.SetSubject(emailMessage.Subject);

        // Beautifully styled email content
        msg.AddContent(MimeType.Html, $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <h2 style='color: #0078d4; text-align: center;'>Reset Your Password</h2>
                        <p style='font-size: 16px; color: #333;'>Dear {emailMessage.Name}</p>
                        <p style='font-size: 16px; color: #333;'>We received a request to reset your password. Click the button below to set a new password for your account:</p>

                        <div style='text-align: center; margin: 20px 0;'>
                            <a href={emailMessage.Body} style='background-color: #0078d4; color: #ffffff; padding: 10px 20px; text-decoration: none; font-size: 16px; border-radius: 5px;'>Reset Password</a>
                        </div>

                        <p style='font-size: 14px; color: #555;'>If you didn't request this, please ignore this email.</p>
                        <p style='font-size: 14px; color: #555;'>Thank you,</p>
                        <p style='font-size: 14px; color: #555;'><strong>The myBooks Team</strong></p>

                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;' />

                        <p style='font-size: 12px; color: #888; text-align: center;'>© {DateTime.Now.Year} MyBooks Application. All rights reserved.</p>
                    </div>
                </body>
                </html>");

        await client.SendEmailAsync(msg);
    }

}
