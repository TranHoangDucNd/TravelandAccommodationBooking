﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using TND.Domain.Interfaces.Services;
using TND.Domain.Models;

namespace TND.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }
        public async Task SendAsync(EmailRequest emailRequest, CancellationToken cancellationToken = default)
        {
            var email = CreateEmail(emailRequest);

            using var smtpClient = new SmtpClient();

            try
            {
                await smtpClient.ConnectAsync(
                  _emailConfig.Server,
                  _emailConfig.Port,
                  useSsl: true,
                  cancellationToken);

                await smtpClient.AuthenticateAsync(
                  _emailConfig.Username,
                  _emailConfig.Password,
                  cancellationToken);

                await smtpClient.SendAsync(email, cancellationToken);
            }
            finally
            {
                await smtpClient.DisconnectAsync(quit: true, cancellationToken);
            }
        }
        public MimeMessage CreateEmail(EmailRequest emailRequest)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.FromEmail));
            emailMessage.To.AddRange(emailRequest.ToEmails.Select(MailboxAddress.Parse));

            emailMessage.Subject = emailRequest.Subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = emailRequest.Body
            };

            foreach(var (fileName, file) in emailRequest.Attachments)
                bodyBuilder.Attachments.Add(fileName, file);

            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }
    }
}
