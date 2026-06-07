using System.Threading.Tasks;

namespace RealmOfLegends.Core.Services.Arena
{
    public interface IArenaService
    {
        Task<ArenaHubDto> GetArenaHubDataAsync(int playerId);
        Task<FightSessionDTO?> CreateFightSessionAsync(int playerId, int level);
        Task<TurnResultDTO> ProcessPlayerTurnAsync(string sessionId, int playerId, string actionType, int? itemId = null);
        Task<EnemyTurnResultDTO> ProcessEnemyTurnAsync(string sessionId, int playerId);
        Task<bool> AbandonSessionAsync(string sessionId, int playerId);
        Task<VictoryRewardDTO?> ProcessVictoryAsync(string sessionId);
        Task ProcessDefeatAsync(string sessionId);
    }
}
