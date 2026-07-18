using wallet_service.Core.DTOs;


namespace wallet_service.Features.Wallet;
using wallet_service.Features.Transaction;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _repo;

    public WalletService(IWalletRepository repo) => _repo = repo;

    public async Task<WalletDto> GetOrCreateWalletAsync(Guid userId)
    {
        var wallet = await _repo.GetByUserIdAsync(userId);
        if (wallet == null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UpdatedAt = DateTime.UtcNow
            };
            await _repo.CreateWalletAsync(wallet);
        }

        return MapToDto(wallet);
    }

    public async Task<WalletDto> GetWalletAsync(Guid userId)
    {
        var wallet = await _repo.GetByUserIdAsync(userId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");
        return MapToDto(wallet);
    }

    public async Task<WalletDto> AddCurrencyAsync(Guid userId, CurrencyType currencyType, int amount, string source)
    {
        var wallet = await _repo.GetByUserIdAsync(userId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");

        AddToWallet(wallet, currencyType, amount);
        wallet.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(wallet);

        await _repo.AddTransactionAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            CurrencyType = currencyType,
            Amount = amount,
            Type = "Earn",
            Source = source,
            CreatedAt = DateTime.UtcNow
        });

        return MapToDto(wallet);
    }

    public async Task<WalletDto> SpendCurrencyAsync(Guid userId, CurrencyType currencyType, int amount, string source)
    {
        var wallet = await _repo.GetByUserIdAsync(userId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");

        if (!HasEnough(wallet, currencyType, amount))
            throw new InvalidOperationException($"Not enough {currencyType}");

        AddToWallet(wallet, currencyType, -amount);
        wallet.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(wallet);

        await _repo.AddTransactionAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            CurrencyType = currencyType,
            Amount = -amount,
            Type = "Spend",
            Source = source,
            CreatedAt = DateTime.UtcNow
        });

        return MapToDto(wallet);
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var wallet = await _repo.GetByUserIdAsync(userId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");

        var transactions = await _repo.GetTransactionsAsync(wallet.Id, page, pageSize);
        return transactions.Select(t => new TransactionDto(
            t.Id, t.CurrencyType, t.Amount, t.Type, t.Source, t.Details, t.CreatedAt
        )).ToList();
    }

    private void AddToWallet(Wallet wallet, CurrencyType currencyType, int amount)
    {
        switch (currencyType)
        {
            case CurrencyType.Gold: wallet.Gold += amount; break;
            case CurrencyType.Silver: wallet.Silver += amount; break;
            case CurrencyType.Bronze: wallet.Bronze += amount; break;
            case CurrencyType.Diamonds: wallet.Diamonds += amount; break;
            default: throw new ArgumentException("Invalid currency type");
        }
    }

    private bool HasEnough(Wallet wallet, CurrencyType currencyType, int amount)
    {
        return currencyType switch
        {
            CurrencyType.Gold => wallet.Gold >= amount,
            CurrencyType.Silver => wallet.Silver >= amount,
            CurrencyType.Bronze => wallet.Bronze >= amount,
            CurrencyType.Diamonds => wallet.Diamonds >= amount,
            _ => false
        };
    }

    private static WalletDto MapToDto(Wallet w) => new(w.Id, w.UserId, w.Gold, w.Silver, w.Bronze, w.Diamonds);
}