using Lifetrons.Erp.Web.Domain.Twilio.MarketingNotifications;
using Lifetrons.Erp.Web.Models.Repository;
using System.Threading.Tasks;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace Lifetrons.Erp.Controllers
{
    public class SubscribersController : TwilioController
    {
        private readonly ISubscribersRepository _repository;

        public SubscribersController() : this(new SubscribersRepository()) { }

        public SubscribersController(ISubscribersRepository repository)
        {
            _repository = repository;
        }

        // POST: Subscribers/Register
        [HttpPost]
        public async Task<TwiMLResult> Register(string from, string body)
        {
            var phoneNumber = from;
            var message = body;

            var messageCreator = new SubscribersNotificationMessageCreator(_repository);
            var outputMessage = await messageCreator.Create(phoneNumber, message);

            var response = new MessagingResponse();
            response.Message(outputMessage);

            return TwiML(response);
        }
    }
}
