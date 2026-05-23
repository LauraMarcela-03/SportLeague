using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services

{

    public class MatchLineupService : IMatchLineupService
    {

        private readonly IMatchRepository _matchRepository;

        private readonly IPlayerRepository _playerRepository;

        private readonly IMatchLineupRepository _matchLineupRepository;

        private readonly ILogger<MatchLineupService> _logger;


        public MatchLineupService(

            IMatchRepository matchRepository,

            IPlayerRepository playerRepository,

            IMatchLineupRepository matchLineupRepository,

            ILogger<MatchLineupService> logger)

        {

            _matchRepository = matchRepository;

            _playerRepository = playerRepository;

            _matchLineupRepository = matchLineupRepository;

            _logger = logger;

        }


        public async Task<MatchLineup> AddPlayerToLineupAsync(int matchId, MatchLineup lineup)

        {

            var match = await _matchRepository.GetByIdAsync(matchId);


            if (match == null)

                throw new KeyNotFoundException($"No existe el partido {matchId}");


            if (match.Status != MatchStatus.Scheduled &&
                match.Status != MatchStatus.InProgress)

            {

                throw new InvalidOperationException(

                    "Solo se pueden registrar alineaciones en partidos Scheduled o InProgress");

            }


            var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);


            if (player == null)

                throw new KeyNotFoundException(

                    $"No existe el jugador {lineup.PlayerId}");


            bool belongsToMatch =

                player.TeamId == match.HomeTeamId ||

                player.TeamId == match.AwayTeamId;


            if (!belongsToMatch)

            {

                throw new InvalidOperationException(

                    "El jugador no pertenece a ninguno de los equipos del partido");

            }


            var exists = await _matchLineupRepository

                .ExistsPlayerInMatchAsync(matchId, lineup.PlayerId);


            if (exists)

            {

                throw new InvalidOperationException(

                    "El jugador ya está registrado en la alineación");

            }


            lineup.MatchId = matchId;


            await _matchLineupRepository.CreateAsync(lineup);


            return lineup;

        }


        public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)

        {

            _logger.LogInformation("Obteniendo la alineación para el partido {MatchId}", matchId);

            return await _matchLineupRepository.GetByMatchAsync(matchId);

        }


        public async Task DeleteLineupAsync(int lineupId)

        {

            _logger.LogInformation("Eliminando el registro de alineación {LineupId}", lineupId);


            var lineup = await _matchLineupRepository.GetByIdAsync(lineupId);

            if (lineup == null)

                throw new KeyNotFoundException($"No existe el registro de alineación con ID {lineupId}");


            await _matchLineupRepository.DeleteAsync(lineupId);

        }


        public async Task<IEnumerable<MatchLineup>> GetAllAsync()

        {

            return await _matchLineupRepository.GetAllAsync();

        }

    }

}