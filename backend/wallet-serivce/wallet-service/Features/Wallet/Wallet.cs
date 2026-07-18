namespace wallet_service.Features.Wallet;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Gold { get; set; }
    public int Silver { get; set; }
    public int Bronze { get; set; }
    public int Diamonds { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum CurrencyType
{
    Gold,
    Silver,
    Bronze,
    Diamonds
}