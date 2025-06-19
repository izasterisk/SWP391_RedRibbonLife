// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BLL.Utils;

public class SendGridEmailUtil
{
    private readonly string _apiKey;
    private readonly string _fromEmail;

    public SendGridEmailUtil(IConfiguration configuration)
    {
        _apiKey = configuration["SENDGRID_API_KEY"];
        _fromEmail = configuration["FROM_EMAIL"];
        
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("SendGrid API key is not set in configuration.");
        }
        
        if (string.IsNullOrEmpty(_fromEmail))
        {
            throw new InvalidOperationException("From email is not set in configuration.");
        }
    }

    public async Task SendVerificationEmailAsync(string toEmail, string verificationCode)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, "Red Ribbon Life");
        var subject = "Xác thực tài khoản Red Ribbon Life";
        var to = new EmailAddress(toEmail);

        var plainTextContent = $@"
Chào bạn,

Cảm ơn bạn đã đăng ký tài khoản với Red Ribbon Life - Cơ sở điều trị HIV. 
Để hoàn tất việc xác thực tài khoản, vui lòng sử dụng mã 6 số sau:

{verificationCode}

Mã này có hiệu lực trong 15 phút.

Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.

Trân trọng,
Đội ngũ Red Ribbon Life";

        var htmlContent = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <h2 style='color: #dc3545;'>Red Ribbon Life - Cơ sở điều trị HIV</h2>
    <p>Chào bạn,</p>
    <p>Cảm ơn bạn đã đăng ký tài khoản với Red Ribbon Life. Để hoàn tất việc xác thực tài khoản, vui lòng sử dụng mã 6 số sau:</p>
    <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
        <h1 style='color: #dc3545; font-size: 32px; margin: 0; letter-spacing: 5px;'>{verificationCode}</h1>
    </div>
    <p style='color: #6c757d; font-size: 14px;'>Mã này có hiệu lực trong 15 phút.</p>
    <p>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>
    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;'>
    <p style='color: #6c757d; font-size: 12px;'>
        Trân trọng,<br>
        <strong>Đội ngũ Red Ribbon Life</strong>
    </p>
</div>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new Exception($"Failed to send email: {response.StatusCode}");
        }
    }
}