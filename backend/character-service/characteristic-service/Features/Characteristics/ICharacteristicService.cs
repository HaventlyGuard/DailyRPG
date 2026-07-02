

using character_service.Core.DTOs;
using character_service.Features.Ranks;
using System.Security.Claims;
namespace character_service.Features.Characteristics;

public interface ICharacteristicService
{
    Task<CharacteristicDto> CreateAsync(Guid userId, CreateCharacteristicRequest request);
    Task<List<CharacteristicDto>> GetByUserAsync(Guid userId);
    Task<CharacteristicDto> GetByIdAsync(Guid id, Guid userId);
    Task<CharacteristicDto> UpdateAsync(Guid id, Guid userId, UpdateCharacteristicRequest request);
    Task DeleteAsync(Guid id, Guid userId);
    Task AddExperienceAsync(Guid characteristicId, Guid userId, int amount);
    Task<UserRankDto> GetUserRankAsync(Guid userId);
}