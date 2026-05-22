using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
namespace SportsLeague.DataAccess.Repositories;

public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
{
    private readonly LeagueDbContext _context;
    public TournamentSponsorRepository(LeagueDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId)
    {
        return await _context.TournamentSponsors
            .Include(ts => ts.Tournament)
            .Include(ts => ts.Sponsor)
            .Where(ts => ts.SponsorId == sponsorId)
            .ToListAsync();
    }
    public async Task<bool> ExistsRelationAsync(int sponsorId, int tournamentId)
    {
        return await _context.TournamentSponsors
            .AnyAsync(ts => ts.SponsorId == sponsorId
                && ts.TournamentId == tournamentId);
    }
    public async Task<TournamentSponsor?> GetRelationAsync(int sponsorId, int tournamentId)
    {
        return await _context.TournamentSponsors
            .FirstOrDefaultAsync(ts => ts.SponsorId == sponsorId
                && ts.TournamentId == tournamentId);
    }
}
