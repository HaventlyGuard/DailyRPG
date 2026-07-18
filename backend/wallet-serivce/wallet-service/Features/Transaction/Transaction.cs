using wallet_service.Features.Wallet;

namespace wallet_service.Features.Transaction;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public Wallet.Wallet Wallet { get; set; } = null!;
    public CurrencyType CurrencyType { get; set; } 
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty; 
    public string Source { get; set; } = string.Empty;  
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum TransactionType
{
    Earn,
    Spend,
    Bonus
}