using Microsoft.EntityFrameworkCore;
using wallet_service.Core.DataAccess;
using wallet_service.Features.Transaction;
using wallet_service.Features.Wallet;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;
    public WalletRepository(AppDbContext context) => _context = context;

    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet?> GetByUserIdAsync(Guid userId)
        => await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

    public async Task<Wallet> UpdateAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetTransactionsAsync(Guid walletId, int page, int pageSize)
        => await _context.Transactions
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
}