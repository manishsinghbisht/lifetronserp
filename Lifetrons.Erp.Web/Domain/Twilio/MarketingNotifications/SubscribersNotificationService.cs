using Lifetrons.Erp.Web.Domain.Twilio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Lifetrons.Erp.Web.Domain.Twilio.MarketingNotifications
{
    public interface ISubscribersNotificationService
    {
        Task<MessageResource> SendMessageAsync(string to, string body, List<Uri> mediaUrl);
        Task<MessageResource> SendMessageAsync(string to, string body);
    }

    public class SubscribersNotificationService : ISubscribersNotificationService
    {
        public SubscribersNotificationService()
        {
            if (SubscribersNotificationConfiguration.Credentials.AccountSID != null && SubscribersNotificationConfiguration.Credentials.AuthToken != null)
            {
                TwilioClient.Init(SubscribersNotificationConfiguration.Credentials.AccountSID, SubscribersNotificationConfiguration.Credentials.AuthToken);
            }
        }

        public async Task<MessageResource> SendMessageAsync(string to, string body, List<Uri> mediaUrl)
        {
            return await MessageResource.CreateAsync(
                from: new PhoneNumber(SubscribersNotificationConfiguration.PhoneNumbers.Twilio),
                to: new PhoneNumber(to),
                body: body,
                mediaUrl: mediaUrl);
        }

        public async Task<MessageResource> SendMessageAsync(string to, string body)
        {
            return await MessageResource.CreateAsync(
                from: new PhoneNumber(SubscribersNotificationConfiguration.PhoneNumbers.Twilio),
                to: new PhoneNumber(to),
                body: body);
        }

    }
}