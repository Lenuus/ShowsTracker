using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using ShowsTracker.Application.Service.Email.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Email
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<MailSettingsOptions> _mailSettingsOptions;

        public EmailService(IOptions<MailSettingsOptions> mailSettingsOptions)
        {
            _mailSettingsOptions = mailSettingsOptions;
        }

        public async Task<ServiceResponse> Send(SendRequestDto request)
        {
            if (!request.To.Any())
            {
                return new ServiceResponse(false, "No mail receiver");
            }
            var email = _mailSettingsOptions.Value.Email;
            var password = _mailSettingsOptions.Value.Password;
            var address = _mailSettingsOptions.Value.Address;
            var port = _mailSettingsOptions.Value.Port;
            var displayName = _mailSettingsOptions.Value.DisplayName;

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(displayName, email));
                message.Subject = request.Subject;
                message.Body = new TextPart("html")
                {
                    Text = request.Body
                };
                foreach (var to in request.To)
                {
                    message.To.Add(new MailboxAddress(to, to));
                }

                using (var client = new SmtpClient())
                {
                    client.Connect(address, port, true);
                    client.Authenticate(email, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new ServiceResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, ex.Message);
            }
        }
    }
}
