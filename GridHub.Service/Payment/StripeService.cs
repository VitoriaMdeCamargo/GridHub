using Stripe;

namespace GridHub.Service.Payment
{
    public class StripeService
    {
        public async Task<PaymentIntent> CreatePaymentIntent(decimal amount, string currency = "usd")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),  
                Currency = currency,
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options); 
        }
    }
}
