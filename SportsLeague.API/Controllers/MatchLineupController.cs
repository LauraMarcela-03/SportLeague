using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(
            IMatchLineupService matchLineupService,
            IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        // ═══ Registrar jugador ═══

        [HttpPost("{matchId}/lineup")]
        public async Task<ActionResult<MatchLineupResponseDTO>> AddPlayer(
            int matchId,
            MatchLineupRequestDTO dto)
        {
            try
            {
                var lineup = _mapper.Map<MatchLineup>(dto);

                var created = await _matchLineupService
                    .AddPlayerToLineupAsync(matchId, lineup);

                var lineups = await _matchLineupService
                    .GetLineupByMatchAsync(matchId);

                var createdLineup = lineups
                    .FirstOrDefault(l => l.Id == created.Id);

                return Ok(
                    _mapper.Map<MatchLineupResponseDTO>(createdLineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // ═══ Obtener alineación de un partido ═══

        [HttpGet("{matchId}/lineup")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineup(
            int matchId)
        {
            try
            {
                var lineup = await _matchLineupService
                    .GetLineupByMatchAsync(matchId);

                return Ok(
                    _mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ═══ Obtener todas las alineaciones ═══

        [HttpGet("lineups")]
        public async Task<IActionResult> GetAll()
        {
            var lineups = await _matchLineupService.GetAllAsync();

            return Ok(
                _mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
        }

        // ═══ Eliminar jugador de alineación ═══

        [HttpDelete("{matchId}/lineup/{lineupId}")]
        public async Task<ActionResult> DeleteLineup(
            int matchId,
            int lineupId)
        {
            try
            {
                await _matchLineupService
                    .DeleteLineupAsync(lineupId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}