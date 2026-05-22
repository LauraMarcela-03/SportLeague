using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;



namespace SportsLeague.Domain.Services;



public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly IGenericRepository<Tournament> _tournamentRepository;
    private readonly ILogger<SponsorService> _logger;



    public SponsorService(
    ISponsorRepository sponsorRepository,
    ITournamentSponsorRepository tournamentSponsorRepository,
    IGenericRepository<Tournament> tournamentRepository,
    ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _tournamentRepository = tournamentRepository;
        _logger = logger;
    }



    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");



        return await _sponsorRepository.GetAllAsync();
    }



    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation(
        "Retrieving sponsor with ID: {SponsorId}",
        id);



        var sponsor = await _sponsorRepository.GetByIdAsync(id);



        if (sponsor == null)
        {
            _logger.LogWarning(
            "Sponsor with ID {SponsorId} not found",
            id);
        }



        return sponsor;
    }



    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        var exists = await _sponsorRepository
        .ExistsByNameAsync(sponsor.Name);



        if (exists)
        {
            _logger.LogWarning(
            "Sponsor with name '{SponsorName}' already exists",
            sponsor.Name);



            throw new InvalidOperationException(
            $"Sponsor with name '{sponsor.Name}' already exists");
        }



        if (!new EmailAddressAttribute()
        .IsValid(sponsor.ContactEmail))
        {
            throw new InvalidOperationException(
            "Invalid email format");
        }



        sponsor.CreatedAt = DateTime.UtcNow;



        _logger.LogInformation(
        "Creating sponsor: {SponsorName}",
        sponsor.Name);



        return await _sponsorRepository.CreateAsync(sponsor);
    }



    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existingSponsor =
        await _sponsorRepository.GetByIdAsync(id);



        if (existingSponsor == null)
        {
            _logger.LogWarning(
            "Sponsor with ID {SponsorId} not found for update",
            id);



            throw new KeyNotFoundException(
            $"Sponsor with ID {id} not found");
        }



        if (existingSponsor.Name != sponsor.Name)
        {
            var exists = await _sponsorRepository
            .ExistsByNameAsync(sponsor.Name);



            if (exists)
            {
                throw new InvalidOperationException(
                $"Sponsor with name '{sponsor.Name}' already exists");
            }
        }



        if (!new EmailAddressAttribute()
        .IsValid(sponsor.ContactEmail))
        {
            throw new InvalidOperationException(
            "Invalid email format");
        }



        existingSponsor.Name = sponsor.Name;
        existingSponsor.ContactEmail = sponsor.ContactEmail;
        existingSponsor.Phone = sponsor.Phone;
        existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
        existingSponsor.Category = sponsor.Category;
        existingSponsor.UpdatedAt = DateTime.UtcNow;



        _logger.LogInformation(
        "Updating sponsor with ID: {SponsorId}",
        id);



        await _sponsorRepository.UpdateAsync(existingSponsor);
    }



    public async Task DeleteAsync(int id)
    {
        var exists = await _sponsorRepository.ExistsAsync(id);



        if (!exists)
        {
            _logger.LogWarning(
            "Sponsor with ID {SponsorId} not found for deletion",
            id);



            throw new KeyNotFoundException(
            $"Sponsor with ID {id} not found");
        }



        _logger.LogInformation(
        "Deleting sponsor with ID: {SponsorId}",
        id);



        await _sponsorRepository.DeleteAsync(id);
    }



    public async Task<IEnumerable<TournamentSponsor>>
    GetSponsorTournamentsAsync(int sponsorId)
    {
        var sponsor = await _sponsorRepository
        .GetByIdAsync(sponsorId);



        if (sponsor == null)
        {
            throw new KeyNotFoundException(
            $"Sponsor with ID {sponsorId} not found");
        }



        return await _tournamentSponsorRepository
        .GetBySponsorIdAsync(sponsorId);
    }



    public async Task<TournamentSponsor> LinkTournamentAsync(
    int sponsorId,
    int tournamentId,
    decimal contractAmount)
    {
        var sponsor = await _sponsorRepository
        .GetByIdAsync(sponsorId);



        if (sponsor == null)
        {
            throw new KeyNotFoundException(
            $"Sponsor with ID {sponsorId} not found");
        }



        var tournament = await _tournamentRepository
        .GetByIdAsync(tournamentId);



        if (tournament == null)
        {
            throw new KeyNotFoundException(
            $"Tournament with ID {tournamentId} not found");
        }



        var relationExists =
        await _tournamentSponsorRepository
        .ExistsRelationAsync(
        sponsorId,
        tournamentId);



        if (relationExists)
        {
            throw new InvalidOperationException(
            "Sponsor is already linked to this tournament");
        }



        if (contractAmount <= 0)
        {
            throw new InvalidOperationException(
            "ContractAmount must be greater than 0");
        }



        var relation = new TournamentSponsor
        {
            SponsorId = sponsorId,
            TournamentId = tournamentId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow
        };



        _logger.LogInformation(
        "Linking sponsor {SponsorId} to tournament {TournamentId}",
        sponsorId,
        tournamentId);



        return await _tournamentSponsorRepository
        .CreateAsync(relation);
    }



    public async Task UnlinkTournamentAsync(
    int sponsorId,
    int tournamentId)
    {
        var relation =
        await _tournamentSponsorRepository
        .GetRelationAsync(
        sponsorId,
        tournamentId);



        if (relation == null)
        {
            throw new KeyNotFoundException(
            "Relation not found");
        }



        _logger.LogInformation(
        "Unlinking sponsor {SponsorId} from tournament {TournamentId}",
        sponsorId,
        tournamentId);



        await _tournamentSponsorRepository
        .DeleteAsync(relation.Id);
    }
}