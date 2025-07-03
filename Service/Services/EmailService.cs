using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.Dto.Email;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Services
{
    public class EmailService
    {
        private readonly MailJetSetting _settings;

        public EmailService(IOptions<MailJetSetting> options)
        {
            _settings = options.Value;
            Console.WriteLine($"✅ MailJet Settings Loaded: {Newtonsoft.Json.JsonConvert.SerializeObject(_settings)}");
            Console.WriteLine(_settings);
            Console.WriteLine(_settings.ApiKey);
            Console.WriteLine(_settings.ApiSecret);
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            MailjetClient client = new MailjetClient(_settings.ApiKey, _settings.ApiSecret);
            var request = new MailjetRequest { Resource = SendV31.Resource }
                .Property(Send.Messages, new JArray {
                new JObject {
                    {"From", new JObject {
                        {"Email", "0264brachi@gmail.com"}, 
                        {"Name", "Social Network"}
                    }},
                    {"To", new JArray {
                        new JObject {
                            {"Email", toEmail},
                            {"Name", toName}
                        }
                    }},
                    {"Subject", subject},
                    {"TextPart", body},
                    {"HTMLPart", $"<h3>{body}</h3>"}
                }
                });

            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("✅ Email sent successfully!");
            }
            else
            {
                Console.WriteLine($"❌ Failed to send email. Status: {response.StatusCode}");
                Console.WriteLine(response.GetErrorMessage());
            }
        }
    }
}
