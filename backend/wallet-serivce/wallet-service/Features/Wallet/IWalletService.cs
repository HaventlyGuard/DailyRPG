using wallet_service.Core.DTOs;

namespace wallet_service.Features.Wallet;

public interface IWalletService
{
    Task<WalletDto> GetOrCreateWalletAsync(Guid userId);
    Task<WalletDto> GetWalletAsync(Guid userId);
    Task<WalletDto> AddCurrencyAsync(Guid userId, CurrencyType currencyType, int amount, string source);
    Task<WalletDto> SpendCurrencyAsync(Guid userId, CurrencyType currencyType, int amount, string source);
    Task<List<TransactionDto>> GetTransactionsAsync(Guid userId, int page = 1, int pageSize = 20);
}