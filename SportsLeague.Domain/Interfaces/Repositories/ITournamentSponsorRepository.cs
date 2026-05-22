using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository
    : IGenericRepository<TournamentSponsor>
{
    Task<IEnumerable<TournamentSponsor>>
        GetBySponsorIdAsync(int sponsorId);

    Task<bool> ExistsRelationAsync(
        int sponsorId,
        int tournamentId);

    Task<TournamentSponsor?> GetRelationAsync(
        int sponsorId,
        int tournamentId);
}