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

    public async Task SendForgotPasswordEmailAsync(string toEmail, string verificationCode)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, "Red Ribbon Life");
        var subject = "Khôi phục mật khẩu Red Ribbon Life";
        var to = new EmailAddress(toEmail);

        var plainTextContent = $@"
Chào bạn,

Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản Red Ribbon Life của bạn.
Để đặt lại mật khẩu, vui lòng sử dụng mã 6 số sau:

{verificationCode}

Mã này có hiệu lực trong 15 phút.

Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này và mật khẩu của bạn sẽ không thay đổi.

Trân trọng,
Đội ngũ Red Ribbon Life";

        var htmlContent = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <h2 style='color: #dc3545;'>Red Ribbon Life - Khôi phục mật khẩu</h2>
    <p>Chào bạn,</p>
    <p>Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản Red Ribbon Life của bạn. Để đặt lại mật khẩu, vui lòng sử dụng mã 6 số sau:</p>
    <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
        <h1 style='color: #dc3545; font-size: 32px; margin: 0; letter-spacing: 5px;'>{verificationCode}</h1>
    </div>
    <p style='color: #6c757d; font-size: 14px;'>Mã này có hiệu lực trong 15 phút.</p>
    <p>Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này và mật khẩu của bạn sẽ không thay đổi.</p>
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
            throw new Exception($"Failed to send forgot password email: {response.StatusCode}");
        }
    }
    
    public async Task SendAppointmentReminderEmailAsync(string toEmail, string patientName, DateTime appointmentDateTime)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, "Red Ribbon Life");
        var subject = "Nhắc nhở lịch hẹn - Red Ribbon Life";
        var to = new EmailAddress(toEmail);

        var plainTextContent = $@"
Xin chào {patientName},

Đây là thông báo nhắc nhở về lịch hẹn sắp tới của bạn:

Thời gian: {appointmentDateTime:dd/MM/yyyy HH:mm}

Vui lòng đến đúng giờ và mang theo các giấy tờ cần thiết.

Trân trọng,
Đội ngũ Red Ribbon Life";

        var htmlContent = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <h2 style='color: #dc3545;'>Red Ribbon Life - Nhắc nhở lịch hẹn</h2>
    <p>Xin chào <strong>{patientName}</strong>,</p>
    <p>Đây là thông báo nhắc nhở về lịch hẹn sắp tới của bạn:</p>
    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
        <p><strong>Thời gian:</strong> {appointmentDateTime:dd/MM/yyyy HH:mm}</p>
    </div>
    <p>Vui lòng đến đúng giờ và mang theo các giấy tờ cần thiết.</p>
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
            throw new Exception($"Failed to send appointment reminder email: {response.StatusCode}");
        }
    }

    public async Task SendMedicationReminderEmailAsync(string toEmail, string patientName, string regimenName, string instructions, int frequency)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, "Red Ribbon Life");
        var subject = "Nhắc nhở uống thuốc - Red Ribbon Life";
        var to = new EmailAddress(toEmail);

        var frequencyText = frequency == 1 ? "1 lần/ngày" : "2 lần/ngày";

        var plainTextContent = $@"
Xin chào {patientName},

Đây là thông báo nhắc nhở uống thuốc của bạn:

Phác đồ: {regimenName}
Tần suất: {frequencyText}
Hướng dẫn: {instructions}

Vui lòng uống thuốc đúng giờ và theo đúng chỉ dẫn của bác sĩ.

Trân trọng,
Đội ngũ Red Ribbon Life";

        var htmlContent = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <h2 style='color: #dc3545;'>Red Ribbon Life - Nhắc nhở uống thuốc</h2>
    <p>Xin chào <strong>{patientName}</strong>,</p>
    <p>Đây là thông báo nhắc nhở uống thuốc của bạn:</p>
    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
        <p><strong>Phác đồ:</strong> {regimenName}</p>
        <p><strong>Tần suất:</strong> {frequencyText}</p>
        <p><strong>Hướng dẫn:</strong> {instructions}</p>
    </div>
    <p>Vui lòng uống thuốc đúng giờ và theo đúng chỉ dẫn của bác sĩ.</p>
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
            throw new Exception($"Failed to send medication reminder email: {response.StatusCode}");
        }
    }

    public async Task SendAppointmentApprovalEmailAsync(string toEmail, string patientName, DateOnly appointmentDate, TimeOnly appointmentTime)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, "Red Ribbon Life");
        var subject = "Yêu cầu đặt lịch khám đã được duyệt - Red Ribbon Life";
        var to = new EmailAddress(toEmail);

        // Format date and time in Vietnamese format
        var formattedDate = appointmentDate.ToString("dd/MM/yyyy");
        var formattedTime = appointmentTime.ToString("HH:mm");

        var plainTextContent = $@"
Xin chào {patientName},

Yêu cầu đặt lịch khám của bạn đã được duyệt!

Thông tin lịch khám:
- Ngày: {formattedDate}
- Giờ: {formattedTime}

Vui lòng đến đúng giờ và mang theo các giấy tờ cần thiết như chứng minh nhân dân, thẻ bảo hiểm y tế (nếu có).

Nếu có bất kỳ thay đổi nào, vui lòng liên hệ với chúng tôi sớm nhất có thể.

Trân trọng,
Đội ngũ Red Ribbon Life";

        var htmlContent = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <h2 style='color: #dc3545;'>Red Ribbon Life - Xác nhận lịch khám</h2>
    <p>Xin chào <strong>{patientName}</strong>,</p>
    <div style='background-color: #d4edda; border: 1px solid #c3e6cb; border-radius: 5px; padding: 15px; margin: 20px 0;'>
        <h3 style='color: #155724; margin: 0 0 10px 0;'>Yêu cầu đặt lịch khám của bạn đã được duyệt!</h3>
    </div>
    <p><strong>Thông tin lịch khám:</strong></p>
    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
        <p style='margin: 5px 0;'><strong>Ngày:</strong> {formattedDate}</p>
        <p style='margin: 5px 0;'><strong>Giờ:</strong> {formattedTime}</p>
    </div>
    <p><strong>Lưu ý quan trọng:</strong></p>
    <ul style='color: #6c757d;'>
        <li>Vui lòng đến đúng giờ</li>
        <li>Mang theo chứng minh nhân dân</li>
        <li>Mang theo thẻ bảo hiểm y tế (nếu có)</li>
        <li>Nếu có thay đổi, vui lòng liên hệ sớm nhất</li>
    </ul>
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
            throw new Exception($"Failed to send appointment approval email: {response.StatusCode}");
        }
    }
}