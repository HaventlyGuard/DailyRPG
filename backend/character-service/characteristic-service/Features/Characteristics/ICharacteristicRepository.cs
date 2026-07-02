namespace character_service.Features.Characteristics;

public interface ICharacteristicRepository
{
    Task<Characteristic> CreateAsync(Characteristic characteristic);
    Task<Characteristic?> GetByIdAsync(Guid id, Guid userId);
    Task<List<Characteristic>> GetByUserAsync(Guid userId);
    Task<Characteristic> UpdateAsync(Characteristic characteristic);
    Task DeleteAsync(Characteristic characteristic);
    Task<int> GetTotalLevelsAsync(Guid userId);
}