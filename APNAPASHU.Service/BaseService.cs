using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using APNAPASHU.DataContract.Models;

namespace APNAPASHU.Service
{
    public abstract class BaseService
    {
        protected IConfiguration Configuration { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        public readonly R2Uploader _uploader;
        public readonly HttpClient _httpClient;

        public BaseService(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            HttpContextAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _httpClient = new HttpClient();

            _uploader = new R2Uploader(CDN_AccountId, CDN_AccessKey, CDN_SecretKey, CDN_BucketName);
        }

        public string CDN_AccountId =>
            Configuration["CloudfairStorageCDN:accountId"]
            ?? throw new Exception("AccountId missing in config");

        public string CDN_AccessKey =>
            Configuration["CloudfairStorageCDN:accessKey"]
            ?? throw new Exception("AccessKey missing in config");

        public string CDN_SecretKey =>
            Configuration["CloudfairStorageCDN:secretKey"]
            ?? throw new Exception("SecretKey missing in config");

        public string CDN_BucketName =>
            Configuration["CloudfairStorageCDN:bucketName"]
            ?? throw new Exception("BucketName missing in config");


        /// <summary>
        /// Send email functionality.
        /// </summary>
        public async Task<bool> SendEmailAsync(EmailModel model)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(Configuration["Email:From"] ?? string.Empty, "ApnaPashu"),
                    Subject = model.Subject,
                    Body = model.Body,
                    IsBodyHtml = true,
                };

                message.To.Add(model.To);

                using var client = new SmtpClient
                {
                    Host = Configuration["Email:Host"] ?? string.Empty,
                    Port = int.Parse(Configuration["Email:Port"] ?? "0"),
                    Credentials = new NetworkCredential(
                        Configuration["Email:Username"] ?? string.Empty,
                        Configuration["Email:Password"] ?? string.Empty
                    ),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Error: " + ex.Message);
                return false;
            }
        }
    }
}