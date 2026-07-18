namespace wallet_service.Features.Wallet;
using wallet_service.Features.Transaction;

public interface IWalletRepository
{
    Task<Wallet> CreateWalletAsync(Wallet wallet);
    Task<Wallet?> GetByUserIdAsync(Guid userId);
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task AddTransactionAsync(Transaction transaction);
    Task<List<Transaction>> GetTransactionsAsync(Guid walletId, int page, int pageSize);
}