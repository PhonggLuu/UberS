using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using UberSystem.Domain.Interfaces.Services;

namespace UberSystem.Service;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly IConfiguration _configuration;

    public EmailService(SmtpClient smtpClient, IConfiguration configuration)
    {
        _smtpClient = smtpClient;
        _configuration = configuration;
    }
	/*
    public async Task SendVerificationEmailAsync(string email, string verificationLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("noreply-UberSystem", "phonglgse160315@fpt.edu.vn"));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Verify your email";

        // Email body (plain text)
        message.Body = new TextPart("plain")
        {
            Text = $"Please verify your email by clicking on this link: {verificationLink}"
        };
        await _smtpClient.SendAsync(message);
    }*/
	public async Task SendVerificationEmailAsync(string email, string token)
	{
        var url = _configuration["VerifyLink"] + token;
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("noreply-UberSystem", "phonglgse160315@fpt.edu.vn"));
		message.To.Add(new MailboxAddress("", email));
		message.Subject = "Verify your email";

		// Email body (plain text)
		message.Body = new TextPart("plain")
		{
			Text = $"Token: {url}"
		};
		await _smtpClient.SendAsync(message);
	}
}