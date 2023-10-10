using Lifetrons.Erp.Web.Models.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebGrease.Css.Extensions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Clients;
using Lifetrons.Erp.Web.Domain.Twilio;

namespace Lifetrons.Erp.Web.Domain.Twilio.Notifications
{
    public class Notifier
    {
        private Uri _imageUrl = new Uri("http://howtodocs.s3.amazonaws.com/new-relic-monitor.png");

        private readonly IAdministratorsRepository _administratorsRepository;
        private readonly ITwilioRestClient _restClient;

        public Notifier(IAdministratorsRepository repository)
        {
            _administratorsRepository = repository;
            _restClient = new TwilioRestClient(Credentials.TwilioAccountSid, Credentials.TwilioAuthToken);
        }

        public Notifier(IAdministratorsRepository repository, ITwilioRestClient client)
        {
            _administratorsRepository = repository;
            _restClient = client;
        }

        public async Task SendMessagesWithMediaAsync(string message)
        {           
            var mediaUrl = new List<Uri> { _imageUrl };

            //Read from data from excel file in App_Data folder
            _administratorsRepository.All().ForEach(async administrator =>
                await MessageResource.CreateAsync(
                    new PhoneNumber(administrator.PhoneNumber),
                    from: new PhoneNumber(Credentials.TwilioPhoneNumber),
                    body: message,
                    mediaUrl: mediaUrl,
                    client: _restClient));
        }

        public async Task<List<MessageResource>> SendMessagesAsync(string message)
        {
            List<MessageResource> messageResults = new List<MessageResource>();

            var excelList = _administratorsRepository.All();

            foreach (var item in excelList)
            {
               var messageResource = await MessageResource.CreateAsync(
                    new PhoneNumber(item.PhoneNumber),
                    from: new PhoneNumber(Credentials.TwilioPhoneNumber),
                    body: message,
                    client: _restClient);

                messageResults.Add(messageResource);
            }

            return messageResults;
        }

    }
}