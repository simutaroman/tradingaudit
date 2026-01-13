using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Services;

public class AzureEmailSender : IEmailSender<ApplicationUser>
{
    private readonly string _connectionString;
    private readonly string _senderAddress;
    private readonly ILogger<AzureEmailSender> _logger;
    private readonly IConfiguration _configuration;

    public AzureEmailSender(IConfiguration configuration, ILogger<AzureEmailSender> logger)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionString = configuration["AzureEmail:ConnectionString"]
                            ?? throw new Exception("AzureEmail connection string missing");
        _senderAddress = configuration["AzureEmail:SenderAddress"]
                         ?? throw new Exception("AzureEmail sender address missing");
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        // 1. Отримуємо базову адресу клієнта з конфігурації
        var clientBaseUrl = _configuration["ClientApp:BaseUrl"];
        var cleanLink = System.Net.WebUtility.HtmlDecode(confirmationLink);
        // 2. Парсимо параметри з оригінального посилання (userId і code)
        var uri = new Uri(cleanLink);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        var userId = query["userId"];
        var code = query["code"]; // Цей код вже закодований для URL

        Console.WriteLine($"code {code}");
        // 3. Формуємо НОВЕ посилання на сторінку Blazor
        // Важливо: code треба передати коректно.
        var newLink = $"{clientBaseUrl}/confirm-email?userId={userId}&code={System.Net.WebUtility.UrlEncode(code)}";

        var htmlContent = $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px;'>
                        <h2 style='color: #2d3e50;'>Вітаємо в TradingAudit!</h2>
                        <p>Дякуємо за реєстрацію, <strong>{user.Nickname ?? "Трейдер"}</strong>.</p>
                        <p>Щоб активувати всі функції, підтверди свою пошту:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{newLink}' style='padding: 12px 24px; background-color: #594ae2; color: white; text-decoration: none; border-radius: 4px; font-weight: bold;'>Підтвердити Email</a>
                        </div>
                        <p style='font-size: 12px; color: #999;'>Якщо кнопка не працює, скопіюй посилання:<br>{newLink}</p>
                    </div>
                </div>
            </body>
        </html>";

        await SendEmailAsync(email, "Підтвердження реєстрації", htmlContent);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var htmlContent = $"<a href='{resetLink}'>Натисни тут для скидання пароля</a>";
        await SendEmailAsync(email, "Скидання пароля", htmlContent);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        await SendEmailAsync(email, "Код скидання пароля", $"Ваш код: {resetCode}");
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        try
        {
            var client = new EmailClient(_connectionString);

            var emailMessage = new EmailMessage(
                senderAddress: _senderAddress,
                content: new EmailContent(subject) { Html = htmlContent },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(toEmail) }));

            EmailSendOperation emailSendOperation = await client.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation($"Email sent. Status: {emailSendOperation.Value.Status}, Id: {emailSendOperation.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email via Azure ACS to {toEmail}");
            throw;
        }
    }
}