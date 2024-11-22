using GridHub.Service.Payment;
using Stripe;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using GridHub.API.Configuration;

public class StripeServiceTests
{
    private readonly StripeService _stripeService;

    public StripeServiceTests()
    {
        // Configuração do Stripe
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        services.Configure<APPConfiguration>(configuration);

        // Carregar a chave da API do Stripe
        StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];

        // Injeção de dependência do StripeService
        services.AddScoped<StripeService>();

        var serviceProvider = services.BuildServiceProvider();
        _stripeService = serviceProvider.GetRequiredService<StripeService>();
    }

    [Fact]
    public async Task CreatePaymentIntent_ShouldReturnPaymentIntent_WhenAmountIsValid()
    {
        // Defina um valor de teste
        decimal amount = 50.00m;  // 50 USD

        // Crie o PaymentIntent
        var paymentIntent = await _stripeService.CreatePaymentIntent(amount);

        // Verifique se o PaymentIntent foi criado com sucesso
        Assert.NotNull(paymentIntent);
        Assert.Equal(5000, paymentIntent.Amount); // O valor deve ser 5000 centavos (50.00 USD)
        Assert.Equal("usd", paymentIntent.Currency);
    }
}
