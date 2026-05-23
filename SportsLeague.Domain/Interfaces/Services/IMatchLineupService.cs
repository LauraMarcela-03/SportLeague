using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services

{

    public interface IMatchLineupService

    {

        Task<MatchLineup> AddPlayerToLineupAsync(int matchId, MatchLineup lineup);

        Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId);

        Task<IEnumerable<MatchLineup>> GetAllAsync();

        Task DeleteLineupAsync(int lineupId);

    }

}
