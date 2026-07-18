using wallet_service.Features.Wallet;

namespace wallet_service.Core.DTOs;

public record WalletDto(
    Guid Id,
    Guid UserId,
    int Gold,
    int Silver,
    int Bronze,
    int Diamonds
);

public record TransactionDto(
    Guid Id,
    CurrencyType CurrencyType,
    int Amount,
    string Type,
    string Source,
    string? Details,
    DateTime CreatedAt
);

public record AddCurrencyRequest(string CurrencyType, int Amount, string Source);