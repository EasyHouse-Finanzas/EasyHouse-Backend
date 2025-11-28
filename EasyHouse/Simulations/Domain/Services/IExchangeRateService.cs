namespace EasyHouse.Simulations.Domain.Services;

public interface IExchangeRateService
{
    Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    Task<decimal> ConvertAmountAsync(decimal amount, string baseCurrency, string targetCurrency);
}